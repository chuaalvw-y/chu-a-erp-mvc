// Copyright (c) 2026 Alvin Wilsen Chan Chua
// GitHub: chuaalvw-y
// Licensed under the Alvin Wilsen Chan Chua Proprietary Use-Only License.
// See LICENSE.txt in the project root for full license information.

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
    MoneyDto OutstandingBalance,
    IReadOnlyCollection<BillLineDto>? Lines = null);

public sealed record BillLineDto(
    Guid Id,
    string Description,
    QuantityDto Quantity,
    MoneyDto UnitPrice,
    MoneyDto LineTotal,
    Guid? ExpenseAccountId);

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
