// Copyright (c) 2026 Alvin Wilsen Chan Chua
// GitHub: chuaalvw-y
// Licensed under the Alvin Wilsen Chan Chua Proprietary Use-Only License.
// See LICENSE.txt in the project root for full license information.

using ChuA.ERP.Web.Mvc.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;

namespace ChuA.ERP.Web.Mvc.Filters;

/// <summary>
/// Catches unhandled exceptions raised from MVC actions and surfaces them as the
/// shared Error view, logging the correlation id for traceability.
/// </summary>
public sealed class GlobalExceptionFilter : IExceptionFilter
{
    private readonly ILogger<GlobalExceptionFilter> _logger;
    private readonly ICorrelationIdAccessor _correlationIds;

    public GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger, ICorrelationIdAccessor correlationIds)
    {
        _logger = logger;
        _correlationIds = correlationIds;
    }

    public void OnException(ExceptionContext context)
    {
        var correlationId = _correlationIds.GetOrCreate();
        _logger.LogError(context.Exception,
            "Unhandled exception in {Controller}.{Action} (correlation {CorrelationId})",
            context.RouteData.Values["controller"], context.RouteData.Values["action"], correlationId);

        var viewData = new ViewDataDictionary<Models.ErrorViewModel>(
            new EmptyModelMetadataProvider(),
            new ModelStateDictionary())
        {
            Model = new Models.ErrorViewModel
            {
                CorrelationId = correlationId,
                Message = "An unexpected error occurred while processing your request.",
                StatusCode = 500,
            }
        };

        context.Result = new ViewResult
        {
            ViewName = "Error",
            ViewData = viewData,
        };
        context.ExceptionHandled = true;
    }
}
