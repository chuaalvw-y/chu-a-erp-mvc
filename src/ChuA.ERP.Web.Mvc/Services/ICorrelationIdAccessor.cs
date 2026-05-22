// Copyright (c) 2026 Alvin Wilsen Chan Chua
// GitHub: chuaalvw-y
// Licensed under the Alvin Wilsen Chan Chua Proprietary Use-Only License.
// See LICENSE.txt in the project root for full license information.

namespace ChuA.ERP.Web.Mvc.Services;

/// <summary>
/// Provides the correlation id for the current HTTP request so API clients can attach it
/// to outbound calls.
/// </summary>
public interface ICorrelationIdAccessor
{
    /// <summary>Returns the active correlation id (never null; generated lazily on first use).</summary>
    string GetOrCreate();
}
