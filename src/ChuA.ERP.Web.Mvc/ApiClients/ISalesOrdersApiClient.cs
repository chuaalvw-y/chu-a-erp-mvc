// Copyright (c) 2026 Alvin Wilsen Chan Chua
// GitHub: chuaalvw-y
// Licensed under the Alvin Wilsen Chan Chua Proprietary Use-Only License.
// See LICENSE.txt in the project root for full license information.

using ChuA.ERP.Web.Mvc.Contracts.Common;
using ChuA.ERP.Web.Mvc.Contracts.Dtos;

namespace ChuA.ERP.Web.Mvc.ApiClients;

/// <summary>Calls /api/v1/sales-orders.</summary>
public interface ISalesOrdersApiClient
{
    /// <summary>Lists sales orders, optionally filtered by customer, status and search text.</summary>
    Task<Result<PagedResult<SalesOrderDto>>> ListAsync(Guid? customerId = null, string? status = null, string? search = null, int pageNumber = 1, int pageSize = 25, string? sort = null, CancellationToken cancellationToken = default);

    /// <summary>Gets a single sales order by id.</summary>
    Task<Result<SalesOrderDto>> GetAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Creates a new sales order.</summary>
    Task<Result<SalesOrderDto>> CreateAsync(CreateSalesOrderRequest request, CancellationToken cancellationToken = default);

    /// <summary>Updates an existing sales order.</summary>
    Task<Result<SalesOrderDto>> UpdateAsync(Guid id, UpdateSalesOrderRequest request, CancellationToken cancellationToken = default);

    /// <summary>Ships a sales order and returns the shipment id.</summary>
    Task<Result<Guid>> ShipAsync(Guid id, ShipSalesOrderRequest request, CancellationToken cancellationToken = default);

    /// <summary>Deletes a sales order by id.</summary>
    Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
