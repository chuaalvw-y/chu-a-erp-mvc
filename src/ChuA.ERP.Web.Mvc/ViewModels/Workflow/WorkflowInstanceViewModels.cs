using System.ComponentModel.DataAnnotations;
using ChuA.ERP.Web.Mvc.Contracts.Dtos;

namespace ChuA.ERP.Web.Mvc.ViewModels.Workflow;

public sealed class WorkflowInstanceListViewModel
{
    public IReadOnlyList<WorkflowInstanceDto> Instances { get; set; } = Array.Empty<WorkflowInstanceDto>();
    public string? Status { get; set; }
    public string? TargetEntityType { get; set; }
    public Guid? TargetEntityId { get; set; }
}

public sealed class CancelWorkflowInstanceFormViewModel
{
    public Guid Id { get; set; }

    [Required, StringLength(1000)]
    [Display(Name = "Reason")]
    public string Reason { get; set; } = string.Empty;

    public CancelWorkflowInstanceRequest ToRequest() => new(Reason);
}
