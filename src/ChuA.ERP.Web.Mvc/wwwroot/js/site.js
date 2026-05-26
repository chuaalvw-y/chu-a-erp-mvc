// Copyright (c) 2026 Alvin Wilsen Chan Chua
// GitHub: chuaalvw-y
// Licensed under the Alvin Wilsen Chan Chua Proprietary Use-Only License.
// See LICENSE.txt in the project root for full license information.

// ChuA ERP – tiny client-side helpers
(function () {
    'use strict';
    document.addEventListener('DOMContentLoaded', function () {
        var toggle = document.getElementById('sidebarToggle');
        var sidebar = document.getElementById('sidebar');
        if (toggle && sidebar) {
            var setSidebarVisible = function (visible) {
                sidebar.classList.toggle('show', visible);
                toggle.setAttribute('aria-expanded', visible ? 'true' : 'false');
                if (visible) { sidebar.focus(); }
            };

            toggle.addEventListener('click', function () {
                setSidebarVisible(!sidebar.classList.contains('show'));
            });

            document.addEventListener('keydown', function (e) {
                if (e.key === 'Escape' && sidebar.classList.contains('show')) {
                    setSidebarVisible(false);
                    toggle.focus();
                }
            });

            document.addEventListener('click', function (e) {
                if (!sidebar.classList.contains('show')) { return; }
                if (sidebar.contains(e.target) || toggle.contains(e.target)) { return; }
                setSidebarVisible(false);
            });
        }

        // Sidebar section collapse toggles (Purchasing / Sales / Inventory /
        // Finance / Workflow / Reports). These USE Bootstrap's Collapse plugin,
        // but we bind the click handler ourselves rather than rely on the
        // data API. Same-page elements (Account dropdown, toasts) confirm the
        // data API is alive, yet clicks on the nav section buttons were being
        // silently swallowed somewhere up the chain — owning the binding here
        // makes the collapse deterministic and avoids the "menu won't collapse"
        // regression noted on the Phase J pass.
        document.querySelectorAll('.nav-section-collapse-toggle').forEach(function (toggle) {
            var selector = toggle.getAttribute('data-bs-target');
            if (!selector) { return; }
            var target = document.querySelector(selector);
            if (!target) { return; }

            // Take this button off the data API so we don't double-fire.
            toggle.removeAttribute('data-bs-toggle');

            var instance = (window.bootstrap && window.bootstrap.Collapse)
                ? window.bootstrap.Collapse.getOrCreateInstance(target, { toggle: false })
                : null;

            toggle.addEventListener('click', function () {
                if (instance) {
                    instance.toggle();
                } else {
                    // Bootstrap JS not loaded — fall back to a manual class flip so the
                    // menu still works (no animation, but functional).
                    target.classList.toggle('show');
                }
                toggle.setAttribute('aria-expanded',
                    target.classList.contains('show') ? 'true' : 'false');
            });
        });

        // Auto-dismiss toasts after 5s
        document.querySelectorAll('.toast.show').forEach(function (t) {
            setTimeout(function () { t.classList.remove('show'); }, 5000);
        });

        // Confirm dialogs via data-confirm="..."
        document.querySelectorAll('form[data-confirm]').forEach(function (form) {
            form.addEventListener('submit', function (e) {
                var msg = form.getAttribute('data-confirm');
                if (!confirm(msg)) { e.preventDefault(); }
            });
        });
    });
})();
