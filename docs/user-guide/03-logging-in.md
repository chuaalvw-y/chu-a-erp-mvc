# 3. Logging In

## Table of Contents
- [Sign-in methods](#sign-in-methods)
- [Standard sign-in](#standard-sign-in)
- [Single sign-on (SSO)](#single-sign-on-sso)
- [Multi-factor authentication](#multi-factor-authentication)
- [Resetting your password](#resetting-your-password)
- [Forgotten user name](#forgotten-user-name)
- [Signing out](#signing-out)
- [Switching companies](#switching-companies)
- [Common sign-in errors](#common-sign-in-errors)

## Sign-in methods

Depending on how your tenant is configured, you will sign in one of two ways:

| Method | Triggered by | What you enter |
|---|---|---|
| Standard sign-in (local accounts) | Tenant runs without SSO | User name + password (+ MFA if enabled) |
| Single sign-on (SSO) | Tenant integrated with corporate IdP | Just click **Sign in** — IdP handles credentials |

To find out which method your tenant uses, look at the sign-in page: an SSO
tenant shows a single **Sign in with [provider]** button. A local-account
tenant shows user name and password fields.

## Standard sign-in

1. Navigate to your tenant URL.
2. The sign-in page appears.
   `[SCREENSHOT: Local-account sign-in page]`
3. Enter your **user name** (your work email, unless your administrator told
   you otherwise).
4. Enter your **password**.
5. Click **Continue**.
6. If MFA is enabled, you are prompted for your second factor — see below.
7. You land on the Dashboard.

> **Tip** — Use a password manager. The login form is a standard HTML form;
> any password manager (1Password, Bitwarden, Microsoft Edge built-in, etc.)
> can save and autofill it.

## Single sign-on (SSO)

1. Navigate to your tenant URL.
2. You are redirected to your organisation's identity provider sign-in page.
   `[SCREENSHOT: SSO sign-in page (Entra ID example)]`
3. Sign in with your usual corporate credentials.
4. Complete any MFA challenge required by IT (push notification, code, key).
5. You are redirected back to ChuA.ERP and land on the Dashboard.

> **Note** — Because SSO sign-in happens outside ChuA.ERP, the ERP can never
> see your corporate password. Password-related issues (forgotten password,
> expired password, locked account) are handled by your IT help desk and the
> identity provider, not by your Company Admin.

## Multi-factor authentication

When MFA is required, you will be prompted for a second factor immediately
after entering your password (or by your identity provider during SSO).

### Enrolling a new factor (local accounts)

If your account requires MFA and you have not yet enrolled:

1. After entering user name and password, you see the **MFA enrolment** page.
2. Choose a method:
   - **Authenticator app** — recommended. Scan the QR code with Microsoft
     Authenticator, Google Authenticator, Duo, or any TOTP-compatible app.
     Enter the six-digit code shown by the app to confirm.
   - **SMS** — enter your mobile number; a verification text is sent.
3. Save the **recovery codes** displayed at the end of enrolment in a safe
   place (your password manager works well). Each recovery code can be used
   once if you lose access to your authenticator.
4. Click **Continue** to enter the app.

`[SCREENSHOT: MFA enrolment page showing QR code]`

> **Warning** — If you lose your authenticator and have not stored a recovery
> code, your Company Admin must reset your MFA. This requires identity
> verification and may take time. Always save recovery codes.

### Signing in with MFA already enrolled

1. After password (or SSO), the **MFA challenge** page appears.
2. Enter the current 6-digit code from your authenticator, or approve the
   push notification, or insert your security key.
3. Click **Verify**.

### Using a recovery code

If you have lost your authenticator:

1. On the MFA challenge page click **Use a recovery code instead**.
2. Enter one of the recovery codes you saved during enrolment.
3. Sign in.
4. **Immediately** re-enrol a new MFA method via *User menu › Security*.
   The recovery code you just used is now invalidated.

## Resetting your password

> **Note** — Applies to local-account tenants only. SSO password reset is
> done at your identity provider (e.g. Entra ID self-service password reset).

1. On the sign-in page click **Forgot password?**.
2. Enter your user name (email) and click **Send reset link**.
3. Check your email — the reset link is valid for **30 minutes**.
4. Click the link in the email; it opens a **Set new password** page.
5. Enter and confirm your new password (must satisfy the password policy).
6. Sign in normally with the new password.

> **Tip** — If the email does not arrive within five minutes, check your
> Junk/Spam folder. Some corporate spam filters quarantine reset emails;
> ask IT to allow `noreply@your-erp-domain` in their gateway rules.

## Forgotten user name

If you do not remember your user name, contact your Company Administrator or
IT help desk. For audit reasons ChuA.ERP does not expose "look up my account
by name" — that would let an attacker confirm the existence of an account.

## Signing out

1. Click your name in the **top-right corner** of any page.
2. Choose **Sign out** from the dropdown.
3. You are returned to the sign-in page.

On SSO tenants, signing out of ChuA.ERP also triggers a sign-out at the
identity provider so the same browser session cannot silently re-sign-in.

> **Tip** — Closing the browser tab does **not** sign you out. Anyone with
> physical access to your workstation can reopen the tab and resume your
> session until it times out. Always sign out on shared computers.

## Switching companies

If your account has access to more than one company (legal entity / tenant
sub-organisation) you can switch between them without signing out and back in.

1. Click your name in the top-right corner.
2. Choose **Switch company**.
3. Select the company you want to work in.
4. The Dashboard reloads with that company's data.

See [Multi-Tenant / Company Switching](../admin/multi-tenant.md) for a
deeper explanation of how multi-company access is managed.

## Common sign-in errors

| Error message | Likely cause | What to do |
|---|---|---|
| "User name or password is incorrect." | Typo, or wrong tenant | Verify caps lock, retry, confirm tenant URL |
| "Your account has been locked." | Too many failed attempts | Wait 15 min, or contact IT to unlock |
| "Your session has expired." | Idle timeout passed | Sign back in; in-progress work is preserved if you have not navigated |
| "Access denied." after sign-in | Account exists but has no roles | Contact Company Admin to assign a role |
| "We could not reach the identity provider." | SSO outage or network issue | Wait a minute and retry; if persistent, contact IT |
| "MFA code is invalid." | Expired or wrong code | Wait for next 30-second window; verify clock on authenticator device is correct |

See [Troubleshooting › Sign-in problems](../reference/troubleshooting.md#sign-in-problems)
for a longer list with detailed resolution steps.
