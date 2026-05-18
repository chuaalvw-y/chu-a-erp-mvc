using System.ComponentModel.DataAnnotations;
using ChuA.ERP.Web.Mvc.Contracts.Dtos;

namespace ChuA.ERP.Web.Mvc.ViewModels.Workflow;

/// <summary>Approve/Reject submission form for a workflow approval request.</summary>
public sealed class SubmitApprovalFormViewModel
{
    /// <summary>Approval request id.</summary>
    public Guid Id { get; set; }

    [Required]
    [RegularExpression("^(Approve|Reject)$", ErrorMessage = "Decision must be 'Approve' or 'Reject'.")]
    [Display(Name = "Decision")]
    public string Decision { get; set; } = "Approve";

    [StringLength(1000)]
    [Display(Name = "Comment")]
    public string? Comment { get; set; }

    public SubmitWorkflowApprovalRequest ToRequest() => new(Decision, Comment);
}
