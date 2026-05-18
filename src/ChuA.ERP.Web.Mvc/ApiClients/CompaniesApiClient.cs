using ChuA.ERP.Web.Mvc.Contracts.Common;
using ChuA.ERP.Web.Mvc.Contracts.Dtos;
using ChuA.ERP.Web.Mvc.Services;
using Microsoft.Extensions.Logging;

namespace ChuA.ERP.Web.Mvc.ApiClients;

/// <inheritdoc cref="ICompaniesApiClient"/>
public sealed class CompaniesApiClient : ApiClientBase, ICompaniesApiClient
{
    public CompaniesApiClient(
        HttpClient httpClient,
        ITokenAcquisitionService tokenAcquisition,
        ICorrelationIdAccessor correlationIds,
        ILogger<CompaniesApiClient> logger)
        : base(httpClient, tokenAcquisition, correlationIds, logger)
    {
    }

    public Task<Result<IReadOnlyList<CompanyDto>>> ListAsync(string? search = null, CancellationToken cancellationToken = default) =>
        SendAsync<IReadOnlyList<CompanyDto>>(HttpMethod.Get, "v1/companies" + QueryString(("search", search)), cancellationToken: cancellationToken);

    public Task<Result<CompanyDto>> GetAsync(Guid id, CancellationToken cancellationToken = default) =>
        SendAsync<CompanyDto>(HttpMethod.Get, $"v1/companies/{id}", cancellationToken: cancellationToken);

    public Task<Result<CompanyDto>> CreateAsync(CreateCompanyRequest request, CancellationToken cancellationToken = default) =>
        SendAsync<CompanyDto>(HttpMethod.Post, "v1/companies", request, cancellationToken);

    public Task<Result<CompanyDto>> UpdateAsync(Guid id, UpdateCompanyRequest request, CancellationToken cancellationToken = default) =>
        SendAsync<CompanyDto>(HttpMethod.Put, $"v1/companies/{id}", request, cancellationToken);

    public Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default) =>
        SendAsync(HttpMethod.Delete, $"v1/companies/{id}", cancellationToken: cancellationToken);
}
