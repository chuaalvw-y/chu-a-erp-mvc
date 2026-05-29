// Copyright (c) 2026 Alvin Wilsen Chan Chua
// GitHub: chuaalvw-y
// Licensed under the Alvin Wilsen Chan Chua Proprietary Use-Only License.
// See LICENSE.txt in the project root for full license information.

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ChuA.ERP.Web.Mvc.Extensions;

/// <summary>
/// Helpers that let a single Razor controller action serve either a full page (for normal
/// navigation / no-JS fallback) or a Razor partial (for the reactive fetch path). Keeps
/// rendering logic single-source so we never duplicate Index() and IndexPartial().
/// </summary>
public static class ReactiveRequestExtensions
{
    /// <summary>
    /// Header AJAX requests set: <c>X-Requested-With: XMLHttpRequest</c>. Our reactive JS
    /// always sets this so the controller can detect the fetch path.
    /// </summary>
    public const string RequestedWithHeader = "X-Requested-With";

    /// <summary>Value of <see cref="RequestedWithHeader"/> our reactive JS uses.</summary>
    public const string XmlHttpRequestValue = "XMLHttpRequest";

    /// <summary>
    /// Optional header reactive callers may set to request a specific partial layout
    /// (e.g. <c>X-Partial: rows</c> vs <c>X-Partial: form</c>). Lets one action return
    /// different fragments without proliferating endpoints.
    /// </summary>
    public const string PartialHeader = "X-Partial";

    /// <summary>
    /// True when the inbound request is an AJAX/fetch call from our reactive JS — checked
    /// via the conventional <c>X-Requested-With</c> header.
    /// </summary>
    public static bool IsAjaxRequest(this HttpRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        return string.Equals(
            request.Headers[RequestedWithHeader],
            XmlHttpRequestValue,
            StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Convenience overload off <see cref="Controller"/> for the common
    /// <c>Request.IsAjaxRequest()</c> check.
    /// </summary>
    public static bool IsAjaxRequest(this Controller controller)
    {
        ArgumentNullException.ThrowIfNull(controller);
        return controller.HttpContext.Request.IsAjaxRequest();
    }

    /// <summary>
    /// Returns the value of the <see cref="PartialHeader"/> header, or null when the
    /// caller did not request a specific partial layout. Result is lower-cased for
    /// case-insensitive comparison.
    /// </summary>
    public static string? RequestedPartial(this Controller controller)
    {
        ArgumentNullException.ThrowIfNull(controller);
        var value = controller.HttpContext.Request.Headers[PartialHeader].ToString();
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim().ToLowerInvariant();
    }

    /// <summary>
    /// Returns a <see cref="PartialViewResult"/> when the request is an AJAX call from
    /// reactive JS, else a full <see cref="ViewResult"/>. The two render the same model
    /// so the partial can be the table-body fragment of the full view's layout.
    /// </summary>
    public static IActionResult PartialOrView(this Controller controller, string viewName, object? model = null)
    {
        ArgumentNullException.ThrowIfNull(controller);
        if (string.IsNullOrWhiteSpace(viewName))
        {
            throw new ArgumentException("A view name is required.", nameof(viewName));
        }
        return controller.IsAjaxRequest()
            ? controller.PartialView(viewName, model)
            : controller.View(viewName, model);
    }
}
