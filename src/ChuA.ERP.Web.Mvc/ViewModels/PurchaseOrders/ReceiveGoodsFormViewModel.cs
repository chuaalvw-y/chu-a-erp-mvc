// Copyright (c) 2026 Alvin Wilsen Chan Chua
// GitHub: chuaalvw-y
// Licensed under the Alvin Wilsen Chan Chua Proprietary Use-Only License.
// See LICENSE.txt in the project root for full license information.

using System.ComponentModel.DataAnnotations;
using ChuA.ERP.Web.Mvc.Contracts.Dtos;

namespace ChuA.ERP.Web.Mvc.ViewModels.PurchaseOrders;

/// <summary>Goods receipt form against an existing purchase order.</summary>
public sealed class ReceiveGoodsFormViewModel
{
    /// <summary>Purchase order id receiving against.</summary>
    public Guid Id { get; set; }

    [Display(Name = "Receipt date")]
    [DataType(DataType.Date)]
    public DateOnly ReceiptDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);

    [Required]
    [Display(Name = "Warehouse")]
    public Guid WarehouseId { get; set; }

    [StringLength(100)]
    [Display(Name = "Receipt number")]
    public string? ReceiptNumber { get; set; }

    [Display(Name = "Lines")]
    public List<ReceiveGoodsLineFormViewModel> Lines { get; set; } = new();

    public ReceiveGoodsRequest ToRequest() => new(
        ReceiptDate,
        WarehouseId,
        ReceiptNumber,
        Lines.Select(l => new ReceiveGoodsLineDto(
            l.PurchaseOrderLineId,
            l.ItemId,
            new QuantityDto(l.ReceivedValue, l.ReceivedUnitOfMeasure),
            l.UnitCostAmount.HasValue
                ? new MoneyDto(l.UnitCostAmount.Value, l.UnitCostCurrencyCode)
                : null)).ToList());
}
