// Copyright (c) 2026 Alvin Wilsen Chan Chua
// GitHub: chuaalvw-y
// Licensed under the Alvin Wilsen Chan Chua Proprietary Use-Only License.
// See LICENSE.txt in the project root for full license information.

namespace ChuA.ERP.Web.Mvc.Models;

/// <summary>View model for /Home/Error and the global exception filter.</summary>
public sealed class ErrorViewModel
{
    public string? CorrelationId { get; set; }
    public string? Message { get; set; }
    public int? StatusCode { get; set; }
    public string? ErrorCode { get; set; }
    public IReadOnlyCollection<string>? Details { get; set; }

    public bool ShowCorrelationId => !string.IsNullOrWhiteSpace(CorrelationId);
}
