// Copyright (c) 2026 Alvin Wilsen Chan Chua
// GitHub: chuaalvw-y
// Licensed under the Alvin Wilsen Chan Chua Proprietary Use-Only License.
// See LICENSE.txt in the project root for full license information.

namespace ChuA.ERP.Web.Mvc.ViewModels;

/// <summary>Shared page header model for list/detail screens with command actions.</summary>
public sealed class PageHeaderViewModel
{
    public required string Title { get; init; }
    public IReadOnlyList<PageHeaderActionViewModel> Actions { get; init; } = Array.Empty<PageHeaderActionViewModel>();
}

public sealed class PageHeaderActionViewModel
{
    public required string Text { get; init; }
    public required string Url { get; init; }
    public string CssClass { get; init; } = "btn btn-primary";
    public string? Policy { get; init; }
    public string? IconCssClass { get; init; }
}
