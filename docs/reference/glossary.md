# Glossary

A reference for ERP and accounting terminology used throughout this
documentation library.

## A

**Account** — A line in the Chart of Accounts; a category for postings (e.g.
*Cash*, *Accounts Receivable*, *Marketing Expense*).

**Account type** — One of *Asset*, *Liability*, *Equity*, *Revenue*,
*Expense* — categorises the account for financial statements.

**Accrual** — A journal entry recognising an expense or revenue in a
period before the cash movement happens. Reverses in the following
period.

**Active company** — The Company you are currently signed in to. Every
transaction you create is tagged with this Company id.

**AP / Accounts Payable** — What you owe vendors. Maintained via the
Bills sub-ledger; balances roll up into the AP control account in the
GL.

**Approval task** — A workflow item that requires a decision (Approve /
Reject) before the underlying document can advance.

**AR / Accounts Receivable** — What customers owe you. Maintained via
the Invoices sub-ledger; balances roll up into the AR control account.

**Audit log** — Immutable record of every state-changing action: who,
when, what, why. See [Audit History](../user-guide/12-audit-history.md).

**Authentication** — Proving you are who you claim to be (sign-in).

**Authorisation** — Proving you are allowed to do a specific thing
(permission check).

**Available quantity** — On-hand minus quantity reserved for open Sales
Orders.

## B

**Base currency** — The reporting currency of a Company. Set at Company
creation; cannot change after transactions exist.

**Bill** — A vendor's invoice as recorded in the system. Has its own
lifecycle (Draft → Submitted → Approved → Paid).

**Blocked vendor / customer** — A flag preventing new POs / SOs /
invoices against the counterparty. Existing documents are unaffected.

**Breadcrumb** — Navigation aid showing the path through page hierarchy
(*Dashboard › Vendors › Acme Corp*).

## C

**Chart of Accounts (COA)** — The hierarchical catalogue of GL accounts.

**Company** — A legal entity / accounting unit inside a tenant. Multi-
company tenants host several Companies.

**Concurrency conflict** — Two users edited the same record at the same
time. The system rejects the later save and asks the second user to
refresh.

**Correlation id** — Unique trace identifier for one request. Shown in
the page footer. Provide it when reporting issues.

**Credit (Cr)** — The right-hand side of a journal entry. Increases
Liability / Equity / Revenue; decreases Asset / Expense.

**Credit hold** — A block on new sales activity due to the customer
exceeding their credit limit or being in dispute.

**Credit limit** — The maximum outstanding receivable balance allowed
for a customer.

**Credit memo** — A negative invoice (or its planned ChuA.ERP
equivalent). Reduces a customer's outstanding balance.

**Currency code** — Three-letter ISO 4217 code (USD, EUR, GBP, JPY, …).

**Cycle count** — A periodic physical count of a sample of inventory to
validate system on-hand quantities.

## D

**Dashboard** — The landing page summarising KPIs and pending work.

**Debit (Dr)** — The left-hand side of a journal entry. Increases Asset
/ Expense; decreases Liability / Equity / Revenue.

**Default currency** — The currency the system pre-fills for a vendor /
customer based on master-data settings.

**Delegation** — Forwarding your approval tasks to someone else for a
date range. **Planned**.

**Document** — Any business artefact tracked in ChuA.ERP: a vendor, a
bill, a PO, an SO, a journal entry, etc.

**Drill-down** — Clicking a summary tile or row to see the underlying
detail.

## E

**Environment** — Where this instance of ChuA.ERP runs: DEV / UAT /
PROD. Shown as a badge in the top bar.

**Escalation** — Workflow action triggered when a task exceeds its SLA
without a decision. Typically reminds the assignee then auto-reassigns
to their manager.

**Exchange rate** — The conversion factor between a transaction
currency and the Company's base currency. Set on each JE.

## F

**Fiscal period** — A span of time over which transactions are tracked
(typically a calendar month).

**Fiscal year** — A 12-month accounting cycle; may not align with
calendar year (e.g. April 1 → March 31).

**Forex / FX** — Foreign exchange.

**Full-page overlay** — Loading mode that blocks the entire page during
a long-running operation (used for imports and reports).

## G

**General Ledger (GL)** — The record of every debit and credit posted
across every account. The "source of truth" for financial statements.

**Goods receipt** — The act of recording that goods arrived from a
vendor against a PO.

## H

**Health check** — Lightweight endpoint reporting whether a service is
up. See *System › System Health*.

## I

**Identity provider (IdP)** — The external system that authenticates
users (Microsoft Entra ID, Okta, Google Workspace, etc.).

**Invoice** — A customer-facing demand for payment. Has its own
lifecycle (Draft → Open → Paid).

**Item** — A SKU / product / service tracked in the Inventory module.

## J

**Journal Entry (JE)** — A balanced debit/credit record posted to the
GL. Either drafted then posted (two-step) or created and posted
together.

## K

