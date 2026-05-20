using ChuA.ERP.Web.Mvc.ApiClients;
using ChuA.ERP.Web.Mvc.Contracts.Dtos;
using ChuA.ERP.Web.Mvc.Extensions;
using ChuA.ERP.Web.Mvc.Security;
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
    private readonly IWorkflowApiClient _workflow;

    public WorkflowController(IWorkflowApiClient workflow)
    {
        _workflow = workflow;
    }

    [HttpGet]
    [Authorize(Policy = AuthorizationPolicies.WorkflowRead)]
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
    [Authorize(Policy = AuthorizationPolicies.WorkflowRead)]
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
    [Authorize(Policy = AuthorizationPolicies.WorkflowApprovalDecide)]
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
    [Authorize(Policy = AuthorizationPolicies.WorkflowApprovalDecide)]
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
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpGet]
    [Authorize(Policy = AuthorizationPolicies.WorkflowApprovalReassign)]
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
    [Authorize(Policy = AuthorizationPolicies.WorkflowApprovalReassign)]
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
        return RedirectToAction(nameof(Details), new { id });
    }
}
