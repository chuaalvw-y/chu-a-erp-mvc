// Copyright (c) 2026 Alvin Wilsen Chan Chua
// GitHub: chuaalvw-y
// Licensed under the Alvin Wilsen Chan Chua Proprietary Use-Only License.
// See LICENSE.txt in the project root for full license information.

using ChuA.ERP.Web.Mvc.Contracts.Common;
using ChuA.ERP.Web.Mvc.Contracts.Dtos;

namespace ChuA.ERP.Web.Mvc.ApiClients;

/// <summary>Calls /api/v1/bills.</summary>
public interface IBillsApiClient
{
    /// <summary>Lists bills, optionally filtered by vendor, status, payment status and search text.</summary>
    Task<Result<PagedResult<BillDto>>> ListAsync(Guid? vendorId = null, string? status = null, string? paymentStatus = null, string? search = null, int pageNumber = 1, int pageSize = 25, string? sort = null, CancellationToken cancellationToken = default);

    /// <summary>Lists bills currently awaiting approval.</summary>
    Task<Result<PagedResult<BillDto>>> GetAwaitingApprovalAsync(int pageNumber = 1, int pageSize = 25, string? sort = null, CancellationToken cancellationToken = default);

    /// <summary>Gets a single bill by id.</summary>
    Task<Result<BillDto>> GetAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Creates a new bill.</summary>
    Task<Result<BillDto>> CreateAsync(CreateBillRequest request, CancellationToken cancellationToken = default);

    /// <summary>Updates an existing bill.</summary>
    Task<Result<BillDto>> UpdateAsync(Guid id, UpdateBillRequest request, CancellationToken cancellationToken = default);

    /// <summary>Approves a bill.</summary>
    Task<Result<BillDto>> ApproveAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Records a payment against a bill.</summary>
    Task<Result<BillDto>> PayAsync(Guid id, PayBillRequest request, CancellationToken cancellationToken = default);

    /// <summary>Deletes a bill by id.</summary>
    Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
