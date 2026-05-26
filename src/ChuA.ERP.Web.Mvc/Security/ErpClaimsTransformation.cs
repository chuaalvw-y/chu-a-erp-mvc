// Copyright (c) 2026 Alvin Wilsen Chan Chua
// GitHub: chuaalvw-y
// Licensed under the Alvin Wilsen Chan Chua Proprietary Use-Only License.
// See LICENSE.txt in the project root for full license information.

using System.Security.Claims;
using ChuA.ERP.Web.Mvc.ApiClients;
using ChuA.ERP.Web.Mvc.Contracts.Dtos;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace ChuA.ERP.Web.Mvc.Security;

/// <summary>
/// Phase J — the MVC's "Auth0 authenticates, ERP DB authorises" bridge.
///
/// <para>Runs on every authenticated request, after the Auth0 OIDC cookie has
/// been validated. Walks the principal's <c>sub</c> claim, calls the ERP API's
/// <c>/api/v1/users/me</c> to resolve the full authorisation profile from
/// <c>[identity].*</c> tables, and stamps <c>role</c> + <c>permission</c> +
/// <c>company_id</c> + <c>companies</c> claims onto a new <c>ClaimsIdentity</c>.</para>
///
/// <para>Mandatory short-circuit: re-entry within the same request — and
/// re-entry within the same user's 30-second cache window — must not
/// re-call the API. We mark each transformed principal with a sentinel
/// claim (<see cref="SentinelClaimType"/>) and additionally cache the
/// hydrated principal in <c>HttpContext.Items</c> (per-request) plus
/// <see cref="IMemoryCache"/> (per-user, 30s TTL) — see the Phase J spec
/// §9 caching strategy.</para>
///
/// <para>Fail-closed contract: if the API returns 404 (no matching
/// ERP user row) or any other failure, we return the principal
/// <em>unchanged</em>. Downstream <c>[Authorize(Policy = ...)]</c> denies
/// and <see cref="Controllers.AccountController.AccessDenied"/> renders
/// the "Access pending" page.</para>
/// </summary>
public sealed class ErpClaimsTransformation : IClaimsTransformation
{
    public const string SentinelClaimType = "erp_profile_loaded";
    public const string SentinelClaimValue = "1";
    private const string HttpContextItemKey = "ChuA.ErpClaimsTransformation.Principal";
    private static readonly TimeSpan CacheTtl = TimeSpan.FromSeconds(30);

    private readonly IUsersApiClient _users;
    private readonly IMemoryCache _cache;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<ErpClaimsTransformation> _logger;

    public ErpClaimsTransformation(
        IUsersApiClient users,
        IMemoryCache cache,
        IHttpContextAccessor httpContextAccessor,
        ILogger<ErpClaimsTransformation> logger)
    {
        _users = users;
        _cache = cache;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        // 1) Already transformed in this request? Return as-is.
        var ctx = _httpContextAccessor.HttpContext;
        if (ctx is not null &&
            ctx.Items.TryGetValue(HttpContextItemKey, out var stash) &&
            stash is ClaimsPrincipal transformed)
        {
            return transformed;
        }

        // 2) Already carries the sentinel claim? Return as-is. Belt-and-braces
        //    against the framework calling Transform multiple times on the
        //    same identity (e.g. policy evaluation after the initial pass).
        if (principal.HasClaim(SentinelClaimType, SentinelClaimValue))
        {
            return principal;
        }

        // 3) Anonymous request? Nothing to transform.
        if (principal.Identity?.IsAuthenticated != true)
        {
            return principal;
        }

        // 4) Need a `sub` claim to know who we are.
        var sub = principal.FindFirst("sub")?.Value
                  ?? principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrWhiteSpace(sub))
        {
            _logger.LogDebug("ErpClaimsTransformation: principal has no sub claim. Skipping.");
            return principal;
        }

