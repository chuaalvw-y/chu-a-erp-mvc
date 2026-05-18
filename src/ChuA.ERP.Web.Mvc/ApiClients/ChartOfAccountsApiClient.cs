using ChuA.ERP.Web.Mvc.Contracts.Common;
using ChuA.ERP.Web.Mvc.Contracts.Dtos;
using ChuA.ERP.Web.Mvc.Services;
using Microsoft.Extensions.Logging;

namespace ChuA.ERP.Web.Mvc.ApiClients;

/// <inheritdoc cref="IChartOfAccountsApiClient"/>
public sealed class ChartOfAccountsApiClient : ApiClientBase, IChartOfAccountsApiClient
{
    public ChartOfAccountsApiClient(
        HttpClient httpClient,
        ITokenAcquisitionService tokenAcquisition,
        ICorrelationIdAccessor correlationIds,
        ILogger<ChartOfAccountsApiClient> logger)
        : base(httpClient, tokenAcquisition, correlationIds, logger)
    {
    }

    public Task<Result<IReadOnlyList<ChartOfAccountDto>>> ListAsync(string? accountType = null, string? search = null, CancellationToken cancellationToken = default) =>
        SendAsync<IReadOnlyList<ChartOfAccountDto>>(
            HttpMethod.Get,
            "v1/chart-of-accounts" + QueryString(("accountType", accountType), ("search", search)),
            cancellationToken: cancellationToken);

    public Task<Result<ChartOfAccountDto>> GetAsync(Guid id, CancellationToken cancellationToken = default) =>
        SendAsync<ChartOfAccountDto>(HttpMethod.Get, $"v1/chart-of-accounts/{id}", cancellationToken: cancellationToken);

    public Task<Result<Guid>> CreateAsync(CreateChartOfAccountRequest request, CancellationToken cancellationToken = default) =>
        SendAsync<Guid>(HttpMethod.Post, "v1/chart-of-accounts", request, cancellationToken);

    public Task<Result<ChartOfAccountDto>> UpdateAsync(Guid id, UpdateChartOfAccountRequest request, CancellationToken cancellationToken = default) =>
        SendAsync<ChartOfAccountDto>(HttpMethod.Put, $"v1/chart-of-accounts/{id}", request, cancellationToken);

    public Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default) =>
        SendAsync(HttpMethod.Delete, $"v1/chart-of-accounts/{id}", cancellationToken: cancellationToken);
}
