/* EmployeeDB List page — admin tabs (Job Categories, Classes, Race, Locations).
 *
 * Layered on top of empdb-edit.js, which exposes:
 *   empdb.api.{get,post,put,del}
 *   empdb.notifySuccess / notifyError
 *   empdb.confirmDelete
 *   empdb.showModal / hideModal
 *
 * Each admin tab follows the same Phones-style pattern:
 *   1. Load rows from /API/{Resource}/All on tab activation.
 *   2. Add button -> blank modal -> POST.
 *   3. Pencil  -> GET /API/{Resource}/{id} -> populated modal -> PUT.
 *   4. Trash   -> swal confirm -> DELETE -> noty toast -> reload.
 *
 * The DataTables instance is destroyed and recreated each time we replace
 * <tbody>, otherwise its row index gets out of sync with what the user sees.
 */
(function (window, $) {
    "use strict";

    if (!window.empdb || !window.empdb.api) {
        // empdb-edit.js must load first.
        return;
    }
    var empdb = window.empdb;

    function esc(s) {
        return (s == null ? "" : String(s))
            .replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;")
            .replace(/"/g, "&quot;").replace(/'/g, "&#39;");
    }
    function fmtMoney(n) {
        if (n == null || n === "") return "";
        var num = Number(n);
        if (isNaN(num)) return "";
        return num.toFixed(2);
    }

    /**
     * Generic factory mirroring empdb-edit.js#makeCrudTab but talking to the
     * /All endpoint (no employee scoping) and rebuilding the DataTables wrapper.
     */
    function makeAdminTab(cfg) {
        // cfg fields:
        //   resource:   "JobGroups" | "JobClasses" | "Races" | "Locations"
        //   idField:    "JobGroupId" | "ClassId" | "RaceId" | "OfficeLocationId"
        //   tableId:    "#tblJobGroups"
        //   modalId:    "JobGroupEditModal"
        //   addBtnId:   "#empdbJobGroupAdd"
        //   saveBtnId:  "#empdbJobGroupSave"
        //   colCount:   number of columns (incl. command columns)
        //   stateKey:   localStorage key used by DataTables stateSave
        //   rowHtml:    function(item) -> "<tr>..."
        //   readForm:   function($modal) -> object to send
        //   fillForm:   function($modal, item|null) — populate inputs
        //   addTitle:   modal title for add
        //   editTitle:  modal title for edit
        //   editClass:  row-edit anchor class
        //   delClass:   row-delete anchor class
        //   confirmText:swal confirm body
        //   savedAdded: noty text after POST
        //   savedUpdated: noty text after PUT
        //   deletedText:noty text after DELETE
        //   validate:   optional function(data) -> error string or null
        var $tbody;
        var dt; // DataTables instance
        var $btnHome; // original parent of the Add button (so we can rescue
                       // it before DataTables destroys its wrapper)

        function destroyDt() {
            if (!dt) return;
            // Pull the Add button back out of .dt-length so DataTables doesn't
            // destroy it along with its wrapper. We'll re-prepend after rebuild.
            if (cfg.addBtnId && $btnHome && $btnHome.length) {
                $btnHome.append($(cfg.addBtnId));
            }
            try { dt.destroy(); } catch (e) {}
            dt = null;
        }

        function buildDt() {
            if (!$.fn.DataTable) return;
            destroyDt();
            dt = $(cfg.tableId).DataTable({
                "order": [[1, "asc"]],
                "pageLength": 25,
                "stateSave": true,
                "stateDuration": 60 * 60 * 24,
                "stateSaveCallback": function (settings, data) {
                    try { localStorage.setItem(cfg.stateKey, JSON.stringify(data)); } catch (e) {}
                },
                "stateLoadCallback": function () {
                    try { return JSON.parse(localStorage.getItem(cfg.stateKey)); } catch (e) { return null; }
                },
                "columnDefs": [{ "orderable": false, "targets": "no-sort" }]
            });

            // Move the tab's Add button into DataTables' .dt-length container
            // (the page-length selector). DataTables rebuilds .dt-length each
            // time the table is initialised, so re-prepending here on every
            // reload keeps the button parked in the right place.
            var $wrapper = $(cfg.tableId).closest('.dataTables_wrapper, .dt-container');
            var $len = $wrapper.length ? $wrapper.find('.dt-length').first() : $();
            if ($len.length) $len.prepend($(cfg.addBtnId));
        }

        function reload() {
            return empdb.api.get(cfg.resource + "/All").then(function (rows) {
                if (!$tbody) return;
                if (!rows || !rows.length) {
                    destroyDt();
                    $tbody.html('<tr><td colspan="' + cfg.colCount + '" class="text-muted text-center">No records.</td></tr>');
                    return;
                }
                destroyDt();
                $tbody.html(rows.map(cfg.rowHtml).join(""));
                buildDt();
            }).catch(function (err) {
                empdb.notifyError("Load failed: " + err.message);
            });
        }

        function init() {
            $tbody = $(cfg.tableId + " tbody");
            if (!$tbody.length) return;

            // Remember where the Add button lives in the source markup so we
            // can put it back any time the DataTables wrapper is destroyed.
            if (cfg.addBtnId) {
                $btnHome = $(cfg.addBtnId).parent();
            }

            // Add
            $(document).on("click", cfg.addBtnId, function (e) {
                e.preventDefault();
                var $m = $("#" + cfg.modalId);
                cfg.fillForm($m, null);
                $m.find('input[name="' + cfg.idField + '"]').val(0);
                $m.find(".modal-title").text(cfg.addTitle);
                empdb.showModal(cfg.modalId);
            });

            // Edit (pencil)
            $tbody.on("click", "." + cfg.editClass, function (e) {
                e.preventDefault();
                var id = $(this).closest("tr").data("id");
                empdb.api.get(cfg.resource + "/" + encodeURIComponent(id)).then(function (item) {
                    var $m = $("#" + cfg.modalId);
                    cfg.fillForm($m, item);
                    $m.find(".modal-title").text(cfg.editTitle);
                    empdb.showModal(cfg.modalId);
                }).catch(function (err) {
                    empdb.notifyError("Load failed: " + err.message);
                });
            });

            // Delete (trash)
            $tbody.on("click", "." + cfg.delClass, function (e) {
                e.preventDefault();
                var $row = $(this).closest("tr");
                var id = $row.data("id");
                empdb.confirmDelete(cfg.confirmText).then(function (ok) {
                    if (!ok) return;
                    empdb.api.del(cfg.resource + "/" + encodeURIComponent(id)).then(function () {
                        empdb.notifySuccess(cfg.deletedText);
                        reload();
                    }).catch(function (err) {
                        empdb.notifyError("Delete failed: " + err.message);
                    });
                });
            });

            // Save (modal)
            $(document).on("click", cfg.saveBtnId, function (e) {
                e.preventDefault();
                var $m = $("#" + cfg.modalId);
                var data = cfg.readForm($m);
                if (cfg.validate) {
                    var err = cfg.validate(data);
                    if (err) { empdb.notifyError(err); return; }
                }
                var idVal = data[cfg.idField] || 0;
                var p = idVal > 0
                    ? empdb.api.put(cfg.resource + "/" + idVal, data)
                    : empdb.api.post(cfg.resource, data);
                p.then(function () {
                    empdb.hideModal(cfg.modalId);
                    empdb.notifySuccess(idVal > 0 ? cfg.savedUpdated : cfg.savedAdded);
                    reload();
                }).catch(function (err) {
                    empdb.notifyError("Save failed: " + err.message);
                });
            });

            // Initial load
            reload();
        }

        return { init: init, reload: reload };
    }

    /* ---------------- Job Categories ---------------- */
    var jobGroups = makeAdminTab({
        resource: "JobGroups",
        idField: "JobGroupId",
        tableId: "#tblJobGroups",
        modalId: "JobGroupEditModal",
        addBtnId: "#empdbJobGroupAdd",
        saveBtnId: "#empdbJobGroupSave",
        colCount: 3,
        stateKey: "empdb-tblJobGroups-v1",
        editClass: "empdb-jobgroup-edit",
        delClass: "empdb-jobgroup-delete",
        addTitle: "Add Job Category",
        editTitle: "Edit Job Category",
        confirmText: "Delete this job category?",
        savedAdded: "Job category added.",
        savedUpdated: "Job category updated.",
        deletedText: "Job category deleted.",
        rowHtml: function (g) {
            return '<tr data-id="' + g.JobGroupId + '">' +
                '<td class="command-icon"><a href="#" class="text-primary empdb-jobgroup-edit" title="Edit"><i class="fas fa-edit"></i></a></td>' +
                '<td>' + esc(g.Description) + '</td>' +
                '<td class="command-icon"><a href="#" class="text-danger empdb-jobgroup-delete" title="Delete"><i class="fas fa-trash"></i></a></td>' +
            '</tr>';
        },
        fillForm: function ($m, g) {
            $m.find('[name="JobGroupId"]').val(g ? g.JobGroupId : 0);
            $m.find('[name="Description"]').val(g ? g.Description || "" : "");
        },
        readForm: function ($m) {
            return {
                JobGroupId: parseInt($m.find('[name="JobGroupId"]').val(), 10) || 0,
                Description: $m.find('[name="Description"]').val() || ""
            };
        },
        validate: function (data) {
            if (!data.Description) return "Description is required.";
            return null;
        }
    });

    /* ---------------- Classes ---------------- */
    var jobClasses = makeAdminTab({
        resource: "JobClasses",
        idField: "ClassId",
        tableId: "#tblJobClasses",
        modalId: "JobClassEditModal",
        addBtnId: "#empdbJobClassAdd",
        saveBtnId: "#empdbJobClassSave",
        colCount: 11,
        stateKey: "empdb-tblJobClasses-v1",
        editClass: "empdb-jobclass-edit",
        delClass: "empdb-jobclass-delete",
        addTitle: "Add Class",
        editTitle: "Edit Class",
        confirmText: "Delete this class?",
        savedAdded: "Class added.",
        savedUpdated: "Class updated.",
        deletedText: "Class deleted.",
        rowHtml: function (c) {
            return '<tr data-id="' + c.ClassId + '">' +
                '<td class="command-icon"><a href="#" class="text-primary empdb-jobclass-edit" title="Edit"><i class="fas fa-edit"></i></a></td>' +
                '<td>' + esc(c.ClassName) + '</td>' +
                '<td>' + esc(c.ClassCode) + '</td>' +
                '<td>' + (c.PayGrade == null ? "" : c.PayGrade) + '</td>' +
                '<td>' + esc(c.FLSA) + '</td>' +
                '<td>' + (c.EEO == null ? "" : c.EEO) + '</td>' +
                '<td>' + fmtMoney(c.MMin) + '</td>' +
                '<td>' + fmtMoney(c.MMax) + '</td>' +
                '<td>' + fmtMoney(c.AMin) + '</td>' +
                '<td>' + fmtMoney(c.AMax) + '</td>' +
                '<td class="command-icon"><a href="#" class="text-danger empdb-jobclass-delete" title="Delete"><i class="fas fa-trash"></i></a></td>' +
            '</tr>';
        },
        fillForm: function ($m, c) {
            $m.find('[name="ClassId"]').val(c ? c.ClassId : 0);
            $m.find('[name="ClassName"]').val(c ? c.ClassName || "" : "");
            $m.find('[name="ClassCode"]').val(c && c.ClassCode != null ? c.ClassCode : "");
            $m.find('[name="PayGrade"]').val(c && c.PayGrade != null ? c.PayGrade : "");
            $m.find('[name="FLSA"]').val(c ? c.FLSA || "" : "");
            $m.find('[name="EEO"]').val(c && c.EEO != null ? c.EEO : "");
            $m.find('[name="MMin"]').val(c && c.MMin != null ? c.MMin : "");
            $m.find('[name="MMax"]').val(c && c.MMax != null ? c.MMax : "");
            $m.find('[name="AMin"]').val(c && c.AMin != null ? c.AMin : "");
            $m.find('[name="AMax"]').val(c && c.AMax != null ? c.AMax : "");
        },
        readForm: function ($m) {
            function num(name) {
                var v = $m.find('[name="' + name + '"]').val();
                return v === "" || v == null ? null : Number(v);
            }
            return {
                ClassId: parseInt($m.find('[name="ClassId"]').val(), 10) || 0,
                ClassName: $m.find('[name="ClassName"]').val() || "",
                ClassCode: num("ClassCode") || 0,
                PayGrade: num("PayGrade"),
                FLSA: $m.find('[name="FLSA"]').val() || "",
                EEO: num("EEO"),
                MMin: num("MMin"),
                MMax: num("MMax"),
                AMin: num("AMin"),
                AMax: num("AMax")
            };
        },
        validate: function (data) {
            if (!data.ClassName) return "Class Name is required.";
            return null;
        }
    });

    /* ---------------- Race ---------------- */
    var races = makeAdminTab({
        resource: "Races",
        idField: "RaceId",
        tableId: "#tblRaces",
        modalId: "RaceEditModal",
        addBtnId: "#empdbRaceAdd",
        saveBtnId: "#empdbRaceSave",
        colCount: 4,
        stateKey: "empdb-tblRaces-v1",
        editClass: "empdb-race-edit",
        delClass: "empdb-race-delete",
        addTitle: "Add Race",
        editTitle: "Edit Race",
        confirmText: "Delete this race entry?",
        savedAdded: "Race added.",
        savedUpdated: "Race updated.",
        deletedText: "Race deleted.",
        rowHtml: function (r) {
            return '<tr data-id="' + r.RaceId + '">' +
                '<td class="command-icon"><a href="#" class="text-primary empdb-race-edit" title="Edit"><i class="fas fa-edit"></i></a></td>' +
                '<td>' + esc(r.RaceCode) + '</td>' +
                '<td>' + esc(r.Description) + '</td>' +
                '<td class="command-icon"><a href="#" class="text-danger empdb-race-delete" title="Delete"><i class="fas fa-trash"></i></a></td>' +
            '</tr>';
        },
        fillForm: function ($m, r) {
            $m.find('[name="RaceId"]').val(r ? r.RaceId : 0);
            $m.find('[name="RaceCode"]').val(r ? r.RaceCode || "" : "");
            $m.find('[name="Description"]').val(r ? r.Description || "" : "");
        },
        readForm: function ($m) {
            return {
                RaceId: parseInt($m.find('[name="RaceId"]').val(), 10) || 0,
                RaceCode: $m.find('[name="RaceCode"]').val() || "",
                Description: $m.find('[name="Description"]').val() || ""
            };
        },
        validate: function (data) {
            if (!data.RaceCode) return "Race Code is required.";
            if (!data.Description) return "Description is required.";
            return null;
        }
    });

    /* ---------------- Office Locations ---------------- */
    var locations = makeAdminTab({
        resource: "Locations",
        idField: "OfficeLocationId",
        tableId: "#tblLocations",
        modalId: "LocationEditModal",
        addBtnId: "#empdbLocationAdd",
        saveBtnId: "#empdbLocationSave",
        colCount: 7,
        stateKey: "empdb-tblLocations-v1",
        editClass: "empdb-location-edit",
        delClass: "empdb-location-delete",
        addTitle: "Add Office Location",
        editTitle: "Edit Office Location",
        confirmText: "Delete this location? The delete will be refused if any employee is still assigned to it.",
        savedAdded: "Location added.",
        savedUpdated: "Location updated.",
        deletedText: "Location deleted.",
        rowHtml: function (l) {
            return '<tr data-id="' + l.OfficeLocationId + '">' +
                '<td class="command-icon"><a href="#" class="text-primary empdb-location-edit" title="Edit"><i class="fas fa-edit"></i></a></td>' +
                '<td>' + esc(l.Description) + '</td>' +
                '<td>' + esc(l.Address) + '</td>' +
                '<td>' + esc(l.City) + '</td>' +
                '<td>' + esc(l.State) + '</td>' +
                '<td>' + esc(l.Zip) + '</td>' +
                '<td class="command-icon"><a href="#" class="text-danger empdb-location-delete" title="Delete"><i class="fas fa-trash"></i></a></td>' +
            '</tr>';
        },
        fillForm: function ($m, l) {
            $m.find('[name="OfficeLocationId"]').val(l ? l.OfficeLocationId : 0);
            $m.find('[name="Description"]').val(l ? l.Description || "" : "");
            $m.find('[name="Address"]').val(l ? l.Address || "" : "");
            $m.find('[name="City"]').val(l ? l.City || "" : "");
            $m.find('[name="State"]').val(l ? l.State || "" : "");
            $m.find('[name="Zip"]').val(l ? l.Zip || "" : "");
        },
        readForm: function ($m) {
            return {
                OfficeLocationId: parseInt($m.find('[name="OfficeLocationId"]').val(), 10) || 0,
                Description: $m.find('[name="Description"]').val() || "",
                Address: $m.find('[name="Address"]').val() || "",
                City: $m.find('[name="City"]').val() || "",
                State: $m.find('[name="State"]').val() || "",
                Zip: $m.find('[name="Zip"]').val() || ""
            };
        },
        validate: function (data) {
            if (!data.Description) return "Description is required.";
            return null;
        }
    });

    /* ---------------- Departments (site admins only) ----------------
       The tab + table are only rendered for IsSiteAdmin in the markup, so
       the JS init is a safe no-op on non-admin sessions because $tbody is
       empty and makeAdminTab.init() short-circuits. */
    var departments = makeAdminTab({
        resource: "Departments",
        idField: "GroupID",
        tableId: "#tblDepartments",
        modalId: "DepartmentEditModal",
        addBtnId: "#empdbDepartmentAdd",
        saveBtnId: "#empdbDepartmentSave",
        colCount: 4,
        stateKey: "empdb-tblDepartments-v1",
        editClass: "empdb-department-edit",
        delClass: "empdb-department-delete",
        addTitle: "Add Department",
        editTitle: "Edit Department",
        confirmText: "Delete this department? The delete will be refused if any employee or group membership still references it.",
        savedAdded: "Department added.",
        savedUpdated: "Department updated.",
        deletedText: "Department deleted.",
        rowHtml: function (g) {
            return '<tr data-id="' + g.GroupID + '">' +
                '<td class="command-icon"><a href="#" class="text-primary empdb-department-edit" title="Edit"><i class="fas fa-edit"></i></a></td>' +
                '<td>' + esc(g.GroupName) + '</td>' +
                '<td class="text-center">' + (g.IsSwnGroup ? '<i class="fas fa-check text-success"></i>' : '') + '</td>' +
                '<td class="command-icon"><a href="#" class="text-danger empdb-department-delete" title="Delete"><i class="fas fa-trash"></i></a></td>' +
            '</tr>';
        },
        fillForm: function ($m, g) {
            $m.find('[name="GroupID"]').val(g ? g.GroupID : 0);
            $m.find('[name="GroupName"]').val(g ? g.GroupName || "" : "");
            $m.find('[name="IsSwnGroup"]').prop('checked', !!(g && g.IsSwnGroup));
        },
        readForm: function ($m) {
            return {
                GroupID: parseInt($m.find('[name="GroupID"]').val(), 10) || 0,
                GroupName: $m.find('[name="GroupName"]').val() || "",
                IsSwnGroup: $m.find('[name="IsSwnGroup"]').is(':checked')
            };
        },
        validate: function (data) {
            if (!data.GroupName) return "Group Name is required.";
            return null;
        }
    });

    empdb.list = {
        jobGroups: jobGroups,
        jobClasses: jobClasses,
        races: races,
        locations: locations,
        departments: departments
    };

    /* ---------------- SWN actions (Sync / AddAllGroups / Show Missing) ----
       AJAX-driven against Components/Api/SwnController.cs. The previous
       postback handlers polluted the URL (DNN's BreadCrumb skin object
       blew up trying to int.Parse a stray /GroupId/0 segment) AND offered
       no progress feedback during the Sync's long run. Each button shows
       the full-screen busy overlay during the call, then displays the
       server's response via SweetAlert. */
    var swn = (function () {
        function showBusy(text) {
            var $o = $("#empdbBusyOverlay");
            if (!$o.length) return;
            if (text) $o.find(".empdb-busy-title").text(text);
            $o.show().attr("aria-hidden", "false");
        }
        function hideBusy() {
            $("#empdbBusyOverlay").hide().attr("aria-hidden", "true");
            // Reset the title so the next call starts from the default copy.
            $("#empdbBusyOverlay .empdb-busy-title").text("Working…");
        }

        function showResult(payload) {
            // Server returns { Success, Title, Html }. The Html is already
            // safe HTML (entity-encoded server-side) so we pass it as-is.
            var icon = payload && payload.Success ? "success" : "info";
            var title = (payload && payload.Title) || "SWN";
            var html = (payload && payload.Html) || "";
            if (window.Swal && window.Swal.fire) {
                window.Swal.fire({
                    title: title,
                    html: html,
                    icon: icon,
                    confirmButtonText: "OK",
                    width: 700
                });
            } else {
                // Fallback if SweetAlert didn't load.
                empdb.notifyInfo(title);
            }
        }

        function call(method, action, busyText) {
            showBusy(busyText);
            var p = method === "POST"
                ? empdb.api.post("Swn/" + action, {})
                : empdb.api.get("Swn/" + action);
            return p.then(function (r) {
                hideBusy();
                showResult(r);
            }).catch(function (err) {
                hideBusy();
                empdb.notifyError((busyText || action) + " failed: " + err.message);
            });
        }

        function init() {
            $(document).on("click", "#empdbSwnMissing", function (e) {
                e.preventDefault();
                call("GET", "MissingContacts", "Looking up missing SWN contacts…");
            });

            // Add Missing — only adds the active employees that don't have
            // an SWN contact yet. Confirms first because it does mutate SWN.
            $(document).on("click", "#empdbSwnAddMissing", function (e) {
                e.preventDefault();
                if (window.Swal && window.Swal.fire) {
                    window.Swal.fire({
                        title: "Add missing employees to SWN?",
                        text: "This will create an SWN contact for every active employee that doesn't already have one. Existing contacts are not modified.",
                        icon: "question",
                        showCancelButton: true,
                        confirmButtonText: "Yes, add missing",
                        cancelButtonText: "Cancel"
                    }).then(function (r) {
                        if (r.isConfirmed) call("POST", "AddMissing", "Adding missing employees to SWN…");
                    });
                } else {
                    call("POST", "AddMissing", "Adding missing employees to SWN…");
                }
            });

            $(document).on("click", "#empdbSwnSync", function (e) {
                e.preventDefault();
                if (window.Swal && window.Swal.fire) {
                    window.Swal.fire({
                        title: "Run full SWN Sync?",
                        text: "This will push every active employee into SWN and may take several minutes. Don't close the page while it runs.",
                        icon: "warning",
                        showCancelButton: true,
                        confirmButtonText: "Yes, run sync",
                        cancelButtonText: "Cancel"
                    }).then(function (r) {
                        if (r.isConfirmed) call("POST", "Sync", "Syncing employees with SWN…");
                    });
                } else {
                    call("POST", "Sync", "Syncing employees with SWN…");
                }
            });

            // SWN Export — pipe-delimited contact file for SWN bulk upload.
            // We can't just window.location to the endpoint: DnnApiController's
            // [DnnModuleAuthorize] checks the ModuleId / TabId headers that
            // empdb.api injects on every fetch, and a plain navigation sends
            // none of them. Pull the file via fetch (which sends the auth
            // headers) and trigger the download client-side via a Blob URL.
            $(document).on("click", "#empdbSwnExport", function (e) {
                e.preventDefault();
                var ctx = empdb.getContext();
                showBusy("Generating SWN export…");
                fetch("/DesktopModules/EmployeeDB/API/Swn/Export", {
                    method: "GET",
                    credentials: "same-origin",
                    headers: {
                        "ModuleId": ctx.moduleId,
                        "TabId": ctx.tabId,
                        "RequestVerificationToken": ctx.verificationToken,
                        "Accept": "text/plain"
                    }
                }).then(function (resp) {
                    if (!resp.ok) {
                        return resp.text().then(function (t) {
                            throw new Error(t || (resp.status + " " + resp.statusText));
                        });
                    }
                    // Pull the suggested filename out of Content-Disposition
                    // so the user gets the timestamped name the server sends.
                    var fileName = "SWN_Export.txt";
                    var cd = resp.headers.get("Content-Disposition") || "";
                    var match = cd.match(/filename\*?=(?:UTF-\d['']*)?["']?([^"';]+)["']?/i);
                    if (match) fileName = decodeURIComponent(match[1].trim());
                    return resp.blob().then(function (blob) {
                        return { blob: blob, fileName: fileName };
                    });
                }).then(function (out) {
                    hideBusy();
                    var url = URL.createObjectURL(out.blob);
                    var a = document.createElement("a");
                    a.href = url;
                    a.download = out.fileName;
                    a.style.display = "none";
                    document.body.appendChild(a);
                    a.click();
                    document.body.removeChild(a);
                    // Revoke after a tick so Chrome has time to commit the download.
                    setTimeout(function () { URL.revokeObjectURL(url); }, 1000);
                    empdb.notifySuccess("SWN export downloaded.");
                }).catch(function (err) {
                    hideBusy();
                    empdb.notifyError("Export failed: " + err.message);
                });
            });
        }

        return { init: init };
    })();
    empdb.swn = swn;

    /* ---------------- Supervisors (HR admins only) ----------------
       Different shape from the makeAdminTab pattern: there's no modal
       form — the user picks a target via the typeahead and the click
       itself does the POST. The roster table below lets the user
       toggle Active (PUT) and Delete (refused with 409 if the
       supervisor is still assigned to any employee). */
    var supervisors = (function () {
        var $tbody;
        var $searchInput;
        var $searchResults;
        var dt;
        var searchTimer = null;
        var SEARCH_DEBOUNCE_MS = 200;

        function destroyDt() {
            if (!dt) return;
            try { dt.destroy(); } catch (e) {}
            dt = null;
        }

        function buildDt() {
            if (!$.fn.DataTable) return;
            destroyDt();
            dt = $("#tblSupervisors").DataTable({
                "order": [[0, "asc"]],
                "pageLength": 25,
                "stateSave": true,
                "stateDuration": 60 * 60 * 24,
                "stateSaveCallback": function (settings, data) {
                    try { localStorage.setItem("empdb-tblSupervisors-v1", JSON.stringify(data)); } catch (e) {}
                },
                "stateLoadCallback": function () {
                    try { return JSON.parse(localStorage.getItem("empdb-tblSupervisors-v1")); } catch (e) { return null; }
                },
                "columnDefs": [{ "orderable": false, "targets": "no-sort" }]
            });
        }

        function rowHtml(s) {
            // s: { SupervisorId, EmployeeId, FirstName, LastName, IsActive,
            //      IsEmployeeActive, AssigneeCount, DisplayName }
            var name = s.DisplayName || (esc(s.LastName) + ", " + esc(s.FirstName));
            var checked = s.IsActive ? "checked" : "";
            var count   = (typeof s.AssigneeCount === "number") ? s.AssigneeCount : 0;
            // Dim the icon + count when nobody's assigned — clicking still
            // opens the modal (it'll just show "No employees assigned") but
            // the visual weight tracks the data.
            var iconClass    = count > 0 ? "text-primary"   : "text-muted";
            var badgeClass   = count > 0 ? "bg-primary"     : "bg-secondary";
            return '<tr data-id="' + s.SupervisorId + '" data-employee-id="' + s.EmployeeId + '">'
                +    '<td>' + esc(name) + '</td>'
                +    '<td class="text-center">'
                +        '<a href="#" class="empdb-supervisor-assignees ' + iconClass + '" '
                +           'data-id="' + s.SupervisorId + '" '
                +           'data-name="' + esc(name) + '" '
                +           'title="View employees assigned to this supervisor">'
                +           '<i class="fas fa-users"></i>'
                +           '&nbsp;<span class="badge ' + badgeClass + '">' + count + '</span>'
                +        '</a>'
                +    '</td>'
                +    '<td class="text-center">'
                +        '<input type="checkbox" class="form-check-input empdb-supervisor-active" '
                +              'data-id="' + s.SupervisorId + '" ' + checked + ' />'
                +    '</td>'
                +    '<td class="command-icon">'
                +        '<a href="#" class="text-danger empdb-supervisor-delete" title="Delete">'
                +            '<i class="fas fa-trash"></i>'
                +        '</a>'
                +    '</td>'
                + '</tr>';
        }

        function reload() {
            return empdb.api.get("Supervisors/All").then(function (rows) {
                if (!$tbody) return;
                if (!rows || !rows.length) {
                    destroyDt();
                    $tbody.html('<tr><td colspan="4" class="text-muted text-center">No supervisors yet — search above to add one.</td></tr>');
                    return;
                }
                destroyDt();
                $tbody.html(rows.map(rowHtml).join(""));
                buildDt();
            }).catch(function (err) {
                empdb.notifyError("Load failed: " + err.message);
            });
        }

        /**
         * Open the assignees modal for a supervisor. Loads the list
         * lazily — the modal opens immediately with a "Loading…"
         * placeholder so the user sees feedback before the API returns.
         */
        function openAssigneesModal(supervisorId, supervisorName) {
            $("#empdbSupAssigneesName").text(supervisorName || "");
            $("#empdbSupAssigneesList")
                .html('<li class="list-group-item text-muted">Loading…</li>');
            empdb.showModal("SupervisorAssigneesModal");

            empdb.api.get("Supervisors/Assignees?id=" + supervisorId)
                .then(function (rows) {
                    if (!rows || !rows.length) {
                        $("#empdbSupAssigneesList").html(
                            '<li class="list-group-item text-muted">No employees assigned to this supervisor.</li>');
                        return;
                    }
                    var html = rows.map(function (r) {
                        var name  = esc(r.DisplayName || (r.LastName + ", " + r.FirstName));
                        var title = r.JobTitle
                            ? ' <small class="text-muted">— ' + esc(r.JobTitle) + '</small>'
                            : '';
                        var badge = r.IsActive
                            ? ''
                            : ' <span class="badge bg-secondary ms-2">terminated</span>';
                        return '<li class="list-group-item">' + name + title + badge + '</li>';
                    }).join("");
                    $("#empdbSupAssigneesList").html(html);
                })
                .catch(function (err) {
                    $("#empdbSupAssigneesList").html(
                        '<li class="list-group-item text-danger">Load failed: ' + esc(err.message) + '</li>');
                });
        }

        function hideSearchResults() {
            if ($searchResults) {
                $searchResults.empty().hide();
            }
        }

        function renderSearchResults(matches) {
            if (!matches || !matches.length) {
                $searchResults
                    .html('<div class="list-group-item text-muted">No matching employees.</div>')
                    .show();
                return;
            }
            var html = matches.map(function (m) {
                var name = esc(m.DisplayName || (m.LastName + ", " + m.FirstName));
                var inactiveBadge = m.IsActive
                    ? ""
                    : ' <span class="badge bg-secondary ms-2">terminated</span>';
                var title = m.JobTitle ? ' <small class="text-muted">— ' + esc(m.JobTitle) + '</small>' : '';
                return '<a href="#" class="list-group-item list-group-item-action empdb-supervisor-add-pick" '
                    +     'data-employee-id="' + m.EmployeeId + '">'
                    +     name + title + inactiveBadge
                    +  '</a>';
            }).join("");
            $searchResults.html(html).show();
        }

        function runSearch(q) {
            if (!q || q.length < 2) {
                hideSearchResults();
                return;
            }
            empdb.api.get("Supervisors/SearchEmployees?q=" + encodeURIComponent(q))
                .then(renderSearchResults)
                .catch(function (err) {
                    empdb.notifyError("Search failed: " + err.message);
                });
        }

        function addSupervisor(employeeId) {
            return empdb.api.post("Supervisors", { EmployeeId: employeeId, IsActive: true })
                .then(function () {
                    empdb.notifySuccess("Supervisor added.");
                    $searchInput.val("");
                    hideSearchResults();
                    reload();
                })
                .catch(function (err) {
                    empdb.notifyError("Add failed: " + err.message);
                });
        }

        function toggleActive(supervisorId, makeActive) {
            return empdb.api.put("Supervisors/" + supervisorId, { IsActive: makeActive })
                .then(function () {
                    empdb.notifySuccess(makeActive ? "Marked active." : "Marked inactive.");
                })
                .catch(function (err) {
                    empdb.notifyError("Update failed: " + err.message);
                    // Roll the checkbox back to match server state.
                    reload();
                });
        }

        function deleteSupervisor(supervisorId) {
            return empdb.confirmDelete("Remove this supervisor from the list?")
                .then(function (ok) {
                    if (!ok) return;
                    return empdb.api.del("Supervisors/" + supervisorId)
                        .then(function () {
                            empdb.notifySuccess("Supervisor removed.");
                            reload();
                        })
                        .catch(function (err) {
                            // Server returns 409 with a message when the supervisor
                            // is still assigned to employees. Surface that text
                            // verbatim — it includes a remediation hint.
                            empdb.notifyError("Delete refused: " + err.message);
                        });
                });
        }

        function init() {
            $tbody         = $("#tblSupervisors tbody");
            $searchInput   = $("#empdbSupervisorSearch");
            $searchResults = $("#empdbSupervisorSearchResults");
            if (!$tbody.length) return;

            // Typeahead: debounce, then call the server's search endpoint.
            $searchInput.on("input", function () {
                var q = $(this).val();
                if (searchTimer) clearTimeout(searchTimer);
                searchTimer = setTimeout(function () { runSearch(q); }, SEARCH_DEBOUNCE_MS);
            });
            // Hide results when the input loses focus, but give the click on
            // a result item enough time to register first.
            $searchInput.on("blur", function () {
                setTimeout(hideSearchResults, 150);
            });
            // Re-show last results if the input is refocused with text in it.
            $searchInput.on("focus", function () {
                if ($searchResults && $searchResults.children().length) $searchResults.show();
            });

            // Click a typeahead result -> POST /Supervisors with that EmployeeId.
            $(document).on("click", ".empdb-supervisor-add-pick", function (e) {
                e.preventDefault();
                var id = parseInt($(this).data("employee-id"), 10);
                if (!id || id <= 0) return;
                addSupervisor(id);
            });

            // Users icon -> assignees modal.
            $tbody.on("click", ".empdb-supervisor-assignees", function (e) {
                e.preventDefault();
                var id   = parseInt($(this).data("id"), 10);
                var name = $(this).data("name") || "";
                if (!id) return;
                openAssigneesModal(id, name);
            });

            // Active toggle (PUT).
            $tbody.on("change", ".empdb-supervisor-active", function () {
                var id = parseInt($(this).data("id"), 10);
                if (!id) return;
                var makeActive = $(this).is(":checked");
                toggleActive(id, makeActive);
            });

            // Trash (DELETE).
            $tbody.on("click", ".empdb-supervisor-delete", function (e) {
                e.preventDefault();
                var id = parseInt($(this).closest("tr").data("id"), 10);
                if (!id) return;
                deleteSupervisor(id);
            });

            reload();
        }

        return { init: init, reload: reload };
    })();

    $(function () {
        // SWN buttons live on EmployeeList; init unconditionally — the
        // handlers themselves no-op on pages without the buttons.
        swn.init();

        // Only initialise the admin-tab tables on the EmployeeList page
        // (the Edit page doesn't ship these tables, so guard against
        // missing elements).
        if (!document.getElementById("tblJobGroups")) return;
        jobGroups.init();
        jobClasses.init();
        races.init();
        locations.init();
        // Departments table is only rendered for site admins; init() no-ops
        // when the table element isn't present.
        departments.init();
        // Supervisors table is only rendered for HR admins; init() no-ops
        // when the table element isn't present.
        supervisors.init();
    });

})(window, window.jQuery);
