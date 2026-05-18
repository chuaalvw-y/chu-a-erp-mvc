using ChuA.ERP.Web.Mvc.Contracts.Common;
using ChuA.ERP.Web.Mvc.Contracts.Dtos;

namespace ChuA.ERP.Web.Mvc.ViewModels.Vendors;

/// <summary>List page view model — wraps a paged result for the data table.</summary>
public sealed class VendorListViewModel
{
    public PagedResult<VendorDto> Page { get; set; } = PagedResult<VendorDto>.Empty();
    public string? Search { get; set; }
}
