// Copyright (c) 2026 Alvin Wilsen Chan Chua
// GitHub: chuaalvw-y
// Licensed under the Alvin Wilsen Chan Chua Proprietary Use-Only License.
// See LICENSE.txt in the project root for full license information.

using ChuA.ERP.Web.Mvc.Contracts.Common;
using ChuA.ERP.Web.Mvc.Contracts.Dtos;

namespace ChuA.ERP.Web.Mvc.ViewModels.JournalEntries;

/// <summary>List page view model for Journal entries.</summary>
public sealed class JournalEntryListViewModel
{
    public PagedResult<JournalEntryDto> Page { get; set; } = PagedResult<JournalEntryDto>.Empty();
    public Guid? FiscalPeriodId { get; set; }
    public string? Status { get; set; }
}
