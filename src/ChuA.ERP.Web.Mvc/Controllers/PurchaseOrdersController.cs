// Copyright (c) 2026 Alvin Wilsen Chan Chua
// GitHub: chuaalvw-y
// Licensed under the Alvin Wilsen Chan Chua Proprietary Use-Only License.
// See LICENSE.txt in the project root for full license information.

using ChuA.ERP.Web.Mvc.ApiClients;
using ChuA.ERP.Web.Mvc.Contracts.Common;
using ChuA.ERP.Web.Mvc.Extensions;
using ChuA.ERP.Web.Mvc.Security;
using ChuA.ERP.Web.Mvc.Utilities;
using ChuA.ERP.Web.Mvc.ViewModels.PurchaseOrders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChuA.ERP.Web.Mvc.Controllers;

/// <summary>UI for the Purchase Orders procurement module.</summary>
[Authorize]
public sealed class PurchaseOrdersController : Controller
{
    private readonly IPurchaseOrdersApiClient _purchaseOrders;
    private readonly IVendorsApiClient _vendors;
    private readonly IInventoryApiClient _inventory;

    public PurchaseOrdersController(IPurchaseOrdersApiClient purchaseOrders, IVendorsApiClient vendors, IInventoryApiClient inventory)
    {
        _purchaseOrders = purchaseOrders;
        _vendors = vendors;
        _inventory = inventory;
    }

    [HttpGet]
    [Authorize(Policy = AuthorizationPolicies.PurchaseOrderView)]
    public async Task<IActionResult> Index(Guid? vendorId, string? status, string? search, int pageNumber = 1, int pageSize = 25, string? sort = null, CancellationToken cancellationToken = default)
    {
        var result = await _purchaseOrders.ListAsync(vendorId, status, search, pageNumber, pageSize, sort, cancellationToken).ConfigureAwait(false);
        if (result.IsFailure)
        {
            ModelState.AddResultErrors(result);
            return View(new PurchaseOrderListViewModel { VendorId = vendorId, Status = status, Search = search, Vendors = await LoadVendorsAsync(cancellationToken).ConfigureAwait(false) });
        }

        ViewData["Search"] = search;
        ViewData["Breadcrumbs"] = new[]
        {
            new Breadcrumb("Dashboard", Url.Action("Index", "Dashboard")),
            new Breadcrumb("Purchase orders", null, true)
        };
        return View(new PurchaseOrderListViewModel
        {
            Page = result.Value,
            VendorId = vendorId,
            Status = status,
            Search = search,
            Vendors = await LoadVendorsAsync(cancellationToken).ConfigureAwait(false),
        });
    }

    [HttpGet]
    [Authorize(Policy = AuthorizationPolicies.PurchaseOrderView)]
    public async Task<IActionResult> Details(Guid id, CancellationToken cancellationToken)
    {
        var result = await _purchaseOrders.GetAsync(id, cancellationToken).ConfigureAwait(false);
        if (result.IsFailure)
        {
            TempData.AddToast(result.Errors.First().Message, ToastLevel.Error);
            return RedirectToAction(nameof(Index));
        }
        ViewData["Breadcrumbs"] = new[]
        {
            new Breadcrumb("Dashboard", Url.Action("Index", "Dashboard")),
            new Breadcrumb("Purchase orders", Url.Action(nameof(Index))),
            new Breadcrumb(result.Value.OrderNumber, null, true)
        };
        return View(result.Value);
    }

