using ChuA.ERP.Web.Mvc.Contracts.Common;
using ChuA.ERP.Web.Mvc.Contracts.Dtos;
using ChuA.ERP.Web.Mvc.Services;
using Microsoft.Extensions.Logging;

namespace ChuA.ERP.Web.Mvc.ApiClients;

/// <inheritdoc cref="IReportsApiClient"/>
public sealed class ReportsApiClient : ApiClientBase, IReportsApiClient
{
    public ReportsApiClient(
        HttpClient httpClient,
        ITokenAcquisitionService tokenAcquisition,
        ICorrelationIdAccessor correlationIds,
        ILogger<ReportsApiClient> logger)
        : base(httpClient, tokenAcquisition, correlationIds, logger)
    {
    }

    public Task<Result<IReadOnlyList<ReportSummaryDto>>> ListAsync(CancellationToken cancellationToken = default) =>
        SendAsync<IReadOnlyList<ReportSummaryDto>>(HttpMethod.Get, "v1/reports", cancellationToken: cancellationToken);

    public Task<Result<IReadOnlyList<OpenDocumentRow>>> RunAsync(string code, object? parameters = null, CancellationToken cancellationToken = default) =>
        SendAsync<IReadOnlyList<OpenDocumentRow>>(HttpMethod.Post, $"v1/reports/{Uri.EscapeDataString(code)}/run", parameters, cancellationToken);
}
