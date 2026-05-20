using ChuA.ERP.Web.Mvc.Contracts.Dtos;

namespace ChuA.ERP.Web.Mvc.ViewModels.BusinessRules;

public sealed class BusinessRuleListViewModel
{
    public IReadOnlyList<BusinessRuleDto> Rules { get; set; } = Array.Empty<BusinessRuleDto>();
    public string? TargetEntity { get; set; }
    public string? TriggerEvent { get; set; }
}
