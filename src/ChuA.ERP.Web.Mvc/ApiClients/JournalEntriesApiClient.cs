// Copyright (c) 2026 Alvin Wilsen Chan Chua
// GitHub: chuaalvw-y
// Licensed under the Alvin Wilsen Chan Chua Proprietary Use-Only License.
// See LICENSE.txt in the project root for full license information.

using ChuA.ERP.Web.Mvc.Contracts.Common;
using ChuA.ERP.Web.Mvc.Contracts.Dtos;
using ChuA.ERP.Web.Mvc.Services;
using Microsoft.Extensions.Logging;

namespace ChuA.ERP.Web.Mvc.ApiClients;

/// <inheritdoc cref="IJournalEntriesApiClient"/>
public sealed class JournalEntriesApiClient : ApiClientBase, IJournalEntriesApiClient
{
    public JournalEntriesApiClient(
        HttpClient httpClient,
        ITokenAcquisitionService tokenAcquisition,
        ICorrelationIdAccessor correlationIds,
        ILogger<JournalEntriesApiClient> logger)
        : base(httpClient, tokenAcquisition, correlationIds, logger)
    {
    }

    public Task<Result<PagedResult<JournalEntryDto>>> ListAsync(Guid? fiscalPeriodId = null, string? status = null, int pageNumber = 1, int pageSize = 25, string? sort = null, CancellationToken cancellationToken = default) =>
        SendPagedAsync<JournalEntryDto>(
            "v1/journal-entries" + QueryString(("fiscalPeriodId", fiscalPeriodId), ("status", status), ("pageNumber", pageNumber), ("pageSize", pageSize), ("sort", sort)),
            pageNumber,
            pageSize,
            cancellationToken);

    public Task<Result<JournalEntryDto>> GetAsync(Guid id, CancellationToken cancellationToken = default) =>
        SendAsync<JournalEntryDto>(HttpMethod.Get, $"v1/journal-entries/{id}", cancellationToken: cancellationToken);

    public Task<Result<Guid>> CreateDraftAsync(PostJournalEntryRequest request, CancellationToken cancellationToken = default) =>
        SendAsync<Guid>(HttpMethod.Post, "v1/journal-entries", request, cancellationToken);

    public Task<Result<Guid>> CreateAndPostAsync(PostJournalEntryRequest request, CancellationToken cancellationToken = default) =>
        SendAsync<Guid>(HttpMethod.Post, "v1/journal-entries/post", request, cancellationToken);

    public Task<Result> PostExistingAsync(Guid id, CancellationToken cancellationToken = default) =>
        SendAsync(HttpMethod.Post, $"v1/journal-entries/{id}/post", cancellationToken: cancellationToken);

    public Task<Result<JournalEntryDto>> UpdateAsync(Guid id, UpdateJournalEntryRequest request, CancellationToken cancellationToken = default) =>
        SendAsync<JournalEntryDto>(HttpMethod.Put, $"v1/journal-entries/{id}", request, cancellationToken);

    public Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default) =>
        SendAsync(HttpMethod.Delete, $"v1/journal-entries/{id}", cancellationToken: cancellationToken);
}
