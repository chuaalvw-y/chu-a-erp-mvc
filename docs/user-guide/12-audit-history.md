# 12. Audit History

## Table of Contents
- [What is recorded](#what-is-recorded)
- [Where to find audit info](#where-to-find-audit-info)
- [Document-level history](#document-level-history)
- [Workflow history](#workflow-history)
- [User & role change history](#user--role-change-history)
- [Correlation ids](#correlation-ids)
- [Retention](#retention)
- [Exporting audit data](#exporting-audit-data)
- [Worked example — investigating a discrepancy](#worked-example--investigating-a-discrepancy)
- [Audit best practices](#audit-best-practices)

## What is recorded

ChuA.ERP records, for every change to a business object:

| Recorded item | Description |
|---|---|
| **Who** | The acting user — `sub` claim from the IdP, plus display name |
| **When** | UTC timestamp, second-precision |
| **What** | The object id (document, user, role) and the field-level diff (before → after) |
| **Why** | Any comment / reason / memo the user typed at the time |
| **From where** | The IP address of the request (admin-visible) |
| **Correlation id** | An end-to-end trace identifier joining the UI action to backend log records |

These records are **immutable** — no user, including the System Admin, can
delete or modify them once written.

> **Note** — Audit retention duration is tenant-configurable but defaults
> to **7 years** to satisfy common financial-records regulations
> (SOX, IFRS, GAAP, GDPR for relevant data).

## Where to find audit info

| You want to see... | Look at... |
|---|---|
| Who approved a specific bill | The **Approval history** block on the bill's detail page |
| Who edited a journal entry before posting | Document history (planned in-UI; today via the API or admin audit dump) |
| When a user's role changed | *Administration › Users › [user] › History* (admin view) |
| All actions a user took on a specific day | Audit log export (admin) |
| When inventory was adjusted and by whom | Item detail page › Adjustment log (planned) |

## Document-level history

> **Availability** — A full **document history pane** is **Planned** on
> every detail page. The information is captured today; surfacing it in
> the UI ships in the next release.

For documents that go through workflow (bills, POs, journal entries), the
workflow history is already visible at the bottom of the document detail.
That covers approve / reject / reassign decisions.

For non-workflow edits (e.g. changing a vendor's payment terms), the
history is recorded but currently only retrievable by your Company Admin
via the audit log export. Once the document history pane ships, every
detail page will show:

```
v History
+------------------------------------------------------------+
| 2026-05-19 14:22  Alice  Updated Payment terms 30 → 45    |
+------------------------------------------------------------+
| 2026-05-18 09:01  Bob    Updated Legal name              |
|                          "Acme" → "Acme Corp Ltd."        |
+------------------------------------------------------------+
| 2026-04-30 16:55  Carol  Created                          |
+------------------------------------------------------------+
```

## Workflow history

Approval tasks have a permanent history block that survives the underlying
document's life-cycle. Specifically:

- Approve / Reject decisions with comments
- Reassignments with reasons
- Escalation events
- Auto-cancellation events (e.g. when the source document is deleted while
  the task is still Pending)

See [Workflow & Approvals › Approval history](08-workflow-approvals.md#approval-history)
for a worked example.

## User & role change history

Administrators can see the history of:

- A user's role membership (when added / removed)
- A role's permission set (when permissions were added / removed)
- Account-status changes (enabled, disabled, locked, MFA-reset)

> **Permission required** — `CompanyAdmin` role or `SystemAdmin` role.

Access via *Administration › Users › [user]* (current activity) or via the
audit-log export described below.

## Correlation ids

Every web request gets a **correlation id** — a unique string that travels
through every backend service involved in handling the request. The id is
visible in three places:

1. The **footer** of every page in the UI: *"request abc-123-def"*
2. The HTTP header `X-Correlation-ID` on requests and responses (admin /
   developer tools view)
3. Every entry the request produces in the audit and system logs

`[SCREENSHOT: Footer with correlation id highlighted]`

When you report a problem to support, **copy the correlation id from the
footer** and include it in your ticket. Support uses it to locate every
log entry related to your specific action.

## Retention

| Audit category | Default retention |
|---|---|
| Financial document audits (bills, invoices, JEs, payments) | 7 years |
| Master data audits (vendors, customers, accounts) | 7 years |
| Workflow audits (approvals, reassignments) | 7 years |
| User account audits (sign-ins, role changes) | 2 years |
| Health & operational logs | 90 days |

Tenant administrators can extend retention in *Administration › System
Settings* but **cannot shorten it below the regulatory minimum** without
explicit platform-level approval.

## Exporting audit data

> **Availability** — A self-service **Audit export** pane in
> *Administration › Audit Log* is **Planned**. Today, exports are
> performed by the platform team on request.

When the export pane ships, Company Admins will be able to:
- Filter audit records by user, by object, by date range, and by action
- Export to CSV (smallest), Excel (formatted), or JSONL (streaming, for
  ingestion into SIEMs)
- Schedule recurring exports to a tenant-owned destination (S3, Azure
  Blob, etc.) for long-term archiving

Until then, your platform team can extract audit data via the audit API.
Talk to IT.

## Worked example — investigating a discrepancy

> **Scenario** — Trial balance is off by $1,250.00. You suspect someone
> posted a journal entry without proper approval.

1. Run the **Trial Balance** report as of the period start and again as of
   "now" to find the account where the variance shows up.
2. Open *Finance › Journal Entries*. Filter by **Fiscal period = current
   period** and **Status = Posted**. Sort by **Posted at** (planned) or
   scan recent entries.
3. Open candidate entries. The workflow history block shows who submitted
   and who approved.
4. If an entry was posted without approval — check the **audit log** via
   admin export for the entry's id; the entry should not have skipped the
   workflow rule unless an admin overrode it (which is itself audited).
5. If you find a problematic posting, **create a reversing journal entry**
   (do **not** edit the original posted entry; posted entries are
   immutable).
6. Document your findings in the reversing entry's Memo field and link to
   the audit trail in your control-review record.

This combination of:
- Immutable posted entries
- Workflow history on every approved transaction
- Audit log with correlation ids
- Reversal-only correction model

is what makes ChuA.ERP suitable for SOX-regulated environments.

## Audit best practices

- **Treat the audit log as a control document.** Auditors will ask for
  evidence of internal controls; the audit log is your primary artefact.
- **Never share account credentials.** If two people sign in with the
  same account, the audit log cannot distinguish them.
- **Use comments on approvals.** Audit value depends on the why, not just
  the what.
- **Reconcile periodically.** Run the Open Documents report monthly and
  cross-check against the trial balance.
- **Keep external attachments named consistently.** Until in-product
  attachments ship, your file-naming convention is part of the audit
  trail.
- **Test your access.** Once a quarter, ask your Company Admin to confirm
  your role membership matches your job. Stale permissions are an audit
  finding waiting to happen.
