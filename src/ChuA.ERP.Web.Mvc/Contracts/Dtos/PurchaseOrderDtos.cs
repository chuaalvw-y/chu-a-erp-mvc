namespace ChuA.ERP.Web.Mvc.Contracts.Dtos;

public sealed record PurchaseOrderDto(
    Guid Id,
    Guid CompanyId,
    Guid VendorId,
    string OrderNumber,
    DateOnly OrderDate,
    string CurrencyCode,
    string Status,
    MoneyDto TotalAmount);

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
    IReadOnlyList<CreatePurchaseOrderLineDto> Lines);

public sealed record UpdatePurchaseOrderRequest(
    string OrderNumber,
    DateOnly OrderDate,
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
