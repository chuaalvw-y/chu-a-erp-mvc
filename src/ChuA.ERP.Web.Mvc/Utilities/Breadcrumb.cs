namespace ChuA.ERP.Web.Mvc.Utilities;

/// <summary>A single segment in a breadcrumb trail.</summary>
public sealed record Breadcrumb(string Text, string? Href = null, bool IsActive = false);
