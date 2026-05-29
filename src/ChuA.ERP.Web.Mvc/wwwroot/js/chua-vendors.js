// Copyright (c) 2026 Alvin Wilsen Chan Chua
// GitHub: chuaalvw-y
// Licensed under the Alvin Wilsen Chan Chua Proprietary Use-Only License.
// See LICENSE.txt in the project root for full license information.

/*
 * Vendors module — reference reactive UX for the master-data grid pattern. Demonstrates:
 *   - Modal CRUD (intercept .js-vendor-create / .js-vendor-edit, fetch _VendorFormModal,
 *     submit via fetch, replace row on success / re-render modal on validation failure)
 *   - Debounced search → /Vendors/IndexPartial (replaces the table body only)
 *   - Reuses the global ChuaReactive helpers for fetch + CSRF + partial swap
 *
 * The other 8 master-data modules (customers, inventory, …) will be copied from this
 * file with the routes/IDs renamed. See the README / deferred-work note in the commit.
 */
(function () {
    'use strict';
    if (!window.ChuaReactive) { return; }
    if (typeof window.bootstrap === 'undefined') { return; }

    var container = document.getElementById('vendors-rows-container');
    var modalHost = document.getElementById('vendor-modal-host');
    if (!container || !modalHost) { return; }

    var rowsUrl = container.getAttribute('data-rows-url');
    var bsModal = null;

    // --- Modal lifecycle ------------------------------------------------------------
    function openModalFromUrl(url) {
        return fetch(url, {
            credentials: 'same-origin',
            headers: { 'X-Requested-With': 'XMLHttpRequest', 'Accept': 'text/html' }
        }).then(function (r) {
            if (!r.ok) { throw new Error('Modal load failed: ' + r.status); }
            return r.text();
        }).then(function (html) {
            modalHost.innerHTML = html;
            bsModal = bootstrap.Modal.getOrCreateInstance(modalHost);
            bsModal.show();
            // Re-bind jQuery Validation unobtrusive if present so the modal form validates client-side.
            if (window.jQuery && jQuery.validator && jQuery.validator.unobtrusive) {
                jQuery.validator.unobtrusive.parse(modalHost);
            }
        }).catch(function (err) {
            console.warn('chua-vendors: failed to open modal', err);
        });
    }

    function closeModal() {
        if (bsModal) { bsModal.hide(); }
    }

    // --- Form submit ---------------------------------------------------------------
    function submitModalForm(form) {
        var formData = new FormData(form);
        var token = formData.get('__RequestVerificationToken') || ChuaReactive.csrfToken();
        return fetch(form.action, {
            method: 'POST',
            credentials: 'same-origin',
            headers: {
                'X-Requested-With': 'XMLHttpRequest',
                'Accept': 'text/html',
                'RequestVerificationToken': token
            },
            body: formData
        }).then(function (response) {
            return response.text().then(function (html) {
                return { ok: response.ok, status: response.status, html: html, response: response };
            });
        });
    }

    function handleSubmitResult(result) {
        if (result.ok) {
            // The server returned a <tr>. Either replace the existing row by data-vendor-id
            // (edit) or prepend the new row (create).
            var tmp = document.createElement('tbody');
            tmp.innerHTML = (result.html || '').trim();
            var newRow = tmp.firstElementChild;
            var rowAction = result.response.headers.get('X-Chua-Row-Action') || 'update';
            var vendorId = result.response.headers.get('X-Chua-Vendor-Id') || (newRow && newRow.getAttribute('data-vendor-id'));
            if (newRow && vendorId) {
                var existing = document.getElementById('vendor-row-' + vendorId);
                var table = container.querySelector('table#vendors-table tbody');
                if (existing && existing.parentNode) {
                    existing.parentNode.replaceChild(newRow, existing);
                } else if (table) {
                    if (rowAction === 'create') { table.insertBefore(newRow, table.firstChild); }
                    else { table.appendChild(newRow); }
                } else {
                    // Empty-state was rendered — reload the whole rows partial.
                    refreshRows();
                }
            }
            closeModal();
        } else if (result.status === 422) {
            // Validation failed — swap the modal contents in place so the existing dialog
            // keeps focus and the user sees the validation messages.
            modalHost.innerHTML = result.html;
            if (window.jQuery && jQuery.validator && jQuery.validator.unobtrusive) {
                jQuery.validator.unobtrusive.parse(modalHost);
            }
        } else {
            console.warn('chua-vendors: unexpected submit status', result.status);
        }
    }

    // --- Filter / paging refresh ---------------------------------------------------
    function refreshRows(extraParams) {
        if (!rowsUrl) { return; }
        var url = rowsUrl;
        if (extraParams) {
            var qs = new URLSearchParams(extraParams).toString();
            if (qs) { url += (url.indexOf('?') === -1 ? '?' : '&') + qs; }
        }
        ChuaReactive.fetchPartial(url, container).catch(function (err) {
            console.warn('chua-vendors: refresh failed', err);
        });
    }

    function readSearchValue() {
        var input = document.querySelector('input[name="search"]');
        return input ? input.value : '';
    }

    // Debounced search box — works against either a <form> submitting GET or an isolated input.
    var searchInput = document.querySelector('input[name="search"]');
    if (searchInput) {
        var debounced = ChuaReactive.debounce(function () {
            refreshRows({ search: readSearchValue(), pageNumber: 1 });
        }, 300);
        searchInput.addEventListener('input', debounced);
        // Suppress the surrounding form's full-page submit so the debounced fetch wins.
        var form = searchInput.closest('form');
        if (form) {
            form.addEventListener('submit', function (e) {
                e.preventDefault();
                refreshRows({ search: readSearchValue(), pageNumber: 1 });
            });
        }
    }

    // --- Delegated click handlers --------------------------------------------------
    document.addEventListener('click', function (evt) {
        var createBtn = evt.target.closest('.js-vendor-create');
        if (createBtn && container.contains.call(document, createBtn)) {
            // Only intercept if the no-JS link points back to the create page — otherwise honour it.
            evt.preventDefault();
            var url = createBtn.getAttribute('data-modal-url') || '/Vendors/CreateModal';
            openModalFromUrl(url);
            return;
        }
        var editBtn = evt.target.closest('.js-vendor-edit');
        if (editBtn) {
            evt.preventDefault();
            var editUrl = editBtn.getAttribute('data-modal-url');
            if (editUrl) { openModalFromUrl(editUrl); }
            return;
        }
        // Pager links inside the rows partial: intercept and refresh just the rows.
        var pagerLink = evt.target.closest('#vendors-rows-container a[href*="pageNumber="]');
        if (pagerLink) {
            evt.preventDefault();
            var hrefUrl = new URL(pagerLink.href, window.location.origin);
            var params = {};
            hrefUrl.searchParams.forEach(function (value, key) { params[key] = value; });
            refreshRows(params);
        }
    });

    // Form submit inside the modal host — delegated because the form is injected dynamically.
    modalHost.addEventListener('submit', function (evt) {
        var form = evt.target.closest('#vendor-modal-form');
        if (!form) { return; }
        evt.preventDefault();
        submitModalForm(form).then(handleSubmitResult).catch(function (err) {
            console.warn('chua-vendors: submit failed', err);
        });
    });
})();
