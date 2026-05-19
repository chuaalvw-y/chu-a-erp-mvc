using Microsoft.AspNetCore.Razor.TagHelpers;

namespace ChuA.ERP.Web.Mvc.Tests.TagHelpers;

public class LoadingFormTagHelperTests
{
    private static (TagHelperContext Ctx, TagHelperOutput Output) NewElement()
    {
        var ctx = new TagHelperContext(new TagHelperAttributeList(), new Dictionary<object, object>(), Guid.NewGuid().ToString());
        var output = new TagHelperOutput(
            "form",
            new TagHelperAttributeList(),
            (_, _) => Task.FromResult<TagHelperContent>(new DefaultTagHelperContent()));
        return (ctx, output);
    }

    [Fact]
    public void Disabled_form_should_emit_data_loading_skip()
    {
        var sut = new LoadingFormTagHelper { Enabled = false };
        var (ctx, output) = NewElement();

        sut.Process(ctx, output);

        output.Attributes.ContainsName("data-loading-skip").Should().BeTrue();
        output.Attributes.ContainsName("data-loading-section").Should().BeFalse();
    }

    [Fact]
    public void Section_selector_and_message_should_emit_both_data_attributes()
    {
        var sut = new LoadingFormTagHelper
        {
            Enabled = true,
            SectionSelector = "#post-section",
            SectionMessage = "Posting journal entry...",
        };
        var (ctx, output) = NewElement();

        sut.Process(ctx, output);

        output.Attributes["data-loading-section"].Value.Should().Be("#post-section");
        output.Attributes["data-loading-section-message"].Value.Should().Be("Posting journal entry...");
        output.Attributes.ContainsName("data-loading-skip").Should().BeFalse();
    }

    [Fact]
    public void Defaults_with_no_section_should_be_noop()
    {
        var sut = new LoadingFormTagHelper();
        var (ctx, output) = NewElement();

        sut.Process(ctx, output);

        output.Attributes.ContainsName("data-loading-skip").Should().BeFalse();
        output.Attributes.ContainsName("data-loading-section").Should().BeFalse();
        output.Attributes.ContainsName("data-loading-section-message").Should().BeFalse();
    }
}