    [HttpGet]
    [Authorize(Policy = AuthorizationPolicies.PurchaseOrderCreate)]
    public async Task<IActionResult> Create(CancellationToken cancellationToken)
    {
        ViewData["Breadcrumbs"] = new[]
        {
            new Breadcrumb("Dashboard", Url.Action("Index", "Dashboard")),
            new Breadcrumb("Purchase orders", Url.Action(nameof(Index))),
            new Breadcrumb("New purchase order", null, true)
        };
        return View(await PopulateLookupsAsync(new PurchaseOrderFormViewModel
        {
            Lines = new List<PurchaseOrderLineFormViewModel> { new() }
        }, cancellationToken).ConfigureAwait(false));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = AuthorizationPolicies.PurchaseOrderCreate)]
    public async Task<IActionResult> Create(PurchaseOrderFormViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return View(await PopulateLookupsAsync(model, cancellationToken).ConfigureAwait(false));

        var result = await _purchaseOrders.CreateAsync(model.ToCreateRequest(), cancellationToken).ConfigureAwait(false);
        if (result.IsFailure)
        {
            ModelState.AddResultErrors(result);
            return View(await PopulateLookupsAsync(model, cancellationToken).ConfigureAwait(false));
        }
        TempData.AddToast($"Purchase order '{result.Value.OrderNumber}' created.", ToastLevel.Success);
        return RedirectToAction(nameof(Details), new { id = result.Value.Id });
    }

    [HttpGet]
    [Authorize(Policy = AuthorizationPolicies.PurchaseOrderUpdate)]
    public async Task<IActionResult> Edit(Guid id, CancellationToken cancellationToken)
    {
        var result = await _purchaseOrders.GetAsync(id, cancellationToken).ConfigureAwait(false);
        if (result.IsFailure)
        {
            TempData.AddToast(result.Errors.First().Message, ToastLevel.Error);
            return RedirectToAction(nameof(Index));
        }
        ViewData["Breadcrumbs"] = new[]
        {
            new Breadcrumb("Dashboard", Url.Action("Index", "Dashboard")),
            new Breadcrumb("Purchase orders", Url.Action(nameof(Index))),
            new Breadcrumb(result.Value.OrderNumber, Url.Action(nameof(Details), new { id })),
            new Breadcrumb("Edit", null, true)
        };
        return View(await PopulateLookupsAsync(PurchaseOrderFormViewModel.FromDto(result.Value), cancellationToken).ConfigureAwait(false));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = AuthorizationPolicies.PurchaseOrderUpdate)]
    public async Task<IActionResult> Edit(Guid id, PurchaseOrderFormViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return View(await PopulateLookupsAsync(model, cancellationToken).ConfigureAwait(false));

        var result = await _purchaseOrders.UpdateAsync(id, model.ToUpdateRequest(), cancellationToken).ConfigureAwait(false);
        if (result.IsFailure)
        {
            ModelState.AddResultErrors(result);
            return View(await PopulateLookupsAsync(model, cancellationToken).ConfigureAwait(false));
        }
        TempData.AddToast("Purchase order updated.", ToastLevel.Success);
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpGet]
    [Authorize(Policy = AuthorizationPolicies.PurchaseOrderDelete)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _purchaseOrders.GetAsync(id, cancellationToken).ConfigureAwait(false);
        if (result.IsFailure)
        {
            TempData.AddToast(result.Errors.First().Message, ToastLevel.Error);
            return RedirectToAction(nameof(Index));
        }
        ViewData["Breadcrumbs"] = new[]
        {
            new Breadcrumb("Dashboard", Url.Action("Index", "Dashboard")),
            new Breadcrumb("Purchase orders", Url.Action(nameof(Index))),
            new Breadcrumb(result.Value.OrderNumber, Url.Action(nameof(Details), new { id })),
            new Breadcrumb("Delete", null, true)
        };
        return View(result.Value);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = AuthorizationPolicies.PurchaseOrderDelete)]
    public async Task<IActionResult> DeleteConfirmed(Guid id, CancellationToken cancellationToken)
    {
        var result = await _purchaseOrders.DeleteAsync(id, cancellationToken).ConfigureAwait(false);
        if (result.IsFailure)
        {
            TempData.AddToast(result.Errors.First().Message, ToastLevel.Error);
            return RedirectToAction(nameof(Index));
        }
        TempData.AddToast("Purchase order deleted.", ToastLevel.Success);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [Authorize(Policy = AuthorizationPolicies.PurchaseOrderApprove)]
    public async Task<IActionResult> Approve(Guid id, CancellationToken cancellationToken)
    {
        var result = await _purchaseOrders.GetAsync(id, cancellationToken).ConfigureAwait(false);
        if (result.IsFailure)
        {
            TempData.AddToast(result.Errors.First().Message, ToastLevel.Error);
            return RedirectToAction(nameof(Index));
        }
        ViewData["Breadcrumbs"] = new[]
        {
            new Breadcrumb("Dashboard", Url.Action("Index", "Dashboard")),
            new Breadcrumb("Purchase orders", Url.Action(nameof(Index))),
            new Breadcrumb(result.Value.OrderNumber, Url.Action(nameof(Details), new { id })),
            new Breadcrumb("Approve", null, true)
        };
        return View(result.Value);
    }

    [HttpPost, ActionName("Approve")]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = AuthorizationPolicies.PurchaseOrderApprove)]
    public async Task<IActionResult> ApproveConfirmed(Guid id, CancellationToken cancellationToken)
    {
        var result = await _purchaseOrders.ApproveAsync(id, cancellationToken).ConfigureAwait(false);
        if (result.IsFailure)
        {
            TempData.AddToast(result.Errors.First().Message, ToastLevel.Error);
            return RedirectToAction(nameof(Details), new { id });
        }
        TempData.AddToast($"Purchase order '{result.Value.OrderNumber}' approved.", ToastLevel.Success);
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpGet]
    [Authorize(Policy = AuthorizationPolicies.PurchaseOrderReceive)]
    public async Task<IActionResult> Receive(Guid id, CancellationToken cancellationToken)
    {
        var result = await _purchaseOrders.GetAsync(id, cancellationToken).ConfigureAwait(false);
        if (result.IsFailure)
        {
            TempData.AddToast(result.Errors.First().Message, ToastLevel.Error);
            return RedirectToAction(nameof(Index));
        }
        ViewData["Breadcrumbs"] = new[]
        {
            new Breadcrumb("Dashboard", Url.Action("Index", "Dashboard")),
            new Breadcrumb("Purchase orders", Url.Action(nameof(Index))),
            new Breadcrumb(result.Value.OrderNumber, Url.Action(nameof(Details), new { id })),
            new Breadcrumb("Receive goods", null, true)
        };
        return View(new ReceiveGoodsFormViewModel
        {
            Id = id,
            Lines = new List<ReceiveGoodsLineFormViewModel> { new() }
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = AuthorizationPolicies.PurchaseOrderReceive)]
    public async Task<IActionResult> Receive(Guid id, ReceiveGoodsFormViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            model.Id = id;
            return View(model);
        }

        var result = await _purchaseOrders.ReceiveAsync(id, model.ToRequest(), cancellationToken).ConfigureAwait(false);
        if (result.IsFailure)
        {
            ModelState.AddResultErrors(result);
            model.Id = id;
            return View(model);
        }
        TempData.AddToast($"Goods receipt {result.Value} recorded.", ToastLevel.Success);
        return RedirectToAction(nameof(Details), new { id });
    }

    private async Task<IReadOnlyList<Contracts.Dtos.VendorDto>> LoadVendorsAsync(CancellationToken cancellationToken)
    {
        var result = await _vendors.ListAsync(pageSize: 200, sort: "legalName", cancellationToken: cancellationToken).ConfigureAwait(false);
        return result.IsSuccess ? result.Value.Items.ToArray() : Array.Empty<Contracts.Dtos.VendorDto>();
    }

    private async Task<IReadOnlyList<Contracts.Dtos.ItemDto>> LoadItemsAsync(CancellationToken cancellationToken)
    {
        var result = await _inventory.ListAsync(pageSize: 200, sort: "sku", cancellationToken: cancellationToken).ConfigureAwait(false);
        return result.IsSuccess ? result.Value.Items.ToArray() : Array.Empty<Contracts.Dtos.ItemDto>();
    }

    private async Task<PurchaseOrderFormViewModel> PopulateLookupsAsync(PurchaseOrderFormViewModel model, CancellationToken cancellationToken)
    {
        model.Vendors = await LoadVendorsAsync(cancellationToken).ConfigureAwait(false);
        model.Items = await LoadItemsAsync(cancellationToken).ConfigureAwait(false);
        return model;
    }
}
