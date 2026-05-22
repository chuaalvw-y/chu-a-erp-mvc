// Copyright (c) 2026 Alvin Wilsen Chan Chua
// GitHub: chuaalvw-y
// Licensed under the Alvin Wilsen Chan Chua Proprietary Use-Only License.
// See LICENSE.txt in the project root for full license information.

using System.ComponentModel.DataAnnotations;

namespace ChuA.ERP.Web.Mvc.ViewModels.SalesOrders;

/// <summary>One row in the Ship Sales Order form lines table.</summary>
public sealed class ShipSalesOrderLineFormViewModel
{
    [Required]
    [Display(Name = "SO line")]
    public Guid SalesOrderLineId { get; set; }

    [Required]
    [Display(Name = "Item")]
    public Guid ItemId { get; set; }

    [Range(typeof(decimal), "0.0001", "999999999")]
    [Display(Name = "Shipped qty")]
    public decimal ShippedValue { get; set; }

    [Required, StringLength(20)]
    [Display(Name = "UoM")]
    public string ShippedUnitOfMeasure { get; set; } = "EA";
}
