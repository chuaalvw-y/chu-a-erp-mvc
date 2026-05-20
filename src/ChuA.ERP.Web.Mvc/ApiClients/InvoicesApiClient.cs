// Copyright (c) 2026 Alvin Wilsen Chan Chua
// GitHub: chuaalvw-y
// Licensed under the Alvin Wilsen Chan Chua Proprietary Use-Only License.
// See LICENSE.txt in the project root for full license information.

using ChuA.ERP.Web.Mvc.Contracts.Common;
using ChuA.ERP.Web.Mvc.Contracts.Dtos;
using ChuA.ERP.Web.Mvc.Services;
using Microsoft.Extensions.Logging;

namespace ChuA.ERP.Web.Mvc.ApiClients;

/// <inheritdoc cref="IInvoicesApiClient"/>
public sealed class InvoicesApiClient : ApiClientBase, IInvoicesApiClient
{
    public InvoicesApiClient(
        HttpClient httpClient,
        ITokenAcquisitionService tokenAcquisition,
        ICorrelationIdAccessor correlationIds,
        ILogger<InvoicesApiClient> logger)
        : base(httpClient, tokenAcquisition, correlationIds, logger)
    {
    }

    public Task<Result<PagedResult<InvoiceDto>>> ListAsync(Guid? customerId = null, string? status = null, string? paymentStatus = null, string? search = null, int pageNumber = 1, int pageSize = 25, string? sort = null, CancellationToken cancellationToken = default) =>
        SendPagedAsync<InvoiceDto>(
            "v1/invoices" + QueryString(("customerId", customerId), ("status", status), ("paymentStatus", paymentStatus), ("search", search), ("pageNumber", pageNumber), ("pageSize", pageSize), ("sort", sort)),
            pageNumber,
            pageSize,
            cancellationToken);

    public Task<Result<InvoiceDto>> GetAsync(Guid id, CancellationToken cancellationToken = default) =>
        SendAsync<InvoiceDto>(HttpMethod.Get, $"v1/invoices/{id}", cancellationToken: cancellationToken);

    public Task<Result<InvoiceDto>> CreateAsync(CreateInvoiceRequest request, CancellationToken cancellationToken = default) =>
        SendAsync<InvoiceDto>(HttpMethod.Post, "v1/invoices", request, cancellationToken);

    public Task<Result<InvoiceDto>> UpdateAsync(Guid id, UpdateInvoiceRequest request, CancellationToken cancellationToken = default) =>
        SendAsync<InvoiceDto>(HttpMethod.Put, $"v1/invoices/{id}", request, cancellationToken);

    public Task<Result<InvoiceDto>> ApplyPaymentAsync(Guid id, ApplyCustomerPaymentRequest request, CancellationToken cancellationToken = default) =>
        SendAsync<InvoiceDto>(HttpMethod.Post, $"v1/invoices/{id}/apply-payment", request, cancellationToken);

    public Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default) =>
        SendAsync(HttpMethod.Delete, $"v1/invoices/{id}", cancellationToken: cancellationToken);
}
