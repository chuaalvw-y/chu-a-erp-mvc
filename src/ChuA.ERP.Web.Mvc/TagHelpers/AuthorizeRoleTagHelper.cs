// Copyright (c) 2026 Alvin Wilsen Chan Chua
// GitHub: chuaalvw-y
// Licensed under the Alvin Wilsen Chan Chua Proprietary Use-Only License.
// See LICENSE.txt in the project root for full license information.

using ChuA.ERP.Web.Mvc.Services;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace ChuA.ERP.Web.Mvc.TagHelpers;

/// <summary>
/// Suppresses rendering unless the current user has any of the listed roles.
/// Usage: <c>&lt;li asp-authorize-role="SystemAdmin,CompanyAdmin"&gt;…&lt;/li&gt;</c>.
/// </summary>
[HtmlTargetElement(Attributes = "asp-authorize-role")]
public sealed class AuthorizeRoleTagHelper : TagHelper
{
    private readonly ICurrentUserService _currentUser;

    public AuthorizeRoleTagHelper(ICurrentUserService currentUser)
    {
        _currentUser = currentUser;
    }

    [HtmlAttributeName("asp-authorize-role")]
    public string Role { get; set; } = string.Empty;

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        var roles = (Role ?? string.Empty)
            .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (roles.Length == 0) return;
        if (!_currentUser.IsInAnyRole(roles)) output.SuppressOutput();
    }
}
