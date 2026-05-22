// Copyright (c) 2026 Alvin Wilsen Chan Chua
// GitHub: chuaalvw-y
// Licensed under the Alvin Wilsen Chan Chua Proprietary Use-Only License.
// See LICENSE.txt in the project root for full license information.

using ChuA.ERP.Web.Mvc.Contracts.Common;
using ChuA.ERP.Web.Mvc.Contracts.Dtos;

namespace ChuA.ERP.Web.Mvc.ApiClients;

/// <summary>Calls /api/v1/inventory.</summary>
public interface IInventoryApiClient
{
    /// <summary>Lists items, optionally filtered by free-text search.</summary>
    Task<Result<PagedResult<ItemDto>>> ListAsync(string? search = null, int pageNumber = 1, int pageSize = 25, string? sort = null, CancellationToken cancellationToken = default);

    /// <summary>Gets a single item by id.</summary>
    Task<Result<ItemDto>> GetAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Gets the on-hand and available balance for an item in a warehouse.</summary>
    Task<Result<InventoryBalanceDto>> GetBalanceAsync(Guid itemId, Guid warehouseId, CancellationToken cancellationToken = default);

    /// <summary>Creates a new item.</summary>
    Task<Result<ItemDto>> CreateAsync(CreateItemRequest request, CancellationToken cancellationToken = default);

    /// <summary>Updates an existing item.</summary>
    Task<Result<ItemDto>> UpdateAsync(Guid id, UpdateItemRequest request, CancellationToken cancellationToken = default);

    /// <summary>Records a manual inventory adjustment for an item.</summary>
    Task<Result> AdjustAsync(Guid id, AdjustInventoryRequest request, CancellationToken cancellationToken = default);

    /// <summary>Deletes an item by id.</summary>
    Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
