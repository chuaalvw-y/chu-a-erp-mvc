// Copyright (c) 2026 Alvin Wilsen Chan Chua
// GitHub: chuaalvw-y
// Licensed under the Alvin Wilsen Chan Chua Proprietary Use-Only License.
// See LICENSE.txt in the project root for full license information.

using System.ComponentModel.DataAnnotations;
using ChuA.ERP.Web.Mvc.Contracts.Dtos;

namespace ChuA.ERP.Web.Mvc.ViewModels.Workflow;

public sealed class WorkflowConfigurationListViewModel
{
    public IReadOnlyList<WorkflowConfigurationDto> Configurations { get; set; } = Array.Empty<WorkflowConfigurationDto>();
    public string? TargetEntityType { get; set; }
}

public sealed class ConfigureWorkflowFormViewModel
{
    [Required, StringLength(128)]
    [Display(Name = "Target entity type")]
    public string TargetEntityType { get; set; } = string.Empty;

    [Required, StringLength(80)]
    [Display(Name = "Workflow code")]
    public string WorkflowCode { get; set; } = string.Empty;

    [Range(1, int.MaxValue)]
    [Display(Name = "Pinned version (leave blank for latest Published)")]
    public int? PinnedVersion { get; set; }

    public ConfigureWorkflowRequest ToRequest() => new(TargetEntityType, WorkflowCode, PinnedVersion);
}

public sealed class ChangeWorkflowConfigurationFormViewModel
{
    public Guid Id { get; set; }
    public string TargetEntityType { get; set; } = string.Empty;

    [Required, StringLength(80)]
    [Display(Name = "Workflow code")]
    public string WorkflowCode { get; set; } = string.Empty;

    [Range(1, int.MaxValue)]
    [Display(Name = "Pinned version (leave blank for latest Published)")]
    public int? PinnedVersion { get; set; }

    public ChangeWorkflowConfigurationRequest ToRequest() => new(WorkflowCode, PinnedVersion);
}
