using ChuA.ERP.Web.Mvc.Contracts.Common;
using ChuA.ERP.Web.Mvc.Contracts.Dtos;

namespace ChuA.ERP.Web.Mvc.ApiClients;

/// <summary>Calls /api/v1/vendors.</summary>
public interface IVendorsApiClient
{
    Task<Result<IReadOnlyList<VendorDto>>> ListAsync(string? search = null, CancellationToken cancellationToken = default);
    Task<Result<VendorDto>> GetAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<VendorDto>> CreateAsync(CreateVendorRequest request, CancellationToken cancellationToken = default);
    Task<Result<VendorDto>> UpdateAsync(Guid id, UpdateVendorRequest request, CancellationToken cancellationToken = default);
    Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
