using ChuA.ERP.Web.Mvc.Contracts.Common;
using ChuA.ERP.Web.Mvc.Contracts.Dtos;
using ChuA.ERP.Web.Mvc.Services;
using Microsoft.Extensions.Logging;

namespace ChuA.ERP.Web.Mvc.ApiClients;

/// <inheritdoc cref="IVendorsApiClient"/>
public sealed class VendorsApiClient : ApiClientBase, IVendorsApiClient
{
    public VendorsApiClient(
        HttpClient httpClient,
        ITokenAcquisitionService tokenAcquisition,
        ICorrelationIdAccessor correlationIds,
        ILogger<VendorsApiClient> logger)
        : base(httpClient, tokenAcquisition, correlationIds, logger)
    {
    }

    public Task<Result<IReadOnlyList<VendorDto>>> ListAsync(string? search = null, CancellationToken cancellationToken = default) =>
        SendAsync<IReadOnlyList<VendorDto>>(HttpMethod.Get, "v1/vendors" + QueryString(("search", search)), cancellationToken: cancellationToken);

    public Task<Result<VendorDto>> GetAsync(Guid id, CancellationToken cancellationToken = default) =>
        SendAsync<VendorDto>(HttpMethod.Get, $"v1/vendors/{id}", cancellationToken: cancellationToken);

    public Task<Result<VendorDto>> CreateAsync(CreateVendorRequest request, CancellationToken cancellationToken = default) =>
        SendAsync<VendorDto>(HttpMethod.Post, "v1/vendors", request, cancellationToken);

    public Task<Result<VendorDto>> UpdateAsync(Guid id, UpdateVendorRequest request, CancellationToken cancellationToken = default) =>
        SendAsync<VendorDto>(HttpMethod.Put, $"v1/vendors/{id}", request, cancellationToken);

    public Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default) =>
        SendAsync(HttpMethod.Delete, $"v1/vendors/{id}", cancellationToken: cancellationToken);
}
