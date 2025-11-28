// ---------------------------------------------
// Animal Care Clinic - Global JavaScript
// ---------------------------------------------
// This file is loaded on ALL pages (via _Layout.cshtml).
// It contains small UX enhancements only. No business logic here.
// ---------------------------------------------

// Ensure everything runs after the DOM is loaded
document.addEventListener('DOMContentLoaded', function () {

    // -----------------------------------------
    // 1. Auto-hide Bootstrap alerts after 5s
    // -----------------------------------------
    // Any alert with class .alert (except those with .alert-permanent)
    // will fade out automatically to keep the UI clean.
    setTimeout(function () {
        var alerts = document.querySelectorAll('.alert:not(.alert-permanent)');
        alerts.forEach(function (alert) {
            // Use Bootstrap's built-in dismissal
            if (typeof bootstrap !== "undefined") {
                var bsAlert = bootstrap.Alert.getOrCreateInstance(alert);
                bsAlert.close();
            } else {
                // Fallback: just hide
                alert.style.display = 'none';
            }
        });
    }, 5000); // 5 seconds



    // -----------------------------------------
    // 2. Add HTML5 validation styling (Bootstrap)
    // -----------------------------------------
    // Any form with class "needs-validation" will show Bootstrap's
    // validation styles when the user submits it.
    (function () {
        'use strict';

        var forms = document.getElementsByClassName('needs-validation');

        Array.prototype.forEach.call(forms, function (form) {
            form.addEventListener('submit', function (event) {
                if (!form.checkValidity()) {
                    // Prevent actual submit if invalid
                    event.preventDefault();
                    event.stopPropagation();

                    // Add Bootstrap's visual feedback class
                    form.classList.add('was-validated');
                }
                // If valid, form will submit normally - do nothing
            }, false);
        });
    })();



    // -----------------------------------------
    // 3. Prevent selecting past dates for appointments
    // -----------------------------------------
    // This is a light client-side validation. The real rules
    // are still enforced on the server in AppointmentsController.
    function initAppointmentDateMin() {
        // We look for any input of type="date" inside a form that
        // has data-appointment-date="true" (so it doesn't affect all forms).
        var dateInputs = document.querySelectorAll('form[data-appointment-date="true"] input[type="date"]');

        if (dateInputs.length === 0) return;

        var today = new Date().toISOString().split('T')[0];

        dateInputs.forEach(function (input) {
            // Set the minimum to today
            input.setAttribute('min', today);

            // If user somehow chooses a past date, reset it
            input.addEventListener('change', function () {
                if (input.value && input.value < today) {
                    alert('You cannot choose a date in the past for an appointment.');
                    input.value = today;
                }
            });
        });
    }

    initAppointmentDateMin();



    // -----------------------------------------
    // 4. Confirm delete helper
    // -----------------------------------------
    // Usage in Razor:
    // <form asp-action="Delete" onsubmit="return confirmDelete('owner');">
    // or
    // <a href="..." onclick="return confirmDelete('appointment');">
    // -----------------------------------------
    window.confirmDelete = function (entityName) {
        entityName = entityName || 'item';
        return confirm(`Are you sure you want to delete this ${entityName}? This action cannot be undone.`);
    };



    // -----------------------------------------
    // 5. Simple debounce utility (for future live search)
    // -----------------------------------------
    // debounce(fn, 300) will delay calling fn until 300ms
    // after the last call. You can reuse this in filters/search.
    window.debounce = function (func, wait) {
        let timeout;
        return function executedFunction() {
            const context = this;
            const args = arguments;

            const later = function () {
                clearTimeout(timeout);
                func.apply(context, args);
            };

            clearTimeout(timeout);
            timeout = setTimeout(later, wait);
        };
    };



    // -----------------------------------------
    // 6. Optional: highlight current nav link
    // -----------------------------------------
    // Adds 'active' class to the navbar link that matches
    // the current URL path.
    (function highlightActiveNav() {
        var path = window.location.pathname.toLowerCase();
        var navLinks = document.querySelectorAll('.navbar-nav .nav-link');

        navLinks.forEach(function (link) {
            var href = link.getAttribute('href');

            if (!href) return;

            // Compare lowercased paths without query string
            var linkPath = href.split('?')[0].toLowerCase();

            if (linkPath !== '/' && path.startsWith(linkPath)) {
                link.classList.add('active');
            }
        });
    })();



    // -----------------------------------------
    // 7. Loading spinner on submit (for main forms)
    // -----------------------------------------
    // You can add the class "btn-with-spinner" to any submit button,
    // and inside it add:
    //   <span class="spinner-border spinner-border-sm d-none" ...></span>
    //   <span class="button-text">Save</span>
    //
    // The spinner will be shown when the form submits.
    (function enableSubmitSpinner() {
        var forms = document.querySelectorAll('form');

        forms.forEach(function (form) {
            form.addEventListener('submit', function () {
                var btn = form.querySelector('button[type="submit"].btn-with-spinner');
                if (!btn) return;

                var spinner = btn.querySelector('.spinner-border');
                var text = btn.querySelector('.button-text');

                if (spinner) {
                    spinner.classList.remove('d-none');
                }
                if (text) {
                    text.textContent = 'Saving...';
                }

                btn.disabled = true;
            });
        });
    })();

});
