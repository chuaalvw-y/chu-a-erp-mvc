using ChuA.ERP.Web.Mvc.Contracts.Common;
using ChuA.ERP.Web.Mvc.Contracts.Dtos;

namespace ChuA.ERP.Web.Mvc.ViewModels.Inventory;

/// <summary>List page view model — wraps a paged result for the inventory items table.</summary>
public sealed class InventoryListViewModel
{
    public PagedResult<ItemDto> Page { get; set; } = PagedResult<ItemDto>.Empty();
    public string? Search { get; set; }
}
