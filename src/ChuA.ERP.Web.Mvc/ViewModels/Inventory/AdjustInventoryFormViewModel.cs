using System.ComponentModel.DataAnnotations;
using ChuA.ERP.Web.Mvc.Contracts.Dtos;

namespace ChuA.ERP.Web.Mvc.ViewModels.Inventory;

/// <summary>Manual inventory adjustment form. Delta may be negative.</summary>
public sealed class AdjustInventoryFormViewModel
{
    /// <summary>Item id being adjusted.</summary>
    public Guid Id { get; set; }

    [Required]
    [Display(Name = "Warehouse")]
    public Guid WarehouseId { get; set; }

    [Display(Name = "Delta")]
    public decimal DeltaValue { get; set; }

    [Required, StringLength(20)]
    [Display(Name = "Unit of measure")]
    public string DeltaUnitOfMeasure { get; set; } = "EA";

    [Required, StringLength(200)]
    [Display(Name = "Reason")]
    public string Reason { get; set; } = string.Empty;

    public AdjustInventoryRequest ToRequest() => new(
        WarehouseId,
        new QuantityDto(DeltaValue, DeltaUnitOfMeasure),
        Reason);
}
