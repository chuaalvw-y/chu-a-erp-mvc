using System.ComponentModel.DataAnnotations;
using ChuA.ERP.Web.Mvc.Contracts.Dtos;

namespace ChuA.ERP.Web.Mvc.ViewModels.PurchaseOrders;

/// <summary>Strongly typed form model shared by Create and Edit screens for purchase orders.</summary>
public sealed class PurchaseOrderFormViewModel
{
    public Guid? Id { get; set; }

    [Required]
    [Display(Name = "Vendor")]
    public Guid VendorId { get; set; }

    [Required, StringLength(60)]
    [Display(Name = "Order number")]
    public string OrderNumber { get; set; } = string.Empty;

    [Display(Name = "Order date")]
    [DataType(DataType.Date)]
    public DateOnly OrderDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);

    [Required, StringLength(3, MinimumLength = 3)]
    [Display(Name = "Currency")]
    public string CurrencyCode { get; set; } = "USD";

    [Display(Name = "Lines")]
    public List<PurchaseOrderLineFormViewModel> Lines { get; set; } = new();

    public bool IsEdit => Id.HasValue;

    public static PurchaseOrderFormViewModel FromDto(PurchaseOrderDto dto) => new()
    {
        Id = dto.Id,
        VendorId = dto.VendorId,
        OrderNumber = dto.OrderNumber,
        OrderDate = dto.OrderDate,
        CurrencyCode = dto.CurrencyCode,
        // Lines are not present on the list DTO — leave empty; UI will let user add new lines on edit.
        Lines = new List<PurchaseOrderLineFormViewModel>(),
    };

    public CreatePurchaseOrderRequest ToCreateRequest() => new(
        VendorId,
        OrderNumber,
        OrderDate,
        CurrencyCode,
        Lines.Select(BuildLine).ToList());

    public UpdatePurchaseOrderRequest ToUpdateRequest() => new(
        OrderNumber,
        OrderDate,
        CurrencyCode,
        Lines.Select(BuildLine).ToList());

    private static CreatePurchaseOrderLineDto BuildLine(PurchaseOrderLineFormViewModel l) => new(
        l.ItemId,
        l.Description,
        new QuantityDto(l.QuantityValue, l.QuantityUnitOfMeasure),
        new MoneyDto(l.UnitPriceAmount, l.UnitPriceCurrencyCode));
}
