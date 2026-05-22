// Copyright (c) 2026 Alvin Wilsen Chan Chua
// GitHub: chuaalvw-y
// Licensed under the Alvin Wilsen Chan Chua Proprietary Use-Only License.
// See LICENSE.txt in the project root for full license information.

namespace ChuA.ERP.Web.Mvc.Utilities;

/// <summary>Toast severity levels, mapped to Bootstrap text-bg-* classes by the partial.</summary>
public enum ToastLevel { Info, Success, Warning, Error }

/// <summary>Single toast notification. Controllers push via <c>TempData.AddToast(...)</c>.</summary>
public sealed record ToastMessage(string Text, ToastLevel Level = ToastLevel.Info, string? Title = null);
