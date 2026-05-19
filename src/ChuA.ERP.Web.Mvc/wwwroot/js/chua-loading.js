/*!
 * ChuA Loading — enterprise loading & progress component for the ChuA ERP UI.
 *
 * Public API (exposed as window.ChuALoading):
 *
 *   ChuALoading.button(buttonEl)
 *     .start({ text })       // disable, swap content for spinner + text, mark aria-busy
 *     .stop()                // restore original content + enabled state
 *
 *   ChuALoading.section(idOrEl)
 *     .show({ message })     // reveal an overlay covering the section
 *     .hide()
 *
 *   ChuALoading.fullPage
 *     .show({ message, cancellable, onCancel })  // page-wide blocking overlay
 *     .update({ percent, message })               // progress feedback
 *     .hide()
 *
 *   ChuALoading.search(formEl)
 *     .start() / .stop()    // inline spinner inside the search button (non-blocking)
 *
 * Auto-wiring:
 *   - Every <form> that is NOT marked data-loading-skip has its submit button
 *     placed in a loading state on submit (after client-side validation passes).
 *     This prevents accidental double-submits across the entire ERP without
 *     any view edits.
 *
 *   - The triggering button's data-loading-text overrides the default text
 *     ("Working...") and the original innerHTML is restored on page navigation
 *     (which, for a traditional MVC POST→redirect, comes "for free" because
 *     the page is replaced).
 *
 * Accessibility:
 *   - aria-busy="true" placed on the host container while loading.
 *   - aria-disabled="true" + the disabled attribute placed on the button.
 *   - Spinner markup carries role="status" with visually-hidden text so screen
 *     readers announce the activity.
 *   - Progress markup uses role="progressbar" with aria-valuemin/max/now.
 */
