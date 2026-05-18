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

public sealed record CurrentUserDto(
    string UserId,
    Guid CompanyId,
    IReadOnlyList<string> Roles,
    IReadOnlyList<string> Permissions);

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
