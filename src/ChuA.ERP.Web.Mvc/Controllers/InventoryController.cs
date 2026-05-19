using ChuA.ERP.Web.Mvc.ApiClients;
using ChuA.ERP.Web.Mvc.Contracts.Common;
using ChuA.ERP.Web.Mvc.Extensions;
using ChuA.ERP.Web.Mvc.Security;
using ChuA.ERP.Web.Mvc.Utilities;
using ChuA.ERP.Web.Mvc.ViewModels.Inventory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChuA.ERP.Web.Mvc.Controllers;

/// <summary>UI for the Inventory (items) module.</summary>
[Authorize]
public sealed class InventoryController : Controller
{
    private readonly IInventoryApiClient _inventory;

    public InventoryController(IInventoryApiClient inventory)
    {
        _inventory = inventory;
    }

    [HttpGet]
    [Authorize(Policy = AuthorizationPolicies.InventoryRead)]
    public async Task<IActionResult> Index(string? search, int pageNumber = 1, int pageSize = 25, string? sort = null, CancellationToken cancellationToken = default)
    {
        var result = await _inventory.ListAsync(search, pageNumber, pageSize, sort, cancellationToken).ConfigureAwait(false);
        if (result.IsFailure)
        {
            ModelState.AddResultErrors(result);
            return View(new InventoryListViewModel { Search = search });
        }

        ViewData["Search"] = search;
        ViewData["Breadcrumbs"] = new[]
        {
            new Breadcrumb("Dashboard", Url.Action("Index", "Dashboard")),
            new Breadcrumb("Inventory", null, true)
        };
        return View(new InventoryListViewModel
        {
            Page = result.Value,
            Search = search,
        });
    }

    [HttpGet]
    [Authorize(Policy = AuthorizationPolicies.InventoryRead)]
    public async Task<IActionResult> Details(Guid id, CancellationToken cancellationToken)
    {
        var result = await _inventory.GetAsync(id, cancellationToken).ConfigureAwait(false);
        if (result.IsFailure)
        {
            TempData.AddToast(result.Errors.First().Message, ToastLevel.Error);
            return RedirectToAction(nameof(Index));
        }
        ViewData["Breadcrumbs"] = new[]
        {
            new Breadcrumb("Dashboard", Url.Action("Index", "Dashboard")),
            new Breadcrumb("Inventory", Url.Action(nameof(Index))),
            new Breadcrumb(result.Value.Name, null, true)
        };
        return View(result.Value);
    }

    [HttpGet]
    [Authorize(Policy = AuthorizationPolicies.InventoryCreate)]
    public IActionResult Create()
    {
        ViewData["Breadcrumbs"] = new[]
        {
            new Breadcrumb("Dashboard", Url.Action("Index", "Dashboard")),
            new Breadcrumb("Inventory", Url.Action(nameof(Index))),
            new Breadcrumb("New item", null, true)
        };
        return View(new ItemFormViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = AuthorizationPolicies.InventoryCreate)]
    public async Task<IActionResult> Create(ItemFormViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return View(model);

        var result = await _inventory.CreateAsync(model.ToCreateRequest(), cancellationToken).ConfigureAwait(false);
        if (result.IsFailure)
        {
            ModelState.AddResultErrors(result);
            return View(model);
        }
        TempData.AddToast($"Item '{result.Value.Name}' created.", ToastLevel.Success);
        return RedirectToAction(nameof(Details), new { id = result.Value.Id });
    }

