// Copyright (c) 2026 Alvin Wilsen Chan Chua
// GitHub: chuaalvw-y
// Licensed under the Alvin Wilsen Chan Chua Proprietary Use-Only License.
// See LICENSE.txt in the project root for full license information.

using System.ComponentModel.DataAnnotations;
using ChuA.ERP.Web.Mvc.Contracts.Dtos;

namespace ChuA.ERP.Web.Mvc.ViewModels.Inventory;

/// <summary>Strongly typed form model shared by Create and Edit screens for inventory items.</summary>
public sealed class ItemFormViewModel
{
    public Guid? Id { get; set; }

    [Required, StringLength(60)]
    [Display(Name = "SKU")]
    public string Sku { get; set; } = string.Empty;

    [Required, StringLength(200)]
    [Display(Name = "Name")]
    public string Name { get; set; } = string.Empty;

    [Required, StringLength(20)]
    [Display(Name = "Default unit of measure")]
    public string DefaultUnitOfMeasure { get; set; } = "EA";

    [Display(Name = "Tracked as stock")]
    public bool IsStocked { get; set; } = true;

    [Range(typeof(decimal), "0", "999999999")]
    [Display(Name = "Reorder point")]
    public decimal? ReorderPoint { get; set; }

    public bool IsEdit => Id.HasValue;

    public static ItemFormViewModel FromDto(ItemDto dto) => new()
    {
        Id = dto.Id,
        Sku = dto.Sku,
        Name = dto.Name,
        DefaultUnitOfMeasure = dto.DefaultUnitOfMeasure,
        IsStocked = dto.IsStocked,
        ReorderPoint = dto.ReorderPoint,
    };

    public CreateItemRequest ToCreateRequest() => new(Sku, Name, DefaultUnitOfMeasure, IsStocked, ReorderPoint);
    public UpdateItemRequest ToUpdateRequest() => new(Name, DefaultUnitOfMeasure, IsStocked, ReorderPoint);
}
