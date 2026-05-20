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
/// Audit view of running workflow instances. The only mutation here is
/// admin/initiator cancellation; approver actions live under
/// <see cref="WorkflowController"/>.
/// </summary>
[Authorize]
public sealed class WorkflowInstancesController : Controller
{
    private readonly IWorkflowInstancesApiClient _instances;

    public WorkflowInstancesController(IWorkflowInstancesApiClient instances)
    {
        _instances = instances;
    }

    [HttpGet]
    [Authorize(Policy = AuthorizationPolicies.WorkflowInstanceRead)]
    public async Task<IActionResult> Index(string? status, string? targetEntityType, Guid? targetEntityId, CancellationToken cancellationToken)
    {
        var result = await _instances.ListAsync(status, targetEntityType, targetEntityId, cancellationToken).ConfigureAwait(false);
        if (result.IsFailure)
        {
            ModelState.AddResultErrors(result);
            return View(new WorkflowInstanceListViewModel { Status = status, TargetEntityType = targetEntityType, TargetEntityId = targetEntityId });
        }
        ViewData["Breadcrumbs"] = new[]
        {
            new Breadcrumb("Dashboard", Url.Action("Index", "Dashboard")),
            new Breadcrumb("Workflow instances", null, true)
        };
        return View(new WorkflowInstanceListViewModel
        {
            Instances = result.Value,
            Status = status,
            TargetEntityType = targetEntityType,
            TargetEntityId = targetEntityId
        });
    }

    [HttpGet]
    [Authorize(Policy = AuthorizationPolicies.WorkflowInstanceRead)]
    public async Task<IActionResult> Details(Guid id, CancellationToken cancellationToken)
    {
        var result = await _instances.GetAsync(id, cancellationToken).ConfigureAwait(false);
        if (result.IsFailure)
        {
            TempData.AddToast(result.Errors.First().Message, ToastLevel.Error);
            return RedirectToAction(nameof(Index));
        }
        ViewData["Breadcrumbs"] = new[]
        {
            new Breadcrumb("Dashboard", Url.Action("Index", "Dashboard")),
            new Breadcrumb("Workflow instances", Url.Action(nameof(Index))),
            new Breadcrumb($"{result.Value.TargetEntityType} {result.Value.TargetEntityId}", null, true)
        };
        return View(result.Value);
    }

    [HttpGet]
    [Authorize(Policy = AuthorizationPolicies.WorkflowInstanceCancel)]
    public IActionResult Cancel(Guid id) => View(new CancelWorkflowInstanceFormViewModel { Id = id });

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = AuthorizationPolicies.WorkflowInstanceCancel)]
    public async Task<IActionResult> Cancel(Guid id, CancelWorkflowInstanceFormViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) { model.Id = id; return View(model); }
        var result = await _instances.CancelAsync(id, model.ToRequest(), cancellationToken).ConfigureAwait(false);
        if (result.IsFailure)
        {
            ModelState.AddResultErrors(result);
            model.Id = id;
            return View(model);
        }
        TempData.AddToast("Workflow instance cancelled.", ToastLevel.Success);
        return RedirectToAction(nameof(Details), new { id });
    }
}
