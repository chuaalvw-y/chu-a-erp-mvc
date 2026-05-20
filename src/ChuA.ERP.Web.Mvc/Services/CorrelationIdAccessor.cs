// Copyright (c) 2026 Alvin Wilsen Chan Chua
// GitHub: chuaalvw-y
// Licensed under the Alvin Wilsen Chan Chua Proprietary Use-Only License.
// See LICENSE.txt in the project root for full license information.

using ChuA.ERP.Web.Mvc.Security;
using Microsoft.AspNetCore.Http;

namespace ChuA.ERP.Web.Mvc.Services;

/// <summary>
/// Default <see cref="ICorrelationIdAccessor"/> implementation. Reads the
/// <c>X-Correlation-ID</c> header from the inbound request, falling back to the
/// <see cref="HttpContext.TraceIdentifier"/>, and stashes the result into
/// <see cref="HttpContext.Items"/> so subsequent accessors are stable.
/// </summary>
public sealed class CorrelationIdAccessor : ICorrelationIdAccessor
{
    internal const string ItemsKey = "ChuA.ERP.CorrelationId";

    private readonly IHttpContextAccessor _httpContextAccessor;

    public CorrelationIdAccessor(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string GetOrCreate()
    {
        var ctx = _httpContextAccessor.HttpContext;
        if (ctx is null)
        {
            return Guid.NewGuid().ToString("N");
        }

        if (ctx.Items.TryGetValue(ItemsKey, out var existing) && existing is string s && !string.IsNullOrWhiteSpace(s))
        {
            return s;
        }

        var header = ctx.Request.Headers[ApiHeaders.CorrelationId].ToString();
        var id = !string.IsNullOrWhiteSpace(header) ? header : ctx.TraceIdentifier;
        ctx.Items[ItemsKey] = id;
        return id;
    }
}
