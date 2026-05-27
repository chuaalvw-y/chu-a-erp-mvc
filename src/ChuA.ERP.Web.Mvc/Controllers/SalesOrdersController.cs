// Copyright (c) 2026 Alvin Wilsen Chan Chua
// GitHub: chuaalvw-y
// Licensed under the Alvin Wilsen Chan Chua Proprietary Use-Only License.
// See LICENSE.txt in the project root for full license information.

using ChuA.ERP.Web.Mvc.ApiClients;
using ChuA.ERP.Web.Mvc.Contracts.Common;
using ChuA.ERP.Web.Mvc.Extensions;
using ChuA.ERP.Web.Mvc.Security;
using ChuA.ERP.Web.Mvc.Utilities;
using ChuA.ERP.Web.Mvc.ViewModels.SalesOrders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChuA.ERP.Web.Mvc.Controllers;

/// <summary>UI for the Sales Orders module.</summary>
[Authorize]
public sealed class SalesOrdersController : Controller
{
    private readonly ISalesOrdersApiClient _salesOrders;
    private readonly ICustomersApiClient _customers;
    private readonly IInventoryApiClient _inventory;

    public SalesOrdersController(ISalesOrdersApiClient salesOrders, ICustomersApiClient customers, IInventoryApiClient inventory)
    {
        _salesOrders = salesOrders;
        _customers = customers;
        _inventory = inventory;
    }

    [HttpGet]
    [Authorize(Policy = AuthorizationPolicies.SalesOrderView)]
    public async Task<IActionResult> Index(Guid? customerId, string? status, string? search, int pageNumber = 1, int pageSize = 25, string? sort = null, CancellationToken cancellationToken = default)
    {
        var result = await _salesOrders.ListAsync(customerId, status, search, pageNumber, pageSize, sort, cancellationToken).ConfigureAwait(false);
        if (result.IsFailure)
        {
            ModelState.AddResultErrors(result);
            return View(new SalesOrderListViewModel { CustomerId = customerId, Status = status, Search = search, Customers = await LoadCustomersAsync(cancellationToken).ConfigureAwait(false) });
        }

        ViewData["Search"] = search;
        ViewData["Breadcrumbs"] = new[]
        {
            new Breadcrumb("Dashboard", Url.Action("Index", "Dashboard")),
            new Breadcrumb("Sales orders", null, true)
        };
        return View(new SalesOrderListViewModel
        {
            Page = result.Value,
            CustomerId = customerId,
            Status = status,
            Search = search,
            Customers = await LoadCustomersAsync(cancellationToken).ConfigureAwait(false),
        });
    }

    [HttpGet]
    [Authorize(Policy = AuthorizationPolicies.SalesOrderView)]
    public async Task<IActionResult> Details(Guid id, CancellationToken cancellationToken)
    {
        var result = await _salesOrders.GetAsync(id, cancellationToken).ConfigureAwait(false);
        if (result.IsFailure)
        {
            TempData.AddToast(result.Errors.First().Message, ToastLevel.Error);
            return RedirectToAction(nameof(Index));
        }
        ViewData["Breadcrumbs"] = new[]
        {
            new Breadcrumb("Dashboard", Url.Action("Index", "Dashboard")),
            new Breadcrumb("Sales orders", Url.Action(nameof(Index))),
            new Breadcrumb(result.Value.OrderNumber, null, true)
        };
        return View(result.Value);
    }

