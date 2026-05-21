# Role-Based Guides

Condensed reference per functional role. Each section lists daily / weekly
work patterns, key permissions, and links to deeper documentation.

## Table of Contents
- [AP Clerk / AP Manager](#ap-clerk--ap-manager)
- [AR Clerk / AR Manager](#ar-clerk--ar-manager)
- [Buyer / Procurement Manager](#buyer--procurement-manager)
- [Sales Rep / Sales Manager](#sales-rep--sales-manager)
- [Warehouse](#warehouse)
- [Accountant / Controller](#accountant--controller)
- [Approver (Manager, Director, VP)](#approver-manager-director-vp)
- [Company Admin](#company-admin)
- [System Admin](#system-admin)
- [Auditor / Compliance](#auditor--compliance)
- [Executive](#executive)

---

## AP Clerk / AP Manager

**You handle**: Vendors, bills, vendor payments.

### Daily

- Open new bills received from vendors → *Purchasing › Bills › New*
- Match to existing POs (verify line items / amounts)
- Submit bills for approval (if above your auto-approve threshold)
- *AP Manager*: triage *Workflow › Approvals*, approve bills in your band
- Execute weekly / daily payment run for approved bills via *Bills › Pay*

### Weekly

- Run *Vendor Aging* report → review aged items
- Run *Bills Awaiting Approval* — escalate stale items
- Vendor master cleanup (new vendors, address updates, blocked)

### Monthly

- Reconcile AP control account to sub-ledger total
- Close out month-end accruals (uninvoiced receipts)

### Key permissions

- `BillRead`, `BillCreate` — both
- `BillApprove`, `BillPay` — AP Manager
- `VendorRead`, `VendorCreate`, `VendorUpdate` — both (Manager usually has Update)
- `WorkflowRead`, `WorkflowApprovalSubmit` — AP Manager
- `ReportRun` — both

### Deep reading

[Purchasing module](../modules/purchasing.md) · [Workflow & Approvals](../user-guide/08-workflow-approvals.md)

---

## AR Clerk / AR Manager

**You handle**: Customers, invoices, customer receipts.

### Daily

- Issue invoices for shipped SOs → *Sales › Invoices › New*
- Apply incoming customer payments → *Invoices › Apply payment*
- Investigate disputed / unapplied receipts

### Weekly

- Run *Customer Aging* → call / email overdue customers
- Review credit limits for top customers
- *AR Manager*: approve credit-limit changes

### Monthly

- Reconcile AR control account
- Period-end AR aging snapshot
- Bad-debt review

### Key permissions

- `InvoiceRead`, `InvoiceCreate` — both
- `InvoiceApplyPayment` — both
- `CustomerRead`, `CustomerCreate`, `CustomerUpdate` — both
- `SalesOrderRead` — both
- `ReportRun` — both

### Deep reading

[Sales module](../modules/sales.md) · [Workflow & Approvals](../user-guide/08-workflow-approvals.md)

---

## Buyer / Procurement Manager

**You handle**: Vendor master, raising POs, supplier relationships.

### Daily

- Raise new POs → *Purchasing › Purchase Orders › New*
- Submit for approval (if above threshold)
- *Procurement Manager*: approve POs in *Workflow › Approvals*

### Weekly

- Open-PO report — chase suppliers for late deliveries
- Review under- / over-receipts with warehouse team
- New supplier onboarding (vendor record + bank details + compliance)

### Monthly

- Spend report by vendor → strategic-sourcing conversations
- Annual vendor reviews

### Key permissions

- `PurchaseOrderRead`, `PurchaseOrderCreate` — both
- `PurchaseOrderApprove` — Procurement Manager
- `VendorRead`, `VendorCreate`, `VendorUpdate` — both
- `InventoryRead` — both
- `WorkflowApprovalSubmit` — Procurement Manager
- `ReportRun` — both

### Deep reading

[Purchasing module](../modules/purchasing.md)

---

## Sales Rep / Sales Manager

**You handle**: Customers, sales orders.

### Daily

- Customer interactions (today: external CRM; planned in-app CRM)
- Create / update sales orders → *Sales › Sales Orders*
- Coordinate with warehouse on shipment timing

### Weekly

- Open SOs report → ensure none stuck waiting on stock
- Pipeline review with sales manager

### Monthly

- Forecast accuracy review
- Commission reconciliation (planned)

### Key permissions

- `SalesOrderRead`, `SalesOrderCreate` — both
- `SalesOrderShip` — usually warehouse, sometimes Sales Manager
- `CustomerRead`, `CustomerCreate`, `CustomerUpdate` — both (Manager has Update)
- `InventoryRead` — both
- `ReportRun` — both
- `WorkflowApprovalSubmit` — Sales Manager (for credit changes)

### Deep reading

[Sales module](../modules/sales.md)

---

## Warehouse

**You handle**: Receipts, shipments, adjustments, items.

### Daily

- Receive goods against POs → *Purchase Orders › [PO] › Receive*
- Ship goods against SOs → *Sales Orders › [SO] › Ship*
- Adjust inventory (supervisor only) → *Inventory › Items › [item] › Adjust*

### Weekly

- Cycle counts (sample of items)
- Item master updates (new SKUs, retired SKUs)
- Slow-moving stock review

### Monthly

- Reconcile inventory valuation to GL
- Cycle-count adjustment summary

### Key permissions

- `InventoryRead` — all warehouse staff
- `InventoryCreate` — supervisors (to add items, run imports)
- `InventoryAdjust` — supervisors only (powerful permission)
- `PurchaseOrderRead`, `PurchaseOrderReceive` — clerks
- `SalesOrderRead`, `SalesOrderShip` — clerks

### Deep reading

[Inventory module](../modules/inventory.md)

---

## Accountant / Controller

**You handle**: COA maintenance, journal entries, period close, financial
reporting.

### Daily

- Review unmatched / unposted items
- Investigate Dashboard banner alerts
- Post manual JEs as needed (accruals, reclassifications)
- *Controller*: approve JEs (two-eyes)

### Weekly

- Trial balance sanity check
- Bank reconciliation (planned in-app; today via export)
- AR / AP aging reviews

### Monthly

- **Period close** (see [Finance module](../modules/finance.md#worked-scenarios))
- Trial balance, P&L, Balance Sheet
- Period-end accruals + reversals
- FX revaluation
- Account reconciliations
- *Controller*: sign-off

### Quarterly / Annual

- Quarter close / year-end close
- Audit support
- Retained-earnings rollover

### Key permissions

- `ChartOfAccountRead`, `ChartOfAccountCreate` — both
- `JournalEntryRead` — Accountant
- `JournalEntryPost` — Senior Accountant + Controller
- `BillRead`, `InvoiceRead` — both
- `BillApprove`, `BillPay`, `InvoiceApplyPayment` — Controller
- `ReportRun` — both
- `WorkflowApprovalSubmit` — both (for JE approvals)

### Deep reading

[Finance module](../modules/finance.md) · [Audit History](../user-guide/12-audit-history.md)

---

## Approver (Manager, Director, VP)

**You handle**: Approval tasks across documents you're authorised for.

### Daily

- Triage *Workflow › Approvals*
- Open each task, review the underlying document, decide
- Reassign anything stuck where you can't act

### Weekly

- Review escalations from the prior week
- Discuss patterns with submitters (recurring issues)

### Key permissions

- `WorkflowRead`, `WorkflowApprovalSubmit`
- Read permission on whatever you approve (`BillRead`, `PurchaseOrderRead`, `JournalEntryRead`, …)
- Approve permission for your bands (`BillApprove`, `PurchaseOrderApprove`, `JournalEntryPost`)

### Deep reading

[Workflow & Approvals](../user-guide/08-workflow-approvals.md) · [Workflow Engine](../modules/workflow-engine.md)

---

## Company Admin

**You handle**: User lifecycle, role catalogue, ad-hoc support.

### Daily

- Process new-user requests
- Unlock accounts when needed
- Respond to "I can't see X" support tickets

### Weekly

- Confirm new-hire onboarding tickets are closed
- Review who has admin roles

### Monthly

- Review user list against HR active-employee list
- Review role assignments for accuracy

### Quarterly

- Permission audit — does each user still need each role?
- Settings register review

### Annual

- Sign off on the auditor's access-control checklist
- Rotate any local-account secrets

### Key permissions

- `CompanyAdmin` role

### Deep reading

[Administrator Guide](../admin/administration.md) · [Security & Permissions](../admin/security-permissions.md)

---

## System Admin

**You handle**: Tenant-wide configuration, new Companies, platform liaison.

### Routine

- Onboard new Companies when the business expands
- Resolve cross-Company access requests
- Liaise with the platform team for settings changes
- Monitor *System Health* daily

### As needed

- Investigate cross-Company audit queries
- Coordinate maintenance / upgrades with platform team

### Key permissions

- `SystemAdmin` role

### Deep reading

[Administrator Guide](../admin/administration.md) · [Multi-Tenant guide](../admin/multi-tenant.md) · [System Settings](../admin/system-settings.md)

---

## Auditor / Compliance

**You handle**: Read-only review of the system for control effectiveness.

### Cadence

- Quarterly review of internal controls
- Sample-based testing of approvals, postings, and access
- Annual external audit support

### What to ask for

| Evidence | Where to find it |
|---|---|
| Permission catalogue + user assignments | *Administration › Users* + *Roles* (admin pulls; ask for export) |
| Sample of high-value approvals | *Workflow › Approvals* filtered, or a workflow report |
| Inventory adjustment log | Adjustment report (Planned UI; today via audit export) |
| Period-end Trial Balance | *Reports › Trial Balance* (admin runs) |
| Audit-log entries for sample dates | Admin-pulled audit export with correlation ids |

### Key permissions

- All `*Read` permissions
- `ReportRun`
- `WorkflowRead`

### Deep reading

[Audit History](../user-guide/12-audit-history.md) · [Security & Permissions](../admin/security-permissions.md)

---

## Executive

**You handle**: Approving high-value items, dashboard monitoring.

### Daily

- Quick scan of dashboard / *My approvals*
- Approve / reject items in your band

### Weekly

- KPI review with finance leadership
- Pipeline review with sales

### Quarterly / Annual

- Capex approvals
- Annual budget / forecast cycle (planned: Budgeting module)
- Audit committee reporting

### Key permissions

- `WorkflowRead`, `WorkflowApprovalSubmit`
- Read on most modules; approval on high bands
- `ReportRun`

### Deep reading

[Dashboard](../user-guide/05-dashboard.md) · [Workflow & Approvals](../user-guide/08-workflow-approvals.md)
