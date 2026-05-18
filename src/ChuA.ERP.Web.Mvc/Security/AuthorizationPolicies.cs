namespace ChuA.ERP.Web.Mvc.Security;

/// <summary>
/// Mirror of <c>ChuA.ERP.Api.Constants.AuthorizationPolicies</c>. The MVC declares the same
/// policy names locally so controllers and tag helpers can decorate actions without taking a
/// project reference to the API.
/// </summary>
public static class AuthorizationPolicies
{
    // Baseline
    public const string AuthenticatedUser = "AuthenticatedUser";
    public const string SystemAdmin = "SystemAdmin";
    public const string CompanyAdmin = "CompanyAdmin";

    // Vendors (AP master data)
    public const string VendorRead = nameof(VendorRead);
    public const string VendorCreate = nameof(VendorCreate);
    public const string VendorUpdate = nameof(VendorUpdate);

    // Customers (AR master data)
    public const string CustomerRead = nameof(CustomerRead);
    public const string CustomerCreate = nameof(CustomerCreate);
    public const string CustomerUpdate = nameof(CustomerUpdate);

    // Finance
    public const string ChartOfAccountRead = nameof(ChartOfAccountRead);
    public const string ChartOfAccountCreate = nameof(ChartOfAccountCreate);
    public const string JournalEntryRead = nameof(JournalEntryRead);
    public const string JournalEntryPost = nameof(JournalEntryPost);

    // Accounts Payable
    public const string BillRead = nameof(BillRead);
    public const string BillCreate = nameof(BillCreate);
    public const string BillApprove = nameof(BillApprove);
    public const string BillPay = nameof(BillPay);

    // Accounts Receivable
    public const string InvoiceRead = nameof(InvoiceRead);
    public const string InvoiceCreate = nameof(InvoiceCreate);
    public const string InvoiceApplyPayment = nameof(InvoiceApplyPayment);

    // Procurement
    public const string PurchaseOrderRead = nameof(PurchaseOrderRead);
    public const string PurchaseOrderCreate = nameof(PurchaseOrderCreate);
    public const string PurchaseOrderApprove = nameof(PurchaseOrderApprove);
    public const string PurchaseOrderReceive = nameof(PurchaseOrderReceive);

    // Inventory
    public const string InventoryRead = nameof(InventoryRead);
    public const string InventoryCreate = nameof(InventoryCreate);
    public const string InventoryAdjust = nameof(InventoryAdjust);

    // Sales
    public const string SalesOrderRead = nameof(SalesOrderRead);
    public const string SalesOrderCreate = nameof(SalesOrderCreate);
    public const string SalesOrderShip = nameof(SalesOrderShip);

    // Workflow
    public const string WorkflowRead = nameof(WorkflowRead);
    public const string WorkflowApprovalSubmit = nameof(WorkflowApprovalSubmit);

    // Reports
    public const string ReportRun = nameof(ReportRun);

    /// <summary>Returns every policy name declared on this class (useful for DI registration).</summary>
    public static IReadOnlyCollection<string> All { get; } = new[]
    {
        AuthenticatedUser, SystemAdmin, CompanyAdmin,
        VendorRead, VendorCreate, VendorUpdate,
        CustomerRead, CustomerCreate, CustomerUpdate,
        ChartOfAccountRead, ChartOfAccountCreate,
        JournalEntryRead, JournalEntryPost,
        BillRead, BillCreate, BillApprove, BillPay,
        InvoiceRead, InvoiceCreate, InvoiceApplyPayment,
        PurchaseOrderRead, PurchaseOrderCreate, PurchaseOrderApprove, PurchaseOrderReceive,
        InventoryRead, InventoryCreate, InventoryAdjust,
        SalesOrderRead, SalesOrderCreate, SalesOrderShip,
        WorkflowRead, WorkflowApprovalSubmit,
        ReportRun
    };
}
