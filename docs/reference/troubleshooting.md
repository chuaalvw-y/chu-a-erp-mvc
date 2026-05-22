# Troubleshooting

When something goes wrong, follow this order:

1. **Take note of the request id** from the page footer.
2. **Read the toast / error message** carefully — the text usually points at
   the cause.
3. **Check the relevant section** below.
4. If unresolved, **contact the help desk** with the request id, the page
   URL, the action you took, and a screenshot.

## Table of Contents
- [Sign-in problems](#sign-in-problems)
- [Permission / access denied](#permission--access-denied)
- [Approval issues](#approval-issues)
- [Forms not saving](#forms-not-saving)
- [Data not appearing](#data-not-appearing)
- [Slow pages](#slow-pages)
- [Browser issues](#browser-issues)
- [Session timeout problems](#session-timeout-problems)
- [Reports](#reports)
- [Inventory discrepancies](#inventory-discrepancies)
- [Payment issues](#payment-issues)
- [Workflow stuck](#workflow-stuck)

## Sign-in problems

### Symptom: "User name or password is incorrect."
**Possible causes**
1. Typo in user name or password (caps lock?)
2. Wrong tenant URL — you may be on a sister tenant
3. Account doesn't exist in this tenant
4. Account exists but is disabled
5. Password expired

**Resolution**
1. Verify caps lock is off; re-type.
2. Confirm tenant URL with IT.
3. Confirm with admin that your account exists in this tenant / Company.
4. Ask admin to confirm account is active.
5. Use *Forgot password?* link or contact IdP help desk if SSO.

### Symptom: "Your account has been locked."
**Possible causes**
- Too many failed sign-in attempts within the lockout window

**Resolution**
1. Wait 15 minutes (some tenants auto-unlock).
2. Contact Company Admin / IT help desk for immediate unlock.
3. If you didn't try to sign in repeatedly yourself, **report this to
   IT security** — someone may be attempting your password.

### Symptom: "We could not reach the identity provider."
**Possible causes**
- IdP outage
- Network / DNS issue from your workstation
- Firewall blocking the OIDC endpoints

**Resolution**
1. Try a known external site — confirm you have internet.
2. Wait 60 seconds and retry.
3. Try from a different network (mobile hotspot).
4. Contact IT.

### Symptom: "MFA code is invalid."
**Possible causes**
- Code expired (codes rotate every 30s)
- Clock drift on your authenticator device
- You typed the wrong code

**Resolution**
1. Wait for the next code to appear in your authenticator.
2. Check your phone's time is correct (auto-sync time on Android / iOS).
3. Try again; if persistent, use a recovery code and re-enrol.

### Symptom: "Access denied." after successful sign-in
**Possible causes**
- Account exists but has no roles in the active Company

**Resolution**
1. Contact your Company Admin to assign at least one role.

### Symptom: Sign-in succeeds but the page is blank
**Possible causes**
- Browser extensions interfering (ad blockers, content security policies)
- Cached corrupted state

**Resolution**
1. Try **Incognito / Private browsing**.
2. Clear browser cache and cookies for the tenant origin.
3. Disable extensions one at a time.

## Permission / access denied

### Symptom: A sidebar item I expect to see is missing
**Possible cause** — The role granting that menu item is not assigned to you.

**Resolution** — Contact your Company Admin. Reference the
[permission catalogue](../admin/security-permissions.md#permission-catalogue)
for the specific permission you need.

### Symptom: Action button (e.g. Approve, Pay) is hidden on a document
**Possible cause** — Either you lack the permission, or the document is in
a state that doesn't permit the action.

**Resolution**
1. Confirm the document's status — e.g. a bill can only be Paid after
   it has been Approved.
2. If status is right but the button is missing, you lack the action
   permission. Contact Company Admin.

### Symptom: "Access denied" page after navigating
**Possible cause** — URL guessing or stale link to a feature you can't use.

**Resolution** — Go back to the Dashboard via the breadcrumb and reach
the feature through the sidebar (which only shows what you can use).

## Approval issues

### Symptom: I can see the bill but can't approve it
**Possible causes**
- You lack `BillApprove` permission
- You are the submitter (self-approval is blocked)
- The bill isn't in `PendingApproval` status
- It's assigned to someone else's queue

**Resolution**
1. Check your permission via *My profile*.
2. Verify you didn't submit the bill yourself.
3. Open the bill's workflow task in *Workflow › Approvals* (not the bill
   detail page); the Submit decision button lives there.

### Symptom: A task is on my queue but the underlying document is gone
**Possible cause** — Document was deleted while the task was still pending.

**Resolution**
1. Reject the task with a comment explaining the document is missing.
2. Or have an admin cancel the orphan task.

### Symptom: Approval task did not arrive (I'm in the policy chain)
**Possible causes**
- The submitter is also you (engine refuses to assign)
- Workflow rule doesn't match (amount band wrong, condition false)
- Another approver was found first

**Resolution**
1. Verify with the submitter who they expected to approve.
2. Have your Company Admin check the rule's condition.
3. Have a colleague reassign the task to you if needed.

## Forms not saving

### Symptom: Submit button is disabled and shows a spinner forever
**Possible cause** — Network or API outage.

**Resolution**
1. Wait 30s — the request may complete.
2. If it never returns, refresh the page. The form draft is **not**
   preserved; you will lose unsaved data.
3. Check *System Health* on the sidebar.
4. If health is fine, contact support with the page URL and request id.

### Symptom: Submitting shows validation errors I can't see
**Possible cause** — Validation summary is above the fold; scroll up.

**Resolution**
1. Scroll to the top of the form.
2. Look for red text inline on each field.

### Symptom: "You don't have permission to perform this action" on save
**Possible cause** — You can read the form but not write it (rare; usually
the form would have been hidden).

**Resolution** — Contact Company Admin.

### Symptom: "Concurrency conflict" or "the record was modified by another user"
**Possible cause** — Someone else saved changes to the same record between
your load and your save.

**Resolution**
1. Refresh the record.
2. Re-apply your changes.
3. Save again.

## Data not appearing

### Symptom: I just created a record but it doesn't show in the list
**Possible causes**
- List is filtered (search / status / vendor filter still active)
- You are on a different page of the paginator
- You are in a different Company than expected
- The record was created in another Company (account access issue)

**Resolution**
1. Clear filters / search.
2. Go to page 1.
3. Check the active Company in *My profile* / user menu.

### Symptom: Counts on the Dashboard don't match what's in lists
**Possible cause** — Dashboard tiles are computed at page load; the list
shows real-time. If something was approved between the Dashboard render
and the list render, they'll differ.

**Resolution** — Refresh the Dashboard.

### Symptom: Reports return zero rows when I expect data
**Possible causes**
- Date range parameters too narrow
- Wrong Company
- Permission scope (you only see records you can read)

**Resolution**
1. Loosen date ranges.
2. Confirm Company.
3. Check permissions.

## Slow pages

### Symptom: A page takes more than 5 seconds to load
**Possible causes**
- Backend (API) degradation
- Your network
- Particular page with heavy queries
- High concurrent load

**Resolution**
1. Check *System Health* — if API is degraded, wait and retry.
2. Try a different page — if all are slow, it's network/backend.
3. If only one page is slow, note the request id and report to support.

## Browser issues

### Symptom: Buttons don't respond / forms don't submit
**Possible causes**
- JavaScript disabled
- Browser extension interfering

**Resolution**
1. Enable JavaScript for the tenant origin.
2. Try Incognito / Private browsing.
3. Update the browser.

### Symptom: Page looks broken / unstyled
**Possible cause** — CSS / JS failed to load (often a corporate proxy issue).

**Resolution**
1. Force-refresh (Ctrl+F5 or Cmd+Shift+R).
2. Clear cache.
3. Ask IT whether the corporate proxy is blocking the static-asset CDN.

### Symptom: Reports / popups don't open
**Possible cause** — Browser popup blocker.

**Resolution** — Allow popups for the tenant origin.

## Session timeout problems

### Symptom: "Your session has expired" while I was actively working
**Possible causes**
- Idle exceeded (no requests in the past 8 hours)
- IdP forced re-authentication
- Cookie cleared by browser

**Resolution**
1. Sign back in. In-progress forms that hadn't been saved are lost.
2. Save more frequently in the future.
3. If recurring within minutes, check for clock drift on your machine.

### Symptom: Signed out unexpectedly after switching companies
**Possible cause** — Company switch invalidates token caches in some
configurations; you may be prompted to re-sign-in.

**Resolution** — Sign back in; the chosen Company is remembered.

## Reports

### Symptom: "Parameters must be valid JSON"
**Resolution** — Validate the JSON. Common pitfalls: trailing commas,
unmatched braces, smart quotes from a copy-paste. Use jsonlint.com.

### Symptom: Report runs but row count is implausible
**Resolution** — Verify the date range is exactly what you intend.
Off-by-one date boundaries are common. Run the same report for a known
small period (e.g. one specific day) to sanity-check.

### Symptom: Excel export missing / disabled
**Resolution** — Built-in Excel export is **Planned**. Use Print → Save
as PDF, or copy-paste the result into Excel.

## Inventory discrepancies

### Symptom: System on-hand differs from physical count
**Resolution**
1. Verify warehouse — are you looking at the right one?
2. Verify recent movements: Receipts, Shipments, Adjustments.
3. Post a cycle-count adjustment with the correct delta and a reason
   like `cycle-count-2026-05`.

### Symptom: An adjustment posted to the wrong warehouse
**Resolution**
1. Adjustments are immutable.
2. Post a counter-adjustment in the wrong warehouse (opposite sign).
3. Post the original adjustment again in the correct warehouse.
4. Reference the corrected pair in the reason field.

## Payment issues

### Symptom: Bill shows Outstanding but I paid it
**Resolution**
1. Open the bill, scroll to the Payments section.
2. Verify the payment was attached to *this* bill — sometimes payments
   land on the wrong bill.
3. If misapplied, post a journal-entry correction.

### Symptom: Tried to pay > Outstanding
**Resolution** — The system blocks. Reduce the payment amount, or split
across multiple bills.

### Symptom: Refund needed
**Resolution** — Journal entry reversing the original payment, then bank
refund executed externally. Record the bank reference in the JE.

## Workflow stuck

### Symptom: Task has been Pending for weeks
**Resolution**
1. Reassign manually to a present approver.
2. Investigate why escalation didn't fire — the SLA configuration may be
   wrong (talk to Company Admin).

### Symptom: Same task appeared on two queues
**Possible cause** — Parallel approval policy (both must approve).

**Resolution** — Confirm with your manager. Both decisions are
required; the system waits for both.

### Symptom: Document went back to Draft after I approved
**Possible cause** — A later approver rejected the task.

**Resolution** — Review the rejection comment; coordinate with the
submitter to address the concern and resubmit.
