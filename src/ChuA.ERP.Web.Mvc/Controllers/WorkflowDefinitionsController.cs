using ChuA.ERP.Web.Mvc.ApiClients;
using ChuA.ERP.Web.Mvc.Extensions;
using ChuA.ERP.Web.Mvc.Security;
using ChuA.ERP.Web.Mvc.Utilities;
using ChuA.ERP.Web.Mvc.ViewModels.Workflow;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChuA.ERP.Web.Mvc.Controllers;

/// <summary>
/// Admin UI for the workflow-definition aggregate. List + details are
/// gated on the read policy; mutating actions require the corresponding
/// definition policy. The definition becomes immutable once Published —
/// admins clone-to-version it for revisions.
/// </summary>
[Authorize]
public sealed class WorkflowDefinitionsController : Controller
{
    private readonly IWorkflowDefinitionsApiClient _definitions;

    public WorkflowDefinitionsController(IWorkflowDefinitionsApiClient definitions)
    {
        _definitions = definitions;
    }

    [HttpGet]
    [Authorize(Policy = AuthorizationPolicies.WorkflowInstanceRead)]
    public async Task<IActionResult> Index(string? targetEntityType, string? status, CancellationToken cancellationToken)
    {
        var result = await _definitions.ListAsync(targetEntityType, status, cancellationToken).ConfigureAwait(false);
        if (result.IsFailure)
        {
            ModelState.AddResultErrors(result);
            return View(new WorkflowDefinitionListViewModel { TargetEntityType = targetEntityType, Status = status });
        }
        ViewData["Breadcrumbs"] = new[]
        {
            new Breadcrumb("Dashboard", Url.Action("Index", "Dashboard")),
            new Breadcrumb("Workflow definitions", null, true)
        };
        return View(new WorkflowDefinitionListViewModel
        {
            Definitions = result.Value,
            TargetEntityType = targetEntityType,
            Status = status
        });
    }

    [HttpGet]
    [Authorize(Policy = AuthorizationPolicies.WorkflowInstanceRead)]
    public async Task<IActionResult> Details(Guid id, CancellationToken cancellationToken)
    {
        var result = await _definitions.GetAsync(id, cancellationToken).ConfigureAwait(false);
        if (result.IsFailure)
        {
            TempData.AddToast(result.Errors.First().Message, ToastLevel.Error);
            return RedirectToAction(nameof(Index));
        }
        ViewData["Breadcrumbs"] = new[]
        {
            new Breadcrumb("Dashboard", Url.Action("Index", "Dashboard")),
            new Breadcrumb("Workflow definitions", Url.Action(nameof(Index))),
            new Breadcrumb($"{result.Value.Code} v{result.Value.Version}", null, true)
        };
        return View(result.Value);
    }

