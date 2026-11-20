(function () {
    'use strict';

    var toggle = document.getElementById('sidebarToggle');
    var sidebar = document.getElementById('sidebar');

    if (toggle && sidebar) {
        toggle.addEventListener('click', function () {
            if (window.innerWidth <= 991) {
                sidebar.classList.toggle('show');
            } else {
                sidebar.classList.toggle('collapsed');
            }
        });
    }

    document.querySelectorAll('form[data-confirm]').forEach(function (form) {
        form.addEventListener('submit', function (event) {
            if (!window.confirm(form.getAttribute('data-confirm') || '¿Continuar?')) {
                event.preventDefault();
            }
        });
    });

    document.querySelectorAll('[data-table-filter]').forEach(function (input) {
        input.addEventListener('input', function () {
            var target = document.getElementById(input.getAttribute('data-table-filter'));
            if (!target) {
                return;
            }
            var term = input.value.toLowerCase();
            target.querySelectorAll('tbody tr').forEach(function (row) {
                row.style.display = row.textContent.toLowerCase().indexOf(term) >= 0 ? '' : 'none';
            });
        });
    });
})();
