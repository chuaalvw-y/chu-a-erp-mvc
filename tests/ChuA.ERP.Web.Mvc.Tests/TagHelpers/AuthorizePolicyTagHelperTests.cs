// Copyright (c) 2026 Alvin Wilsen Chan Chua
// GitHub: chuaalvw-y
// Licensed under the Alvin Wilsen Chan Chua Proprietary Use-Only License.
// See LICENSE.txt in the project root for full license information.

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
    public async Task Suppresses_when_user_lacks_permission()
    {
        var user = new Mock<ICurrentUserService>();
        user.Setup(u => u.HasAnyPermission(It.IsAny<string[]>())).Returns(false);
        var sut = new AuthorizePolicyTagHelper(user.Object) { Policy = "vendor:create" };

        var output = NewOutput();
        await sut.ProcessAsync(NewContext(), output);

        output.TagName.Should().BeNull();
    }

    [Fact]
    public async Task Renders_when_user_has_permission()
    {
        var user = new Mock<ICurrentUserService>();
        user.Setup(u => u.HasAnyPermission(It.IsAny<string[]>())).Returns(true);
        var sut = new AuthorizePolicyTagHelper(user.Object) { Policy = "vendor:create" };

        var output = NewOutput();
        await sut.ProcessAsync(NewContext(), output);

        output.TagName.Should().Be("li");
    }

    [Fact]
    public async Task Splits_comma_separated_policy_list()
    {
        var user = new Mock<ICurrentUserService>();
        await sut(user.Object, "VendorRead, BillRead").ProcessAsync(NewContext(), NewOutput());
        user.Verify(u => u.HasAnyPermission(It.Is<string[]>(arr => arr.Length == 2 && arr.Contains("vendor:view") && arr.Contains("bill:view"))));
    }

    [Fact]
    public async Task Loads_profile_before_suppressing_when_permission_is_not_in_claims()
    {
        var user = new Mock<ICurrentUserService>();
        user.SetupSequence(u => u.HasAnyPermission(It.IsAny<string[]>()))
            .Returns(false)
            .Returns(true);

        var output = NewOutput();
        await sut(user.Object, "vendor:view").ProcessAsync(NewContext(), output);

        user.Verify(u => u.LoadProfileAsync(It.IsAny<CancellationToken>()), Times.Once);
        output.TagName.Should().Be("li");
    }

    private static AuthorizePolicyTagHelper sut(ICurrentUserService user, string policy) => new(user) { Policy = policy };
}
