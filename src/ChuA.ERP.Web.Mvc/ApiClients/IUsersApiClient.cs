using ChuA.ERP.Web.Mvc.Contracts.Common;
using ChuA.ERP.Web.Mvc.Contracts.Dtos;

namespace ChuA.ERP.Web.Mvc.ApiClients;

/// <summary>Calls /api/v1/users and /api/v1/users/me.</summary>
public interface IUsersApiClient
{
    Task<Result<CurrentUserDto>> GetMeAsync(CancellationToken cancellationToken = default);
    Task<Result<PagedResult<UserDto>>> ListAsync(string? search = null, int pageNumber = 1, int pageSize = 25, string? sort = null, CancellationToken cancellationToken = default);
    Task<Result<UserDto>> GetAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<UserDto>> CreateAsync(CreateUserRequest request, CancellationToken cancellationToken = default);
    Task<Result<UserDto>> UpdateAsync(Guid id, UpdateUserRequest request, CancellationToken cancellationToken = default);
    Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
