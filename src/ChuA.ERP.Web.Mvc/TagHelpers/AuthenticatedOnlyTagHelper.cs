// Copyright (c) 2026 Alvin Wilsen Chan Chua
// GitHub: chuaalvw-y
// Licensed under the Alvin Wilsen Chan Chua Proprietary Use-Only License.
// See LICENSE.txt in the project root for full license information.

using ChuA.ERP.Web.Mvc.Services;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace ChuA.ERP.Web.Mvc.TagHelpers;

/// <summary>
/// Suppresses rendering unless the user is authenticated.
/// Usage: <c>&lt;div asp-authenticated="true"&gt;…&lt;/div&gt;</c>.
/// </summary>
[HtmlTargetElement(Attributes = "asp-authenticated")]
public sealed class AuthenticatedOnlyTagHelper : TagHelper
{
    private readonly ICurrentUserService _currentUser;

    public AuthenticatedOnlyTagHelper(ICurrentUserService currentUser)
    {
        _currentUser = currentUser;
    }

    [HtmlAttributeName("asp-authenticated")]
    public bool RequireAuthenticated { get; set; } = true;

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        if (RequireAuthenticated && !_currentUser.IsAuthenticated)
        {
            output.SuppressOutput();
        }
        else if (!RequireAuthenticated && _currentUser.IsAuthenticated)
        {
            output.SuppressOutput();
        }
    }
}
