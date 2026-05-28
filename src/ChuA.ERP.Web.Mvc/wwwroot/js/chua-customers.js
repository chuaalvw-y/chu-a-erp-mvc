// Copyright (c) 2026 Alvin Wilsen Chan Chua
// GitHub: chuaalvw-y
// Licensed under the Alvin Wilsen Chan Chua Proprietary Use-Only License.
// See LICENSE.txt in the project root for full license information.

/*
 * Customers module - reactive UX following the Vendors template:
 *   - Modal CRUD (intercept .js-customer-create / .js-customer-edit, fetch _CustomerFormModal,
 *     submit via fetch, replace row on success / re-render modal on validation failure)
 *   - Debounced search → /Customers/IndexPartial (replaces the table body only)
 *   - Reuses the global ChuaReactive helpers for fetch + CSRF + partial swap
 */
(function () {
    'use strict';
    if (!window.ChuaReactive) { return; }
    if (typeof window.bootstrap === 'undefined') { return; }

    var container = document.getElementById('customers-rows-container');
    var modalHost = document.getElementById('customer-modal-host');
    if (!container || !modalHost) { return; }

    var rowsUrl = container.getAttribute('data-rows-url');
    var bsModal = null;

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
            if (window.jQuery && jQuery.validator && jQuery.validator.unobtrusive) {
                jQuery.validator.unobtrusive.parse(modalHost);
            }
        }).catch(function (err) {
            console.warn('chua-customers: failed to open modal', err);
        });
    }

    function closeModal() { if (bsModal) { bsModal.hide(); } }

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
            var tmp = document.createElement('tbody');
            tmp.innerHTML = (result.html || '').trim();
            var newRow = tmp.firstElementChild;
            var rowAction = result.response.headers.get('X-Chua-Row-Action') || 'update';
            var customerId = result.response.headers.get('X-Chua-Customer-Id') || (newRow && newRow.getAttribute('data-customer-id'));
            if (newRow && customerId) {
                var existing = document.getElementById('customer-row-' + customerId);
                var table = container.querySelector('table#customers-table tbody');
                if (existing && existing.parentNode) {
                    existing.parentNode.replaceChild(newRow, existing);
                } else if (table) {
                    if (rowAction === 'create') { table.insertBefore(newRow, table.firstChild); }
                    else { table.appendChild(newRow); }
                } else {
                    refreshRows();
                }
            }
            closeModal();
        } else if (result.status === 422) {
            modalHost.innerHTML = result.html;
            if (window.jQuery && jQuery.validator && jQuery.validator.unobtrusive) {
                jQuery.validator.unobtrusive.parse(modalHost);
            }
        } else {
            console.warn('chua-customers: unexpected submit status', result.status);
        }
    }

    function refreshRows(extraParams) {
        if (!rowsUrl) { return; }
        var url = rowsUrl;
        if (extraParams) {
            var qs = new URLSearchParams(extraParams).toString();
            if (qs) { url += (url.indexOf('?') === -1 ? '?' : '&') + qs; }
        }
        ChuaReactive.fetchPartial(url, container).catch(function (err) {
            console.warn('chua-customers: refresh failed', err);
        });
    }

    function readSearchValue() {
        var input = document.querySelector('input[name="search"]');
        return input ? input.value : '';
    }

    var searchInput = document.querySelector('input[name="search"]');
    if (searchInput) {
        var debounced = ChuaReactive.debounce(function () {
            refreshRows({ search: readSearchValue(), pageNumber: 1 });
        }, 300);
        searchInput.addEventListener('input', debounced);
        var form = searchInput.closest('form');
        if (form) {
            form.addEventListener('submit', function (e) {
                e.preventDefault();
                refreshRows({ search: readSearchValue(), pageNumber: 1 });
            });
        }
    }

    document.addEventListener('click', function (evt) {
        var createBtn = evt.target.closest('.js-customer-create');
        if (createBtn) {
            evt.preventDefault();
            var url = createBtn.getAttribute('data-modal-url') || '/Customers/CreateModal';
            openModalFromUrl(url);
            return;
        }
        var editBtn = evt.target.closest('.js-customer-edit');
        if (editBtn) {
            evt.preventDefault();
            var editUrl = editBtn.getAttribute('data-modal-url');
            if (editUrl) { openModalFromUrl(editUrl); }
            return;
        }
        var pagerLink = evt.target.closest('#customers-rows-container a[href*="pageNumber="]');
        if (pagerLink) {
            evt.preventDefault();
            var hrefUrl = new URL(pagerLink.href, window.location.origin);
            var params = {};
            hrefUrl.searchParams.forEach(function (value, key) { params[key] = value; });
            refreshRows(params);
        }
    });

    modalHost.addEventListener('submit', function (evt) {
        var form = evt.target.closest('#customer-modal-form');
        if (!form) { return; }
        evt.preventDefault();
        submitModalForm(form).then(handleSubmitResult).catch(function (err) {
            console.warn('chua-customers: submit failed', err);
        });
    });
})();
