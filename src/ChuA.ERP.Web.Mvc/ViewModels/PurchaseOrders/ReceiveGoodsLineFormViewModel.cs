using System.ComponentModel.DataAnnotations;

namespace ChuA.ERP.Web.Mvc.ViewModels.PurchaseOrders;

/// <summary>One row in the Receive Goods form lines table.</summary>
public sealed class ReceiveGoodsLineFormViewModel
{
    [Required]
    [Display(Name = "PO line")]
    public Guid PurchaseOrderLineId { get; set; }

    [Required]
    [Display(Name = "Item")]
    public Guid ItemId { get; set; }

    [Range(typeof(decimal), "0.0001", "999999999")]
    [Display(Name = "Received qty")]
    public decimal ReceivedValue { get; set; }

    [Required, StringLength(20)]
    [Display(Name = "UoM")]
    public string ReceivedUnitOfMeasure { get; set; } = "EA";

    [Display(Name = "Unit cost")]
    public decimal? UnitCostAmount { get; set; }

    [Required, StringLength(3, MinimumLength = 3)]
    [Display(Name = "Currency")]
    public string UnitCostCurrencyCode { get; set; } = "USD";
}
