using ChuA.ERP.Web.Mvc.Contracts.Common;
using ChuA.ERP.Web.Mvc.Contracts.Dtos;

namespace ChuA.ERP.Web.Mvc.ApiClients;

/// <summary>Calls /api/v1/workflow-instances — running-instance audit and cancellation.</summary>
public interface IWorkflowInstancesApiClient
{
    Task<Result<IReadOnlyList<WorkflowInstanceDto>>> ListAsync(
        string? status = null,
        string? targetEntityType = null,
        Guid? targetEntityId = null,
        CancellationToken cancellationToken = default);

    Task<Result<WorkflowInstanceDto>> GetAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Result> CancelAsync(Guid id, CancelWorkflowInstanceRequest request, CancellationToken cancellationToken = default);
}
