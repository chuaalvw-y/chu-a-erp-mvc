// Copyright (c) 2026 Alvin Wilsen Chan Chua
// GitHub: chuaalvw-y
// Licensed under the Alvin Wilsen Chan Chua Proprietary Use-Only License.
// See LICENSE.txt in the project root for full license information.

using ChuA.ERP.Web.Mvc.Services;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace ChuA.ERP.Web.Mvc.TagHelpers;

/// <summary>
/// Suppresses the rendering of any element decorated with <c>asp-authorize-policy="X"</c>
/// (or comma-separated list) unless the current user owns at least one of the listed
/// permissions. Used for menu/UI visibility — defense in depth alongside server-side
/// <c>[Authorize(Policy=...)]</c> on the actions themselves.
///
/// <para>Phase J — permissions are already on the principal by the time this
/// helper runs (<see cref="Security.ErpClaimsTransformation"/> hydrates them
/// once per request from <c>/api/v1/users/me</c>, cached 30s). The previous
/// per-call <c>LoadProfileAsync</c> fallback has been removed: it's now
/// redundant work that fires once per nav item on every page render. The
/// helper became synchronous as a result.</para>
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

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        var policies = (Policy ?? string.Empty)
            .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (policies.Length == 0)
        {
            return;
        }

        if (!_currentUser.HasAnyPermission(policies))
        {
            output.SuppressOutput();
        }
    }
}
