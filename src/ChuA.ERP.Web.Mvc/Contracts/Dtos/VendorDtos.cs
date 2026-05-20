// Copyright (c) 2026 Alvin Wilsen Chan Chua
// GitHub: chuaalvw-y
// Licensed under the Alvin Wilsen Chan Chua Proprietary Use-Only License.
// See LICENSE.txt in the project root for full license information.

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
