// Copyright (c) 2026 Alvin Wilsen Chan Chua
// GitHub: chuaalvw-y
// Licensed under the Alvin Wilsen Chan Chua Proprietary Use-Only License.
// See LICENSE.txt in the project root for full license information.

namespace ChuA.ERP.Web.Mvc.Models.Auth;

/// <summary>
/// View model for the Development-only Auth diagnostic page. Surfaces everything an
/// engineer needs to figure out why a role/permission isn't propagating from Auth0
/// (or any other OIDC IdP) into the user's principal.
/// </summary>
public sealed class AuthDebugViewModel
{
    /// <summary>True when the inbound request carries an authenticated principal.</summary>
    public bool IsAuthenticated { get; init; }

    /// <summary>The authentication scheme that minted the principal (e.g. <c>Cookies</c>).</summary>
    public string? AuthenticationType { get; init; }

    /// <summary>Display name from the principal's <c>Identity.Name</c>.</summary>
    public string? Name { get; init; }

    /// <summary>Stable user identifier (sub / nameidentifier).</summary>
    public string? UserId { get; init; }

    /// <summary>Email claim, when present.</summary>
    public string? Email { get; init; }

    /// <summary>Roles extracted from the principal's role-flavoured claim types, normalized and deduped.</summary>
    public IReadOnlyList<string> Roles { get; init; } = Array.Empty<string>();

    /// <summary>Permissions extracted from <c>permission(s)</c>-flavoured claim types, normalized and deduped.</summary>
    public IReadOnlyList<string> Permissions { get; init; } = Array.Empty<string>();

    /// <summary>Scopes extracted from <c>scope</c>/<c>scp</c> claims (space-split per OAuth 2.0), normalized and deduped.</summary>
    public IReadOnlyList<string> Scopes { get; init; } = Array.Empty<string>();

    /// <summary>Every claim on the principal, sorted alphabetically by claim type.</summary>
    public IReadOnlyList<AuthClaimViewModel> Claims { get; init; } = Array.Empty<AuthClaimViewModel>();

    /// <summary>The discovered claim types that yielded the roles list (so the view can show which names Auth0 actually sent).</summary>
    public IReadOnlyList<string> DiscoveredRoleClaimTypes { get; init; } = Array.Empty<string>();

    /// <summary>The discovered claim types that yielded the permissions list.</summary>
    public IReadOnlyList<string> DiscoveredPermissionClaimTypes { get; init; } = Array.Empty<string>();

    /// <summary>The discovered claim types that yielded the scopes list.</summary>
    public IReadOnlyList<string> DiscoveredScopeClaimTypes { get; init; } = Array.Empty<string>();
}
