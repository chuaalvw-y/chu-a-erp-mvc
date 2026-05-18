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
