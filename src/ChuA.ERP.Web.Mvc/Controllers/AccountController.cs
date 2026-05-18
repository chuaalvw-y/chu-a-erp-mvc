using System.Security.Claims;
using ChuA.ERP.Web.Mvc.Configuration;
using ChuA.ERP.Web.Mvc.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace ChuA.ERP.Web.Mvc.Controllers;

/// <summary>
/// Handles sign-in, sign-out, profile, and access-denied flows. When OIDC is enabled the
/// login/logout actions delegate to the OIDC handler; otherwise they issue/clear a local
/// cookie containing a development principal (so the rest of the UI is browsable in dev).
/// </summary>
public sealed class AccountController : Controller
{
    private readonly IOptions<OidcOptions> _oidcOptions;
    private readonly ICurrentUserService _currentUser;

    public AccountController(IOptions<OidcOptions> oidcOptions, ICurrentUserService currentUser)
    {
        _oidcOptions = oidcOptions;
        _currentUser = currentUser;
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login(string? returnUrl = null)
    {
        returnUrl ??= "/";
        if (_oidcOptions.Value.Enabled)
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
        return LocalRedirect(string.IsNullOrEmpty(returnUrl) ? "/" : returnUrl);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme).ConfigureAwait(false);
        if (_oidcOptions.Value.Enabled)
        {
            return SignOut(new AuthenticationProperties { RedirectUri = "/" }, OpenIdConnectDefaults.AuthenticationScheme);
        }
        return RedirectToAction(nameof(Login));
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
