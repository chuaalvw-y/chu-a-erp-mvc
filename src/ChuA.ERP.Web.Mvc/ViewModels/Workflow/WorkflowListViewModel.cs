using ChuA.ERP.Web.Mvc.Contracts.Dtos;

namespace ChuA.ERP.Web.Mvc.ViewModels.Workflow;

/// <summary>List page view model for workflow approval requests.</summary>
public sealed class WorkflowListViewModel
{
    public IReadOnlyList<ApprovalRequestDto> Tasks { get; set; } = Array.Empty<ApprovalRequestDto>();
    public string? Status { get; set; }
    public string? SubjectType { get; set; }
}
