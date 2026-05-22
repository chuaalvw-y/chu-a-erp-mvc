// Copyright (c) 2026 Alvin Wilsen Chan Chua
// GitHub: chuaalvw-y
// Licensed under the Alvin Wilsen Chan Chua Proprietary Use-Only License.
// See LICENSE.txt in the project root for full license information.

using ChuA.ERP.Web.Mvc.Contracts.Dtos;

namespace ChuA.ERP.Web.Mvc.ViewModels.Reports;

/// <summary>List page view model — wraps the available report definitions.</summary>
public sealed class ReportListViewModel
{
    public IReadOnlyList<ReportSummaryDto> Reports { get; set; } = Array.Empty<ReportSummaryDto>();
}
