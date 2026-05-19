using ChuA.ERP.Web.Mvc.Contracts.Common;
using ChuA.ERP.Web.Mvc.Contracts.Dtos;

namespace ChuA.ERP.Web.Mvc.ApiClients;

/// <summary>Calls /api/v1/roles.</summary>
public interface IRolesApiClient
{
    /// <summary>Lists roles, optionally filtered by free-text search.</summary>
    Task<Result<PagedResult<RoleDto>>> ListAsync(string? search = null, int pageNumber = 1, int pageSize = 25, string? sort = null, CancellationToken cancellationToken = default);

    /// <summary>Gets a single role by id.</summary>
    Task<Result<RoleDto>> GetAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Creates a new role.</summary>
    Task<Result<RoleDto>> CreateAsync(CreateRoleRequest request, CancellationToken cancellationToken = default);

    /// <summary>Updates an existing role.</summary>
    Task<Result<RoleDto>> UpdateAsync(Guid id, UpdateRoleRequest request, CancellationToken cancellationToken = default);

    /// <summary>Deletes a role by id.</summary>
    Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
