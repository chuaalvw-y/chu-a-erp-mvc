using System.ComponentModel.DataAnnotations;
using ChuA.ERP.Web.Mvc.Contracts.Dtos;

namespace ChuA.ERP.Web.Mvc.ViewModels.Bills;

/// <summary>Form model for the Pay Bill action.</summary>
public sealed class PayBillFormViewModel
{
    /// <summary>The bill being paid.</summary>
    public Guid Id { get; set; }

    [Required, DataType(DataType.Date)]
    [Display(Name = "Payment date")]
    public DateOnly PaymentDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);

    [Range(typeof(decimal), "0.0001", "79228162514264337593543950335")]
    [Display(Name = "Amount")]
    public decimal Amount { get; set; }

    [Required, StringLength(3, MinimumLength = 3)]
    [Display(Name = "Currency")]
    public string CurrencyCode { get; set; } = "USD";

    [Required, StringLength(50)]
    [Display(Name = "Payment method")]
    public string PaymentMethod { get; set; } = "BankTransfer";

    [StringLength(200)]
    [Display(Name = "Reference")]
    public string? Reference { get; set; }

    public PayBillRequest ToRequest() =>
        new(PaymentDate, new MoneyDto(Amount, CurrencyCode), PaymentMethod, Reference);
}
