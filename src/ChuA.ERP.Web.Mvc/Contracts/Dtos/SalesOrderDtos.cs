// Copyright (c) 2026 Alvin Wilsen Chan Chua
// GitHub: chuaalvw-y
// Licensed under the Alvin Wilsen Chan Chua Proprietary Use-Only License.
// See LICENSE.txt in the project root for full license information.

namespace ChuA.ERP.Web.Mvc.Contracts.Dtos;

public sealed record SalesOrderDto(
    Guid Id,
    Guid CompanyId,
    Guid CustomerId,
    string OrderNumber,
    DateOnly OrderDate,
    string CurrencyCode,
    string Status,
    MoneyDto TotalAmount,
    DateOnly? RequestedShipDate = null,
    IReadOnlyCollection<SalesOrderLineDto>? Lines = null);

public sealed record SalesOrderLineDto(
    Guid Id,
    Guid ItemId,
    string Description,
    QuantityDto OrderedQuantity,
    QuantityDto ShippedQuantity,
    MoneyDto UnitPrice,
    MoneyDto LineTotal);

public sealed record CreateSalesOrderLineDto(
    Guid ItemId,
    string Description,
    QuantityDto Quantity,
    MoneyDto UnitPrice);

public sealed record CreateSalesOrderRequest(
    Guid CustomerId,
    string OrderNumber,
    DateOnly OrderDate,
    string CurrencyCode,
    DateOnly? RequestedShipDate,
    IReadOnlyList<CreateSalesOrderLineDto> Lines);

public sealed record UpdateSalesOrderRequest(
    string OrderNumber,
    DateOnly OrderDate,
    DateOnly? RequestedShipDate,
    string CurrencyCode,
    IReadOnlyList<CreateSalesOrderLineDto> Lines);

public sealed record ShipSalesOrderLineDto(
    Guid SalesOrderLineId,
    Guid ItemId,
    QuantityDto Shipped);

public sealed record ShipSalesOrderRequest(
    DateOnly ShipmentDate,
    Guid WarehouseId,
    string? ShipmentNumber,
    string? Carrier,
    string? TrackingNumber,
    IReadOnlyList<ShipSalesOrderLineDto> Lines);
