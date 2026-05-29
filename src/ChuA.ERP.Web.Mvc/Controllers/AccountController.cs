// Copyright (c) 2026 Alvin Wilsen Chan Chua
// GitHub: chuaalvw-y
// Licensed under the Alvin Wilsen Chan Chua Proprietary Use-Only License.
// See LICENSE.txt in the project root for full license information.

using System.Security.Claims;
using ChuA.ERP.Web.Mvc.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChuA.ERP.Web.Mvc.Controllers;

/// <summary>
/// Handles sign-in, sign-out, profile, and access-denied flows. When the OIDC scheme is
/// registered (ChuAAuthentication:Providers.&lt;name&gt;.ClientId is set), the login/logout
/// actions delegate to the OIDC handler; otherwise they issue/clear a local cookie
/// containing a development principal (so the rest of the UI is browsable in dev).
/// </summary>
public sealed class AccountController : Controller
{
    private readonly IAuthenticationSchemeProvider _schemeProvider;
    private readonly ICurrentUserService _currentUser;

    public AccountController(IAuthenticationSchemeProvider schemeProvider, ICurrentUserService currentUser)
    {
        _schemeProvider = schemeProvider;
        _currentUser = currentUser;
    }

    private async Task<bool> IsOidcConfiguredAsync()
        => await _schemeProvider
            .GetSchemeAsync(OpenIdConnectDefaults.AuthenticationScheme)
            .ConfigureAwait(false) is not null;

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Login(string? returnUrl = null)
    {
        returnUrl ??= "/";
        if (await IsOidcConfiguredAsync().ConfigureAwait(false))
        {
            var props = new AuthenticationProperties { RedirectUri = returnUrl };
            return Challenge(props, OpenIdConnectDefaults.AuthenticationScheme);
        }

        // Local dev shortcut: render the placeholder login view that posts to DevLogin.
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DevLogin(string? userName, string? returnUrl = null)
    {
        var name = string.IsNullOrWhiteSpace(userName) ? "dev.user" : userName.Trim();
        var claims = new List<Claim>
        {
            new("sub", Guid.NewGuid().ToString()),
            new(ClaimTypes.Name, name),
            new("preferred_username", name),
            new(ClaimTypes.Role, "SystemAdmin"),
            new(ClaimTypes.Role, "CompanyAdmin"),
        };
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity)).ConfigureAwait(false);

        // Only honour a return URL that is genuinely local (open-redirect guard).
        // Use RedirectToAction for the fallback so we never call LocalRedirect with
        // a value it would reject — that throws a 500 even though sign-in succeeded.
        if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }

        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        // Decide whether to federate the sign-out to the IdP BEFORE we sign
        // the cookie out — the AuthenticationProperties (which carry the
        // OIDC id_token stored by SaveTokens=true) disappear with the
        // cookie.
        //
        // The previous gate checked only whether the OIDC scheme was
        // REGISTERED (i.e. Auth0 ClientId is set in user-secrets) — so a
        // session signed in via DevLogin (cookie-only, used when the dev
        // bypass is active) still federated to Auth0, and Auth0's
        // /v2/logout errored on the unknown id_token_hint. The session-
        // level check below skips the federated call for any cookie-only
        // sign-in, while preserving the real-Auth0 sign-out flow for
        // sessions that were actually OIDC-established.
        var hasFederatedSession = await HasOidcSessionAsync().ConfigureAwait(false);

        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme).ConfigureAwait(false);

        if (hasFederatedSession && await IsOidcConfiguredAsync().ConfigureAwait(false))
        {
            return SignOut(new AuthenticationProperties { RedirectUri = "/" }, OpenIdConnectDefaults.AuthenticationScheme);
        }
        return RedirectToAction(nameof(Login));
    }

    /// <summary>
    /// Returns <c>true</c> when the current cookie session was issued via
    /// the OIDC handler — detected by the presence of a stored
    /// <c>id_token</c> in the authentication properties (the
    /// <c>SaveTokens=true</c> artefact of the OIDC sign-in). Returns
    /// <c>false</c> for DevLogin / bypass sessions, which sign the cookie
    /// directly without going through OIDC.
    /// </summary>
    private async Task<bool> HasOidcSessionAsync()
    {
        var auth = await HttpContext
            .AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme)
            .ConfigureAwait(false);
        return auth.Succeeded
            && auth.Properties?.GetTokenValue("id_token") is not null;
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Profile(CancellationToken cancellationToken)
    {
        var profile = await _currentUser.LoadProfileAsync(cancellationToken).ConfigureAwait(false);
        return View(profile);
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult AccessDenied() => View();
}
