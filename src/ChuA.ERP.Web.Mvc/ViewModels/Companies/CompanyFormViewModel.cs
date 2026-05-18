using System.ComponentModel.DataAnnotations;
using ChuA.ERP.Web.Mvc.Contracts.Dtos;

namespace ChuA.ERP.Web.Mvc.ViewModels.Companies;

/// <summary>Strongly typed form model shared by Create and Edit screens.</summary>
public sealed class CompanyFormViewModel
{
    public Guid? Id { get; set; }

    [Required, StringLength(40)]
    [Display(Name = "Code")]
    public string Code { get; set; } = string.Empty;

    [Required, StringLength(200)]
    [Display(Name = "Name")]
    public string Name { get; set; } = string.Empty;

    [StringLength(200)]
    [Display(Name = "Legal name")]
    public string? LegalName { get; set; }

    [StringLength(50)]
    [Display(Name = "Tax identification number")]
    public string? TaxIdentificationNumber { get; set; }

    [StringLength(50)]
    [Display(Name = "Registration number")]
    public string? RegistrationNumber { get; set; }

    [Required, StringLength(3, MinimumLength = 3)]
    [Display(Name = "Base currency")]
    public string BaseCurrencyCode { get; set; } = "USD";

    // Registered address (flattened)
    [Required, StringLength(200)]
    [Display(Name = "Address line 1")]
    public string AddressLine1 { get; set; } = string.Empty;

    [StringLength(200)]
    [Display(Name = "Address line 2")]
    public string? AddressLine2 { get; set; }

    [Required, StringLength(100)]
    [Display(Name = "City")]
    public string City { get; set; } = string.Empty;

    [StringLength(100)]
    [Display(Name = "State / region")]
    public string? State { get; set; }

    [Required, StringLength(20)]
    [Display(Name = "Postal code")]
    public string PostalCode { get; set; } = string.Empty;

    [Required, StringLength(2, MinimumLength = 2)]
    [Display(Name = "Country code")]
    public string CountryCode { get; set; } = "US";

    // Contact info
    [StringLength(50)]
    [Display(Name = "Phone")]
    public string? Phone { get; set; }

    [EmailAddress, StringLength(200)]
    [Display(Name = "Email")]
    public string? Email { get; set; }

    [StringLength(200)]
    [Display(Name = "Website")]
    public string? Website { get; set; }

    public bool IsEdit => Id.HasValue;

    public static CompanyFormViewModel FromDto(CompanyDto dto) => new()
    {
        Id = dto.Id,
        Code = dto.Code,
        Name = dto.Name,
        LegalName = dto.LegalName,
        TaxIdentificationNumber = dto.TaxIdentificationNumber,
        RegistrationNumber = dto.RegistrationNumber,
        BaseCurrencyCode = dto.BaseCurrencyCode,
        AddressLine1 = dto.RegisteredAddress.Line1,
        AddressLine2 = dto.RegisteredAddress.Line2,
        City = dto.RegisteredAddress.City,
        State = dto.RegisteredAddress.State,
        PostalCode = dto.RegisteredAddress.PostalCode,
        CountryCode = dto.RegisteredAddress.CountryCode,
        Phone = dto.Phone,
        Email = dto.Email,
        Website = dto.Website,
    };

    public CreateCompanyRequest ToCreateRequest() => new(
        Code,
        Name,
        LegalName,
        TaxIdentificationNumber,
        RegistrationNumber,
        BaseCurrencyCode,
        new AddressDto(AddressLine1, AddressLine2, City, State, PostalCode, CountryCode),
        null,
        Phone,
        Email,
        Website);

    public UpdateCompanyRequest ToUpdateRequest() => new(
        Name,
        LegalName,
        TaxIdentificationNumber,
        RegistrationNumber,
        new AddressDto(AddressLine1, AddressLine2, City, State, PostalCode, CountryCode),
        null,
        Phone,
        Email,
        Website);
}
