// Copyright (c) 2026 Alvin Wilsen Chan Chua
// GitHub: chuaalvw-y
// Licensed under the Alvin Wilsen Chan Chua Proprietary Use-Only License.
// See LICENSE.txt in the project root for full license information.

using System.ComponentModel.DataAnnotations;
using ChuA.ERP.Web.Mvc.Contracts.Dtos;

namespace ChuA.ERP.Web.Mvc.ViewModels.SalesOrders;

/// <summary>Shipment form against an existing sales order.</summary>
public sealed class ShipSalesOrderFormViewModel
{
    /// <summary>Sales order id shipping against.</summary>
    public Guid Id { get; set; }

    [Display(Name = "Shipment date")]
    [DataType(DataType.Date)]
    public DateOnly ShipmentDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);

    [Required]
    [Display(Name = "Warehouse")]
    public Guid WarehouseId { get; set; }

    [StringLength(100)]
    [Display(Name = "Shipment number")]
    public string? ShipmentNumber { get; set; }

    [StringLength(100)]
    [Display(Name = "Carrier")]
    public string? Carrier { get; set; }

    [StringLength(100)]
    [Display(Name = "Tracking number")]
    public string? TrackingNumber { get; set; }

    [Display(Name = "Lines")]
    public List<ShipSalesOrderLineFormViewModel> Lines { get; set; } = new();

    public ShipSalesOrderRequest ToRequest() => new(
        ShipmentDate,
        WarehouseId,
        ShipmentNumber,
        Carrier,
        TrackingNumber,
        Lines.Select(l => new ShipSalesOrderLineDto(
            l.SalesOrderLineId,
            l.ItemId,
            new QuantityDto(l.ShippedValue, l.ShippedUnitOfMeasure))).ToList());
}
