// Copyright (c) 2026 Alvin Wilsen Chan Chua
// GitHub: chuaalvw-y
// Licensed under the Alvin Wilsen Chan Chua Proprietary Use-Only License.
// See LICENSE.txt in the project root for full license information.

using ChuA.ERP.Web.Mvc.ApiClients;
using ChuA.ERP.Web.Mvc.Contracts.Common;
using ChuA.ERP.Web.Mvc.Extensions;
using ChuA.ERP.Web.Mvc.Security;
using ChuA.ERP.Web.Mvc.Utilities;
using ChuA.ERP.Web.Mvc.ViewModels.ChartOfAccounts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChuA.ERP.Web.Mvc.Controllers;

/// <summary>UI for the Chart of Accounts master data module.</summary>
[Authorize]
public sealed class ChartOfAccountsController : Controller
{
    private readonly IChartOfAccountsApiClient _coa;

    public ChartOfAccountsController(IChartOfAccountsApiClient coa)
    {
        _coa = coa;
    }

    [HttpGet]
    [Authorize(Policy = AuthorizationPolicies.ChartOfAccountView)]
    public async Task<IActionResult> Index(string? accountType, string? search, int pageNumber = 1, int pageSize = 25, string? sort = null, CancellationToken cancellationToken = default)
    {
        var result = await _coa.ListAsync(accountType, search, pageNumber, pageSize, sort, cancellationToken).ConfigureAwait(false);
        if (result.IsFailure)
        {
            ModelState.AddResultErrors(result);
            return View(new ChartOfAccountListViewModel { Search = search, AccountType = accountType });
        }

        ViewData["Search"] = search;
        ViewData["AccountType"] = accountType;
        ViewData["Breadcrumbs"] = new[]
        {
            new Breadcrumb("Dashboard", Url.Action("Index", "Dashboard")),
            new Breadcrumb("Chart of accounts", null, true)
        };
        return View(new ChartOfAccountListViewModel
        {
            Page = result.Value,
            Search = search,
            AccountType = accountType,
        });
    }

    [HttpGet]
    [Authorize(Policy = AuthorizationPolicies.ChartOfAccountView)]
    public async Task<IActionResult> Details(Guid id, CancellationToken cancellationToken)
    {
        var result = await _coa.GetAsync(id, cancellationToken).ConfigureAwait(false);
        if (result.IsFailure)
        {
            TempData.AddToast(result.Errors.First().Message, ToastLevel.Error);
            return RedirectToAction(nameof(Index));
        }
        ViewData["Breadcrumbs"] = new[]
        {
            new Breadcrumb("Dashboard", Url.Action("Index", "Dashboard")),
            new Breadcrumb("Chart of accounts", Url.Action(nameof(Index))),
            new Breadcrumb(result.Value.Name, null, true)
        };
        return View(result.Value);
    }

    [HttpGet]
    [Authorize(Policy = AuthorizationPolicies.ChartOfAccountCreate)]
    public IActionResult Create()
    {
        ViewData["Breadcrumbs"] = new[]
        {
            new Breadcrumb("Dashboard", Url.Action("Index", "Dashboard")),
            new Breadcrumb("Chart of accounts", Url.Action(nameof(Index))),
            new Breadcrumb("New account", null, true)
        };
        return View(new ChartOfAccountFormViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = AuthorizationPolicies.ChartOfAccountCreate)]
    public async Task<IActionResult> Create(ChartOfAccountFormViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return View(model);

        var result = await _coa.CreateAsync(model.ToCreateRequest(), cancellationToken).ConfigureAwait(false);
        if (result.IsFailure)
        {
            ModelState.AddResultErrors(result);
            return View(model);
        }
        TempData.AddToast($"Account '{model.Name}' created.", ToastLevel.Success);
        return RedirectToAction(nameof(Details), new { id = result.Value });
    }

