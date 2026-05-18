namespace ChuA.ERP.Web.Mvc.Contracts.Dtos;

public sealed record CustomerDto(
    Guid Id,
    Guid CompanyId,
    string CustomerCode,
    string LegalName,
    string DefaultCurrencyCode,
    int PaymentTermsDays,
    MoneyDto CreditLimit,
    bool IsBlocked);

public sealed record CreateCustomerRequest(
    string CustomerCode,
    string LegalName,
    string DefaultCurrencyCode,
    int PaymentTermsDays,
    MoneyDto CreditLimit);

public sealed record UpdateCustomerRequest(
    string LegalName,
    string DefaultCurrencyCode,
    int PaymentTermsDays,
    MoneyDto CreditLimit,
    bool IsBlocked);
