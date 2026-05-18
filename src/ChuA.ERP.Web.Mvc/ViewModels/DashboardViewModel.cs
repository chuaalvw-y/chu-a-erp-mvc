namespace ChuA.ERP.Web.Mvc.ViewModels;

/// <summary>Tiles shown on the landing dashboard.</summary>
public sealed class DashboardViewModel
{
    public string DisplayName { get; set; } = "";
    public Guid? CompanyId { get; set; }
    public int BillsAwaitingApproval { get; set; }
    public int PendingWorkflowTasks { get; set; }
    public int OutstandingInvoices { get; set; }
    public bool ApiReachable { get; set; }
}
