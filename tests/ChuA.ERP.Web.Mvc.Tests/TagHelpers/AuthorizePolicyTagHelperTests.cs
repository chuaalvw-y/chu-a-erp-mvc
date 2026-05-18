using Microsoft.AspNetCore.Razor.TagHelpers;

namespace ChuA.ERP.Web.Mvc.Tests.TagHelpers;

public class AuthorizePolicyTagHelperTests
{
    private static TagHelperOutput NewOutput() => new(
        "li",
        new TagHelperAttributeList(),
        (_, _) => Task.FromResult<TagHelperContent>(new DefaultTagHelperContent()));

    private static TagHelperContext NewContext() => new(
        new TagHelperAttributeList(),
        new Dictionary<object, object>(),
        Guid.NewGuid().ToString());

    [Fact]
    public void Suppresses_when_user_lacks_permission()
    {
        var user = new Mock<ICurrentUserService>();
        user.Setup(u => u.HasAnyPermission(It.IsAny<string[]>())).Returns(false);
        var sut = new AuthorizePolicyTagHelper(user.Object) { Policy = "VendorCreate" };

        var output = NewOutput();
        sut.Process(NewContext(), output);

        output.TagName.Should().BeNull();
    }

    [Fact]
    public void Renders_when_user_has_permission()
    {
        var user = new Mock<ICurrentUserService>();
        user.Setup(u => u.HasAnyPermission(It.IsAny<string[]>())).Returns(true);
        var sut = new AuthorizePolicyTagHelper(user.Object) { Policy = "VendorCreate" };

        var output = NewOutput();
        sut.Process(NewContext(), output);

        output.TagName.Should().Be("li");
    }

    [Fact]
    public void Splits_comma_separated_policy_list()
    {
        var user = new Mock<ICurrentUserService>();
        sut(user.Object, "VendorRead, BillRead").Process(NewContext(), NewOutput());
        user.Verify(u => u.HasAnyPermission(It.Is<string[]>(arr => arr.Length == 2 && arr.Contains("VendorRead") && arr.Contains("BillRead"))));
    }

    private static AuthorizePolicyTagHelper sut(ICurrentUserService user, string policy) => new(user) { Policy = policy };
}
