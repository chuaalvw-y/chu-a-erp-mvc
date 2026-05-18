using ChuA.ERP.Web.Mvc.Contracts.Common;
using ChuA.ERP.Web.Mvc.Contracts.Dtos;

namespace ChuA.ERP.Web.Mvc.ViewModels.SalesOrders;

/// <summary>List page view model — wraps a paged result for the sales orders table.</summary>
public sealed class SalesOrderListViewModel
{
    public PagedResult<SalesOrderDto> Page { get; set; } = PagedResult<SalesOrderDto>.Empty();
    public Guid? CustomerId { get; set; }
    public string? Status { get; set; }
    public string? Search { get; set; }
}
