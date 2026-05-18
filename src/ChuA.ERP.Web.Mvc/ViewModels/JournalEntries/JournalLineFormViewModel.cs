using System.ComponentModel.DataAnnotations;

namespace ChuA.ERP.Web.Mvc.ViewModels.JournalEntries;

/// <summary>Single editable journal line row.</summary>
public sealed class JournalLineFormViewModel
{
    [Required]
    [Display(Name = "Account")]
    public Guid AccountId { get; set; }

    [Range(typeof(decimal), "0", "79228162514264337593543950335")]
    [Display(Name = "Debit")]
    public decimal Debit { get; set; }

    [Range(typeof(decimal), "0", "79228162514264337593543950335")]
    [Display(Name = "Credit")]
    public decimal Credit { get; set; }

    [StringLength(500)]
    [Display(Name = "Description")]
    public string? Description { get; set; }
}
