using System.ComponentModel.DataAnnotations;

namespace ChuA.ERP.Web.Mvc.ViewModels.Bills;

/// <summary>Single editable bill line row.</summary>
public sealed class BillLineFormViewModel
{
    [Required, StringLength(500)]
    [Display(Name = "Description")]
    public string Description { get; set; } = string.Empty;

    [Range(typeof(decimal), "0.0001", "79228162514264337593543950335")]
    [Display(Name = "Qty")]
    public decimal QuantityValue { get; set; } = 1m;

    [Required, StringLength(20)]
    [Display(Name = "Unit")]
    public string QuantityUnitOfMeasure { get; set; } = "EA";

    [Range(typeof(decimal), "0", "79228162514264337593543950335")]
    [Display(Name = "Unit price")]
    public decimal UnitPriceAmount { get; set; }

    [Required, StringLength(3, MinimumLength = 3)]
    [Display(Name = "Currency")]
    public string UnitPriceCurrencyCode { get; set; } = "USD";

    [Display(Name = "Expense account")]
    public Guid? ExpenseAccountId { get; set; }
}
