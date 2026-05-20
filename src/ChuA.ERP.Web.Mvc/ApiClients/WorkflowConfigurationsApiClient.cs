// Copyright (c) 2026 Alvin Wilsen Chan Chua
// GitHub: chuaalvw-y
// Licensed under the Alvin Wilsen Chan Chua Proprietary Use-Only License.
// See LICENSE.txt in the project root for full license information.

using ChuA.ERP.Web.Mvc.Contracts.Common;
using ChuA.ERP.Web.Mvc.Contracts.Dtos;
using ChuA.ERP.Web.Mvc.Services;
using Microsoft.Extensions.Logging;

namespace ChuA.ERP.Web.Mvc.ApiClients;

/// <inheritdoc cref="IWorkflowConfigurationsApiClient"/>
public sealed class WorkflowConfigurationsApiClient : ApiClientBase, IWorkflowConfigurationsApiClient
{
    public WorkflowConfigurationsApiClient(
        HttpClient httpClient,
        ITokenAcquisitionService tokenAcquisition,
        ICorrelationIdAccessor correlationIds,
        ILogger<WorkflowConfigurationsApiClient> logger)
        : base(httpClient, tokenAcquisition, correlationIds, logger)
    {
    }

    public Task<Result<IReadOnlyList<WorkflowConfigurationDto>>> ListAsync(
        string? targetEntityType = null,
        CancellationToken cancellationToken = default) =>
        SendAsync<IReadOnlyList<WorkflowConfigurationDto>>(
            HttpMethod.Get,
            "v1/workflow-configurations" + QueryString(("targetEntityType", targetEntityType)),
            cancellationToken: cancellationToken);

    public Task<Result<WorkflowConfigurationDto>> GetAsync(Guid id, CancellationToken cancellationToken = default) =>
        SendAsync<WorkflowConfigurationDto>(HttpMethod.Get, $"v1/workflow-configurations/{id}", cancellationToken: cancellationToken);

    public Task<Result<Guid>> ConfigureAsync(ConfigureWorkflowRequest request, CancellationToken cancellationToken = default) =>
        SendAsync<Guid>(HttpMethod.Post, "v1/workflow-configurations", request, cancellationToken);

    public Task<Result> ChangeAsync(Guid id, ChangeWorkflowConfigurationRequest request, CancellationToken cancellationToken = default) =>
        SendAsync(HttpMethod.Put, $"v1/workflow-configurations/{id}", request, cancellationToken);

    public Task<Result> ActivateAsync(Guid id, CancellationToken cancellationToken = default) =>
        SendAsync(HttpMethod.Post, $"v1/workflow-configurations/{id}/activate", cancellationToken: cancellationToken);

    public Task<Result> DeactivateAsync(Guid id, CancellationToken cancellationToken = default) =>
        SendAsync(HttpMethod.Post, $"v1/workflow-configurations/{id}/deactivate", cancellationToken: cancellationToken);
}
