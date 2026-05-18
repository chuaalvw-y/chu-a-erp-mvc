using ChuA.ERP.Web.Mvc.Contracts.Common;
using ChuA.ERP.Web.Mvc.Contracts.Dtos;

namespace ChuA.ERP.Web.Mvc.ViewModels.Customers;

/// <summary>List page view model — wraps a paged result for the data table.</summary>
public sealed class CustomerListViewModel
{
    public PagedResult<CustomerDto> Page { get; set; } = PagedResult<CustomerDto>.Empty();
    public string? Search { get; set; }
}
