using Microsoft.AspNetCore.Razor.TagHelpers;

namespace ChuA.ERP.Web.Mvc.TagHelpers;

/// <summary>
/// Wraps a chunk of Razor with the markup needed for a section-level loading
/// overlay. Renders as a positioned host plus a hidden overlay child; JS toggles
/// the overlay via <c>ChuALoading.section('#id').show({ message })</c>.
///
/// Usage:
/// <code>
///   &lt;loading-overlay id="post-section" message="Posting journal entry..."&gt;
///     &lt;form ... asp-loading-section="#post-section"
///                 asp-loading-section-message="Posting journal entry..."&gt;
///       ...
///     &lt;/form&gt;
///   &lt;/loading-overlay&gt;
/// </code>
///
/// The host is rendered as a <c>&lt;div&gt;</c>; an explicit <c>id</c> is
/// required so the JS API can reference it. The overlay is hidden initially.
/// </summary>
[HtmlTargetElement("loading-overlay", Attributes = "id")]
public sealed class LoadingOverlayTagHelper : TagHelper
{
    /// <summary>Element id (required) — used by ChuALoading.section('#id').show().</summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>Initial message shown when the overlay is revealed (may be overridden in JS).</summary>
    public string Message { get; set; } = "Loading...";

    /// <summary>Extra CSS classes appended to the host div.</summary>
    [HtmlAttributeName("class")]
    public string? CssClass { get; set; }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        output.TagName = "div";
        output.TagMode = TagMode.StartTagAndEndTag;

        var classList = "chua-loading-host";
        if (!string.IsNullOrWhiteSpace(CssClass))
        {
            classList += " " + CssClass;
        }
        output.Attributes.SetAttribute("class", classList);
        output.Attributes.SetAttribute("id", Id);
        output.Attributes.SetAttribute("aria-busy", "false");

        // Append the hidden overlay markup AFTER the children render. Using
        // PostContent ensures it doesn't conflict with whatever the consumer
        // puts inside.
        var safeMessage = System.Net.WebUtility.HtmlEncode(Message);
        output.PostContent.SetHtmlContent(
            "<div class=\"chua-loading-overlay\" role=\"status\" aria-live=\"polite\" hidden>" +
                "<div class=\"chua-loading-overlay__inner\">" +
                    "<div class=\"spinner-border\" aria-hidden=\"true\"></div>" +
                    "<p class=\"chua-loading-overlay__text\">" + safeMessage + "</p>" +
                "</div>" +
            "</div>");
    }
}
