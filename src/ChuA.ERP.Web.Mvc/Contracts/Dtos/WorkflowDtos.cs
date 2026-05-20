namespace ChuA.ERP.Web.Mvc.Contracts.Dtos;

/// <summary>
/// Mirror of <c>ChuA.ERP.Application.DTOs.WorkflowApprovalDto</c>. The
/// inbox endpoints (<c>GET /workflow/tasks</c>, <c>GET /workflow/tasks/{id}</c>)
/// return a single approval row out of the running engine; each row
/// carries the owning <see cref="WorkflowInstanceId"/> so the
/// decision / delegate / reassign forms can echo it back.
/// </summary>
public sealed record WorkflowApprovalDto(
    Guid Id,
    Guid WorkflowInstanceId,
    short StepNumber,
    Guid AssignedUserId,
    string Decision,
    DateTimeOffset AssignedAt,
    DateTimeOffset? DueAt,
    DateTimeOffset? DecidedAt,
    string? Comments,
    Guid? DelegatedToUserId,
    Guid? EscalatedFromUserId);

/// <summary>
/// Body of <c>POST /workflow/approvals/{approvalId}/decision</c>. The
/// instance id rides in the body because the API route only carries
/// the approval id; the engine looks up the instance through it but
/// the client sends both to avoid an extra round-trip.
/// </summary>
public sealed record WorkflowApprovalDecisionRequest(
    Guid InstanceId,
    string Decision,
    string? Comment);

/// <summary>
/// Body of <c>POST /workflow/approvals/{approvalId}/reassign</c>.
/// Admin-only path on the API.
/// </summary>
public sealed record WorkflowReassignRequest(
    Guid InstanceId,
    Guid ToUserId,
    string Reason);
