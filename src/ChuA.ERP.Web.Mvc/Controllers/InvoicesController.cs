// Copyright (c) 2026 Alvin Wilsen Chan Chua
// GitHub: chuaalvw-y
// Licensed under the Alvin Wilsen Chan Chua Proprietary Use-Only License.
// See LICENSE.txt in the project root for full license information.

using ChuA.ERP.Web.Mvc.ApiClients;
using ChuA.ERP.Web.Mvc.Contracts.Common;
using ChuA.ERP.Web.Mvc.Extensions;
using ChuA.ERP.Web.Mvc.Security;
using ChuA.ERP.Web.Mvc.Utilities;
using ChuA.ERP.Web.Mvc.ViewModels.Invoices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChuA.ERP.Web.Mvc.Controllers;

/// <summary>UI for the Invoices (AR) module.</summary>
[Authorize]
public sealed class InvoicesController : Controller
{
    private readonly IInvoicesApiClient _invoices;
    private readonly ICustomersApiClient _customers;

    public InvoicesController(IInvoicesApiClient invoices, ICustomersApiClient customers)
    {
        _invoices = invoices;
        _customers = customers;
    }

    [HttpGet]
    [Authorize(Policy = AuthorizationPolicies.InvoiceView)]
    public async Task<IActionResult> Index(Guid? customerId, string? status, string? paymentStatus, string? search, int pageNumber = 1, int pageSize = 25, string? sort = null, CancellationToken cancellationToken = default)
    {
        var result = await _invoices.ListAsync(customerId, status, paymentStatus, search, pageNumber, pageSize, sort, cancellationToken).ConfigureAwait(false);
        if (result.IsFailure)
        {
            ModelState.AddResultErrors(result);
            return View(new InvoiceListViewModel { CustomerId = customerId, Status = status, PaymentStatus = paymentStatus, Search = search, Customers = await LoadCustomersAsync(cancellationToken).ConfigureAwait(false) });
        }

        ViewData["Search"] = search;
        ViewData["Breadcrumbs"] = new[]
        {
            new Breadcrumb("Dashboard", Url.Action("Index", "Dashboard")),
            new Breadcrumb("Invoices", null, true)
        };
        return View(new InvoiceListViewModel
        {
            Page = result.Value,
            CustomerId = customerId,
            Status = status,
            PaymentStatus = paymentStatus,
            Search = search,
            Customers = await LoadCustomersAsync(cancellationToken).ConfigureAwait(false),
        });
    }

    [HttpGet]
    [Authorize(Policy = AuthorizationPolicies.InvoiceView)]
    public async Task<IActionResult> Details(Guid id, CancellationToken cancellationToken)
    {
        var result = await _invoices.GetAsync(id, cancellationToken).ConfigureAwait(false);
        if (result.IsFailure)
        {
            TempData.AddToast(result.Errors.First().Message, ToastLevel.Error);
            return RedirectToAction(nameof(Index));
        }
        ViewData["Breadcrumbs"] = new[]
        {
            new Breadcrumb("Dashboard", Url.Action("Index", "Dashboard")),
            new Breadcrumb("Invoices", Url.Action(nameof(Index))),
            new Breadcrumb(result.Value.InvoiceNumber, null, true)
        };
        return View(result.Value);
    }

