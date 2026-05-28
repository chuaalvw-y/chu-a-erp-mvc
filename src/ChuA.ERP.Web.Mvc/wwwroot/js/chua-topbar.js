// Copyright (c) 2026 Alvin Wilsen Chan Chua
// GitHub: chuaalvw-y
// Licensed under the Alvin Wilsen Chan Chua Proprietary Use-Only License.
// See LICENSE.txt in the project root for full license information.

/*
 * Topbar live updates — owns the workflow approvals badge and the notification center.
 * Subscribes to ChuaReactive events:
 *   - workflowInboxChanged: refreshes the approvals count
 *   - notificationReceived: refreshes the notification dropdown items + bell badge
 *
 * Renders correctly when the user is not authenticated (the corresponding partials are
 * empty in that case, so all the .getElementById() calls return null and the script
 * silently no-ops).
 */
(function () {
    'use strict';
    if (!window.ChuaReactive) { return; }

    var approvalsBadgeEl = document.getElementById('chua-approvals-badge');
    var approvalsCountEl = document.getElementById('chua-approvals-count');
    var notificationsRoot = document.getElementById('chua-notification-center');
    var notificationsBadge = document.getElementById('chua-notification-badge');
    var notificationsItemsContainer = document.getElementById('chua-notification-items');
    var notificationsMenu = notificationsRoot ? notificationsRoot.querySelector('.dropdown-menu') : null;
    var markAllBtn = document.getElementById('chua-mark-all-read');

    var FALLBACK_POLL_MS = 60000;

    function setBadgeCount(badgeEl, count) {
        if (!badgeEl) { return; }
        badgeEl.textContent = count;
        badgeEl.setAttribute('data-count', count);
        if (Number(count) > 0) {
            badgeEl.classList.remove('d-none');
        } else {
            badgeEl.classList.add('d-none');
        }
    }

    // --- Approvals badge ---
    function refreshApprovalsCount() {
        if (!approvalsBadgeEl) { return Promise.resolve(); }
        var url = approvalsBadgeEl.getAttribute('data-count-url');
        if (!url) { return Promise.resolve(); }
        return fetch(url, {
            credentials: 'same-origin',
            headers: { 'X-Requested-With': 'XMLHttpRequest', 'Accept': 'application/json' }
        }).then(function (r) { return r.ok ? r.json() : null; }).then(function (payload) {
            if (!payload) { return; }
            setBadgeCount(approvalsCountEl, payload.count);
        }).catch(function () { /* swallow */ });
    }

    if (approvalsBadgeEl) {
        ChuaReactive.subscribe('workflow-inbox');
        ChuaReactive.on('workflowInboxChanged', refreshApprovalsCount);
        // Initial load and slow fallback poll for cross-user cases.
        refreshApprovalsCount();
        setInterval(refreshApprovalsCount, FALLBACK_POLL_MS);
    }

    // --- Notification center ---
    function refreshNotificationItems() {
        if (!notificationsItemsContainer || !notificationsMenu) { return Promise.resolve(); }
        var url = notificationsMenu.getAttribute('data-items-url');
        if (!url) { return Promise.resolve(); }
        return ChuaReactive.fetchPartial(url, notificationsItemsContainer).catch(function () { /* swallow */ });
    }

    function refreshNotificationCount() {
        if (!notificationsMenu || !notificationsBadge) { return Promise.resolve(); }
        var url = notificationsMenu.getAttribute('data-count-url');
        if (!url) { return Promise.resolve(); }
        return fetch(url, {
            credentials: 'same-origin',
            headers: { 'X-Requested-With': 'XMLHttpRequest', 'Accept': 'application/json' }
        }).then(function (r) { return r.ok ? r.json() : null; }).then(function (payload) {
            if (!payload) { return; }
            setBadgeCount(notificationsBadge, payload.count);
        }).catch(function () { /* swallow */ });
    }

    function postWithCsrf(url) {
        var token = ChuaReactive.csrfToken();
        return fetch(url, {
            method: 'POST',
            credentials: 'same-origin',
            headers: {
                'X-Requested-With': 'XMLHttpRequest',
                'Accept': 'application/json',
                'RequestVerificationToken': token
            }
        });
    }

    if (notificationsRoot) {
        ChuaReactive.on('notificationReceived', function () {
            refreshNotificationItems();
            refreshNotificationCount();
        });
        // Lazy-load the first batch when the dropdown is first opened.
        var loaded = false;
        notificationsRoot.addEventListener('show.bs.dropdown', function () {
            if (!loaded) { loaded = true; refreshNotificationItems(); }
        });
        refreshNotificationCount();
        setInterval(refreshNotificationCount, FALLBACK_POLL_MS);

        // Per-item "mark read" buttons (delegated because items are partial-rendered).
        notificationsRoot.addEventListener('click', function (evt) {
            var btn = evt.target.closest('.js-mark-read');
            if (!btn) { return; }
            evt.preventDefault();
            var id = btn.getAttribute('data-notification-id');
            var url = (notificationsMenu.getAttribute('data-mark-read-url') || '') + '?id=' + encodeURIComponent(id);
            postWithCsrf(url).then(function (r) { return r.ok ? r.json() : null; }).then(function (payload) {
                if (!payload) { return; }
                refreshNotificationItems();
                setBadgeCount(notificationsBadge, payload.unreadCount);
            });
        });

        if (markAllBtn) {
            markAllBtn.addEventListener('click', function (evt) {
                evt.preventDefault();
                var url = notificationsMenu.getAttribute('data-mark-all-url');
                if (!url) { return; }
                postWithCsrf(url).then(function (r) { return r.ok ? r.json() : null; }).then(function (payload) {
                    if (!payload) { return; }
                    refreshNotificationItems();
                    setBadgeCount(notificationsBadge, 0);
                });
            });
        }
    }
})();
