using ChuA.ERP.Web.Mvc.ApiClients;
using ChuA.ERP.Web.Mvc.Services;
using ChuA.ERP.Web.Mvc.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChuA.ERP.Web.Mvc.Controllers;

/// <summary>Landing dashboard — summarises counts and outstanding work for the user.</summary>
[Authorize]
public sealed class DashboardController : Controller
{
    private readonly IBillsApiClient _bills;
    private readonly IInvoicesApiClient _invoices;
    private readonly IWorkflowApiClient _workflow;
    private readonly ICurrentUserService _currentUser;

    public DashboardController(
        IBillsApiClient bills,
        IInvoicesApiClient invoices,
        IWorkflowApiClient workflow,
        ICurrentUserService currentUser)
    {
        _bills = bills;
        _invoices = invoices;
        _workflow = workflow;
        _currentUser = currentUser;
    }

    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        await _currentUser.LoadProfileAsync(cancellationToken).ConfigureAwait(false);

        var billsAwaiting = await _bills.GetAwaitingApprovalAsync(cancellationToken).ConfigureAwait(false);
        var workflowTasks = await _workflow.ListTasksAsync(status: "Pending", subjectType: null, cancellationToken).ConfigureAwait(false);
        var invoices = await _invoices.ListAsync(customerId: null, status: null, paymentStatus: "Outstanding", search: null, cancellationToken).ConfigureAwait(false);

        var vm = new DashboardViewModel
        {
            DisplayName = _currentUser.DisplayName,
            CompanyId = _currentUser.CompanyId,
            BillsAwaitingApproval = billsAwaiting.IsSuccess ? billsAwaiting.Value.Count : 0,
            PendingWorkflowTasks = workflowTasks.IsSuccess ? workflowTasks.Value.Count : 0,
            OutstandingInvoices = invoices.IsSuccess ? invoices.Value.Count : 0,
            ApiReachable = billsAwaiting.IsSuccess && workflowTasks.IsSuccess && invoices.IsSuccess,
        };
        return View(vm);
    }
}
