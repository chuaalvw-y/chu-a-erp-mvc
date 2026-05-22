# 10. Search & Filtering

## Table of Contents
- [Per-module search](#per-module-search)
- [Filter rows](#filter-rows)
- [Sorting](#sorting)
- [Pagination](#pagination)
- [Search operators (planned)](#search-operators-planned)
- [Saved filters (planned)](#saved-filters-planned)
- [Global search (planned)](#global-search-planned)
- [Examples](#examples)

## Per-module search

Every list page has a **search box** above the table:

`[SCREENSHOT: Search box on Vendors list]`

The search is a **case-insensitive substring** match against the module's
key fields:

| Module | Searched fields |
|---|---|
| Vendors | Vendor code, Legal name |
| Customers | Customer code, Legal name |
| Inventory | SKU, Name |
| Chart of Accounts | Account code, Name |
| Bills | Bill number |
| Invoices | Invoice number |
| Purchase Orders | Order number |
| Sales Orders | Order number |
| Users | User name, Email |
| Roles | Role name |
| Workflow | Subject id (Guid) — exact match |

Type a term, press **Enter** or click **Search**. The list reloads showing
matches.

The page address bar reflects the search:

```
/Vendors?search=acme
```

You can bookmark or share that URL; reopening it re-runs the same search.

> **Tip** — Searching for `acme` also matches `Acme Corp`, `acme-logistics`,
> and `subacme`. Most modules use contains-match, not exact match.

## Filter rows

Some modules have a **filter row** above the table for typed filters that
substring-search can't express. The standard filters per module:

| Module | Filters |
|---|---|
| Bills | Vendor · Status · Payment status |
| Invoices | Customer · Status · Payment status |
| Purchase Orders | Vendor · Status |
| Sales Orders | Customer · Status |
| Journal Entries | Fiscal period · Status |
| Chart of Accounts | Account type · Search |
| Workflow tasks | Status · Subject type |

`[SCREENSHOT: Bills filter row with status, vendor, payment status]`

Filters compose with the search box. Submit the filter form (Enter on any
field) and the URL reflects all selected filters:

```
/Bills?vendorId=5e4c…&status=PendingApproval&search=B-2026
```

## Sorting

> **Availability** — Click-to-sort column headers are **Planned**.

Today, list pages return rows in the server's natural order — typically
**most recent first** for transactional documents and **alphabetical** for
master data.

Once the planned sorting ships, every column header will be clickable; one
click sorts ascending, two clicks descending. Sort selection appears in the
URL (`sort=billDate desc`) so it can be bookmarked.

For the moment, if you need a specific sort:
- Re-run the same data as a [Report](09-reports-exports.md) (reports support
  ordering)
- Or copy the page to Excel and sort there

## Pagination

The bottom of every list page has a **paginator**:

```
                            ‹ Previous   1   [2]   3   4   5   Next ›
```

| Element | Meaning |
|---|---|
| `‹ Previous` / `Next ›` | Move one page in either direction; disabled at the ends |
| Numbered links | Jump to a page directly |
| Active page | Highlighted (square bracket pattern in this manual) |

Page size is **25 rows** by default. The URL carries page state:

```
/Vendors?pageNumber=3&pageSize=25&search=acme
```

> **Availability** — A **per-page-size selector** (10 / 25 / 50 / 100) is
> **Planned**.

> **Tip** — If you need to see "all" results, run a [Report](09-reports-exports.md);
> the list view is intentionally paged to keep the UI fast.

## Search operators (planned)

> **Availability** — **Planned**.

Roadmap operators (modeled after enterprise search syntaxes used by Jira,
NetSuite, Workday):

| Operator | Meaning | Example |
|---|---|---|
| `"exact phrase"` | Match an exact phrase, not a substring | `"acme inc"` |
| `field:value` | Match a specific field | `vendorCode:V-100` |
| `field:>value` | Comparison (numbers, dates) | `totalAmount:>10000` |
| `-term` | Exclude term | `acme -test` |
| `OR` | Either of two terms | `acme OR beta` |

The current single-box search is **always** a substring contains. The
operators above will be a superset, not a replacement.

## Saved filters (planned)

> **Availability** — **Planned**.

Coming soon: a *Save view* button on every list page that stores your
current filter + sort selection under a name. Saved views become items in
the sidebar under the module they belong to.

Until then, **bookmark the URL** of a filtered list. Browser bookmarks are
the working equivalent of saved filters.

## Global search (planned)

> **Availability** — **Planned**.

The top bar will gain a global search input that queries every module at
once and groups results by entity type (Vendors, Customers, Documents,
Reports). Until then use the per-module list pages.

## Examples

### Find an unpaid bill by approximate vendor name

1. *Purchasing › Bills*
2. In the search box type part of the vendor name, e.g. `delta`.
3. In the filter row choose **Payment status = Outstanding**.
4. Press **Enter**.
5. The list shows bills from any vendor containing "delta" with outstanding
   balance.

### Find all draft journal entries in a specific period

1. *Finance › Journal Entries*
2. In the filter row choose **Fiscal period = May 2026** and **Status = Draft**.
3. Press **Enter**.

### Find a customer by partial code

1. *Sales › Customers*
2. Type the partial code in the search box, e.g. `C-200`.
3. Press **Enter**.
4. The list shows customers whose code or name contains the term.

### Share a filtered view

1. Filter the list as desired.
2. Copy the **browser URL** from the address bar.
3. Paste into Slack, Teams, or email.
4. Anyone with the same permission who opens the link sees the same view.
