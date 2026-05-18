using ChuA.ERP.Web.Mvc.Contracts.Common;
using ChuA.ERP.Web.Mvc.Contracts.Dtos;

namespace ChuA.ERP.Web.Mvc.ApiClients;

/// <summary>Calls /api/v1/reports.</summary>
public interface IReportsApiClient
{
    /// <summary>Lists all reports available to the current user.</summary>
    Task<Result<IReadOnlyList<ReportSummaryDto>>> ListAsync(CancellationToken cancellationToken = default);

    /// <summary>Runs the named report with the supplied parameters and returns the open-document rows.</summary>
    Task<Result<IReadOnlyList<OpenDocumentRow>>> RunAsync(string code, object? parameters = null, CancellationToken cancellationToken = default);
}
