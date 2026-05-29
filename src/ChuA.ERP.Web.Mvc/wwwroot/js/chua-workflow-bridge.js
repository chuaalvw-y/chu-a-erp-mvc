// Copyright (c) 2026 Alvin Wilsen Chan Chua
// GitHub: chuaalvw-y
// Licensed under the Alvin Wilsen Chan Chua Proprietary Use-Only License.
// See LICENSE.txt in the project root for full license information.

/*
 * Workflow hub bridge — opens a SECOND SignalR connection (in addition to the MVC's local
 * /hubs/erp connection owned by chua-reactive.js) targeting the API's /hubs/workflow.
 * Cross-origin, so it uses an accessTokenFactory that calls a same-origin MVC endpoint
 * that surfaces the OIDC cookie-saved access_token.
 *
 * Server events forwarded into ChuaReactive's local event bus so the rest of the page
 * stack (chua-topbar.js, the workflow inbox script) can listen via the same
 * ChuaReactive.on(...) API regardless of which hub the event originated on:
 *
 *   API → workflowInboxChanged    →  ChuaReactive.dispatch('workflowInboxChanged', ...)
 *   API → workflowApprovalAssigned →  ChuaReactive.dispatch('workflowApprovalAssigned', ...)
 *                                   →  shows a Bootstrap toast (for the cross-user "you have new work" case)
 *
 * Config comes from window.ChuaReactiveConfig (rendered in _Layout.cshtml):
 *   workflowHubUrl  — absolute URL of the API hub (https://localhost:53740/hubs/workflow)
 *   signalRTokenUrl — same-origin URL on the MVC that returns { token: "..." }
 *
 * Failures are non-fatal: if the API is offline or auth fails the bridge logs a warning
 * and the page falls back to the slow-poll fallbacks already present in chua-topbar.js.
 */
(function () {
    'use strict';
    if (!window.ChuaReactive) { return; }
    if (typeof window.signalR === 'undefined') {
        console.warn('chua-workflow-bridge: signalR client library not loaded');
        return;
    }

    var cfg = window.ChuaReactiveConfig || {};
    if (!cfg.workflowHubUrl) {
        // Bridge intentionally not configured (e.g. anonymous page, missing Dashboard
        // base URL). No-op silently.
        return;
    }

    var connection = null;

    function fetchAccessToken() {
        if (!cfg.signalRTokenUrl) { return Promise.resolve(''); }
        return fetch(cfg.signalRTokenUrl, {
            credentials: 'same-origin',
            headers: { 'X-Requested-With': 'XMLHttpRequest', 'Accept': 'application/json' }
        }).then(function (r) {
            return r.ok ? r.json() : null;
        }).then(function (payload) {
            return (payload && payload.token) || '';
        }).catch(function () { return ''; });
    }

    function showAssignmentToast(payload) {
        if (typeof window.bootstrap === 'undefined') { return; }
        var container = document.getElementById('chua-toast-container');
        if (!container) {
            container = document.createElement('div');
            container.id = 'chua-toast-container';
            container.className = 'toast-container position-fixed top-0 end-0 p-3';
            container.style.zIndex = '1080';
            document.body.appendChild(container);
        }
        var stepText = (payload && payload.stepNumber) ? (' (step ' + payload.stepNumber + ')') : '';
        var toastEl = document.createElement('div');
        toastEl.className = 'toast align-items-center text-bg-warning border-0';
        toastEl.setAttribute('role', 'status');
        toastEl.setAttribute('aria-live', 'polite');
        toastEl.setAttribute('aria-atomic', 'true');
        toastEl.innerHTML =
            '<div class="d-flex">' +
            '  <div class="toast-body">' +
            '    <i class="bi bi-inboxes me-2"></i>' +
            '    A new approval was assigned to you' + stepText + '.' +
            '  </div>' +
            '  <button type="button" class="btn-close me-2 m-auto" data-bs-dismiss="toast" aria-label="Close"></button>' +
            '</div>';
        container.appendChild(toastEl);
        var toast = new window.bootstrap.Toast(toastEl, { autohide: true, delay: 6000 });
        toastEl.addEventListener('hidden.bs.toast', function () { toastEl.remove(); });
        toast.show();
    }

    function start() {
        connection = new window.signalR.HubConnectionBuilder()
            .withUrl(cfg.workflowHubUrl, {
                // accessTokenFactory is invoked once per negotiate + on every WebSocket
                // reconnect, so token refresh is transparent.
                accessTokenFactory: fetchAccessToken
            })
            .withAutomaticReconnect()
            .build();

        connection.on('workflowInboxChanged', function (payload) {
            ChuaReactive.dispatch && ChuaReactive.dispatch('workflowInboxChanged', payload);
        });

        connection.on('workflowApprovalAssigned', function (payload) {
            ChuaReactive.dispatch && ChuaReactive.dispatch('workflowApprovalAssigned', payload);
            // Workflow inbox row count changed too — refresh badge.
            ChuaReactive.dispatch && ChuaReactive.dispatch('workflowInboxChanged', payload);
            showAssignmentToast(payload);
        });

        connection.onreconnecting(function () {
            console.debug('chua-workflow-bridge: reconnecting to API hub');
        });
        connection.onreconnected(function () {
            console.debug('chua-workflow-bridge: reconnected to API hub');
        });

        connection.start().catch(function (err) {
            console.warn('chua-workflow-bridge: failed to connect to API workflow hub', err);
            connection = null;
        });
    }

    start();
})();
