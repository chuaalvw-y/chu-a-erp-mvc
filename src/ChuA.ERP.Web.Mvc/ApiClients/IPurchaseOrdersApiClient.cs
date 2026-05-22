// Copyright (c) 2026 Alvin Wilsen Chan Chua
// GitHub: chuaalvw-y
// Licensed under the Alvin Wilsen Chan Chua Proprietary Use-Only License.
// See LICENSE.txt in the project root for full license information.

using ChuA.ERP.Web.Mvc.Contracts.Common;
using ChuA.ERP.Web.Mvc.Contracts.Dtos;

namespace ChuA.ERP.Web.Mvc.ApiClients;

/// <summary>Calls /api/v1/purchase-orders.</summary>
public interface IPurchaseOrdersApiClient
{
    /// <summary>Lists purchase orders, optionally filtered by vendor, status and search text.</summary>
    Task<Result<PagedResult<PurchaseOrderDto>>> ListAsync(Guid? vendorId = null, string? status = null, string? search = null, int pageNumber = 1, int pageSize = 25, string? sort = null, CancellationToken cancellationToken = default);

    /// <summary>Gets a single purchase order by id.</summary>
    Task<Result<PurchaseOrderDto>> GetAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Creates a new purchase order.</summary>
    Task<Result<PurchaseOrderDto>> CreateAsync(CreatePurchaseOrderRequest request, CancellationToken cancellationToken = default);

    /// <summary>Updates an existing purchase order.</summary>
    Task<Result<PurchaseOrderDto>> UpdateAsync(Guid id, UpdatePurchaseOrderRequest request, CancellationToken cancellationToken = default);

    /// <summary>Approves a purchase order.</summary>
    Task<Result<PurchaseOrderDto>> ApproveAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Records a goods receipt against a purchase order and returns the receipt id.</summary>
    Task<Result<Guid>> ReceiveAsync(Guid id, ReceiveGoodsRequest request, CancellationToken cancellationToken = default);

    /// <summary>Deletes a purchase order by id.</summary>
    Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