    [HttpGet]
    [Authorize(Policy = AuthorizationPolicies.ChartOfAccountUpdate)]
    public async Task<IActionResult> Edit(Guid id, CancellationToken cancellationToken)
    {
        var result = await _coa.GetAsync(id, cancellationToken).ConfigureAwait(false);
        if (result.IsFailure)
        {
            TempData.AddToast(result.Errors.First().Message, ToastLevel.Error);
            return RedirectToAction(nameof(Index));
        }
        ViewData["Breadcrumbs"] = new[]
        {
            new Breadcrumb("Dashboard", Url.Action("Index", "Dashboard")),
            new Breadcrumb("Chart of accounts", Url.Action(nameof(Index))),
            new Breadcrumb(result.Value.Name, Url.Action(nameof(Details), new { id })),
            new Breadcrumb("Edit", null, true)
        };
        return View(ChartOfAccountFormViewModel.FromDto(result.Value));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = AuthorizationPolicies.ChartOfAccountUpdate)]
    public async Task<IActionResult> Edit(Guid id, ChartOfAccountFormViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return View(model);

        var result = await _coa.UpdateAsync(id, model.ToUpdateRequest(), cancellationToken).ConfigureAwait(false);
        if (result.IsFailure)
        {
            ModelState.AddResultErrors(result);
            return View(model);
        }
        TempData.AddToast("Account updated.", ToastLevel.Success);
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpGet]
    [Authorize(Policy = AuthorizationPolicies.ChartOfAccountDelete)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _coa.GetAsync(id, cancellationToken).ConfigureAwait(false);
        if (result.IsFailure)
        {
            TempData.AddToast(result.Errors.First().Message, ToastLevel.Error);
            return RedirectToAction(nameof(Index));
        }
        ViewData["Breadcrumbs"] = new[]
        {
            new Breadcrumb("Dashboard", Url.Action("Index", "Dashboard")),
            new Breadcrumb("Chart of accounts", Url.Action(nameof(Index))),
            new Breadcrumb(result.Value.Name, Url.Action(nameof(Details), new { id })),
            new Breadcrumb("Delete", null, true)
        };
        return View(result.Value);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = AuthorizationPolicies.ChartOfAccountDelete)]
    public async Task<IActionResult> DeleteConfirmed(Guid id, CancellationToken cancellationToken)
    {
        var result = await _coa.DeleteAsync(id, cancellationToken).ConfigureAwait(false);
        if (result.IsFailure)
        {
            TempData.AddToast(result.Errors.First().Message, ToastLevel.Error);
            return RedirectToAction(nameof(Index));
        }
        TempData.AddToast("Account deleted.", ToastLevel.Success);
        return RedirectToAction(nameof(Index));
    }

    // ----- Reactive partial endpoints -----
    // Mirror the Vendors reactive pattern. Threads the accountType filter alongside search.

    [HttpGet]
    [Authorize(Policy = AuthorizationPolicies.ChartOfAccountView)]
    public async Task<IActionResult> IndexPartial(string? accountType, string? search, int pageNumber = 1, int pageSize = 25, string? sort = null, CancellationToken cancellationToken = default)
    {
        var result = await _coa.ListAsync(accountType, search, pageNumber, pageSize, sort, cancellationToken).ConfigureAwait(false);
        if (result.IsFailure)
        {
            ModelState.AddResultErrors(result);
            return PartialView("_ChartOfAccountRowsPartial", new ChartOfAccountListViewModel { Search = search, AccountType = accountType });
        }
        return PartialView("_ChartOfAccountRowsPartial", new ChartOfAccountListViewModel { Page = result.Value, Search = search, AccountType = accountType });
    }

    [HttpGet]
    [Authorize(Policy = AuthorizationPolicies.ChartOfAccountView)]
    public async Task<IActionResult> RowPartial(Guid id, CancellationToken cancellationToken)
    {
        var result = await _coa.GetAsync(id, cancellationToken).ConfigureAwait(false);
        if (result.IsFailure) return NotFound();
        return PartialView("_ChartOfAccountRow", result.Value);
    }

    [HttpGet]
    [Authorize(Policy = AuthorizationPolicies.ChartOfAccountCreate)]
    public IActionResult CreateModal() => PartialView("_ChartOfAccountFormModal", new ChartOfAccountFormViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = AuthorizationPolicies.ChartOfAccountCreate)]
    public async Task<IActionResult> CreateModal(ChartOfAccountFormViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            Response.StatusCode = StatusCodes.Status422UnprocessableEntity;
            return PartialView("_ChartOfAccountFormModal", model);
        }
        var result = await _coa.CreateAsync(model.ToCreateRequest(), cancellationToken).ConfigureAwait(false);
        if (result.IsFailure)
        {
            ModelState.AddResultErrors(result);
            Response.StatusCode = StatusCodes.Status422UnprocessableEntity;
            return PartialView("_ChartOfAccountFormModal", model);
        }
        var refresh = await _coa.GetAsync(result.Value, cancellationToken).ConfigureAwait(false);
        if (refresh.IsFailure) return NotFound();
        Response.Headers["X-Chua-Row-Action"] = "create";
        Response.Headers["X-Chua-Coa-Id"] = result.Value.ToString();
        return PartialView("_ChartOfAccountRow", refresh.Value);
    }

    [HttpGet]
    [Authorize(Policy = AuthorizationPolicies.ChartOfAccountUpdate)]
    public async Task<IActionResult> EditModal(Guid id, CancellationToken cancellationToken)
    {
        var result = await _coa.GetAsync(id, cancellationToken).ConfigureAwait(false);
        if (result.IsFailure) return NotFound();
        return PartialView("_ChartOfAccountFormModal", ChartOfAccountFormViewModel.FromDto(result.Value));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = AuthorizationPolicies.ChartOfAccountUpdate)]
    public async Task<IActionResult> EditModal(Guid id, ChartOfAccountFormViewModel model, CancellationToken cancellationToken)
    {
        model.Id = id;
        if (!ModelState.IsValid)
        {
            Response.StatusCode = StatusCodes.Status422UnprocessableEntity;
            return PartialView("_ChartOfAccountFormModal", model);
        }
        var result = await _coa.UpdateAsync(id, model.ToUpdateRequest(), cancellationToken).ConfigureAwait(false);
        if (result.IsFailure)
        {
            ModelState.AddResultErrors(result);
            Response.StatusCode = StatusCodes.Status422UnprocessableEntity;
            return PartialView("_ChartOfAccountFormModal", model);
        }
        var refresh = await _coa.GetAsync(id, cancellationToken).ConfigureAwait(false);
        if (refresh.IsFailure) return NotFound();
        Response.Headers["X-Chua-Row-Action"] = "update";
        Response.Headers["X-Chua-Coa-Id"] = id.ToString();
        return PartialView("_ChartOfAccountRow", refresh.Value);
    }
}
