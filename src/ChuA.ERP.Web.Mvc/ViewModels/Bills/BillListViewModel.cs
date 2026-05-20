// Copyright (c) 2026 Alvin Wilsen Chan Chua
// GitHub: chuaalvw-y
// Licensed under the Alvin Wilsen Chan Chua Proprietary Use-Only License.
// See LICENSE.txt in the project root for full license information.

using ChuA.ERP.Web.Mvc.Contracts.Common;
using ChuA.ERP.Web.Mvc.Contracts.Dtos;

namespace ChuA.ERP.Web.Mvc.ViewModels.Bills;

/// <summary>List page view model for Bills. Wraps a paged result and exposes filter fields.</summary>
public sealed class BillListViewModel
{
    public PagedResult<BillDto> Page { get; set; } = PagedResult<BillDto>.Empty();
    public Guid? VendorId { get; set; }
    public string? Status { get; set; }
    public string? PaymentStatus { get; set; }
    public string? Search { get; set; }
    public IReadOnlyList<VendorDto> Vendors { get; set; } = Array.Empty<VendorDto>();
}
