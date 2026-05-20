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
    public const string VendorDelete = nameof(VendorDelete);

    // Customers (AR master data)
    public const string CustomerRead = nameof(CustomerRead);
    public const string CustomerCreate = nameof(CustomerCreate);
    public const string CustomerUpdate = nameof(CustomerUpdate);
    public const string CustomerDelete = nameof(CustomerDelete);

    // Finance
    public const string ChartOfAccountRead = nameof(ChartOfAccountRead);
    public const string ChartOfAccountCreate = nameof(ChartOfAccountCreate);
    public const string ChartOfAccountUpdate = nameof(ChartOfAccountUpdate);
    public const string ChartOfAccountDelete = nameof(ChartOfAccountDelete);
    public const string JournalEntryRead = nameof(JournalEntryRead);
    public const string JournalEntryCreate = nameof(JournalEntryCreate);
    public const string JournalEntryUpdate = nameof(JournalEntryUpdate);
    public const string JournalEntryDelete = nameof(JournalEntryDelete);
    public const string JournalEntryPost = nameof(JournalEntryPost);

    // Accounts Payable
    public const string BillRead = nameof(BillRead);
    public const string BillCreate = nameof(BillCreate);
    public const string BillUpdate = nameof(BillUpdate);
    public const string BillDelete = nameof(BillDelete);
    public const string BillApprove = nameof(BillApprove);
    public const string BillPay = nameof(BillPay);

    // Accounts Receivable
    public const string InvoiceRead = nameof(InvoiceRead);
    public const string InvoiceCreate = nameof(InvoiceCreate);
    public const string InvoiceUpdate = nameof(InvoiceUpdate);
    public const string InvoiceDelete = nameof(InvoiceDelete);
    public const string InvoiceApplyPayment = nameof(InvoiceApplyPayment);

    // Procurement
    public const string PurchaseOrderRead = nameof(PurchaseOrderRead);
    public const string PurchaseOrderCreate = nameof(PurchaseOrderCreate);
    public const string PurchaseOrderUpdate = nameof(PurchaseOrderUpdate);
    public const string PurchaseOrderDelete = nameof(PurchaseOrderDelete);
    public const string PurchaseOrderApprove = nameof(PurchaseOrderApprove);
    public const string PurchaseOrderReceive = nameof(PurchaseOrderReceive);

    // Inventory
    public const string InventoryRead = nameof(InventoryRead);
    public const string InventoryCreate = nameof(InventoryCreate);
    public const string InventoryUpdate = nameof(InventoryUpdate);
    public const string InventoryDelete = nameof(InventoryDelete);
    public const string InventoryAdjust = nameof(InventoryAdjust);

    // Sales
    public const string SalesOrderRead = nameof(SalesOrderRead);
    public const string SalesOrderCreate = nameof(SalesOrderCreate);
    public const string SalesOrderUpdate = nameof(SalesOrderUpdate);
    public const string SalesOrderDelete = nameof(SalesOrderDelete);
    public const string SalesOrderShip = nameof(SalesOrderShip);

    // Workflow inbox (approver-facing)
    public const string WorkflowRead = nameof(WorkflowRead);
    public const string WorkflowApprovalDecide = nameof(WorkflowApprovalDecide);
    public const string WorkflowApprovalDelegate = nameof(WorkflowApprovalDelegate);
    public const string WorkflowApprovalReassign = nameof(WorkflowApprovalReassign);

    // Workflow engine admin (slice B)
    public const string WorkflowDefinitionCreate = nameof(WorkflowDefinitionCreate);
    public const string WorkflowDefinitionUpdate = nameof(WorkflowDefinitionUpdate);
    public const string WorkflowDefinitionPublish = nameof(WorkflowDefinitionPublish);
    public const string WorkflowDefinitionRetire = nameof(WorkflowDefinitionRetire);
    public const string WorkflowConfigManage = nameof(WorkflowConfigManage);
    public const string WorkflowInstanceRead = nameof(WorkflowInstanceRead);
    public const string WorkflowInstanceCancel = nameof(WorkflowInstanceCancel);

    // Business rules (slice C — read-only V1)
    public const string BusinessRuleRead = nameof(BusinessRuleRead);

    // Reports
    public const string ReportRun = nameof(ReportRun);

    /// <summary>Returns every policy name declared on this class (useful for DI registration).</summary>
    public static IReadOnlyCollection<string> All { get; } = new[]
    {
        AuthenticatedUser, SystemAdmin, CompanyAdmin,
        VendorRead, VendorCreate, VendorUpdate, VendorDelete,
        CustomerRead, CustomerCreate, CustomerUpdate, CustomerDelete,
        ChartOfAccountRead, ChartOfAccountCreate, ChartOfAccountUpdate, ChartOfAccountDelete,
        JournalEntryRead, JournalEntryCreate, JournalEntryUpdate, JournalEntryDelete, JournalEntryPost,
        BillRead, BillCreate, BillUpdate, BillDelete, BillApprove, BillPay,
        InvoiceRead, InvoiceCreate, InvoiceUpdate, InvoiceDelete, InvoiceApplyPayment,
        PurchaseOrderRead, PurchaseOrderCreate, PurchaseOrderUpdate, PurchaseOrderDelete, PurchaseOrderApprove, PurchaseOrderReceive,
        InventoryRead, InventoryCreate, InventoryUpdate, InventoryDelete, InventoryAdjust,
        SalesOrderRead, SalesOrderCreate, SalesOrderUpdate, SalesOrderDelete, SalesOrderShip,
        WorkflowRead, WorkflowApprovalDecide, WorkflowApprovalDelegate, WorkflowApprovalReassign,
        WorkflowDefinitionCreate, WorkflowDefinitionUpdate, WorkflowDefinitionPublish, WorkflowDefinitionRetire,
        WorkflowConfigManage, WorkflowInstanceRead, WorkflowInstanceCancel,
        BusinessRuleRead,
        ReportRun
    };
}
