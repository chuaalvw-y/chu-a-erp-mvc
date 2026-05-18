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

    [Required, StringLength(3, MinimumLength = 3)]
    [Display(Name = "Currency")]
    public string CurrencyCode { get; set; } = "USD";

    [Display(Name = "Lines")]
    public List<SalesOrderLineFormViewModel> Lines { get; set; } = new();

    public bool IsEdit => Id.HasValue;

    public static SalesOrderFormViewModel FromDto(SalesOrderDto dto) => new()
    {
        Id = dto.Id,
        CustomerId = dto.CustomerId,
        OrderNumber = dto.OrderNumber,
        OrderDate = dto.OrderDate,
        CurrencyCode = dto.CurrencyCode,
        Lines = new List<SalesOrderLineFormViewModel>(),
    };

    public CreateSalesOrderRequest ToCreateRequest() => new(
        CustomerId,
        OrderNumber,
        OrderDate,
        CurrencyCode,
        Lines.Select(BuildLine).ToList());

    public UpdateSalesOrderRequest ToUpdateRequest() => new(
        OrderNumber,
        OrderDate,
        CurrencyCode,
        Lines.Select(BuildLine).ToList());

    private static CreateSalesOrderLineDto BuildLine(SalesOrderLineFormViewModel l) => new(
        l.ItemId,
        l.Description,
        new QuantityDto(l.QuantityValue, l.QuantityUnitOfMeasure),
        new MoneyDto(l.UnitPriceAmount, l.UnitPriceCurrencyCode));
}
