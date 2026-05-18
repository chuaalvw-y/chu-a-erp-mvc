namespace ChuA.ERP.Web.Mvc.Contracts.Dtos;

public sealed record InvoiceDto(
    Guid Id,
    Guid CompanyId,
    Guid CustomerId,
    string InvoiceNumber,
    DateOnly InvoiceDate,
    DateOnly DueDate,
    string CurrencyCode,
    string Status,
    string PaymentStatus,
    MoneyDto TotalAmount,
    MoneyDto OutstandingBalance);

public sealed record CreateInvoiceLineDto(
    string Description,
    QuantityDto Quantity,
    MoneyDto UnitPrice,
    Guid? RevenueAccountId = null);

public sealed record CreateInvoiceRequest(
    Guid CustomerId,
    string InvoiceNumber,
    DateOnly InvoiceDate,
    DateOnly DueDate,
    string CurrencyCode,
    IReadOnlyList<CreateInvoiceLineDto> Lines);

public sealed record UpdateInvoiceRequest(
    string InvoiceNumber,
    DateOnly InvoiceDate,
    DateOnly DueDate,
    string CurrencyCode,
    IReadOnlyList<CreateInvoiceLineDto> Lines);

public sealed record ApplyCustomerPaymentRequest(
    DateOnly PaymentDate,
    MoneyDto Amount,
    string PaymentMethod,
    string? Reference);