        // 5) Fetch (or load + cache) the profile from the API.
        var cacheKey = $"erp:auth-profile:{sub}";
        if (!_cache.TryGetValue<CurrentUserDto>(cacheKey, out var profile) || profile is null)
        {
            var result = await _users.GetMeAsync().ConfigureAwait(false);
            if (!result.IsSuccess || result.Value is null)
            {
                _logger.LogWarning(
                    "ErpClaimsTransformation: /users/me returned no profile for sub={Subject}. " +
                    "Failing closed — downstream [Authorize] will deny.", sub);

                // Stash the bare principal so we don't call /me again this request.
                Stash(ctx, principal);
                return principal;
            }

            profile = result.Value;
            _cache.Set(cacheKey, profile, CacheTtl);
        }

        // 6) Build a new identity carrying the ERP-resolved claims. We clone
        //    the existing identity so cookie/OIDC claims (sub, name, email,
        //    picture) survive — then layer the ERP claims on top, using the
        //    identity's configured RoleClaimType so role-aware APIs
        //    (User.IsInRole, [Authorize(Roles=...)], RequireRole(...)) all
        //    agree on a single source.
        var existingIdentity = principal.Identity as ClaimsIdentity ?? new ClaimsIdentity();
        var roleClaimType = string.IsNullOrEmpty(existingIdentity.RoleClaimType)
            ? ClaimTypes.Role
            : existingIdentity.RoleClaimType;

        var hydrated = new ClaimsIdentity(existingIdentity);

        // Sentinel — added first so re-entry short-circuits cleanly.
        hydrated.AddClaim(new Claim(SentinelClaimType, SentinelClaimValue));

        // Phase J explicit "resolved ERP user" marker. The API's /me endpoint
        // reads this claim (not sub) so an authenticated-but-unresolved
        // principal cannot reach the profile data — see UsersController.
        // GetCurrentUserAsync. Symmetric with BypassAuthenticationHandler.
        if (!hydrated.HasClaim(c => c.Type == "erp_user_id"))
        {
            hydrated.AddClaim(new Claim("erp_user_id", profile.UserId.ToString()));
        }

        // Active company + memberships (de-duplicate against any claims
        // already on the principal — the bypass handler may have stamped
        // a companies claim too).
        if (profile.ActiveCompanyId is Guid active &&
            !hydrated.HasClaim(c => c.Type == "company_id"))
        {
            hydrated.AddClaim(new Claim("company_id", active.ToString()));
        }

        if (profile.Companies is { Count: > 0 } memberships &&
            !hydrated.HasClaim(c => c.Type == "companies"))
        {
            var joined = string.Join(',', memberships.Select(m => m.CompanyId.ToString()));
            hydrated.AddClaim(new Claim("companies", joined));
        }

        // Roles — use the configured RoleClaimType. Skip duplicates so
        // re-issued cookies don't double up.
        var existingRoles = hydrated.FindAll(roleClaimType).Select(c => c.Value).ToHashSet(StringComparer.Ordinal);
        foreach (var role in profile.Roles ?? Array.Empty<string>())
        {
            if (existingRoles.Add(role))
            {
                hydrated.AddClaim(new Claim(roleClaimType, role));
            }
        }

        // Permissions — one claim per value, matches PermissionAuthorizationHandler's
        // expected shape on the API side and any future server-side policy
        // evaluation in MVC.
        var existingPerms = hydrated.FindAll("permission").Select(c => c.Value).ToHashSet(StringComparer.Ordinal);
        foreach (var perm in profile.Permissions ?? Array.Empty<string>())
        {
            if (existingPerms.Add(perm))
            {
                hydrated.AddClaim(new Claim("permission", perm));
            }
        }

        var newPrincipal = new ClaimsPrincipal(hydrated);
        Stash(ctx, newPrincipal);
        return newPrincipal;
    }

    private static void Stash(HttpContext? ctx, ClaimsPrincipal principal)
    {
        if (ctx is not null)
        {
            ctx.Items[HttpContextItemKey] = principal;
        }
    }
}
