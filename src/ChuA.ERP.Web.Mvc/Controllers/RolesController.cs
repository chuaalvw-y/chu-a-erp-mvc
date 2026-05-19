using ChuA.ERP.Web.Mvc.ApiClients;
using ChuA.ERP.Web.Mvc.Contracts.Common;
using ChuA.ERP.Web.Mvc.Extensions;
using ChuA.ERP.Web.Mvc.Utilities;
using ChuA.ERP.Web.Mvc.ViewModels.Roles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChuA.ERP.Web.Mvc.Controllers;

/// <summary>UI for the Roles administration module.</summary>
[Authorize(Roles = "SystemAdmin,CompanyAdmin")]
public sealed class RolesController : Controller
{
    private readonly IRolesApiClient _roles;

    public RolesController(IRolesApiClient roles)
    {
        _roles = roles;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? search, int pageNumber = 1, int pageSize = 25, string? sort = null, CancellationToken cancellationToken = default)
    {
        var result = await _roles.ListAsync(search, pageNumber, pageSize, sort, cancellationToken).ConfigureAwait(false);
        if (result.IsFailure)
        {
            ModelState.AddResultErrors(result);
            return View(new RoleListViewModel { Search = search });
        }

        ViewData["Search"] = search;
        ViewData["Breadcrumbs"] = new[]
        {
            new Breadcrumb("Dashboard", Url.Action("Index", "Dashboard")),
            new Breadcrumb("Roles", null, true)
        };
        return View(new RoleListViewModel
        {
            Page = result.Value,
            Search = search,
        });
    }

    [HttpGet]
    public async Task<IActionResult> Details(Guid id, CancellationToken cancellationToken)
    {
        var result = await _roles.GetAsync(id, cancellationToken).ConfigureAwait(false);
        if (result.IsFailure)
        {
            TempData.AddToast(result.Errors.First().Message, ToastLevel.Error);
            return RedirectToAction(nameof(Index));
        }
        ViewData["Breadcrumbs"] = new[]
        {
            new Breadcrumb("Dashboard", Url.Action("Index", "Dashboard")),
            new Breadcrumb("Roles", Url.Action(nameof(Index))),
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
            new Breadcrumb("Roles", Url.Action(nameof(Index))),
            new Breadcrumb("New role", null, true)
        };
        return View(new RoleFormViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(RoleFormViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return View(model);

        var result = await _roles.CreateAsync(model.ToCreateRequest(), cancellationToken).ConfigureAwait(false);
        if (result.IsFailure)
        {
            ModelState.AddResultErrors(result);
            return View(model);
        }
        TempData.AddToast($"Role '{result.Value.Name}' created.", ToastLevel.Success);
        return RedirectToAction(nameof(Details), new { id = result.Value.Id });
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id, CancellationToken cancellationToken)
    {
        var result = await _roles.GetAsync(id, cancellationToken).ConfigureAwait(false);
        if (result.IsFailure)
        {
            TempData.AddToast(result.Errors.First().Message, ToastLevel.Error);
            return RedirectToAction(nameof(Index));
        }
        ViewData["Breadcrumbs"] = new[]
        {
            new Breadcrumb("Dashboard", Url.Action("Index", "Dashboard")),
            new Breadcrumb("Roles", Url.Action(nameof(Index))),
            new Breadcrumb(result.Value.Name, Url.Action(nameof(Details), new { id })),
            new Breadcrumb("Edit", null, true)
        };
        return View(RoleFormViewModel.FromDto(result.Value));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, RoleFormViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return View(model);

        var result = await _roles.UpdateAsync(id, model.ToUpdateRequest(), cancellationToken).ConfigureAwait(false);
        if (result.IsFailure)
        {
            ModelState.AddResultErrors(result);
            return View(model);
        }
        TempData.AddToast("Role updated.", ToastLevel.Success);
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpGet]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _roles.GetAsync(id, cancellationToken).ConfigureAwait(false);
        if (result.IsFailure)
        {
            TempData.AddToast(result.Errors.First().Message, ToastLevel.Error);
            return RedirectToAction(nameof(Index));
        }
        ViewData["Breadcrumbs"] = new[]
        {
            new Breadcrumb("Dashboard", Url.Action("Index", "Dashboard")),
            new Breadcrumb("Roles", Url.Action(nameof(Index))),
            new Breadcrumb(result.Value.Name, Url.Action(nameof(Details), new { id })),
            new Breadcrumb("Delete", null, true)
        };
        return View(result.Value);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid id, CancellationToken cancellationToken)
    {
        // Server-side guard: system roles cannot be deleted.
        var existing = await _roles.GetAsync(id, cancellationToken).ConfigureAwait(false);
        if (existing.IsFailure)
        {
            TempData.AddToast(existing.Errors.First().Message, ToastLevel.Error);
            return RedirectToAction(nameof(Index));
        }
        if (existing.Value.IsSystemRole)
        {
            TempData.AddToast("System roles cannot be deleted", ToastLevel.Error);
            return RedirectToAction(nameof(Details), new { id });
        }

        var result = await _roles.DeleteAsync(id, cancellationToken).ConfigureAwait(false);
        if (result.IsFailure)
        {
            TempData.AddToast(result.Errors.First().Message, ToastLevel.Error);
            return RedirectToAction(nameof(Index));
        }
        TempData.AddToast("Role deleted.", ToastLevel.Success);
        return RedirectToAction(nameof(Index));
    }
}
