# Quick-Start Guide

> **For** — Anyone signing in to ChuA.ERP for the first time. About a
> 30-minute read-through. Pair with hands-on time in your tenant.

## Table of Contents
- [Before you begin](#before-you-begin)
- [Sign in](#sign-in)
- [Find your way around](#find-your-way-around)
- [Check your queue](#check-your-queue)
- [Try a read-only action](#try-a-read-only-action)
- [Try a write action (if your role permits)](#try-a-write-action-if-your-role-permits)
- [Where to go next](#where-to-go-next)

## Before you begin

You should have received from your admin or manager:

1. The **tenant URL** (e.g. `https://erp.your-company.com`)
2. Your **user name** (often your email)
3. Either a **temporary password** OR notice that you sign in via SSO

You will need:

- A supported browser (Edge, Chrome, Firefox, Safari — latest two versions)
- A few minutes
- Your phone if MFA is configured for your tenant

## Sign in

1. Open your browser, paste the tenant URL.
2. Either:
   - Enter user name + password, then complete MFA prompts, OR
   - Click **Sign in** and be redirected to your corporate sign-in.
3. After successful sign-in you land on the **Dashboard**.

`[SCREENSHOT: First-time dashboard]`

If sign-in fails, see [Logging In › Common sign-in errors](../user-guide/03-logging-in.md#common-sign-in-errors).

## Find your way around

Take 30 seconds to orient yourself:

- **Top bar** — Your name (top-right), environment badge (top-left), product
  name.
- **Sidebar** — Modules grouped by function: Purchasing, Sales, Inventory,
  Finance, Workflow, Reports, Administration (if you have admin rights).
- **Breadcrumbs** — Below the top bar; show your path through the app.
- **Footer** — Request id (useful when contacting support).

Click around the sidebar. Don't worry — read pages don't change anything.

## Check your queue

Click **Workflow › Approvals** in the sidebar.

If you have approval permissions you'll see your pending tasks here. Each
row shows what needs your decision and who submitted it.

| Action | What it does |
|---|---|
| Click a row | Open the task; review the underlying document |
| **Submit decision** | Approve or Reject with optional comment |
| **Reassign** | Pass it to someone else |

If the list is empty, that's normal — most users only see tasks
occasionally. Move on.

## Try a read-only action

Pick a module where you have at least Read permission. Let's say
**Purchasing › Vendors**.

1. Click *Purchasing › Vendors*.
2. Try the **Search** box — type a few letters of a vendor name.
3. Click any vendor row to see its detail.
4. Click your browser **Back** button to return to the list.
5. Try the **paginator** at the bottom — navigate to page 2 if there are
   more than 25 vendors.

Repeat for **Customers**, **Inventory**, **Chart of Accounts** if you
have Read permission for them.

## Try a write action (if your role permits)

> **Warning** — If you are in production, **only** practise this in a
> sandbox / UAT environment. Don't create test data in production.

The smallest write action is creating a vendor / customer:

1. *Purchasing › Vendors › New vendor*.
2. Enter:
   - Vendor code `TEST-001` (or a code your admin reserved for training)
   - Legal name "Training Vendor"
   - Default currency `USD`
   - Payment terms `30`
3. Click **Create vendor**.
4. You land on the new vendor's detail page. A green toast confirms
   success.

Now edit it:

1. From the detail page, click **Edit**.
2. Change the legal name to "Training Vendor (Updated)".
3. Click **Save changes**.
4. You return to the detail page; the new name is shown.

Finally delete the training record (so you don't pollute the master
list):

1. From the detail page, click **Delete**.
2. The confirmation page asks "Delete this vendor? This cannot be
   undone."
3. Click **Delete vendor**.
4. You return to the list; a green toast confirms.

You have now performed every step of the **create → edit → delete**
cycle that underpins every module.

## Where to go next

| If you are... | Read |
|---|---|
| An AP clerk | [Role guide: AP](role-based-guides.md#ap-clerk--ap-manager) and the [Purchasing module](../modules/purchasing.md) |
| An AR clerk | [Role guide: AR](role-based-guides.md#ar-clerk--ar-manager) and the [Sales module](../modules/sales.md) |
| A buyer | [Role guide: Buyer](role-based-guides.md#buyer--procurement-manager) and the [Purchasing module](../modules/purchasing.md) |
| A sales rep | [Role guide: Sales](role-based-guides.md#sales-rep--sales-manager) and the [Sales module](../modules/sales.md) |
| A warehouse clerk | [Role guide: Warehouse](role-based-guides.md#warehouse) and the [Inventory module](../modules/inventory.md) |
| An accountant | [Role guide: Accountant](role-based-guides.md#accountant--controller) and the [Finance module](../modules/finance.md) |
| An approver | [Workflow & Approvals](../user-guide/08-workflow-approvals.md) |
| An admin | [Administrator Guide](../admin/administration.md) |
| Stuck | [Troubleshooting](troubleshooting.md) and [FAQ](faq.md) |

> **Tip** — Keep this document open in a browser tab during your first
> week. Practise the workflows that match your role. Ask your manager
> to pair-walkthrough a real transaction before you fly solo.
