// Copyright (c) 2026 Alvin Wilsen Chan Chua
// GitHub: chuaalvw-y
// Licensed under the Alvin Wilsen Chan Chua Proprietary Use-Only License.
// See LICENSE.txt in the project root for full license information.

/*
 * Chart of Accounts module - reactive UX following the Vendors template, with the extra
 * accountType filter threaded alongside the search box.
 */
(function () {
    'use strict';
    if (!window.ChuaReactive) { return; }
    if (typeof window.bootstrap === 'undefined') { return; }

    var container = document.getElementById('coa-rows-container');
    var modalHost = document.getElementById('coa-modal-host');
    if (!container || !modalHost) { return; }

    var rowsUrl = container.getAttribute('data-rows-url');
    var filterForm = document.getElementById('coa-filter-form');
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
            console.warn('chua-coa: failed to open modal', err);
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
            var coaId = result.response.headers.get('X-Chua-Coa-Id') || (newRow && newRow.getAttribute('data-coa-id'));
            if (newRow && coaId) {
                var existing = document.getElementById('coa-row-' + coaId);
                var table = container.querySelector('table#coa-table tbody');
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
            console.warn('chua-coa: unexpected submit status', result.status);
        }
    }

    function currentFilterParams(overrides) {
        var params = { pageNumber: 1 };
        if (filterForm) {
            var search = filterForm.querySelector('input[name="search"]');
            var typeSel = filterForm.querySelector('select[name="accountType"]');
            if (search) { params.search = search.value; }
            if (typeSel) { params.accountType = typeSel.value; }
        }
        if (overrides) {
            Object.keys(overrides).forEach(function (k) { params[k] = overrides[k]; });
        }
        return params;
    }

    function refreshRows(extraParams) {
        if (!rowsUrl) { return; }
        var url = rowsUrl;
        var qs = new URLSearchParams(currentFilterParams(extraParams)).toString();
        if (qs) { url += (url.indexOf('?') === -1 ? '?' : '&') + qs; }
        ChuaReactive.fetchPartial(url, container).catch(function (err) {
            console.warn('chua-coa: refresh failed', err);
        });
    }

    if (filterForm) {
        var searchInput = filterForm.querySelector('input[name="search"]');
        var typeSelect = filterForm.querySelector('select[name="accountType"]');

        if (searchInput) {
            var debounced = ChuaReactive.debounce(function () { refreshRows(); }, 300);
            searchInput.addEventListener('input', debounced);
        }
        if (typeSelect) {
            typeSelect.addEventListener('change', function () { refreshRows(); });
        }
        filterForm.addEventListener('submit', function (e) {
            e.preventDefault();
            refreshRows();
        });
    }

    document.addEventListener('click', function (evt) {
        var createBtn = evt.target.closest('.js-coa-create');
        if (createBtn) {
            evt.preventDefault();
            var url = createBtn.getAttribute('data-modal-url') || '/ChartOfAccounts/CreateModal';
            openModalFromUrl(url);
            return;
        }
        var editBtn = evt.target.closest('.js-coa-edit');
        if (editBtn) {
            evt.preventDefault();
            var editUrl = editBtn.getAttribute('data-modal-url');
            if (editUrl) { openModalFromUrl(editUrl); }
            return;
        }
        var pagerLink = evt.target.closest('#coa-rows-container a[href*="pageNumber="]');
        if (pagerLink) {
            evt.preventDefault();
            var hrefUrl = new URL(pagerLink.href, window.location.origin);
            var overrides = {};
            hrefUrl.searchParams.forEach(function (value, key) { overrides[key] = value; });
            refreshRows(overrides);
        }
    });

    modalHost.addEventListener('submit', function (evt) {
        var form = evt.target.closest('#coa-modal-form');
        if (!form) { return; }
        evt.preventDefault();
        submitModalForm(form).then(handleSubmitResult).catch(function (err) {
            console.warn('chua-coa: submit failed', err);
        });
    });
})();
