// Copyright (c) 2026 Alvin Wilsen Chan Chua
// GitHub: chuaalvw-y
// Licensed under the Alvin Wilsen Chan Chua Proprietary Use-Only License.
// See LICENSE.txt in the project root for full license information.

namespace ChuA.ERP.Web.Mvc.ViewModels;

/// <summary>API health summary shown on the System Health page.</summary>
public sealed class HealthIndexViewModel
{
    public bool ApiReachable { get; set; }
    public string? Service { get; set; }
    public string Status { get; set; } = "Unknown";
    public DateTimeOffset? Timestamp { get; set; }
    public string? ErrorMessage { get; set; }
}
