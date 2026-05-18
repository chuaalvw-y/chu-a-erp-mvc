using ChuA.ERP.Web.Mvc.Contracts.Common;
using ChuA.ERP.Web.Mvc.Contracts.Dtos;
using ChuA.ERP.Web.Mvc.Services;
using Microsoft.Extensions.Logging;

namespace ChuA.ERP.Web.Mvc.ApiClients;

/// <inheritdoc cref="ICustomersApiClient"/>
public sealed class CustomersApiClient : ApiClientBase, ICustomersApiClient
{
    public CustomersApiClient(
        HttpClient httpClient,
        ITokenAcquisitionService tokenAcquisition,
        ICorrelationIdAccessor correlationIds,
        ILogger<CustomersApiClient> logger)
        : base(httpClient, tokenAcquisition, correlationIds, logger)
    {
    }

    public Task<Result<IReadOnlyList<CustomerDto>>> ListAsync(string? search = null, CancellationToken cancellationToken = default) =>
        SendAsync<IReadOnlyList<CustomerDto>>(HttpMethod.Get, "v1/customers" + QueryString(("search", search)), cancellationToken: cancellationToken);

    public Task<Result<CustomerDto>> GetAsync(Guid id, CancellationToken cancellationToken = default) =>
        SendAsync<CustomerDto>(HttpMethod.Get, $"v1/customers/{id}", cancellationToken: cancellationToken);

    public Task<Result<CustomerDto>> CreateAsync(CreateCustomerRequest request, CancellationToken cancellationToken = default) =>
        SendAsync<CustomerDto>(HttpMethod.Post, "v1/customers", request, cancellationToken);

    public Task<Result<CustomerDto>> UpdateAsync(Guid id, UpdateCustomerRequest request, CancellationToken cancellationToken = default) =>
        SendAsync<CustomerDto>(HttpMethod.Put, $"v1/customers/{id}", request, cancellationToken);

    public Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default) =>
        SendAsync(HttpMethod.Delete, $"v1/customers/{id}", cancellationToken: cancellationToken);
}
