using ChuA.ERP.Web.Mvc.Contracts.Common;
using ChuA.ERP.Web.Mvc.Contracts.Dtos;

namespace ChuA.ERP.Web.Mvc.ApiClients;

/// <summary>Calls /api/v1/workflow.</summary>
public interface IWorkflowApiClient
{
    /// <summary>Lists workflow tasks, optionally filtered by status and subject type.</summary>
    Task<Result<PagedResult<ApprovalRequestDto>>> ListTasksAsync(string? status = null, string? subjectType = null, int pageNumber = 1, int pageSize = 25, string? sort = null, CancellationToken cancellationToken = default);

    /// <summary>Gets a single workflow task by id.</summary>
    Task<Result<ApprovalRequestDto>> GetTaskAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Submits an approval decision for a workflow approval.</summary>
    Task<Result> SubmitApprovalAsync(Guid id, SubmitWorkflowApprovalRequest request, CancellationToken cancellationToken = default);

    /// <summary>Reassigns a workflow task to another user.</summary>
    Task<Result> ReassignTaskAsync(Guid id, ReassignWorkflowTaskRequest request, CancellationToken cancellationToken = default);
}
