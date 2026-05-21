# System Settings

> **Availability** — A dedicated **System Settings** admin screen is
> **Planned**. Today, settings are managed by the platform team via tenant
> configuration files. This guide documents the **logical settings model**
> so admins know what they can ask the platform team to adjust.

## Table of Contents
- [Categories of settings](#categories-of-settings)
- [Tenant identity](#tenant-identity)
- [API & integration settings](#api--integration-settings)
- [Authentication settings](#authentication-settings)
- [Session settings](#session-settings)
- [Approval & workflow settings](#approval--workflow-settings)
- [Notification settings](#notification-settings)
- [Audit & retention settings](#audit--retention-settings)
- [Currency settings](#currency-settings)
- [Fiscal-period settings](#fiscal-period-settings)
- [Branding settings](#branding-settings)
- [Feature flags](#feature-flags)
- [Change-management process](#change-management-process)

## Categories of settings

| Category | Examples |
|---|---|
| Tenant identity | Tenant name, company short code, time zone |
| Authentication | OIDC enabled, identity provider URL, MFA required |
| Session | Cookie expiry, sliding timeout |
| Approval / workflow | Default thresholds, escalation SLA |
| Notification | Per-event email rules, digest cadence |
| Audit / retention | Retention period (years) |
| Currency | Base currency, allowed currencies, FX rate source |
| Fiscal period | Calendar (4-4-5, monthly, 13-period, custom) |
| Branding | Logo, colour palette, product name override |
| Feature flags | Which preview features are on / off |

## Tenant identity

| Setting | Example | Notes |
|---|---|---|
| Tenant name | "Acme Corporation" | Shown in the top bar |
| Tenant short code | `ACME` | Used in PR / audit references |
| Time zone | `America/New_York` | Server-side conversions; UI shows local time |
| Locale | `en-US` | Date / number formatting default |

## API & integration settings

These settings live in the MVC application's configuration (`appsettings.json` /
environment variables). Your platform team owns them.

| Setting | Purpose |
|---|---|
| `Api:BaseUrl` | Root URL of the API the MVC talks to |
| `Api:Version` | Path segment for API versioning (`v1`) |
| `Api:TimeoutSeconds` | Per-request HTTP timeout |
| `Api:RetryCount` | Number of automatic retries for transient failures |
| `Api:UseAuthBypass` | Development only — **must be false in production** |

## Authentication settings

| Setting | Purpose |
|---|---|
| `Oidc:Enabled` | Turn OIDC sign-in on / off |
| `Oidc:Authority` | Identity provider URL |
| `Oidc:ClientId` | Application's IdP client id |
| `Oidc:ClientSecret` | Stored in user-secrets / Key Vault — never in source control |
| `Oidc:CallbackPath` | OIDC redirect URI (default `/signin-oidc`) |
| `Oidc:Scopes` | `openid profile email chua-erp-api` |
| `Oidc:RequireHttpsMetadata` | True in production |
| MFA required | Tenant policy — enforce MFA on every user |

> **Warning** — Switching `Oidc:Enabled` from false to true while users
> are signed in invalidates all sessions. Plan a maintenance window.

## Session settings

| Setting | Default | Effect |
|---|---|---|
| Cookie name | `ChuA.ERP.Auth` | The session cookie name |
| Cookie SameSite | `Lax` | Required for OIDC redirect flow |
| Cookie Secure | `Always` in production | HTTPS only |
| Cookie expiry | 8 hours | Absolute session length |
| Sliding expiration | Enabled | Activity resets the clock |

## Approval & workflow settings

Configurable per subject type (Bill, PurchaseOrder, JournalEntry, …):

| Setting | Example |
|---|---|
| Threshold ladders | Amount bands → required approver level |
| Sequential vs parallel | Mode for each ladder rung |
| Reminder SLA | 3 business days |
| Escalation SLA | 5 business days |
| Escalation target | Direct manager / specific role |
| Self-approval policy | Allowed / Forbidden |

## Notification settings

> **Availability** — Per-user override is **Planned**. Tenant-level
> configuration today.

| Event | Default channel | Cadence |
|---|---|---|
| New approval task assigned | Email | Immediate |
| Approval task reassigned | Email | Immediate |
| Escalation reminder | Email | 3 days after creation |
| Auto-reassignment | Email (both parties) | 5 days after creation |
| Daily digest | Email | 08:00 local time |
| Period closing soon | Email to Finance | 3 days before period end |

## Audit & retention settings

| Category | Default retention |
|---|---|
| Financial transaction audit | 7 years |
| Master data audit | 7 years |
| Workflow audit | 7 years |
| Authentication / session audit | 2 years |
| System / operational logs | 90 days |

> **Note** — Retention extensions are routine; reductions require
> explicit platform-level approval to ensure regulatory compliance.

## Currency settings

| Setting | Purpose |
|---|---|
| Base currency | Per Company — the GL reporting currency |
| Allowed transaction currencies | Whitelist (e.g. USD, EUR, GBP) |
| FX rate source | Manual upload · Daily feed · External provider |
| FX rate scope | Daily · Monthly · Per-transaction |

## Fiscal-period settings

| Setting | Purpose |
|---|---|
| Period calendar | Monthly · 4-4-5 · 13 periods · Custom |
| Fiscal year start | Month/day (e.g. April 1) |
| Period open/close policy | Manual or auto on month-end |
| Re-open policy | Allowed / Forbidden / Requires CFO |

## Branding settings

> **Availability** — Per-tenant **theme override** is **Planned**.

Customisable when shipped:
- Logo image
- Brand colour (primary + accent)
- Product name override (e.g. show "Acme Finance Hub" instead of "ChuA ERP")
- Sign-in page background

## Feature flags

Preview features can be enabled per tenant. Common flags:

| Flag | Effect |
|---|---|
| `CRM.Enabled` | Show CRM module in the sidebar (when shipped) |
| `Inventory.LotTracking` | Enable lot tracking UI on items |
| `Workflow.RuleEditor` | Visual rule editor |
| `Attachments.Enabled` | In-product file attachments |
| `BankReconciliation` | Bank rec screens |

## Change-management process

For settings the platform team manages on your behalf:

1. **Request** — Open a ticket with your account team or platform team.
2. **Review** — Their team confirms business impact (e.g. retention
   reductions need compliance sign-off).
3. **Schedule** — Most settings change without downtime; auth and session
   settings may require a coordinated rollout.
4. **Test in UAT** — Material changes are applied to your UAT tenant first.
5. **Apply to production** — After your sign-off.
6. **Audit** — Settings changes are logged in the platform's
   configuration audit.

> **Tip** — Keep a **settings register** alongside your tenant: a
> spreadsheet listing every non-default setting, the date it was set, and
> who approved it. Auditors will ask.
