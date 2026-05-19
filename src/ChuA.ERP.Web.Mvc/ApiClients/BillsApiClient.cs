using ChuA.ERP.Web.Mvc.Contracts.Common;
using ChuA.ERP.Web.Mvc.Contracts.Dtos;
using ChuA.ERP.Web.Mvc.Services;
using Microsoft.Extensions.Logging;

namespace ChuA.ERP.Web.Mvc.ApiClients;

/// <inheritdoc cref="IBillsApiClient"/>
public sealed class BillsApiClient : ApiClientBase, IBillsApiClient
{
    public BillsApiClient(
        HttpClient httpClient,
        ITokenAcquisitionService tokenAcquisition,
        ICorrelationIdAccessor correlationIds,
        ILogger<BillsApiClient> logger)
        : base(httpClient, tokenAcquisition, correlationIds, logger)
    {
    }

    public Task<Result<PagedResult<BillDto>>> ListAsync(Guid? vendorId = null, string? status = null, string? paymentStatus = null, string? search = null, int pageNumber = 1, int pageSize = 25, string? sort = null, CancellationToken cancellationToken = default) =>
        SendPagedAsync<BillDto>(
            "v1/bills" + QueryString(("vendorId", vendorId), ("status", status), ("paymentStatus", paymentStatus), ("search", search), ("pageNumber", pageNumber), ("pageSize", pageSize), ("sort", sort)),
            pageNumber,
            pageSize,
            cancellationToken);

    public Task<Result<PagedResult<BillDto>>> GetAwaitingApprovalAsync(int pageNumber = 1, int pageSize = 25, string? sort = null, CancellationToken cancellationToken = default) =>
        SendPagedAsync<BillDto>("v1/bills/awaiting-approval" + QueryString(("pageNumber", pageNumber), ("pageSize", pageSize), ("sort", sort)), pageNumber, pageSize, cancellationToken);

    public Task<Result<BillDto>> GetAsync(Guid id, CancellationToken cancellationToken = default) =>
        SendAsync<BillDto>(HttpMethod.Get, $"v1/bills/{id}", cancellationToken: cancellationToken);

    public Task<Result<BillDto>> CreateAsync(CreateBillRequest request, CancellationToken cancellationToken = default) =>
        SendAsync<BillDto>(HttpMethod.Post, "v1/bills", request, cancellationToken);

    public Task<Result<BillDto>> UpdateAsync(Guid id, UpdateBillRequest request, CancellationToken cancellationToken = default) =>
        SendAsync<BillDto>(HttpMethod.Put, $"v1/bills/{id}", request, cancellationToken);

    public Task<Result<BillDto>> ApproveAsync(Guid id, CancellationToken cancellationToken = default) =>
        SendAsync<BillDto>(HttpMethod.Post, $"v1/bills/{id}/approve", cancellationToken: cancellationToken);

    public Task<Result<BillDto>> PayAsync(Guid id, PayBillRequest request, CancellationToken cancellationToken = default) =>
        SendAsync<BillDto>(HttpMethod.Post, $"v1/bills/{id}/payments", request, cancellationToken);

    public Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default) =>
        SendAsync(HttpMethod.Delete, $"v1/bills/{id}", cancellationToken: cancellationToken);
}
