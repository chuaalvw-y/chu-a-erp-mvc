// Copyright (c) 2026 Alvin Wilsen Chan Chua
// GitHub: chuaalvw-y
// Licensed under the Alvin Wilsen Chan Chua Proprietary Use-Only License.
// See LICENSE.txt in the project root for full license information.

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChuA.ERP.Web.Mvc.Controllers;

/// <summary>
/// Same-origin token endpoint consumed by client-side SignalR <c>accessTokenFactory</c>
/// callbacks. JS code negotiating against a hub on the API side (different origin) cannot
/// read the OIDC HttpOnly cookie directly; it has to call this endpoint to retrieve the
/// cookie-saved access token, then attach it to the SignalR negotiate request.
///
/// <para>
/// Returns an empty token (rather than 401) when no access token is on the principal so JS
/// can negotiate anonymously and let the hub itself decide whether to reject. This matches
/// the Dashboard.Mvc pattern (see <c>DashboardController.SignalRToken</c>) so the two hubs
/// behave identically from a client perspective.
/// </para>
///
/// <para>
/// Never exposes refresh tokens, id tokens, or any other credential. The access token is
/// already in the user's possession via the browser session — surfacing it for the JS-
/// driven hub connection does not lower the trust boundary.
/// </para>
/// </summary>
[Authorize]
[Route("[controller]")]
public sealed class SignalRController : Controller
{
    /// <summary>
    /// Returns <c>{ "token": "..." }</c> with the cookie-saved OIDC access token, or
    /// <c>{ "token": "" }</c> when none is available (e.g. DevLogin path with no real IdP).
    /// </summary>
    [HttpGet("Token")]
    public async Task<IActionResult> Token()
    {
        var token = await HttpContext.GetTokenAsync("access_token").ConfigureAwait(false);
        return Json(new { token = token ?? string.Empty });
    }
}
