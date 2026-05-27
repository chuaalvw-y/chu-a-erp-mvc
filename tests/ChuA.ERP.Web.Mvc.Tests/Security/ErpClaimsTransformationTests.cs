// Copyright (c) 2026 Alvin Wilsen Chan Chua
// GitHub: chuaalvw-y
// Licensed under the Alvin Wilsen Chan Chua Proprietary Use-Only License.
// See LICENSE.txt in the project root for full license information.

using System.Security.Claims;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;

namespace ChuA.ERP.Web.Mvc.Tests.Security;

/// <summary>
/// Unit tests for the Phase J <c>ErpClaimsTransformation</c> — the MVC's
/// "Auth0 authenticates, ERP DB authorises" bridge. Implemented as an
/// <see cref="ChuA.Authentication.Claims.IClaimsEnricher"/>: the library's
/// <c>ChuAClaimsTransformation</c> handles claim mapping and per-request
/// re-entry guarding, so these tests only cover the enricher's own contract:
///
/// <list type="bullet">
///   <item>Role claims are stamped using the identity's <c>RoleClaimType</c>
///         (NOT the <c>ClaimTypes.Role</c> URI) — closes the "Admin nav
///         disappears under Auth0" regression.</item>
///   <item>Anonymous and missing-sub principals are returned unchanged
///         with no API hit.</item>
///   <item>API failures fail closed: no role/permission/erp_user_id claims
///         on the returned principal.</item>
///   <item>An <c>erp_user_id</c> claim is stamped on success so the API
///         <c>/me</c> endpoint can distinguish "authenticated and resolved"
///         from "authenticated but unresolved".</item>
///   <item>Cache hits avoid a second API round-trip across calls.</item>
/// </list>
/// </summary>
public sealed class ErpClaimsTransformationTests
{
    private readonly Mock<IUsersApiClient> _users = new(MockBehavior.Strict);
    private readonly IMemoryCache _cache = new MemoryCache(new MemoryCacheOptions());

    private ErpClaimsTransformation BuildSut() => new(
        _users.Object,
        _cache,
        NullLogger<ErpClaimsTransformation>.Instance);

    private static ClaimsPrincipal AuthenticatedPrincipal(
        string sub = "auth0|abc123",
        string? roleClaimType = "role")
    {
        var claims = new List<Claim>
        {
            new("sub", sub),
            new(ClaimTypes.Name, "test-user"),
        };
        var identity = new ClaimsIdentity(
            claims,
            authenticationType: "TestCookie",
            nameType: ClaimTypes.Name,
            roleType: roleClaimType ?? ClaimTypes.Role);
        return new ClaimsPrincipal(identity);
    }

    private static CurrentUserDto Profile(
        Guid? userId = null,
        IEnumerable<string>? roles = null,
        IEnumerable<string>? permissions = null,
        Guid? activeCompanyId = null,
        IEnumerable<MembershipDto>? companies = null)
    {
        return new CurrentUserDto(
            UserId: userId ?? Guid.NewGuid(),
            Email: "u@chua.test",
            DisplayName: "U",
            ActiveCompanyId: activeCompanyId ?? Guid.NewGuid(),
            Companies: (companies ?? new[]
            {
                new MembershipDto(activeCompanyId ?? Guid.NewGuid(), IsDefault: true),
            }).ToList(),
            Roles: (roles ?? new[] { "SystemAdmin", "CompanyAdmin" }).ToList(),
            Permissions: (permissions ?? new[] { "vendor:view", "vendor:create" }).ToList());
    }

    [Fact]
    public async Task Adds_role_claims_using_the_identity_RoleClaimType()
    {
        // OIDC handler is configured with RoleClaimType="role"; the enricher
        // must emit role claims at that type, not the legacy ClaimTypes.Role URI.
        var profile = Profile(roles: new[] { "SystemAdmin", "FinanceManager" });
        _users.Setup(u => u.GetMeAsync(It.IsAny<CancellationToken>()))
              .ReturnsAsync(Result<CurrentUserDto>.Success(profile));

        var principal = AuthenticatedPrincipal(roleClaimType: "role");
        var enriched = await BuildSut().EnrichAsync(principal);

        var emittedRoles = enriched.FindAll("role").Select(c => c.Value).ToArray();
        emittedRoles.Should().BeEquivalentTo("SystemAdmin", "FinanceManager");

        // The literal URI bucket should be EMPTY — otherwise we've re-introduced
        // the bug Phase J was meant to close.
        enriched.FindAll(ClaimTypes.Role).Should().BeEmpty();
    }

