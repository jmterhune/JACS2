/* New Hire IT Worksheet — front-end glue
 *
 * Depends on the empdb namespace from empdb-edit.js for:
 *   empdb.api.{get,post,put,del}     (DNN-aware fetch wrapper)
 *   empdb.notifySuccess / Error / Info
 *   empdb.confirmDelete
 *   empdb.showModal / hideModal
 *   empdb.phoneMask / digitsOnly
 *
 * The view (NewHireITWorksheet.ascx) is mostly static HTML with name=
 * attributes matching the server-side NhitRequestInfo / NhitProfileInfo /
 * NhitItemInfo models. We:
 *   1. Load active items into the three checklist sections.
 *   2. Load profile dropdown.
 *   3. On profile change, fetch the profile and apply its values to the
 *      form (employee-unique fields are NOT touched).
 *   4. Save / Update / Delete profile (admin-only — buttons are hidden
 *      for non-admins via the .admin-only class).
 *   5. Manage Items modal: CRUD against /API/NhitItems.
 *   6. Submit: POST to /API/NhitRequests/Submit, surface helpdesk email
 *      result, reset employee-unique fields on success.
 */
(function (window, $) {
    "use strict";
    if (!window.empdb || !window.empdb.api) return; // empdb-edit.js is required first

    var empdb = window.empdb;

    // Fields we DELIBERATELY skip when applying a profile or saving one.
    // These are the unique-per-employee / unique-per-submission fields
    // that should never bleed across hires.
    var EMPLOYEE_UNIQUE_FIELDS = [
        "EmployeeName", "AKA", "PositionTitle", "SupervisorName",
        "DepartmentUnitGroup", "OfficeSuiteNumber", "DeskPhoneNumber",
        "TodaysDate", "EffectiveDate", "TempInternEndDate"
    ];

    // Profile-bearing fields (text inputs + selects + checkboxes that DO
    // belong to a profile). Used by both readProfileFromForm and applyProfile.
    var PROFILE_TEXT_FIELDS = [
        "AccessCardTo", "KeysNeeded", "ParkingAccess",
        "EmailDistributionGroups", "CalendarAccess", "ShareDriveAccess",
        "AdditionalPrinterAccess", "Notes"
    ];
    var PROFILE_BOOL_FIELDS = [
        "EquipmentLaptop", "EquipmentTwoInOne", "EquipmentDesktop", "EquipmentCellPhone",
        "ManagerBlog", "AddToSupervisorDropdown", "WorkCellphoneSetup"
    ];
    var PROFILE_RADIO_FIELDS = ["BuildingLocation", "EmployeeType"];

    // ----------------------------------------------------------------
    //  Bootstrapping helpers
    // ----------------------------------------------------------------
    function ctx() { return (window.__empdbCtx || {}); }

    // Toggle a button's busy state — disables it and swaps its leading
    // <i> icon for a spinner so the user sees instant feedback on click.
    // The original icon's class is stashed on the jQuery data slot so we
    // can restore it without having to know what the icon was.
    function setButtonBusy($btn, busy) {
        if (!$btn || !$btn.length) return;
        var $icon = $btn.find("i").first();
        if (busy) {
            if ($icon.length && !$btn.data("origIconClass")) {
                $btn.data("origIconClass", $icon.attr("class"));
            }
            if ($icon.length) $icon.attr("class", "fas fa-spinner fa-spin");
            $btn.prop("disabled", true);
        } else {
            var orig = $btn.data("origIconClass");
            if (orig && $icon.length) $icon.attr("class", orig);
            $btn.prop("disabled", false);
        }
    }

    function applyAdminVisibility() {
        // The .admin-only buttons are sprinkled across the form. Hide the
        // whole button when the current user isn't a site admin so non-
        // admins don't get false 403 toasts when clicking.
        if (!ctx().isAdmin) {
            $(".admin-only").hide();
        }
    }

    function setVal(name, value) {
        var $el = $('#NhitForm [name="' + name + '"]');
        if (!$el.length) return;
        if ($el.is(":checkbox")) {
            $el.prop("checked", !!value);
        } else if ($el.is("textarea") || $el.is("select") || $el.is("input")) {
            $el.val(value == null ? "" : value);
        }
    }
    function getVal(name) {
        var $el = $('#NhitForm [name="' + name + '"]');
        if (!$el.length) return "";
        if ($el.is(":checkbox")) return $el.is(":checked");
        return $el.val() || "";
    }
    function getRadio(name) {
        var $el = $('#NhitForm input[type="radio"][name="' + name + '"]:checked');
        return $el.length ? $el.val() : "";
    }
    function setRadio(name, value) {
        $('#NhitForm input[type="radio"][name="' + name + '"]').prop("checked", false);
        if (value) {
            $('#NhitForm input[type="radio"][name="' + name + '"][value="' + value + '"]').prop("checked", true);
        }
    }

    // Strip the (999) 999-9999 mask so the API receives raw digits.
    function digitsFromPhoneInput(name) {
        var raw = getVal(name);
        return empdb.digitsOnly ? empdb.digitsOnly(raw) : String(raw).replace(/\D/g, "");
    }

    // ISO-date-only string for the API. <input type="date"> already gives
    // us yyyy-MM-dd, but be defensive against blank strings.
    function dateOrNull(name) {
        var v = getVal(name);
        return v ? v : null;
    }

    // ----------------------------------------------------------------
    //  Catalog rendering (Software / Intranet / Judicial)
    // ----------------------------------------------------------------
    var allItems = [];

    function escapeHtml(s) {
        return (s == null ? "" : String(s))
            .replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;")
            .replace(/"/g, "&quot;").replace(/'/g, "&#39;");
    }

    function renderChecklist(category, $target) {
        var rows = allItems.filter(function (i) { return i.Category === category; });
        if (!rows.length) {
            $target.html('<span class="text-muted">(no items configured — click <em>Manage Items</em> to add)</span>');
            return;
        }
        var html = rows.map(function (i) {
            var label = escapeHtml(i.Name);
            if (i.Notes) label += ' <span class="text-muted">(' + escapeHtml(i.Notes) + ')</span>';
            return '<div class="form-check">' +
                '<input type="checkbox" class="form-check-input empdb-nhit-item" id="nhitItem_' + i.NhitItemId + '" data-id="' + i.NhitItemId + '" />' +
                '<label class="form-check-label" for="nhitItem_' + i.NhitItemId + '">' + label + '</label>' +
                '</div>';
        }).join("");
        $target.html(html);
    }

    function loadItemsThenRender() {
        return empdb.api.get("NhitItems/Active").then(function (rows) {
            allItems = rows || [];
            renderChecklist("Software", $("#empdbNhitItemsSoftware"));
            renderChecklist("Intranet", $("#empdbNhitItemsIntranet"));
            renderChecklist("Judicial", $("#empdbNhitItemsJudicial"));
        });
    }

    function getCheckedItemIds() {
        return $('.empdb-nhit-item:checked').map(function () {
            return parseInt(this.getAttribute("data-id"), 10);
        }).get().filter(function (n) { return n > 0; });
    }

    function setCheckedItemIds(ids) {
        var set = {};
        (ids || []).forEach(function (n) { set[n] = true; });
        $('.empdb-nhit-item').each(function () {
            var id = parseInt(this.getAttribute("data-id"), 10);
            this.checked = !!set[id];
        });
    }

    // ----------------------------------------------------------------
    //  Profile load / save / delete
    // ----------------------------------------------------------------
    function loadProfileList() {
        return empdb.api.get("NhitProfiles/All").then(function (rows) {
            var $sel = $("#empdbNhitProfile");
            var current = $sel.val();
            $sel.empty();
            $sel.append('<option value="">(no profile — start blank)</option>');
            (rows || []).forEach(function (p) {
                $sel.append('<option value="' + p.NhitProfileId + '">' + escapeHtml(p.ProfileName) + '</option>');
            });
            if (current) $sel.val(current);
        });
    }

    function applyProfile(p) {
        // Update non-employee-unique fields ONLY. Anything in
        // EMPLOYEE_UNIQUE_FIELDS keeps whatever the user already typed.
        if (!p) return;
        PROFILE_RADIO_FIELDS.forEach(function (n) { setRadio(n, p[n]); });
        PROFILE_BOOL_FIELDS.forEach(function (n) { setVal(n, !!p[n]); });
        PROFILE_TEXT_FIELDS.forEach(function (n) { setVal(n, p[n] || ""); });
        setCheckedItemIds(p.SelectedItemIds || []);
    }

    function clearProfileFields() {
        // Reset only the profile-bearing fields, leaving employee-unique
        // entries alone. Used when the user picks "(no profile)".
        PROFILE_RADIO_FIELDS.forEach(function (n) { setRadio(n, ""); });
        PROFILE_BOOL_FIELDS.forEach(function (n) { setVal(n, false); });
        PROFILE_TEXT_FIELDS.forEach(function (n) { setVal(n, ""); });
        setCheckedItemIds([]);
    }

    function readProfileFromForm(profileName) {
        var dto = { ProfileName: profileName };
        PROFILE_RADIO_FIELDS.forEach(function (n) { dto[n] = getRadio(n) || null; });
        PROFILE_BOOL_FIELDS.forEach(function (n) { dto[n] = !!getVal(n); });
        PROFILE_TEXT_FIELDS.forEach(function (n) { dto[n] = getVal(n) || null; });
        dto.SelectedItemIds = getCheckedItemIds();
        return dto;
    }

    function selectedProfileId() {
        var v = parseInt($("#empdbNhitProfile").val(), 10);
        return isNaN(v) || v <= 0 ? 0 : v;
    }

    function selectedProfileName() {
        var $opt = $("#empdbNhitProfile option:selected");
        return $opt.length ? $opt.text() : "";
    }

    // ----------------------------------------------------------------
    //  Manage Items modal
    // ----------------------------------------------------------------
    var manageRows = [];

    function reloadManageItems() {
        var filter = $("#empdbNhitItemsFilter").val() || "";
        return empdb.api.get("NhitItems/All").then(function (rows) {
            manageRows = rows || [];
            renderManageTable(filter);
        });
    }

    function renderManageTable(filter) {
        var $tbody = $("#empdbNhitItemsTable tbody");
        var rows = filter
            ? manageRows.filter(function (r) { return r.Category === filter; })
            : manageRows.slice();
        if (!rows.length) {
            $tbody.html('<tr><td colspan="7" class="text-muted text-center">No applications.</td></tr>');
            return;
        }
        $tbody.html(rows.map(function (r) {
            return '<tr data-id="' + r.NhitItemId + '">' +
                '<td class="command-icon"><a href="#" class="text-primary empdb-nhit-item-edit" title="Edit"><i class="fas fa-edit"></i></a></td>' +
                '<td>' + escapeHtml(r.Category) + '</td>' +
                '<td>' + escapeHtml(r.Name) + '</td>' +
                '<td>' + escapeHtml(r.Notes || "") + '</td>' +
                '<td>' + (r.SortOrder == null ? "" : r.SortOrder) + '</td>' +
                '<td class="text-center">' + (r.IsActive ? '<i class="fas fa-check text-success"></i>' : '') + '</td>' +
                '<td class="command-icon"><a href="#" class="text-danger empdb-nhit-item-delete" title="Delete"><i class="fas fa-trash"></i></a></td>' +
            '</tr>';
        }).join(""));
    }

    function fillItemModal(item) {
        var $m = $("#empdbNhitItemModal");
        $m.find('[name="NhitItemId"]').val(item ? item.NhitItemId : 0);
        $m.find('[name="Category"]').val(item && item.Category ? item.Category : "Software");
        $m.find('[name="Name"]').val(item ? item.Name || "" : "");
        $m.find('[name="Notes"]').val(item ? item.Notes || "" : "");
        $m.find('[name="SortOrder"]').val(item && item.SortOrder != null ? item.SortOrder : "");
        $m.find('[name="IsActive"]').prop("checked", item ? !!item.IsActive : true);
        $m.find(".modal-title").text(item ? "Edit Application" : "Add Application");
    }

    function readItemModal() {
        var $m = $("#empdbNhitItemModal");
        return {
            NhitItemId: parseInt($m.find('[name="NhitItemId"]').val(), 10) || 0,
            Category: $m.find('[name="Category"]').val(),
            Name: $m.find('[name="Name"]').val() || "",
            Notes: $m.find('[name="Notes"]').val() || "",
            SortOrder: parseInt($m.find('[name="SortOrder"]').val(), 10) || 0,
            IsActive: $m.find('[name="IsActive"]').is(":checked")
        };
    }

    // ----------------------------------------------------------------
    //  Submit (POST -> /API/NhitRequests/Submit)
    // ----------------------------------------------------------------
    function buildRequestPayload() {
        // Forward the EmployeeId from the preload payload so the audit row
        // (tjc_nhit_request.EmployeeId) FKs back to the employee record.
        var preload = ctx().preload || {};
        var dto = {
            EmployeeId: preload.EmployeeId || null,
            EmployeeName: getVal("EmployeeName"),
            AKA: getVal("AKA") || null,
            PositionTitle: getVal("PositionTitle") || null,
            SupervisorName: getVal("SupervisorName") || null,
            DepartmentUnitGroup: getVal("DepartmentUnitGroup") || null,
            OfficeSuiteNumber: getVal("OfficeSuiteNumber") || null,
            DeskPhoneNumber: digitsFromPhoneInput("DeskPhoneNumber") || null,
            TodaysDate: dateOrNull("TodaysDate"),
            EffectiveDate: dateOrNull("EffectiveDate"),
            TempInternEndDate: dateOrNull("TempInternEndDate"),
            BuildingLocation: getRadio("BuildingLocation") || null,
            EmployeeType: getRadio("EmployeeType") || null,
            SelectedItemIds: getCheckedItemIds()
        };
        PROFILE_BOOL_FIELDS.forEach(function (n) { dto[n] = !!getVal(n); });
        PROFILE_TEXT_FIELDS.forEach(function (n) { dto[n] = getVal(n) || null; });
        return dto;
    }

    function resetEmployeeUniqueFields() {
        EMPLOYEE_UNIQUE_FIELDS.forEach(function (n) { setVal(n, ""); });
    }

    // ----------------------------------------------------------------
    //  Wire up
    // ----------------------------------------------------------------
    // Apply the preload payload emitted by NewHireITWorksheet.ascx.cs when
    // the user arrives via the new-hire redirect (?EmployeeId=N). Only
    // fields actually present in the payload are written — the user keeps
    // any value they may have already typed for fields the preload didn't
    // populate (e.g. Office Suite #, AKA, Notes).
    function applyPreload() {
        var preload = ctx().preload;
        if (!preload) return;

        // Text/textarea/date inputs by name.
        ["EmployeeName", "AKA", "PositionTitle", "SupervisorName",
         "DepartmentUnitGroup", "OfficeSuiteNumber", "DeskPhoneNumber",
         "TodaysDate", "EffectiveDate", "TempInternEndDate"].forEach(function (n) {
            if (preload[n] != null) setVal(n, preload[n]);
        });
        PROFILE_TEXT_FIELDS.forEach(function (n) {
            if (preload[n] != null) setVal(n, preload[n]);
        });
        PROFILE_BOOL_FIELDS.forEach(function (n) {
            if (preload[n] != null) setVal(n, !!preload[n]);
        });
        PROFILE_RADIO_FIELDS.forEach(function (n) {
            if (preload[n]) setRadio(n, preload[n]);
        });
    }

    function init() {
        if (!document.getElementById("NhitForm")) return;

        applyAdminVisibility();

        // Default Today's Date if blank.
        if (!getVal("TodaysDate")) {
            var d = new Date();
            var iso = d.getFullYear() + "-" +
                String(d.getMonth() + 1).padStart(2, "0") + "-" +
                String(d.getDate()).padStart(2, "0");
            setVal("TodaysDate", iso);
        }

        // Pre-populate from the just-saved employee, if present.
        applyPreload();

        // Initial loads — items + profile dropdown — in parallel.
        loadItemsThenRender().catch(function (err) { empdb.notifyError("Loading items failed: " + err.message); });
        loadProfileList().catch(function (err) { empdb.notifyError("Loading profiles failed: " + err.message); });

        // ---- Profile dropdown change
        $("#empdbNhitProfile").on("change", function () {
            var id = selectedProfileId();
            if (id <= 0) { clearProfileFields(); return; }
            empdb.api.get("NhitProfiles/" + id).then(function (p) {
                applyProfile(p);
                empdb.notifyInfo("Loaded profile: " + p.ProfileName);
            }).catch(function (err) { empdb.notifyError("Could not load profile: " + err.message); });
        });

        // ---- Manage Profiles modal (top-of-form button)
        // Lists every profile and lets the user delete one. Loading a profile
        // still happens via the dropdown next to this button.
        // Same modal-first / spinner pattern as Manage Applications so the
        // user gets immediate feedback even on a cold-start fetch.
        $("#empdbNhitProfileManage").on("click", function () {
            var $btn = $(this);
            $("#empdbNhitProfilesTable tbody").html(
                '<tr><td colspan="2" class="text-muted text-center">' +
                '<i class="fas fa-spinner fa-spin"></i>&nbsp;Loading profiles…</td></tr>'
            );
            setButtonBusy($btn, true);
            empdb.showModal("empdbNhitProfilesModal");
            reloadManageProfiles().then(function () {
                setButtonBusy($btn, false);
            }, function (err) {
                setButtonBusy($btn, false);
                empdb.notifyError("Loading profiles failed: " + err.message);
            });
        });
        $("#empdbNhitProfilesTable").on("click", ".empdb-nhit-profile-delete", function (e) {
            e.preventDefault();
            var $row = $(this).closest("tr");
            var id = parseInt($row.data("id"), 10);
            var name = $row.find("td").first().text();
            empdb.confirmDelete("Delete the profile \"" + name + "\"?").then(function (ok) {
                if (!ok) return;
                empdb.api.del("NhitProfiles/" + id).then(function () {
                    empdb.notifySuccess("Profile deleted.");
                    // Refresh the modal table AND the dropdown. If the deleted
                    // profile was the one currently selected, clear the form's
                    // profile-bearing fields too.
                    var wasSelected = selectedProfileId() === id;
                    return Promise.all([reloadManageProfiles(), loadProfileList()]).then(function () {
                        if (wasSelected) {
                            $("#empdbNhitProfile").val("");
                            clearProfileFields();
                        }
                    });
                }).catch(function (err) { empdb.notifyError("Delete failed: " + err.message); });
            });
        });

        // ---- Manage Applications button (in the Software fieldset legend)
        // The fetch can take 500-1500ms on the first cold-start hit (PetaPoco
        // PocoData cache + DnnModuleAuthorize pipeline), so we open the modal
        // immediately and show a spinner on the button instead of waiting
        // silently for the round trip — the user sees instant feedback and
        // the modal's "Loading…" placeholder until the rows arrive.
        $("#empdbNhitManageApps").on("click", function () {
            var $btn = $(this);
            // Reset the table to its loading placeholder so a previous open's
            // rows don't briefly flash before the reload completes.
            $("#empdbNhitItemsTable tbody").html(
                '<tr><td colspan="7" class="text-muted text-center">' +
                '<i class="fas fa-spinner fa-spin"></i>&nbsp;Loading applications…</td></tr>'
            );
            setButtonBusy($btn, true);
            empdb.showModal("empdbNhitItemsModal");
            reloadManageItems().then(function () {
                setButtonBusy($btn, false);
            }, function (err) {
                setButtonBusy($btn, false);
                empdb.notifyError("Loading applications failed: " + err.message);
            });
        });
        $("#empdbNhitItemsFilter").on("change", function () { renderManageTable(this.value); });
        $("#empdbNhitItemAdd").on("click", function () {
            fillItemModal(null);
            empdb.showModal("empdbNhitItemModal");
        });
        $("#empdbNhitItemsTable").on("click", ".empdb-nhit-item-edit", function (e) {
            e.preventDefault();
            var id = parseInt($(this).closest("tr").data("id"), 10);
            empdb.api.get("NhitItems/" + id).then(function (item) {
                fillItemModal(item);
                empdb.showModal("empdbNhitItemModal");
            }).catch(function (err) { empdb.notifyError("Load failed: " + err.message); });
        });
        $("#empdbNhitItemsTable").on("click", ".empdb-nhit-item-delete", function (e) {
            e.preventDefault();
            var id = parseInt($(this).closest("tr").data("id"), 10);
            empdb.confirmDelete("Delete this application? (Use the Active checkbox to keep history but hide it instead.)").then(function (ok) {
                if (!ok) return;
                empdb.api.del("NhitItems/" + id).then(function () {
                    empdb.notifySuccess("Application deleted.");
                    return reloadManageItems().then(loadItemsThenRender);
                }).catch(function (err) { empdb.notifyError("Delete failed: " + err.message); });
            });
        });
        $("#empdbNhitItemSave").on("click", function () {
            var item = readItemModal();
            if (!item.Name) { empdb.notifyError("Name is required."); return; }
            var p = item.NhitItemId > 0
                ? empdb.api.put("NhitItems/" + item.NhitItemId, item)
                : empdb.api.post("NhitItems", item);
            p.then(function () {
                empdb.hideModal("empdbNhitItemModal");
                empdb.notifySuccess(item.NhitItemId > 0 ? "Application updated." : "Application added.");
                // Refresh both the manage table AND the form's checklists
                // so newly-added applications appear without a page reload.
                return reloadManageItems().then(loadItemsThenRender);
            }).catch(function (err) { empdb.notifyError("Save failed: " + err.message); });
        });

        // ---- Submit & email (bottom button)
        $("#empdbNhitSubmit").on("click", function () {
            var dto = buildRequestPayload();
            if (!dto.EmployeeName) { empdb.notifyError("Employee Name is required."); return; }
            empdb.confirmDelete("Submit this worksheet and email it to the helpdesk?").then(function (ok) {
                if (!ok) return;
                doSubmit(dto).then(function (result) {
                    // Navigate back to the main (EmployeeList) view only when
                    // the helpdesk actually got the email. If submit succeeded
                    // but the email layer failed, the request row still exists
                    // in the DB but the user needs to know — stay on the page.
                    if (result && result.EmailSuccess) {
                        navigateToMainView();
                    }
                });
            });
        });

        // ---- Save as Profile (bottom button)
        // Same path as Submit, then either updates the selected profile or
        // prompts for a new profile name and creates one. Profile save fires
        // only after a successful submit so we don't end up with a saved
        // profile but no helpdesk ticket.
        $("#empdbNhitSubmitSaveProfile").on("click", function () {
            var dto = buildRequestPayload();
            if (!dto.EmployeeName) { empdb.notifyError("Employee Name is required."); return; }

            var profileId = selectedProfileId();
            if (profileId > 0) {
                var existingName = selectedProfileName();
                empdb.confirmDelete(
                    "Submit this worksheet to the helpdesk AND update the profile \"" + existingName + "\"?"
                ).then(function (ok) {
                    if (!ok) return;
                    submitThenSaveProfile(dto, profileId, existingName);
                });
            } else {
                // Need a name for the brand-new profile. SweetAlert2 input
                // gives us a clean inline prompt with validation; falls
                // back to window.prompt if Swal isn't loaded.
                askForProfileName().then(function (name) {
                    if (!name) return;
                    submitThenSaveProfile(dto, 0, name);
                });
            }
        });
    }

    // ----------------------------------------------------------------
    //  Submit + (optional) profile save helpers used by both bottom buttons.
    // ----------------------------------------------------------------

    // After a successful Submit (or Submit+SaveProfile), drop the user
    // back on the EmployeeList view. The URL is built server-side and
    // stashed on __empdbCtx.mainViewUrl by NewHireITWorksheet.ascx. The
    // small delay lets the success-toast register before the navigation
    // wipes the page; the user sees the green confirmation flash and
    // then lands on the list.
    function navigateToMainView() {
        var url = ctx().mainViewUrl;
        if (!url) return;
        setTimeout(function () { window.location.href = url; }, 800);
    }

    function doSubmit(dto) {
        return empdb.api.post("NhitRequests/Submit", dto).then(function (result) {
            if (result && result.EmailSuccess) {
                empdb.notifySuccess(result.EmailMessage || ("Worksheet sent to " + (result.EmailSentTo || "helpdesk")));
                return result;
            }
            // Server-side persisted but email layer failed — keep going
            // (the request row was saved) but surface the failure.
            empdb.notifyError("Saved, but email failed: " + (result && result.EmailMessage ? result.EmailMessage : "(unknown)"));
            return result;
        }).catch(function (err) {
            empdb.notifyError("Submit failed: " + err.message);
            throw err;
        });
    }

    function submitThenSaveProfile(dto, profileId, profileName) {
        // Submit must succeed before the profile save fires; if submit
        // throws we surfaced the error in doSubmit and bail out.
        doSubmit(dto).then(function (submitResult) {
            // If the email layer failed, the submit returned successfully
            // but EmailSuccess is false — we surfaced an error toast and
            // should NOT navigate. We DO still save the profile though,
            // since that's a separate concern.
            var emailOk = submitResult && submitResult.EmailSuccess;

            var profileDto = readProfileFromForm(profileName);
            var p = profileId > 0
                ? empdb.api.put("NhitProfiles/" + profileId, profileDto)
                : empdb.api.post("NhitProfiles", profileDto);
            return p.then(function (saved) {
                empdb.notifySuccess(profileId > 0 ? "Profile updated." : "Profile saved.");
                // Navigate back to the main view only when EVERYTHING
                // (submit + email + profile save) succeeded. Email-failure
                // keeps the user on the page so they can see the issue.
                if (emailOk) {
                    navigateToMainView();
                    return;
                }
                // Email failed but profile saved — refresh the dropdown so
                // the new profile is selectable on the next submit attempt.
                if (profileId <= 0 && saved && saved.NhitProfileId) {
                    return loadProfileList().then(function () {
                        $("#empdbNhitProfile").val(saved.NhitProfileId);
                    });
                }
            }).catch(function (err) { empdb.notifyError("Profile save failed: " + err.message); });
        }, function () { /* doSubmit already toasted; nothing to do */ });
    }

    // SweetAlert input modal for the new-profile-name prompt. Returns a
    // Promise<string|null> — null/empty means the user cancelled.
    function askForProfileName() {
        if (typeof window.Swal === "function" || typeof window.Swal === "object") {
            return window.Swal.fire({
                title: "Save as New Profile",
                input: "text",
                inputLabel: "Profile Name",
                inputPlaceholder: "Enter a name for this new profile",
                showCancelButton: true,
                confirmButtonText: "Save & Submit",
                cancelButtonText: "Cancel",
                inputValidator: function (value) {
                    if (!value || !value.trim()) return "Profile name is required";
                    return null;
                }
            }).then(function (r) {
                return r.isConfirmed && r.value ? r.value.trim() : null;
            });
        }
        return new Promise(function (resolve) {
            var name = window.prompt("Profile name:");
            resolve(name && name.trim() ? name.trim() : null);
        });
    }

    // ----------------------------------------------------------------
    //  Manage Profiles modal — list + per-row delete.
    // ----------------------------------------------------------------
    function reloadManageProfiles() {
        return empdb.api.get("NhitProfiles/All").then(function (rows) {
            var $tbody = $("#empdbNhitProfilesTable tbody");
            if (!rows || !rows.length) {
                $tbody.html('<tr><td colspan="2" class="text-muted text-center">No profiles.</td></tr>');
                return;
            }
            $tbody.html(rows.map(function (p) {
                return '<tr data-id="' + p.NhitProfileId + '">' +
                    '<td>' + escapeHtml(p.ProfileName) + '</td>' +
                    '<td class="command-icon"><a href="#" class="text-danger empdb-nhit-profile-delete" title="Delete"><i class="fas fa-trash"></i></a></td>' +
                '</tr>';
            }).join(""));
        });
    }

    $(function () { init(); });

})(window, window.jQuery);
