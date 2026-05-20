// Copyright (c) 2026 Alvin Wilsen Chan Chua
// GitHub: chuaalvw-y
// Licensed under the Alvin Wilsen Chan Chua Proprietary Use-Only License.
// See LICENSE.txt in the project root for full license information.

namespace ChuA.ERP.Web.Mvc.Configuration;

/// <summary>
/// Display-time environment context shown in the top navigation bar.
/// Bound to the "AppEnvironment" configuration section.
/// </summary>
public sealed class AppEnvironmentOptions
{
    public const string SectionName = "AppEnvironment";

    /// <summary>Short label shown in the top nav, e.g. "DEV", "UAT", "PROD".</summary>
    public string Label { get; set; } = "DEV";

    /// <summary>Bootstrap color name used for the badge background.</summary>
    public string BadgeColor { get; set; } = "warning";

    /// <summary>Optional product name shown in the header.</summary>
    public string ProductName { get; set; } = "ChuA ERP";
}