**KPI** — Key Performance Indicator. The numbers on Dashboard tiles.

## L

**Lead** — A prospect not yet a customer. Planned CRM artefact.

**Ledger** — A book of accounts. The GL is the most comprehensive.

**Locked period** — A fiscal period closed for posting.

**Lot tracking** — Recording inventory by manufacturing batch. **Planned**.

## M

**MFA / Multi-Factor Authentication** — Sign-in requiring more than just
a password.

**Module** — A functional area of the ERP (Purchasing, Sales, Inventory,
Finance, …).

## N

**Normal balance** — Whether an account naturally carries a debit
balance (assets, expenses) or credit balance (liabilities, equity,
revenue).

## O

**OIDC / OpenID Connect** — Standard protocol for SSO authentication.

**On-hand quantity** — The physical inventory present at a warehouse.

**Opportunity** — A qualified lead with revenue probability. Planned
CRM artefact.

**Outstanding balance** — How much is still owed on a bill or invoice.

## P

**Paginator** — The page-number controls at the bottom of a list view.

**Partial payment** — A payment less than the full outstanding balance.

**Partial receipt / shipment** — Receiving / shipping less than the
ordered quantity in one transaction.

**Period close** — The process of finalising all postings for a fiscal
period and locking it.

**Permission** — A fine-grained verb (`BillApprove`, `VendorRead`, …)
granted to a role.

**PO / Purchase Order** — A commitment to buy from a vendor.

**Postable account** — A GL account that accepts direct journal-entry
postings. Header / category accounts are non-postable.

**Posting** — Writing a journal entry to the GL. Irreversible.

**ProblemDetails** — A standard format used to communicate error
responses from the API.

## Q

**Quote** — A pre-SO price proposal. **Planned**.

## R

**Receipt** — The record of goods received from a vendor against a PO.

**Reconciliation** — Comparing the GL balance with an external source
(bank statement, sub-ledger total) and identifying differences.

**Reorder point** — A quantity below which an item should be
re-ordered.

**Request id** — See *Correlation id*.

**Reversal / reversing entry** — A journal entry that undoes another
JE by swapping debits and credits.

**Role** — A named bundle of permissions assigned to users.

## S

**Sales Order (SO)** — A commitment to deliver goods to a customer.

**Section overlay** — Loading mode that covers a specific panel during
a critical write (used for journal-entry posting, period close).

**Segregation of duties** — Control principle that the person who
enters a transaction cannot approve it.

**Sequential approval** — An approval policy where each approver in a
chain must approve before the next is asked.

**Shipment** — The record of goods shipped to a customer against an
SO.

**SKU** — Stock-keeping unit; an item code.

**SLA** — Service Level Agreement. In workflow, the time within which
a task must be decided.

**SSO / Single Sign-On** — Signing in once to your identity provider
to access multiple applications.

**Sub-ledger** — A detailed register supporting a GL control account
(AP sub-ledger backs the AP control account, etc.).

**System Admin** — The highest-privilege role; manages all Companies.

## T

**Task** — A workflow item (approval, reassignment, …).

**Tenant** — A platform installation. One tenant can host multiple
Companies.

**Three-way match** — Cross-check of PO + Receipt + Bill before
approving payment. **Planned**.

**Trial Balance** — A report listing every account with its debit and
credit balance at a point in time. Total debits must equal total
credits.

**Two-eyes principle** — Control requiring two different people for
sensitive actions (preparer + reviewer for JEs).

## U

**UoM / Unit of Measure** — Each, kilograms, litres, boxes, etc.

## V

**Vendor** — A supplier.

**Vendor code** — Short unique identifier for a vendor within a
Company. Immutable after creation.

## W

**Warehouse** — A physical location holding inventory.

**Workflow engine** — The component that creates and routes approval
tasks. See [Workflow Engine](../modules/workflow-engine.md).

**Workflow task** — Same as *Approval task*.

## Acronyms summary

| Acronym | Expansion |
|---|---|
| AP | Accounts Payable |
| AR | Accounts Receivable |
| COA | Chart of Accounts |
| COGS | Cost of Goods Sold |
| CRM | Customer Relationship Management |
| ERP | Enterprise Resource Planning |
| FX | Foreign Exchange |
| GL | General Ledger |
| GRN | Goods Receipt Note |
| IdP | Identity Provider |
| JE | Journal Entry |
| KPI | Key Performance Indicator |
| MFA | Multi-Factor Authentication |
| OIDC | OpenID Connect |
| P&L | Profit and Loss (income statement) |
| PO | Purchase Order |
| PR | Purchase Requisition |
| RBAC | Role-Based Access Control |
| SKU | Stock Keeping Unit |
| SLA | Service Level Agreement |
| SO | Sales Order |
| SOX | Sarbanes-Oxley Act |
| SSO | Single Sign-On |
| TB | Trial Balance |
| UoM | Unit of Measure |
| YTD | Year to Date |
