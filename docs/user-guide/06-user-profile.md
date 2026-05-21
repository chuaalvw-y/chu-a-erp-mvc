# 6. User Profile & Preferences

## Table of Contents
- [Opening your profile](#opening-your-profile)
- [What the profile shows](#what-the-profile-shows)
- [Changing your password](#changing-your-password)
- [MFA management](#mfa-management)
- [Display preferences](#display-preferences)
- [Active company](#active-company)
- [Why you cannot change every field](#why-you-cannot-change-every-field)

## Opening your profile

1. Click your **name** in the top-right corner of any page.
2. Choose **My profile** from the dropdown.

`[SCREENSHOT: User menu open]`

You land on the **Profile** page.

## What the profile shows

The profile is a read-only summary of your identity within the active company:

| Field | Source | Editable here? |
|---|---|---|
| **User id** | Identity provider (the `sub` claim) | No — set by the IdP |
| **Display name** | IdP preferred user name | No |
| **Email** | IdP claim | No |
| **Active company** | Selected via top-right menu | Yes, via *Switch company* |
| **Roles** | Assigned by Company Admin | No |
| **Permissions** | Derived from roles + direct grants | No — set by your Company Admin |

`[SCREENSHOT: Profile page]`

> **Note** — The roles and permissions on this page combine claims from your
> identity provider and the live response of the **`/users/me`** call. If
> you were granted a new role moments ago, sign out and back in to reload
> the claims.

## Changing your password

| Identity model | How to change password |
|---|---|
| Local accounts | *User menu › My profile › Change password* |
| SSO (Entra ID, Okta, etc.) | Use your IdP's self-service password page; ChuA.ERP cannot change your corporate password |

For local accounts:

1. Open your profile.
2. Click **Change password**.
3. Enter your **current password**.
4. Enter and confirm your **new password** (must satisfy policy).
5. Click **Save**.
6. You remain signed in; the change applies on next sign-in.

> **Warning** — Changing your password invalidates all sessions on other
> devices. If you were signed in on your phone, you will need to sign in
> again there.

## MFA management

> **Availability** — Self-service MFA enrolment **changes** are managed by
> your identity provider when SSO is enabled. For local accounts, MFA reset
> is a Company Admin function.

Talk to IT or your Company Admin to:
- Enrol a new authenticator after losing your phone
- Generate fresh recovery codes
- Disable MFA temporarily (rare; requires manager approval per most security
  policies)

## Display preferences

> **Availability** — User-level preferences are **Planned**.

Until the preferences pane ships, display behaviour is dictated by:

| Behaviour | Source |
|---|---|
| Date format | Browser locale (e.g. en-US shows `5/19/2026`, en-GB shows `19/05/2026`) |
| Number format | Browser locale (`1,234.56` vs `1.234,56`) |
| Currency display | Document's currency code (USD, EUR, GBP, etc. shown explicitly) |
| Time zone | UTC for stored timestamps; displayed in your browser's time zone |
| Theme | Light theme; dark theme is on the roadmap |

The next release adds a *Preferences* tab on the profile page with overrides
for date format, page size on list views, and theme.

## Active company

The active company is shown:

- On the profile page (the Company id field)
- Inferred whenever you create or edit a record (every business document is
  tagged with the active company id, automatically)
- In the audit log of every change you make

To change company, see [Switching companies](03-logging-in.md#switching-companies)
in chapter 3 or the deeper [Multi-Tenant guide](../admin/multi-tenant.md).

> **Warning** — Always confirm the active company before posting journal
> entries, approving bills, or running reports. Posting to the wrong company
> is one of the most common operator errors and can be costly to unwind.

## Why you cannot change every field

ChuA.ERP follows the **principle of least privilege**: the platform
acknowledges users but does not grant them the right to redefine themselves.

| Action | Required role |
|---|---|
| Change your own roles | Company Admin |
| Change your own email | Company Admin (or IdP, for SSO tenants) |
| Be added to a new company | System Admin |
| Be removed from a company | Company Admin |
| Deactivate your own account | Company Admin |
| Update display name | Identity provider |

If you need a change that you cannot make yourself, contact your manager,
your Company Admin, or your IT help desk. Every change to your account is
recorded in the audit log (see [Audit History](12-audit-history.md)) so you
can later see who changed what and when.
