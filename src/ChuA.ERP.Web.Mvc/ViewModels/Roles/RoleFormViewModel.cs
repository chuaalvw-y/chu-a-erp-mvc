using System.ComponentModel.DataAnnotations;
using ChuA.ERP.Web.Mvc.Contracts.Dtos;

namespace ChuA.ERP.Web.Mvc.ViewModels.Roles;

/// <summary>Strongly typed form model shared by Create and Edit screens.</summary>
public sealed class RoleFormViewModel
{
    public Guid? Id { get; set; }

    [Required, StringLength(100)]
    [Display(Name = "Name")]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    [Display(Name = "Description")]
    public string? Description { get; set; }

    [Display(Name = "System role")]
    public bool IsSystemRole { get; set; }

    public bool IsEdit => Id.HasValue;

    public static RoleFormViewModel FromDto(RoleDto dto) => new()
    {
        Id = dto.Id,
        Name = dto.Name,
        Description = dto.Description,
        IsSystemRole = dto.IsSystemRole,
    };

    public CreateRoleRequest ToCreateRequest() => new(Name, Description);

    public UpdateRoleRequest ToUpdateRequest() => new(Name, Description);
}
