// Copyright (c) 2026 Alvin Wilsen Chan Chua
// GitHub: chuaalvw-y
// Licensed under the Alvin Wilsen Chan Chua Proprietary Use-Only License.
// See LICENSE.txt in the project root for full license information.

using ChuA.ERP.Web.Mvc.Contracts.Common;
using ChuA.ERP.Web.Mvc.Contracts.Dtos;

namespace ChuA.ERP.Web.Mvc.ApiClients;

/// <summary>
/// Read-only client for the V1 BusinessRules API. Create/Update/Delete
/// surfaces don't exist yet — the rule evaluation engine ships in a
/// later phase.
/// </summary>
public interface IBusinessRulesApiClient
{
    Task<Result<IReadOnlyList<BusinessRuleDto>>> ListAsync(
        string? targetEntity = null,
        string? triggerEvent = null,
        CancellationToken cancellationToken = default);

    Task<Result<BusinessRuleDto>> GetAsync(Guid id, CancellationToken cancellationToken = default);
}
