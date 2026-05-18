namespace ChuA.ERP.Web.Mvc.Contracts.Dtos;

public sealed record BillDto(
    Guid Id,
    Guid CompanyId,
    Guid VendorId,
    string BillNumber,
    DateOnly BillDate,
    DateOnly DueDate,
    string CurrencyCode,
    string Status,
    string PaymentStatus,
    MoneyDto TotalAmount,
    MoneyDto OutstandingBalance);

public sealed record CreateBillLineDto(
    string Description,
    QuantityDto Quantity,
    MoneyDto UnitPrice,
    Guid? ExpenseAccountId = null);

public sealed record CreateBillRequest(
    Guid VendorId,
    string BillNumber,
    DateOnly BillDate,
    DateOnly DueDate,
    string CurrencyCode,
    IReadOnlyList<CreateBillLineDto> Lines);

public sealed record UpdateBillRequest(
    string BillNumber,
    DateOnly BillDate,
    DateOnly DueDate,
    string CurrencyCode,
    IReadOnlyList<CreateBillLineDto> Lines);

public sealed record PayBillRequest(
    DateOnly PaymentDate,
    MoneyDto Amount,
    string PaymentMethod,
    string? Reference);
