using ChuA.ERP.Web.Mvc.Services;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace ChuA.ERP.Web.Mvc.TagHelpers;

/// <summary>
/// Suppresses the rendering of any element decorated with <c>asp-authorize-policy="X"</c>
/// (or comma-separated list) unless the current user owns at least one of the listed
/// permissions. Used for menu/UI visibility — defense in depth alongside server-side
/// <c>[Authorize(Policy=...)]</c> on the actions themselves.
/// </summary>
[HtmlTargetElement(Attributes = "asp-authorize-policy")]
public sealed class AuthorizePolicyTagHelper : TagHelper
{
    private readonly ICurrentUserService _currentUser;

    public AuthorizePolicyTagHelper(ICurrentUserService currentUser)
    {
        _currentUser = currentUser;
    }

    [HtmlAttributeName("asp-authorize-policy")]
    public string Policy { get; set; } = string.Empty;

    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        var policies = (Policy ?? string.Empty)
            .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (policies.Length == 0)
        {
            return;
        }

        if (!_currentUser.HasAnyPermission(policies))
        {
            await _currentUser.LoadProfileAsync().ConfigureAwait(false);
        }

        if (!_currentUser.HasAnyPermission(policies))
        {
            output.SuppressOutput();
        }
    }
}
