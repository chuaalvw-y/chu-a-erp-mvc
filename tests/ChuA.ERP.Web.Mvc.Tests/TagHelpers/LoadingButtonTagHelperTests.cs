using Microsoft.AspNetCore.Razor.TagHelpers;

namespace ChuA.ERP.Web.Mvc.Tests.TagHelpers;

public class LoadingButtonTagHelperTests
{
    private static (TagHelperContext Ctx, TagHelperOutput Output) NewElement(string tagName = "button")
    {
        var ctx = new TagHelperContext(new TagHelperAttributeList(), new Dictionary<object, object>(), Guid.NewGuid().ToString());
        var output = new TagHelperOutput(
            tagName,
            new TagHelperAttributeList(),
            (_, _) => Task.FromResult<TagHelperContent>(new DefaultTagHelperContent()));
        return (ctx, output);
    }

    [Fact]
    public void Loading_text_should_become_data_loading_text_attribute()
    {
        var sut = new LoadingButtonTagHelper { LoadingText = "Saving..." };
        var (ctx, output) = NewElement();

        sut.Process(ctx, output);

        output.Attributes.ContainsName("data-loading-text").Should().BeTrue();
        output.Attributes["data-loading-text"].Value.Should().Be("Saving...");
    }

    [Fact]
    public void Skip_flag_should_emit_data_loading_skip()
    {
        var sut = new LoadingButtonTagHelper { Skip = true };
        var (ctx, output) = NewElement();

        sut.Process(ctx, output);

        output.Attributes.ContainsName("data-loading-skip").Should().BeTrue();
    }

    [Fact]
    public void No_loading_text_no_skip_should_be_a_noop()
    {
        var sut = new LoadingButtonTagHelper();
        var (ctx, output) = NewElement();

        sut.Process(ctx, output);

        output.Attributes.ContainsName("data-loading-text").Should().BeFalse();
        output.Attributes.ContainsName("data-loading-skip").Should().BeFalse();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Empty_loading_text_should_not_emit_attribute(string? value)
    {
        var sut = new LoadingButtonTagHelper { LoadingText = value };
        var (ctx, output) = NewElement();

        sut.Process(ctx, output);

        output.Attributes.ContainsName("data-loading-text").Should().BeFalse();
    }
}
