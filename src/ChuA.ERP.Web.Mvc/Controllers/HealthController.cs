using ChuA.ERP.Web.Mvc.ApiClients;
using ChuA.ERP.Web.Mvc.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChuA.ERP.Web.Mvc.Controllers;

/// <summary>Surfaces the API's /health endpoints inside the MVC UI.</summary>
[Authorize]
public sealed class HealthController : Controller
{
    private readonly IHealthApiClient _health;

    public HealthController(IHealthApiClient health)
    {
        _health = health;
    }

    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var status = await _health.GetHealthAsync(cancellationToken).ConfigureAwait(false);
        var vm = new HealthIndexViewModel
        {
            ApiReachable = status.IsSuccess,
            Service = status.IsSuccess ? status.Value.Service : null,
            Status = status.IsSuccess ? status.Value.Status : "Unreachable",
            Timestamp = status.IsSuccess ? status.Value.Timestamp : null,
            ErrorMessage = status.IsFailure ? status.Errors.FirstOrDefault()?.Message : null,
        };
        return View(vm);
    }
}
