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
    [Authorize(Policy = AuthorizationPolicies.VendorRead)]
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
    [Authorize(Policy = AuthorizationPolicies.VendorRead)]
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
}
