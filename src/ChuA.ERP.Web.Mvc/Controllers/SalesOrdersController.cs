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

    public SalesOrdersController(ISalesOrdersApiClient salesOrders)
    {
        _salesOrders = salesOrders;
    }

    [HttpGet]
    [Authorize(Policy = AuthorizationPolicies.SalesOrderRead)]
    public async Task<IActionResult> Index(Guid? customerId, string? status, string? search, int pageNumber = 1, int pageSize = 25, CancellationToken cancellationToken = default)
    {
        var result = await _salesOrders.ListAsync(customerId, status, search, cancellationToken).ConfigureAwait(false);
        if (result.IsFailure)
        {
            ModelState.AddResultErrors(result);
            return View(new SalesOrderListViewModel { CustomerId = customerId, Status = status, Search = search });
        }

        ViewData["Search"] = search;
        ViewData["Breadcrumbs"] = new[]
        {
            new Breadcrumb("Dashboard", Url.Action("Index", "Dashboard")),
            new Breadcrumb("Sales orders", null, true)
        };
        return View(new SalesOrderListViewModel
        {
            Page = PagedResult<Contracts.Dtos.SalesOrderDto>.FromCollection(result.Value, pageNumber, pageSize),
            CustomerId = customerId,
            Status = status,
            Search = search,
        });
    }

    [HttpGet]
    [Authorize(Policy = AuthorizationPolicies.SalesOrderRead)]
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
    public IActionResult Create()
    {
        ViewData["Breadcrumbs"] = new[]
        {
            new Breadcrumb("Dashboard", Url.Action("Index", "Dashboard")),
            new Breadcrumb("Sales orders", Url.Action(nameof(Index))),
            new Breadcrumb("New sales order", null, true)
        };
        return View(new SalesOrderFormViewModel
        {
            Lines = new List<SalesOrderLineFormViewModel> { new() }
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = AuthorizationPolicies.SalesOrderCreate)]
    public async Task<IActionResult> Create(SalesOrderFormViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return View(model);

        var result = await _salesOrders.CreateAsync(model.ToCreateRequest(), cancellationToken).ConfigureAwait(false);
        if (result.IsFailure)
        {
            ModelState.AddResultErrors(result);
            return View(model);
        }
        TempData.AddToast($"Sales order '{result.Value.OrderNumber}' created.", ToastLevel.Success);
        return RedirectToAction(nameof(Details), new { id = result.Value.Id });
    }

    [HttpGet]
    [Authorize(Policy = AuthorizationPolicies.SalesOrderCreate)]
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
        return View(SalesOrderFormViewModel.FromDto(result.Value));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = AuthorizationPolicies.SalesOrderCreate)]
    public async Task<IActionResult> Edit(Guid id, SalesOrderFormViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return View(model);

        var result = await _salesOrders.UpdateAsync(id, model.ToUpdateRequest(), cancellationToken).ConfigureAwait(false);
        if (result.IsFailure)
        {
            ModelState.AddResultErrors(result);
            return View(model);
        }
        TempData.AddToast("Sales order updated.", ToastLevel.Success);
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpGet]
    [Authorize(Policy = AuthorizationPolicies.SalesOrderCreate)]
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
    [Authorize(Policy = AuthorizationPolicies.SalesOrderCreate)]
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
}
