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

    public Task<Result<IReadOnlyList<BillDto>>> ListAsync(Guid? vendorId = null, string? status = null, string? paymentStatus = null, string? search = null, CancellationToken cancellationToken = default) =>
        SendAsync<IReadOnlyList<BillDto>>(
            HttpMethod.Get,
            "v1/bills" + QueryString(("vendorId", vendorId), ("status", status), ("paymentStatus", paymentStatus), ("search", search)),
            cancellationToken: cancellationToken);

    public Task<Result<IReadOnlyList<BillDto>>> GetAwaitingApprovalAsync(CancellationToken cancellationToken = default) =>
        SendAsync<IReadOnlyList<BillDto>>(HttpMethod.Get, "v1/bills/awaiting-approval", cancellationToken: cancellationToken);

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
