// Copyright (c) 2026 Alvin Wilsen Chan Chua
// GitHub: chuaalvw-y
// Licensed under the Alvin Wilsen Chan Chua Proprietary Use-Only License.
// See LICENSE.txt in the project root for full license information.

using ChuA.ERP.Web.Mvc.Contracts.Common;
using ChuA.ERP.Web.Mvc.Contracts.Dtos;

namespace ChuA.ERP.Web.Mvc.ApiClients;

/// <summary>Calls /api/v1/companies.</summary>
public interface ICompaniesApiClient
{
    /// <summary>Lists companies, optionally filtered by free-text search.</summary>
    Task<Result<PagedResult<CompanyDto>>> ListAsync(string? search = null, int pageNumber = 1, int pageSize = 25, string? sort = null, CancellationToken cancellationToken = default);

    /// <summary>Gets a single company by id.</summary>
    Task<Result<CompanyDto>> GetAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Creates a new company.</summary>
    Task<Result<CompanyDto>> CreateAsync(CreateCompanyRequest request, CancellationToken cancellationToken = default);

    /// <summary>Updates an existing company.</summary>
    Task<Result<CompanyDto>> UpdateAsync(Guid id, UpdateCompanyRequest request, CancellationToken cancellationToken = default);

    /// <summary>Deletes a company by id.</summary>
    Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
