using ChuA.ERP.Web.Mvc.ApiClients;
using ChuA.ERP.Web.Mvc.Contracts.Common;
using ChuA.ERP.Web.Mvc.Extensions;
using ChuA.ERP.Web.Mvc.Utilities;
using ChuA.ERP.Web.Mvc.ViewModels.Companies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChuA.ERP.Web.Mvc.Controllers;

/// <summary>UI for the Companies admin module. Restricted to SystemAdmin.</summary>
[Authorize(Roles = "SystemAdmin")]
public sealed class CompaniesController : Controller
{
    private readonly ICompaniesApiClient _companies;

    public CompaniesController(ICompaniesApiClient companies)
    {
        _companies = companies;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? search, int pageNumber = 1, int pageSize = 25, CancellationToken cancellationToken = default)
    {
        var result = await _companies.ListAsync(search, cancellationToken).ConfigureAwait(false);
        if (result.IsFailure)
        {
            ModelState.AddResultErrors(result);
            return View(new CompanyListViewModel { Search = search });
        }

        ViewData["Search"] = search;
        ViewData["Breadcrumbs"] = new[]
        {
            new Breadcrumb("Dashboard", Url.Action("Index", "Dashboard")),
            new Breadcrumb("Companies", null, true)
        };
        return View(new CompanyListViewModel
        {
            Page = PagedResult<Contracts.Dtos.CompanyDto>.FromCollection(result.Value, pageNumber, pageSize),
            Search = search,
        });
    }

    [HttpGet]
    public async Task<IActionResult> Details(Guid id, CancellationToken cancellationToken)
    {
        var result = await _companies.GetAsync(id, cancellationToken).ConfigureAwait(false);
        if (result.IsFailure)
        {
            TempData.AddToast(result.Errors.First().Message, ToastLevel.Error);
            return RedirectToAction(nameof(Index));
        }
        ViewData["Breadcrumbs"] = new[]
        {
            new Breadcrumb("Dashboard", Url.Action("Index", "Dashboard")),
            new Breadcrumb("Companies", Url.Action(nameof(Index))),
            new Breadcrumb(result.Value.Name, null, true)
        };
        return View(result.Value);
    }

    [HttpGet]
    public IActionResult Create()
    {
        ViewData["Breadcrumbs"] = new[]
        {
            new Breadcrumb("Dashboard", Url.Action("Index", "Dashboard")),
            new Breadcrumb("Companies", Url.Action(nameof(Index))),
            new Breadcrumb("New company", null, true)
        };
        return View(new CompanyFormViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CompanyFormViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return View(model);

        var result = await _companies.CreateAsync(model.ToCreateRequest(), cancellationToken).ConfigureAwait(false);
        if (result.IsFailure)
        {
            ModelState.AddResultErrors(result);
            return View(model);
        }
        TempData.AddToast($"Company '{result.Value.Name}' created.", ToastLevel.Success);
        return RedirectToAction(nameof(Details), new { id = result.Value.Id });
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id, CancellationToken cancellationToken)
    {
        var result = await _companies.GetAsync(id, cancellationToken).ConfigureAwait(false);
        if (result.IsFailure)
        {
            TempData.AddToast(result.Errors.First().Message, ToastLevel.Error);
            return RedirectToAction(nameof(Index));
        }
        ViewData["Breadcrumbs"] = new[]
        {
            new Breadcrumb("Dashboard", Url.Action("Index", "Dashboard")),
            new Breadcrumb("Companies", Url.Action(nameof(Index))),
            new Breadcrumb(result.Value.Name, Url.Action(nameof(Details), new { id })),
            new Breadcrumb("Edit", null, true)
        };
        return View(CompanyFormViewModel.FromDto(result.Value));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, CompanyFormViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return View(model);

        var result = await _companies.UpdateAsync(id, model.ToUpdateRequest(), cancellationToken).ConfigureAwait(false);
        if (result.IsFailure)
        {
            ModelState.AddResultErrors(result);
            return View(model);
        }
        TempData.AddToast("Company updated.", ToastLevel.Success);
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpGet]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _companies.GetAsync(id, cancellationToken).ConfigureAwait(false);
        if (result.IsFailure)
        {
            TempData.AddToast(result.Errors.First().Message, ToastLevel.Error);
            return RedirectToAction(nameof(Index));
        }
        ViewData["Breadcrumbs"] = new[]
        {
            new Breadcrumb("Dashboard", Url.Action("Index", "Dashboard")),
            new Breadcrumb("Companies", Url.Action(nameof(Index))),
            new Breadcrumb(result.Value.Name, Url.Action(nameof(Details), new { id })),
            new Breadcrumb("Delete", null, true)
        };
        return View(result.Value);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid id, CancellationToken cancellationToken)
    {
        var result = await _companies.DeleteAsync(id, cancellationToken).ConfigureAwait(false);
        if (result.IsFailure)
        {
            TempData.AddToast(result.Errors.First().Message, ToastLevel.Error);
            return RedirectToAction(nameof(Index));
        }
        TempData.AddToast("Company deleted.", ToastLevel.Success);
        return RedirectToAction(nameof(Index));
    }
}
