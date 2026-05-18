using ChuA.ERP.Web.Mvc.Contracts.Common;
using ChuA.ERP.Web.Mvc.Contracts.Dtos;

namespace ChuA.ERP.Web.Mvc.ViewModels.Companies;

/// <summary>List page view model — wraps a paged result for the data table.</summary>
public sealed class CompanyListViewModel
{
    public PagedResult<CompanyDto> Page { get; set; } = PagedResult<CompanyDto>.Empty();
    public string? Search { get; set; }
}
