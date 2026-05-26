// Copyright (c) 2026 Alvin Wilsen Chan Chua
// GitHub: chuaalvw-y
// Licensed under the Alvin Wilsen Chan Chua Proprietary Use-Only License.
// See LICENSE.txt in the project root for full license information.

using System.Security.Claims;
using ChuA.Authentication.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;

namespace ChuA.ERP.Web.Mvc.Tests.Security;

/// <summary>
/// Unit tests for the Phase J <c>ErpClaimsTransformation</c> — the
/// MVC's "Auth0 authenticates, ERP DB authorises" bridge. The contract
/// these tests pin down is load-bearing for the entire Phase J pipeline:
///
/// <list type="bullet">
///   <item>Role claims must be stamped using the identity's <c>RoleClaimType</c>
///         (NOT the <c>ClaimTypes.Role</c> URI) — the root cause of the
///         "Admin nav disappears under Auth0" symptom.</item>
///   <item>Re-entry within a request must short-circuit (sentinel claim
///         + <c>HttpContext.Items</c>) — otherwise every nav-item render
///         on a page triggers an API hit.</item>
///   <item>Failure modes (missing sub, unsuccessful /me, anonymous principal)
///         must fail closed by returning the principal unchanged.</item>
///   <item>An <c>erp_user_id</c> claim must be stamped on success so the
///         API <c>/me</c> endpoint can distinguish "authenticated and
///         resolved" from "authenticated but unresolved".</item>
/// </list>
/// </summary>
public sealed class ErpClaimsTransformationTests
{
    private readonly Mock<IUsersApiClient> _users = new(MockBehavior.Strict);
    private readonly IMemoryCache _cache = new MemoryCache(new MemoryCacheOptions());
    private readonly DefaultHttpContext _httpContext = new();
    private readonly Mock<IHttpContextAccessor> _httpContextAccessor = new();
    private readonly Mock<IClaimsMappingService> _claimsMapping = new();

    public ErpClaimsTransformationTests()
    {
        _httpContextAccessor.Setup(a => a.HttpContext).Returns(_httpContext);
        // Default: pass-through. Tests that care about the chained mapping
        // (e.g. asserting roles still surface at the configured claim type)
        // override this Setup explicitly.
        _claimsMapping.Setup(s => s.MapClaims(It.IsAny<ClaimsPrincipal>()))
                      .Returns((ClaimsPrincipal p) => p);
    }

    private ErpClaimsTransformation BuildSut() => new(
        _users.Object,
        _cache,
        _httpContextAccessor.Object,
        _claimsMapping.Object,
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
        // OIDC handler is configured with RoleClaimType="role"; the transformation
        // must emit role claims at that type, not the legacy ClaimTypes.Role URI.
        var profile = Profile(roles: new[] { "SystemAdmin", "FinanceManager" });
        _users.Setup(u => u.GetMeAsync(It.IsAny<CancellationToken>()))
              .ReturnsAsync(Result<CurrentUserDto>.Success(profile));

        var principal = AuthenticatedPrincipal(roleClaimType: "role");
        var transformed = await BuildSut().TransformAsync(principal);

        // Read using the configured role claim type, not ClaimTypes.Role.
        var emittedRoles = transformed.FindAll("role").Select(c => c.Value).ToArray();
        emittedRoles.Should().BeEquivalentTo("SystemAdmin", "FinanceManager");

        // And the literal URI bucket should be EMPTY — otherwise we've
        // re-introduced the bug Phase J was meant to close.
        transformed.FindAll(ClaimTypes.Role).Should().BeEmpty();
    }

    [Fact]
    public async Task Adds_one_permission_claim_per_value()
    {
        var profile = Profile(permissions: new[] { "vendor:view", "vendor:create", "invoice:approve" });
        _users.Setup(u => u.GetMeAsync(It.IsAny<CancellationToken>()))
              .ReturnsAsync(Result<CurrentUserDto>.Success(profile));

        var transformed = await BuildSut().TransformAsync(AuthenticatedPrincipal());

        var emitted = transformed.FindAll("permission").Select(c => c.Value).ToArray();
        emitted.Should().BeEquivalentTo("vendor:view", "vendor:create", "invoice:approve");
    }

    [Fact]
    public async Task Adds_erp_user_id_claim_after_resolution()
    {
        // The API /me endpoint reads erp_user_id (not sub) to enforce
        // fail-closed when the resolver couldn't link the IdP subject to
        // an ERP user — the transformation must stamp this claim on
        // every successful resolution.
        var erpUserId = Guid.NewGuid();
        _users.Setup(u => u.GetMeAsync(It.IsAny<CancellationToken>()))
              .ReturnsAsync(Result<CurrentUserDto>.Success(Profile(userId: erpUserId)));

        var transformed = await BuildSut().TransformAsync(AuthenticatedPrincipal());

        transformed.FindFirst("erp_user_id")?.Value.Should().Be(erpUserId.ToString());
    }

