/* ExpertWitness admin lists — generic CRUD-tab factory on top of ew-core.js.
 *
 *   ew.makeAdminTab(cfg).init();
 *
 * cfg:
 *   resource     "Types" | "Locations" | "Requests"  (Web API controller)
 *   tableId      "#tblTypes"
 *   colCount     number of <td> columns (for the empty-state row)
 *   rowHtml      function(item) -> "<tr data-id='..'>..."
 *   order        DataTables order array (default [[0,"asc"]])
 *   // edit/create modal (omit all three for a read-only list):
 *   idField      "TypeID"
 *   modalId      "TypeEditModal"
 *   addBtnId     "#ewTypeAdd"
 *   saveBtnId    "#ewTypeSave"
 *   editClass    "ew-type-edit"     (pencil; GET {id} -> fillForm -> PUT/POST)
 *   fillForm     function($modal, item|null)
 *   readForm     function($modal) -> data object
 *   validate     function(data) -> error string | null
 *   addTitle / editTitle / addedText / updatedText
 *   // read-only detail (Requests):
 *   viewClass    "ew-request-view"  (eye; GET {id} -> onView -> show modal)
 *   onView       function(item, $modal)
 *   // delete:
 *   delClass     "ew-type-delete"
 *   confirmText / deletedText
 */
(function (window, $) {
    "use strict";
    if (!window.ew || !window.ew.api) return; // ew-core.js must load first
    var ew = window.ew;

    ew.makeAdminTab = function (cfg) {
        var $tbody, dt, $btnHome;

        function destroyDt() {
            if (!dt) return;
            // Rescue the Add button before DataTables destroys its wrapper.
            if (cfg.addBtnId && $btnHome && $btnHome.length) $btnHome.append($(cfg.addBtnId));
            try { dt.destroy(); } catch (e) {}
            dt = null;
        }

        function buildDt() {
            if (!$.fn.DataTable) return;
            destroyDt();
            dt = $(cfg.tableId).DataTable({
                "order": cfg.order || [[0, "asc"]],
                "pageLength": 25,
                "stateSave": true,
                "columnDefs": [{ "orderable": false, "targets": "no-sort" }]
            });
            // Park the Add button in DataTables' length container.
            if (cfg.addBtnId) {
                var $wrap = $(cfg.tableId).closest('.dataTables_wrapper, .dt-container');
                var $len = $wrap.length ? $wrap.find('.dt-length, .dataTables_length').first() : $();
                if ($len.length) $len.prepend($(cfg.addBtnId));
            }
        }

        function reload() {
            return ew.api.get(cfg.resource + "/All").then(function (rows) {
                if (!$tbody) return;
                destroyDt();
                if (!rows || !rows.length) {
                    $tbody.html('<tr><td colspan="' + cfg.colCount + '" class="text-center text-muted">No records.</td></tr>');
                    return;
                }
                $tbody.html(rows.map(cfg.rowHtml).join(""));
                buildDt();
            }).catch(function (err) { ew.notifyError("Load failed: " + err.message); });
        }

        function init() {
            $tbody = $(cfg.tableId + " tbody");
            if (!$tbody.length) return;
            if (cfg.addBtnId) $btnHome = $(cfg.addBtnId).parent();

            // Add -> blank modal
            if (cfg.addBtnId) {
                $(document).on("click", cfg.addBtnId, function (e) {
                    e.preventDefault();
                    var $m = $("#" + cfg.modalId);
                    cfg.fillForm($m, null);
                    if (cfg.idField) $m.find('[name="' + cfg.idField + '"]').val(0);
                    $m.find(".modal-title").text(cfg.addTitle);
                    ew.showModal(cfg.modalId);
                });
            }

            // Pencil -> GET {id} -> populated modal
            if (cfg.editClass) {
                $tbody.on("click", "." + cfg.editClass, function (e) {
                    e.preventDefault();
                    var id = $(this).closest("tr").data("id");
                    ew.api.get(cfg.resource + "/" + encodeURIComponent(id)).then(function (item) {
                        var $m = $("#" + cfg.modalId);
                        cfg.fillForm($m, item);
                        $m.find(".modal-title").text(cfg.editTitle);
                        ew.showModal(cfg.modalId);
                    }).catch(function (err) { ew.notifyError("Load failed: " + err.message); });
                });
            }

            // Eye -> GET {id} -> read-only detail modal
            if (cfg.viewClass) {
                $tbody.on("click", "." + cfg.viewClass, function (e) {
                    e.preventDefault();
                    var id = $(this).closest("tr").data("id");
                    ew.api.get(cfg.resource + "/" + encodeURIComponent(id)).then(function (item) {
                        if (cfg.onView) cfg.onView(item, $("#" + cfg.modalId));
                        ew.showModal(cfg.modalId);
                    }).catch(function (err) { ew.notifyError("Load failed: " + err.message); });
                });
            }

            // Trash -> confirm -> DELETE
            if (cfg.delClass) {
                $tbody.on("click", "." + cfg.delClass, function (e) {
                    e.preventDefault();
                    var id = $(this).closest("tr").data("id");
                    ew.confirmDelete(cfg.confirmText).then(function (ok) {
                        if (!ok) return;
                        ew.api.del(cfg.resource + "/" + encodeURIComponent(id)).then(function () {
                            ew.notifySuccess(cfg.deletedText || "Deleted.");
                            reload();
                        }).catch(function (err) { ew.notifyError("Delete failed: " + err.message); });
                    });
                });
            }

            // Save (modal) -> POST/PUT
            if (cfg.saveBtnId) {
                $(document).on("click", cfg.saveBtnId, function (e) {
                    e.preventDefault();
                    var $m = $("#" + cfg.modalId);
                    var data = cfg.readForm($m);
                    if (cfg.validate) {
                        var err = cfg.validate(data);
                        if (err) { ew.notifyError(err); return; }
                    }
                    var idVal = (cfg.idField && data[cfg.idField]) || 0;
                    var p = idVal > 0
                        ? ew.api.put(cfg.resource + "/" + idVal, data)
                        : ew.api.post(cfg.resource, data);
                    p.then(function () {
                        ew.hideModal(cfg.modalId);
                        ew.notifySuccess(idVal > 0 ? (cfg.updatedText || "Saved.") : (cfg.addedText || "Added."));
                        reload();
                    }).catch(function (err) { ew.notifyError("Save failed: " + err.message); });
                });
            }

            reload();
        }

        return { init: init, reload: reload };
    };
})(window, jQuery);
