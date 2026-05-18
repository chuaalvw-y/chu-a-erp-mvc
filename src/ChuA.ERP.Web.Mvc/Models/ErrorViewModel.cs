namespace ChuA.ERP.Web.Mvc.Models;

/// <summary>View model for /Home/Error and the global exception filter.</summary>
public sealed class ErrorViewModel
{
    public string? CorrelationId { get; set; }
    public string? Message { get; set; }
    public int? StatusCode { get; set; }
    public string? ErrorCode { get; set; }
    public IReadOnlyCollection<string>? Details { get; set; }

    public bool ShowCorrelationId => !string.IsNullOrWhiteSpace(CorrelationId);
}
