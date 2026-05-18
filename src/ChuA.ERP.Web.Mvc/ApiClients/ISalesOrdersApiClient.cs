using ChuA.ERP.Web.Mvc.Contracts.Common;
using ChuA.ERP.Web.Mvc.Contracts.Dtos;

namespace ChuA.ERP.Web.Mvc.ApiClients;

/// <summary>Calls /api/v1/sales-orders.</summary>
public interface ISalesOrdersApiClient
{
    /// <summary>Lists sales orders, optionally filtered by customer, status and search text.</summary>
    Task<Result<IReadOnlyList<SalesOrderDto>>> ListAsync(Guid? customerId = null, string? status = null, string? search = null, CancellationToken cancellationToken = default);

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
