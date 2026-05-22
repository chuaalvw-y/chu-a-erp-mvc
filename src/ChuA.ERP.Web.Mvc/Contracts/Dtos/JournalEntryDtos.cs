// Copyright (c) 2026 Alvin Wilsen Chan Chua
// GitHub: chuaalvw-y
// Licensed under the Alvin Wilsen Chan Chua Proprietary Use-Only License.
// See LICENSE.txt in the project root for full license information.

namespace ChuA.ERP.Web.Mvc.Contracts.Dtos;

public sealed record JournalEntryDto(
    Guid Id,
    Guid CompanyId,
    string EntryNumber,
    DateOnly EntryDate,
    string? Memo,
    string? Reference,
    Guid FiscalPeriodId,
    string CurrencyCode,
    decimal ExchangeRate,
    decimal TotalDebit,
    decimal TotalCredit,
    string Status,
    Guid? ReversedByJournalEntryId);

public sealed record JournalLineDto(
    Guid AccountId,
    decimal Debit,
    decimal Credit,
    string? Description = null);

public sealed record PostJournalEntryRequest(
    string EntryNumber,
    DateOnly EntryDate,
    Guid FiscalPeriodId,
    string CurrencyCode,
    decimal ExchangeRate,
    string? Memo,
    string? Reference,
    IReadOnlyList<JournalLineDto> Lines);

public sealed record UpdateJournalEntryRequest(
    string EntryNumber,
    DateOnly EntryDate,
    Guid FiscalPeriodId,
    string CurrencyCode,
    decimal ExchangeRate,
    string? Memo,
    string? Reference,
    IReadOnlyList<JournalLineDto> Lines);
