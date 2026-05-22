// Copyright (c) 2026 Alvin Wilsen Chan Chua
// GitHub: chuaalvw-y
// Licensed under the Alvin Wilsen Chan Chua Proprietary Use-Only License.
// See LICENSE.txt in the project root for full license information.

namespace ChuA.ERP.Web.Mvc.Contracts.Dtos;

public sealed record ChartOfAccountDto(
    Guid Id,
    Guid CompanyId,
    string AccountCode,
    string Name,
    string AccountType,
    string NormalBalance,
    bool IsPostable,
    Guid? ParentAccountId,
    string CurrencyCode);

public sealed record CreateChartOfAccountRequest(
    string AccountCode,
    string Name,
    string AccountType,
    string NormalBalance,
    bool IsPostable,
    Guid? ParentAccountId,
    string CurrencyCode);

public sealed record UpdateChartOfAccountRequest(
    string Name,
    bool IsPostable,
    Guid? ParentAccountId);
