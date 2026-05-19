using ChuA.ERP.Web.Mvc.Contracts.Common;
using ChuA.ERP.Web.Mvc.Contracts.Dtos;

namespace ChuA.ERP.Web.Mvc.ApiClients;

/// <summary>Calls /api/v1/invoices.</summary>
public interface IInvoicesApiClient
{
    /// <summary>Lists invoices, optionally filtered by customer, status, payment status and search text.</summary>
    Task<Result<PagedResult<InvoiceDto>>> ListAsync(Guid? customerId = null, string? status = null, string? paymentStatus = null, string? search = null, int pageNumber = 1, int pageSize = 25, string? sort = null, CancellationToken cancellationToken = default);

    /// <summary>Gets a single invoice by id.</summary>
    Task<Result<InvoiceDto>> GetAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Creates a new invoice.</summary>
    Task<Result<InvoiceDto>> CreateAsync(CreateInvoiceRequest request, CancellationToken cancellationToken = default);

    /// <summary>Updates an existing invoice.</summary>
    Task<Result<InvoiceDto>> UpdateAsync(Guid id, UpdateInvoiceRequest request, CancellationToken cancellationToken = default);

    /// <summary>Applies a customer payment to an invoice.</summary>
    Task<Result<InvoiceDto>> ApplyPaymentAsync(Guid id, ApplyCustomerPaymentRequest request, CancellationToken cancellationToken = default);

    /// <summary>Deletes an invoice by id.</summary>
    Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
