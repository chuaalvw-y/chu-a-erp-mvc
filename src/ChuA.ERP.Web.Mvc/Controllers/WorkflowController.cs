using ChuA.ERP.Web.Mvc.ApiClients;
using ChuA.ERP.Web.Mvc.Extensions;
using ChuA.ERP.Web.Mvc.Security;
using ChuA.ERP.Web.Mvc.Utilities;
using ChuA.ERP.Web.Mvc.ViewModels.Workflow;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChuA.ERP.Web.Mvc.Controllers;

/// <summary>UI for the Workflow module — surfaces approval requests originating upstream.</summary>
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
    public async Task<IActionResult> Index(string? status, string? subjectType, int pageNumber = 1, int pageSize = 25, string? sort = null, CancellationToken cancellationToken = default)
    {
        var result = await _workflow.ListTasksAsync(status, subjectType, pageNumber, pageSize, sort, cancellationToken).ConfigureAwait(false);
        if (result.IsFailure)
        {
            ModelState.AddResultErrors(result);
            return View(new WorkflowListViewModel { Status = status, SubjectType = subjectType });
        }

        ViewData["Breadcrumbs"] = new[]
        {
            new Breadcrumb("Dashboard", Url.Action("Index", "Dashboard")),
            new Breadcrumb("Workflow", null, true)
        };
        return View(new WorkflowListViewModel
        {
            Tasks = result.Value,
            Status = status,
            SubjectType = subjectType,
        });
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
            new Breadcrumb($"{result.Value.SubjectType} {result.Value.SubjectId}", null, true)
        };
        return View(result.Value);
    }

    [HttpGet]
    [Authorize(Policy = AuthorizationPolicies.WorkflowApprovalSubmit)]
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
            new Breadcrumb($"{result.Value.SubjectType} {result.Value.SubjectId}", Url.Action(nameof(Details), new { id })),
            new Breadcrumb("Submit decision", null, true)
        };
        return View(new SubmitApprovalFormViewModel { Id = id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = AuthorizationPolicies.WorkflowApprovalSubmit)]
    public async Task<IActionResult> Submit(Guid id, SubmitApprovalFormViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            model.Id = id;
            return View(model);
        }

        var result = await _workflow.SubmitApprovalAsync(id, model.ToRequest(), cancellationToken).ConfigureAwait(false);
        if (result.IsFailure)
        {
            ModelState.AddResultErrors(result);
            model.Id = id;
            return View(model);
        }
        TempData.AddToast($"Decision '{model.Decision}' submitted.", ToastLevel.Success);
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpGet]
    [Authorize(Policy = AuthorizationPolicies.WorkflowReassign)]
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
            new Breadcrumb($"{result.Value.SubjectType} {result.Value.SubjectId}", Url.Action(nameof(Details), new { id })),
            new Breadcrumb("Reassign", null, true)
        };
        return View(new ReassignFormViewModel { Id = id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = AuthorizationPolicies.WorkflowReassign)]
    public async Task<IActionResult> Reassign(Guid id, ReassignFormViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            model.Id = id;
            return View(model);
        }

        var result = await _workflow.ReassignTaskAsync(id, model.ToRequest(), cancellationToken).ConfigureAwait(false);
        if (result.IsFailure)
        {
            ModelState.AddResultErrors(result);
            model.Id = id;
            return View(model);
        }
        TempData.AddToast("Task reassigned.", ToastLevel.Success);
        return RedirectToAction(nameof(Details), new { id });
    }
}
