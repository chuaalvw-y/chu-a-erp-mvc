// Copyright (c) 2026 Alvin Wilsen Chan Chua
// GitHub: chuaalvw-y
// Licensed under the Alvin Wilsen Chan Chua Proprietary Use-Only License.
// See LICENSE.txt in the project root for full license information.

using System.Security.Claims;
using ChuA.ERP.Web.Mvc.ApiClients;
using ChuA.ERP.Web.Mvc.Contracts.Dtos;
using ChuA.ERP.Web.Mvc.Extensions;
using ChuA.ERP.Web.Mvc.Models.Notifications;
using ChuA.ERP.Web.Mvc.Security;
using ChuA.ERP.Web.Mvc.Services;
using ChuA.ERP.Web.Mvc.Utilities;
using ChuA.ERP.Web.Mvc.ViewModels.Workflow;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChuA.ERP.Web.Mvc.Controllers;

/// <summary>
/// UI for the workflow inbox. Surfaces the caller's pending workflow
/// approvals from <c>/api/v1/workflow/tasks</c> and lets them decide,
/// or — with the admin policy — reassign to another user.
/// </summary>
[Authorize]
public sealed class WorkflowController : Controller
{
    /// <summary>Event name broadcast to the user's tabs when their workflow inbox has changed.</summary>
    public const string InboxChangedEvent = "workflowInboxChanged";

    private readonly IWorkflowApiClient _workflow;
    private readonly INotificationPublisher _notifications;

    public WorkflowController(
        IWorkflowApiClient workflow,
        INotificationPublisher notifications)
    {
        _workflow = workflow;
        _notifications = notifications;
    }

    private string? CurrentUserId =>
        User.FindFirstValue("sub")
        ?? User.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? User.Identity?.Name;

    [HttpGet]
    [Authorize(Policy = AuthorizationPolicies.WorkflowView)]
    public async Task<IActionResult> Index(CancellationToken cancellationToken = default)
    {
        var result = await _workflow.ListTasksAsync(cancellationToken).ConfigureAwait(false);
        if (result.IsFailure)
        {
            ModelState.AddResultErrors(result);
            return View(new WorkflowListViewModel());
        }

        ViewData["Breadcrumbs"] = new[]
        {
            new Breadcrumb("Dashboard", Url.Action("Index", "Dashboard")),
            new Breadcrumb("Workflow", null, true)
        };
        return View(new WorkflowListViewModel { Tasks = result.Value });
    }

    [HttpGet]
    [Authorize(Policy = AuthorizationPolicies.WorkflowView)]
    public async Task<IActionResult> Details(Guid id, CancellationToken cancellationToken)
    {
        var result = await _workflow.GetTaskAsync(id, cancellationToken).ConfigureAwait(false);
        if (result.IsFailure)
        {
            TempData.AddToast(result.Errors.First().Message, ToastLevel.Error);
            return RedirectToAction(nameof(Index));
        }
        ViewData["Breadcrumbs"] = new[]
        {
            new Breadcrumb("Dashboard", Url.Action("Index", "Dashboard")),
            new Breadcrumb("Workflow", Url.Action(nameof(Index))),
            new Breadcrumb($"Approval step {result.Value.StepNumber}", null, true)
        };
        return View(result.Value);
    }

