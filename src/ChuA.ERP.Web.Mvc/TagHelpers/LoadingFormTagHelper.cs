using Microsoft.AspNetCore.Razor.TagHelpers;

namespace ChuA.ERP.Web.Mvc.TagHelpers;

/// <summary>
/// Per-form opt-in / opt-out for the loading auto-wiring. Examples:
///
/// <code>
///   &lt;form asp-loading="false"&gt;...&lt;/form&gt;            <!-- opt-out entirely -->
///   &lt;form asp-loading-section="#post-section"
///         asp-loading-section-message="Posting..."&gt;...&lt;/form&gt;
/// </code>
///
/// <c>asp-loading</c> defaults to true. Setting it false sets
/// <c>data-loading-skip</c> so the global submit listener leaves the form alone
/// (useful for tiny non-critical forms or for views with bespoke handling).
///
/// <c>asp-loading-section</c> points to a host element (CSS selector) that will
/// be covered by a section overlay during submit. Used for critical operations
/// like posting a journal entry where the whole panel should be blocked, not
/// just the button.
/// </summary>
[HtmlTargetElement("form", Attributes = LoadingAttribute)]
[HtmlTargetElement("form", Attributes = SectionAttribute)]
[HtmlTargetElement("form", Attributes = SectionMessageAttribute)]
public sealed class LoadingFormTagHelper : TagHelper
{
    private const string LoadingAttribute = "asp-loading";
    private const string SectionAttribute = "asp-loading-section";
    private const string SectionMessageAttribute = "asp-loading-section-message";

    /// <summary>Set false to disable auto-wiring for this form.</summary>
    [HtmlAttributeName(LoadingAttribute)]
    public bool Enabled { get; set; } = true;

    /// <summary>CSS selector for an element to cover with a section overlay during submit.</summary>
    [HtmlAttributeName(SectionAttribute)]
    public string? SectionSelector { get; set; }

    /// <summary>Friendly message shown inside the section overlay.</summary>
    [HtmlAttributeName(SectionMessageAttribute)]
    public string? SectionMessage { get; set; }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        if (!Enabled)
        {
            output.Attributes.SetAttribute("data-loading-skip", "true");
        }
        if (!string.IsNullOrWhiteSpace(SectionSelector))
        {
            output.Attributes.SetAttribute("data-loading-section", SectionSelector);
        }
        if (!string.IsNullOrWhiteSpace(SectionMessage))
        {
            output.Attributes.SetAttribute("data-loading-section-message", SectionMessage);
        }
    }
}
