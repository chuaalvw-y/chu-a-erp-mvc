// Copyright (c) 2026 Alvin Wilsen Chan Chua
// GitHub: chuaalvw-y
// Licensed under the Alvin Wilsen Chan Chua Proprietary Use-Only License.
// See LICENSE.txt in the project root for full license information.

using ChuA.ERP.Web.Mvc.ApiClients;
using ChuA.ERP.Web.Mvc.Contracts.Common;
using ChuA.ERP.Web.Mvc.Extensions;
using ChuA.ERP.Web.Mvc.Security;
using ChuA.ERP.Web.Mvc.Utilities;
using ChuA.ERP.Web.Mvc.ViewModels.Vendors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChuA.ERP.Web.Mvc.Controllers;

/// <summary>UI for the Vendors master data module.</summary>
[Authorize]
public sealed class VendorsController : Controller
{
    private readonly IVendorsApiClient _vendors;

    public VendorsController(IVendorsApiClient vendors)
    {
        _vendors = vendors;
    }

    [HttpGet]
    [Authorize(Policy = AuthorizationPolicies.VendorView)]
    public async Task<IActionResult> Index(string? search, int pageNumber = 1, int pageSize = 25, string? sort = null, CancellationToken cancellationToken = default)
    {
        var result = await _vendors.ListAsync(search, pageNumber, pageSize, sort, cancellationToken).ConfigureAwait(false);
        if (result.IsFailure)
        {
            ModelState.AddResultErrors(result);
            return View(new VendorListViewModel { Search = search });
        }

        ViewData["Search"] = search;
        ViewData["Breadcrumbs"] = new[]
        {
            new Breadcrumb("Dashboard", Url.Action("Index", "Dashboard")),
            new Breadcrumb("Vendors", null, true)
        };
        return View(new VendorListViewModel
        {
            Page = result.Value,
            Search = search,
        });
    }

    [HttpGet]
    [Authorize(Policy = AuthorizationPolicies.VendorView)]
    public async Task<IActionResult> Details(Guid id, CancellationToken cancellationToken)
    {
        var result = await _vendors.GetAsync(id, cancellationToken).ConfigureAwait(false);
        if (result.IsFailure)
        {
            TempData.AddToast(result.Errors.First().Message, ToastLevel.Error);
            return RedirectToAction(nameof(Index));
        }
        ViewData["Breadcrumbs"] = new[]
        {
            new Breadcrumb("Dashboard", Url.Action("Index", "Dashboard")),
            new Breadcrumb("Vendors", Url.Action(nameof(Index))),
            new Breadcrumb(result.Value.LegalName, null, true)
        };
        return View(result.Value);
    }

    [HttpGet]
    [Authorize(Policy = AuthorizationPolicies.VendorCreate)]
    public IActionResult Create()
    {
        ViewData["Breadcrumbs"] = new[]
        {
            new Breadcrumb("Dashboard", Url.Action("Index", "Dashboard")),
            new Breadcrumb("Vendors", Url.Action(nameof(Index))),
            new Breadcrumb("New vendor", null, true)
        };
        return View(new VendorFormViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = AuthorizationPolicies.VendorCreate)]
    public async Task<IActionResult> Create(VendorFormViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return View(model);

        var result = await _vendors.CreateAsync(model.ToCreateRequest(), cancellationToken).ConfigureAwait(false);
        if (result.IsFailure)
        {
            ModelState.AddResultErrors(result);
            return View(model);
        }
        TempData.AddToast($"Vendor '{result.Value.LegalName}' created.", ToastLevel.Success);
        return RedirectToAction(nameof(Details), new { id = result.Value.Id });
    }

