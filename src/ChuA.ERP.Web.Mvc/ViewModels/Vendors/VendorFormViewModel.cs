// Copyright (c) 2026 Alvin Wilsen Chan Chua
// GitHub: chuaalvw-y
// Licensed under the Alvin Wilsen Chan Chua Proprietary Use-Only License.
// See LICENSE.txt in the project root for full license information.

using System.ComponentModel.DataAnnotations;
using ChuA.ERP.Web.Mvc.Contracts.Dtos;

namespace ChuA.ERP.Web.Mvc.ViewModels.Vendors;

/// <summary>Strongly typed form model shared by Create and Edit screens.</summary>
public sealed class VendorFormViewModel
{
    public Guid? Id { get; set; }

    [Required, StringLength(40)]
    [Display(Name = "Vendor code")]
    public string VendorCode { get; set; } = string.Empty;

    [Required, StringLength(200)]
    [Display(Name = "Legal name")]
    public string LegalName { get; set; } = string.Empty;

    [Required, StringLength(3, MinimumLength = 3)]
    [Display(Name = "Default currency")]
    public string DefaultCurrencyCode { get; set; } = "USD";

    [Range(0, 365)]
    [Display(Name = "Payment terms (days)")]
    public int PaymentTermsDays { get; set; } = 30;

    [Display(Name = "Blocked from new transactions")]
    public bool IsBlocked { get; set; }

    public bool IsEdit => Id.HasValue;

    public static VendorFormViewModel FromDto(VendorDto dto) => new()
    {
        Id = dto.Id,
        VendorCode = dto.VendorCode,
        LegalName = dto.LegalName,
        DefaultCurrencyCode = dto.DefaultCurrencyCode,
        PaymentTermsDays = dto.PaymentTermsDays,
        IsBlocked = dto.IsBlocked,
    };

    public CreateVendorRequest ToCreateRequest() => new(VendorCode, LegalName, DefaultCurrencyCode, PaymentTermsDays);
    public UpdateVendorRequest ToUpdateRequest() => new(LegalName, DefaultCurrencyCode, PaymentTermsDays, IsBlocked);
}
