using System.ComponentModel.DataAnnotations;
using ChuA.ERP.Web.Mvc.Contracts.Dtos;

namespace ChuA.ERP.Web.Mvc.ViewModels.ChartOfAccounts;

/// <summary>Strongly typed form model shared by Create and Edit screens.</summary>
public sealed class ChartOfAccountFormViewModel
{
    public Guid? Id { get; set; }

    [Required, StringLength(40)]
    [Display(Name = "Account code")]
    public string AccountCode { get; set; } = string.Empty;

    [Required, StringLength(200)]
    [Display(Name = "Name")]
    public string Name { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Account type")]
    public string AccountType { get; set; } = "Asset";

    [Required]
    [Display(Name = "Normal balance")]
    public string NormalBalance { get; set; } = "Debit";

    [Display(Name = "Postable")]
    public bool IsPostable { get; set; } = true;

    [Display(Name = "Parent account")]
    public Guid? ParentAccountId { get; set; }

    [Required, StringLength(3, MinimumLength = 3)]
    [Display(Name = "Currency")]
    public string CurrencyCode { get; set; } = "USD";

    public bool IsEdit => Id.HasValue;

    public static ChartOfAccountFormViewModel FromDto(ChartOfAccountDto dto) => new()
    {
        Id = dto.Id,
        AccountCode = dto.AccountCode,
        Name = dto.Name,
        AccountType = dto.AccountType,
        NormalBalance = dto.NormalBalance,
        IsPostable = dto.IsPostable,
        ParentAccountId = dto.ParentAccountId,
        CurrencyCode = dto.CurrencyCode,
    };

    public CreateChartOfAccountRequest ToCreateRequest() =>
        new(AccountCode, Name, AccountType, NormalBalance, IsPostable, ParentAccountId, CurrencyCode);

    public UpdateChartOfAccountRequest ToUpdateRequest() =>
        new(Name, IsPostable, ParentAccountId);
}
