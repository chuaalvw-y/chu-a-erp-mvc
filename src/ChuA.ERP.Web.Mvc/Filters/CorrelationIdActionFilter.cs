using ChuA.ERP.Web.Mvc.Security;
using ChuA.ERP.Web.Mvc.Services;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ChuA.ERP.Web.Mvc.Filters;

/// <summary>
/// Action filter that ensures every request has a correlation id and that the
/// outbound response echoes <c>X-Correlation-ID</c> for client/log stitching.
/// </summary>
public sealed class CorrelationIdActionFilter : IAsyncActionFilter
{
    private readonly ICorrelationIdAccessor _correlationIds;

    public CorrelationIdActionFilter(ICorrelationIdAccessor correlationIds)
    {
        _correlationIds = correlationIds;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var id = _correlationIds.GetOrCreate();
        var response = context.HttpContext.Response;
        if (!response.Headers.ContainsKey(ApiHeaders.CorrelationId))
        {
            response.Headers[ApiHeaders.CorrelationId] = id;
        }
        await next().ConfigureAwait(false);
    }
}
