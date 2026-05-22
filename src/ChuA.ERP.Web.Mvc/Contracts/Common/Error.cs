// Copyright (c) 2026 Alvin Wilsen Chan Chua
// GitHub: chuaalvw-y
// Licensed under the Alvin Wilsen Chan Chua Proprietary Use-Only License.
// See LICENSE.txt in the project root for full license information.

namespace ChuA.ERP.Web.Mvc.Contracts.Common;

/// <summary>
/// UI-side mirror of <c>ChuA.SharedKernel.Models.Error</c>. Represents a single failure
/// reason returned by the API (code + message + optional target field).
/// </summary>
public sealed record Error(string Code, string Message, string? Target = null)
{
    /// <summary>The neutral "no error" sentinel.</summary>
    public static readonly Error None = new("None", string.Empty);

    /// <summary>Convenience builder for unhandled exceptions.</summary>
    public static Error FromException(Exception ex, string code = "Unhandled") =>
        new(code, ex.Message);
}
