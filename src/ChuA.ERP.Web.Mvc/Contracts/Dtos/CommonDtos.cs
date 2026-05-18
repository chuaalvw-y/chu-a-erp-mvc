namespace ChuA.ERP.Web.Mvc.Contracts.Dtos;

// Mirrors of ChuA.ERP.Application.DTOs.* — kept here so the MVC has no project reference
// back to the API. Property names/order must match exactly so default JSON deserialization works.

public sealed record MoneyDto(decimal Amount, string CurrencyCode);

public sealed record QuantityDto(decimal Value, string UnitOfMeasure);

public sealed record AddressDto(
    string Line1,
    string? Line2,
    string City,
    string? State,
    string PostalCode,
    string CountryCode);
