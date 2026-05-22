# 4. Navigation

## Table of Contents
- [Page anatomy](#page-anatomy)
- [Top bar](#top-bar)
- [Sidebar navigation](#sidebar-navigation)
- [Breadcrumbs](#breadcrumbs)
- [Main content area](#main-content-area)
- [Toasts and feedback](#toasts-and-feedback)
- [Footer](#footer)
- [Quick actions](#quick-actions)
- [Keyboard shortcuts](#keyboard-shortcuts)
- [Mobile / responsive layout](#mobile--responsive-layout)
- [Global search](#global-search)
- [Notifications panel](#notifications-panel)

## Page anatomy

```
+--------------------------------------------------------+
| Top bar:  [≡] ChuA ERP  [DEV]                Alice ▼   |
+----------+---------------------------------------------+
|          |  Dashboard › Vendors › Acme Corp            |  ← Breadcrumbs
|          |---------------------------------------------|
|          |                                             |
| Sidebar  |  Main content                               |
|          |                                             |
|  Dash    |                                             |
|  Purch.  |                                             |
|  Sales   |                                             |
|  ...     |                                             |
+----------+---------------------------------------------+
|  © ChuA ERP · request abc-123                          |
+--------------------------------------------------------+
```

`[SCREENSHOT: Annotated full page]`

## Top bar

| Element | Purpose |
|---|---|
| **Hamburger** (≡) | Visible only on small screens — opens the sidebar |
| **Product name** | Clicking it returns to the Dashboard |
| **Environment badge** | Shows whether you are on `LOCAL`, `DEV`, `UAT`, or `PROD`. The badge colour also changes (yellow for non-prod, info for local) so that you cannot mistake your test environment for production |
| **Current user menu** | Your display name. Click to access *My profile*, *Switch company*, *Sign out* |

> **Warning** — Always check the environment badge before performing
> irreversible actions like posting a journal entry. A bright "UAT" badge is
> there to stop you from posting test data into the live ledger.

## Sidebar navigation

The sidebar lists every module you have permission to use. Items you cannot
access are hidden entirely — there is no "greyed out" state, so the menu
contains only choices you can actually make.

| Section | Modules |
|---|---|
| **Dashboard** | Landing page summarising your KPIs |
| **Purchasing** | Vendors · Purchase Orders · Bills |
| **Sales** | Customers · Sales Orders · Invoices |
| **Inventory** | Items · Import items |
| **Finance** | Chart of Accounts · Journal Entries |
| **Workflow** | Approvals |
| **Reports** | Reports |
| **Administration** *(admins only)* | Companies · Users · Roles |
| **System** | System Health |

Each section collapses; the currently active section opens automatically based
on the page you are on. The active link is highlighted.

> **Tip** — If you do not see a module you expect, you may not have the
> required permission. Ask your Company Admin to assign the relevant role.
> See [Security & Permissions](../admin/security-permissions.md) for the full
> permission catalogue.

## Breadcrumbs

Breadcrumbs sit just above the main content and show your path through the
hierarchy:

```
Dashboard › Vendors › Acme Corporation › Edit
```

Each segment except the last is a clickable link back to that point. Use them
to return to a list view from a detail page without losing your filter or
search state.

## Main content area

The main area changes per page but consistent patterns appear throughout:

- **List pages** show a search box, an optional filter row, the data table,
  and a paginator at the bottom.
- **Detail pages** show a header with the record name, action buttons
  (Edit, Delete, plus any document-specific actions like Approve or Pay),
  and a definition list of fields.
- **Form pages** (Create, Edit, action pages like Pay or Ship) show a heading,
  a validation summary (when validation fails), the form fields, and
  Save/Cancel buttons.
- **Confirmation pages** (Delete, Approve, Post) show the record summary plus
  Confirm/Cancel buttons in red-or-green to indicate impact.

## Toasts and feedback

Short-lived notifications appear in the top-right corner. They auto-dismiss
after **5 seconds** but can be closed manually with the × button:

| Colour | Meaning |
|---|---|
| Green | Success — your action completed |
| Yellow | Warning — completed, but please check |
| Red | Error — your action did not complete; see the message |
| Blue | Information — neutral status update |

When you see a red toast, the corresponding error detail also appears in the
top of the form (or in the Error page if the issue was system-wide). Toasts
are transient; if you missed one, check the field-level validation messages
and the page header.

## Footer

The footer always shows the current year, the product name, and a
**request id** (also called *correlation id*) for the page you are on. If you
report a problem to the help desk, copy the request id from the footer and
include it in your ticket — it lets support locate the exact transaction
in the logs.

`[SCREENSHOT: Footer with correlation id highlighted]`

## Quick actions

On every list page the right side of the page header has a primary "New X"
action button (e.g. **New vendor** on the Vendors page, **New journal entry**
on the Journal Entries page). The button is only visible if you have the
relevant Create permission.

Detail pages show secondary actions in a row at the top:

| Module | Detail-page actions |
|---|---|
| Vendors / Customers / Items | Edit · Delete |
| Bills | Edit · Delete · **Approve** · **Pay** |
| Invoices | Edit · Delete · **Apply payment** |
| Purchase Orders | Edit · Delete · **Approve** · **Receive** |
| Sales Orders | Edit · Delete · **Ship** |
| Journal Entries | Edit · Delete · **Post** |
| Workflow tasks | **Submit decision** · **Reassign** |

Buttons that are not allowed by your permissions are hidden. Buttons that
would be invalid in the current document state (e.g. Approve on an
already-approved bill) are also hidden so you cannot fire actions that the
server would reject.

## Keyboard shortcuts

| Shortcut | Action |
|---|---|
| `Tab` / `Shift+Tab` | Move focus through form fields |
| `Enter` | Submit form (when focused inside a single-line field) |
| `Esc` | Close modal or cancel inline action |
| `/` | Focus the main search box (on list pages) |
| `Alt+Home` | Return to Dashboard |
| `Alt+1` … `Alt+9` | Quick-jump to the Nth sidebar item (browser permitting) |

> **Note** — Keyboard shortcuts depend on your browser's modifier conventions
> (macOS substitutes `Cmd`/`Option`). The `/` shortcut works on list pages
> but not inside text fields.

## Mobile / responsive layout

Below 768px viewport width:

- The sidebar is hidden by default; tap the **≡** hamburger icon to reveal it
- Tables become scrollable horizontally rather than collapsing rows
- The action button row on detail pages stacks vertically below the title
- Form fields go to full-width single-column

`[SCREENSHOT: Mobile view of Vendor detail]`

## Global search

> **Availability** — Cross-module global search is **Planned**.

Today, search is **per-module** — each list page has its own search box that
matches against record codes and names within that module. To search across
all modules at once, use the corresponding module's list page.

Once global search ships, the top bar will gain a unified search field that
queries vendors, customers, items, documents, and reports in a single result
panel.

## Notifications panel

> **Availability** — Push notifications panel is **Planned**.

In the current release, "notifications" surface in two places:
- The **Dashboard** tiles for pending approvals (Bills awaiting approval,
  Workflow tasks, Outstanding invoices)
- The **Workflow / Approvals** queue, which is your primary work-list

A bell icon in the top bar with new-item indicators is planned for the next
release. See [Notifications](07-notifications.md) for the current behaviour
and the roadmap.
