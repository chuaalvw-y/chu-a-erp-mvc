// Copyright (c) 2026 Alvin Wilsen Chan Chua
// GitHub: chuaalvw-y
// Licensed under the Alvin Wilsen Chan Chua Proprietary Use-Only License.
// See LICENSE.txt in the project root for full license information.

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

    public Task<Result<IReadOnlyList<WorkflowApprovalDto>>> ListTasksAsync(CancellationToken cancellationToken = default) =>
        SendAsync<IReadOnlyList<WorkflowApprovalDto>>(HttpMethod.Get, "v1/workflow/tasks", cancellationToken: cancellationToken);

    public Task<Result<WorkflowApprovalDto>> GetTaskAsync(Guid id, CancellationToken cancellationToken = default) =>
        SendAsync<WorkflowApprovalDto>(HttpMethod.Get, $"v1/workflow/tasks/{id}", cancellationToken: cancellationToken);

    public Task<Result> DecideAsync(Guid approvalId, WorkflowApprovalDecisionRequest request, CancellationToken cancellationToken = default) =>
        SendAsync(HttpMethod.Post, $"v1/workflow/approvals/{approvalId}/decision", request, cancellationToken);

    public Task<Result> ReassignAsync(Guid approvalId, WorkflowReassignRequest request, CancellationToken cancellationToken = default) =>
        SendAsync(HttpMethod.Post, $"v1/workflow/approvals/{approvalId}/reassign", request, cancellationToken);
}
