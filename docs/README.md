# ChuA.ERP — Documentation

Welcome to the **ChuA.ERP** documentation library. This site is written for the
people who use the system every day — accounting staff, purchasing buyers,
inventory clerks, sales representatives, finance analysts, approvers, and
system administrators. It is **not** a developer manual.

The documentation is structured so it can be:
- Exported to PDF for offline training and SOP binders
- Published to Confluence, SharePoint, or any internal wiki
- Indexed by a help-desk knowledge base
- Used for onboarding new hires
- Referenced ad-hoc by experienced users

## How this library is organised

| Library section | Audience | When to read it |
|---|---|---|
| **[Quick Start Guide](reference/quick-start.md)** | Brand new users | Your first 30 minutes in ChuA.ERP |
| **[User Guide](user-guide/)** | Everyone | Cross-cutting features: login, navigation, dashboards, approvals, reports, search, etc. |
| **[Module Guides](modules/)** | Department staff | Deep-dive guides for Purchasing, Sales, Inventory, Finance, CRM, and the Workflow Engine |
| **[Administrator Guide](admin/)** | IT, security, system admins | User and role management, multi-tenant, system settings, audit log |
| **[Role-Based Guides](reference/role-based-guides.md)** | Each functional role | "I'm an AP clerk — what do I need to know?" condensed reference for each role |
| **[Troubleshooting](reference/troubleshooting.md)** | Help-desk & end users | Symptom → cause → fix tables for common problems |
| **[FAQ](reference/faq.md)** | Everyone | Quick answers to the most frequently asked questions |
| **[Glossary](reference/glossary.md)** | Everyone | ERP and accounting terminology |

## Site map

### User Guide
1. [Introduction](user-guide/01-introduction.md)
2. [Getting Started](user-guide/02-getting-started.md)
3. [Logging In](user-guide/03-logging-in.md)
4. [Navigation](user-guide/04-navigation.md)
5. [Dashboard](user-guide/05-dashboard.md)
6. [User Profile & Preferences](user-guide/06-user-profile.md)
7. [Notifications](user-guide/07-notifications.md)
8. [Workflow & Approvals](user-guide/08-workflow-approvals.md)
9. [Reports & Exports](user-guide/09-reports-exports.md)
10. [Search & Filtering](user-guide/10-search-filtering.md)
11. [Attachments & Documents](user-guide/11-attachments.md)
12. [Audit History](user-guide/12-audit-history.md)

### Module Guides
- [Purchasing](modules/purchasing.md)
- [Sales](modules/sales.md)
- [Inventory](modules/inventory.md)
- [Finance](modules/finance.md)
- [CRM](modules/crm.md)
- [Workflow Engine](modules/workflow-engine.md)

### Administrator Guide
- [Administration overview](admin/administration.md)
- [System Settings](admin/system-settings.md)
- [Multi-Tenant / Company Switching](admin/multi-tenant.md)
- [Security & Permissions](admin/security-permissions.md)

### Reference
- [Quick-Start Guide](reference/quick-start.md)
- [Role-Based Guides](reference/role-based-guides.md)
- [Troubleshooting](reference/troubleshooting.md)
- [FAQ](reference/faq.md)
- [Glossary](reference/glossary.md)

## Document conventions

Throughout the library we use these markers:

> **Note** — A clarification or piece of context the reader may find useful.

> **Tip** — A best-practice shortcut or recommendation.

> **Warning** — An irreversible action or a step that, if skipped, will cause problems downstream.

> **Permission required** — Lists which authorisation policies a user needs to perform the action.

`[SCREENSHOT: Description]` placeholders mark where screenshots should be
inserted by your documentation team when localising or republishing.

Code, button names and field names appear `like this`.
**Menu paths** look like *Purchasing › Purchase Orders › New*.

## Release & version note

This documentation corresponds to **ChuA.ERP — Phase 5** (the ASP.NET Core MVC
user interface running against the v1 API). Some features described here are
forward-looking — they are part of the enterprise roadmap and may not yet be
generally available in your tenant. Look for the **Availability** badge at the
top of each module guide:

- **Available** — shipped in the current release
- **Limited release** — shipped to selected tenants
- **Planned** — described for context; not yet released

## Localisation & accessibility

The user interface ships with English (United States) as the default language.
Date formats, number formats and currency display follow your tenant's locale
configuration (see *Administrator Guide › System Settings*).

ChuA.ERP follows WCAG 2.1 AA accessibility guidance: every interactive control
has a keyboard equivalent, ARIA labels are present on dynamic regions, and the
contrast ratio of the default theme meets AA. Users who prefer reduced motion
have animations slowed but not removed, so progress indicators remain visible.
