using ChuA.ERP.Web.Mvc.Contracts.Common;
using ChuA.ERP.Web.Mvc.Contracts.Dtos;

namespace ChuA.ERP.Web.Mvc.ViewModels.Workflow;

/// <summary>List page view model for workflow approval requests.</summary>
public sealed class WorkflowListViewModel
{
    public PagedResult<ApprovalRequestDto> Tasks { get; set; } = PagedResult<ApprovalRequestDto>.Empty();
    public string? Status { get; set; }
    public string? SubjectType { get; set; }
}
