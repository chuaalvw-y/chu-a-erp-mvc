// Copyright (c) 2026 Alvin Wilsen Chan Chua
// GitHub: chuaalvw-y
// Licensed under the Alvin Wilsen Chan Chua Proprietary Use-Only License.
// See LICENSE.txt in the project root for full license information.

using System.ComponentModel.DataAnnotations;

namespace ChuA.ERP.Web.Mvc.ViewModels.PurchaseOrders;

/// <summary>One row in the PO lines editor.</summary>
public sealed class PurchaseOrderLineFormViewModel
{
    [Required]
    [Display(Name = "Item")]
    public Guid ItemId { get; set; }

    [Required, StringLength(500)]
    [Display(Name = "Description")]
    public string Description { get; set; } = string.Empty;

    [Range(typeof(decimal), "0.0001", "999999999")]
    [Display(Name = "Quantity")]
    public decimal QuantityValue { get; set; }

    [Required, StringLength(20)]
    [Display(Name = "UoM")]
    public string QuantityUnitOfMeasure { get; set; } = "EA";

    [Range(typeof(decimal), "0", "999999999")]
    [Display(Name = "Unit price")]
    public decimal UnitPriceAmount { get; set; }

    [Required, StringLength(3, MinimumLength = 3)]
    [Display(Name = "Currency")]
    public string UnitPriceCurrencyCode { get; set; } = "USD";
}