    [HttpGet]
    [Authorize(Policy = AuthorizationPolicies.InventoryUpdate)]
    public async Task<IActionResult> Edit(Guid id, CancellationToken cancellationToken)
    {
        var result = await _inventory.GetAsync(id, cancellationToken).ConfigureAwait(false);
        if (result.IsFailure)
        {
            TempData.AddToast(result.Errors.First().Message, ToastLevel.Error);
            return RedirectToAction(nameof(Index));
        }
        ViewData["Breadcrumbs"] = new[]
        {
            new Breadcrumb("Dashboard", Url.Action("Index", "Dashboard")),
            new Breadcrumb("Inventory", Url.Action(nameof(Index))),
            new Breadcrumb(result.Value.Name, Url.Action(nameof(Details), new { id })),
            new Breadcrumb("Edit", null, true)
        };
        return View(ItemFormViewModel.FromDto(result.Value));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = AuthorizationPolicies.InventoryUpdate)]
    public async Task<IActionResult> Edit(Guid id, ItemFormViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return View(model);

        var result = await _inventory.UpdateAsync(id, model.ToUpdateRequest(), cancellationToken).ConfigureAwait(false);
        if (result.IsFailure)
        {
            ModelState.AddResultErrors(result);
            return View(model);
        }
        TempData.AddToast("Item updated.", ToastLevel.Success);
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpGet]
    [Authorize(Policy = AuthorizationPolicies.InventoryDelete)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _inventory.GetAsync(id, cancellationToken).ConfigureAwait(false);
        if (result.IsFailure)
        {
            TempData.AddToast(result.Errors.First().Message, ToastLevel.Error);
            return RedirectToAction(nameof(Index));
        }
        ViewData["Breadcrumbs"] = new[]
        {
            new Breadcrumb("Dashboard", Url.Action("Index", "Dashboard")),
            new Breadcrumb("Inventory", Url.Action(nameof(Index))),
            new Breadcrumb(result.Value.Name, Url.Action(nameof(Details), new { id })),
            new Breadcrumb("Delete", null, true)
        };
        return View(result.Value);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = AuthorizationPolicies.InventoryDelete)]
    public async Task<IActionResult> DeleteConfirmed(Guid id, CancellationToken cancellationToken)
    {
        var result = await _inventory.DeleteAsync(id, cancellationToken).ConfigureAwait(false);
        if (result.IsFailure)
        {
            TempData.AddToast(result.Errors.First().Message, ToastLevel.Error);
            return RedirectToAction(nameof(Index));
        }
        TempData.AddToast("Item deleted.", ToastLevel.Success);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [Authorize(Policy = AuthorizationPolicies.InventoryAdjust)]
    public async Task<IActionResult> Adjust(Guid id, CancellationToken cancellationToken)
    {
        var result = await _inventory.GetAsync(id, cancellationToken).ConfigureAwait(false);
        if (result.IsFailure)
        {
            TempData.AddToast(result.Errors.First().Message, ToastLevel.Error);
            return RedirectToAction(nameof(Index));
        }
        ViewData["Item"] = result.Value;
        ViewData["Breadcrumbs"] = new[]
        {
            new Breadcrumb("Dashboard", Url.Action("Index", "Dashboard")),
            new Breadcrumb("Inventory", Url.Action(nameof(Index))),
            new Breadcrumb(result.Value.Name, Url.Action(nameof(Details), new { id })),
            new Breadcrumb("Adjust", null, true)
        };
        return View(new AdjustInventoryFormViewModel
        {
            Id = id,
            DeltaUnitOfMeasure = result.Value.DefaultUnitOfMeasure,
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = AuthorizationPolicies.InventoryAdjust)]
    public async Task<IActionResult> Adjust(Guid id, AdjustInventoryFormViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            model.Id = id;
            return View(model);
        }

        var result = await _inventory.AdjustAsync(id, model.ToRequest(), cancellationToken).ConfigureAwait(false);
        if (result.IsFailure)
        {
            ModelState.AddResultErrors(result);
            model.Id = id;
            return View(model);
        }
        TempData.AddToast("Inventory adjustment recorded.", ToastLevel.Success);
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpGet]
    [Authorize(Policy = AuthorizationPolicies.InventoryRead)]
    public async Task<IActionResult> Balance(Guid itemId, Guid warehouseId, CancellationToken cancellationToken)
    {
        var itemResult = await _inventory.GetAsync(itemId, cancellationToken).ConfigureAwait(false);
        if (itemResult.IsFailure)
        {
            TempData.AddToast(itemResult.Errors.First().Message, ToastLevel.Error);
            return RedirectToAction(nameof(Index));
        }

        var balanceResult = await _inventory.GetBalanceAsync(itemId, warehouseId, cancellationToken).ConfigureAwait(false);
        if (balanceResult.IsFailure)
        {
            TempData.AddToast(balanceResult.Errors.First().Message, ToastLevel.Error);
            return RedirectToAction(nameof(Details), new { id = itemId });
        }

        ViewData["Breadcrumbs"] = new[]
        {
            new Breadcrumb("Dashboard", Url.Action("Index", "Dashboard")),
            new Breadcrumb("Inventory", Url.Action(nameof(Index))),
            new Breadcrumb(itemResult.Value.Name, Url.Action(nameof(Details), new { id = itemId })),
            new Breadcrumb("Balance", null, true)
        };

        return View(new InventoryBalanceViewModel
        {
            ItemId = balanceResult.Value.ItemId,
            WarehouseId = balanceResult.Value.WarehouseId,
            OnHand = balanceResult.Value.OnHand,
            Available = balanceResult.Value.Available,
            Item = itemResult.Value,
        });
    }

    /// <summary>
    /// UI-only demo of the full-page loading overlay (progress + cancel). The
    /// import endpoint is not implemented server-side yet; this view shows what
    /// the long-running-job UX looks like by simulating progress in the browser.
    /// Wire it to a real chunked import (SSE/WebSocket) when available.
    /// </summary>
    [HttpGet]
    [Authorize(Policy = AuthorizationPolicies.InventoryCreate)]
    public IActionResult ImportDemo()
    {
        ViewData["Breadcrumbs"] = new[]
        {
            new Breadcrumb("Dashboard", Url.Action("Index", "Dashboard")),
            new Breadcrumb("Inventory", Url.Action(nameof(Index))),
            new Breadcrumb("Import items", null, true)
        };
        return View();
    }
}
