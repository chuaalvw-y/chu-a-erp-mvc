// Copyright (c) 2026 Alvin Wilsen Chan Chua
// GitHub: chuaalvw-y
// Licensed under the Alvin Wilsen Chan Chua Proprietary Use-Only License.
// See LICENSE.txt in the project root for full license information.

namespace ChuA.ERP.Web.Mvc.Security;

/// <summary>
/// MVC-local mirror of the canonical <c>ChuA.ERP.Domain.Authorization.ErpPermissions</c>
/// catalogue. Controllers and tag helpers reference these constants so the MVC project
/// stays decoupled from the API and Domain assemblies; the policy strings (values) are
/// identical to the Domain catalogue, so a permission claim issued by the API or the
/// Phase J IClaimsTransformation is recognised by both sides.
///
/// <para>Phase J convention: <c>module:action</c>, lowercase, colon-separated. Adding
/// a new permission here MUST be accompanied by the matching entry in
/// <c>ErpPermissions</c> in the Domain project, otherwise the API will reject the
/// policy at startup.</para>
/// </summary>
public static class AuthorizationPolicies
{
    // ---- Baseline (not permission-backed) ---------------------------------
    public const string AuthenticatedUser = "AuthenticatedUser";
    public const string SystemAdmin       = "SystemAdmin";
    public const string CompanyAdmin      = "CompanyAdmin";

    // ---- Baseline permission ----------------------------------------------
    public const string Admin = "erp:admin";

    // ---- Dashboard --------------------------------------------------------
    public const string DashboardView      = "dashboard:view";
    public const string DashboardConfigure = "dashboard:configure";

    // ---- Identity ---------------------------------------------------------
    public const string UsersView         = "users:view";
    public const string UsersCreate       = "users:create";
    public const string UsersUpdate       = "users:update";
    public const string UsersDelete       = "users:delete";
    public const string RolesManage       = "roles:manage";
    public const string PermissionsManage = "permissions:manage";
    public const string CredentialManage  = "credential:manage";

    // ---- Organisation / Tenancy -------------------------------------------
    public const string CompanyView      = "company:view";
    public const string CompanyConfigure = "company:configure";
    public const string TenantManage     = "tenant:manage";

    // ---- Vendors ----------------------------------------------------------
    public const string VendorView   = "vendor:view";
    public const string VendorCreate = "vendor:create";
    public const string VendorUpdate = "vendor:update";
    public const string VendorDelete = "vendor:delete";

    // ---- Customers --------------------------------------------------------
    public const string CustomerView   = "customer:view";
    public const string CustomerCreate = "customer:create";
    public const string CustomerUpdate = "customer:update";
    public const string CustomerDelete = "customer:delete";

    // ---- Purchase Orders --------------------------------------------------
    public const string PurchaseOrderView    = "purchase-order:view";
    public const string PurchaseOrderCreate  = "purchase-order:create";
    public const string PurchaseOrderUpdate  = "purchase-order:update";
    public const string PurchaseOrderDelete  = "purchase-order:delete";
    public const string PurchaseOrderApprove = "purchase-order:approve";
    public const string PurchaseOrderCancel  = "purchase-order:cancel";
    public const string PurchaseOrderReceive = "purchase-order:receive";

    // ---- Sales Orders -----------------------------------------------------
    public const string SalesOrderView    = "sales-order:view";
    public const string SalesOrderCreate  = "sales-order:create";
    public const string SalesOrderUpdate  = "sales-order:update";
    public const string SalesOrderDelete  = "sales-order:delete";
    public const string SalesOrderApprove = "sales-order:approve";
    public const string SalesOrderCancel  = "sales-order:cancel";
    public const string SalesOrderShip    = "sales-order:ship";

    // ---- Inventory --------------------------------------------------------
    public const string InventoryView     = "inventory:view";
    public const string InventoryCreate   = "inventory:create";
    public const string InventoryUpdate   = "inventory:update";
    public const string InventoryDelete   = "inventory:delete";
    public const string InventoryAdjust   = "inventory:adjust";
    public const string InventoryTransfer = "inventory:transfer";
    public const string InventoryCount    = "inventory:count";

    // ---- Invoices (AR) ----------------------------------------------------
    public const string InvoiceView         = "invoice:view";
    public const string InvoiceCreate       = "invoice:create";
    public const string InvoiceUpdate       = "invoice:update";
    public const string InvoiceDelete       = "invoice:delete";
    public const string InvoiceApprove      = "invoice:approve";
    public const string InvoicePost         = "invoice:post";
    public const string InvoiceVoid         = "invoice:void";
    public const string InvoiceApplyPayment = "invoice:apply-payment";

