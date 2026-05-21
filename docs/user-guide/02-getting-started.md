# 2. Getting Started

## Table of Contents
- [Browser requirements](#browser-requirements)
- [Supported devices](#supported-devices)
- [Receiving your account](#receiving-your-account)
- [First sign-in](#first-sign-in)
- [Password policy](#password-policy)
- [Multi-factor authentication (MFA)](#multi-factor-authentication-mfa)
- [Session timeout and idle expiry](#session-timeout-and-idle-expiry)
- [Account lockout](#account-lockout)
- [Security reminders](#security-reminders)
- [Where to go next](#where-to-go-next)

## Browser requirements

ChuA.ERP is a web application. Supported browsers (latest two stable versions):

| Browser | Recommended | Notes |
|---|---|---|
| Microsoft Edge | ✓ | Best on corporate Windows workstations |
| Google Chrome | ✓ | Use the Chrome menu to enable popups for the ERP origin if reports open in new tabs |
| Mozilla Firefox | ✓ | Disable strict cookie isolation for the ERP origin if OIDC sign-in fails |
| Apple Safari | ✓ | macOS 13 or later |

The application requires JavaScript and cookies to be enabled. It does **not**
support Internet Explorer. Mobile browsers will render the UI in a responsive
layout but are intended for review actions (approvals, dashboards); heavy data
entry should be done on a desktop.

> **Tip** — If your organisation enforces a corporate browser policy, ask IT
> to allow the ERP origin (for example `erp.your-company.com`) for cookies,
> popups (used by reports), and the SSO redirect chain.

## Supported devices

| Device | Best for |
|---|---|
| Desktop / laptop (≥ 1366 × 768) | Data entry, multi-line documents, reports |
| Tablet (≥ 768px wide) | Approvals, dashboards, status lookups |
| Phone (< 768px) | Approvals, notifications, single-record look-ups |

On screens narrower than 768px the sidebar navigation collapses behind a
hamburger menu icon; tap it to reveal the menu.

## Receiving your account

Accounts are created by your System Administrator or Company Administrator. You
will typically receive:

1. An **invitation email** from your IT or HR team containing the URL of your
   tenant's ChuA.ERP instance.
2. A **sign-in identifier** — usually your work email address.
3. Instructions for setting your initial password, OR a notice that you can
   sign in directly with your existing corporate credentials via SSO.

> **Note** — If your organisation uses single sign-on (Microsoft Entra ID,
> Okta, Google Workspace, etc.), ChuA.ERP will not have a separate password
> for you. Sign-in flows through your identity provider and inherits whatever
> MFA your IT department has configured there.

## First sign-in

1. Open your browser and navigate to the URL provided by IT.
2. You will be redirected to the sign-in page.
   `[SCREENSHOT: Sign-in page]`
3. **If your tenant uses SSO** — click **Sign in** to be redirected to your
   organisation's identity provider, then follow your normal corporate sign-in
   (which may include MFA).
4. **If your tenant uses local accounts** — enter your user name and the
   temporary password you received. You will be prompted to set a new password
   on first sign-in.
5. After successful sign-in you land on the **Dashboard**.
   `[SCREENSHOT: Dashboard immediately after first sign-in]`

## Password policy

When local accounts are in use, passwords must:

- Be at least 12 characters long
- Include a mix of upper-case letters, lower-case letters, digits, and at
  least one symbol
- Not repeat any of your previous five passwords
- Not contain your user name, full name, or email local-part

If your tenant uses SSO, password policy is dictated by your identity provider
and ChuA.ERP simply trusts the identity it receives. Talk to IT about password
requirements.

## Multi-factor authentication (MFA)

MFA is provided by the identity provider when SSO is enabled. Typical methods:

| Method | Description |
|---|---|
| Authenticator app | A six-digit code from Microsoft Authenticator, Google Authenticator, Duo, or similar |
| SMS / phone | One-time code sent to your mobile number |
| Hardware token | FIDO2 / WebAuthn security key |
| Push notification | Approval prompt sent to your phone |

For tenants with local accounts, ChuA.ERP can flag a user account as
"requires MFA"; the next sign-in then prompts for enrolment. See
[Logging In](03-logging-in.md) for the enrolment flow.

> **Tip** — Keep one backup MFA method enrolled (e.g. backup phone or
> recovery code) so a lost device does not lock you out.

## Session timeout and idle expiry

For security, sessions expire automatically:

| Setting | Default | What it means |
|---|---|---|
| Absolute session length | 8 hours | After 8 hours since sign-in, you must sign in again — even if you have been active. |
| Sliding (idle) timeout | 8 hours | Every request you make resets the clock; closing your browser for less than 8 hours and reopening keeps you signed in. |

Long-running approval pages do not count as activity — you must actually
submit something or navigate to reset the timer. If your session expires you
will be redirected to the sign-in page with a friendly message; your
in-progress work is **not** lost as long as you have not navigated away.

> **Warning** — Never share a sign-in URL with an active session id. If
> someone else needs to see a record, send them the **document number** and
> let them open it in their own session.

## Account lockout

If you (or someone using your account) enter the wrong password too many times,
the account may be locked. Lockout thresholds depend on your identity
provider:

| If the IdP is... | Lockout behaviour |
|---|---|
| Microsoft Entra ID | Smart lockout — variable, contact IT |
| Okta | Configurable, typically 10 attempts in 10 minutes |
| Local accounts | 5 consecutive failed attempts within 15 minutes |

A locked account can be unlocked by a Company Admin or your IT help desk. If
you suspect someone is trying to guess your password, **contact IT
immediately** — the failed attempts are logged with the attempting IP address.

## Security reminders

- **Always sign out** from shared workstations. Click the user menu in the
  top-right corner and choose **Sign out**.
- **Lock your workstation** (Windows: ⊞ Win + L) when you step away.
- **Do not** share your password with anyone — IT will never ask for it.
- Report suspicious sign-in emails (e.g. "your session was unlocked") to
  IT security; do not click links inside them.
- Approve approval requests on your phone only over a trusted network
  (corporate Wi-Fi, VPN, or mobile data — not public Wi-Fi).

## Where to go next

- [Logging In](03-logging-in.md) — detailed sign-in, MFA enrolment, password reset
- [Navigation](04-navigation.md) — how to move around the application
- [Quick-Start Guide](../reference/quick-start.md) — your first 30 minutes
- Your **role-based guide** in [Role-Based Guides](../reference/role-based-guides.md)
