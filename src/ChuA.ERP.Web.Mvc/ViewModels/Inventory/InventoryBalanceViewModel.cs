using ChuA.ERP.Web.Mvc.Contracts.Dtos;

namespace ChuA.ERP.Web.Mvc.ViewModels.Inventory;

/// <summary>Composite view model for the Balance action — shows on-hand and available together with parent item context.</summary>
public sealed class InventoryBalanceViewModel
{
    public Guid ItemId { get; set; }
    public Guid WarehouseId { get; set; }
    public QuantityDto OnHand { get; set; } = new(0m, "EA");
    public QuantityDto Available { get; set; } = new(0m, "EA");
    public ItemDto Item { get; set; } = default!;
}
