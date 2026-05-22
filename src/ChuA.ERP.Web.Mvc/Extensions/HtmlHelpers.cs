// Copyright (c) 2026 Alvin Wilsen Chan Chua
// GitHub: chuaalvw-y
// Licensed under the Alvin Wilsen Chan Chua Proprietary Use-Only License.
// See LICENSE.txt in the project root for full license information.

using ChuA.ERP.Web.Mvc.Contracts.Dtos;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ChuA.ERP.Web.Mvc.Extensions;

/// <summary>Lightweight HTML helpers used across ERP forms and tables.</summary>
public static class HtmlHelpers
{
    /// <summary>Formats a money DTO as "1,234.56 USD".</summary>
    public static IHtmlContent Money(this IHtmlHelper html, MoneyDto? money)
    {
        if (money is null) return HtmlString.Empty;
        return new HtmlString($"{money.Amount:N2} {html.Encode(money.CurrencyCode)}");
    }

    /// <summary>Formats a quantity DTO as "10.00 EA".</summary>
    public static IHtmlContent Quantity(this IHtmlHelper html, QuantityDto? quantity)
    {
        if (quantity is null) return HtmlString.Empty;
        return new HtmlString($"{quantity.Value:N2} {html.Encode(quantity.UnitOfMeasure)}");
    }

    /// <summary>Renders a Bootstrap status badge for a free-form status string.</summary>
    public static IHtmlContent StatusBadge(this IHtmlHelper html, string? status)
    {
        var s = status ?? "Unknown";
        var color = s.ToLowerInvariant() switch
        {
            "draft" or "open" => "secondary",
            "submitted" or "pending" or "pendingapproval" => "info",
            "approved" or "posted" or "shipped" or "received" => "success",
            "paid" or "closed" or "completed" => "primary",
            "rejected" or "cancelled" or "void" or "blocked" => "danger",
            "partiallypaid" or "partial" or "partiallyreceived" or "partiallyshipped" => "warning",
            _ => "light text-dark",
        };
        return new HtmlString($"<span class=\"badge text-bg-{color}\">{html.Encode(s)}</span>");
    }

    /// <summary>Empty-state message panel for tables with no rows.</summary>
    public static IHtmlContent EmptyState(this IHtmlHelper html, string message, string? actionHtml = null)
    {
        var inner = $"<p class=\"text-muted mb-2\">{html.Encode(message)}</p>{actionHtml ?? string.Empty}";
        return new HtmlString($"<div class=\"empty-state text-center py-5\">{inner}</div>");
    }
}
