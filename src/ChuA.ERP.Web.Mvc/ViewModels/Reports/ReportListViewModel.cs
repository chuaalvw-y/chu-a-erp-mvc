using ChuA.ERP.Web.Mvc.Contracts.Dtos;

namespace ChuA.ERP.Web.Mvc.ViewModels.Reports;

/// <summary>List page view model — wraps the available report definitions.</summary>
public sealed class ReportListViewModel
{
    public IReadOnlyList<ReportSummaryDto> Reports { get; set; } = Array.Empty<ReportSummaryDto>();
}
