// Copyright (c) 2026 Alvin Wilsen Chan Chua
// GitHub: chuaalvw-y
// Licensed under the Alvin Wilsen Chan Chua Proprietary Use-Only License.
// See LICENSE.txt in the project root for full license information.

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
