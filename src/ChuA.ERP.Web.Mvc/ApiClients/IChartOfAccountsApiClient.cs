using ChuA.ERP.Web.Mvc.Contracts.Common;
using ChuA.ERP.Web.Mvc.Contracts.Dtos;

namespace ChuA.ERP.Web.Mvc.ApiClients;

/// <summary>Calls /api/v1/chart-of-accounts.</summary>
public interface IChartOfAccountsApiClient
{
    /// <summary>Lists chart-of-accounts entries, optionally filtered by account type and search text.</summary>
    Task<Result<PagedResult<ChartOfAccountDto>>> ListAsync(string? accountType = null, string? search = null, int pageNumber = 1, int pageSize = 25, string? sort = null, CancellationToken cancellationToken = default);

    /// <summary>Gets a single chart-of-account by id.</summary>
    Task<Result<ChartOfAccountDto>> GetAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Creates a new chart-of-account and returns its identifier.</summary>
    Task<Result<Guid>> CreateAsync(CreateChartOfAccountRequest request, CancellationToken cancellationToken = default);

    /// <summary>Updates an existing chart-of-account.</summary>
    Task<Result<ChartOfAccountDto>> UpdateAsync(Guid id, UpdateChartOfAccountRequest request, CancellationToken cancellationToken = default);

    /// <summary>Deletes a chart-of-account by id.</summary>
    Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