    // ---- Bills (AP) -------------------------------------------------------
    public const string BillView    = "bill:view";
    public const string BillCreate  = "bill:create";
    public const string BillUpdate  = "bill:update";
    public const string BillDelete  = "bill:delete";
    public const string BillApprove = "bill:approve";
    public const string BillPay     = "bill:pay";
    public const string BillVoid    = "bill:void";

    // ---- Payments ---------------------------------------------------------
    public const string PaymentView    = "payment:view";
    public const string PaymentCreate  = "payment:create";
    public const string PaymentApprove = "payment:approve";
    public const string PaymentVoid    = "payment:void";

    // ---- General Ledger ---------------------------------------------------
    public const string JournalEntryView    = "journal-entry:view";
    public const string JournalEntryCreate  = "journal-entry:create";
    public const string JournalEntryUpdate  = "journal-entry:update";
    public const string JournalEntryDelete  = "journal-entry:delete";
    public const string JournalEntryApprove = "journal-entry:approve";
    public const string JournalEntryPost    = "journal-entry:post";
    public const string JournalEntryVoid    = "journal-entry:void";

    public const string ChartOfAccountView   = "chart-of-accounts:view";
    public const string ChartOfAccountCreate = "chart-of-accounts:create";
    public const string ChartOfAccountUpdate = "chart-of-accounts:update";
    public const string ChartOfAccountDelete = "chart-of-accounts:delete";

    // ---- Workflow ---------------------------------------------------------
    public const string WorkflowView      = "workflow:view";
    public const string WorkflowConfigure = "workflow:configure";
    public const string WorkflowApprove   = "workflow:approve";
    public const string WorkflowDelegate  = "workflow:delegate";
    public const string WorkflowCancel    = "workflow:cancel";

    // ---- Business rules ---------------------------------------------------
    public const string BusinessRuleView   = "business-rule:view";
    public const string BusinessRuleManage = "business-rule:manage";

    // ---- Reports ----------------------------------------------------------
    public const string ReportsView   = "reports:view";
    public const string ReportsExport = "reports:export";
    public const string ReportsAdmin  = "reports:admin";

    // ---- Audit ------------------------------------------------------------
    public const string AuditView   = "audit:view";
    public const string AuditExport = "audit:export";

    // ---- System -----------------------------------------------------------
    public const string SystemConfigure = "system:configure";
    public const string SystemHealth    = "system:health";

    /// <summary>Returns every policy name declared on this class (useful for DI registration).</summary>
    public static IReadOnlyCollection<string> All { get; } = new[]
    {
        AuthenticatedUser, SystemAdmin, CompanyAdmin,
        Admin,
        DashboardView, DashboardConfigure,
        UsersView, UsersCreate, UsersUpdate, UsersDelete,
        RolesManage, PermissionsManage, CredentialManage,
        CompanyView, CompanyConfigure, TenantManage,
        VendorView, VendorCreate, VendorUpdate, VendorDelete,
        CustomerView, CustomerCreate, CustomerUpdate, CustomerDelete,
        PurchaseOrderView, PurchaseOrderCreate, PurchaseOrderUpdate, PurchaseOrderDelete,
        PurchaseOrderApprove, PurchaseOrderCancel, PurchaseOrderReceive,
        SalesOrderView, SalesOrderCreate, SalesOrderUpdate, SalesOrderDelete,
        SalesOrderApprove, SalesOrderCancel, SalesOrderShip,
        InventoryView, InventoryCreate, InventoryUpdate, InventoryDelete,
        InventoryAdjust, InventoryTransfer, InventoryCount,
        InvoiceView, InvoiceCreate, InvoiceUpdate, InvoiceDelete,
        InvoiceApprove, InvoicePost, InvoiceVoid, InvoiceApplyPayment,
        BillView, BillCreate, BillUpdate, BillDelete, BillApprove, BillPay, BillVoid,
        PaymentView, PaymentCreate, PaymentApprove, PaymentVoid,
        JournalEntryView, JournalEntryCreate, JournalEntryUpdate, JournalEntryDelete,
        JournalEntryApprove, JournalEntryPost, JournalEntryVoid,
        ChartOfAccountView, ChartOfAccountCreate, ChartOfAccountUpdate, ChartOfAccountDelete,
        WorkflowView, WorkflowConfigure, WorkflowApprove, WorkflowDelegate, WorkflowCancel,
        BusinessRuleView, BusinessRuleManage,
        ReportsView, ReportsExport, ReportsAdmin,
        AuditView, AuditExport,
        SystemConfigure, SystemHealth,
    };
}
