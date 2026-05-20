// Copyright (c) 2026 Alvin Wilsen Chan Chua
// GitHub: chuaalvw-y
// Licensed under the Alvin Wilsen Chan Chua Proprietary Use-Only License.
// See LICENSE.txt in the project root for full license information.

using System.Text.Json;
using ChuA.ERP.Web.Mvc.Contracts.Common;
using ChuA.ERP.Web.Mvc.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace ChuA.ERP.Web.Mvc.Extensions;

/// <summary>Convenience methods that let controllers push toasts and surface API failures cleanly.</summary>
public static class ControllerExtensions
{
    private const string ToastsKey = "ChuA.ERP.Toasts";

    /// <summary>Adds a toast message that will render after the next page load.</summary>
    public static void AddToast(this ITempDataDictionary tempData, string text, ToastLevel level = ToastLevel.Info, string? title = null)
    {
        var list = tempData.TryGetValue(ToastsKey, out var raw) && raw is string serialized
            ? JsonSerializer.Deserialize<List<ToastMessage>>(serialized) ?? new List<ToastMessage>()
            : new List<ToastMessage>();
        list.Add(new ToastMessage(text, level, title));
        tempData[ToastsKey] = JsonSerializer.Serialize(list);
    }

    /// <summary>Pulls and clears any toasts queued for the current render.</summary>
    public static IReadOnlyList<ToastMessage> ConsumeToasts(this ITempDataDictionary tempData)
    {
        if (!tempData.TryGetValue(ToastsKey, out var raw) || raw is not string serialized)
        {
            return Array.Empty<ToastMessage>();
        }
        var list = JsonSerializer.Deserialize<List<ToastMessage>>(serialized) ?? new List<ToastMessage>();
        tempData.Remove(ToastsKey);
        return list;
    }

    /// <summary>Copies errors from a failed <see cref="Result"/> into ModelState.</summary>
    public static void AddResultErrors(this ModelStateDictionary modelState, Result result)
    {
        if (result.IsSuccess) return;
        foreach (var error in result.Errors)
        {
            modelState.AddModelError(error.Target ?? string.Empty, error.Message);
        }
    }
}
