# Frequently Asked Questions

Sorted by topic. Use Ctrl/Cmd+F to find a specific question.

## Table of Contents
- [Sign-in & account](#sign-in--account)
- [Permissions](#permissions)
- [Approvals](#approvals)
- [Purchasing & AP](#purchasing--ap)
- [Sales & AR](#sales--ar)
- [Inventory](#inventory)
- [Finance & GL](#finance--gl)
- [Reports](#reports)
- [Multi-company](#multi-company)
- [Performance & reliability](#performance--reliability)
- [Mobile](#mobile)
- [Help & support](#help--support)

## Sign-in & account

**Q: How do I reset my password?**
A: For local accounts, click **Forgot password?** on the sign-in page.
For SSO tenants, use your identity provider's self-service password
reset (typically a portal like `passwordreset.your-company.com`).

**Q: I forgot my user name. How do I look it up?**
A: For security, ChuA.ERP does not expose account lookup. Contact your
Company Admin or IT help desk.

**Q: I'm locked out after multiple attempts.**
A: Wait 15 minutes for auto-unlock (some tenants) or contact your
Company Admin. If you didn't try repeatedly, report to IT security.

**Q: Can I be signed in on multiple devices at once?**
A: Yes. Each device has its own session. Signing out on one device does
not sign out the others; changing your password does.

**Q: How long until my session expires?**
A: 8 hours of activity-resetting (sliding) inactivity, or 8 hours
maximum absolute, whichever comes first.

**Q: My identity provider added MFA. Will I be prompted in ChuA.ERP?**
A: Yes — at sign-in, the IdP handles MFA before redirecting back to the
app. After that, ChuA.ERP trusts the IdP-asserted identity for the
session.

## Permissions

**Q: Why don't I see the Vendors menu?**
A: You don't have `VendorRead` permission. Contact your Company Admin.

**Q: I see Vendors but can't create new ones.**
A: You have `VendorRead` but not `VendorCreate`. The New vendor button
is hidden when you can't use it.

**Q: I'm a manager — should I have SystemAdmin?**
A: Almost certainly **no**. SystemAdmin is a platform-wide break-glass
role; managers typically get `CompanyAdmin` for their Company plus the
functional permissions they need.

**Q: How do I find out what permissions I have?**
A: *User menu › My profile*. The Permissions block lists every grant.

**Q: Can permissions be granted temporarily?**
A: Today, no — they are sticky until removed. Time-bounded grants are
on the roadmap.

## Approvals

**Q: Why can't I approve a purchase order I submitted?**
A: Segregation of duties — the system blocks self-approval. Have a
colleague approve.

**Q: I rejected a bill by mistake. Can I undo?**
A: No — rejection is final from the workflow engine's perspective. The
bill returned to Draft; the submitter must resubmit. You can add a
comment on the resubmission to acknowledge the error.

**Q: My delegate didn't get my tasks while I was on leave.**
A: Self-service delegation is **Planned**. Today, manually reassign
tasks before going on leave.

**Q: How do I know an approval workflow rule applies to my submission?**
A: Submit the document; if a task is created in *Workflow › Approvals*
the rule fired. If not, either the submission auto-approved (under
threshold) or the rule didn't match.

**Q: Two approvers are needed but only one task appeared on my queue.**
A: That's correct for parallel approval — each approver sees the task
in their own queue. The document advances only when all required
approvers have submitted.

## Purchasing & AP

**Q: Can I create a bill before the PO?**
A: Technically yes, but best practice is PO → Receipt → Bill. Auditors
prefer the trail.

**Q: How do I match a bill to a PO?**
A: Three-way match is **Planned**. Today, reference the PO number in
the bill's Reference field and verify manually.

**Q: A vendor's invoice arrives in a different currency than the PO.**
A: Enter the bill in the vendor's currency. The GL handles FX
conversion. Document the rate used in the JE.

**Q: How do I write off a small under-payment from a vendor?**
A: Create a journal entry: Debit AP, Credit a "small balance write-off"
expense account.

**Q: How do I see all bills I haven't approved yet?**
A: *Purchasing › Bills › Awaiting Approval*, or the Dashboard tile.

**Q: Can a vendor be both a supplier and a customer?**
A: They are separate records (Vendor + Customer) with separate codes.
Maintain both, cross-reference via the Reference field if needed.

## Sales & AR

**Q: Customer paid by credit card. How do I record it?**
A: Apply payment with Method = `CreditCard` and the processor's
reference id in the Reference field. The merchant fee (if separate)
posts as a journal entry to a Card Fees expense.

**Q: I shipped to the wrong warehouse — how do I correct?**
A: Cycle-count adjustment at both warehouses (out from the wrong, in
to the right). Reference the SO and shipment in the reason fields.

**Q: A customer disputes part of an invoice. How do I handle it?**
A: Comment on the invoice with the dispute reason. Either issue a
credit memo (when shipped) or wait for resolution. Don't auto-apply
future payments against a disputed invoice.

**Q: How do I track credit limits?**
A: The customer record has a Credit limit field. The system warns when
new invoices would exceed it (and may block, per tenant policy).

**Q: When does revenue recognise — at shipment or invoice?**
A: Today, when the invoice posts. Configure your accounting policy
accordingly. Future revenue-recognition rules (ASC 606 / IFRS 15) are
on the roadmap.

## Inventory

**Q: Why is my available quantity less than on-hand?**
A: Available = on-hand − quantity reserved for open Sales Orders.

**Q: How do I do a stock take?**
A: Until cycle-count automation ships: print on-hand, count physically,
post Adjustments for the variances with a `cycle-count-YYYY-MM` reason.

**Q: How do I transfer stock between warehouses?**
A: Two adjustments today (negative at source, positive at destination).
Transfer documents are **Planned**.

**Q: My item disappeared from dropdowns.**
A: It was unticked **Stocked** by someone. Edit the item and tick
Stocked.

**Q: We have lot-controlled inventory. When does that ship?**
A: Lot tracking is **Planned**. Talk to your account team for ETA.

## Finance & GL

**Q: Can I edit a posted journal entry?**
A: No. Post a reversing entry, then post the corrected entry.

**Q: How do I know my JE is balanced?**
A: The form shows running debit and credit totals at the bottom. The
**Save** button stays disabled until ∑debit = ∑credit.

**Q: I posted to the wrong period.**
A: Post a reversal in the wrong period and a fresh entry in the right
period.

**Q: How is FX revaluation handled?**
A: Today, manually each period via a JE that revalues monetary
positions to the period-end rate. Automation is on the roadmap.

**Q: Can I delete an account from the COA?**
A: Only if it has no postings. Otherwise mark it non-postable; the
account stays in the hierarchy for historical reports.

## Reports

**Q: How do I export to Excel?**
A: Built-in Excel export is **Planned**. Today, use the browser
Print → Save as PDF, or copy-paste the table into Excel.

**Q: How do I switch companies/tenants?**
A: User menu (top-right) → **Switch company**.

**Q: Why doesn't my report parameter form have typed inputs?**
A: Reports accept parameters as JSON today; typed inputs are **Planned**.

**Q: Can I schedule a report?**
A: Self-service scheduling is **Planned**. Today, ask IT to schedule
extracts via the platform API.

## Multi-company

**Q: I work for both Acme US and Acme UK. How does that work?**
A: Your account is created in both Companies. Use the user menu to
switch.

**Q: Can I see numbers across all companies at once?**
A: Consolidated reporting is **Planned**. Today, run reports per
Company and consolidate in Excel.

**Q: I posted to the wrong Company.**
A: Reverse in the wrong Company (post a reversal JE), then post the
correct entry in the correct Company.

## Performance & reliability

**Q: The app is slow today.**
A: Check the **System Health** page. If the API is degraded, support
is likely already aware. If health is green but pages feel slow,
note your request id and ping support.

**Q: I see a yellow banner on the Dashboard.**
A: One of the dashboard's API calls failed. Tile values may be stale.
Visit System Health to see which dependency is unhappy.

**Q: My data hasn't refreshed.**
A: Most pages fetch at load. Hit refresh in the browser or revisit
the page.

## Mobile

**Q: Can I work on my phone?**
A: Yes, with caveats. The UI is responsive: approvals, dashboards,
and lookups work well. Heavy data entry (multi-line documents) is
better on a desktop.

**Q: Is there a mobile app?**
A: A dedicated mobile app is on the roadmap. Today, use mobile Safari
/ Chrome / Edge.

## Help & support

**Q: How do I report a bug?**
A: Send your help desk:
- The page URL
- The request id from the footer
- A description of what you did and what you expected
- A screenshot if visual
- The time of the issue (so logs can be correlated)

**Q: How do I request a feature?**
A: Via your account team — they collect requests and feed them into
the product roadmap.

**Q: Where do I get training?**
A: Start with the [Quick-Start Guide](quick-start.md) and your
[role-based guide](role-based-guides.md). Your Company Admin may also
have scheduled training in your tenant.
