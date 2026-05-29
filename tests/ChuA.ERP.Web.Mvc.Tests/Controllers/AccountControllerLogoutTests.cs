// Copyright (c) 2026 Alvin Wilsen Chan Chua
// GitHub: chuaalvw-y
// Licensed under the Alvin Wilsen Chan Chua Proprietary Use-Only License.
// See LICENSE.txt in the project root for full license information.

using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace ChuA.ERP.Web.Mvc.Tests.Controllers;

/// <summary>
/// Regression coverage for the Phase J+ logout flow:
/// <see cref="AccountController.Logout"/> must only federate the sign-out to
/// the IdP when the CURRENT cookie session was actually established via
/// OIDC. A DevLogin / bypass session has no Auth0 session to terminate, so
/// federating it makes Auth0's <c>/v2/logout</c> error on an unknown
/// <c>id_token_hint</c> — and locks the user out of a clean dev sign-in.
///
/// <para>The signal we read is the presence of a stored <c>id_token</c> in
/// the cookie's <see cref="AuthenticationProperties"/> (the
/// <c>SaveTokens=true</c> artefact of the OIDC sign-in). It survives until
/// the cookie itself is cleared — so the controller captures it BEFORE
/// calling SignOutAsync.</para>
/// </summary>
public sealed class AccountControllerLogoutTests
{
    private const string CookieScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    private const string OidcScheme = OpenIdConnectDefaults.AuthenticationScheme;

    private readonly Mock<IAuthenticationService> _authService = new();
    private readonly Mock<IAuthenticationSchemeProvider> _schemeProvider = new();

    private AccountController BuildSut()
    {
        var services = new ServiceCollection();
        services.AddSingleton(_authService.Object);
        services.AddLogging();
        var http = new DefaultHttpContext { RequestServices = services.BuildServiceProvider() };

        var ctrl = new AccountController(_schemeProvider.Object, Mock.Of<ICurrentUserService>())
        {
            ControllerContext = new ControllerContext { HttpContext = http }
        };
        // RedirectToAction(string) lazily resolves IUrlHelperFactory from
        // RequestServices to build / validate the URL — stubbing Url is
        // cheaper than wiring the full helper.
        var url = new Mock<IUrlHelper>();
        url.Setup(u => u.Action(It.IsAny<UrlActionContext>())).Returns("/stub");
        ctrl.Url = url.Object;
        return ctrl;
    }

    private void SetupCookieAuth(AuthenticateResult result)
    {
        _authService
            .Setup(s => s.AuthenticateAsync(It.IsAny<HttpContext>(), CookieScheme))
            .ReturnsAsync(result);
        _authService
            .Setup(s => s.SignOutAsync(It.IsAny<HttpContext>(), CookieScheme, It.IsAny<AuthenticationProperties?>()))
            .Returns(Task.CompletedTask);
    }

    private void OidcSchemeRegistered(bool registered)
    {
        AuthenticationScheme? scheme = registered
            ? new AuthenticationScheme(OidcScheme, displayName: null, handlerType: typeof(DummyHandler))
            : null;
        _schemeProvider
            .Setup(p => p.GetSchemeAsync(OidcScheme))
            .ReturnsAsync(scheme);
    }

    private static AuthenticateResult OidcSessionResult()
    {
        var props = new AuthenticationProperties();
        props.StoreTokens(new[]
        {
            new AuthenticationToken { Name = "id_token", Value = "stubbed.jwt.value" },
            new AuthenticationToken { Name = "access_token", Value = "stubbed.access" },
        });
        var identity = new ClaimsIdentity(new[]
        {
            new Claim("sub", "auth0|68976ba75f159fca7eb7e55b"),
            new Claim("name", "alvin"),
        }, CookieScheme);
        var ticket = new AuthenticationTicket(new ClaimsPrincipal(identity), props, CookieScheme);
        return AuthenticateResult.Success(ticket);
    }

