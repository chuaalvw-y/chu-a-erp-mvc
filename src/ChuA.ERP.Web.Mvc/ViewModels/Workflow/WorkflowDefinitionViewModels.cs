using System.ComponentModel.DataAnnotations;
using ChuA.ERP.Web.Mvc.Contracts.Dtos;

namespace ChuA.ERP.Web.Mvc.ViewModels.Workflow;

public sealed class WorkflowDefinitionListViewModel
{
    public IReadOnlyList<WorkflowDefinitionDto> Definitions { get; set; } = Array.Empty<WorkflowDefinitionDto>();
    public string? TargetEntityType { get; set; }
    public string? Status { get; set; }
}

public sealed class CreateWorkflowDefinitionFormViewModel
{
    [Required, StringLength(80)]
    [Display(Name = "Code")]
    public string Code { get; set; } = string.Empty;

    [Required, StringLength(200)]
    [Display(Name = "Name")]
    public string Name { get; set; } = string.Empty;

    [Required, StringLength(128)]
    [Display(Name = "Target entity type")]
    public string TargetEntityType { get; set; } = string.Empty;

    [StringLength(1000)]
    [Display(Name = "Description")]
    public string? Description { get; set; }

    [Range(1, int.MaxValue)]
    [Display(Name = "Version")]
    public int Version { get; set; } = 1;

    [Display(Name = "System-wide template (V2 — leave unchecked)")]
    public bool SystemWide { get; set; }

    public CreateWorkflowDefinitionRequest ToRequest() => new(Code, Name, TargetEntityType, Description, Version, SystemWide);
}

public sealed class AddWorkflowStepFormViewModel
{
    public Guid DefinitionId { get; set; }

    [Required, Range(1, short.MaxValue)]
    [Display(Name = "Step number")]
    public short StepNumber { get; set; }

    [Required, StringLength(150)]
    [Display(Name = "Step name")]
    public string Name { get; set; } = string.Empty;

    [Required]
    [RegularExpression("^(Single|AllRequired|NOfM)$", ErrorMessage = "Approval mode must be Single, AllRequired, or NOfM.")]
    [Display(Name = "Approval mode")]
    public string ApprovalMode { get; set; } = "Single";

    [Range(1, int.MaxValue)]
    [Display(Name = "Required count (NOfM only)")]
    public int? RequiredCount { get; set; }

    [StringLength(2000)]
    [Display(Name = "Condition expression")]
    public string? ConditionExpression { get; set; }

    [Range(1, int.MaxValue)]
    [Display(Name = "Timeout (hours)")]
    public int? TimeoutHours { get; set; }

    [Range(1, short.MaxValue)]
    [Display(Name = "Escalate to step number")]
    public short? EscalationStepNumber { get; set; }

    public AddWorkflowStepRequest ToRequest() => new(
        StepNumber, Name, ApprovalMode, RequiredCount, ConditionExpression, TimeoutHours, EscalationStepNumber);
}

public sealed class AddWorkflowApproverFormViewModel
{
    public Guid DefinitionId { get; set; }
    public short StepNumber { get; set; }

    [Required]
    [RegularExpression("^(Role|User|Department|CostCenter)$",
        ErrorMessage = "Assignee type must be Role, User, Department, or CostCenter.")]
    [Display(Name = "Assignee type")]
    public string AssigneeType { get; set; } = "Role";

    [Required]
    [Display(Name = "Assignee id")]
    public Guid AssigneeId { get; set; }

    public AddWorkflowApproverRequest ToRequest() => new(AssigneeType, AssigneeId);
}
