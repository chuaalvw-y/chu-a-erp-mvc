using ChuA.ERP.Web.Mvc.Contracts.Common;
using ChuA.ERP.Web.Mvc.Contracts.Dtos;
using ChuA.ERP.Web.Mvc.Services;
using Microsoft.Extensions.Logging;

namespace ChuA.ERP.Web.Mvc.ApiClients;

/// <inheritdoc cref="IPurchaseOrdersApiClient"/>
public sealed class PurchaseOrdersApiClient : ApiClientBase, IPurchaseOrdersApiClient
{
    public PurchaseOrdersApiClient(
        HttpClient httpClient,
        ITokenAcquisitionService tokenAcquisition,
        ICorrelationIdAccessor correlationIds,
        ILogger<PurchaseOrdersApiClient> logger)
        : base(httpClient, tokenAcquisition, correlationIds, logger)
    {
    }

    public Task<Result<PagedResult<PurchaseOrderDto>>> ListAsync(Guid? vendorId = null, string? status = null, string? search = null, int pageNumber = 1, int pageSize = 25, string? sort = null, CancellationToken cancellationToken = default) =>
        SendPagedAsync<PurchaseOrderDto>(
            "v1/purchase-orders" + QueryString(("vendorId", vendorId), ("status", status), ("search", search), ("pageNumber", pageNumber), ("pageSize", pageSize), ("sort", sort)),
            pageNumber,
            pageSize,
            cancellationToken);

    public Task<Result<PurchaseOrderDto>> GetAsync(Guid id, CancellationToken cancellationToken = default) =>
        SendAsync<PurchaseOrderDto>(HttpMethod.Get, $"v1/purchase-orders/{id}", cancellationToken: cancellationToken);

    public Task<Result<PurchaseOrderDto>> CreateAsync(CreatePurchaseOrderRequest request, CancellationToken cancellationToken = default) =>
        SendAsync<PurchaseOrderDto>(HttpMethod.Post, "v1/purchase-orders", request, cancellationToken);

    public Task<Result<PurchaseOrderDto>> UpdateAsync(Guid id, UpdatePurchaseOrderRequest request, CancellationToken cancellationToken = default) =>
        SendAsync<PurchaseOrderDto>(HttpMethod.Put, $"v1/purchase-orders/{id}", request, cancellationToken);

    public Task<Result<PurchaseOrderDto>> ApproveAsync(Guid id, CancellationToken cancellationToken = default) =>
        SendAsync<PurchaseOrderDto>(HttpMethod.Post, $"v1/purchase-orders/{id}/approve", cancellationToken: cancellationToken);

    public Task<Result<Guid>> ReceiveAsync(Guid id, ReceiveGoodsRequest request, CancellationToken cancellationToken = default) =>
        SendAsync<Guid>(HttpMethod.Post, $"v1/purchase-orders/{id}/receipts", request, cancellationToken);

    public Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default) =>
        SendAsync(HttpMethod.Delete, $"v1/purchase-orders/{id}", cancellationToken: cancellationToken);
}
