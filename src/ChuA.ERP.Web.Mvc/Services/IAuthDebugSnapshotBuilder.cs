// Copyright (c) 2026 Alvin Wilsen Chan Chua
// GitHub: chuaalvw-y
// Licensed under the Alvin Wilsen Chan Chua Proprietary Use-Only License.
// See LICENSE.txt in the project root for full license information.

using System.Security.Claims;
using ChuA.ERP.Web.Mvc.Models.Auth;

namespace ChuA.ERP.Web.Mvc.Services;

/// <summary>
/// Projects a <see cref="ClaimsPrincipal"/> into a flat <see cref="AuthDebugViewModel"/> used by
/// the Development-only Auth diagnostic page. Encapsulates the rules for extracting and
/// normalizing roles, permissions and scopes from a heterogeneous set of claim types.
/// </summary>
public interface IAuthDebugSnapshotBuilder
{
    /// <summary>Builds a complete snapshot for the supplied principal.</summary>
    /// <param name="principal">The principal to inspect. Typically <c>HttpContext.User</c>.</param>
    AuthDebugViewModel Build(ClaimsPrincipal principal);
}
