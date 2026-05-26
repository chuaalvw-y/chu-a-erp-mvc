// Copyright (c) 2026 Alvin Wilsen Chan Chua
// GitHub: chuaalvw-y
// Licensed under the Alvin Wilsen Chan Chua Proprietary Use-Only License.
// See LICENSE.txt in the project root for full license information.

namespace ChuA.ERP.Web.Mvc.Models.Auth;

/// <summary>
/// One row in the diagnostic claims table. A flat, view-only projection of a
/// <see cref="System.Security.Claims.Claim"/> so the Razor view stays free of System.* types.
/// </summary>
public sealed class AuthClaimViewModel
{
    /// <summary>The claim type (e.g. <c>sub</c>, <c>role</c>, <c>https://schemas.../emailaddress</c>).</summary>
    public string Type { get; init; } = string.Empty;

    /// <summary>The claim value.</summary>
    public string Value { get; init; } = string.Empty;

    /// <summary>The issuer reported by the authentication handler (e.g. the Auth0 tenant URL).</summary>
    public string Issuer { get; init; } = string.Empty;

    /// <summary>The XML/JWT value-type identifier (e.g. <c>http://www.w3.org/2001/XMLSchema#string</c>).</summary>
    public string ValueType { get; init; } = string.Empty;
}
