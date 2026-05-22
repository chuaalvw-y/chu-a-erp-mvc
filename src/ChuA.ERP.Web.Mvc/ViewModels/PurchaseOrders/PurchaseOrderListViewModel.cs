// Copyright (c) 2026 Alvin Wilsen Chan Chua
// GitHub: chuaalvw-y
// Licensed under the Alvin Wilsen Chan Chua Proprietary Use-Only License.
// See LICENSE.txt in the project root for full license information.

using ChuA.ERP.Web.Mvc.Contracts.Common;
using ChuA.ERP.Web.Mvc.Contracts.Dtos;

namespace ChuA.ERP.Web.Mvc.ViewModels.PurchaseOrders;

/// <summary>List page view model — wraps a paged result for the purchase orders table.</summary>
public sealed class PurchaseOrderListViewModel
{
    public PagedResult<PurchaseOrderDto> Page { get; set; } = PagedResult<PurchaseOrderDto>.Empty();
    public Guid? VendorId { get; set; }
    public string? Status { get; set; }
    public string? Search { get; set; }
    public IReadOnlyList<VendorDto> Vendors { get; set; } = Array.Empty<VendorDto>();
}
