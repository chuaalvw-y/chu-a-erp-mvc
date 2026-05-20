using System.ComponentModel.DataAnnotations;
using ChuA.ERP.Web.Mvc.Contracts.Dtos;

namespace ChuA.ERP.Web.Mvc.ViewModels.Workflow;

/// <summary>Approve/Reject decision form for a single workflow approval.</summary>
public sealed class SubmitApprovalFormViewModel
{
    /// <summary>The approval row id (path parameter on the API).</summary>
    public Guid Id { get; set; }

    /// <summary>The owning workflow-instance id — required by the API body.</summary>
    [Required]
    public Guid InstanceId { get; set; }

    /// <summary>
    /// Approve or Reject. Maps to <c>WorkflowDecision</c> on the API
    /// (Approved / Rejected). Delegated / Escalated outcomes go through
    /// dedicated endpoints, not this form.
    /// </summary>
    [Required]
    [RegularExpression("^(Approved|Rejected)$", ErrorMessage = "Decision must be 'Approved' or 'Rejected'.")]
    [Display(Name = "Decision")]
    public string Decision { get; set; } = "Approved";

    [StringLength(2000)]
    [Display(Name = "Comment")]
    public string? Comment { get; set; }

    public WorkflowApprovalDecisionRequest ToRequest() => new(InstanceId, Decision, Comment);
}
