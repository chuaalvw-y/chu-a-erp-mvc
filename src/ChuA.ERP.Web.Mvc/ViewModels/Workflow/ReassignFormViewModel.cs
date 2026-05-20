using System.ComponentModel.DataAnnotations;
using ChuA.ERP.Web.Mvc.Contracts.Dtos;

namespace ChuA.ERP.Web.Mvc.ViewModels.Workflow;

/// <summary>
/// Admin form for reassigning a pending workflow approval to a different
/// user. Admin-only on the API.
/// </summary>
public sealed class ReassignFormViewModel
{
    /// <summary>The approval row id (path parameter on the API).</summary>
    public Guid Id { get; set; }

    /// <summary>The owning workflow-instance id — required by the API body.</summary>
    [Required]
    public Guid InstanceId { get; set; }

    [Required]
    [Display(Name = "Reassign to user")]
    public Guid ToUserId { get; set; }

    [Required]
    [StringLength(500)]
    [Display(Name = "Reason")]
    public string Reason { get; set; } = string.Empty;

    public WorkflowReassignRequest ToRequest() => new(InstanceId, ToUserId, Reason);
}
