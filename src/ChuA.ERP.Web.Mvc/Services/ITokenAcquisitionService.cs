// Copyright (c) 2026 Alvin Wilsen Chan Chua
// GitHub: chuaalvw-y
// Licensed under the Alvin Wilsen Chan Chua Proprietary Use-Only License.
// See LICENSE.txt in the project root for full license information.

namespace ChuA.ERP.Web.Mvc.Services;

/// <summary>
/// Acquires a bearer token to attach to outbound API calls. Implementations may pull tokens
/// from the OIDC session, exchange refresh tokens, or return a fixed development token.
/// </summary>
public interface ITokenAcquisitionService
{
    /// <summary>Returns a bearer access token, or null if the call should be unauthenticated.</summary>
    Task<string?> GetAccessTokenAsync(CancellationToken cancellationToken = default);
}
