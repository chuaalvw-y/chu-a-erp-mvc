using ChuA.ERP.Web.Mvc.Contracts.Common;
using ChuA.ERP.Web.Mvc.Contracts.Dtos;
using ChuA.ERP.Web.Mvc.Services;
using Microsoft.Extensions.Logging;

namespace ChuA.ERP.Web.Mvc.ApiClients;

/// <inheritdoc cref="IWorkflowApiClient"/>
public sealed class WorkflowApiClient : ApiClientBase, IWorkflowApiClient
{
    public WorkflowApiClient(
        HttpClient httpClient,
        ITokenAcquisitionService tokenAcquisition,
        ICorrelationIdAccessor correlationIds,
        ILogger<WorkflowApiClient> logger)
        : base(httpClient, tokenAcquisition, correlationIds, logger)
    {
    }

    public Task<Result<IReadOnlyList<ApprovalRequestDto>>> ListTasksAsync(string? status = null, string? subjectType = null, CancellationToken cancellationToken = default) =>
        SendAsync<IReadOnlyList<ApprovalRequestDto>>(
            HttpMethod.Get,
            "v1/workflow/tasks" + QueryString(("status", status), ("subjectType", subjectType)),
            cancellationToken: cancellationToken);

    public Task<Result<ApprovalRequestDto>> GetTaskAsync(Guid id, CancellationToken cancellationToken = default) =>
        SendAsync<ApprovalRequestDto>(HttpMethod.Get, $"v1/workflow/tasks/{id}", cancellationToken: cancellationToken);

    public Task<Result> SubmitApprovalAsync(Guid id, SubmitWorkflowApprovalRequest request, CancellationToken cancellationToken = default) =>
        SendAsync(HttpMethod.Post, $"v1/workflow/approvals/{id}/submit", request, cancellationToken);

    public Task<Result> ReassignTaskAsync(Guid id, ReassignWorkflowTaskRequest request, CancellationToken cancellationToken = default) =>
        SendAsync(HttpMethod.Post, $"v1/workflow/tasks/{id}/reassign", request, cancellationToken);
}
