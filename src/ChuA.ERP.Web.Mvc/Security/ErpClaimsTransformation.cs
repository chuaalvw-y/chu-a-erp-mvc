// Copyright (c) 2026 Alvin Wilsen Chan Chua
// GitHub: chuaalvw-y
// Licensed under the Alvin Wilsen Chan Chua Proprietary Use-Only License.
// See LICENSE.txt in the project root for full license information.

using System.Security.Claims;
using ChuA.Authentication.Claims;
using ChuA.ERP.Web.Mvc.ApiClients;
using ChuA.ERP.Web.Mvc.Contracts.Dtos;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace ChuA.ERP.Web.Mvc.Security;

/// <summary>
/// Phase J — the MVC's "Auth0 authenticates, ERP DB authorises" bridge.
///
/// <para>Implemented as an <see cref="IClaimsEnricher"/>: ChuA.Authentication's
/// single registered <c>ChuAClaimsTransformation</c> runs its own claim mapping,
/// invokes our <see cref="EnrichAsync"/>, and handles the per-request re-entry
/// guard via <c>HttpContext.Items</c>. We only need to call the ERP API and
/// stamp the hydrated claims.</para>
///
/// <para>Fail-closed contract: if <c>/users/me</c> returns 404 (no matching ERP
/// user row) or any other failure, the principal is returned unchanged. Every
/// downstream <c>[Authorize(Policy = ...)]</c> denies and the AccessDenied page
/// surfaces the missing provisioning.</para>
/// </summary>
public sealed class ErpClaimsTransformation : IClaimsEnricher
{
    private static readonly TimeSpan CacheTtl = TimeSpan.FromSeconds(30);

    private readonly IUsersApiClient _users;
    private readonly IMemoryCache _cache;
    private readonly ILogger<ErpClaimsTransformation> _logger;

    public ErpClaimsTransformation(
        IUsersApiClient users,
        IMemoryCache cache,
        ILogger<ErpClaimsTransformation> logger)
    {
        _users = users;
        _cache = cache;
        _logger = logger;
    }

    public async Task<ClaimsPrincipal> EnrichAsync(ClaimsPrincipal principal)
    {
        if (principal.Identity?.IsAuthenticated != true)
        {
            return principal;
        }

        var sub = principal.FindFirst("sub")?.Value
                  ?? principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrWhiteSpace(sub))
        {
            _logger.LogDebug("ErpClaimsTransformation: principal has no sub claim. Skipping.");
            return principal;
        }

        var cacheKey = $"erp:auth-profile:{sub}";
        if (!_cache.TryGetValue<CurrentUserDto>(cacheKey, out var profile) || profile is null)
        {
            var result = await _users.GetMeAsync().ConfigureAwait(false);
            if (!result.IsSuccess || result.Value is null)
            {
                _logger.LogWarning(
                    "ErpClaimsTransformation: /users/me returned no profile for sub={Subject}. " +
                    "Failing closed — downstream [Authorize] will deny.", sub);
                return principal;
            }

            profile = result.Value;
            _cache.Set(cacheKey, profile, CacheTtl);
        }

        // Build a new identity carrying the ERP-resolved claims. Clone the
        // existing identity so cookie/OIDC claims (sub, name, email, picture)
        // survive, then layer the ERP claims on top using the identity's
        // configured RoleClaimType so role-aware APIs (User.IsInRole,
        // [Authorize(Roles=...)], RequireRole(...)) all agree on a single source.
        var existingIdentity = principal.Identity as ClaimsIdentity ?? new ClaimsIdentity();
        var roleClaimType = string.IsNullOrEmpty(existingIdentity.RoleClaimType)
            ? ClaimTypes.Role
            : existingIdentity.RoleClaimType;

        var hydrated = new ClaimsIdentity(existingIdentity);

        // Phase J explicit "resolved ERP user" marker. The API's /me endpoint
        // reads this claim (not sub) so an authenticated-but-unresolved
        // principal cannot reach the profile data — symmetric with the API's
        // BypassAuthenticationHandler and ErpAuthClaimsEnricher.
        if (!hydrated.HasClaim(c => c.Type == "erp_user_id"))
        {
            hydrated.AddClaim(new Claim("erp_user_id", profile.UserId.ToString()));
        }

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

        var existingRoles = hydrated.FindAll(roleClaimType).Select(c => c.Value).ToHashSet(StringComparer.Ordinal);
        foreach (var role in profile.Roles ?? Array.Empty<string>())
        {
            if (existingRoles.Add(role))
            {
                hydrated.AddClaim(new Claim(roleClaimType, role));
            }
        }

        var existingPerms = hydrated.FindAll("permission").Select(c => c.Value).ToHashSet(StringComparer.Ordinal);
        foreach (var perm in profile.Permissions ?? Array.Empty<string>())
        {
            if (existingPerms.Add(perm))
            {
                hydrated.AddClaim(new Claim("permission", perm));
            }
        }

        return new ClaimsPrincipal(hydrated);
    }
}
