// Copyright (c) 2026 Alvin Wilsen Chan Chua
// GitHub: chuaalvw-y
// Licensed under the Alvin Wilsen Chan Chua Proprietary Use-Only License.
// See LICENSE.txt in the project root for full license information.

using ChuA.ERP.Web.Mvc.Contracts.Common;
using ChuA.ERP.Web.Mvc.Contracts.Dtos;

namespace ChuA.ERP.Web.Mvc.ViewModels.Vendors;

/// <summary>List page view model — wraps a paged result for the data table.</summary>
public sealed class VendorListViewModel
{
    public PagedResult<VendorDto> Page { get; set; } = PagedResult<VendorDto>.Empty();
    public string? Search { get; set; }
}
