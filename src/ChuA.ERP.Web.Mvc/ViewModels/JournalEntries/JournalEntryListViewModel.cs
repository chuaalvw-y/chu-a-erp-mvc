using ChuA.ERP.Web.Mvc.Contracts.Common;
using ChuA.ERP.Web.Mvc.Contracts.Dtos;

namespace ChuA.ERP.Web.Mvc.ViewModels.JournalEntries;

/// <summary>List page view model for Journal entries.</summary>
public sealed class JournalEntryListViewModel
{
    public PagedResult<JournalEntryDto> Page { get; set; } = PagedResult<JournalEntryDto>.Empty();
    public Guid? FiscalPeriodId { get; set; }
    public string? Status { get; set; }
}
