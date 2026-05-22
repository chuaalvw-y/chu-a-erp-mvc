// Copyright (c) 2026 Alvin Wilsen Chan Chua
// GitHub: chuaalvw-y
// Licensed under the Alvin Wilsen Chan Chua Proprietary Use-Only License.
// See LICENSE.txt in the project root for full license information.

using System.ComponentModel.DataAnnotations;
using ChuA.ERP.Web.Mvc.Contracts.Dtos;

namespace ChuA.ERP.Web.Mvc.ViewModels.JournalEntries;

/// <summary>Strongly typed form model shared by Create and Edit screens for Journal entries.</summary>
public sealed class JournalEntryFormViewModel : IValidatableObject
{
    public Guid? Id { get; set; }

    [Required, StringLength(60)]
    [Display(Name = "Entry number")]
    public string EntryNumber { get; set; } = string.Empty;

    [Required, DataType(DataType.Date)]
    [Display(Name = "Entry date")]
    public DateOnly EntryDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);

    [Required]
    [Display(Name = "Fiscal period")]
    public Guid FiscalPeriodId { get; set; }

    [Required, StringLength(3, MinimumLength = 3)]
    [Display(Name = "Currency")]
    public string CurrencyCode { get; set; } = "USD";

    [Display(Name = "Exchange rate")]
    public decimal ExchangeRate { get; set; } = 1.0m;

    [StringLength(500)]
    [Display(Name = "Memo")]
    public string? Memo { get; set; }

    [StringLength(200)]
    [Display(Name = "Reference")]
    public string? Reference { get; set; }

    [Display(Name = "Lines")]
    public List<JournalLineFormViewModel> Lines { get; set; } = new()
    {
        new JournalLineFormViewModel(),
        new JournalLineFormViewModel(),
    };

    public bool IsEdit => Id.HasValue;

    public decimal TotalDebit => Lines.Sum(l => l.Debit);
    public decimal TotalCredit => Lines.Sum(l => l.Credit);

    /// <remarks>
    /// Lines stay empty when loading from a list endpoint that does not return lines.
    /// </remarks>
    public static JournalEntryFormViewModel FromDto(JournalEntryDto dto) => new()
    {
        Id = dto.Id,
        EntryNumber = dto.EntryNumber,
        EntryDate = dto.EntryDate,
        FiscalPeriodId = dto.FiscalPeriodId,
        CurrencyCode = dto.CurrencyCode,
        ExchangeRate = dto.ExchangeRate,
        Memo = dto.Memo,
        Reference = dto.Reference,
        Lines = new List<JournalLineFormViewModel>(),
    };

    private IReadOnlyList<JournalLineDto> ToLineDtos() =>
        Lines.Select(l => new JournalLineDto(l.AccountId, l.Debit, l.Credit, l.Description)).ToArray();

    public PostJournalEntryRequest ToPostRequest() =>
        new(EntryNumber, EntryDate, FiscalPeriodId, CurrencyCode, ExchangeRate, Memo, Reference, ToLineDtos());

    public UpdateJournalEntryRequest ToUpdateRequest() =>
        new(EntryNumber, EntryDate, FiscalPeriodId, CurrencyCode, ExchangeRate, Memo, Reference, ToLineDtos());

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (TotalDebit != TotalCredit)
        {
            yield return new ValidationResult(
                $"Total debit ({TotalDebit:N2}) must equal total credit ({TotalCredit:N2}).",
                new[] { nameof(Lines) });
        }
        if (TotalDebit == 0m && TotalCredit == 0m)
        {
            yield return new ValidationResult(
                "Journal entry must have at least one non-zero debit or credit.",
                new[] { nameof(Lines) });
        }
    }
}
