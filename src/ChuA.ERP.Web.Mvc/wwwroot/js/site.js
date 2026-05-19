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
