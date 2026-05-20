// Copyright (c) 2026 Alvin Wilsen Chan Chua
// GitHub: chuaalvw-y
// Licensed under the Alvin Wilsen Chan Chua Proprietary Use-Only License.
// See LICENSE.txt in the project root for full license information.

namespace ChuA.ERP.Web.Mvc.Contracts.Dtos;

/// <summary>Mirror of <c>ChuA.ERP.Application.DTOs.RuleConditionDto</c>.</summary>
public sealed record RuleConditionDto(
    Guid Id,
    short Sequence,
    string FieldPath,
    string Operator,
    string? Value,
    string LogicalConnector);

/// <summary>Mirror of <c>ChuA.ERP.Application.DTOs.RuleActionDto</c>.</summary>
public sealed record RuleActionDto(
    Guid Id,
    short Sequence,
    string ActionType,
    string? Parameters);

/// <summary>
/// Mirror of <c>ChuA.ERP.Application.DTOs.BusinessRuleDto</c>. V1 read-only
/// — the evaluation engine for these rules ships in a later phase.
/// </summary>
public sealed record BusinessRuleDto(
    Guid Id,
    Guid CompanyId,
    string Code,
    string Name,
    string? Description,
    string TargetEntity,
    string TriggerEvent,
    int Priority,
    bool StopOnMatch,
    IReadOnlyCollection<RuleConditionDto> Conditions,
    IReadOnlyCollection<RuleActionDto> Actions);
