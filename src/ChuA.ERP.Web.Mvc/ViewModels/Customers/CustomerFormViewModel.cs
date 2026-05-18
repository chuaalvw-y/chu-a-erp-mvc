using System.ComponentModel.DataAnnotations;
using ChuA.ERP.Web.Mvc.Contracts.Dtos;

namespace ChuA.ERP.Web.Mvc.ViewModels.Customers;

/// <summary>Strongly typed form model shared by Create and Edit screens.</summary>
public sealed class CustomerFormViewModel
{
    public Guid? Id { get; set; }

    [Required, StringLength(40)]
    [Display(Name = "Customer code")]
    public string CustomerCode { get; set; } = string.Empty;

    [Required, StringLength(200)]
    [Display(Name = "Legal name")]
    public string LegalName { get; set; } = string.Empty;

    [Required, StringLength(3, MinimumLength = 3)]
    [Display(Name = "Default currency")]
    public string DefaultCurrencyCode { get; set; } = "USD";

    [Range(0, 365)]
    [Display(Name = "Payment terms (days)")]
    public int PaymentTermsDays { get; set; } = 30;

    [Required]
    [Range(0, 1_000_000_000)]
    [Display(Name = "Credit limit amount")]
    public decimal CreditLimitAmount { get; set; }

    [Required, StringLength(3, MinimumLength = 3)]
    [Display(Name = "Credit limit currency")]
    public string CreditLimitCurrencyCode { get; set; } = "USD";

    [Display(Name = "Blocked from new transactions")]
    public bool IsBlocked { get; set; }

    public bool IsEdit => Id.HasValue;

    public static CustomerFormViewModel FromDto(CustomerDto dto) => new()
    {
        Id = dto.Id,
        CustomerCode = dto.CustomerCode,
        LegalName = dto.LegalName,
        DefaultCurrencyCode = dto.DefaultCurrencyCode,
        PaymentTermsDays = dto.PaymentTermsDays,
        CreditLimitAmount = dto.CreditLimit.Amount,
        CreditLimitCurrencyCode = dto.CreditLimit.CurrencyCode,
        IsBlocked = dto.IsBlocked,
    };

    public CreateCustomerRequest ToCreateRequest() =>
        new(CustomerCode, LegalName, DefaultCurrencyCode, PaymentTermsDays, new MoneyDto(CreditLimitAmount, CreditLimitCurrencyCode));

    public UpdateCustomerRequest ToUpdateRequest() =>
        new(LegalName, DefaultCurrencyCode, PaymentTermsDays, new MoneyDto(CreditLimitAmount, CreditLimitCurrencyCode), IsBlocked);
}