    [HttpGet]
    [Authorize(Policy = AuthorizationPolicies.WorkflowApprove)]
    public async Task<IActionResult> Submit(Guid id, CancellationToken cancellationToken)
    {
        var result = await _workflow.GetTaskAsync(id, cancellationToken).ConfigureAwait(false);
        if (result.IsFailure)
        {
            TempData.AddToast(result.Errors.First().Message, ToastLevel.Error);
            return RedirectToAction(nameof(Index));
        }
        ViewData["Task"] = result.Value;
        ViewData["Breadcrumbs"] = new[]
        {
            new Breadcrumb("Dashboard", Url.Action("Index", "Dashboard")),
            new Breadcrumb("Workflow", Url.Action(nameof(Index))),
            new Breadcrumb($"Approval step {result.Value.StepNumber}", Url.Action(nameof(Details), new { id })),
            new Breadcrumb("Submit decision", null, true)
        };
        return View(new SubmitApprovalFormViewModel
        {
            Id = id,
            InstanceId = result.Value.WorkflowInstanceId
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = AuthorizationPolicies.WorkflowApprove)]
    public async Task<IActionResult> Submit(Guid id, SubmitApprovalFormViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            model.Id = id;
            return View(model);
        }

        var result = await _workflow.DecideAsync(id, model.ToRequest(), cancellationToken).ConfigureAwait(false);
        if (result.IsFailure)
        {
            ModelState.AddResultErrors(result);
            model.Id = id;
            // Refetch so the surrounding context (step, due date, etc.) still
            // renders if validation roundtrips a second time.
            var task = await _workflow.GetTaskAsync(id, cancellationToken).ConfigureAwait(false);
            if (task.IsSuccess) ViewData["Task"] = task.Value;
            return View(model);
        }
        TempData.AddToast($"Decision '{model.Decision}' submitted.", ToastLevel.Success);
        await BroadcastInboxChangedAsync(cancellationToken).ConfigureAwait(false);
        if (CurrentUserId is { Length: > 0 } uid)
        {
            await _notifications.PublishAsync(
                uid,
                $"Decision '{model.Decision}' submitted",
                body: $"Approval step {id.ToString()[..8]} decided.",
                level: NotificationLevel.Success,
                link: Url.Action(nameof(Details), new { id }),
                category: "workflow",
                cancellationToken: cancellationToken).ConfigureAwait(false);
        }
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpGet]
    [Authorize(Policy = AuthorizationPolicies.WorkflowDelegate)]
    public async Task<IActionResult> Reassign(Guid id, CancellationToken cancellationToken)
    {
        var result = await _workflow.GetTaskAsync(id, cancellationToken).ConfigureAwait(false);
        if (result.IsFailure)
        {
            TempData.AddToast(result.Errors.First().Message, ToastLevel.Error);
            return RedirectToAction(nameof(Index));
        }
        ViewData["Task"] = result.Value;
        ViewData["Breadcrumbs"] = new[]
        {
            new Breadcrumb("Dashboard", Url.Action("Index", "Dashboard")),
            new Breadcrumb("Workflow", Url.Action(nameof(Index))),
            new Breadcrumb($"Approval step {result.Value.StepNumber}", Url.Action(nameof(Details), new { id })),
            new Breadcrumb("Reassign", null, true)
        };
        return View(new ReassignFormViewModel
        {
            Id = id,
            InstanceId = result.Value.WorkflowInstanceId
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = AuthorizationPolicies.WorkflowDelegate)]
    public async Task<IActionResult> Reassign(Guid id, ReassignFormViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            model.Id = id;
            return View(model);
        }

        var result = await _workflow.ReassignAsync(id, model.ToRequest(), cancellationToken).ConfigureAwait(false);
        if (result.IsFailure)
        {
            ModelState.AddResultErrors(result);
            model.Id = id;
            var task = await _workflow.GetTaskAsync(id, cancellationToken).ConfigureAwait(false);
            if (task.IsSuccess) ViewData["Task"] = task.Value;
            return View(model);
        }
        TempData.AddToast("Task reassigned.", ToastLevel.Success);
        await BroadcastInboxChangedAsync(cancellationToken).ConfigureAwait(false);
        if (CurrentUserId is { Length: > 0 } uid)
        {
            await _notifications.PublishAsync(
                uid,
                "Approval reassigned",
                body: $"Approval {id.ToString()[..8]} reassigned.",
                level: NotificationLevel.Info,
                link: Url.Action(nameof(Details), new { id }),
                category: "workflow",
                cancellationToken: cancellationToken).ConfigureAwait(false);
        }
        return RedirectToAction(nameof(Details), new { id });
    }

    /// <summary>
    /// Returns the workflow-inbox table fragment for the current user. Hit by
    /// <c>chua-reactive.js</c> after a decision/reassign or when a <c>workflowInboxChanged</c>
    /// SignalR event arrives.
    /// </summary>
    [HttpGet]
    [Authorize(Policy = AuthorizationPolicies.WorkflowView)]
    public async Task<IActionResult> InboxPartial(CancellationToken cancellationToken = default)
    {
        var result = await _workflow.ListTasksAsync(cancellationToken).ConfigureAwait(false);
        if (result.IsFailure)
        {
            ModelState.AddResultErrors(result);
            return PartialView("_WorkflowInboxTable", new WorkflowListViewModel());
        }
        return PartialView("_WorkflowInboxTable", new WorkflowListViewModel { Tasks = result.Value });
    }

    /// <summary>
    /// Returns the count of pending workflow tasks for the current user as JSON. Cheap
    /// poll target for the topbar badge.
    /// </summary>
    [HttpGet]
    [Authorize(Policy = AuthorizationPolicies.WorkflowView)]
    public async Task<IActionResult> Count(CancellationToken cancellationToken = default)
    {
        var result = await _workflow.ListTasksAsync(cancellationToken).ConfigureAwait(false);
        var count = result.IsSuccess ? result.Value.Count : 0;
        return Json(new { count });
    }

    /// <summary>Fires the <c>workflowInboxChanged</c> event to the current user's tabs.</summary>
    private Task BroadcastInboxChangedAsync(CancellationToken cancellationToken)
        => CurrentUserId is { Length: > 0 } uid
            ? _notifications.BroadcastEventAsync(uid, InboxChangedEvent, cancellationToken: cancellationToken)
            : Task.CompletedTask;
}
