// Copyright (c) 2026 Alvin Wilsen Chan Chua
// GitHub: chuaalvw-y
// Licensed under the Alvin Wilsen Chan Chua Proprietary Use-Only License.
// See LICENSE.txt in the project root for full license information.

using ChuA.ERP.Web.Mvc.ApiClients;
using ChuA.ERP.Web.Mvc.Extensions;
using ChuA.ERP.Web.Mvc.Security;
using ChuA.ERP.Web.Mvc.Utilities;
using ChuA.ERP.Web.Mvc.ViewModels.Workflow;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChuA.ERP.Web.Mvc.Controllers;

/// <summary>
/// Admin UI for per-tenant workflow opt-in. Each row maps a document
/// type (e.g. "Bill") to a workflow code; the engine consults this
/// when the source aggregate raises a SubmittedForApproval event.
/// </summary>
[Authorize]
public sealed class WorkflowConfigurationsController : Controller
{
    private readonly IWorkflowConfigurationsApiClient _configs;

    public WorkflowConfigurationsController(IWorkflowConfigurationsApiClient configs)
    {
        _configs = configs;
    }

    [HttpGet]
    [Authorize(Policy = AuthorizationPolicies.WorkflowView)]
    public async Task<IActionResult> Index(string? targetEntityType, CancellationToken cancellationToken)
    {
        var result = await _configs.ListAsync(targetEntityType, cancellationToken).ConfigureAwait(false);
        if (result.IsFailure)
        {
            ModelState.AddResultErrors(result);
            return View(new WorkflowConfigurationListViewModel { TargetEntityType = targetEntityType });
        }
        ViewData["Breadcrumbs"] = new[]
        {
            new Breadcrumb("Dashboard", Url.Action("Index", "Dashboard")),
            new Breadcrumb("Workflow configurations", null, true)
        };
        return View(new WorkflowConfigurationListViewModel
        {
            Configurations = result.Value,
            TargetEntityType = targetEntityType
        });
    }

    [HttpGet]
    [Authorize(Policy = AuthorizationPolicies.WorkflowConfigure)]
    public IActionResult Create() => View(new ConfigureWorkflowFormViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = AuthorizationPolicies.WorkflowConfigure)]
    public async Task<IActionResult> Create(ConfigureWorkflowFormViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return View(model);
        var result = await _configs.ConfigureAsync(model.ToRequest(), cancellationToken).ConfigureAwait(false);
        if (result.IsFailure)
        {
            ModelState.AddResultErrors(result);
            return View(model);
        }
        TempData.AddToast($"'{model.TargetEntityType}' opted into '{model.WorkflowCode}'.", ToastLevel.Success);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [Authorize(Policy = AuthorizationPolicies.WorkflowConfigure)]
    public async Task<IActionResult> Edit(Guid id, CancellationToken cancellationToken)
    {
        var result = await _configs.GetAsync(id, cancellationToken).ConfigureAwait(false);
        if (result.IsFailure)
        {
            TempData.AddToast(result.Errors.First().Message, ToastLevel.Error);
            return RedirectToAction(nameof(Index));
        }
        return View(new ChangeWorkflowConfigurationFormViewModel
        {
            Id = id,
            TargetEntityType = result.Value.TargetEntityType,
            WorkflowCode = result.Value.WorkflowCode,
            PinnedVersion = result.Value.PinnedVersion
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = AuthorizationPolicies.WorkflowConfigure)]
    public async Task<IActionResult> Edit(Guid id, ChangeWorkflowConfigurationFormViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) { model.Id = id; return View(model); }
        var result = await _configs.ChangeAsync(id, model.ToRequest(), cancellationToken).ConfigureAwait(false);
        if (result.IsFailure)
        {
            ModelState.AddResultErrors(result);
            model.Id = id;
            return View(model);
        }
        TempData.AddToast("Configuration updated.", ToastLevel.Success);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("WorkflowConfigurations/{id:guid}/activate")]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = AuthorizationPolicies.WorkflowConfigure)]
    public async Task<IActionResult> Activate(Guid id, CancellationToken cancellationToken)
    {
        var result = await _configs.ActivateAsync(id, cancellationToken).ConfigureAwait(false);
        if (result.IsFailure) TempData.AddToast(result.Errors.First().Message, ToastLevel.Error);
        else TempData.AddToast("Configuration activated.", ToastLevel.Success);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("WorkflowConfigurations/{id:guid}/deactivate")]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = AuthorizationPolicies.WorkflowConfigure)]
    public async Task<IActionResult> Deactivate(Guid id, CancellationToken cancellationToken)
    {
        var result = await _configs.DeactivateAsync(id, cancellationToken).ConfigureAwait(false);
        if (result.IsFailure) TempData.AddToast(result.Errors.First().Message, ToastLevel.Error);
        else TempData.AddToast("Configuration deactivated.", ToastLevel.Success);
        return RedirectToAction(nameof(Index));
    }
}
