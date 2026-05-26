// Copyright (c) 2026 Alvin Wilsen Chan Chua
// GitHub: chuaalvw-y
// Licensed under the Alvin Wilsen Chan Chua Proprietary Use-Only License.
// See LICENSE.txt in the project root for full license information.

using ChuA.ERP.Web.Mvc.Models.Auth;
using ChuA.ERP.Web.Mvc.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ChuA.ERP.Web.Mvc.Controllers;

/// <summary>
/// Development-only diagnostic endpoint that renders the authenticated user's claims, roles,
/// permissions and scopes. Used to troubleshoot why Auth0 (or any OIDC IdP) roles aren't
/// reaching the application — typically because the IdP emits them under a non-standard claim
/// name and the framework's <c>RoleClaimType</c> mapping hasn't been pointed at it.
///
/// Hard-blocked outside <c>IHostEnvironment.IsDevelopment()</c>: every action returns
/// <see cref="NotFoundResult"/> in any non-Development host, so the route is invisible in
/// staging/production even if a stray nav link slips through.
///
/// Never exposes raw access tokens, refresh tokens, ID tokens, the OIDC client secret, or
/// cookie values — only the principal's claims (which the user already implicitly possesses).
/// </summary>
[Authorize]
[Route("[controller]")]
public sealed class AuthDebugController : Controller
{
    private readonly IWebHostEnvironment _environment;
    private readonly IAuthDebugSnapshotBuilder _snapshotBuilder;
    private readonly ILogger<AuthDebugController> _logger;

    /// <summary>Constructs the controller with its injected dependencies.</summary>
    public AuthDebugController(
        IWebHostEnvironment environment,
        IAuthDebugSnapshotBuilder snapshotBuilder,
        ILogger<AuthDebugController> logger)
    {
        _environment = environment;
        _snapshotBuilder = snapshotBuilder;
        _logger = logger;
    }

    /// <summary>
    /// Renders the diagnostic page at <c>GET /AuthDebug</c>.
    /// Returns 404 in any non-Development environment.
    /// </summary>
    [HttpGet("")]
    [HttpGet("Index")]
    public IActionResult Index()
    {
        if (!_environment.IsDevelopment())
        {
            return NotFound();
        }

        var model = _snapshotBuilder.Build(User);
        LogSnapshot(model);
        return View(model);
    }

    /// <summary>
    /// Returns the same snapshot as <see cref="Index"/> but as <c>application/json</c> at
    /// <c>GET /AuthDebug/Claims</c> — useful for copy-paste into a bug report or for ad-hoc
    /// scripting. Returns 404 in any non-Development environment.
    /// </summary>
    [HttpGet("Claims")]
    public IActionResult Claims()
    {
        if (!_environment.IsDevelopment())
        {
            return NotFound();
        }

        var model = _snapshotBuilder.Build(User);
        LogSnapshot(model);
        return Json(model);
    }

    /// <summary>
    /// Structured Serilog (via <see cref="ILogger"/>) entry every time the page is hit, so a
    /// dev can audit accidental usage even when running under <c>Development</c>.
    /// </summary>
    private void LogSnapshot(AuthDebugViewModel model)
    {
        _logger.LogInformation(
            "AuthDebug loaded for {User} (authenticated={IsAuthenticated}, scheme={Scheme}) with {ClaimCount} claims, {RoleCount} roles, {PermissionCount} permissions, {ScopeCount} scopes; roleClaimTypes={RoleClaimTypes}; permissionClaimTypes={PermissionClaimTypes}; scopeClaimTypes={ScopeClaimTypes}",
            model.Name ?? model.UserId ?? "<anonymous>",
            model.IsAuthenticated,
            model.AuthenticationType ?? "<none>",
            model.Claims.Count,
            model.Roles.Count,
            model.Permissions.Count,
            model.Scopes.Count,
            model.DiscoveredRoleClaimTypes,
            model.DiscoveredPermissionClaimTypes,
            model.DiscoveredScopeClaimTypes);
    }
}
