using ChuA.ERP.Web.Mvc.Contracts.Common;
using ChuA.ERP.Web.Mvc.Contracts.Dtos;

namespace ChuA.ERP.Web.Mvc.ViewModels.Users;

/// <summary>List page view model — wraps a paged result for the data table.</summary>
public sealed class UserListViewModel
{
    public PagedResult<UserDto> Page { get; set; } = PagedResult<UserDto>.Empty();
    public string? Search { get; set; }
}
