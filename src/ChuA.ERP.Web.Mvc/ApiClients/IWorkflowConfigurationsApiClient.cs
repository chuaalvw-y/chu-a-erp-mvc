// Copyright (c) 2026 Alvin Wilsen Chan Chua
// GitHub: chuaalvw-y
// Licensed under the Alvin Wilsen Chan Chua Proprietary Use-Only License.
// See LICENSE.txt in the project root for full license information.

using ChuA.ERP.Web.Mvc.Contracts.Common;
using ChuA.ERP.Web.Mvc.Contracts.Dtos;

namespace ChuA.ERP.Web.Mvc.ApiClients;

/// <summary>Calls /api/v1/workflow-configurations.</summary>
public interface IWorkflowConfigurationsApiClient
{
    Task<Result<IReadOnlyList<WorkflowConfigurationDto>>> ListAsync(
        string? targetEntityType = null,
        CancellationToken cancellationToken = default);

    Task<Result<WorkflowConfigurationDto>> GetAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Opts a document type into the workflow engine. Returns the new configuration id.</summary>
    Task<Result<Guid>> ConfigureAsync(ConfigureWorkflowRequest request, CancellationToken cancellationToken = default);

    Task<Result> ChangeAsync(Guid id, ChangeWorkflowConfigurationRequest request, CancellationToken cancellationToken = default);

    Task<Result> ActivateAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result> DeactivateAsync(Guid id, CancellationToken cancellationToken = default);
}
