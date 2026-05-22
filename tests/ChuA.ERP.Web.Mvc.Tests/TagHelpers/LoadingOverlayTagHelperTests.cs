// Copyright (c) 2026 Alvin Wilsen Chan Chua
// GitHub: chuaalvw-y
// Licensed under the Alvin Wilsen Chan Chua Proprietary Use-Only License.
// See LICENSE.txt in the project root for full license information.

using System.IO;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace ChuA.ERP.Web.Mvc.Tests.TagHelpers;

public class LoadingOverlayTagHelperTests
{
    private static (TagHelperContext Ctx, TagHelperOutput Output) NewElement()
    {
        var ctx = new TagHelperContext(new TagHelperAttributeList(), new Dictionary<object, object>(), Guid.NewGuid().ToString());
        var output = new TagHelperOutput(
            "loading-overlay",
            new TagHelperAttributeList(),
            (_, _) =>
            {
                TagHelperContent content = new DefaultTagHelperContent();
                content.SetHtmlContent("<form>child</form>");
                return Task.FromResult(content);
            });
        return (ctx, output);
    }

    private static string Render(TagHelperOutput output)
    {
        using var writer = new StringWriter();
        output.WriteTo(writer, System.Text.Encodings.Web.HtmlEncoder.Default);
        return writer.ToString();
    }

    [Fact]
    public void Renders_as_div_with_required_class_and_id()
    {
        var sut = new LoadingOverlayTagHelper { Id = "post-section", Message = "Posting..." };
        var (ctx, output) = NewElement();

        sut.Process(ctx, output);

        output.TagName.Should().Be("div");
        output.Attributes["class"].Value.ToString().Should().Contain("chua-loading-host");
        output.Attributes["id"].Value.Should().Be("post-section");
        output.Attributes["aria-busy"].Value.Should().Be("false");
    }

    [Fact]
    public void Overlay_markup_should_be_hidden_with_spinner_and_message()
    {
        var sut = new LoadingOverlayTagHelper { Id = "x", Message = "Working..." };
        var (ctx, output) = NewElement();

        sut.Process(ctx, output);

        // PostContent holds the hidden overlay markup; combine the body + PostContent for rendering.
        var post = output.PostContent.GetContent();
        post.Should().Contain("chua-loading-overlay");
        post.Should().Contain("hidden");
        post.Should().Contain("spinner-border");
        post.Should().Contain("Working...");
        post.Should().Contain("role=\"status\"");
        post.Should().Contain("aria-live=\"polite\"");
    }

    [Fact]
    public void Custom_class_should_be_appended_to_default()
    {
        var sut = new LoadingOverlayTagHelper { Id = "x", Message = "hi", CssClass = "card p-3" };
        var (ctx, output) = NewElement();

        sut.Process(ctx, output);

        var cls = output.Attributes["class"].Value.ToString()!;
        cls.Should().Contain("chua-loading-host");
        cls.Should().Contain("card p-3");
    }

    [Fact]
    public void Message_should_be_html_encoded()
    {
        var sut = new LoadingOverlayTagHelper { Id = "x", Message = "<script>alert(1)</script>" };
        var (ctx, output) = NewElement();

        sut.Process(ctx, output);

        output.PostContent.GetContent().Should().Contain("&lt;script&gt;alert(1)&lt;/script&gt;");
    }
}
