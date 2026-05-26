// Copyright (c) 2026 Alvin Wilsen Chan Chua
// GitHub: chuaalvw-y
// Licensed under the Alvin Wilsen Chan Chua Proprietary Use-Only License.
// See LICENSE.txt in the project root for full license information.

using System.Security.Claims;
using System.Text.Json;
using ChuA.ERP.Web.Mvc.Models.Auth;

namespace ChuA.ERP.Web.Mvc.Services;

/// <summary>
/// Default <see cref="IAuthDebugSnapshotBuilder"/>. Pure function over a
/// <see cref="ClaimsPrincipal"/>; no I/O, no DI dependencies beyond the input — trivially
/// unit-testable.
/// </summary>
public sealed class AuthDebugSnapshotBuilder : IAuthDebugSnapshotBuilder
{
    /// <summary>
    /// Claim types commonly used to carry role values. Auth0 typically uses a namespaced
    /// custom claim (e.g. <c>https://chua-erp.com/roles</c>) added via an Action; we still
    /// match the unprefixed forms because that is what shows up after the OIDC handler's
    /// <c>RoleClaimType</c> mapping has run.
    /// </summary>
    private static readonly string[] RoleClaimTypes =
    [
        ClaimTypes.Role,
        "role",
        "roles",
    ];

    /// <summary>Claim types commonly used to carry application permission values.</summary>
    private static readonly string[] PermissionClaimTypes =
    [
        "permissions",
        "permission",
    ];

    /// <summary>Claim types commonly used to carry OAuth scope values.</summary>
    private static readonly string[] ScopeClaimTypes =
    [
        "scope",
        "scp",
    ];

    /// <inheritdoc />
    public AuthDebugViewModel Build(ClaimsPrincipal principal)
    {
        ArgumentNullException.ThrowIfNull(principal);

        var identity = principal.Identity;
        var isAuthenticated = identity?.IsAuthenticated == true;

        var claims = principal.Claims
            .Select(claim => new AuthClaimViewModel
            {
                Type = claim.Type,
                Value = claim.Value,
                Issuer = claim.Issuer,
                ValueType = claim.ValueType,
            })
            .OrderBy(claim => claim.Type, StringComparer.OrdinalIgnoreCase)
            .ThenBy(claim => claim.Value, StringComparer.OrdinalIgnoreCase)
            .ToArray();

        var (roles, roleClaimsHit) = ExtractMultiValue(principal, RoleClaimTypes, splitOnWhitespace: false);
        var (permissions, permissionClaimsHit) = ExtractMultiValue(principal, PermissionClaimTypes, splitOnWhitespace: false);
        var (scopes, scopeClaimsHit) = ExtractMultiValue(principal, ScopeClaimTypes, splitOnWhitespace: true);

        return new AuthDebugViewModel
        {
            IsAuthenticated = isAuthenticated,
            AuthenticationType = identity?.AuthenticationType,
            Name = identity?.Name,
            UserId = FindFirst(principal, ClaimTypes.NameIdentifier, "sub", "oid", "nameidentifier"),
            Email = FindFirst(principal, ClaimTypes.Email, "email", "upn", "preferred_username"),
            Roles = roles,
            Permissions = permissions,
            Scopes = scopes,
            Claims = claims,
            DiscoveredRoleClaimTypes = roleClaimsHit,
            DiscoveredPermissionClaimTypes = permissionClaimsHit,
            DiscoveredScopeClaimTypes = scopeClaimsHit,
        };
    }

    /// <summary>
    /// Returns the deduplicated, normalized values across every claim whose type matches one of
    /// <paramref name="candidateClaimTypes"/>. Handles three on-the-wire shapes:
    /// <list type="bullet">
    /// <item>A scalar string (the common case).</item>
    /// <item>A JSON array string — e.g. <c>["Admin","Approver"]</c> — emitted by some IdPs when
    /// the underlying JWT claim is an array.</item>
    /// <item>A space-delimited list — only when <paramref name="splitOnWhitespace"/> is true
    /// (the OAuth 2.0 <c>scope</c> convention).</item>
    /// </list>
    /// Returns both the merged values and the actual claim types we saw — useful for the view
    /// to show "Auth0 sent your roles under <c>https://chua-erp.com/roles</c>" when none of the
    /// expected types matched.
    /// </summary>
    private static (IReadOnlyList<string> Values, IReadOnlyList<string> HitClaimTypes) ExtractMultiValue(
        ClaimsPrincipal principal,
        IEnumerable<string> candidateClaimTypes,
        bool splitOnWhitespace)
    {
        var hits = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var values = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var candidate in candidateClaimTypes)
        {
            foreach (var claim in principal.FindAll(candidate))
            {
                hits.Add(claim.Type);
                foreach (var value in ExpandClaimValue(claim.Value, splitOnWhitespace))
                {
                    values.Add(value);
                }
            }
        }

        return (
            values.OrderBy(v => v, StringComparer.OrdinalIgnoreCase).ToArray(),
            hits.OrderBy(h => h, StringComparer.OrdinalIgnoreCase).ToArray());
    }

    /// <summary>
    /// Expands a single claim string into one-or-more discrete values, handling JSON-array
    /// payloads and (optionally) whitespace splitting.
    /// </summary>
    private static IEnumerable<string> ExpandClaimValue(string rawValue, bool splitOnWhitespace)
    {
        if (string.IsNullOrWhiteSpace(rawValue))
        {
            yield break;
        }

        var trimmed = rawValue.Trim();
        if (trimmed.StartsWith('[') && trimmed.EndsWith(']'))
        {
            string[]? parsed = null;
            try
            {
                parsed = JsonSerializer.Deserialize<string[]>(trimmed);
            }
            catch (JsonException)
            {
                // Fall through to the scalar path — the value isn't a JSON array after all.
            }

            if (parsed is not null)
            {
                foreach (var element in parsed)
                {
                    if (!string.IsNullOrWhiteSpace(element))
                    {
                        yield return element.Trim();
                    }
                }
                yield break;
            }
        }

        if (splitOnWhitespace)
        {
            foreach (var part in trimmed.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            {
                yield return part;
            }
            yield break;
        }

        yield return trimmed;
    }

    /// <summary>
    /// Returns the first non-empty claim value across the supplied candidate types, in priority
    /// order. Avoids the more expensive multi-value extraction when callers only want a single
    /// representative value.
    /// </summary>
    private static string? FindFirst(ClaimsPrincipal principal, params string[] candidateClaimTypes)
    {
        foreach (var candidate in candidateClaimTypes)
        {
            var value = principal.FindFirst(candidate)?.Value;
            if (!string.IsNullOrWhiteSpace(value))
            {
                return value;
            }
        }
        return null;
    }
}
