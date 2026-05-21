# 7. Notifications

## Table of Contents
- [What counts as a notification](#what-counts-as-a-notification)
- [Where notifications appear today](#where-notifications-appear-today)
- [In-page toasts](#in-page-toasts)
- [Email notifications](#email-notifications)
- [Push notifications panel](#push-notifications-panel)
- [Per-event preferences](#per-event-preferences)
- [Frequently asked questions](#frequently-asked-questions)

## What counts as a notification

A "notification" in ChuA.ERP is any signal that prompts you to act or to be
aware. Examples:

| Trigger | Notification kind |
|---|---|
| Someone submits a bill that requires your approval | Approval task → Workflow queue + email |
| A purchase order you submitted was approved | Status change → Workflow comment + email |
| A journal entry you posted failed validation | In-page error + audit entry |
| Your password is about to expire (local accounts) | Email + sign-in banner |
| Inventory dipped below an item's reorder point | Planned — alert tile |

## Where notifications appear today

| Channel | Status | Used for |
|---|---|---|
| **Toasts** (in-page) | Available | Outcome of the action you just took (success / warning / error) |
| **Dashboard tiles** | Available | Pending work in your queue (approvals, outstanding invoices) |
| **Workflow › Approvals page** | Available | Your work-list of approval tasks |
| **Email** | Available — depends on tenant SMTP configuration | Out-of-band reminders (digest, escalation) |
| **In-app push panel** (bell icon) | Planned | Real-time arrival of new tasks |
| **SMS / mobile push** | Planned | Optional second channel for urgent approvals |

## In-page toasts

Toasts appear in the top-right corner immediately after an action completes:

`[SCREENSHOT: Three toasts stacked — success, warning, error]`

| Toast colour | Meaning | Example |
|---|---|---|
| Green | Your action succeeded | "Vendor 'Acme Corp' created." |
| Yellow | Action completed but please check something | "Bill approved. Payment terms exceed configured threshold." |
| Red | Your action failed | "Cannot delete: the role is in use by 3 users." |
| Blue | Information only | "Active company switched to 'EU Operations'." |

Toasts:
- Auto-dismiss after **5 seconds**
- Can be closed manually with the × button
- Stack vertically when several arrive at once
- Are announced to screen readers via `role="status"`

## Email notifications

Email is the primary out-of-band channel. The ERP can send email when:

- A new approval task is assigned to you
- A task assigned to you is reassigned (now or away)
- A task is about to escalate to your manager
- A document you submitted is approved or rejected
- A workflow comment is added by another approver

Each email contains:
- The document type, number, and amount
- The actor (who triggered the email)
- A deep link directly to the document in ChuA.ERP
- A reply-to address that is **not monitored** — do not reply to alerts;
  use the application to comment

> **Tip** — Add the email-sending address (typically `erp-notifications@your-domain`)
> to your safe-senders list to prevent alerts from going to Junk.

> **Note** — Email cadence is configurable per tenant. Some tenants prefer
> a **digest** (one email summarising all your pending tasks at 8 AM) rather
> than per-event emails. Talk to your Company Admin about which is enabled.

## Push notifications panel

> **Availability** — **Planned** for an upcoming release.

The next release adds a bell icon in the top bar showing a numeric badge for
unread notifications. Clicking it opens a panel listing the most recent
events:

```
[ Notifications  ✕ ]
+----------------------------------+
| ● Bill B-2026-0142 needs approval|
|   from Acme Corp — $4,500.00     |
|   2 minutes ago                   |
+----------------------------------+
| ● Your PO #PO-001 was approved   |
|   by Sam Manager                  |
|   1 hour ago                      |
+----------------------------------+
| ○ Period Apr-2026 will close in  |
|   3 days                          |
|   yesterday                       |
+----------------------------------+
| Mark all as read · See all       |
+----------------------------------+
```

`[SCREENSHOT: Future-state notifications panel]`

Until this ships, the **Workflow › Approvals** page is your authoritative
work-list.

## Per-event preferences

> **Availability** — **Planned**.

Today, notification preferences are set at the **tenant level** by your
Company Admin (which events trigger email, which do not). The roadmap adds
per-user overrides: you will be able to mute specific event types or
choose between *immediate*, *digest*, and *none* per channel.

In the meantime, if you receive too much or too little email:
1. Contact your Company Admin
2. Ask them to review the tenant's notification matrix
3. Use email rules in your mail client to file or filter ERP alerts

## Frequently asked questions

**Q: I'm not getting email alerts for new bills to approve.**
A: Three likely causes — (1) the events are not enabled in your tenant
   configuration; ask your Company Admin. (2) Your email address on the
   account is wrong; check **My profile**. (3) Your mail gateway is
   quarantining them; ask IT to allow the sender.

**Q: I get the same alert twice — once by email, once on the dashboard.**
A: That's intentional. Email is for when you are away from the app; the
   dashboard tile is for when you are in it. The same item appears in both
   until you act on it.

**Q: Can I forward an approval task to someone else?**
A: Yes — open the task in *Workflow › Approvals*, click **Reassign**, choose
   the new assignee and add a reason. See
   [Workflow & Approvals](08-workflow-approvals.md#reassignment).

**Q: I approved a task on my phone but the count on the dashboard didn't go down.**
A: Refresh the dashboard. KPI tiles are computed at page load, not in real
   time.
