// Copyright (c) 2026 Alvin Wilsen Chan Chua
// GitHub: chuaalvw-y
// Licensed under the Alvin Wilsen Chan Chua Proprietary Use-Only License.
// See LICENSE.txt in the project root for full license information.

using ChuA.ERP.Web.Mvc.Contracts.Dtos;

namespace ChuA.ERP.Web.Mvc.ViewModels.Workflow;

/// <summary>
/// List page view model for the caller's pending workflow approvals.
/// The API filters server-side by user; status/subject filters from the
/// legacy ApprovalRequest path no longer apply.
/// </summary>
public sealed class WorkflowListViewModel
{
    public IReadOnlyList<WorkflowApprovalDto> Tasks { get; set; } = Array.Empty<WorkflowApprovalDto>();
}
