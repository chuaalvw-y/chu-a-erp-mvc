// Copyright (c) 2026 Alvin Wilsen Chan Chua
// GitHub: chuaalvw-y
// Licensed under the Alvin Wilsen Chan Chua Proprietary Use-Only License.
// See LICENSE.txt in the project root for full license information.

using System.Security.Claims;
using ChuA.ERP.Web.Mvc.ApiClients;
using ChuA.ERP.Web.Mvc.Contracts.Dtos;
using Microsoft.AspNetCore.Http;

namespace ChuA.ERP.Web.Mvc.Services;

/// <summary>
/// Default <see cref="ICurrentUserService"/> implementation. Reads claims from
/// <see cref="HttpContext.User"/> and lazily hydrates the profile from <c>/users/me</c>
/// using the <see cref="IUsersApiClient"/>.
/// </summary>
public sealed class CurrentUserService : ICurrentUserService
{
    private const string ProfileItemsKey = "ChuA.ERP.CurrentUserProfile";
    private const string PermissionClaimType = "permission";

    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUsersApiClient _usersApiClient;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor, IUsersApiClient usersApiClient)
    {
        _httpContextAccessor = httpContextAccessor;
        _usersApiClient = usersApiClient;
    }

    private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

    public bool IsAuthenticated => User?.Identity?.IsAuthenticated == true;

    public string? UserId =>
        User?.FindFirst("sub")?.Value
        ?? User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
        ?? User?.Identity?.Name;

    public string DisplayName =>
        User?.FindFirst("preferred_username")?.Value
        ?? User?.FindFirst("name")?.Value
        ?? User?.Identity?.Name
        ?? "Guest";

    public Guid? CompanyId
    {
        get
        {
            var raw = User?.FindFirst("company_id")?.Value;
            return Guid.TryParse(raw, out var id) ? id : null;
        }
    }

    public IReadOnlyCollection<string> Roles
    {
        get
        {
            // Phase J — the OIDC handler is configured with MapInboundClaims=false
            // and RoleClaimType="role" (see OidcWebAppAuthenticationProviderConfigurator),
            // so Auth0 role claims arrive at type "role" — NOT the ClaimTypes.Role URI.
            // Reading by the URI literal silently misses every role and is the
            // root cause of the "Admin nav disappears under Auth0" symptom. Read
            // via the identity's configured RoleClaimType so cookie auth (legacy
            // URI), JWT bearer ("role"), and the bypass handler all agree.
            var roleClaimType = RoleClaimType(User);
            return User?.FindAll(roleClaimType).Select(c => c.Value).Distinct(StringComparer.Ordinal).ToArray()
                   ?? Array.Empty<string>();
        }
    }

    private static string RoleClaimType(ClaimsPrincipal? user) =>
        (user?.Identity as ClaimsIdentity)?.RoleClaimType is { Length: > 0 } configured
            ? configured
            : ClaimTypes.Role;

    public IReadOnlyCollection<string> Permissions
    {
        get
        {
            var fromClaims = User?.FindAll(PermissionClaimType).Select(c => c.Value) ?? Enumerable.Empty<string>();
            var fromProfile = Profile?.Permissions ?? Array.Empty<string>();
            return fromClaims.Concat(fromProfile).Distinct(StringComparer.Ordinal).ToArray();
        }
    }

    public bool IsInAnyRole(params string[] roles)
    {
        if (roles is null || roles.Length == 0) return false;
        var owned = Roles;
        foreach (var r in roles)
        {
            if (owned.Contains(r, StringComparer.Ordinal)) return true;
        }
        return false;
    }

    public bool HasAnyPermission(params string[] permissions)
    {
        if (permissions is null || permissions.Length == 0) return false;
        var owned = Permissions;
        foreach (var p in permissions)
        {
            if (owned.Contains(p, StringComparer.Ordinal)) return true;
        }
        return false;
    }

    public CurrentUserDto? Profile
    {
        get
        {
            var ctx = _httpContextAccessor.HttpContext;
            if (ctx is null) return null;
            return ctx.Items.TryGetValue(ProfileItemsKey, out var stash) ? stash as CurrentUserDto : null;
        }
    }

    public async Task<CurrentUserDto?> LoadProfileAsync(CancellationToken cancellationToken = default)
    {
        var ctx = _httpContextAccessor.HttpContext;
        if (ctx is null || !IsAuthenticated) return null;

        if (Profile is { } cached) return cached;

        var result = await _usersApiClient.GetMeAsync(cancellationToken).ConfigureAwait(false);
        if (result.IsSuccess)
        {
            ctx.Items[ProfileItemsKey] = result.Value;
            return result.Value;
        }
        return null;
    }
}
