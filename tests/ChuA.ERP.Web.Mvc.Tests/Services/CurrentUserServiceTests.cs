using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace ChuA.ERP.Web.Mvc.Tests.Services;

public class CurrentUserServiceTests
{
    private static (CurrentUserService Sut, Mock<IUsersApiClient> Users, DefaultHttpContext Ctx) Build(Action<List<Claim>>? configureClaims = null)
    {
        var claims = new List<Claim>
        {
            new("sub", "u1"),
            new(ClaimTypes.Name, "alice"),
            new("preferred_username", "alice"),
            new("company_id", Guid.Empty.ToString()),
        };
        configureClaims?.Invoke(claims);
        var identity = new ClaimsIdentity(claims, "Test");
        var ctx = new DefaultHttpContext { User = new ClaimsPrincipal(identity) };
        var accessor = new HttpContextAccessor { HttpContext = ctx };
        var users = new Mock<IUsersApiClient>();
        var sut = new CurrentUserService(accessor, users.Object);
        return (sut, users, ctx);
    }

    [Fact]
    public void IsAuthenticated_should_reflect_principal()
    {
        var (sut, _, _) = Build();
        sut.IsAuthenticated.Should().BeTrue();
        sut.UserId.Should().Be("u1");
        sut.DisplayName.Should().Be("alice");
    }

    [Fact]
    public void IsInAnyRole_should_match_role_claims()
    {
        var (sut, _, _) = Build(claims => claims.Add(new Claim(ClaimTypes.Role, "SystemAdmin")));
        sut.IsInAnyRole("SystemAdmin", "CompanyAdmin").Should().BeTrue();
        sut.IsInAnyRole("VendorClerk").Should().BeFalse();
    }

    [Fact]
    public void HasAnyPermission_should_match_permission_claims()
    {
        var (sut, _, _) = Build(claims => claims.Add(new Claim("permission", "VendorRead")));
        sut.HasAnyPermission("VendorRead").Should().BeTrue();
        sut.HasAnyPermission("BillApprove").Should().BeFalse();
    }

    [Fact]
    public async Task LoadProfileAsync_should_cache_the_first_successful_call()
    {
        var (sut, users, _) = Build();
        var profile = new CurrentUserDto("u1", Guid.NewGuid(), new[] { "SystemAdmin" }, new[] { "VendorRead" });
        users.Setup(u => u.GetMeAsync(It.IsAny<CancellationToken>())).ReturnsAsync(Result<CurrentUserDto>.Success(profile));

        var first = await sut.LoadProfileAsync();
        var second = await sut.LoadProfileAsync();

        first.Should().BeSameAs(profile);
        second.Should().BeSameAs(profile);
        users.Verify(u => u.GetMeAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
