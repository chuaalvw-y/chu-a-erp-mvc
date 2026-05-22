// Copyright (c) 2026 Alvin Wilsen Chan Chua
// GitHub: chuaalvw-y
// Licensed under the Alvin Wilsen Chan Chua Proprietary Use-Only License.
// See LICENSE.txt in the project root for full license information.

namespace ChuA.ERP.Web.Mvc.Contracts.Dtos;

public sealed record PurchaseOrderDto(
    Guid Id,
    Guid CompanyId,
    Guid VendorId,
    string OrderNumber,
    DateOnly OrderDate,
    string CurrencyCode,
    string Status,
    MoneyDto TotalAmount,
    DateOnly? ExpectedDeliveryDate = null,
    IReadOnlyCollection<PurchaseOrderLineDto>? Lines = null);

public sealed record PurchaseOrderLineDto(
    Guid Id,
    Guid ItemId,
    string Description,
    QuantityDto OrderedQuantity,
    QuantityDto ReceivedQuantity,
    MoneyDto UnitPrice,
    MoneyDto LineTotal);

public sealed record CreatePurchaseOrderLineDto(
    Guid ItemId,
    string Description,
    QuantityDto Quantity,
    MoneyDto UnitPrice);

public sealed record CreatePurchaseOrderRequest(
    Guid VendorId,
    string OrderNumber,
    DateOnly OrderDate,
    string CurrencyCode,
    DateOnly? ExpectedDeliveryDate,
    IReadOnlyList<CreatePurchaseOrderLineDto> Lines);

public sealed record UpdatePurchaseOrderRequest(
    string OrderNumber,
    DateOnly OrderDate,
    DateOnly? ExpectedDeliveryDate,
    string CurrencyCode,
    IReadOnlyList<CreatePurchaseOrderLineDto> Lines);

public sealed record ReceiveGoodsLineDto(
    Guid PurchaseOrderLineId,
    Guid ItemId,
    QuantityDto Received,
    MoneyDto? UnitCost);

public sealed record ReceiveGoodsRequest(
    DateOnly ReceiptDate,
    Guid WarehouseId,
    string? ReceiptNumber,
    IReadOnlyList<ReceiveGoodsLineDto> Lines);
