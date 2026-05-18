namespace ChuA.ERP.Web.Mvc.Contracts.Dtos;

public sealed record ItemDto(
    Guid Id,
    Guid CompanyId,
    string Sku,
    string Name,
    string DefaultUnitOfMeasure,
    bool IsStocked,
    decimal? ReorderPoint);

public sealed record InventoryBalanceDto(
    Guid ItemId,
    Guid WarehouseId,
    QuantityDto OnHand,
    QuantityDto Available);

public sealed record CreateItemRequest(
    string Sku,
    string Name,
    string DefaultUnitOfMeasure,
    bool IsStocked,
    decimal? ReorderPoint);

public sealed record UpdateItemRequest(
    string Name,
    string DefaultUnitOfMeasure,
    bool IsStocked,
    decimal? ReorderPoint);

public sealed record AdjustInventoryRequest(
    Guid WarehouseId,
    QuantityDto Delta,
    string Reason);