(function (window, document) {
    'use strict';

    var DEFAULT_BUSY_TEXT = 'Working...';
    var SPINNER_HTML = '<span class="spinner-border spinner-border-sm me-2" aria-hidden="true"></span>';
    var SR_HTML = function (text) { return '<span class="visually-hidden" role="status">' + escapeHtml(text) + '</span>'; };

    function escapeHtml(s) {
        return String(s || '').replace(/[&<>"']/g, function (c) {
            return { '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#39;' }[c];
        });
    }

    function resolveEl(target) {
        if (!target) return null;
        if (typeof target === 'string') return document.querySelector(target);
        if (target.jquery) return target[0];
        return target;
    }

    // ---------------------- Button controller ----------------------

    function ButtonController(btn) {
        this.btn = btn;
    }

    ButtonController.prototype.start = function (opts) {
        opts = opts || {};
        var btn = this.btn;
        if (!btn || btn.disabled) return this;                       // already busy → no-op (prevents double-click)

        var text = opts.text || btn.getAttribute('data-loading-text') || DEFAULT_BUSY_TEXT;
        if (!btn.hasAttribute('data-original-html')) {
            btn.setAttribute('data-original-html', btn.innerHTML);
        }
        btn.innerHTML = SPINNER_HTML + escapeHtml(text) + SR_HTML(text);
        btn.disabled = true;
        btn.setAttribute('aria-disabled', 'true');
        btn.setAttribute('aria-busy', 'true');
        btn.classList.add('chua-btn-loading');
        return this;
    };

    ButtonController.prototype.stop = function () {
        var btn = this.btn;
        if (!btn) return this;
        var original = btn.getAttribute('data-original-html');
        if (original !== null) {
            btn.innerHTML = original;
            btn.removeAttribute('data-original-html');
        }
        btn.disabled = false;
        btn.removeAttribute('aria-disabled');
        btn.removeAttribute('aria-busy');
        btn.classList.remove('chua-btn-loading');
        return this;
    };

    function button(target) { return new ButtonController(resolveEl(target)); }

    // ---------------------- Section overlay controller ----------------------

    function SectionController(host) {
        this.host = host;
    }

    SectionController.prototype._ensureOverlay = function (message) {
        var overlay = this.host.querySelector(':scope > .chua-loading-overlay');
        if (!overlay) {
            overlay = document.createElement('div');
            overlay.className = 'chua-loading-overlay';
            overlay.setAttribute('role', 'status');
            overlay.setAttribute('aria-live', 'polite');
            overlay.innerHTML =
                '<div class="chua-loading-overlay__inner">' +
                    '<div class="spinner-border" aria-hidden="true"></div>' +
                    '<p class="chua-loading-overlay__text">' + escapeHtml(message || 'Loading...') + '</p>' +
                '</div>';
            // Ensure positioning context so overlay covers only this host.
            var pos = window.getComputedStyle(this.host).position;
            if (pos === 'static' || !pos) this.host.classList.add('chua-loading-host');
            this.host.appendChild(overlay);
        } else if (message) {
            var p = overlay.querySelector('.chua-loading-overlay__text');
            if (p) p.textContent = message;
        }
        return overlay;
    };

    SectionController.prototype.show = function (opts) {
        if (!this.host) return this;
        var overlay = this._ensureOverlay((opts || {}).message);
        overlay.removeAttribute('hidden');
        this.host.setAttribute('aria-busy', 'true');
        return this;
    };

    SectionController.prototype.hide = function () {
        if (!this.host) return this;
        var overlay = this.host.querySelector(':scope > .chua-loading-overlay');
        if (overlay) overlay.setAttribute('hidden', '');
        this.host.removeAttribute('aria-busy');
        return this;
    };

    function section(target) { return new SectionController(resolveEl(target)); }

    // ---------------------- Full-page overlay ----------------------

    var FULL_PAGE_ID = 'chua-fullpage-loading';

    function fullPageEl() {
        return document.getElementById(FULL_PAGE_ID);
    }

    var fullPage = {
        show: function (opts) {
            opts = opts || {};
            var el = fullPageEl();
            if (!el) return this;
            el.querySelector('.chua-fullpage__text').textContent = opts.message || 'Please wait...';

            // Progress
            this.update({ percent: typeof opts.percent === 'number' ? opts.percent : null });

            // Cancel button
            var cancelBtn = el.querySelector('.chua-fullpage__cancel');
            if (opts.cancellable && typeof opts.onCancel === 'function') {
                cancelBtn.removeAttribute('hidden');
                cancelBtn.onclick = function () {
                    try { opts.onCancel(); } finally { /* keep overlay until caller hides */ }
                };
            } else {
                cancelBtn.setAttribute('hidden', '');
                cancelBtn.onclick = null;
            }

            el.removeAttribute('hidden');
            el.setAttribute('aria-busy', 'true');
            document.documentElement.classList.add('chua-no-scroll');
            return this;
        },

        update: function (opts) {
            opts = opts || {};
            var el = fullPageEl();
            if (!el) return this;
            if (typeof opts.message === 'string') {
                el.querySelector('.chua-fullpage__text').textContent = opts.message;
            }
            var bar = el.querySelector('.chua-fullpage__progress');
            var fill = el.querySelector('.chua-fullpage__progress-bar');
            var label = el.querySelector('.chua-fullpage__progress-label');
            if (typeof opts.percent === 'number' && !isNaN(opts.percent)) {
                var pct = Math.max(0, Math.min(100, Math.round(opts.percent)));
                bar.removeAttribute('hidden');
                fill.style.width = pct + '%';
                bar.setAttribute('aria-valuenow', pct);
                label.textContent = pct + '%';
            } else if (opts.percent === null) {
                // Reset to indeterminate (hide bar)
                bar.setAttribute('hidden', '');
                fill.style.width = '0%';
                bar.setAttribute('aria-valuenow', '0');
                label.textContent = '';
            }
            return this;
        },

        hide: function () {
            var el = fullPageEl();
            if (!el) return this;
            el.setAttribute('hidden', '');
            el.removeAttribute('aria-busy');
            document.documentElement.classList.remove('chua-no-scroll');
            return this;
        }
    };

    // ---------------------- Search box helper ----------------------

    function SearchController(form) {
        this.form = form;
        this.btn = form ? form.querySelector('button[type="submit"], input[type="submit"]') : null;
    }

    SearchController.prototype.start = function () {
        if (!this.btn) return this;
        button(this.btn).start({ text: 'Searching...' });
        this.form.setAttribute('aria-busy', 'true');
        return this;
    };

    SearchController.prototype.stop = function () {
        if (this.btn) button(this.btn).stop();
        if (this.form) this.form.removeAttribute('aria-busy');
        return this;
    };

    function search(target) { return new SearchController(resolveEl(target)); }

    // ---------------------- Auto-wiring for form submits ----------------------

    function findSubmitter(form, evt) {
        if (evt && evt.submitter) return evt.submitter;
        // Fallback: first <button type=submit> in form
        return form.querySelector('button[type="submit"], input[type="submit"]');
    }

    function isFormValid(form) {
        // Prefer jQuery validation if present (matches MVC's unobtrusive pipeline).
        if (window.jQuery && window.jQuery.fn && typeof window.jQuery.fn.valid === 'function') {
            try {
                var $form = window.jQuery(form);
                if ($form.data('validator')) return $form.valid();
            } catch (e) { /* fall through */ }
        }
        // Browser-native check.
        if (typeof form.checkValidity === 'function') return form.checkValidity();
        return true;
    }

    function onFormSubmit(evt) {
        var form = evt.target;
        if (!form || form.tagName !== 'FORM') return;
        if (form.hasAttribute('data-loading-skip')) return;
        if (!isFormValid(form)) return;                                   // validation failed → don't start loader

        var btn = findSubmitter(form, evt);
        if (btn && !btn.hasAttribute('data-loading-skip')) {
            button(btn).start();
        }

        // Optional: per-form section overlay during submit.
        var sectionTarget = form.getAttribute('data-loading-section');
        if (sectionTarget) {
            section(sectionTarget).show({ message: form.getAttribute('data-loading-section-message') });
        }
    }

    // Re-enable buttons that came back from bfcache (Back/Forward Cache).
    function onPageShow(evt) {
        if (!evt.persisted) return;
        document.querySelectorAll('.chua-btn-loading').forEach(function (btn) {
            button(btn).stop();
        });
        document.querySelectorAll('.chua-loading-overlay:not([hidden])').forEach(function (overlay) {
            overlay.setAttribute('hidden', '');
            var host = overlay.parentElement;
            if (host) host.removeAttribute('aria-busy');
        });
        fullPage.hide();
    }

    function init() {
        document.addEventListener('submit', onFormSubmit, true);
        window.addEventListener('pageshow', onPageShow);
    }

    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', init);
    } else {
        init();
    }

    window.ChuALoading = {
        button: button,
        section: section,
        fullPage: fullPage,
        search: search,
        // Exposed for tests / advanced use:
        _internals: { isFormValid: isFormValid, findSubmitter: findSubmitter }
    };
})(window, document);
