using ChuA.ERP.Web.Mvc.ApiClients;
using ChuA.ERP.Web.Mvc.Contracts.Common;
using ChuA.ERP.Web.Mvc.Extensions;
using ChuA.ERP.Web.Mvc.Security;
using ChuA.ERP.Web.Mvc.Utilities;
using ChuA.ERP.Web.Mvc.ViewModels.JournalEntries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChuA.ERP.Web.Mvc.Controllers;

/// <summary>UI for the Journal entries (general ledger) module.</summary>
[Authorize]
[Authorize(Policy = AuthorizationPolicies.JournalEntryRead)]
public sealed class JournalEntriesController : Controller
{
    private readonly IJournalEntriesApiClient _entries;

    public JournalEntriesController(IJournalEntriesApiClient entries)
    {
        _entries = entries;
    }

    [HttpGet]
    public async Task<IActionResult> Index(Guid? fiscalPeriodId, string? status, int pageNumber = 1, int pageSize = 25, CancellationToken cancellationToken = default)
    {
        var result = await _entries.ListAsync(fiscalPeriodId, status, cancellationToken).ConfigureAwait(false);
        if (result.IsFailure)
        {
            ModelState.AddResultErrors(result);
            return View(new JournalEntryListViewModel { FiscalPeriodId = fiscalPeriodId, Status = status });
        }

        ViewData["Breadcrumbs"] = new[]
        {
            new Breadcrumb("Dashboard", Url.Action("Index", "Dashboard")),
            new Breadcrumb("Journal entries", null, true)
        };
        return View(new JournalEntryListViewModel
        {
            Page = PagedResult<Contracts.Dtos.JournalEntryDto>.FromCollection(result.Value, pageNumber, pageSize),
            FiscalPeriodId = fiscalPeriodId,
            Status = status,
        });
    }

    [HttpGet]
    public async Task<IActionResult> Details(Guid id, CancellationToken cancellationToken)
    {
        var result = await _entries.GetAsync(id, cancellationToken).ConfigureAwait(false);
        if (result.IsFailure)
        {
            TempData.AddToast(result.Errors.First().Message, ToastLevel.Error);
            return RedirectToAction(nameof(Index));
        }
        ViewData["Breadcrumbs"] = new[]
        {
            new Breadcrumb("Dashboard", Url.Action("Index", "Dashboard")),
            new Breadcrumb("Journal entries", Url.Action(nameof(Index))),
            new Breadcrumb(result.Value.EntryNumber, null, true)
        };
        return View(result.Value);
    }

    [HttpGet]
    public IActionResult Create()
    {
        ViewData["Breadcrumbs"] = new[]
        {
            new Breadcrumb("Dashboard", Url.Action("Index", "Dashboard")),
            new Breadcrumb("Journal entries", Url.Action(nameof(Index))),
            new Breadcrumb("New entry", null, true)
        };
        return View(new JournalEntryFormViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(JournalEntryFormViewModel model, bool postImmediately = false, CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid) return View(model);

        var request = model.ToPostRequest();
        var result = postImmediately
            ? await _entries.CreateAndPostAsync(request, cancellationToken).ConfigureAwait(false)
            : await _entries.CreateDraftAsync(request, cancellationToken).ConfigureAwait(false);

        if (result.IsFailure)
        {
            ModelState.AddResultErrors(result);
            return View(model);
        }

        TempData.AddToast(
            postImmediately
                ? $"Journal entry '{model.EntryNumber}' created and posted."
                : $"Journal entry '{model.EntryNumber}' saved as draft.",
            ToastLevel.Success);
        return RedirectToAction(nameof(Details), new { id = result.Value });
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id, CancellationToken cancellationToken)
    {
        var result = await _entries.GetAsync(id, cancellationToken).ConfigureAwait(false);
        if (result.IsFailure)
        {
            TempData.AddToast(result.Errors.First().Message, ToastLevel.Error);
            return RedirectToAction(nameof(Index));
        }
        ViewData["Breadcrumbs"] = new[]
        {
            new Breadcrumb("Dashboard", Url.Action("Index", "Dashboard")),
            new Breadcrumb("Journal entries", Url.Action(nameof(Index))),
            new Breadcrumb(result.Value.EntryNumber, Url.Action(nameof(Details), new { id })),
            new Breadcrumb("Edit", null, true)
        };
        return View(JournalEntryFormViewModel.FromDto(result.Value));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, JournalEntryFormViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return View(model);

        var result = await _entries.UpdateAsync(id, model.ToUpdateRequest(), cancellationToken).ConfigureAwait(false);
        if (result.IsFailure)
        {
            ModelState.AddResultErrors(result);
            return View(model);
        }
        TempData.AddToast("Journal entry updated.", ToastLevel.Success);
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpGet]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _entries.GetAsync(id, cancellationToken).ConfigureAwait(false);
        if (result.IsFailure)
        {
            TempData.AddToast(result.Errors.First().Message, ToastLevel.Error);
            return RedirectToAction(nameof(Index));
        }
        ViewData["Breadcrumbs"] = new[]
        {
            new Breadcrumb("Dashboard", Url.Action("Index", "Dashboard")),
            new Breadcrumb("Journal entries", Url.Action(nameof(Index))),
            new Breadcrumb(result.Value.EntryNumber, Url.Action(nameof(Details), new { id })),
            new Breadcrumb("Delete", null, true)
        };
        return View(result.Value);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid id, CancellationToken cancellationToken)
    {
        var result = await _entries.DeleteAsync(id, cancellationToken).ConfigureAwait(false);
        if (result.IsFailure)
        {
            TempData.AddToast(result.Errors.First().Message, ToastLevel.Error);
            return RedirectToAction(nameof(Index));
        }
        TempData.AddToast("Journal entry deleted.", ToastLevel.Success);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [Authorize(Policy = AuthorizationPolicies.JournalEntryPost)]
    public async Task<IActionResult> Post(Guid id, CancellationToken cancellationToken)
    {
        var result = await _entries.GetAsync(id, cancellationToken).ConfigureAwait(false);
        if (result.IsFailure)
        {
            TempData.AddToast(result.Errors.First().Message, ToastLevel.Error);
            return RedirectToAction(nameof(Index));
        }
        ViewData["Breadcrumbs"] = new[]
        {
            new Breadcrumb("Dashboard", Url.Action("Index", "Dashboard")),
            new Breadcrumb("Journal entries", Url.Action(nameof(Index))),
            new Breadcrumb(result.Value.EntryNumber, Url.Action(nameof(Details), new { id })),
            new Breadcrumb("Post", null, true)
        };
        return View(result.Value);
    }

    [HttpPost, ActionName("Post")]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = AuthorizationPolicies.JournalEntryPost)]
    public async Task<IActionResult> PostConfirmed(Guid id, CancellationToken cancellationToken)
    {
        var result = await _entries.PostExistingAsync(id, cancellationToken).ConfigureAwait(false);
        if (result.IsFailure)
        {
            TempData.AddToast(result.Errors.First().Message, ToastLevel.Error);
            return RedirectToAction(nameof(Details), new { id });
        }
        TempData.AddToast("Journal entry posted.", ToastLevel.Success);
        return RedirectToAction(nameof(Details), new { id });
    }
}
