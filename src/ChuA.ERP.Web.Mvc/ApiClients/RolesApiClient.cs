using ChuA.ERP.Web.Mvc.Contracts.Common;
using ChuA.ERP.Web.Mvc.Contracts.Dtos;
using ChuA.ERP.Web.Mvc.Services;
using Microsoft.Extensions.Logging;

namespace ChuA.ERP.Web.Mvc.ApiClients;

/// <inheritdoc cref="IRolesApiClient"/>
public sealed class RolesApiClient : ApiClientBase, IRolesApiClient
{
    public RolesApiClient(
        HttpClient httpClient,
        ITokenAcquisitionService tokenAcquisition,
        ICorrelationIdAccessor correlationIds,
        ILogger<RolesApiClient> logger)
        : base(httpClient, tokenAcquisition, correlationIds, logger)
    {
    }

    public Task<Result<IReadOnlyList<RoleDto>>> ListAsync(string? search = null, CancellationToken cancellationToken = default) =>
        SendAsync<IReadOnlyList<RoleDto>>(HttpMethod.Get, "v1/roles" + QueryString(("search", search)), cancellationToken: cancellationToken);

    public Task<Result<RoleDto>> GetAsync(Guid id, CancellationToken cancellationToken = default) =>
        SendAsync<RoleDto>(HttpMethod.Get, $"v1/roles/{id}", cancellationToken: cancellationToken);

    public Task<Result<RoleDto>> CreateAsync(CreateRoleRequest request, CancellationToken cancellationToken = default) =>
        SendAsync<RoleDto>(HttpMethod.Post, "v1/roles", request, cancellationToken);

    public Task<Result<RoleDto>> UpdateAsync(Guid id, UpdateRoleRequest request, CancellationToken cancellationToken = default) =>
        SendAsync<RoleDto>(HttpMethod.Put, $"v1/roles/{id}", request, cancellationToken);

    public Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default) =>
        SendAsync(HttpMethod.Delete, $"v1/roles/{id}", cancellationToken: cancellationToken);
}
