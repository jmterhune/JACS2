/* EEO Setup — API-driven CRUD for the EEO List tab.
 *
 * Layered on top of empdb-edit.js for empdb.api / notify / confirmDelete /
 * showModal / hideModal. The EEO row carries 5 categories (Population,
 * Hire, Promotion, Transfer, Termination) x 8 race/gender slots
 * (M/F/W/B/A/H/O/I) plus Year + JobGroup — 42 columns of data per row.
 * fillForm/readForm walk those slot pairs in a loop instead of writing 35
 * lines by hand.
 */
(function (window, $) {
    "use strict";

    if (!window.empdb || !window.empdb.api) return;
    var empdb = window.empdb;

    // The five EEO category prefixes the data model uses. Each combines
    // with one of the eight slot suffixes below to produce a property name
    // (e.g. "Population" + "Male" = "PopulationMale").
    var CATEGORIES = ["Population", "Hire", "Promo", "Transfer", "Term"];

    // Modal field-name suffix -> EEO model property suffix. The modal uses
    // single letters (Population_M etc.) for compactness; the model uses
    // full words.
    var SLOTS = [
        ["M", "Male"],
        ["F", "Female"],
        ["W", "White"],
        ["B", "Black"],
        ["A", "Asian"],
        ["H", "Hispanic"],
        ["O", "Other"],
        ["I", "Indian"]   // Native American — appended after the legacy seven.
    ];

    function esc(s) {
        return (s == null ? "" : String(s))
            .replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;")
            .replace(/"/g, "&quot;").replace(/'/g, "&#39;");
    }

    /** Format an EEO numeric cell — the values are stored as decimal? in the
     *  DB, but the legacy view always rendered them with no decimal places.
     *  Empty cells stay empty (don't render "0" for nothing). */
    function fmtInt(n) {
        if (n == null || n === "") return "";
        var num = Number(n);
        if (isNaN(num)) return "";
        return Math.round(num).toString();
    }

    function rowHtml(r) {
        // Build the 5 x 8 numeric cells in the same order as the table head.
        var cells = "";
        CATEGORIES.forEach(function (cat) {
            SLOTS.forEach(function (slot) {
                cells += "<td>" + fmtInt(r[cat + slot[1]]) + "</td>";
            });
        });

        return '<tr data-id="' + r.EeoId + '">' +
            '<td class="command-icon"><a href="#" class="text-primary empdb-eeo-edit" title="Edit"><i class="fas fa-edit"></i></a></td>' +
            '<td>' + esc(r.JobGroupName) + '</td>' +
            '<td>' + (r.Year == null ? "" : r.Year) + '</td>' +
            cells +
            '<td class="command-icon"><a href="#" class="text-danger empdb-eeo-delete" title="Delete"><i class="fas fa-trash"></i></a></td>' +
        '</tr>';
    }

    /** Walk the modal inputs and produce the EeoInfo JSON payload. */
    function readForm($m) {
        function num(name) {
            var v = $m.find('[name="' + name + '"]').val();
            return v === "" || v == null ? null : Number(v);
        }
        var data = {
            EeoId: parseInt($m.find('[name="EeoId"]').val(), 10) || 0,
            JobGroupId: parseInt($m.find('[name="JobGroupId"]').val(), 10) || null,
            Year: num("Year")
        };
        CATEGORIES.forEach(function (cat) {
            SLOTS.forEach(function (slot) {
                data[cat + slot[1]] = num(cat + "_" + slot[0]);
            });
        });
        return data;
    }

    /** Populate the modal inputs from an EeoInfo object (or clear if null). */
    function fillForm($m, r) {
        $m.find('[name="EeoId"]').val(r ? r.EeoId : 0);
        $m.find('[name="JobGroupId"]').val(r && r.JobGroupId != null ? String(r.JobGroupId) : "");
        $m.find('[name="Year"]').val(r && r.Year != null ? r.Year : "");
        CATEGORIES.forEach(function (cat) {
            SLOTS.forEach(function (slot) {
                var prop = cat + slot[1];
                var modalName = cat + "_" + slot[0];
                $m.find('[name="' + modalName + '"]').val(
                    r && r[prop] != null ? Math.round(Number(r[prop])) : ""
                );
            });
        });
    }

    var $table, $tbody, $modal, dt;
    // Original parent of the #empdbEeoAdd button so we can rescue it before
    // DataTables destroys its wrapper (which the button has been prepended into).
    var $btnHome;

    function destroyDt() {
        if (!dt) return;
        // Move the Add button back to its source markup parent so it doesn't
        // get garbage-collected with the DataTables wrapper.
        if ($btnHome && $btnHome.length) {
            $btnHome.append($("#empdbEeoAdd"));
        }
        try { dt.destroy(); } catch (e) {}
        dt = null;
    }

    function buildDt() {
        if (!$.fn.DataTable) return;
        destroyDt();

        // The 40 numeric race/gender columns (indexes 3..42 — after the
        // command icon, Job Category, and Year columns) are non-sortable so
        // DataTables doesn't render its sort arrow next to each single-letter
        // header code. That arrow would otherwise pad each column to ~30px
        // and defeat the point of the compact layout.
        var numericIndexes = [];
        for (var i = 3; i <= 42; i++) numericIndexes.push(i);

        dt = $table.DataTable({
            "order": [[2, "desc"], [1, "asc"]],   // Year desc, Job Category asc
            "pageLength": 25,
            "scrollX": true,
            "stateSave": true,
            "stateDuration": 60 * 60 * 24,
            "stateSaveCallback": function (settings, data) {
                try { localStorage.setItem("empdb-tblEeo-v1", JSON.stringify(data)); } catch (e) {}
            },
            "stateLoadCallback": function () {
                try { return JSON.parse(localStorage.getItem("empdb-tblEeo-v1")); } catch (e) { return null; }
            },
            "columnDefs": [
                { "orderable": false, "targets": "no-sort" },
                { "orderable": false, "targets": numericIndexes }
            ]
        });

        // Park the Add button inside DataTables' .dt-length container so it
        // sits flush-left in front of the page-length selector. Same pattern
        // used by transcriptDatabase/DesignationList.ascx and empdb-list.js.
        var $wrapper = $table.closest('.dataTables_wrapper, .dt-container');
        var $len = $wrapper.length ? $wrapper.find('.dt-length').first() : $();
        if ($len.length) $len.prepend($("#empdbEeoAdd"));
    }

    function reload() {
        return empdb.api.get("Eeos/All").then(function (rows) {
            destroyDt();
            if (!rows || !rows.length) {
                $tbody.html('<tr><td colspan="44" class="text-muted text-center">No EEO records.</td></tr>');
                return;
            }
            $tbody.html(rows.map(rowHtml).join(""));
            buildDt();
        }).catch(function (err) { empdb.notifyError("Load failed: " + err.message); });
    }

    function init() {
        $table = $("#table-eeo-list");
        $tbody = $table.find("tbody");
        $modal = $("#EeoEditModal");
        if (!$table.length) return;

        // Remember where the Add button lives in the source markup so we
        // can put it back any time the DataTables wrapper is destroyed.
        $btnHome = $("#empdbEeoAdd").parent();

        // Add
        $(document).on("click", "#empdbEeoAdd", function (e) {
            e.preventDefault();
            fillForm($modal, null);
            $modal.find(".modal-title").text("Add EEO Row");
            empdb.showModal("EeoEditModal");
        });

        // Edit
        $tbody.on("click", ".empdb-eeo-edit", function (e) {
            e.preventDefault();
            var id = $(this).closest("tr").data("id");
            empdb.api.get("Eeos/" + encodeURIComponent(id)).then(function (item) {
                fillForm($modal, item);
                $modal.find(".modal-title").text("Edit EEO Row");
                empdb.showModal("EeoEditModal");
            }).catch(function (err) { empdb.notifyError("Load failed: " + err.message); });
        });

        // Delete
        $tbody.on("click", ".empdb-eeo-delete", function (e) {
            e.preventDefault();
            var id = $(this).closest("tr").data("id");
            empdb.confirmDelete("Delete this EEO row?").then(function (ok) {
                if (!ok) return;
                empdb.api.del("Eeos/" + encodeURIComponent(id)).then(function () {
                    empdb.notifySuccess("EEO row deleted.");
                    reload();
                }).catch(function (err) { empdb.notifyError("Delete failed: " + err.message); });
            });
        });

        // Save
        $(document).on("click", "#empdbEeoSave", function (e) {
            e.preventDefault();
            var data = readForm($modal);
            if (!data.JobGroupId) { empdb.notifyError("Job Category is required."); return; }
            if (!data.Year) { empdb.notifyError("Year is required."); return; }
            var idVal = data.EeoId || 0;
            var p = idVal > 0
                ? empdb.api.put("Eeos/" + idVal, data)
                : empdb.api.post("Eeos", data);
            p.then(function () {
                empdb.hideModal("EeoEditModal");
                fillForm($modal, null);
                empdb.notifySuccess(idVal > 0 ? "EEO row updated." : "EEO row added.");
                reload();
            }).catch(function (err) { empdb.notifyError("Save failed: " + err.message); });
        });

        // Belt-and-suspenders: clear the modal whenever it closes for any reason.
        $modal.on("hidden.bs.modal", function () { fillForm($modal, null); });

        reload();
    }

    empdb.eeo = { init: init, reload: reload };

    $(function () {
        if (document.getElementById("table-eeo-list")) init();
    });

})(window, window.jQuery);
