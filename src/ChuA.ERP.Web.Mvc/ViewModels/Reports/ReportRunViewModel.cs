using System.ComponentModel.DataAnnotations;
using ChuA.ERP.Web.Mvc.Contracts.Dtos;

namespace ChuA.ERP.Web.Mvc.ViewModels.Reports;

/// <summary>Strongly typed view model for the Run report screen.</summary>
public sealed class ReportRunViewModel
{
    [Required]
    public string Code { get; set; } = string.Empty;

    public string? Name { get; set; }

    public string? Description { get; set; }

    [Required]
    [Display(Name = "Parameters (JSON)")]
    public string ParametersJson { get; set; } = "{}";

    public IReadOnlyList<OpenDocumentRow>? Results { get; set; }
}
