// Copyright (c) 2026 Alvin Wilsen Chan Chua
// GitHub: chuaalvw-y
// Licensed under the Alvin Wilsen Chan Chua Proprietary Use-Only License.
// See LICENSE.txt in the project root for full license information.

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
