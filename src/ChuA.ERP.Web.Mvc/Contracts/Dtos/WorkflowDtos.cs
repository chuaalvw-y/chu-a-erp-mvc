// Copyright (c) 2026 Alvin Wilsen Chan Chua
// GitHub: chuaalvw-y
// Licensed under the Alvin Wilsen Chan Chua Proprietary Use-Only License.
// See LICENSE.txt in the project root for full license information.

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

// ---- WorkflowDefinitions admin (B) ---------------------------------------

public sealed record ApproverAssignmentDto(string AssigneeType, Guid AssigneeId);

public sealed record WorkflowStepDto(
    Guid Id,
    short StepNumber,
    string Name,
    string ApprovalMode,
    int? RequiredCount,
    string? ConditionExpression,
    int? TimeoutHours,
    short? EscalationStepNumber,
    IReadOnlyCollection<ApproverAssignmentDto> Approvers);

public sealed record WorkflowDefinitionDto(
    Guid Id,
    Guid? CompanyId,
    string Code,
    string Name,
    string? Description,
    string TargetEntityType,
    int Version,
    string Status,
    DateTimeOffset CreatedAt,
    DateTimeOffset? PublishedAt,
    DateTimeOffset? RetiredAt,
    IReadOnlyCollection<WorkflowStepDto> Steps);

public sealed record CreateWorkflowDefinitionRequest(
    string Code,
    string Name,
    string TargetEntityType,
    string? Description = null,
    int Version = 1,
    bool SystemWide = false);

public sealed record AddWorkflowStepRequest(
    short StepNumber,
    string Name,
    string ApprovalMode,
    int? RequiredCount = null,
    string? ConditionExpression = null,
    int? TimeoutHours = null,
    short? EscalationStepNumber = null);

public sealed record UpdateWorkflowStepRequest(
    string Name,
    string ApprovalMode,
    int? RequiredCount = null,
    string? ConditionExpression = null,
    int? TimeoutHours = null,
    short? EscalationStepNumber = null);

public sealed record AddWorkflowApproverRequest(
    string AssigneeType,
    Guid AssigneeId);

// ---- WorkflowConfigurations admin (B) -----------------------------------

public sealed record WorkflowConfigurationDto(
    Guid Id,
    Guid CompanyId,
    string TargetEntityType,
    string WorkflowCode,
    int? PinnedVersion,
    bool IsActive);

public sealed record ConfigureWorkflowRequest(
    string TargetEntityType,
    string WorkflowCode,
    int? PinnedVersion = null);

public sealed record ChangeWorkflowConfigurationRequest(
    string WorkflowCode,
    int? PinnedVersion = null);

// ---- WorkflowInstances admin (B) ----------------------------------------

public sealed record WorkflowInstanceDto(
    Guid Id,
    Guid CompanyId,
    Guid WorkflowDefinitionId,
    int DefinitionVersionPinned,
    string TargetEntityType,
    Guid TargetEntityId,
    string Status,
    short CurrentStepNumber,
    string? Context,
    DateTimeOffset StartedAt,
    DateTimeOffset? CompletedAt,
    string InitiatedBy,
    string? CancellationReason,
    IReadOnlyCollection<WorkflowApprovalDto> Approvals);

public sealed record CancelWorkflowInstanceRequest(string Reason);
