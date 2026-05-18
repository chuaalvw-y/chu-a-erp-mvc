namespace ChuA.ERP.Web.Mvc.Utilities;

/// <summary>Toast severity levels, mapped to Bootstrap text-bg-* classes by the partial.</summary>
public enum ToastLevel { Info, Success, Warning, Error }

/// <summary>Single toast notification. Controllers push via <c>TempData.AddToast(...)</c>.</summary>
public sealed record ToastMessage(string Text, ToastLevel Level = ToastLevel.Info, string? Title = null);
