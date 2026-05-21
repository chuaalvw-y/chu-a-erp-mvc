# 9. Reports & Exports

## Table of Contents
- [How reports are organised](#how-reports-are-organised)
- [Running a report](#running-a-report)
- [Report parameters](#report-parameters)
- [Viewing report output](#viewing-report-output)
- [Exporting](#exporting)
- [Saved reports](#saved-reports)
- [Scheduled reports](#scheduled-reports)
- [Printing](#printing)
- [Permissions](#permissions)
- [Standard reports catalogue](#standard-reports-catalogue)
- [Troubleshooting](#troubleshooting)

## How reports are organised

The **Reports** module is reached from the sidebar. The landing page lists
every report your tenant has registered, with its **Code**, **Name**, and
**Description**. Reports are run from this list — each row has a **Run**
action.

`[SCREENSHOT: Reports index]`

> **Permission required** — `ReportRun` to access this module.

## Running a report

1. *Reports › Reports* in the sidebar.
2. Find the report — scroll, or use the page filter (planned).
3. Click **Run** in the row.
4. The **Run report** page opens, pre-populated with the report's metadata.
5. Provide parameters (see next section) as JSON, or leave the defaults.
6. Click **Run**.
7. The result table renders below the form.

`[SCREENSHOT: Report run page with results table]`

## Report parameters

Today, parameters are supplied as **JSON** in a single text area. Example
for an *Open documents by counterparty* report:

```json
{
  "counterpartyId": "5e4c2bd6-...-7c0e",
  "asOfDate": "2026-05-31",
  "includeClosedPeriods": false
}
```

Each report tells you which parameters it expects (in its **Description**
on the Reports list). Unknown parameters are ignored; missing required
parameters return an error.

> **Availability** — A **guided parameter form** (typed inputs per
> parameter) is **Planned**. The current JSON area gives full flexibility
> for power users; the upcoming form makes parameter entry idiot-proof.

> **Tip** — If you are unsure of the JSON shape, run the report with
> `{}` first; the resulting error message lists the required parameters.

## Viewing report output

Standard reports return a tabular result. Columns depend on the report; a
typical *Open Documents* report includes:

| Column | Meaning |
|---|---|
| Doc type | Bill / Invoice / PO / SO |
| Doc number | Human-readable identifier |
| Doc date | Date on the document |
| Due date | When the document is due (if applicable) |
| Counterparty | Vendor / customer code and name |
| Currency | Document currency |
| Total amount | Total value |
| Outstanding | Amount still owed / receivable |
| Status | Status badge |

Click any document number to drill into the source document (where you have
permission).

## Exporting

> **Availability** — Built-in **Excel / CSV / PDF export buttons** are
> **Planned**.

Until export ships you have three options:

| Workaround | Best for |
|---|---|
| Browser **Print → Save as PDF** | One-off PDF copies |
| Browser select-all → paste into Excel | Quick ad-hoc analysis |
| Use the platform API directly (admin/power-user) | Repeatable extracts to Excel via Power Query, BI tools, or scripts |

The next release adds a toolbar above results with **Export to Excel**,
**Export to CSV**, and **Print PDF** buttons that preserve current
filters and totals.

## Saved reports

> **Availability** — **Planned**.

Once available, you will be able to save a parameter set with a name
("Open AP for Vendor X, as of period 5") and return to it from a *My
Reports* pane on the Reports index.

Today, your workaround is to keep parameter JSONs in a notes app and paste
them into the Run page.

## Scheduled reports

> **Availability** — **Planned**.

Coming features will let admins schedule:
- A nightly batch that emails a Trial Balance PDF to the controller
- A Monday-morning aging report to AP and the CFO
- A weekly "open POs by vendor" to procurement

Today, scheduled reports must be implemented by your platform team via the
API. Talk to IT about extraction jobs.

## Printing

Every list page and detail page is **print-friendly** — using your browser's
*Print* command (Ctrl/Cmd+P) produces a clean printout with the sidebar and
toolbars hidden. This works as a fallback until PDF export is available.

> **Tip** — Choose *Save as PDF* in the print dialog to keep an electronic
> copy.

## Permissions

| Permission | Grants |
|---|---|
| `ReportRun` | Access to the Reports module and the right to run any registered report |

Reports are not separately permissioned today — anyone with `ReportRun` can
run any registered report. Granular per-report permissions are planned for
when sensitive reports (e.g. payroll exports, tax filings) are introduced.

## Standard reports catalogue

The catalogue is tenant-configurable. Typical out-of-the-box reports:

| Code | Name | Description |
|---|---|---|
| `open-documents` | Open Documents | Bills + Invoices outstanding as of a date |
| `vendor-aging` | Vendor Aging | AP aging buckets per vendor (Current, 1-30, 31-60, 61-90, 90+) |
| `customer-aging` | Customer Aging | AR aging buckets per customer |
| `trial-balance` | Trial Balance | Account balances at a period end |
| `gl-detail` | General Ledger Detail | All postings for an account in a date range |
| `inventory-balance` | Inventory Balance | On-hand and available by item and warehouse |
| `po-status` | Purchase Order Status | Open POs with receipt status |
| `so-status` | Sales Order Status | Open SOs with shipment status |

Your tenant's actual report list may differ. Open *Reports* in the sidebar
to see what is registered for your tenant.

## Troubleshooting

| Symptom | Likely cause | Fix |
|---|---|---|
| "Parameters must be valid JSON" | JSON syntax error | Validate the JSON in a tool like jsonlint.com |
| Empty result table | No data matches the parameters | Check date ranges and IDs |
| "An unexpected error occurred." | Server-side report failure | Note the **correlation id** in the footer and report to IT |
| Report missing from the list | Not registered for your tenant | Ask your Company Admin |
| Run button is greyed out | Missing `ReportRun` permission | Talk to Company Admin |

See [Troubleshooting › Reports](../reference/troubleshooting.md#reports) for
additional cases.
