// Copyright (c) 2026 Alvin Wilsen Chan Chua
// GitHub: chuaalvw-y
// Licensed under the Alvin Wilsen Chan Chua Proprietary Use-Only License.
// See LICENSE.txt in the project root for full license information.

using ChuA.ERP.Web.Mvc.Contracts.Common;
using ChuA.ERP.Web.Mvc.Contracts.Dtos;
using ChuA.ERP.Web.Mvc.Services;
using Microsoft.Extensions.Logging;

namespace ChuA.ERP.Web.Mvc.ApiClients;

/// <inheritdoc cref="IHealthApiClient"/>
public sealed class HealthApiClient : ApiClientBase, IHealthApiClient
{
    public HealthApiClient(
        HttpClient httpClient,
        ITokenAcquisitionService tokenAcquisition,
        ICorrelationIdAccessor correlationIds,
        ILogger<HealthApiClient> logger)
        : base(httpClient, tokenAcquisition, correlationIds, logger)
    {
    }

    public Task<Result<HealthStatusDto>> GetHealthAsync(CancellationToken cancellationToken = default) =>
        SendAsync<HealthStatusDto>(HttpMethod.Get, "v1/health", cancellationToken: cancellationToken);

    public Task<Result> GetLiveAsync(CancellationToken cancellationToken = default) =>
        SendAsync(HttpMethod.Get, "../health/live", cancellationToken: cancellationToken);

    public Task<Result> GetReadyAsync(CancellationToken cancellationToken = default) =>
        SendAsync(HttpMethod.Get, "../health/ready", cancellationToken: cancellationToken);
}
