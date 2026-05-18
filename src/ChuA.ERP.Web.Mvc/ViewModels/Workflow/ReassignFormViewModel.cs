using System.ComponentModel.DataAnnotations;
using ChuA.ERP.Web.Mvc.Contracts.Dtos;

namespace ChuA.ERP.Web.Mvc.ViewModels.Workflow;

/// <summary>Form for reassigning a workflow task to a different user.</summary>
public sealed class ReassignFormViewModel
{
    /// <summary>Approval request id.</summary>
    public Guid Id { get; set; }

    [Required]
    [Display(Name = "New assignee")]
    public Guid NewAssigneeId { get; set; }

    [StringLength(500)]
    [Display(Name = "Reason")]
    public string? Reason { get; set; }

    public ReassignWorkflowTaskRequest ToRequest() => new(NewAssigneeId, Reason);
}
