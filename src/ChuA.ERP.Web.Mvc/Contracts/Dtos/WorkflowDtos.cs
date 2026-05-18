namespace ChuA.ERP.Web.Mvc.Contracts.Dtos;

public sealed record ApprovalRequestDto(
    Guid Id,
    Guid CompanyId,
    string SubjectType,
    Guid SubjectId,
    string RequestedBy,
    DateTimeOffset RequestedAt,
    int RequiredApprovals,
    string Status);

public sealed record SubmitWorkflowApprovalRequest(
    string Decision,
    string? Comment);

public sealed record ReassignWorkflowTaskRequest(
    Guid NewAssigneeId,
    string? Reason);
