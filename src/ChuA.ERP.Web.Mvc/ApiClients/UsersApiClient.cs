using ChuA.ERP.Web.Mvc.Contracts.Common;
using ChuA.ERP.Web.Mvc.Contracts.Dtos;
using ChuA.ERP.Web.Mvc.Services;
using Microsoft.Extensions.Logging;

namespace ChuA.ERP.Web.Mvc.ApiClients;

/// <inheritdoc cref="IUsersApiClient"/>
public sealed class UsersApiClient : ApiClientBase, IUsersApiClient
{
    public UsersApiClient(
        HttpClient httpClient,
        ITokenAcquisitionService tokenAcquisition,
        ICorrelationIdAccessor correlationIds,
        ILogger<UsersApiClient> logger)
        : base(httpClient, tokenAcquisition, correlationIds, logger)
    {
    }

    public Task<Result<CurrentUserDto>> GetMeAsync(CancellationToken cancellationToken = default) =>
        SendAsync<CurrentUserDto>(HttpMethod.Get, "v1/users/me", cancellationToken: cancellationToken);

    public Task<Result<IReadOnlyList<UserDto>>> ListAsync(string? search = null, CancellationToken cancellationToken = default) =>
        SendAsync<IReadOnlyList<UserDto>>(HttpMethod.Get, "v1/users" + QueryString(("search", search)), cancellationToken: cancellationToken);

    public Task<Result<UserDto>> GetAsync(Guid id, CancellationToken cancellationToken = default) =>
        SendAsync<UserDto>(HttpMethod.Get, $"v1/users/{id}", cancellationToken: cancellationToken);

    public Task<Result<UserDto>> CreateAsync(CreateUserRequest request, CancellationToken cancellationToken = default) =>
        SendAsync<UserDto>(HttpMethod.Post, "v1/users", request, cancellationToken);

    public Task<Result<UserDto>> UpdateAsync(Guid id, UpdateUserRequest request, CancellationToken cancellationToken = default) =>
        SendAsync<UserDto>(HttpMethod.Put, $"v1/users/{id}", request, cancellationToken);

    public Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default) =>
        SendAsync(HttpMethod.Delete, $"v1/users/{id}", cancellationToken: cancellationToken);
}
