// Copyright (c) 2026 Alvin Wilsen Chan Chua
// GitHub: chuaalvw-y
// Licensed under the Alvin Wilsen Chan Chua Proprietary Use-Only License.
// See LICENSE.txt in the project root for full license information.

using ChuA.ERP.Web.Mvc.Contracts.Dtos;

namespace ChuA.ERP.Web.Mvc.ViewModels.BusinessRules;

public sealed class BusinessRuleListViewModel
{
    public IReadOnlyList<BusinessRuleDto> Rules { get; set; } = Array.Empty<BusinessRuleDto>();
    public string? TargetEntity { get; set; }
    public string? TriggerEvent { get; set; }
}
