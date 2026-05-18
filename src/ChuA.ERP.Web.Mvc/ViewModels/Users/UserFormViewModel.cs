using System.ComponentModel.DataAnnotations;
using ChuA.ERP.Web.Mvc.Contracts.Dtos;

namespace ChuA.ERP.Web.Mvc.ViewModels.Users;

/// <summary>Strongly typed form model shared by Create and Edit screens.</summary>
public sealed class UserFormViewModel
{
    public Guid? Id { get; set; }

    [Required, StringLength(100)]
    [Display(Name = "User name")]
    public string UserName { get; set; } = string.Empty;

    [Required, EmailAddress, StringLength(200)]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [Required, StringLength(100)]
    [Display(Name = "First name")]
    public string FirstName { get; set; } = string.Empty;

    [Required, StringLength(100)]
    [Display(Name = "Last name")]
    public string LastName { get; set; } = string.Empty;

    [Display(Name = "Employee id")]
    public Guid? EmployeeId { get; set; }

    [Display(Name = "Two-factor authentication enabled")]
    public bool TwoFactorEnabled { get; set; }

    public bool IsEdit => Id.HasValue;

    public static UserFormViewModel FromDto(UserDto dto) => new()
    {
        Id = dto.Id,
        UserName = dto.UserName,
        Email = dto.Email,
        FirstName = dto.FirstName,
        LastName = dto.LastName,
        EmployeeId = dto.EmployeeId,
        TwoFactorEnabled = dto.TwoFactorEnabled,
    };

    public CreateUserRequest ToCreateRequest() =>
        new(UserName, Email, FirstName, LastName, EmployeeId);

    public UpdateUserRequest ToUpdateRequest() =>
        new(Email, FirstName, LastName, TwoFactorEnabled, EmployeeId);
}
