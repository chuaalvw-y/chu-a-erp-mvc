// Copyright (c) 2026 Alvin Wilsen Chan Chua
// GitHub: chuaalvw-y
// Licensed under the Alvin Wilsen Chan Chua Proprietary Use-Only License.
// See LICENSE.txt in the project root for full license information.

using ChuA.ERP.Web.Mvc.Contracts.Common;
using ChuA.ERP.Web.Mvc.Contracts.Dtos;
using ChuA.ERP.Web.Mvc.Services;
using Microsoft.Extensions.Logging;

namespace ChuA.ERP.Web.Mvc.ApiClients;

/// <inheritdoc cref="ISalesOrdersApiClient"/>
public sealed class SalesOrdersApiClient : ApiClientBase, ISalesOrdersApiClient
{
    public SalesOrdersApiClient(
        HttpClient httpClient,
        ITokenAcquisitionService tokenAcquisition,
        ICorrelationIdAccessor correlationIds,
        ILogger<SalesOrdersApiClient> logger)
        : base(httpClient, tokenAcquisition, correlationIds, logger)
    {
    }

    public Task<Result<PagedResult<SalesOrderDto>>> ListAsync(Guid? customerId = null, string? status = null, string? search = null, int pageNumber = 1, int pageSize = 25, string? sort = null, CancellationToken cancellationToken = default) =>
        SendPagedAsync<SalesOrderDto>(
            "v1/sales-orders" + QueryString(("customerId", customerId), ("status", status), ("search", search), ("pageNumber", pageNumber), ("pageSize", pageSize), ("sort", sort)),
            pageNumber,
            pageSize,
            cancellationToken);

    public Task<Result<SalesOrderDto>> GetAsync(Guid id, CancellationToken cancellationToken = default) =>
        SendAsync<SalesOrderDto>(HttpMethod.Get, $"v1/sales-orders/{id}", cancellationToken: cancellationToken);

    public Task<Result<SalesOrderDto>> CreateAsync(CreateSalesOrderRequest request, CancellationToken cancellationToken = default) =>
        SendAsync<SalesOrderDto>(HttpMethod.Post, "v1/sales-orders", request, cancellationToken);

    public Task<Result<SalesOrderDto>> UpdateAsync(Guid id, UpdateSalesOrderRequest request, CancellationToken cancellationToken = default) =>
        SendAsync<SalesOrderDto>(HttpMethod.Put, $"v1/sales-orders/{id}", request, cancellationToken);

    public Task<Result<Guid>> ShipAsync(Guid id, ShipSalesOrderRequest request, CancellationToken cancellationToken = default) =>
        SendAsync<Guid>(HttpMethod.Post, $"v1/sales-orders/{id}/ship", request, cancellationToken);

    public Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default) =>
        SendAsync(HttpMethod.Delete, $"v1/sales-orders/{id}", cancellationToken: cancellationToken);
}
