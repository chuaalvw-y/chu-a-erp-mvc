// Copyright (c) 2026 Alvin Wilsen Chan Chua
// GitHub: chuaalvw-y
// Licensed under the Alvin Wilsen Chan Chua Proprietary Use-Only License.
// See LICENSE.txt in the project root for full license information.

namespace ChuA.ERP.Web.Mvc.Contracts.Dtos;

public sealed record CompanyDto(
    Guid Id,
    string Code,
    string Name,
    string? LegalName,
    string? TaxIdentificationNumber,
    string? RegistrationNumber,
    string BaseCurrencyCode,
    AddressDto RegisteredAddress,
    AddressDto? MailingAddress,
    string? Phone,
    string? Email,
    string? Website);

public sealed record CreateCompanyRequest(
    string Code,
    string Name,
    string? LegalName,
    string? TaxIdentificationNumber,
    string? RegistrationNumber,
    string BaseCurrencyCode,
    AddressDto RegisteredAddress,
    AddressDto? MailingAddress,
    string? Phone,
    string? Email,
    string? Website);

public sealed record UpdateCompanyRequest(
    string Name,
    string? LegalName,
    string? TaxIdentificationNumber,
    string? RegistrationNumber,
    AddressDto RegisteredAddress,
    AddressDto? MailingAddress,
    string? Phone,
    string? Email,
    string? Website);

public sealed record RoleDto(
    Guid Id,
    string Name,
    string? Description,
    bool IsSystemRole);

public sealed record CreateRoleRequest(string Name, string? Description);

public sealed record UpdateRoleRequest(string Name, string? Description);

public sealed record UserDto(
    Guid Id,
    string UserName,
    string Email,
    string FirstName,
    string LastName,
    bool EmailConfirmed,
    bool TwoFactorEnabled,
    Guid? EmployeeId);

public sealed record CreateUserRequest(
    string UserName,
    string Email,
    string FirstName,
    string LastName,
    Guid? EmployeeId);

public sealed record UpdateUserRequest(
    string Email,
    string FirstName,
    string LastName,
    bool TwoFactorEnabled,
    Guid? EmployeeId);

/// <summary>
/// Mirror of <c>ChuA.ERP.Application.DTOs.ErpAuthorizationProfileDto</c> on
/// the MVC side. Returned by <c>GET /api/v1/users/me</c>. Consumed by:
/// <list type="bullet">
///   <item>The MVC <c>ErpClaimsTransformation</c> — hydrates role and
///         permission claims onto the cookie principal once per request.</item>
///   <item><c>CurrentUserService</c> — surfaces the active company + the
///         memberships dropdown to layout/nav code.</item>
///   <item>The diagnostic /Account/Claims page.</item>
/// </list>
/// </summary>
public sealed record CurrentUserDto(
    Guid UserId,
    string? Email,
    string? DisplayName,
    Guid? ActiveCompanyId,
    IReadOnlyList<MembershipDto> Companies,
    IReadOnlyList<string> Roles,
    IReadOnlyList<string> Permissions);

/// <summary>One membership row from <c>identity.UserCompanies</c>.</summary>
public sealed record MembershipDto(Guid CompanyId, bool IsDefault);

public sealed record ReportSummaryDto(string Code, string Name, string Description);

public sealed record OpenDocumentRow(
    Guid CompanyId,
    string DocumentType,
    Guid DocumentId,
    string DocumentNumber,
    DateOnly DocumentDate,
    DateOnly? DueDate,
    int Status,
    string CounterpartyCode,
    string CounterpartyName,
    string CurrencyCode,
    decimal TotalAmount,
    decimal OutstandingAmount);

public sealed record HealthStatusDto(string Status, string Service, DateTimeOffset Timestamp);
