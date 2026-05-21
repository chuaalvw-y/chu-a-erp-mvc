# Administrator Guide — Overview

## Table of Contents
- [Who is an administrator?](#who-is-an-administrator)
- [Administrative areas](#administrative-areas)
- [User management](#user-management)
- [Role management](#role-management)
- [Permission management](#permission-management)
- [Tenant / company management](#tenant--company-management)
- [Notification settings](#notification-settings)
- [Audit logs](#audit-logs)
- [Day-to-day admin checklist](#day-to-day-admin-checklist)
- [Onboarding a new user](#onboarding-a-new-user)
- [Offboarding a leaver](#offboarding-a-leaver)

## Who is an administrator?

ChuA.ERP recognises two organisation-wide admin roles:

| Role | Scope | Granted by |
|---|---|---|
| **System Admin** | Cross-tenant — can create companies, manage every user, configure platform-wide settings | Platform team (very small group) |
| **Company Admin** | A single company — manages users, roles, and permissions within that company | A System Admin |

Most administrative actions in this guide are open to **either** role. Where a
restriction applies (e.g. only System Admins can create new companies) the
section calls it out.

> **Permission required** — Items in this guide assume you hold the
> `SystemAdmin` or `CompanyAdmin` role on the active company. If you do
> not, those menus are hidden.

## Administrative areas

The sidebar **Administration** section exposes these areas:

| Area | Sidebar entry | Purpose |
|---|---|---|
| Companies | *Administration › Companies* | System Admin only — tenant/company onboarding |
| Users | *Administration › Users* | User lifecycle inside the active company |
| Roles | *Administration › Roles* | Role catalogue |
| System Health | *System › System Health* | API status snapshot |

Additional admin facets are not yet UI-driven:
- **System Settings** — see [System Settings](system-settings.md). Today
  configured by the platform team via configuration files; UI is **Planned**.
- **Audit log export** — admin pane is **Planned**; today via API.
- **Notification matrix** — **Planned**; today via tenant config.

## User management

The Users page lists every user with access to the active company.

`[SCREENSHOT: Administration › Users list]`

| Action | Permission | Effect |
|---|---|---|
| View list | `CompanyAdmin` | Shows users in active company |
| Open details | `CompanyAdmin` | User profile + role assignment |
| Create user | `CompanyAdmin` | Adds a new user; emails invitation (planned) |
| Edit user | `CompanyAdmin` | Update email, name, employee linkage, 2FA flag |
| Delete user | `CompanyAdmin` | Removes user from the company; SystemAdmin can hard-delete from the platform |

> **Warning** — Deleting a user is a *removal from the company*, not a
> deletion of the audit history. All transactions and decisions made by
> that user remain attributed to them in the log forever.

### Creating a user

1. *Administration › Users › New user*.
2. Enter user name (typically the email), email, first name, last name.
3. Optionally link to an Employee record.
4. Save.
5. (If SSO) The user signs in via your identity provider; no password is
   needed.
6. (If local accounts) An invitation email is sent (planned) with a
   set-password link. Today, communicate the temporary password
   out-of-band.
7. **Assign roles** (see below).

### Editing a user

Open the user → **Edit**. Update email, names, employee linkage, 2FA flag.
Changes are audited.

### Deactivating a user

> **Availability** — A clean **Active / Inactive** toggle is **Planned**.

In the current release, removing a user means deleting their company
membership via the Delete action. To temporarily suspend access without
data loss, either:
- Have IT disable their identity-provider account (SSO tenants)
- Remove all their roles (local-account tenants) — they sign in but see
  no menus

## Role management

The Roles page lists the role catalogue.

`[SCREENSHOT: Administration › Roles list]`

| Action | Permission | Effect |
|---|---|---|
| View | `CompanyAdmin` | List roles |
| Create | `CompanyAdmin` | Add a tenant-specific role |
| Edit | `CompanyAdmin` | Rename or re-describe |
| Delete | `CompanyAdmin` | Refused if any user is assigned to the role |

System roles (those with `IsSystemRole = true`) cannot be edited or deleted —
they ship with the platform.

### Creating a role

Roles are **named collections of permissions**. The current release
provides system roles (`SystemAdmin`, `CompanyAdmin`) plus the platform's
permission catalogue. Customer-defined roles let you group permissions
to match your job titles:

1. *Administration › Roles › New role*.
2. Enter Name (e.g. "AP Clerk") and Description.
3. Save.
4. (Once available) Attach the role's permission set in the role's detail
   page — today permissions are mapped to roles at the identity-provider
   level for SSO tenants, or in the user's permission claim list directly.

> **Availability** — A **role permission editor** with click-to-toggle
> permission membership is **Planned**.

### Assigning a role to a user

Today, role membership is communicated in the user's identity-provider
claims (SSO) or in the user's permission claim array (local). A UI for
direct in-app role assignment is **Planned**.

For SSO tenants, configure role mapping in your identity provider:
- **Microsoft Entra ID**: app role assignment under the application
- **Okta**: group-to-role mapping in the SAML / OIDC config
- **Google Workspace**: groups → SAML assertions

## Permission management

Permissions are the **fine-grained verbs** (`VendorRead`, `BillApprove`, …).
They are platform-defined; customers cannot add new permissions but can
combine them into roles.

The catalogue is documented in [Security & Permissions](security-permissions.md).

## Tenant / company management

> **Permission required** — `SystemAdmin`.

A **Company** in ChuA.ERP is a separate legal entity / accounting unit.
Multi-company tenants run several Companies under a single platform
installation. Users may be granted access to one or many Companies.

The Companies page lets a System Admin:
- Create a new Company (Code, Name, Address, Tax id, Base currency)
- Edit a Company's metadata
- Delete a Company (only if it has zero transactions)

See [Multi-Tenant / Company Switching](multi-tenant.md) for the user-facing
side of multi-company access.

## Notification settings

> **Availability** — In-product notification settings UI is **Planned**.

Today, the tenant's notification matrix (which events trigger email, with
what cadence, to which group) is configured by the platform team. To
adjust, request a change via your platform account.

Typical configurations:
- New-task email per-event (default)
- Daily digest at 8 AM
- Weekly summary on Mondays
- Escalation reminders to direct manager

## Audit logs

> **Availability** — Audit log query UI is **Planned**. Today, audit data
> is exposed via the platform's audit API to System Admins.

The audit log captures every state-changing action in the system; see
[Audit History (user guide)](../user-guide/12-audit-history.md) for the
data model and retention rules.

## Day-to-day admin checklist

| Cadence | Task |
|---|---|
| Daily | Triage Workflow › Approvals (your own queue) |
| Daily | Check System Health page |
| Weekly | Review new-user onboarding requests |
| Weekly | Review locked accounts |
| Monthly | Review role membership for departing employees |
| Quarterly | Permission audit — does every user still need every role? |
| Annually | Tenant configuration review with the platform team |

## Onboarding a new user

```mermaid
flowchart LR
    A[New hire confirmed] --> B[IT creates corporate account]
    B --> C[Manager requests ERP access]
    C --> D[Admin creates User in ChuA.ERP]
    D --> E[Admin assigns Role(s)]
    E --> F[User signs in]
    F --> G[Admin verifies access on a test action]
```

1. Receive an access request from the user's manager (preferably via your
   ticketing system).
2. Confirm what role(s) the user needs based on their job description.
3. *Administration › Users › New user*.
4. Assign role(s).
5. Send the user a welcome email pointing to the tenant URL and the
   [Quick-Start Guide](../reference/quick-start.md).
6. Once they confirm sign-in works, mark the ticket complete.

## Offboarding a leaver

When an employee leaves:

1. **Immediately** — IT disables their corporate identity (SSO tenants).
   For local-account tenants, change their password to a random value and
   sign out all their sessions.
2. *Administration › Users › [user] › Delete* (removes company membership).
3. Verify no active workflow tasks are assigned to them; reassign any
   pending tasks to their replacement or manager.
4. Verify they are not the sole approver for any approval rule; update
   rules if needed.
5. Record the offboarding in your ticket; the audit log preserves all
   their historical actions.

> **Warning** — Do not delete the user *before* reassigning their tasks.
> A deleted user with pending tasks leaves the workflow in a broken
> state. Reassign first, then delete.