    private static AuthenticateResult DevLoginSessionResult()
    {
        // Mirrors AccountController.DevLogin: cookie sign-in with random
        // sub + name + roles, NO stored tokens in AuthenticationProperties.
        var identity = new ClaimsIdentity(new[]
        {
            new Claim("sub", Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Name, "dev.user"),
            new Claim(ClaimTypes.Role, "SystemAdmin"),
        }, CookieScheme);
        var ticket = new AuthenticationTicket(
            new ClaimsPrincipal(identity),
            new AuthenticationProperties(),
            CookieScheme);
        return AuthenticateResult.Success(ticket);
    }

    [Fact]
    public async Task Logout_with_OIDC_session_and_OIDC_registered_federates_to_IdP()
    {
        // The unchanged real-Auth0 sign-out flow: stored id_token + OIDC
        // scheme registered ⇒ return SignOutResult so the framework hands
        // off to the OIDC handler (which builds the federated /v2/logout
        // URL with id_token_hint=...).
        SetupCookieAuth(OidcSessionResult());
        OidcSchemeRegistered(true);
        var ctrl = BuildSut();

        var result = await ctrl.Logout();

        var signOut = result.Should().BeOfType<SignOutResult>().Subject;
        signOut.AuthenticationSchemes.Should().BeEquivalentTo(new[] { OidcScheme });
        signOut.Properties!.RedirectUri.Should().Be("/");
        _authService.Verify(
            s => s.SignOutAsync(It.IsAny<HttpContext>(), CookieScheme, It.IsAny<AuthenticationProperties?>()),
            Times.Once);
    }

    [Fact]
    public async Task Logout_with_DevLogin_session_skips_federated_signout_even_when_OIDC_registered()
    {
        // The Phase J+ fix. A cookie-only session (DevLogin / bypass) has
        // no id_token in its AuthenticationProperties — the controller
        // must NOT federate to Auth0 even though the OIDC scheme is
        // registered (which it always is in dev because user-secrets
        // populates ClientId). Lands the user at /Account/Login with a
        // clean cookie clear.
        SetupCookieAuth(DevLoginSessionResult());
        OidcSchemeRegistered(true);
        var ctrl = BuildSut();

        var result = await ctrl.Logout();

        var redirect = result.Should().BeOfType<RedirectToActionResult>().Subject;
        redirect.ActionName.Should().Be(nameof(AccountController.Login));
        _authService.Verify(
            s => s.SignOutAsync(It.IsAny<HttpContext>(), CookieScheme, It.IsAny<AuthenticationProperties?>()),
            Times.Once);
    }

    [Fact]
    public async Task Logout_with_no_session_redirects_to_Login_without_federating()
    {
        // AuthenticateAsync returns NoResult — covers both "user was never
        // signed in" and "cookie expired before they hit logout".
        SetupCookieAuth(AuthenticateResult.NoResult());
        OidcSchemeRegistered(true);
        var ctrl = BuildSut();

        var result = await ctrl.Logout();

        result.Should().BeOfType<RedirectToActionResult>()
            .Subject.ActionName.Should().Be(nameof(AccountController.Login));
    }

    [Fact]
    public async Task Logout_with_OIDC_session_but_OIDC_not_registered_redirects_to_Login()
    {
        // Defensive: if the OIDC scheme was deregistered between sign-in
        // and sign-out (e.g. operator blanked ClientId mid-session) the
        // controller must still clean the cookie and surface a working
        // sign-in path — not crash trying to challenge an unregistered
        // scheme.
        SetupCookieAuth(OidcSessionResult());
        OidcSchemeRegistered(false);
        var ctrl = BuildSut();

        var result = await ctrl.Logout();

        result.Should().BeOfType<RedirectToActionResult>()
            .Subject.ActionName.Should().Be(nameof(AccountController.Login));
    }

    private sealed class DummyHandler : IAuthenticationHandler
    {
        public Task<AuthenticateResult> AuthenticateAsync() => throw new NotSupportedException();
        public Task ChallengeAsync(AuthenticationProperties? properties) => throw new NotSupportedException();
        public Task ForbidAsync(AuthenticationProperties? properties) => throw new NotSupportedException();
        public Task InitializeAsync(AuthenticationScheme scheme, HttpContext context) => Task.CompletedTask;
    }
}
