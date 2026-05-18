namespace ChuA.ERP.Web.Mvc.Contracts.Dtos;

public sealed record SalesOrderDto(
    Guid Id,
    Guid CompanyId,
    Guid CustomerId,
    string OrderNumber,
    DateOnly OrderDate,
    string CurrencyCode,
    string Status,
    MoneyDto TotalAmount);

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
    IReadOnlyList<CreateSalesOrderLineDto> Lines);

public sealed record UpdateSalesOrderRequest(
    string OrderNumber,
    DateOnly OrderDate,
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
