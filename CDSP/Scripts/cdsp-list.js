/*
 * CDSP Submissions admin — list behavior.
 * AJAX status toggle (shared by the row checkbox and the modal button),
 * SweetAlert2 confirms/toasts, AJAX detail modal, and DataTables wiring.
 */
(function () {
    "use strict";

    var TBL = '#tblSubmissions';
    var SHOW_KEY = 'cdsp-showCompleted';
    var filterInstalled = false;

    function ctx() { return window.__cdspCtx || {}; }

    function token() {
        var el = document.querySelector('input[name=__RequestVerificationToken]');
        return el ? el.value : '';
    }

    // Shared fetch helper — sends the DNN module + anti-forgery headers.
    function api(path, method, body) {
        var c = ctx();
        var opts = {
            method: method,
            headers: {
                'ModuleId': c.moduleId,
                'TabId': c.tabId,
                'RequestVerificationToken': token()
            }
        };
        if (body !== undefined) {
            opts.headers['Content-Type'] = 'application/json';
            opts.body = JSON.stringify(body);
        }
        return fetch(c.serviceRoot + path, opts).then(function (r) {
            if (!r.ok) throw new Error('HTTP ' + r.status);
            var ct = r.headers.get('content-type') || '';
            return ct.indexOf('application/json') !== -1 ? r.json() : null;
        });
    }

    function dataTable() {
        var $ = window.jQuery;
        if ($ && $.fn.DataTable && $.fn.DataTable.isDataTable(TBL)) return $(TBL).DataTable();
        return null;
    }

    // ---- status toggle (the one call shared by checkbox + modal button) ----

    function setCompleted(id, newCompleted, onSuccess) {
        return api('Submissions/SetCompleted', 'POST', { id: id, completed: newCompleted })
            .then(function () {
                updateRow(id, newCompleted);
                syncModalButton(id, newCompleted);
                if (typeof onSuccess === 'function') onSuccess();
                if (window.Swal) {
                    Swal.fire({
                        toast: true, position: 'top-end', showConfirmButton: false,
                        timer: 2200, timerProgressBar: true, icon: 'success',
                        title: newCompleted ? 'Marked completed' : 'Marked open'
                    });
                }
            })
            .catch(function () {
                if (window.Swal) {
                    Swal.fire({ icon: 'error', title: 'Update failed', text: 'Could not update the submission status. Please try again.' });
                }
            });
    }

    function confirmToggle(id, currentCompleted, onSuccess) {
        var target = !currentCompleted;
        if (!window.Swal) {
            if (window.confirm('Update the completed status for this submission?')) setCompleted(id, target, onSuccess);
            return;
        }
        Swal.fire({
            title: target ? 'Mark as completed?' : 'Reopen this submission?',
            icon: 'question',
            showCancelButton: true,
            confirmButtonText: target ? 'Yes, complete it' : 'Yes, reopen it',
            cancelButtonText: 'Cancel'
        }).then(function (res) { if (res.isConfirmed) setCompleted(id, target, onSuccess); });
    }

    function updateRow(id, completed) {
        var $ = window.jQuery;
        var $row = $(TBL + ' tr[data-id="' + id + '"]');
        if (!$row.length) return;
        $row.attr('data-completed', completed ? '1' : '0');
        $row.find('.cdsp-toggle i').attr('class', completed ? 'fas fa-check-square text-success' : 'far fa-square text-muted');
        $row.find('.cdsp-status').html(completed
            ? '<span class="badge bg-success">Completed</span>'
            : '<span class="badge bg-warning text-dark">Open</span>');
        var dt = dataTable();
        if (dt) dt.row($row).invalidate('dom').draw(false); // re-apply the hide-completed filter, keep page
    }

    // ---- detail modal ----

    function showModal() {
        var el = document.getElementById('cdspDetailModal');
        if (!el) return;
        if (window.bootstrap && window.bootstrap.Modal) { window.bootstrap.Modal.getOrCreateInstance(el).show(); return; }
        if (window.jQuery && window.jQuery.fn.modal) { window.jQuery(el).modal('show'); return; }
        el.classList.add('show'); el.style.display = 'block';
    }

    function hideModal() {
        var el = document.getElementById('cdspDetailModal');
        if (!el) return;
        if (window.bootstrap && window.bootstrap.Modal) { window.bootstrap.Modal.getOrCreateInstance(el).hide(); return; }
        if (window.jQuery && window.jQuery.fn.modal) { window.jQuery(el).modal('hide'); return; }
        el.classList.remove('show'); el.style.display = 'none';
    }

    function syncModalButton(id, completed) {
        var btn = document.getElementById('cdspModalToggle');
        if (!btn || String(btn.getAttribute('data-id')) !== String(id)) return;
        btn.setAttribute('data-completed', completed ? '1' : '0');
        btn.className = 'btn ' + (completed ? 'btn-warning' : 'btn-success') + ' cdsp-modal-toggle';
        btn.innerHTML = completed
            ? '<i class="far fa-square"></i>&nbsp;Mark as Open'
            : '<i class="fas fa-check-square"></i>&nbsp;Set Completed';
    }

    function openDetail(id) {
        var body = document.getElementById('cdspDetailBody');
        var btn = document.getElementById('cdspModalToggle');
        if (body) body.innerHTML = '<div class="text-center text-muted cdsp-loading">Loading…</div>';
        if (btn) { btn.setAttribute('data-id', id); btn.style.visibility = 'hidden'; }
        showModal();
        api('Submissions/Get?id=' + encodeURIComponent(id), 'GET')
            .then(function (data) {
                if (!data) { if (body) body.innerHTML = '<div class="alert alert-warning">Submission not found.</div>'; return; }
                if (body) body.innerHTML = data.html;
                if (btn) { btn.style.visibility = ''; syncModalButton(data.id, data.completed); }
            })
            .catch(function () {
                if (body) body.innerHTML = '<div class="alert alert-danger">Could not load the submission.</div>';
            });
    }

    // ---- DataTables + hide-completed filter ----

    function initTable() {
        var $ = window.jQuery;
        if (!$ || !$.fn.DataTable) return;

        if ($(TBL).length && !$.fn.DataTable.isDataTable(TBL)) {
            $(TBL).DataTable({
                order: [[1, 'desc']],
                pageLength: 25,
                stateSave: true,
                stateDuration: 60 * 60 * 24,
                columnDefs: [{ orderable: false, targets: 'no-sort' }]
            });
        }

        var $toggle = $('#cdspShowCompleted');
        if ($toggle.length && !filterInstalled) {
            filterInstalled = true;
            try { if (localStorage.getItem(SHOW_KEY) === '1') $toggle.prop('checked', true); } catch (e) { }

            $.fn.dataTable.ext.search.push(function (settings, data, dataIndex) {
                if (settings.nTable.id !== 'tblSubmissions') return true;
                var row = settings.aoData[dataIndex].nTr;
                if (!row) return true;
                if ($toggle.is(':checked')) return true;           // show everything
                return row.getAttribute('data-completed') !== '1';  // hide completed
            });

            $toggle.on('change', function () {
                try { localStorage.setItem(SHOW_KEY, $toggle.is(':checked') ? '1' : '0'); } catch (e) { }
                var dt = dataTable(); if (dt) dt.draw(false);
            });
        }

        var dt = dataTable(); if (dt) dt.draw(false);
    }

    function wire() {
        var $ = window.jQuery;
        if (!$) return;

        $(document).on('click', TBL + ' .cdsp-view', function (e) {
            e.preventDefault();
            openDetail($(this).data('id'));
        });

        $(document).on('click', TBL + ' .cdsp-toggle', function (e) {
            e.preventDefault();
            var $row = $(this).closest('tr');
            confirmToggle($(this).data('id'), $row.attr('data-completed') === '1');
        });

        // Modal button toggles status immediately (no confirm prompt), then
        // closes the modal. (The row checkbox still confirms via Swal.)
        $(document).on('click', '#cdspModalToggle', function (e) {
            e.preventDefault();
            var current = this.getAttribute('data-completed') === '1';
            setCompleted(parseInt(this.getAttribute('data-id'), 10), !current, hideModal);
        });
    }

    if (window.jQuery) {
        window.jQuery(function () { wire(); initTable(); });
    }
})();