    [HttpGet]
    [Authorize(Policy = AuthorizationPolicies.SalesOrderCreate)]
    public async Task<IActionResult> Create(CancellationToken cancellationToken)
    {
        ViewData["Breadcrumbs"] = new[]
        {
            new Breadcrumb("Dashboard", Url.Action("Index", "Dashboard")),
            new Breadcrumb("Sales orders", Url.Action(nameof(Index))),
            new Breadcrumb("New sales order", null, true)
        };
        return View(await PopulateLookupsAsync(new SalesOrderFormViewModel
        {
            Lines = new List<SalesOrderLineFormViewModel> { new() }
        }, cancellationToken).ConfigureAwait(false));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = AuthorizationPolicies.SalesOrderCreate)]
    public async Task<IActionResult> Create(SalesOrderFormViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return View(await PopulateLookupsAsync(model, cancellationToken).ConfigureAwait(false));

        var result = await _salesOrders.CreateAsync(model.ToCreateRequest(), cancellationToken).ConfigureAwait(false);
        if (result.IsFailure)
        {
            ModelState.AddResultErrors(result);
            return View(await PopulateLookupsAsync(model, cancellationToken).ConfigureAwait(false));
        }
        TempData.AddToast($"Sales order '{result.Value.OrderNumber}' created.", ToastLevel.Success);
        return RedirectToAction(nameof(Details), new { id = result.Value.Id });
    }

    [HttpGet]
    [Authorize(Policy = AuthorizationPolicies.SalesOrderUpdate)]
    public async Task<IActionResult> Edit(Guid id, CancellationToken cancellationToken)
    {
        var result = await _salesOrders.GetAsync(id, cancellationToken).ConfigureAwait(false);
        if (result.IsFailure)
        {
            TempData.AddToast(result.Errors.First().Message, ToastLevel.Error);
            return RedirectToAction(nameof(Index));
        }
        ViewData["Breadcrumbs"] = new[]
        {
            new Breadcrumb("Dashboard", Url.Action("Index", "Dashboard")),
            new Breadcrumb("Sales orders", Url.Action(nameof(Index))),
            new Breadcrumb(result.Value.OrderNumber, Url.Action(nameof(Details), new { id })),
            new Breadcrumb("Edit", null, true)
        };
        return View(await PopulateLookupsAsync(SalesOrderFormViewModel.FromDto(result.Value), cancellationToken).ConfigureAwait(false));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = AuthorizationPolicies.SalesOrderUpdate)]
    public async Task<IActionResult> Edit(Guid id, SalesOrderFormViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return View(await PopulateLookupsAsync(model, cancellationToken).ConfigureAwait(false));

        var result = await _salesOrders.UpdateAsync(id, model.ToUpdateRequest(), cancellationToken).ConfigureAwait(false);
        if (result.IsFailure)
        {
            ModelState.AddResultErrors(result);
            return View(await PopulateLookupsAsync(model, cancellationToken).ConfigureAwait(false));
        }
        TempData.AddToast("Sales order updated.", ToastLevel.Success);
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpGet]
    [Authorize(Policy = AuthorizationPolicies.SalesOrderDelete)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _salesOrders.GetAsync(id, cancellationToken).ConfigureAwait(false);
        if (result.IsFailure)
        {
            TempData.AddToast(result.Errors.First().Message, ToastLevel.Error);
            return RedirectToAction(nameof(Index));
        }
        ViewData["Breadcrumbs"] = new[]
        {
            new Breadcrumb("Dashboard", Url.Action("Index", "Dashboard")),
            new Breadcrumb("Sales orders", Url.Action(nameof(Index))),
            new Breadcrumb(result.Value.OrderNumber, Url.Action(nameof(Details), new { id })),
            new Breadcrumb("Delete", null, true)
        };
        return View(result.Value);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = AuthorizationPolicies.SalesOrderDelete)]
    public async Task<IActionResult> DeleteConfirmed(Guid id, CancellationToken cancellationToken)
    {
        var result = await _salesOrders.DeleteAsync(id, cancellationToken).ConfigureAwait(false);
        if (result.IsFailure)
        {
            TempData.AddToast(result.Errors.First().Message, ToastLevel.Error);
            return RedirectToAction(nameof(Index));
        }
        TempData.AddToast("Sales order deleted.", ToastLevel.Success);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [Authorize(Policy = AuthorizationPolicies.SalesOrderShip)]
    public async Task<IActionResult> Ship(Guid id, CancellationToken cancellationToken)
    {
        var result = await _salesOrders.GetAsync(id, cancellationToken).ConfigureAwait(false);
        if (result.IsFailure)
        {
            TempData.AddToast(result.Errors.First().Message, ToastLevel.Error);
            return RedirectToAction(nameof(Index));
        }
        ViewData["Breadcrumbs"] = new[]
        {
            new Breadcrumb("Dashboard", Url.Action("Index", "Dashboard")),
            new Breadcrumb("Sales orders", Url.Action(nameof(Index))),
            new Breadcrumb(result.Value.OrderNumber, Url.Action(nameof(Details), new { id })),
            new Breadcrumb("Ship", null, true)
        };
        return View(new ShipSalesOrderFormViewModel
        {
            Id = id,
            Lines = new List<ShipSalesOrderLineFormViewModel> { new() }
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = AuthorizationPolicies.SalesOrderShip)]
    public async Task<IActionResult> Ship(Guid id, ShipSalesOrderFormViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            model.Id = id;
            return View(model);
        }

        var result = await _salesOrders.ShipAsync(id, model.ToRequest(), cancellationToken).ConfigureAwait(false);
        if (result.IsFailure)
        {
            ModelState.AddResultErrors(result);
            model.Id = id;
            return View(model);
        }
        TempData.AddToast($"Shipment {result.Value} recorded.", ToastLevel.Success);
        return RedirectToAction(nameof(Details), new { id });
    }

    private async Task<IReadOnlyList<Contracts.Dtos.CustomerDto>> LoadCustomersAsync(CancellationToken cancellationToken)
    {
        var result = await _customers.ListAsync(pageSize: 200, sort: "legalName", cancellationToken: cancellationToken).ConfigureAwait(false);
        return result.IsSuccess ? result.Value.Items.ToArray() : Array.Empty<Contracts.Dtos.CustomerDto>();
    }

    private async Task<IReadOnlyList<Contracts.Dtos.ItemDto>> LoadItemsAsync(CancellationToken cancellationToken)
    {
        var result = await _inventory.ListAsync(pageSize: 200, sort: "sku", cancellationToken: cancellationToken).ConfigureAwait(false);
        return result.IsSuccess ? result.Value.Items.ToArray() : Array.Empty<Contracts.Dtos.ItemDto>();
    }

    private async Task<SalesOrderFormViewModel> PopulateLookupsAsync(SalesOrderFormViewModel model, CancellationToken cancellationToken)
    {
        model.Customers = await LoadCustomersAsync(cancellationToken).ConfigureAwait(false);
        model.Items = await LoadItemsAsync(cancellationToken).ConfigureAwait(false);
        return model;
    }
}
