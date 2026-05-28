// Copyright (c) 2026 Alvin Wilsen Chan Chua
// GitHub: chuaalvw-y
// Licensed under the Alvin Wilsen Chan Chua Proprietary Use-Only License.
// See LICENSE.txt in the project root for full license information.

/*
 * ChuaReactive — small, opinionated reactive helper for the ChuA ERP MVC shell.
 *
 * Design goals:
 *   - One persistent SignalR connection per page (singleton, lazily started)
 *   - Plain DOM event-style on/off API so module JS does not need to know
 *     about the underlying signalR client object
 *   - fetchPartial(url, target) — XHR replacement for full-page reloads,
 *     preserving the surrounding shell (sidebar, breadcrumbs, etc.)
 *   - debounce(fn, ms) — used by search/filter wiring
 *   - Antiforgery token attached automatically on non-GET fetches
 *
 * The page should include /lib/signalr/signalr.min.js before this file.
 */
(function (global) {
    'use strict';

    var HUB_PATH = '/hubs/erp';
    var ANTIFORGERY_HEADER = 'RequestVerificationToken';

    /** Read the MVC antiforgery token from the rendered <input name="__RequestVerificationToken">. */
    function csrfToken() {
        var input = document.querySelector('input[name="__RequestVerificationToken"]');
        return input ? input.value : '';
    }

    /** Debounces calls to {fn} so it only fires {ms} ms after the most recent invocation. */
    function debounce(fn, ms) {
        var t = null;
        return function () {
            var ctx = this;
            var args = arguments;
            if (t) { clearTimeout(t); }
            t = setTimeout(function () {
                t = null;
                fn.apply(ctx, args);
            }, ms);
        };
    }

    /**
     * Fetch a Razor partial via XHR and replace the contents of {target} with the response HTML.
     * Sets X-Requested-With so the controller can return a partial via Controller.PartialOrView().
     */
    function fetchPartial(url, target, options) {
        options = options || {};
        if (typeof target === 'string') {
            target = document.querySelector(target);
        }
        if (!target) {
            return Promise.reject(new Error('chua-reactive.fetchPartial: target not found'));
        }
        var headers = Object.assign({
            'X-Requested-With': 'XMLHttpRequest',
            'Accept': 'text/html,application/xhtml+xml'
        }, options.headers || {});
        if (options.partial) { headers['X-Partial'] = options.partial; }
        var method = (options.method || 'GET').toUpperCase();
        if (method !== 'GET' && method !== 'HEAD') {
            var token = csrfToken();
            if (token) { headers[ANTIFORGERY_HEADER] = token; }
        }
        return fetch(url, {
            method: method,
            credentials: 'same-origin',
            headers: headers,
            body: options.body
        }).then(function (response) {
            if (!response.ok) {
                throw Object.assign(new Error('Partial fetch failed: ' + response.status), { response: response });
            }
            return response.text();
        }).then(function (html) {
            target.innerHTML = html;
            // Allow page JS to re-bind into the swapped-in fragment.
            target.dispatchEvent(new CustomEvent('chua:partial-loaded', { bubbles: true, detail: { url: url } }));
            return target;
        });
    }

    /**
     * Fetch and replace a single row by id ({rowId}) using a partial endpoint that returns
     * a <tr>. Convenience over fetchPartial for the grid-row-update pattern.
     */
    function fetchRow(url, rowId, options) {
        options = options || {};
        var target = typeof rowId === 'string' ? document.getElementById(rowId) : rowId;
        if (!target) { return Promise.reject(new Error('chua-reactive.fetchRow: row not found')); }
        return fetchPartial(url, target.parentNode, Object.assign({}, options, { replaceRowId: rowId })).then(function () {
            // fetchPartial replaces the parent's innerHTML; for rows we want to replace only
            // the row. Re-do it the proper way:
            return fetch(url, {
                method: (options.method || 'GET').toUpperCase(),
                credentials: 'same-origin',
                headers: { 'X-Requested-With': 'XMLHttpRequest', 'Accept': 'text/html' }
            }).then(function (r) { return r.text(); }).then(function (html) {
                var tmp = document.createElement('tbody');
                tmp.innerHTML = html.trim();
                var newRow = tmp.firstElementChild;
                if (newRow && target.parentNode) {
                    target.parentNode.replaceChild(newRow, target);
                    newRow.dispatchEvent(new CustomEvent('chua:row-updated', { bubbles: true, detail: { url: url } }));
                }
            });
        });
    }

    /**
     * POST a <form> via fetch and either replace the form contents with the response (when the
     * server returns validation HTML) or invoke {onSuccess} when the server returns a 204 / a
     * non-HTML success signal.
     */
    function submitForm(form, options) {
        options = options || {};
        if (typeof form === 'string') { form = document.querySelector(form); }
        if (!form) { return Promise.reject(new Error('chua-reactive.submitForm: form not found')); }

        var formData = new FormData(form);
        var headers = {
            'X-Requested-With': 'XMLHttpRequest',
            'Accept': 'text/html,application/json'
        };
        // Hidden field __RequestVerificationToken is already in formData; also surface it as
        // a header for endpoints configured with AddAntiforgery(o => o.HeaderName = ...).
        var token = formData.get('__RequestVerificationToken') || csrfToken();
        if (token) { headers[ANTIFORGERY_HEADER] = token; }

        return fetch(form.action, {
            method: (form.method || 'POST').toUpperCase(),
            credentials: 'same-origin',
            headers: headers,
            body: formData
        }).then(function (response) {
            var contentType = response.headers.get('Content-Type') || '';
            if (response.ok && contentType.indexOf('application/json') !== -1) {
                return response.json().then(function (payload) {
                    return { ok: true, json: payload, response: response };
                });
            }
            return response.text().then(function (html) {
                return { ok: response.ok, html: html, response: response, status: response.status };
            });
        });
    }

    // --- SignalR singleton ---
    var connection = null;
    var connectionStarted = null;
    var listeners = {};
    var subscribedTopics = {};

    function ensureConnection() {
        if (connection) { return connectionStarted; }
        if (typeof global.signalR === 'undefined') {
            console.warn('chua-reactive: signalR client library not loaded');
            return Promise.reject(new Error('signalR client not loaded'));
        }
        connection = new global.signalR.HubConnectionBuilder()
            .withUrl(HUB_PATH)
            .withAutomaticReconnect()
            .build();
        connection.onreconnected(function () {
            // Re-subscribe to topics after a reconnect — the server's groups are wiped on disconnect.
            Object.keys(subscribedTopics).forEach(function (topic) {
                connection.invoke('SubscribeToTopicAsync', topic).catch(function () { /* swallow */ });
            });
        });
        connectionStarted = connection.start().then(function () {
            return connection;
        }).catch(function (err) {
            console.warn('chua-reactive: SignalR connection failed', err);
            connection = null;
            connectionStarted = null;
            throw err;
        });
        return connectionStarted;
    }

    /** Register {handler} for the named server event. */
    function on(eventName, handler) {
        if (!eventName || typeof handler !== 'function') { return; }
        listeners[eventName] = listeners[eventName] || [];
        listeners[eventName].push(handler);
        ensureConnection().then(function (conn) {
            // Bind once per event name; the dispatch loop handles multi-listener fan-out.
            if (listeners[eventName].length === 1) {
                conn.on(eventName, function () {
                    var args = Array.prototype.slice.call(arguments);
                    (listeners[eventName] || []).forEach(function (h) {
                        try { h.apply(null, args); }
                        catch (e) { console.error('chua-reactive: handler for ' + eventName + ' threw', e); }
                    });
                });
            }
        }).catch(function () { /* connection failure already logged */ });
    }

    /** Unregister a previously registered handler. */
    function off(eventName, handler) {
        if (!eventName || !listeners[eventName]) { return; }
        listeners[eventName] = listeners[eventName].filter(function (h) { return h !== handler; });
    }

    /** Ask the hub to add this connection to a topic group. */
    function subscribe(topic) {
        if (!topic) { return Promise.resolve(); }
        subscribedTopics[topic] = true;
        return ensureConnection().then(function (conn) {
            return conn.invoke('SubscribeToTopicAsync', topic);
        }).catch(function () { /* already logged */ });
    }

    /** Remove this connection from a topic group. */
    function unsubscribe(topic) {
        if (!topic) { return Promise.resolve(); }
        delete subscribedTopics[topic];
        return ensureConnection().then(function (conn) {
            return conn.invoke('UnsubscribeFromTopicAsync', topic);
        }).catch(function () { /* already logged */ });
    }

    global.ChuaReactive = {
        csrfToken: csrfToken,
        debounce: debounce,
        fetchPartial: fetchPartial,
        fetchRow: fetchRow,
        submitForm: submitForm,
        connect: ensureConnection,
        on: on,
        off: off,
        subscribe: subscribe,
        unsubscribe: unsubscribe,
        HUB_PATH: HUB_PATH
    };
})(window);