    [HttpGet]
    [Authorize(Policy = AuthorizationPolicies.VendorUpdate)]
    public async Task<IActionResult> Edit(Guid id, CancellationToken cancellationToken)
    {
        var result = await _vendors.GetAsync(id, cancellationToken).ConfigureAwait(false);
        if (result.IsFailure)
        {
            TempData.AddToast(result.Errors.First().Message, ToastLevel.Error);
            return RedirectToAction(nameof(Index));
        }
        ViewData["Breadcrumbs"] = new[]
        {
            new Breadcrumb("Dashboard", Url.Action("Index", "Dashboard")),
            new Breadcrumb("Vendors", Url.Action(nameof(Index))),
            new Breadcrumb(result.Value.LegalName, Url.Action(nameof(Details), new { id })),
            new Breadcrumb("Edit", null, true)
        };
        return View(VendorFormViewModel.FromDto(result.Value));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = AuthorizationPolicies.VendorUpdate)]
    public async Task<IActionResult> Edit(Guid id, VendorFormViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return View(model);

        var result = await _vendors.UpdateAsync(id, model.ToUpdateRequest(), cancellationToken).ConfigureAwait(false);
        if (result.IsFailure)
        {
            ModelState.AddResultErrors(result);
            return View(model);
        }
        TempData.AddToast("Vendor updated.", ToastLevel.Success);
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpGet]
    [Authorize(Policy = AuthorizationPolicies.VendorDelete)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _vendors.GetAsync(id, cancellationToken).ConfigureAwait(false);
        if (result.IsFailure)
        {
            TempData.AddToast(result.Errors.First().Message, ToastLevel.Error);
            return RedirectToAction(nameof(Index));
        }
        ViewData["Breadcrumbs"] = new[]
        {
            new Breadcrumb("Dashboard", Url.Action("Index", "Dashboard")),
            new Breadcrumb("Vendors", Url.Action(nameof(Index))),
            new Breadcrumb(result.Value.LegalName, Url.Action(nameof(Details), new { id })),
            new Breadcrumb("Delete", null, true)
        };
        return View(result.Value);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = AuthorizationPolicies.VendorDelete)]
    public async Task<IActionResult> DeleteConfirmed(Guid id, CancellationToken cancellationToken)
    {
        var result = await _vendors.DeleteAsync(id, cancellationToken).ConfigureAwait(false);
        if (result.IsFailure)
        {
            TempData.AddToast(result.Errors.First().Message, ToastLevel.Error);
            return RedirectToAction(nameof(Index));
        }
        TempData.AddToast("Vendor deleted.", ToastLevel.Success);
        return RedirectToAction(nameof(Index));
    }

    // ----- Reactive partial endpoints -----
    // These supplement the existing Index/Create/Edit/Delete actions; they never replace
    // them so no-JS clients keep working via the original PRG flow. The partial endpoints
    // are consumed by chua-vendors.js — modal create/edit + per-row refresh + filtered
    // table reload without a full page render.

    /// <summary>
    /// Returns the table-body partial for the current search/page/sort. The reactive JS
    /// hits this when the user types in the search box or pages so only the rows refresh.
    /// </summary>
    [HttpGet]
    [Authorize(Policy = AuthorizationPolicies.VendorView)]
    public async Task<IActionResult> IndexPartial(string? search, int pageNumber = 1, int pageSize = 25, string? sort = null, CancellationToken cancellationToken = default)
    {
        var result = await _vendors.ListAsync(search, pageNumber, pageSize, sort, cancellationToken).ConfigureAwait(false);
        if (result.IsFailure)
        {
            ModelState.AddResultErrors(result);
            return PartialView("_VendorRowsPartial", new VendorListViewModel { Search = search });
        }
        return PartialView("_VendorRowsPartial", new VendorListViewModel { Page = result.Value, Search = search });
    }

    /// <summary>Returns a single row partial for the supplied vendor.</summary>
    [HttpGet]
    [Authorize(Policy = AuthorizationPolicies.VendorView)]
    public async Task<IActionResult> RowPartial(Guid id, CancellationToken cancellationToken)
    {
        var result = await _vendors.GetAsync(id, cancellationToken).ConfigureAwait(false);
        if (result.IsFailure) return NotFound();
        return PartialView("_VendorRow", result.Value);
    }

    /// <summary>Returns the create-modal partial.</summary>
    [HttpGet]
    [Authorize(Policy = AuthorizationPolicies.VendorCreate)]
    public IActionResult CreateModal() => PartialView("_VendorFormModal", new VendorFormViewModel());

    /// <summary>
    /// Modal create submit. Returns the row partial (HTTP 200) on success, or the modal
    /// partial again (HTTP 422) with validation errors. The JS layer distinguishes via
    /// the status code.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = AuthorizationPolicies.VendorCreate)]
    public async Task<IActionResult> CreateModal(VendorFormViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            Response.StatusCode = StatusCodes.Status422UnprocessableEntity;
            return PartialView("_VendorFormModal", model);
        }
        var result = await _vendors.CreateAsync(model.ToCreateRequest(), cancellationToken).ConfigureAwait(false);
        if (result.IsFailure)
        {
            ModelState.AddResultErrors(result);
            Response.StatusCode = StatusCodes.Status422UnprocessableEntity;
            return PartialView("_VendorFormModal", model);
        }
        Response.Headers["X-Chua-Row-Action"] = "create";
        Response.Headers["X-Chua-Vendor-Id"] = result.Value.Id.ToString();
        return PartialView("_VendorRow", result.Value);
    }

    /// <summary>Returns the edit-modal partial pre-populated for the supplied vendor.</summary>
    [HttpGet]
    [Authorize(Policy = AuthorizationPolicies.VendorUpdate)]
    public async Task<IActionResult> EditModal(Guid id, CancellationToken cancellationToken)
    {
        var result = await _vendors.GetAsync(id, cancellationToken).ConfigureAwait(false);
        if (result.IsFailure) return NotFound();
        return PartialView("_VendorFormModal", VendorFormViewModel.FromDto(result.Value));
    }

    /// <summary>Modal edit submit. Same response semantics as <see cref="CreateModal(VendorFormViewModel, CancellationToken)"/>.</summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = AuthorizationPolicies.VendorUpdate)]
    public async Task<IActionResult> EditModal(Guid id, VendorFormViewModel model, CancellationToken cancellationToken)
    {
        model.Id = id;
        if (!ModelState.IsValid)
        {
            Response.StatusCode = StatusCodes.Status422UnprocessableEntity;
            return PartialView("_VendorFormModal", model);
        }
        var result = await _vendors.UpdateAsync(id, model.ToUpdateRequest(), cancellationToken).ConfigureAwait(false);
        if (result.IsFailure)
        {
            ModelState.AddResultErrors(result);
            Response.StatusCode = StatusCodes.Status422UnprocessableEntity;
            return PartialView("_VendorFormModal", model);
        }
        // Re-fetch so the row partial sees the canonical server-side projection.
        var refresh = await _vendors.GetAsync(id, cancellationToken).ConfigureAwait(false);
        if (refresh.IsFailure) return NotFound();
        Response.Headers["X-Chua-Row-Action"] = "update";
        Response.Headers["X-Chua-Vendor-Id"] = id.ToString();
        return PartialView("_VendorRow", refresh.Value);
    }
}