    [Fact]
    public async Task Adds_one_permission_claim_per_value()
    {
        var profile = Profile(permissions: new[] { "vendor:view", "vendor:create", "invoice:approve" });
        _users.Setup(u => u.GetMeAsync(It.IsAny<CancellationToken>()))
              .ReturnsAsync(Result<CurrentUserDto>.Success(profile));

        var enriched = await BuildSut().EnrichAsync(AuthenticatedPrincipal());

        var emitted = enriched.FindAll("permission").Select(c => c.Value).ToArray();
        emitted.Should().BeEquivalentTo("vendor:view", "vendor:create", "invoice:approve");
    }

    [Fact]
    public async Task Adds_erp_user_id_claim_after_resolution()
    {
        // The API /me endpoint reads erp_user_id (not sub) to enforce
        // fail-closed when the resolver couldn't link the IdP subject to
        // an ERP user — the enricher must stamp this claim on every
        // successful resolution.
        var erpUserId = Guid.NewGuid();
        _users.Setup(u => u.GetMeAsync(It.IsAny<CancellationToken>()))
              .ReturnsAsync(Result<CurrentUserDto>.Success(Profile(userId: erpUserId)));

        var enriched = await BuildSut().EnrichAsync(AuthenticatedPrincipal());

        enriched.FindFirst("erp_user_id")?.Value.Should().Be(erpUserId.ToString());
    }

    [Fact]
    public async Task Caches_profile_so_subsequent_calls_skip_the_API()
    {
        // The 30s IMemoryCache keyed on sub avoids a /me round-trip for every
        // request from the same user. The library's per-request stash handles
        // re-entry within a SINGLE request; this cache handles re-entry ACROSS
        // requests within the cache TTL.
        _users.Setup(u => u.GetMeAsync(It.IsAny<CancellationToken>()))
              .ReturnsAsync(Result<CurrentUserDto>.Success(Profile()));

        var sut = BuildSut();
        var first = await sut.EnrichAsync(AuthenticatedPrincipal());
        var second = await sut.EnrichAsync(AuthenticatedPrincipal());

        first.FindFirst("erp_user_id").Should().NotBeNull();
        second.FindFirst("erp_user_id").Should().NotBeNull();
        _users.Verify(u => u.GetMeAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Fails_closed_when_api_returns_unsuccessful()
    {
        // Failure path: no role/permission/erp_user_id claims added; downstream
        // [Authorize] denies; AccessDenied.cshtml renders.
        _users.Setup(u => u.GetMeAsync(It.IsAny<CancellationToken>()))
              .ReturnsAsync(Result<CurrentUserDto>.Failure(new Error("api.404", "user not provisioned")));

        var principal = AuthenticatedPrincipal();
        var enriched = await BuildSut().EnrichAsync(principal);

        enriched.FindAll("role").Should().BeEmpty();
        enriched.FindAll("permission").Should().BeEmpty();
        enriched.FindFirst("erp_user_id").Should().BeNull();
    }

    [Fact]
    public async Task Returns_principal_unchanged_when_anonymous()
    {
        var anonymous = new ClaimsPrincipal(new ClaimsIdentity());

        var enriched = await BuildSut().EnrichAsync(anonymous);

        enriched.Should().BeSameAs(anonymous);
        _users.Verify(u => u.GetMeAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Returns_principal_unchanged_when_sub_claim_is_missing()
    {
        // An identity authenticated against some non-OIDC source that doesn't
        // carry sub — we can't resolve it, so we don't add ERP claims. The
        // principal is otherwise valid for routes that only require
        // RequireAuthenticatedUser().
        var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, "no-sub") }, "Test");
        var principal = new ClaimsPrincipal(identity);

        var enriched = await BuildSut().EnrichAsync(principal);

        enriched.Should().BeSameAs(principal);
        _users.Verify(u => u.GetMeAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Deduplicates_role_and_permission_claims_already_on_the_principal()
    {
        // If the OIDC handler (or the library's claim mapping) stamped overlapping
        // claims, the enricher must not double them.
        var identity = new ClaimsIdentity(
            new[]
            {
                new Claim("sub", "auth0|xyz"),
                new Claim("role", "SystemAdmin"),
                new Claim("permission", "vendor:view"),
            },
            authenticationType: "TestCookie",
            nameType: ClaimTypes.Name,
            roleType: "role");
        var principal = new ClaimsPrincipal(identity);

        _users.Setup(u => u.GetMeAsync(It.IsAny<CancellationToken>()))
              .ReturnsAsync(Result<CurrentUserDto>.Success(
                  Profile(roles: new[] { "SystemAdmin", "FinanceManager" },
                          permissions: new[] { "vendor:view", "invoice:approve" })));

        var enriched = await BuildSut().EnrichAsync(principal);

        enriched.FindAll("role").Select(c => c.Value)
            .Should().BeEquivalentTo(new[] { "SystemAdmin", "FinanceManager" });
        enriched.FindAll("permission").Select(c => c.Value)
            .Should().BeEquivalentTo(new[] { "vendor:view", "invoice:approve" });
    }
}
