using ChuA.ERP.Web.Mvc.Contracts.Common;
using ChuA.ERP.Web.Mvc.Contracts.Dtos;
using ChuA.ERP.Web.Mvc.Services;
using Microsoft.Extensions.Logging;

namespace ChuA.ERP.Web.Mvc.ApiClients;

/// <inheritdoc cref="IWorkflowDefinitionsApiClient"/>
public sealed class WorkflowDefinitionsApiClient : ApiClientBase, IWorkflowDefinitionsApiClient
{
    public WorkflowDefinitionsApiClient(
        HttpClient httpClient,
        ITokenAcquisitionService tokenAcquisition,
        ICorrelationIdAccessor correlationIds,
        ILogger<WorkflowDefinitionsApiClient> logger)
        : base(httpClient, tokenAcquisition, correlationIds, logger)
    {
    }

    public Task<Result<IReadOnlyList<WorkflowDefinitionDto>>> ListAsync(
        string? targetEntityType = null,
        string? status = null,
        CancellationToken cancellationToken = default) =>
        SendAsync<IReadOnlyList<WorkflowDefinitionDto>>(
            HttpMethod.Get,
            "v1/workflow-definitions" + QueryString(("targetEntityType", targetEntityType), ("status", status)),
            cancellationToken: cancellationToken);

    public Task<Result<WorkflowDefinitionDto>> GetAsync(Guid id, CancellationToken cancellationToken = default) =>
        SendAsync<WorkflowDefinitionDto>(HttpMethod.Get, $"v1/workflow-definitions/{id}", cancellationToken: cancellationToken);

    public Task<Result<Guid>> CreateAsync(CreateWorkflowDefinitionRequest request, CancellationToken cancellationToken = default) =>
        SendAsync<Guid>(HttpMethod.Post, "v1/workflow-definitions", request, cancellationToken);

    public Task<Result> AddStepAsync(Guid id, AddWorkflowStepRequest request, CancellationToken cancellationToken = default) =>
        SendAsync(HttpMethod.Post, $"v1/workflow-definitions/{id}/steps", request, cancellationToken);

    public Task<Result> UpdateStepAsync(Guid id, int stepNumber, UpdateWorkflowStepRequest request, CancellationToken cancellationToken = default) =>
        SendAsync(HttpMethod.Put, $"v1/workflow-definitions/{id}/steps/{stepNumber}", request, cancellationToken);

    public Task<Result> RemoveStepAsync(Guid id, int stepNumber, CancellationToken cancellationToken = default) =>
        SendAsync(HttpMethod.Delete, $"v1/workflow-definitions/{id}/steps/{stepNumber}", cancellationToken: cancellationToken);

    public Task<Result> AddApproverAsync(Guid id, int stepNumber, AddWorkflowApproverRequest request, CancellationToken cancellationToken = default) =>
        SendAsync(HttpMethod.Post, $"v1/workflow-definitions/{id}/steps/{stepNumber}/approvers", request, cancellationToken);

    public Task<Result> RemoveApproverAsync(Guid id, int stepNumber, string assigneeType, Guid assigneeId, CancellationToken cancellationToken = default) =>
        SendAsync(HttpMethod.Delete, $"v1/workflow-definitions/{id}/steps/{stepNumber}/approvers/{assigneeType}/{assigneeId}", cancellationToken: cancellationToken);

    public Task<Result> PublishAsync(Guid id, CancellationToken cancellationToken = default) =>
        SendAsync(HttpMethod.Post, $"v1/workflow-definitions/{id}/publish", cancellationToken: cancellationToken);

    public Task<Result> RetireAsync(Guid id, CancellationToken cancellationToken = default) =>
        SendAsync(HttpMethod.Post, $"v1/workflow-definitions/{id}/retire", cancellationToken: cancellationToken);

    public Task<Result<Guid>> CloneAsync(Guid id, CancellationToken cancellationToken = default) =>
        SendAsync<Guid>(HttpMethod.Post, $"v1/workflow-definitions/{id}/clone", cancellationToken: cancellationToken);
}
