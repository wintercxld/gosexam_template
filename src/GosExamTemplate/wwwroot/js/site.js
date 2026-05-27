(function () {
    'use strict';

    function initToasts() {
        if (typeof bootstrap === 'undefined' || !bootstrap.Toast) {
            return;
        }
        document.querySelectorAll('.app-toast').forEach(function (el) {
            try {
                bootstrap.Toast.getOrCreateInstance(el).show();
            } catch (err) {
                console.error('Toast init failed', err);
            }
        });
    }

    function bindSubmitSpinners() {
        document.querySelectorAll('form[data-disable-on-submit]').forEach(function (form) {
            form.addEventListener('submit', function () {
                form.querySelectorAll('button[type="submit"]').forEach(function (btn) {
                    btn.disabled = true;
                    var spinner = document.createElement('span');
                    spinner.className = 'spinner-border spinner-border-sm me-1';
                    spinner.setAttribute('role', 'status');
                    spinner.setAttribute('aria-hidden', 'true');
                    btn.prepend(spinner);
                });
            });
        });
    }

    function autoSubmitFilters() {
        document.querySelectorAll('form[data-auto-submit]').forEach(function (form) {
            var timer = null;
            var triggerSubmit = function () {
                var pageInput = form.querySelector('input[name="Page"]');
                if (pageInput) {
                    pageInput.value = '1';
                }
                window.clearTimeout(timer);
                timer = window.setTimeout(function () { form.submit(); }, 350);
            };
            form.querySelectorAll('input[type="search"], input[type="text"]').forEach(function (input) {
                input.addEventListener('input', triggerSubmit);
            });
            form.querySelectorAll('select, input[type="date"], input[type="number"]').forEach(function (input) {
                input.addEventListener('change', function () { form.submit(); });
            });
        });
    }

    function setupConfirmLinks() {
        document.querySelectorAll('[data-confirm]').forEach(function (el) {
            el.addEventListener('click', function (e) {
                var message = el.getAttribute('data-confirm') || 'Подтвердите действие';
                if (!window.confirm(message)) {
                    e.preventDefault();
                    e.stopPropagation();
                }
            });
        });
    }

    document.addEventListener('DOMContentLoaded', function () {
        initToasts();
        bindSubmitSpinners();
        autoSubmitFilters();
        setupConfirmLinks();
    });
}());
