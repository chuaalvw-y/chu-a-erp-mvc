// Copyright (c) 2026 Alvin Wilsen Chan Chua
// GitHub: chuaalvw-y
// Licensed under the Alvin Wilsen Chan Chua Proprietary Use-Only License.
// See LICENSE.txt in the project root for full license information.

using System.ComponentModel.DataAnnotations;
using ChuA.ERP.Web.Mvc.Contracts.Dtos;

namespace ChuA.ERP.Web.Mvc.ViewModels.Invoices;

/// <summary>Strongly typed form model shared by Create and Edit screens for Invoices.</summary>
public sealed class InvoiceFormViewModel
{
    public Guid? Id { get; set; }

    [Required]
    [Display(Name = "Customer")]
    public Guid CustomerId { get; set; }

    [Required, StringLength(60)]
    [Display(Name = "Invoice number")]
    public string InvoiceNumber { get; set; } = string.Empty;

    [Required, DataType(DataType.Date)]
    [Display(Name = "Invoice date")]
    public DateOnly InvoiceDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);

    [Required, DataType(DataType.Date)]
    [Display(Name = "Due date")]
    public DateOnly DueDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(30));

    [Required, StringLength(3, MinimumLength = 3)]
    [Display(Name = "Currency")]
    public string CurrencyCode { get; set; } = "USD";

    [Display(Name = "Lines")]
    public List<InvoiceLineFormViewModel> Lines { get; set; } = new() { new InvoiceLineFormViewModel() };

    public IReadOnlyList<CustomerDto> Customers { get; set; } = Array.Empty<CustomerDto>();

    public bool IsEdit => Id.HasValue;

    public static InvoiceFormViewModel FromDto(InvoiceDto dto) => new()
    {
        Id = dto.Id,
        CustomerId = dto.CustomerId,
        InvoiceNumber = dto.InvoiceNumber,
        InvoiceDate = dto.InvoiceDate,
        DueDate = dto.DueDate,
        CurrencyCode = dto.CurrencyCode,
        Lines = dto.Lines?.Select(l => new InvoiceLineFormViewModel
        {
            Description = l.Description,
            QuantityValue = l.Quantity.Value,
            QuantityUnitOfMeasure = l.Quantity.UnitOfMeasure,
            UnitPriceAmount = l.UnitPrice.Amount,
            UnitPriceCurrencyCode = l.UnitPrice.CurrencyCode,
            RevenueAccountId = l.RevenueAccountId,
        }).ToList() ?? new List<InvoiceLineFormViewModel> { new() },
    };

    private IReadOnlyList<CreateInvoiceLineDto> ToLineDtos() =>
        Lines.Select(l => new CreateInvoiceLineDto(
            l.Description,
            new QuantityDto(l.QuantityValue, l.QuantityUnitOfMeasure),
            new MoneyDto(l.UnitPriceAmount, l.UnitPriceCurrencyCode),
            l.RevenueAccountId)).ToArray();

    public CreateInvoiceRequest ToCreateRequest() =>
        new(CustomerId, InvoiceNumber, InvoiceDate, DueDate, CurrencyCode, ToLineDtos());

    public UpdateInvoiceRequest ToUpdateRequest() =>
        new(InvoiceNumber, InvoiceDate, DueDate, CurrencyCode, ToLineDtos());
}
