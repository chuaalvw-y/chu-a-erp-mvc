using ChuA.ERP.Web.Mvc.Contracts.Common;
using ChuA.ERP.Web.Mvc.Contracts.Dtos;

namespace ChuA.ERP.Web.Mvc.ApiClients;

/// <summary>Calls /api/v1/workflow-definitions.</summary>
public interface IWorkflowDefinitionsApiClient
{
    /// <summary>Lists workflow definitions visible to the caller's company.</summary>
    Task<Result<IReadOnlyList<WorkflowDefinitionDto>>> ListAsync(
        string? targetEntityType = null,
        string? status = null,
        CancellationToken cancellationToken = default);

    Task<Result<WorkflowDefinitionDto>> GetAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Creates a new <c>Draft</c> definition. Returns the new id.</summary>
    Task<Result<Guid>> CreateAsync(CreateWorkflowDefinitionRequest request, CancellationToken cancellationToken = default);

    Task<Result> AddStepAsync(Guid id, AddWorkflowStepRequest request, CancellationToken cancellationToken = default);
    Task<Result> UpdateStepAsync(Guid id, int stepNumber, UpdateWorkflowStepRequest request, CancellationToken cancellationToken = default);
    Task<Result> RemoveStepAsync(Guid id, int stepNumber, CancellationToken cancellationToken = default);

    Task<Result> AddApproverAsync(Guid id, int stepNumber, AddWorkflowApproverRequest request, CancellationToken cancellationToken = default);
    Task<Result> RemoveApproverAsync(Guid id, int stepNumber, string assigneeType, Guid assigneeId, CancellationToken cancellationToken = default);

    Task<Result> PublishAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result> RetireAsync(Guid id, CancellationToken cancellationToken = default);
    /// <summary>Creates a fresh <c>Draft</c> with <c>Version + 1</c>. Returns the new draft id.</summary>
    Task<Result<Guid>> CloneAsync(Guid id, CancellationToken cancellationToken = default);
}