    [HttpGet]
    [Authorize(Policy = AuthorizationPolicies.InvoiceCreate)]
    public async Task<IActionResult> Create(CancellationToken cancellationToken)
    {
        ViewData["Breadcrumbs"] = new[]
        {
            new Breadcrumb("Dashboard", Url.Action("Index", "Dashboard")),
            new Breadcrumb("Invoices", Url.Action(nameof(Index))),
            new Breadcrumb("New invoice", null, true)
        };
        return View(await PopulateLookupsAsync(new InvoiceFormViewModel(), cancellationToken).ConfigureAwait(false));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = AuthorizationPolicies.InvoiceCreate)]
    public async Task<IActionResult> Create(InvoiceFormViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return View(await PopulateLookupsAsync(model, cancellationToken).ConfigureAwait(false));

        var result = await _invoices.CreateAsync(model.ToCreateRequest(), cancellationToken).ConfigureAwait(false);
        if (result.IsFailure)
        {
            ModelState.AddResultErrors(result);
            return View(await PopulateLookupsAsync(model, cancellationToken).ConfigureAwait(false));
        }
        TempData.AddToast($"Invoice '{result.Value.InvoiceNumber}' created.", ToastLevel.Success);
        return RedirectToAction(nameof(Details), new { id = result.Value.Id });
    }

    [HttpGet]
    [Authorize(Policy = AuthorizationPolicies.InvoiceUpdate)]
    public async Task<IActionResult> Edit(Guid id, CancellationToken cancellationToken)
    {
        var result = await _invoices.GetAsync(id, cancellationToken).ConfigureAwait(false);
        if (result.IsFailure)
        {
            TempData.AddToast(result.Errors.First().Message, ToastLevel.Error);
            return RedirectToAction(nameof(Index));
        }
        ViewData["Breadcrumbs"] = new[]
        {
            new Breadcrumb("Dashboard", Url.Action("Index", "Dashboard")),
            new Breadcrumb("Invoices", Url.Action(nameof(Index))),
            new Breadcrumb(result.Value.InvoiceNumber, Url.Action(nameof(Details), new { id })),
            new Breadcrumb("Edit", null, true)
        };
        return View(await PopulateLookupsAsync(InvoiceFormViewModel.FromDto(result.Value), cancellationToken).ConfigureAwait(false));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = AuthorizationPolicies.InvoiceUpdate)]
    public async Task<IActionResult> Edit(Guid id, InvoiceFormViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return View(await PopulateLookupsAsync(model, cancellationToken).ConfigureAwait(false));

        var result = await _invoices.UpdateAsync(id, model.ToUpdateRequest(), cancellationToken).ConfigureAwait(false);
        if (result.IsFailure)
        {
            ModelState.AddResultErrors(result);
            return View(await PopulateLookupsAsync(model, cancellationToken).ConfigureAwait(false));
        }
        TempData.AddToast("Invoice updated.", ToastLevel.Success);
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpGet]
    [Authorize(Policy = AuthorizationPolicies.InvoiceDelete)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _invoices.GetAsync(id, cancellationToken).ConfigureAwait(false);
        if (result.IsFailure)
        {
            TempData.AddToast(result.Errors.First().Message, ToastLevel.Error);
            return RedirectToAction(nameof(Index));
        }
        ViewData["Breadcrumbs"] = new[]
        {
            new Breadcrumb("Dashboard", Url.Action("Index", "Dashboard")),
            new Breadcrumb("Invoices", Url.Action(nameof(Index))),
            new Breadcrumb(result.Value.InvoiceNumber, Url.Action(nameof(Details), new { id })),
            new Breadcrumb("Delete", null, true)
        };
        return View(result.Value);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = AuthorizationPolicies.InvoiceDelete)]
    public async Task<IActionResult> DeleteConfirmed(Guid id, CancellationToken cancellationToken)
    {
        var result = await _invoices.DeleteAsync(id, cancellationToken).ConfigureAwait(false);
        if (result.IsFailure)
        {
            TempData.AddToast(result.Errors.First().Message, ToastLevel.Error);
            return RedirectToAction(nameof(Index));
        }
        TempData.AddToast("Invoice deleted.", ToastLevel.Success);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [Authorize(Policy = AuthorizationPolicies.InvoiceApplyPayment)]
    public async Task<IActionResult> ApplyPayment(Guid id, CancellationToken cancellationToken)
    {
        var result = await _invoices.GetAsync(id, cancellationToken).ConfigureAwait(false);
        if (result.IsFailure)
        {
            TempData.AddToast(result.Errors.First().Message, ToastLevel.Error);
            return RedirectToAction(nameof(Index));
        }
        ViewData["Breadcrumbs"] = new[]
        {
            new Breadcrumb("Dashboard", Url.Action("Index", "Dashboard")),
            new Breadcrumb("Invoices", Url.Action(nameof(Index))),
            new Breadcrumb(result.Value.InvoiceNumber, Url.Action(nameof(Details), new { id })),
            new Breadcrumb("Apply payment", null, true)
        };
        ViewData["Invoice"] = result.Value;
        return View(new InvoicePaymentFormViewModel
        {
            Id = result.Value.Id,
            Amount = result.Value.OutstandingBalance.Amount,
            CurrencyCode = result.Value.OutstandingBalance.CurrencyCode,
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = AuthorizationPolicies.InvoiceApplyPayment)]
    public async Task<IActionResult> ApplyPayment(Guid id, InvoicePaymentFormViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            var inv = await _invoices.GetAsync(id, cancellationToken).ConfigureAwait(false);
            if (inv.IsSuccess) ViewData["Invoice"] = inv.Value;
            return View(model);
        }

        var result = await _invoices.ApplyPaymentAsync(id, model.ToRequest(), cancellationToken).ConfigureAwait(false);
        if (result.IsFailure)
        {
            ModelState.AddResultErrors(result);
            var inv = await _invoices.GetAsync(id, cancellationToken).ConfigureAwait(false);
            if (inv.IsSuccess) ViewData["Invoice"] = inv.Value;
            return View(model);
        }
        TempData.AddToast($"Payment applied to '{result.Value.InvoiceNumber}'.", ToastLevel.Success);
        return RedirectToAction(nameof(Details), new { id });
    }

    private async Task<IReadOnlyList<Contracts.Dtos.CustomerDto>> LoadCustomersAsync(CancellationToken cancellationToken)
    {
        var result = await _customers.ListAsync(pageSize: 200, sort: "legalName", cancellationToken: cancellationToken).ConfigureAwait(false);
        return result.IsSuccess ? result.Value.Items.ToArray() : Array.Empty<Contracts.Dtos.CustomerDto>();
    }

    private async Task<InvoiceFormViewModel> PopulateLookupsAsync(InvoiceFormViewModel model, CancellationToken cancellationToken)
    {
        model.Customers = await LoadCustomersAsync(cancellationToken).ConfigureAwait(false);
        return model;
    }
}
