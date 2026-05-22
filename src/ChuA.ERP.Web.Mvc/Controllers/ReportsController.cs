// Copyright (c) 2026 Alvin Wilsen Chan Chua
// GitHub: chuaalvw-y
// Licensed under the Alvin Wilsen Chan Chua Proprietary Use-Only License.
// See LICENSE.txt in the project root for full license information.

using System.Text.Json;
using ChuA.ERP.Web.Mvc.ApiClients;
using ChuA.ERP.Web.Mvc.Extensions;
using ChuA.ERP.Web.Mvc.Security;
using ChuA.ERP.Web.Mvc.Utilities;
using ChuA.ERP.Web.Mvc.ViewModels.Reports;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChuA.ERP.Web.Mvc.Controllers;

/// <summary>UI for listing and running reports.</summary>
[Authorize(Policy = AuthorizationPolicies.ReportRun)]
public sealed class ReportsController : Controller
{
    private readonly IReportsApiClient _reports;

    public ReportsController(IReportsApiClient reports)
    {
        _reports = reports;
    }

    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var result = await _reports.ListAsync(cancellationToken).ConfigureAwait(false);
        ViewData["Breadcrumbs"] = new[]
        {
            new Breadcrumb("Dashboard", Url.Action("Index", "Dashboard")),
            new Breadcrumb("Reports", null, true)
        };
        if (result.IsFailure)
        {
            TempData.AddToast(result.Errors.First().Message, ToastLevel.Error);
            return View(new ReportListViewModel());
        }
        return View(new ReportListViewModel { Reports = result.Value });
    }

    [HttpGet]
    public async Task<IActionResult> Run(string code, CancellationToken cancellationToken)
    {
        var listResult = await _reports.ListAsync(cancellationToken).ConfigureAwait(false);
        if (listResult.IsFailure)
        {
            TempData.AddToast(listResult.Errors.First().Message, ToastLevel.Error);
            return RedirectToAction(nameof(Index));
        }

        var meta = listResult.Value.FirstOrDefault(r => string.Equals(r.Code, code, StringComparison.OrdinalIgnoreCase));
        if (meta is null)
        {
            TempData.AddToast($"Report '{code}' not found.", ToastLevel.Error);
            return RedirectToAction(nameof(Index));
        }

        ViewData["Breadcrumbs"] = new[]
        {
            new Breadcrumb("Dashboard", Url.Action("Index", "Dashboard")),
            new Breadcrumb("Reports", Url.Action(nameof(Index))),
            new Breadcrumb(meta.Name, null, true)
        };

        return View(new ReportRunViewModel
        {
            Code = meta.Code,
            Name = meta.Name,
            Description = meta.Description,
            ParametersJson = "{}",
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Run(string code, ReportRunViewModel model, CancellationToken cancellationToken)
    {
        ViewData["Breadcrumbs"] = new[]
        {
            new Breadcrumb("Dashboard", Url.Action("Index", "Dashboard")),
            new Breadcrumb("Reports", Url.Action(nameof(Index))),
            new Breadcrumb(model.Name ?? code, null, true)
        };

        if (!ModelState.IsValid) return View(model);

        object? parameters;
        try
        {
            var doc = JsonDocument.Parse(model.ParametersJson);
            parameters = doc.RootElement;
        }
        catch (JsonException)
        {
            ModelState.AddModelError(nameof(model.ParametersJson), "Parameters must be valid JSON");
            return View(model);
        }

        var result = await _reports.RunAsync(code, parameters, cancellationToken).ConfigureAwait(false);
        if (result.IsFailure)
        {
            ModelState.AddResultErrors(result);
            return View(model);
        }

        model.Results = result.Value;
        return View(model);
    }
}
