using System.ComponentModel.DataAnnotations;
using ChuA.ERP.Web.Mvc.Contracts.Dtos;

namespace ChuA.ERP.Web.Mvc.ViewModels.SalesOrders;

/// <summary>Strongly typed form model shared by Create and Edit screens for sales orders.</summary>
public sealed class SalesOrderFormViewModel
{
    public Guid? Id { get; set; }

    [Required]
    [Display(Name = "Customer")]
    public Guid CustomerId { get; set; }

    [Required, StringLength(60)]
    [Display(Name = "Order number")]
    public string OrderNumber { get; set; } = string.Empty;

    [Display(Name = "Order date")]
    [DataType(DataType.Date)]
    public DateOnly OrderDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);

    [Display(Name = "Requested ship")]
    [DataType(DataType.Date)]
    public DateOnly? RequestedShipDate { get; set; }

    [Required, StringLength(3, MinimumLength = 3)]
    [Display(Name = "Currency")]
    public string CurrencyCode { get; set; } = "USD";

    [Display(Name = "Lines")]
    public List<SalesOrderLineFormViewModel> Lines { get; set; } = new();

    public IReadOnlyList<CustomerDto> Customers { get; set; } = Array.Empty<CustomerDto>();
    public IReadOnlyList<ItemDto> Items { get; set; } = Array.Empty<ItemDto>();

    public bool IsEdit => Id.HasValue;

    public static SalesOrderFormViewModel FromDto(SalesOrderDto dto) => new()
    {
        Id = dto.Id,
        CustomerId = dto.CustomerId,
        OrderNumber = dto.OrderNumber,
        OrderDate = dto.OrderDate,
        RequestedShipDate = dto.RequestedShipDate,
        CurrencyCode = dto.CurrencyCode,
        Lines = dto.Lines?.Select(l => new SalesOrderLineFormViewModel
        {
            ItemId = l.ItemId,
            Description = l.Description,
            QuantityValue = l.OrderedQuantity.Value,
            QuantityUnitOfMeasure = l.OrderedQuantity.UnitOfMeasure,
            UnitPriceAmount = l.UnitPrice.Amount,
            UnitPriceCurrencyCode = l.UnitPrice.CurrencyCode,
        }).ToList() ?? new List<SalesOrderLineFormViewModel> { new() },
    };

    public CreateSalesOrderRequest ToCreateRequest() => new(
        CustomerId,
        OrderNumber,
        OrderDate,
        CurrencyCode,
        RequestedShipDate,
        Lines.Select(BuildLine).ToList());

    public UpdateSalesOrderRequest ToUpdateRequest() => new(
        OrderNumber,
        OrderDate,
        RequestedShipDate,
        CurrencyCode,
        Lines.Select(BuildLine).ToList());

    private static CreateSalesOrderLineDto BuildLine(SalesOrderLineFormViewModel l) => new(
        l.ItemId,
        l.Description,
        new QuantityDto(l.QuantityValue, l.QuantityUnitOfMeasure),
        new MoneyDto(l.UnitPriceAmount, l.UnitPriceCurrencyCode));
}
