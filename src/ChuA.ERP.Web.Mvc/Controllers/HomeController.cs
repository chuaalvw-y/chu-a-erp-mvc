using ChuA.ERP.Web.Mvc.Models;
using ChuA.ERP.Web.Mvc.Services;
using Microsoft.AspNetCore.Mvc;

namespace ChuA.ERP.Web.Mvc.Controllers;

/// <summary>Default entry point — redirects to the dashboard, surfaces error and 404 pages.</summary>
public sealed class HomeController : Controller
{
    private readonly ICorrelationIdAccessor _correlationIds;

    public HomeController(ICorrelationIdAccessor correlationIds)
    {
        _correlationIds = correlationIds;
    }

    [HttpGet]
    public IActionResult Index() => RedirectToAction("Index", "Dashboard");

    [HttpGet]
    public IActionResult Error()
    {
        var vm = new ErrorViewModel
        {
            CorrelationId = _correlationIds.GetOrCreate(),
            Message = "An unexpected error occurred while processing your request.",
            StatusCode = 500,
        };
        return View(vm);
    }

    [HttpGet("/Home/StatusCode/{code:int}")]
    public IActionResult StatusCodeError(int code)
    {
        var vm = new ErrorViewModel
        {
            CorrelationId = _correlationIds.GetOrCreate(),
            StatusCode = code,
            Message = code switch
            {
                401 => "You need to sign in to view this resource.",
                403 => "You don't have permission to view this resource.",
                404 => "The page or record you were looking for could not be found.",
                _ => $"The server returned HTTP {code}.",
            },
        };
        if (code == 404)
        {
            return View("NotFound", vm);
        }
        if (code == 403)
        {
            return View("AccessDenied", vm);
        }
        return View("Error", vm);
    }
}
