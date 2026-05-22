// Copyright (c) 2026 Alvin Wilsen Chan Chua
// GitHub: chuaalvw-y
// Licensed under the Alvin Wilsen Chan Chua Proprietary Use-Only License.
// See LICENSE.txt in the project root for full license information.

namespace ChuA.ERP.Web.Mvc.Configuration;

/// <summary>
/// Options that describe how the MVC layer reaches ChuA.ERP.Api.
/// Bound to the "Api" configuration section.
/// </summary>
public sealed class ApiOptions
{
    public const string SectionName = "Api";

    /// <summary>Base URL of the API, e.g. https://localhost:5001 (no trailing /api/v1).</summary>
    public string BaseUrl { get; set; } = "https://localhost:5001";

    /// <summary>Version segment of the API path; defaults to "v1".</summary>
    public string Version { get; set; } = "v1";

    /// <summary>Per-request timeout in seconds.</summary>
    public int TimeoutSeconds { get; set; } = 30;

    /// <summary>Number of automatic retries for transient failures.</summary>
    public int RetryCount { get; set; } = 3;

    /// <summary>If true the MVC will use the API's AuthBypass scheme (no bearer token).</summary>
    public bool UseAuthBypass { get; set; } = false;

    /// <summary>Optional fixed bearer token to attach (development convenience).</summary>
    public string? DevelopmentBearerToken { get; set; }
}
