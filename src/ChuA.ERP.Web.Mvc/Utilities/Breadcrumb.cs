// Copyright (c) 2026 Alvin Wilsen Chan Chua
// GitHub: chuaalvw-y
// Licensed under the Alvin Wilsen Chan Chua Proprietary Use-Only License.
// See LICENSE.txt in the project root for full license information.

namespace ChuA.ERP.Web.Mvc.Utilities;

/// <summary>A single segment in a breadcrumb trail.</summary>
public sealed record Breadcrumb(string Text, string? Href = null, bool IsActive = false);
