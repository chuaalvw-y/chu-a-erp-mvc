// Copyright (c) 2026 Alvin Wilsen Chan Chua
// GitHub: chuaalvw-y
// Licensed under the Alvin Wilsen Chan Chua Proprietary Use-Only License.
// See LICENSE.txt in the project root for full license information.

using ChuA.ERP.Web.Mvc.Contracts.Common;
using ChuA.ERP.Web.Mvc.Contracts.Dtos;

namespace ChuA.ERP.Web.Mvc.ApiClients;

/// <summary>Calls /api/v1/workflow.</summary>
public interface IWorkflowApiClient
{
    /// <summary>
    /// Lists the caller's pending workflow approvals. The API filters
    /// server-side by the authenticated user and doesn't accept status
    /// or subject-type filters today — all inbox-rows return Pending.
    /// </summary>
    Task<Result<IReadOnlyList<WorkflowApprovalDto>>> ListTasksAsync(
        CancellationToken cancellationToken = default);

    /// <summary>Gets a single workflow approval by id.</summary>
    Task<Result<WorkflowApprovalDto>> GetTaskAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Records an Approve/Reject decision on a pending approval. Replaces
    /// the legacy <c>/submit</c> route; the request body now carries the
    /// owning instance id alongside the decision.
    /// </summary>
    Task<Result> DecideAsync(
        Guid approvalId,
        WorkflowApprovalDecisionRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Admin operation: reassigns a pending approval to a different user
    /// without an approver decision. Maps to
    /// <c>POST /workflow/approvals/{approvalId}/reassign</c>.
    /// </summary>
    Task<Result> ReassignAsync(
        Guid approvalId,
        WorkflowReassignRequest request,
        CancellationToken cancellationToken = default);
}
