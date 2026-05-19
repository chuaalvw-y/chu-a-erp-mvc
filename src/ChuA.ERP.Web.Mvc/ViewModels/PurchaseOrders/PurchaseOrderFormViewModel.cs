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

    [Display(Name = "Expected delivery")]
    [DataType(DataType.Date)]
    public DateOnly? ExpectedDeliveryDate { get; set; }

    [Required, StringLength(3, MinimumLength = 3)]
    [Display(Name = "Currency")]
    public string CurrencyCode { get; set; } = "USD";

    [Display(Name = "Lines")]
    public List<PurchaseOrderLineFormViewModel> Lines { get; set; } = new();

    public IReadOnlyList<VendorDto> Vendors { get; set; } = Array.Empty<VendorDto>();
    public IReadOnlyList<ItemDto> Items { get; set; } = Array.Empty<ItemDto>();

    public bool IsEdit => Id.HasValue;

    public static PurchaseOrderFormViewModel FromDto(PurchaseOrderDto dto) => new()
    {
        Id = dto.Id,
        VendorId = dto.VendorId,
        OrderNumber = dto.OrderNumber,
        OrderDate = dto.OrderDate,
        ExpectedDeliveryDate = dto.ExpectedDeliveryDate,
        CurrencyCode = dto.CurrencyCode,
        Lines = dto.Lines?.Select(l => new PurchaseOrderLineFormViewModel
        {
            ItemId = l.ItemId,
            Description = l.Description,
            QuantityValue = l.OrderedQuantity.Value,
            QuantityUnitOfMeasure = l.OrderedQuantity.UnitOfMeasure,
            UnitPriceAmount = l.UnitPrice.Amount,
            UnitPriceCurrencyCode = l.UnitPrice.CurrencyCode,
        }).ToList() ?? new List<PurchaseOrderLineFormViewModel> { new() },
    };

    public CreatePurchaseOrderRequest ToCreateRequest() => new(
        VendorId,
        OrderNumber,
        OrderDate,
        CurrencyCode,
        ExpectedDeliveryDate,
        Lines.Select(BuildLine).ToList());

    public UpdatePurchaseOrderRequest ToUpdateRequest() => new(
        OrderNumber,
        OrderDate,
        ExpectedDeliveryDate,
        CurrencyCode,
        Lines.Select(BuildLine).ToList());

    private static CreatePurchaseOrderLineDto BuildLine(PurchaseOrderLineFormViewModel l) => new(
        l.ItemId,
        l.Description,
        new QuantityDto(l.QuantityValue, l.QuantityUnitOfMeasure),
        new MoneyDto(l.UnitPriceAmount, l.UnitPriceCurrencyCode));
}
