// Copyright (c) 2026 Alvin Wilsen Chan Chua
// GitHub: chuaalvw-y
// Licensed under the Alvin Wilsen Chan Chua Proprietary Use-Only License.
// See LICENSE.txt in the project root for full license information.

using ChuA.ERP.Web.Mvc.Contracts.Common;
using ChuA.ERP.Web.Mvc.Contracts.Dtos;

namespace ChuA.ERP.Web.Mvc.ApiClients;

/// <summary>Calls /api/v1/journal-entries.</summary>
public interface IJournalEntriesApiClient
{
    /// <summary>Lists journal entries, optionally filtered by fiscal period and status.</summary>
    Task<Result<PagedResult<JournalEntryDto>>> ListAsync(Guid? fiscalPeriodId = null, string? status = null, int pageNumber = 1, int pageSize = 25, string? sort = null, CancellationToken cancellationToken = default);

    /// <summary>Gets a single journal entry by id.</summary>
    Task<Result<JournalEntryDto>> GetAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Creates a draft journal entry and returns its identifier.</summary>
    Task<Result<Guid>> CreateDraftAsync(PostJournalEntryRequest request, CancellationToken cancellationToken = default);

    /// <summary>Creates and immediately posts a journal entry, returning its identifier.</summary>
    Task<Result<Guid>> CreateAndPostAsync(PostJournalEntryRequest request, CancellationToken cancellationToken = default);

    /// <summary>Posts a previously created draft journal entry.</summary>
    Task<Result> PostExistingAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Updates an existing journal entry.</summary>
    Task<Result<JournalEntryDto>> UpdateAsync(Guid id, UpdateJournalEntryRequest request, CancellationToken cancellationToken = default);

    /// <summary>Deletes a journal entry by id.</summary>
    Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
