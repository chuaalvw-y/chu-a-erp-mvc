using ChuA.ERP.Web.Mvc.Contracts.Common;
using ChuA.ERP.Web.Mvc.Contracts.Dtos;
using ChuA.ERP.Web.Mvc.Services;
using Microsoft.Extensions.Logging;

namespace ChuA.ERP.Web.Mvc.ApiClients;

/// <inheritdoc cref="IInventoryApiClient"/>
public sealed class InventoryApiClient : ApiClientBase, IInventoryApiClient
{
    public InventoryApiClient(
        HttpClient httpClient,
        ITokenAcquisitionService tokenAcquisition,
        ICorrelationIdAccessor correlationIds,
        ILogger<InventoryApiClient> logger)
        : base(httpClient, tokenAcquisition, correlationIds, logger)
    {
    }

    public Task<Result<IReadOnlyList<ItemDto>>> ListAsync(string? search = null, CancellationToken cancellationToken = default) =>
        SendAsync<IReadOnlyList<ItemDto>>(HttpMethod.Get, "v1/inventory" + QueryString(("search", search)), cancellationToken: cancellationToken);

    public Task<Result<ItemDto>> GetAsync(Guid id, CancellationToken cancellationToken = default) =>
        SendAsync<ItemDto>(HttpMethod.Get, $"v1/inventory/{id}", cancellationToken: cancellationToken);

    public Task<Result<InventoryBalanceDto>> GetBalanceAsync(Guid itemId, Guid warehouseId, CancellationToken cancellationToken = default) =>
        SendAsync<InventoryBalanceDto>(HttpMethod.Get, $"v1/inventory/{itemId}/warehouses/{warehouseId}/balance", cancellationToken: cancellationToken);

    public Task<Result<ItemDto>> CreateAsync(CreateItemRequest request, CancellationToken cancellationToken = default) =>
        SendAsync<ItemDto>(HttpMethod.Post, "v1/inventory", request, cancellationToken);

    public Task<Result<ItemDto>> UpdateAsync(Guid id, UpdateItemRequest request, CancellationToken cancellationToken = default) =>
        SendAsync<ItemDto>(HttpMethod.Put, $"v1/inventory/{id}", request, cancellationToken);

    public Task<Result> AdjustAsync(Guid id, AdjustInventoryRequest request, CancellationToken cancellationToken = default) =>
        SendAsync(HttpMethod.Post, $"v1/inventory/{id}/adjust", request, cancellationToken);

    public Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default) =>
        SendAsync(HttpMethod.Delete, $"v1/inventory/{id}", cancellationToken: cancellationToken);
}
