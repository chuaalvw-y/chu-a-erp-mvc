using ChuA.ERP.Web.Mvc.Contracts.Dtos;

namespace ChuA.ERP.Web.Mvc.Services;

/// <summary>
/// Resolves information about the currently signed-in user (display name, roles, permissions).
/// Backed by claims plus the API's <c>/users/me</c> projection.
/// </summary>
public interface ICurrentUserService
{
    /// <summary>True when the inbound request carries an authenticated principal.</summary>
    bool IsAuthenticated { get; }

    /// <summary>The user id (sub claim).</summary>
    string? UserId { get; }

    /// <summary>Display name pulled from the principal.</summary>
    string DisplayName { get; }

    /// <summary>The company id active for this session, if any.</summary>
    Guid? CompanyId { get; }

    /// <summary>The set of role names assigned to the user.</summary>
    IReadOnlyCollection<string> Roles { get; }

    /// <summary>The set of permissions/policies granted to the user.</summary>
    IReadOnlyCollection<string> Permissions { get; }

    /// <summary>Returns true if the user has any of the given roles.</summary>
    bool IsInAnyRole(params string[] roles);

    /// <summary>Returns true if the user has any of the given permissions.</summary>
    bool HasAnyPermission(params string[] permissions);

    /// <summary>Cached projection from <c>/users/me</c>. May be null until populated.</summary>
    CurrentUserDto? Profile { get; }

    /// <summary>Fetches /users/me and caches the result on the current request.</summary>
    Task<CurrentUserDto?> LoadProfileAsync(CancellationToken cancellationToken = default);
}
