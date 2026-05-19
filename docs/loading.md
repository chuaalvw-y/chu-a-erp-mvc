# ChuA Loading & Progress Component

A reusable client-side loading/progress component for the ChuA ERP MVC UI.
It enforces enterprise UX standards (prevents duplicate form submissions,
disables triggering buttons, surfaces progress for long-running jobs) without
requiring controllers or services to know about UI state.

## Files at a glance

| Layer | File | Purpose |
|---|---|---|
| JS | `wwwroot/js/chua-loading.js` | The single client module. Exposes `window.ChuALoading`. |
| CSS | `wwwroot/css/chua-loading.css` | Bootstrap 5-compatible styles for overlays, spinners, and progress bars. |
| Tag helper | `TagHelpers/LoadingButtonTagHelper.cs` | `<button asp-loading-text="Saving...">` |
| Tag helper | `TagHelpers/LoadingFormTagHelper.cs` | `<form asp-loading="false">`, `<form asp-loading-section="#x">` |
| Tag helper | `TagHelpers/LoadingOverlayTagHelper.cs` | `<loading-overlay id="x" message="...">` |
| Partial | `Views/Shared/_FullPageLoading.cshtml` | Singleton page-level overlay (always present, hidden). |
| Layout | `Views/Shared/_Layout.cshtml` | Includes the CSS, JS, and `<partial name="_FullPageLoading">`. |

Everything is wired automatically by `_Layout.cshtml`. Consumers only opt in
to the behaviours they want.

## Choosing a loading mode

| Operation type | Mode | Why |
|---|---|---|
| **Read-only** (search, filter, paging) | Button spinner only | Don't block the whole page — the user can still browse navigation while results load. |
| **Single-record write** (Create, Update, Delete, Approve, Reject, Pay, Ship, Receive, Adjust) | Button disabled + label change | The triggering button cannot be double-clicked; other controls remain usable in case the user wants to cancel by navigating away. |
| **Critical financial commit** (Post journal entry, period close, irreversible payments) | Section overlay | Locks the affected panel — once you click Confirm, no other control on that section can be touched until the server replies. |
| **Long-running batch** (Import CSV, generate report, recalculate balances) | Full-page overlay + progress bar + Cancel | Blocks all input; communicates ETA via percentage; allows cancellation of background work. |

These map exactly to the four `ChuALoading` JS modes: `button`, *(auto-wire only)*,
`section`, and `fullPage`.

## Behaviour out of the box (no opt-in required)

Every `<form>` rendered by the app is wired automatically:

1. On submit, the JS waits for jQuery validation (or the browser's native
   `checkValidity()`) to pass.
2. If valid, the **clicked submit button is disabled**, gets `aria-busy="true"`
   + `aria-disabled="true"`, swaps its inner HTML for a Bootstrap spinner +
   the configured progress text (or `"Working..."` by default), and adds a
   visually-hidden `role="status"` element so screen readers announce it.
3. The traditional MVC POST→302→GET cycle replaces the page, which restores
   the button to its original state for free.
4. Buttons that come back from the Back/Forward cache (`pageshow` with
   `persisted: true`) are also re-enabled, in case the browser restored a
   stale "loading" state.

To **opt a form out** entirely (e.g. small tag-form filters):

```html
<form asp-loading="false">...</form>
<!-- or, low level: -->
<form data-loading-skip>...</form>
```

## Usage examples

### 1. Search screen — non-blocking spinner

`Views/Shared/_SearchBox.cshtml`:

```html
<form method="get" class="d-flex mb-3" role="search">
    <input type="search" name="search" value="@Model" class="form-control me-2" />
    <button type="submit" class="btn btn-outline-primary"
            asp-loading-text="Searching...">Search</button>
</form>
```

No JS opt-in needed — the global auto-wire handles it. The button shows a
spinner + "Searching..." while the page reloads.

### 2. Save form — button-only loading

`Views/Vendors/Create.cshtml`:

```html
<form asp-action="Create" method="post" novalidate>
    @Html.AntiForgeryToken()
    <partial name="_Form" model="Model" />
    <button type="submit" class="btn btn-primary"
            asp-loading-text="Saving...">Create vendor</button>
</form>
```

Validation runs first — if `Required` rules fail, the button stays clickable
so the user can correct the form and resubmit. On real submit it disables.

### 3. Approve bill — button with processing text

`Views/Bills/Approve.cshtml`:

```html
<form asp-action="Approve" asp-route-id="@Model.Id" method="post"
      data-confirm="Approve this bill?">
    @Html.AntiForgeryToken()
    <button type="submit" class="btn btn-success"
            asp-loading-text="Approving...">Confirm approval</button>
</form>
```

The `data-confirm` runs before submit (handled by `site.js`); if the user
confirms, the button locks immediately. Duplicate clicks/Enter-key spam
cannot reach the server.

### 4. Post journal entry — section overlay

`Views/JournalEntries/Post.cshtml`:

```html
<loading-overlay id="post-section" message="Posting journal entry..." class="card p-4">
    <dl class="row">… summary …</dl>

    <form asp-action="Post" asp-route-id="@Model.Id" method="post"
          asp-loading-section="#post-section"
          asp-loading-section-message="Posting journal entry...">
        @Html.AntiForgeryToken()
        <button type="submit" class="btn btn-success"
                asp-loading-text="Posting...">Confirm post</button>
        <a asp-action="Details" asp-route-id="@Model.Id" class="btn btn-link">Cancel</a>
    </form>
</loading-overlay>
```