    [HttpGet]
    [Authorize(Policy = AuthorizationPolicies.WorkflowDefinitionCreate)]
    public IActionResult Create() => View(new CreateWorkflowDefinitionFormViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = AuthorizationPolicies.WorkflowDefinitionCreate)]
    public async Task<IActionResult> Create(CreateWorkflowDefinitionFormViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return View(model);
        var result = await _definitions.CreateAsync(model.ToRequest(), cancellationToken).ConfigureAwait(false);
        if (result.IsFailure)
        {
            ModelState.AddResultErrors(result);
            return View(model);
        }
        TempData.AddToast($"Workflow definition '{model.Code}' created.", ToastLevel.Success);
        return RedirectToAction(nameof(Details), new { id = result.Value });
    }

    // ---- Step administration -----------------------------------------------

    [HttpGet]
    [Authorize(Policy = AuthorizationPolicies.WorkflowDefinitionUpdate)]
    public IActionResult AddStep(Guid id) =>
        View(new AddWorkflowStepFormViewModel { DefinitionId = id });

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = AuthorizationPolicies.WorkflowDefinitionUpdate)]
    public async Task<IActionResult> AddStep(Guid id, AddWorkflowStepFormViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) { model.DefinitionId = id; return View(model); }
        var result = await _definitions.AddStepAsync(id, model.ToRequest(), cancellationToken).ConfigureAwait(false);
        if (result.IsFailure)
        {
            ModelState.AddResultErrors(result);
            model.DefinitionId = id;
            return View(model);
        }
        TempData.AddToast($"Step {model.StepNumber} added.", ToastLevel.Success);
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost("WorkflowDefinitions/{id:guid}/steps/{stepNumber:int}/delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = AuthorizationPolicies.WorkflowDefinitionUpdate)]
    public async Task<IActionResult> RemoveStep(Guid id, int stepNumber, CancellationToken cancellationToken)
    {
        var result = await _definitions.RemoveStepAsync(id, stepNumber, cancellationToken).ConfigureAwait(false);
        if (result.IsFailure)
        {
            TempData.AddToast(result.Errors.First().Message, ToastLevel.Error);
        }
        else
        {
            TempData.AddToast($"Step {stepNumber} removed.", ToastLevel.Success);
        }
        return RedirectToAction(nameof(Details), new { id });
    }

    // ---- Approver administration -------------------------------------------

    [HttpGet]
    [Authorize(Policy = AuthorizationPolicies.WorkflowDefinitionUpdate)]
    public IActionResult AddApprover(Guid id, int stepNumber) =>
        View(new AddWorkflowApproverFormViewModel { DefinitionId = id, StepNumber = (short)stepNumber });

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = AuthorizationPolicies.WorkflowDefinitionUpdate)]
    public async Task<IActionResult> AddApprover(Guid id, int stepNumber, AddWorkflowApproverFormViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            model.DefinitionId = id;
            model.StepNumber = (short)stepNumber;
            return View(model);
        }
        var result = await _definitions.AddApproverAsync(id, stepNumber, model.ToRequest(), cancellationToken).ConfigureAwait(false);
        if (result.IsFailure)
        {
            ModelState.AddResultErrors(result);
            model.DefinitionId = id;
            model.StepNumber = (short)stepNumber;
            return View(model);
        }
        TempData.AddToast($"Approver added to step {stepNumber}.", ToastLevel.Success);
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost("WorkflowDefinitions/{id:guid}/steps/{stepNumber:int}/approvers/{assigneeType}/{assigneeId:guid}/delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = AuthorizationPolicies.WorkflowDefinitionUpdate)]
    public async Task<IActionResult> RemoveApprover(Guid id, int stepNumber, string assigneeType, Guid assigneeId, CancellationToken cancellationToken)
    {
        var result = await _definitions.RemoveApproverAsync(id, stepNumber, assigneeType, assigneeId, cancellationToken).ConfigureAwait(false);
        if (result.IsFailure)
        {
            TempData.AddToast(result.Errors.First().Message, ToastLevel.Error);
        }
        else
        {
            TempData.AddToast("Approver removed.", ToastLevel.Success);
        }
        return RedirectToAction(nameof(Details), new { id });
    }

    // ---- Lifecycle transitions ---------------------------------------------

    [HttpPost("WorkflowDefinitions/{id:guid}/publish")]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = AuthorizationPolicies.WorkflowDefinitionPublish)]
    public async Task<IActionResult> Publish(Guid id, CancellationToken cancellationToken)
    {
        var result = await _definitions.PublishAsync(id, cancellationToken).ConfigureAwait(false);
        if (result.IsFailure) TempData.AddToast(result.Errors.First().Message, ToastLevel.Error);
        else TempData.AddToast("Definition published.", ToastLevel.Success);
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost("WorkflowDefinitions/{id:guid}/retire")]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = AuthorizationPolicies.WorkflowDefinitionRetire)]
    public async Task<IActionResult> Retire(Guid id, CancellationToken cancellationToken)
    {
        var result = await _definitions.RetireAsync(id, cancellationToken).ConfigureAwait(false);
        if (result.IsFailure) TempData.AddToast(result.Errors.First().Message, ToastLevel.Error);
        else TempData.AddToast("Definition retired.", ToastLevel.Success);
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost("WorkflowDefinitions/{id:guid}/clone")]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = AuthorizationPolicies.WorkflowDefinitionCreate)]
    public async Task<IActionResult> Clone(Guid id, CancellationToken cancellationToken)
    {
        var result = await _definitions.CloneAsync(id, cancellationToken).ConfigureAwait(false);
        if (result.IsFailure)
        {
            TempData.AddToast(result.Errors.First().Message, ToastLevel.Error);
            return RedirectToAction(nameof(Details), new { id });
        }
        TempData.AddToast("New draft version created.", ToastLevel.Success);
        return RedirectToAction(nameof(Details), new { id = result.Value });
    }
}
