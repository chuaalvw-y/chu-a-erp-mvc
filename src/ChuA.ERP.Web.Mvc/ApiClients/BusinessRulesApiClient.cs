// Copyright (c) 2026 Alvin Wilsen Chan Chua
// GitHub: chuaalvw-y
// Licensed under the Alvin Wilsen Chan Chua Proprietary Use-Only License.
// See LICENSE.txt in the project root for full license information.

using ChuA.ERP.Web.Mvc.Contracts.Common;
using ChuA.ERP.Web.Mvc.Contracts.Dtos;
using ChuA.ERP.Web.Mvc.Services;
using Microsoft.Extensions.Logging;

namespace ChuA.ERP.Web.Mvc.ApiClients;

/// <inheritdoc cref="IBusinessRulesApiClient"/>
public sealed class BusinessRulesApiClient : ApiClientBase, IBusinessRulesApiClient
{
    public BusinessRulesApiClient(
        HttpClient httpClient,
        ITokenAcquisitionService tokenAcquisition,
        ICorrelationIdAccessor correlationIds,
        ILogger<BusinessRulesApiClient> logger)
        : base(httpClient, tokenAcquisition, correlationIds, logger)
    {
    }

    public Task<Result<IReadOnlyList<BusinessRuleDto>>> ListAsync(
        string? targetEntity = null,
        string? triggerEvent = null,
        CancellationToken cancellationToken = default) =>
        SendAsync<IReadOnlyList<BusinessRuleDto>>(
            HttpMethod.Get,
            "v1/business-rules" + QueryString(("targetEntity", targetEntity), ("triggerEvent", triggerEvent)),
            cancellationToken: cancellationToken);

    public Task<Result<BusinessRuleDto>> GetAsync(Guid id, CancellationToken cancellationToken = default) =>
        SendAsync<BusinessRuleDto>(HttpMethod.Get, $"v1/business-rules/{id}", cancellationToken: cancellationToken);
}