The `<loading-overlay>` tag helper renders a positioned host with a hidden
overlay child. When the form submits, `asp-loading-section="#post-section"`
tells the JS to reveal the overlay over the whole card — the summary stops
being interactive and the form is fully visually locked, not just the button.

### 5. File import — full-page overlay + progress + cancel

`Views/Inventory/ImportDemo.cshtml`:

```html
<form id="import-form" data-loading-skip enctype="multipart/form-data">
    <input type="file" name="file" accept=".csv" required />
    <button type="submit" class="btn btn-primary">Start import</button>
</form>

<script>
    document.getElementById('import-form').addEventListener('submit', function (e) {
        e.preventDefault();
        ChuALoading.fullPage.show({
            message: 'Importing items...',
            percent: 0,
            cancellable: true,
            onCancel: function () { /* abort the upload */ ChuALoading.fullPage.hide(); }
        });
        // ... start the upload, then for each progress event:
        // ChuALoading.fullPage.update({ percent: 42, message: '…' });
        // and when complete:
        // ChuALoading.fullPage.hide();
    });
</script>
```

`data-loading-skip` is set on the form because the JS module would otherwise
auto-lock the Submit button on form submit; for long-running async uploads
the page wants to keep control of the lifecycle.

## Public JS API reference

```js
// Button — one-off control.
ChuALoading.button(buttonEl)
  .start({ text: 'Saving...' })  // disable + spinner + aria-busy
  .stop();                       // restore (page nav usually does this for you)

// Section overlay — for write panels and critical financial commits.
ChuALoading.section('#post-section')
  .show({ message: 'Posting...' })
  .hide();

// Full-page overlay — for batches/imports/reports.
ChuALoading.fullPage
  .show({ message: 'Importing...', percent: 0, cancellable: true, onCancel: () => {…} })
  .update({ percent: 50, message: '500 / 1000 rows' })
  .hide();

// Search box helper — convenience wrapper around button().
ChuALoading.search('#searchForm').start();
```

All methods are chainable. Element selectors accept CSS strings or raw DOM
elements (jQuery objects also accepted via `.0` unwrap).

## Accessibility

| Behaviour | Markup contract |
|---|---|
| Button busy | `disabled`, `aria-disabled="true"`, `aria-busy="true"`, spinner has `aria-hidden="true"` to avoid double-announcement, an accompanying `<span class="visually-hidden" role="status">{text}</span>` announces the change. |
| Section overlay | Host gets `aria-busy="true"`; overlay div has `role="status"` and `aria-live="polite"` so the change is announced. |
| Full-page overlay | `<div role="alertdialog" aria-modal="true" aria-labelledby="chua-fullpage-text" aria-busy="true">`. While shown, the underlying page is not scrollable (`html.chua-no-scroll`). |
| Progress bar | `role="progressbar"` with `aria-valuemin="0"`, `aria-valuemax="100"`, live `aria-valuenow`. |
| Reduced motion | `@media (prefers-reduced-motion: reduce)` slows the spin animation to 2s rather than disabling it, so users still see activity but without rapid motion. |

## Error handling

The component never invents an error UI of its own — that would duplicate the
existing `_ValidationSummary` partial and `ProblemDetails` flow.

- **Client-side validation failed**: JS checks `isFormValid(form)` before
  starting the loader, so an invalid form leaves the button enabled and the
  inline validation messages render as usual.
- **Server-side validation failed**: the controller returns the same view with
  `ModelState.AddResultErrors(result)` populated; the page reload restores the
  enabled button and shows the validation summary. No additional wiring needed.
- **Server returned 5xx / network failure**: the existing
  `GlobalExceptionFilter` redirects to `Views/Shared/Error.cshtml`. Page
  navigation again restores the button.
- **Long-running job failed mid-stream**: the caller must catch the failure in
  JS and call `ChuALoading.fullPage.hide()` followed by surfacing a toast or
  banner. The `ImportDemo` view shows the pattern.

## Testing

Tag-helper unit tests live in `tests/.../TagHelpers/`:

- `LoadingButtonTagHelperTests.cs` — verifies `data-loading-text` /
  `data-loading-skip` attribute emission and empty-input handling.
- `LoadingFormTagHelperTests.cs` — verifies the section selector + message
  attributes and the opt-out path.
- `LoadingOverlayTagHelperTests.cs` — verifies the host markup, the hidden
  overlay child, custom class merging, and HTML encoding of the message.

The JS module is browser-only; behaviour is covered by manual testing against
the demo pages (`Vendors/Index` for search, `Vendors/Create` for button,
`Bills/Approve` for confirm + processing label, `JournalEntries/Post` for
section overlay, `Inventory/ImportDemo` for the full-page overlay).

## When **not** to use this

- **Genuine async XHR/fetch flows** that don't navigate away on completion —
  add `data-loading-skip` on the form and call the JS API manually so the
  start/stop lifecycle is bound to the XHR.
- **Tooltips, dropdowns, modals** loading their content — Bootstrap already
  provides spinner classes; use those rather than a full overlay.
- **Page navigation** (`<a>` links to other pages) — the browser's own
  loading bar handles this.
