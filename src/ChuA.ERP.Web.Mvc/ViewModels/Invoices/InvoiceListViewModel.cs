using ChuA.ERP.Web.Mvc.Contracts.Common;
using ChuA.ERP.Web.Mvc.Contracts.Dtos;

namespace ChuA.ERP.Web.Mvc.ViewModels.Invoices;

/// <summary>List page view model for Invoices. Wraps a paged result and exposes filter fields.</summary>
public sealed class InvoiceListViewModel
{
    public PagedResult<InvoiceDto> Page { get; set; } = PagedResult<InvoiceDto>.Empty();
    public Guid? CustomerId { get; set; }
    public string? Status { get; set; }
    public string? PaymentStatus { get; set; }
    public string? Search { get; set; }
    public IReadOnlyList<CustomerDto> Customers { get; set; } = Array.Empty<CustomerDto>();
}
