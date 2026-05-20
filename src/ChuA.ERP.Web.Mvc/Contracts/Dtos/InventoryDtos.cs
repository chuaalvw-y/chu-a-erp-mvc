// Copyright (c) 2026 Alvin Wilsen Chan Chua
// GitHub: chuaalvw-y
// Licensed under the Alvin Wilsen Chan Chua Proprietary Use-Only License.
// See LICENSE.txt in the project root for full license information.

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
