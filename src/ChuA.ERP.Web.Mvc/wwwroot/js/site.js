// ChuA ERP – tiny client-side helpers
(function () {
    'use strict';
    document.addEventListener('DOMContentLoaded', function () {
        var toggle = document.getElementById('sidebarToggle');
        var sidebar = document.getElementById('sidebar');
        if (toggle && sidebar) {
            toggle.addEventListener('click', function () { sidebar.classList.toggle('show'); });
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
