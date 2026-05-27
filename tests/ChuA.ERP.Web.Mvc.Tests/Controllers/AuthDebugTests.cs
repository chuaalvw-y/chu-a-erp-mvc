// Copyright (c) 2026 Alvin Wilsen Chan Chua
// GitHub: chuaalvw-y
// Licensed under the Alvin Wilsen Chan Chua Proprietary Use-Only License.
// See LICENSE.txt in the project root for full license information.

using System.Security.Claims;
using ChuA.ERP.Web.Mvc.Models.Auth;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging.Abstractions;

namespace ChuA.ERP.Web.Mvc.Tests.Controllers;

/// <summary>
/// Tests for <see cref="AuthDebugController"/> and the underlying
/// <see cref="AuthDebugSnapshotBuilder"/>. The controller is a thin dev-only gate; the
/// builder is a pure function and gets the bulk of the coverage.
/// </summary>
public class AuthDebugTests
{
    private static AuthDebugSnapshotBuilder Builder() => new();

    private static ClaimsPrincipal PrincipalOf(string authenticationType, params Claim[] claims)
        => new(new ClaimsIdentity(claims, authenticationType));

    // ----- AuthDebugSnapshotBuilder -----

    [Fact]
    public void Build_should_normalize_roles_across_common_claim_types()
    {
        var principal = PrincipalOf(
            "Cookies",
            new Claim(ClaimTypes.Role, "SystemAdmin"),
            new Claim("role", "CompanyAdmin"),
            new Claim("roles", "Approver"));

        var model = Builder().Build(principal);

        model.Roles.Should().BeEquivalentTo(["Approver", "CompanyAdmin", "SystemAdmin"], options => options.WithStrictOrdering());
        model.DiscoveredRoleClaimTypes.Should().Contain([ClaimTypes.Role, "role", "roles"]);
    }

    [Fact]
    public void Build_should_dedupe_role_values_case_insensitively()
    {
        var principal = PrincipalOf(
            "Cookies",
            new Claim(ClaimTypes.Role, "SystemAdmin"),
            new Claim("role", "systemadmin"));

        var model = Builder().Build(principal);

        model.Roles.Should().HaveCount(1);
        model.Roles[0].Should().BeEquivalentTo("SystemAdmin");
    }

    [Fact]
    public void Build_should_parse_json_array_claim_values()
    {
        // Auth0 sometimes emits the underlying JWT array claim as a JSON-array string.
        var principal = PrincipalOf(
            "Cookies",
            new Claim("permissions", "[\"orders:read\",\"orders:write\"]"));

        var model = Builder().Build(principal);

        model.Permissions.Should().BeEquivalentTo(["orders:read", "orders:write"]);
    }

    [Fact]
    public void Build_should_split_scopes_on_whitespace()
    {
        // OAuth 2.0 scope claim is a single space-delimited string.
        var principal = PrincipalOf(
            "Cookies",
            new Claim("scope", "openid profile email orders:read"));

        var model = Builder().Build(principal);

        model.Scopes.Should().BeEquivalentTo(["email", "openid", "orders:read", "profile"], options => options.WithStrictOrdering());
    }

    [Fact]
    public void Build_should_sort_claims_alphabetically_by_type()
    {
        var principal = PrincipalOf(
            "Cookies",
            new Claim("zzz", "last"),
            new Claim("aaa", "first"),
            new Claim("mmm", "middle"));

        var model = Builder().Build(principal);

        model.Claims.Select(c => c.Type).Should().ContainInOrder("aaa", "mmm", "zzz");
    }

    [Fact]
    public void Build_should_extract_user_id_and_email_from_oidc_friendly_claims()
    {
        var principal = PrincipalOf(
            "Cookies",
            new Claim("sub", "auth0|abc123"),
            new Claim("email", "alice@example.test"));

        var model = Builder().Build(principal);

        model.UserId.Should().Be("auth0|abc123");
        model.Email.Should().Be("alice@example.test");
    }

    [Fact]
    public void Build_should_report_no_discovered_role_types_when_idp_uses_namespaced_claim()
    {
        // Simulates Auth0 emitting roles under a namespaced custom claim — the diagnostic
        // page surfaces this as "Roles from: none matched" so the engineer realises why
        // [Authorize(Roles=...)] checks are failing.
        var principal = PrincipalOf(
            "Cookies",
            new Claim("https://chua-erp.com/roles", "SystemAdmin"));

        var model = Builder().Build(principal);

        model.Roles.Should().BeEmpty();
        model.DiscoveredRoleClaimTypes.Should().BeEmpty();
        model.Claims.Should().ContainSingle(c => c.Type == "https://chua-erp.com/roles" && c.Value == "SystemAdmin");
    }

    [Fact]
    public void Build_should_record_authentication_state_for_anonymous_principal()
    {
        var anonymous = new ClaimsPrincipal(new ClaimsIdentity());

        var model = Builder().Build(anonymous);

        model.IsAuthenticated.Should().BeFalse();
        model.AuthenticationType.Should().BeNull();
        model.Claims.Should().BeEmpty();
    }

    // ----- AuthDebugController gate -----

    [Fact]
    public void Index_should_return_NotFound_outside_Development()
    {
        var controller = BuildController(Environments.Production);

        var result = controller.Index();

        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public void Claims_should_return_NotFound_outside_Development()
    {
        var controller = BuildController(Environments.Production);

        var result = controller.Claims();

        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public void Index_should_return_View_in_Development()
    {
        var controller = BuildController(Environments.Development);

        var result = controller.Index();

        result.Should().BeOfType<ViewResult>()
            .Which.Model.Should().BeOfType<AuthDebugViewModel>();
    }

    [Fact]
    public void Claims_should_return_Json_in_Development()
    {
        var controller = BuildController(Environments.Development);

        var result = controller.Claims();

        result.Should().BeOfType<JsonResult>()
            .Which.Value.Should().BeOfType<AuthDebugViewModel>();
    }

    private static AuthDebugController BuildController(string environmentName)
    {
        var environment = new StubHostEnvironment(environmentName);
        var builder = new AuthDebugSnapshotBuilder();
        var controller = new AuthDebugController(environment, builder, NullLogger<AuthDebugController>.Instance);
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(
                    [new Claim(ClaimTypes.Name, "test.user")],
                    "Cookies")),
            },
        };
        return controller;
    }

    private sealed class StubHostEnvironment(string environmentName) : IWebHostEnvironment
    {
        public string EnvironmentName { get; set; } = environmentName;
        public string ApplicationName { get; set; } = "ChuA.ERP.Web.Mvc.Tests";
        public string WebRootPath { get; set; } = string.Empty;
        public IFileProvider WebRootFileProvider { get; set; } = new NullFileProvider();
        public string ContentRootPath { get; set; } = AppContext.BaseDirectory;
        public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
    }
}
