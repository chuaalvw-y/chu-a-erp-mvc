using Microsoft.AspNetCore.Razor.TagHelpers;

namespace ChuA.ERP.Web.Mvc.TagHelpers;

/// <summary>
/// Decorates a <c>&lt;button&gt;</c> with the data attributes consumed by
/// <c>chua-loading.js</c> so that on form submit the button is disabled and its
/// label changes to the configured progress text.
///
/// Usage:
/// <code>
///   &lt;button type="submit" asp-loading-text="Saving..." class="btn btn-primary"&gt;Save&lt;/button&gt;
/// </code>
///
/// Notes:
/// - This helper does not add behaviour by itself; the auto-wiring lives in
///   <c>chua-loading.js</c>. It just sets <c>data-loading-text</c> so the JS
///   knows what label to show.
/// - The button does not need to be inside a form managed by the loading
///   helpers — any submit button benefits because the JS hooks every form
///   submit by default.
/// </summary>
[HtmlTargetElement("button", Attributes = LoadingTextAttribute)]
[HtmlTargetElement("input", Attributes = LoadingTextAttribute)]
public sealed class LoadingButtonTagHelper : TagHelper
{
    private const string LoadingTextAttribute = "asp-loading-text";
    private const string SkipAttribute = "asp-loading-skip";

    /// <summary>Replacement label displayed while the operation is in progress (e.g. "Saving...").</summary>
    [HtmlAttributeName(LoadingTextAttribute)]
    public string? LoadingText { get; set; }

    /// <summary>If true, the global auto-wiring will leave this button alone.</summary>
    [HtmlAttributeName(SkipAttribute)]
    public bool Skip { get; set; }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        if (!string.IsNullOrWhiteSpace(LoadingText))
        {
            output.Attributes.SetAttribute("data-loading-text", LoadingText);
        }
        if (Skip)
        {
            output.Attributes.SetAttribute("data-loading-skip", "true");
        }
    }
}
