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

    public IReadOnlyCollection<string> Roles =>
        User?.FindAll(ClaimTypes.Role).Select(c => c.Value).Distinct().ToArray()
        ?? Array.Empty<string>();

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
