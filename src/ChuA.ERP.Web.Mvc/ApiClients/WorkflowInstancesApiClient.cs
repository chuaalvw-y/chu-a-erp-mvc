using ChuA.ERP.Web.Mvc.Contracts.Common;
using ChuA.ERP.Web.Mvc.Contracts.Dtos;
using ChuA.ERP.Web.Mvc.Services;
using Microsoft.Extensions.Logging;

namespace ChuA.ERP.Web.Mvc.ApiClients;

/// <inheritdoc cref="IWorkflowInstancesApiClient"/>
public sealed class WorkflowInstancesApiClient : ApiClientBase, IWorkflowInstancesApiClient
{
    public WorkflowInstancesApiClient(
        HttpClient httpClient,
        ITokenAcquisitionService tokenAcquisition,
        ICorrelationIdAccessor correlationIds,
        ILogger<WorkflowInstancesApiClient> logger)
        : base(httpClient, tokenAcquisition, correlationIds, logger)
    {
    }

    public Task<Result<IReadOnlyList<WorkflowInstanceDto>>> ListAsync(
        string? status = null,
        string? targetEntityType = null,
        Guid? targetEntityId = null,
        CancellationToken cancellationToken = default) =>
        SendAsync<IReadOnlyList<WorkflowInstanceDto>>(
            HttpMethod.Get,
            "v1/workflow-instances" + QueryString(
                ("status", status),
                ("targetEntityType", targetEntityType),
                ("targetEntityId", targetEntityId)),
            cancellationToken: cancellationToken);

    public Task<Result<WorkflowInstanceDto>> GetAsync(Guid id, CancellationToken cancellationToken = default) =>
        SendAsync<WorkflowInstanceDto>(HttpMethod.Get, $"v1/workflow-instances/{id}", cancellationToken: cancellationToken);

    public Task<Result> CancelAsync(Guid id, CancelWorkflowInstanceRequest request, CancellationToken cancellationToken = default) =>
        SendAsync(HttpMethod.Post, $"v1/workflow-instances/{id}/cancel", request, cancellationToken);
}
