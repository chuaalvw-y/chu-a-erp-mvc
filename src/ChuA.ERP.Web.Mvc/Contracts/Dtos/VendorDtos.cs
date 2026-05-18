namespace ChuA.ERP.Web.Mvc.Contracts.Dtos;

public sealed record VendorDto(
    Guid Id,
    Guid CompanyId,
    string VendorCode,
    string LegalName,
    string DefaultCurrencyCode,
    int PaymentTermsDays,
    bool IsBlocked);

public sealed record CreateVendorRequest(
    string VendorCode,
    string LegalName,
    string DefaultCurrencyCode,
    int PaymentTermsDays);

public sealed record UpdateVendorRequest(
    string LegalName,
    string DefaultCurrencyCode,
    int PaymentTermsDays,
    bool IsBlocked);
