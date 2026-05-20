// Copyright (c) 2026 Alvin Wilsen Chan Chua
// GitHub: chuaalvw-y
// Licensed under the Alvin Wilsen Chan Chua Proprietary Use-Only License.
// See LICENSE.txt in the project root for full license information.

using ChuA.ERP.Web.Mvc.Contracts.Common;
using ChuA.ERP.Web.Mvc.Contracts.Dtos;

namespace ChuA.ERP.Web.Mvc.ApiClients;

/// <summary>Calls the API's health endpoints.</summary>
public interface IHealthApiClient
{
    Task<Result<HealthStatusDto>> GetHealthAsync(CancellationToken cancellationToken = default);
    Task<Result> GetLiveAsync(CancellationToken cancellationToken = default);
    Task<Result> GetReadyAsync(CancellationToken cancellationToken = default);
}
