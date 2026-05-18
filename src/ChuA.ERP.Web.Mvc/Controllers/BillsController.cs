using ChuA.ERP.Web.Mvc.ApiClients;
using ChuA.ERP.Web.Mvc.Contracts.Common;
using ChuA.ERP.Web.Mvc.Extensions;
using ChuA.ERP.Web.Mvc.Security;
using ChuA.ERP.Web.Mvc.Utilities;
using ChuA.ERP.Web.Mvc.ViewModels.Bills;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChuA.ERP.Web.Mvc.Controllers;

/// <summary>UI for the Bills (AP) module.</summary>
[Authorize]
public sealed class BillsController : Controller
{
    private readonly IBillsApiClient _bills;

    public BillsController(IBillsApiClient bills)
    {
        _bills = bills;
    }

    [HttpGet]
    [Authorize(Policy = AuthorizationPolicies.BillRead)]
    public async Task<IActionResult> Index(Guid? vendorId, string? status, string? paymentStatus, string? search, int pageNumber = 1, int pageSize = 25, CancellationToken cancellationToken = default)
    {
        var result = await _bills.ListAsync(vendorId, status, paymentStatus, search, cancellationToken).ConfigureAwait(false);
        if (result.IsFailure)
        {
            ModelState.AddResultErrors(result);
            return View(new BillListViewModel { VendorId = vendorId, Status = status, PaymentStatus = paymentStatus, Search = search });
        }

        ViewData["Search"] = search;
        ViewData["Breadcrumbs"] = new[]
        {
            new Breadcrumb("Dashboard", Url.Action("Index", "Dashboard")),
            new Breadcrumb("Bills", null, true)
        };
        return View(new BillListViewModel
        {
            Page = PagedResult<Contracts.Dtos.BillDto>.FromCollection(result.Value, pageNumber, pageSize),
            VendorId = vendorId,
            Status = status,
            PaymentStatus = paymentStatus,
            Search = search,
        });
    }

    [HttpGet]
    [Authorize(Policy = AuthorizationPolicies.BillRead)]
    public async Task<IActionResult> AwaitingApproval(int pageNumber = 1, int pageSize = 25, CancellationToken cancellationToken = default)
    {
        var result = await _bills.GetAwaitingApprovalAsync(cancellationToken).ConfigureAwait(false);
        if (result.IsFailure)
        {
            ModelState.AddResultErrors(result);
            return View(new BillListViewModel());
        }

        ViewData["Breadcrumbs"] = new[]
        {
            new Breadcrumb("Dashboard", Url.Action("Index", "Dashboard")),
            new Breadcrumb("Bills", Url.Action(nameof(Index))),
            new Breadcrumb("Awaiting approval", null, true)
        };
        return View(new BillListViewModel
        {
            Page = PagedResult<Contracts.Dtos.BillDto>.FromCollection(result.Value, pageNumber, pageSize),
        });
    }

    [HttpGet]
    [Authorize(Policy = AuthorizationPolicies.BillRead)]
    public async Task<IActionResult> Details(Guid id, CancellationToken cancellationToken)
    {
        var result = await _bills.GetAsync(id, cancellationToken).ConfigureAwait(false);
        if (result.IsFailure)
        {
            TempData.AddToast(result.Errors.First().Message, ToastLevel.Error);
            return RedirectToAction(nameof(Index));
        }
        ViewData["Breadcrumbs"] = new[]
        {
            new Breadcrumb("Dashboard", Url.Action("Index", "Dashboard")),
            new Breadcrumb("Bills", Url.Action(nameof(Index))),
            new Breadcrumb(result.Value.BillNumber, null, true)
        };
        return View(result.Value);
    }

    [HttpGet]
    [Authorize(Policy = AuthorizationPolicies.BillCreate)]
    public IActionResult Create()
    {
        ViewData["Breadcrumbs"] = new[]
        {
            new Breadcrumb("Dashboard", Url.Action("Index", "Dashboard")),
            new Breadcrumb("Bills", Url.Action(nameof(Index))),
            new Breadcrumb("New bill", null, true)
        };
        return View(new BillFormViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = AuthorizationPolicies.BillCreate)]
    public async Task<IActionResult> Create(BillFormViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return View(model);

        var result = await _bills.CreateAsync(model.ToCreateRequest(), cancellationToken).ConfigureAwait(false);
        if (result.IsFailure)
        {
            ModelState.AddResultErrors(result);
            return View(model);
        }
        TempData.AddToast($"Bill '{result.Value.BillNumber}' created.", ToastLevel.Success);
        return RedirectToAction(nameof(Details), new { id = result.Value.Id });
    }