    [Fact]
    public async Task Short_circuits_on_sentinel_claim_within_the_same_request()
    {
        // First call: API hit, sentinel stamped.
        _users.Setup(u => u.GetMeAsync(It.IsAny<CancellationToken>()))
              .ReturnsAsync(Result<CurrentUserDto>.Success(Profile()));

        var sut = BuildSut();
        var transformed = await sut.TransformAsync(AuthenticatedPrincipal());
        transformed.HasClaim(ErpClaimsTransformation.SentinelClaimType,
                              ErpClaimsTransformation.SentinelClaimValue)
            .Should().BeTrue();

        // Second call: same principal, must short-circuit (no second API hit).
        var again = await sut.TransformAsync(transformed);
        again.Should().BeSameAs(transformed);
        _users.Verify(u => u.GetMeAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Re_entry_during_GetMeAsync_does_not_recurse_into_TransformAsync()
    {
        // Pins down the fix for a stack overflow discovered the first time
        // ErpClaimsTransformation actually ran end-to-end against Auth0.
        //
        // The call chain that recurses:
        //   TransformAsync
        //     -> _users.GetMeAsync()
        //       -> ApiClientBase.BuildRequestAsync()
        //         -> CookieTokenAcquisitionService.GetAccessTokenAsync()
        //           -> ctx.GetTokenAsync("access_token")
        //             -> IAuthenticationService.AuthenticateAsync()
        //               -> IClaimsTransformation.TransformAsync()   <-- recursion
        //
        // The contract this test enforces: the transformation stashes the
        // untransformed principal in HttpContext.Items BEFORE the GetMeAsync
        // call, so the re-entry short-circuits at step 1 instead of looping
        // back through the /me round-trip.

        ErpClaimsTransformation sut = null!;
        var reentryHitSentinel = false;
        _users.Setup(u => u.GetMeAsync(It.IsAny<CancellationToken>()))
              .Returns(async (CancellationToken _) =>
              {
                  // Simulate CookieTokenAcquisitionService triggering re-auth
                  // halfway through the outer call. If the re-entry doesn't
                  // hit the HttpContext.Items short-circuit, this either
                  // recurses infinitely (test framework times out / stack
                  // overflows) or fires _users.GetMeAsync() a second time.
                  var inner = await sut!.TransformAsync(AuthenticatedPrincipal()).ConfigureAwait(false);
                  reentryHitSentinel = inner is not null;
                  return Result<CurrentUserDto>.Success(Profile());
              });

        sut = BuildSut();
        var outer = await sut.TransformAsync(AuthenticatedPrincipal());

        // Exactly one /me round-trip across the outer + re-entrant calls.
        _users.Verify(u => u.GetMeAsync(It.IsAny<CancellationToken>()), Times.Once);
        reentryHitSentinel.Should().BeTrue();

        // The outer call still produces a fully hydrated principal (sentinel +
        // erp_user_id + permission claims), proving the guard only short-
        // circuits the re-entry — not the outer call.
        outer.HasClaim(ErpClaimsTransformation.SentinelClaimType,
                       ErpClaimsTransformation.SentinelClaimValue).Should().BeTrue();
        outer.FindFirst("erp_user_id").Should().NotBeNull();
    }

    [Fact]
    public async Task Memoizes_in_HttpContext_Items_so_re_entry_reuses_the_principal()
    {
        // Even if the framework re-runs the transformation against the original
        // (pre-transformed) principal — which doesn't yet carry the sentinel —
        // we should reuse the cached, hydrated principal from HttpContext.Items
        // rather than re-call the API.
        _users.Setup(u => u.GetMeAsync(It.IsAny<CancellationToken>()))
              .ReturnsAsync(Result<CurrentUserDto>.Success(Profile()));

        var sut = BuildSut();
        var first = await sut.TransformAsync(AuthenticatedPrincipal());
        var second = await sut.TransformAsync(AuthenticatedPrincipal());     // fresh principal

        first.Should().BeSameAs(second);
        _users.Verify(u => u.GetMeAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Fails_closed_when_api_returns_unsuccessful()
    {
        // Failure path: no role/permission claims added; downstream
        // [Authorize] denies; AccessDenied.cshtml renders.
        _users.Setup(u => u.GetMeAsync(It.IsAny<CancellationToken>()))
              .ReturnsAsync(Result<CurrentUserDto>.Failure(new Error("api.404", "user not provisioned")));

        var principal = AuthenticatedPrincipal();
        var transformed = await BuildSut().TransformAsync(principal);

        transformed.FindAll("role").Should().BeEmpty();
        transformed.FindAll("permission").Should().BeEmpty();
        transformed.FindFirst("erp_user_id").Should().BeNull();
    }

    [Fact]
    public async Task Returns_principal_unchanged_when_anonymous()
    {
        var anonymous = new ClaimsPrincipal(new ClaimsIdentity());   // not authenticated

        var transformed = await BuildSut().TransformAsync(anonymous);

        transformed.Should().BeSameAs(anonymous);
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

        var transformed = await BuildSut().TransformAsync(principal);

        transformed.Should().BeSameAs(principal);
        _users.Verify(u => u.GetMeAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Deduplicates_role_and_permission_claims_already_on_the_principal()
    {
        // If the OIDC handler somehow stamped overlapping claims (e.g. a
        // custom Action that emitted a "role" claim too), the transformation
        // must not double them.
        var identity = new ClaimsIdentity(
            new[]
            {
                new Claim("sub", "auth0|xyz"),
                new Claim("role", "SystemAdmin"),               // already present
                new Claim("permission", "vendor:view"),         // already present
            },
            authenticationType: "TestCookie",
            nameType: ClaimTypes.Name,
            roleType: "role");
        var principal = new ClaimsPrincipal(identity);

        _users.Setup(u => u.GetMeAsync(It.IsAny<CancellationToken>()))
              .ReturnsAsync(Result<CurrentUserDto>.Success(
                  Profile(roles: new[] { "SystemAdmin", "FinanceManager" },
                          permissions: new[] { "vendor:view", "invoice:approve" })));

        var transformed = await BuildSut().TransformAsync(principal);

        transformed.FindAll("role").Select(c => c.Value)
            .Should().BeEquivalentTo(new[] { "SystemAdmin", "FinanceManager" });
        transformed.FindAll("permission").Select(c => c.Value)
            .Should().BeEquivalentTo(new[] { "vendor:view", "invoice:approve" });
    }
}
