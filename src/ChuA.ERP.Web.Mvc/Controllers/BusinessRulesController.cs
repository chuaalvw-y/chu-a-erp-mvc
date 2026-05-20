// Copyright (c) 2026 Alvin Wilsen Chan Chua
// GitHub: chuaalvw-y
// Licensed under the Alvin Wilsen Chan Chua Proprietary Use-Only License.
// See LICENSE.txt in the project root for full license information.

using ChuA.ERP.Web.Mvc.ApiClients;
using ChuA.ERP.Web.Mvc.Extensions;
using ChuA.ERP.Web.Mvc.Security;
using ChuA.ERP.Web.Mvc.Utilities;
using ChuA.ERP.Web.Mvc.ViewModels.BusinessRules;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChuA.ERP.Web.Mvc.Controllers;

/// <summary>
/// Read-only UI for configured business rules. V1 surfaces list +
/// details; create / update / delete arrive with the rule evaluator
/// in a later phase.
/// </summary>
[Authorize]
public sealed class BusinessRulesController : Controller
{
    private readonly IBusinessRulesApiClient _rules;

    public BusinessRulesController(IBusinessRulesApiClient rules)
    {
        _rules = rules;
    }

    [HttpGet]
    [Authorize(Policy = AuthorizationPolicies.BusinessRuleRead)]
    public async Task<IActionResult> Index(string? targetEntity, string? triggerEvent, CancellationToken cancellationToken)
    {
        var result = await _rules.ListAsync(targetEntity, triggerEvent, cancellationToken).ConfigureAwait(false);
        if (result.IsFailure)
        {
            ModelState.AddResultErrors(result);
            return View(new BusinessRuleListViewModel { TargetEntity = targetEntity, TriggerEvent = triggerEvent });
        }
        ViewData["Breadcrumbs"] = new[]
        {
            new Breadcrumb("Dashboard", Url.Action("Index", "Dashboard")),
            new Breadcrumb("Business rules", null, true)
        };
        return View(new BusinessRuleListViewModel
        {
            Rules = result.Value,
            TargetEntity = targetEntity,
            TriggerEvent = triggerEvent
        });
    }

    [HttpGet]
    [Authorize(Policy = AuthorizationPolicies.BusinessRuleRead)]
    public async Task<IActionResult> Details(Guid id, CancellationToken cancellationToken)
    {
        var result = await _rules.GetAsync(id, cancellationToken).ConfigureAwait(false);
        if (result.IsFailure)
        {
            TempData.AddToast(result.Errors.First().Message, ToastLevel.Error);
            return RedirectToAction(nameof(Index));
        }
        ViewData["Breadcrumbs"] = new[]
        {
            new Breadcrumb("Dashboard", Url.Action("Index", "Dashboard")),
            new Breadcrumb("Business rules", Url.Action(nameof(Index))),
            new Breadcrumb(result.Value.Code, null, true)
        };
        return View(result.Value);
    }
}