    [HttpGet]
    [Authorize(Policy = AuthorizationPolicies.BillCreate)]
    public async Task<IActionResult> Edit(Guid id, CancellationToken cancellationToken)
    {
        var result = await _bills.GetAsync(id, cancellationToken).ConfigureAwait(false);
        if (result.IsFailure)
        {
            TempData.AddToast(result.Errors.First().Message, ToastLevel.Error);
            return RedirectToAction(nameof(Index));
        }
        ViewData["Breadcrumbs"] = new[]
        {
            new Breadcrumb("Dashboard", Url.Action("Index", "Dashboard")),
            new Breadcrumb("Bills", Url.Action(nameof(Index))),
            new Breadcrumb(result.Value.BillNumber, Url.Action(nameof(Details), new { id })),
            new Breadcrumb("Edit", null, true)
        };
        return View(BillFormViewModel.FromDto(result.Value));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = AuthorizationPolicies.BillCreate)]
    public async Task<IActionResult> Edit(Guid id, BillFormViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return View(model);

        var result = await _bills.UpdateAsync(id, model.ToUpdateRequest(), cancellationToken).ConfigureAwait(false);
        if (result.IsFailure)
        {
            ModelState.AddResultErrors(result);
            return View(model);
        }
        TempData.AddToast("Bill updated.", ToastLevel.Success);
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpGet]
    [Authorize(Policy = AuthorizationPolicies.BillCreate)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _bills.GetAsync(id, cancellationToken).ConfigureAwait(false);
        if (result.IsFailure)
        {
            TempData.AddToast(result.Errors.First().Message, ToastLevel.Error);
            return RedirectToAction(nameof(Index));
        }
        ViewData["Breadcrumbs"] = new[]
        {
            new Breadcrumb("Dashboard", Url.Action("Index", "Dashboard")),
            new Breadcrumb("Bills", Url.Action(nameof(Index))),
            new Breadcrumb(result.Value.BillNumber, Url.Action(nameof(Details), new { id })),
            new Breadcrumb("Delete", null, true)
        };
        return View(result.Value);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = AuthorizationPolicies.BillCreate)]
    public async Task<IActionResult> DeleteConfirmed(Guid id, CancellationToken cancellationToken)
    {
        var result = await _bills.DeleteAsync(id, cancellationToken).ConfigureAwait(false);
        if (result.IsFailure)
        {
            TempData.AddToast(result.Errors.First().Message, ToastLevel.Error);
            return RedirectToAction(nameof(Index));
        }
        TempData.AddToast("Bill deleted.", ToastLevel.Success);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [Authorize(Policy = AuthorizationPolicies.BillApprove)]
    public async Task<IActionResult> Approve(Guid id, CancellationToken cancellationToken)
    {
        var result = await _bills.GetAsync(id, cancellationToken).ConfigureAwait(false);
        if (result.IsFailure)
        {
            TempData.AddToast(result.Errors.First().Message, ToastLevel.Error);
            return RedirectToAction(nameof(Index));
        }
        ViewData["Breadcrumbs"] = new[]
        {
            new Breadcrumb("Dashboard", Url.Action("Index", "Dashboard")),
            new Breadcrumb("Bills", Url.Action(nameof(Index))),
            new Breadcrumb(result.Value.BillNumber, Url.Action(nameof(Details), new { id })),
            new Breadcrumb("Approve", null, true)
        };
        return View(result.Value);
    }

    [HttpPost, ActionName("Approve")]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = AuthorizationPolicies.BillApprove)]
    public async Task<IActionResult> ApproveConfirmed(Guid id, CancellationToken cancellationToken)
    {
        var result = await _bills.ApproveAsync(id, cancellationToken).ConfigureAwait(false);
        if (result.IsFailure)
        {
            TempData.AddToast(result.Errors.First().Message, ToastLevel.Error);
            return RedirectToAction(nameof(Details), new { id });
        }
        TempData.AddToast($"Bill '{result.Value.BillNumber}' approved.", ToastLevel.Success);
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpGet]
    [Authorize(Policy = AuthorizationPolicies.BillPay)]
    public async Task<IActionResult> Pay(Guid id, CancellationToken cancellationToken)
    {
        var result = await _bills.GetAsync(id, cancellationToken).ConfigureAwait(false);
        if (result.IsFailure)
        {
            TempData.AddToast(result.Errors.First().Message, ToastLevel.Error);
            return RedirectToAction(nameof(Index));
        }
        ViewData["Breadcrumbs"] = new[]
        {
            new Breadcrumb("Dashboard", Url.Action("Index", "Dashboard")),
            new Breadcrumb("Bills", Url.Action(nameof(Index))),
            new Breadcrumb(result.Value.BillNumber, Url.Action(nameof(Details), new { id })),
            new Breadcrumb("Pay", null, true)
        };
        ViewData["Bill"] = result.Value;
        return View(new PayBillFormViewModel
        {
            Id = result.Value.Id,
            Amount = result.Value.OutstandingBalance.Amount,
            CurrencyCode = result.Value.OutstandingBalance.CurrencyCode,
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = AuthorizationPolicies.BillPay)]
    public async Task<IActionResult> Pay(Guid id, PayBillFormViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            var bill = await _bills.GetAsync(id, cancellationToken).ConfigureAwait(false);
            if (bill.IsSuccess) ViewData["Bill"] = bill.Value;
            return View(model);
        }

        var result = await _bills.PayAsync(id, model.ToRequest(), cancellationToken).ConfigureAwait(false);
        if (result.IsFailure)
        {
            ModelState.AddResultErrors(result);
            var bill = await _bills.GetAsync(id, cancellationToken).ConfigureAwait(false);
            if (bill.IsSuccess) ViewData["Bill"] = bill.Value;
            return View(model);
        }
        TempData.AddToast($"Payment recorded against '{result.Value.BillNumber}'.", ToastLevel.Success);
        return RedirectToAction(nameof(Details), new { id });
    }
}
