/* EmployeeDB Edit page — API + modal helpers
 *
 * Exposes window.empdb with:
 *   notifySuccess(text)  — green Noty toast
 *   notifyError(text)    — red Noty toast
 *   confirmDelete(text)  — SweetAlert2, returns Promise<boolean>
 *   api.get / post / put / del — fetch wrappers that include the DNN
 *                                AntiForgery token + ModuleId + TabId headers
 *                                so DnnApiController accepts the request.
 *
 * The JSON contract: snake-case-free PascalCase, matching the EmployeeInfo /
 * PhoneInfo / etc. POCOs server-side.
 */
(function (window, $) {
    "use strict";

    var empdb = window.empdb || {};

    /* ------------------------------------------------------------------
     *  Notifications
     * ------------------------------------------------------------------ */
    function noty(type, text, timeout) {
        if (typeof window.Noty !== "function") {
            // Fallback if the lib didn't load — at least surface the message.
            try { console.log("[empdb " + type + "] " + text); } catch (e) {}
            return;
        }
        new window.Noty({
            type: type,
            text: text,
            theme: "bootstrap-v4",
            layout: "topRight",
            timeout: timeout == null ? 3500 : timeout,
            progressBar: true
        }).show();
    }
    empdb.notifySuccess = function (text) { noty("success", text, 3000); };
    empdb.notifyError = function (text) { noty("error", text, 6000); };
    empdb.notifyInfo = function (text) { noty("info", text, 3500); };

    empdb.confirmDelete = function (text) {
        if (typeof window.Swal !== "function" && typeof window.Swal !== "object") {
            return new Promise(function (resolve) { resolve(window.confirm(text)); });
        }
        return window.Swal.fire({
            title: "Are you sure?",
            text: text || "This cannot be undone.",
            icon: "warning",
            showCancelButton: true,
            confirmButtonText: "Yes, delete",
            cancelButtonText: "Cancel",
            confirmButtonColor: "#d33"
        }).then(function (result) { return !!result.isConfirmed; });
    };

    /* ------------------------------------------------------------------
     *  API wrapper
     *
     *  DNN Web API auth needs three headers:
     *     ModuleId
     *     TabId
     *     RequestVerificationToken
     *
     *  Conveniently DNN exposes them via $.dnnSF(moduleId).getAntiForgeryValue()
     *  but to avoid coupling to dnn.js we resolve them from the page meta + a
     *  hidden input that the server emits (see EditEmployee.ascx).
     * ------------------------------------------------------------------ */
    function getCtx() {
        var meta = function (name) { var m = document.querySelector('meta[name="' + name + '"]'); return m ? m.content : ""; };
        var ctx = window.__empdbCtx || {};
        // For employeeId we have to use a null-check rather than `||` —
        // ASP.NET emits 0 for the Add Employee flow (?EmployeeId=0), and
        // `0 || ""` evaluates to "", which sends an empty `?employeeId=`
        // and trips Web API model binding ("The request is invalid"). We
        // want the digit 0 to flow through so the API returns an empty
        // result set instead of a 400.
        var asString = function (v) { return v == null ? "" : String(v); };
        return {
            tabId: ctx.tabId || meta("dnn-tab-id") || "",
            moduleId: ctx.moduleId || meta("dnn-module-id") || "",
            verificationToken: (function () {
                var i = document.querySelector('input[name="__RequestVerificationToken"]');
                return i ? i.value : "";
            })(),
            employeeId: asString(ctx.employeeId),
            // Position / Service History rows are server-keyed by SSN, so the
            // markup ships the loaded employee's SSN through __empdbCtx for the
            // modal Save handlers to read.
            ssn: asString(ctx.ssn)
        };
    }
    empdb.getContext = getCtx;

    function apiCall(method, urlSuffix, body) {
        var ctx = getCtx();
        var url = "/DesktopModules/EmployeeDB/API/" + urlSuffix;
        var opts = {
            method: method,
            credentials: "same-origin",
            headers: {
                "Accept": "application/json",
                "ModuleId": ctx.moduleId,
                "TabId": ctx.tabId,
                "RequestVerificationToken": ctx.verificationToken
            }
        };
        if (body !== undefined && body !== null) {
            opts.headers["Content-Type"] = "application/json";
            opts.body = JSON.stringify(body);
        }
        return fetch(url, opts).then(function (resp) {
            // PhonesController attaches X-Swn-Warning when the per-row SWN
            // sync fails after a successful DB write. Surface it as a Noty
            // warning toast so the user knows SWN is out of sync — the local
            // save still went through.
            var swnWarning = resp.headers.get("X-Swn-Warning");
            if (swnWarning) empdb.notifyError("SWN sync warning: " + swnWarning);

            if (resp.status === 204) return null;
            var ct = resp.headers.get("Content-Type") || "";
            var parse = ct.indexOf("application/json") >= 0 ? resp.json() : resp.text();
            if (!resp.ok) {
                return parse.then(function (payload) {
                    var msg = (payload && (payload.Message || payload.message || payload)) || (resp.status + " " + resp.statusText);
                    throw new Error(typeof msg === "string" ? msg : JSON.stringify(msg));
                });
            }
            return parse;
        });
    }
    empdb.api = {
        get: function (url) { return apiCall("GET", url); },
        post: function (url, body) { return apiCall("POST", url, body); },
        put: function (url, body) { return apiCall("PUT", url, body); },
        del: function (url) { return apiCall("DELETE", url); }
    };

    /* ------------------------------------------------------------------
     *  Display masks for SSN and phone numbers.
     *
     *  Rules of the road:
     *   - The DATABASE stores raw digits only. Any non-digit character
     *     gets stripped server-side by ModelNormalizer.StripDigitsOnly.
     *   - The UI applies a visual mask for legibility:
     *       phoneMask("9415551234")  -> "(941) 555-1234"
     *       ssnMask("123456789")     -> "123-45-6789"
     *   - Inputs carrying the corresponding class get reformatted on each
     *     keystroke; existing server-rendered values are pre-formatted on
     *     DOM-ready so loaded forms don't show raw digits.
     *   - Use digitsOnly() when you need to send only digits over the wire
     *     (defense in depth — the server normalizer would catch it anyway).
     * ------------------------------------------------------------------ */
    function digitsOnly(value) {
        return value == null ? "" : String(value).replace(/\D/g, "");
    }
    empdb.digitsOnly = digitsOnly;

    function phoneMask(value) {
        var digits = digitsOnly(value).slice(0, 10);
        if (digits.length === 0) return "";
        if (digits.length <= 3) return "(" + digits + (digits.length === 3 ? ") " : "");
        if (digits.length <= 6) return "(" + digits.slice(0, 3) + ") " + digits.slice(3);
        return "(" + digits.slice(0, 3) + ") " + digits.slice(3, 6) + "-" + digits.slice(6);
    }
    empdb.phoneMask = phoneMask;

    function ssnMask(value) {
        var digits = digitsOnly(value).slice(0, 9);
        if (digits.length === 0) return "";
        if (digits.length <= 3) return digits;
        if (digits.length <= 5) return digits.slice(0, 3) + "-" + digits.slice(3);
        return digits.slice(0, 3) + "-" + digits.slice(3, 5) + "-" + digits.slice(5);
    }
    empdb.ssnMask = ssnMask;

    function applyMaskOnInput(selector, formatter) {
        $(document).on("input", selector, function () {
            var formatted = formatter(this.value);
            if (this.value !== formatted) {
                this.value = formatted;
                // Park the cursor at the end so the user can keep typing without
                // it jumping back to the middle of the formatting.
                try { this.setSelectionRange(formatted.length, formatted.length); } catch (e) {}
            }
        });
    }
    applyMaskOnInput(".empdb-phone-mask", phoneMask);
    applyMaskOnInput(".empdb-ssn-mask", ssnMask);

    // Pre-format any server-rendered values that are already in the DOM —
    // covers postback re-renders and the SSN field on Page_Load.
    $(function () {
        $(".empdb-phone-mask").each(function () { this.value = phoneMask(this.value); });
        $(".empdb-ssn-mask").each(function () { this.value = ssnMask(this.value); });
    });

    /* ------------------------------------------------------------------
     *  Modal helpers
     * ------------------------------------------------------------------ */
    function showModal(id) {
        var el = document.getElementById(id);
        if (!el || !window.bootstrap) return null;
        var existing = window.bootstrap.Modal.getInstance(el);
        if (existing) { try { existing.dispose(); } catch (e) {} }
        var m = new window.bootstrap.Modal(el);
        m.show();
        return m;
    }
    function hideModal(id) {
        var el = document.getElementById(id);
        if (!el || !window.bootstrap) return;
        var inst = window.bootstrap.Modal.getInstance(el);
        if (inst) { inst.hide(); }
        // belt-and-suspenders: clean up any orphaned backdrop the Porto skin leaves behind
        setTimeout(function () {
            document.querySelectorAll(".modal-backdrop").forEach(function (b) { b.remove(); });
            document.body.classList.remove("modal-open");
            document.body.style.paddingRight = "";
            document.body.style.overflow = "";
        }, 350);
    }
    empdb.showModal = showModal;
    empdb.hideModal = hideModal;

    /* ------------------------------------------------------------------
     *  Phones tab
     *
     *  SWN allowance enforced here AND on the server (PhonesController):
     *   - Max 5 phones per employee with SwnCall checked  (5 voice slots)
     *   - Max 3 phones per employee with SwnText checked  (5 SMS/email slots
     *     minus 2 email addresses on the Details tab)
     *   - SwnText is only valid for PhoneType "Mobile" or "Work Cell".
     * ------------------------------------------------------------------ */
    var phones = (function () {
        var $tbody, $modal;
        // Cache the most recent rows from the server so the modal Save handler
        // can count "other phones with SwnCall/SwnText checked" without making
        // an extra API round-trip for every save.
        var rowsCache = [];

        // Phone types that may receive SMS in SWN. Anything else gets the
        // SwnText checkbox disabled & unchecked the moment the user picks it.
        var SMS_TYPES = { "Mobile": true, "Work Cell": true };
        var MAX_SWN_CALL = 5;
        var MAX_SWN_TEXT = 3;

        function fmtBool(v) { return v ? '<i class="fas fa-check text-success"></i>' : ""; }
        function esc(s) {
            return (s == null ? "" : String(s))
                .replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;")
                .replace(/"/g, "&quot;").replace(/'/g, "&#39;");
        }

        function rowHtml(p) {
            // p.LocationName is an ignored-column property populated by
            // PhonesController on read — no client-side id-to-name lookup.
            return '<tr data-id="' + p.PhoneId + '">' +
                '<td class="command-icon">' +
                    '<a href="#" class="text-primary empdb-phone-edit" title="Edit"><i class="fas fa-edit"></i></a>' +
                '</td>' +
                '<td>' + esc(p.PhoneType) + '</td>' +
                '<td>' + esc(p.LocationName) + '</td>' +
                '<td>' + esc(phoneMask(p.PhoneNumber)) + '</td>' +
                '<td>' + esc(p.Extension) + '</td>' +
                '<td>' + (p.PhoneCascade == null ? "" : p.PhoneCascade) + '</td>' +
                '<td class="text-center">' + fmtBool(p.SwnCall) + '</td>' +
                '<td class="text-center">' + fmtBool(p.SwnText) + '</td>' +
                '<td class="text-center">' + fmtBool(p.SwnExcludeExtension) + '</td>' +
                '<td class="command-icon">' +
                    '<a href="#" class="text-danger empdb-phone-delete" title="Delete"><i class="fas fa-trash"></i></a>' +
                '</td>' +
            '</tr>';
        }

        function reload() {
            var ctx = getCtx();
            return empdb.api.get("Phones/ForEmployee?employeeId=" + encodeURIComponent(ctx.employeeId))
                .then(function (rows) {
                    rowsCache = rows || [];
                    if (!$tbody) return;
                    if (!rowsCache.length) {
                        $tbody.html('<tr><td colspan="10" class="text-muted text-center">No phones on file.</td></tr>');
                        return;
                    }
                    $tbody.html(rowsCache.map(rowHtml).join(""));
                });
        }

        function readForm() {
            var $m = $modal;
            return {
                PhoneId: parseInt($m.find('[name="PhoneId"]').val(), 10) || 0,
                EmployeeId: parseInt(getCtx().employeeId, 10) || 0,
                PhoneType: $m.find('[name="PhoneType"]').val() || "",
                OfficeLocationId: parseInt($m.find('[name="OfficeLocationId"]').val(), 10) || null,
                PhoneNumber: $m.find('[name="PhoneNumber"]').val() || "",
                Extension: $m.find('[name="Extension"]').val() || "",
                PhoneCascade: $m.find('[name="PhoneCascade"]').val()
                    ? parseInt($m.find('[name="PhoneCascade"]').val(), 10) : null,
                SwnCall: $m.find('[name="SwnCall"]').is(":checked"),
                SwnText: $m.find('[name="SwnText"]').is(":checked"),
                SwnExcludeExtension: $m.find('[name="SwnExcludeExtension"]').is(":checked")
            };
        }

        // Disable / uncheck the SWN Text checkbox when the selected PhoneType
        // can't receive SMS. Called on PhoneType change AND from fillForm so
        // that opening an Edit on a non-mobile phone shows the box disabled.
        function syncSmsAvailability() {
            var $m = $modal;
            var type = $m.find('[name="PhoneType"]').val() || "";
            var $text = $m.find('[name="SwnText"]');
            if (SMS_TYPES[type]) {
                $text.prop("disabled", false);
            } else {
                // Untick it so a subsequent type-change doesn't surprise the
                // user with a phantom "still-checked" SwnText they can't see.
                $text.prop("checked", false).prop("disabled", true);
            }
        }

        function fillForm(p) {
            var $m = $modal;
            $m.find('[name="PhoneId"]').val(p ? p.PhoneId : 0);
            $m.find('[name="PhoneType"]').val(p ? (p.PhoneType || "") : "");
            // Empty string here picks the placeholder <option value="">,
            // which clears the previous selection. Use String() so the
            // numeric OfficeLocationId matches the option's string value.
            $m.find('[name="OfficeLocationId"]').val(p && p.OfficeLocationId != null ? String(p.OfficeLocationId) : "");
            // Pre-format the saved number so the modal displays it in the
            // (999) 999-9999 mask instead of the raw stored characters.
            $m.find('[name="PhoneNumber"]').val(p ? phoneMask(p.PhoneNumber) : "");
            $m.find('[name="Extension"]').val(p ? (p.Extension || "") : "");
            $m.find('[name="PhoneCascade"]').val(p && p.PhoneCascade != null ? p.PhoneCascade : "");
            $m.find('[name="SwnCall"]').prop("checked", !!(p && p.SwnCall));
            $m.find('[name="SwnText"]').prop("checked", !!(p && p.SwnText));
            $m.find('[name="SwnExcludeExtension"]').prop("checked", !!(p && p.SwnExcludeExtension));
            $m.find(".modal-title").text(p ? "Edit Phone" : "Add Phone");
            // Sync after the type has been set so the disabled state of the
            // SwnText checkbox reflects the loaded row's PhoneType.
            syncSmsAvailability();
        }

        // Returns an error string if the proposed save violates SWN allowance,
        // or null if the row is OK to send. Counts other rows from rowsCache,
        // skipping the row currently being edited (matched by PhoneId).
        function validateSwnLimits(data) {
            // SwnText only allowed for Mobile / Work Cell. UI prevents the
            // checkbox being ticked, but the user could still toggle it via
            // dev tools, so re-check here defensively.
            if (data.SwnText && !SMS_TYPES[data.PhoneType]) {
                return "SWN Text is only allowed for Mobile or Work Cell phones.";
            }

            var callCount = data.SwnCall ? 1 : 0;
            var textCount = data.SwnText ? 1 : 0;
            for (var i = 0; i < rowsCache.length; i++) {
                var r = rowsCache[i];
                // Skip the row we're editing — its updated values are already
                // counted above. New rows have PhoneId 0 and won't match.
                if (data.PhoneId > 0 && r.PhoneId === data.PhoneId) continue;
                if (r.SwnCall) callCount++;
                if (r.SwnText) textCount++;
            }
            if (callCount > MAX_SWN_CALL) {
                return "An employee can have at most " + MAX_SWN_CALL + " phones with SWN Call checked.";
            }
            if (textCount > MAX_SWN_TEXT) {
                return "An employee can have at most " + MAX_SWN_TEXT + " phones with SWN Text checked (the two email addresses count toward the SWN 5-text/email limit).";
            }
            return null;
        }

        function init() {
            $tbody = $("#empdbPhonesTable tbody");
            $modal = $("#empdbPhoneModal");
            if (!$tbody.length || !$modal.length) return;

            // Add
            $(document).on("click", "#empdbPhoneAdd", function (e) {
                e.preventDefault();
                fillForm(null);
                showModal("empdbPhoneModal");
            });

            // PhoneType change -> recalculate SwnText availability.
            $modal.on("change", '[name="PhoneType"]', function () { syncSmsAvailability(); });

            // Row Edit
            $tbody.on("click", ".empdb-phone-edit", function (e) {
                e.preventDefault();
                var id = $(this).closest("tr").data("id");
                empdb.api.get("Phones/" + encodeURIComponent(id)).then(function (p) {
                    fillForm(p);
                    showModal("empdbPhoneModal");
                }).catch(function (err) { empdb.notifyError("Could not load phone: " + err.message); });
            });

            // Row Delete
            $tbody.on("click", ".empdb-phone-delete", function (e) {
                e.preventDefault();
                var $row = $(this).closest("tr");
                var id = $row.data("id");
                empdb.confirmDelete("Delete this phone?").then(function (ok) {
                    if (!ok) return;
                    empdb.api.del("Phones/" + encodeURIComponent(id)).then(function () {
                        empdb.notifySuccess("Phone deleted.");
                        reload();
                    }).catch(function (err) { empdb.notifyError("Delete failed: " + err.message); });
                });
            });

            // Modal Save
            $modal.on("click", "#empdbPhoneSave", function (e) {
                e.preventDefault();
                var data = readForm();
                if (!data.PhoneType) { empdb.notifyError("Phone Type is required."); return; }
                if (!data.PhoneNumber) { empdb.notifyError("Phone Number is required."); return; }
                var swnError = validateSwnLimits(data);
                if (swnError) { empdb.notifyError(swnError); return; }
                var p = data.PhoneId > 0
                    ? empdb.api.put("Phones/" + data.PhoneId, data)
                    : empdb.api.post("Phones", data);
                p.then(function () {
                    hideModal("empdbPhoneModal");
                    // Clear the modal so the next open (Add or Edit) can't
                    // inherit stale values like a dropdown selection.
                    fillForm(null);
                    empdb.notifySuccess(data.PhoneId > 0 ? "Phone updated." : "Phone added.");
                    reload();
                }).catch(function (err) { empdb.notifyError("Save failed: " + err.message); });
            });

            // Also clear when the user dismisses the modal (Cancel / X / ESC),
            // so an aborted Edit doesn't bleed values into the next Add.
            $modal.on("hidden.bs.modal", function () { fillForm(null); });

            reload();
        }

        return { init: init, reload: reload };
    })();
    empdb.phones = phones;

    /* ------------------------------------------------------------------
     *  Generic CRUD-tab factory.
     *
     *  Each tab on the Edit page (Phones, Positions, Services, Contacts) is the
     *  same shape: an HTML table populated from /api/{Resource}/ForEmployee, an
     *  Add button, an Edit pencil per row, a Delete trash per row, and a single
     *  Bootstrap modal whose <input>/<select>/<textarea> elements are read by
     *  `name=`. This factory wires all of that up given a small config block.
     * ------------------------------------------------------------------ */
    function esc(s) {
        return (s == null ? "" : String(s))
            .replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;")
            .replace(/"/g, "&quot;").replace(/'/g, "&#39;");
    }
    function fmtBool(v) { return v ? '<i class="fas fa-check text-success"></i>' : ""; }
    function fmtDate(d) {
        if (!d) return "";
        // PetaPoco serializes DateTime as ISO 8601; trim to yyyy-MM-dd.
        var m = String(d).match(/^(\d{4})-(\d{2})-(\d{2})/);
        return m ? m[2] + "/" + m[3] + "/" + m[1] : "";
    }
    function isoDate(d) {
        if (!d) return "";
        var m = String(d).match(/^(\d{4})-(\d{2})-(\d{2})/);
        return m ? m[0] : "";
    }
    function fmtMoney(n) {
        if (n == null || n === "") return "";
        var num = Number(n);
        if (isNaN(num)) return "";
        return "$" + num.toFixed(2);
    }

    function makeCrudTab(cfg) {
        // cfg shape:
        //   resource:    "Phones" | "Positions" | "Services" | "Contacts"
        //   idField:     primary-key property on the DTO (e.g. "PositionId")
        //   tableId:     "#empdbXxxTable"
        //   modalId:     "empdbXxxModal"
        //   addBtnId:    "#empdbXxxAdd"
        //   saveBtnId:   "#empdbXxxSave"
        //   colCount:    number of columns in the empty/loading row
        //   rowHtml:     function(item) -> <tr>...</tr>
        //   readForm:    function($modal) -> object to POST/PUT
        //   fillForm:    function($modal, item|null) — populate inputs (null = Add)
        //   addTitle:    e.g. "Add Phone"
        //   editTitle:   e.g. "Edit Phone"
        //   editClass:   row-edit anchor class, e.g. "empdb-phone-edit"
        //   delClass:    row-delete anchor class, e.g. "empdb-phone-delete"
        //   confirmText: passed to swal confirm
        //   savedAdded:  toast text after a successful POST
        //   savedUpdated:toast text after a successful PUT
        //   deletedText: toast text after a successful DELETE
        //   validate:    optional function(data) -> error string or null
        var $tbody, $modal;

        function reload() {
            var ctx = getCtx();
            return empdb.api.get(cfg.resource + "/ForEmployee?employeeId=" + encodeURIComponent(ctx.employeeId))
                .then(function (rows) {
                    if (!$tbody) return;
                    if (!rows || !rows.length) {
                        $tbody.html('<tr><td colspan="' + cfg.colCount + '" class="text-muted text-center">No records.</td></tr>');
                        return;
                    }
                    $tbody.html(rows.map(cfg.rowHtml).join(""));
                })
                .catch(function (err) { empdb.notifyError("Load failed: " + err.message); });
        }

        function init() {
            $tbody = $(cfg.tableId + " tbody");
            $modal = $("#" + cfg.modalId);
            if (!$tbody.length || !$modal.length) return;

            $(document).on("click", cfg.addBtnId, function (e) {
                e.preventDefault();
                cfg.fillForm($modal, null);
                $modal.find(".modal-title").text(cfg.addTitle);
                $modal.find('input[name="' + cfg.idField + '"]').val(0);
                showModal(cfg.modalId);
            });

            $tbody.on("click", "." + cfg.editClass, function (e) {
                e.preventDefault();
                var id = $(this).closest("tr").data("id");
                empdb.api.get(cfg.resource + "/" + encodeURIComponent(id)).then(function (item) {
                    cfg.fillForm($modal, item);
                    $modal.find(".modal-title").text(cfg.editTitle);
                    showModal(cfg.modalId);
                }).catch(function (err) { empdb.notifyError("Load failed: " + err.message); });
            });

            $tbody.on("click", "." + cfg.delClass, function (e) {
                e.preventDefault();
                var $row = $(this).closest("tr");
                var id = $row.data("id");
                empdb.confirmDelete(cfg.confirmText).then(function (ok) {
                    if (!ok) return;
                    empdb.api.del(cfg.resource + "/" + encodeURIComponent(id)).then(function () {
                        empdb.notifySuccess(cfg.deletedText);
                        reload();
                    }).catch(function (err) { empdb.notifyError("Delete failed: " + err.message); });
                });
            });

            $modal.on("click", cfg.saveBtnId, function (e) {
                e.preventDefault();
                var data = cfg.readForm($modal);
                if (cfg.validate) {
                    var err = cfg.validate(data);
                    if (err) { empdb.notifyError(err); return; }
                }
                var idVal = data[cfg.idField] || 0;
                var p = idVal > 0
                    ? empdb.api.put(cfg.resource + "/" + idVal, data)
                    : empdb.api.post(cfg.resource, data);
                p.then(function () {
                    hideModal(cfg.modalId);
                    empdb.notifySuccess(idVal > 0 ? cfg.savedUpdated : cfg.savedAdded);
                    reload();
                }).catch(function (err) { empdb.notifyError("Save failed: " + err.message); });
            });

            reload();
        }

        return { init: init, reload: reload };
    }

    /* ------------------------------------------------------------------
     *  Position History
     * ------------------------------------------------------------------ */
    var entryTypeLabels = { "T": "Transfer", "P": "Promotion", "O": "Other" };
    var positions = makeCrudTab({
        resource: "Positions",
        idField: "PositionId",
        tableId: "#empdbPositionsTable",
        modalId: "empdbPositionModal",
        addBtnId: "#empdbPositionAdd",
        saveBtnId: "#empdbPositionSave",
        colCount: 7,
        editClass: "empdb-position-edit",
        delClass: "empdb-position-delete",
        addTitle: "Add Position",
        editTitle: "Edit Position",
        confirmText: "Delete this position history entry?",
        savedAdded: "Position added.",
        savedUpdated: "Position updated.",
        deletedText: "Position deleted.",
        rowHtml: function (p) {
            // Column order is kept in sync with EditEmployee.ascx's <thead>:
            //   [edit] Entry Type | Description | Internal/External | Start Date | End Date [delete]
            return '<tr data-id="' + p.PositionId + '">' +
                '<td class="command-icon"><a href="#" class="text-primary empdb-position-edit" title="Edit"><i class="fas fa-edit"></i></a></td>' +
                '<td>' + esc(entryTypeLabels[p.EntryType] || p.EntryType || "") + '</td>' +
                '<td>' + esc(p.Description) + '</td>' +
                '<td>' + (p.IsInternal ? "Internal" : "External") + '</td>' +
                '<td>' + fmtDate(p.StartDate) + '</td>' +
                '<td>' + fmtDate(p.EndDate) + '</td>' +
                '<td class="command-icon"><a href="#" class="text-danger empdb-position-delete" title="Delete"><i class="fas fa-trash"></i></a></td>' +
            '</tr>';
        },
        fillForm: function ($m, p) {
            $m.find('[name="PositionId"]').val(p ? p.PositionId : 0);
            $m.find('[name="StartDate"]').val(p ? isoDate(p.StartDate) : "");
            $m.find('[name="EndDate"]').val(p ? isoDate(p.EndDate) : "");
            $m.find('[name="Description"]').val(p ? p.Description || "" : "");
            $m.find('[name="EntryType"]').val(p ? p.EntryType || "" : "");
            $m.find('[name="IsInternal"]').val(p ? (p.IsInternal ? "true" : "false") : "true");
        },
        readForm: function ($m) {
            var ctx = getCtx();
            return {
                PositionId: parseInt($m.find('[name="PositionId"]').val(), 10) || 0,
                // The server expects SocialSecurityNumber, but we don't know it
                // client-side. Page_Load injected it on window.__empdbCtx so the
                // POST/PUT can pass it through.
                SocialSecurityNumber: ctx.ssn || "",
                StartDate: $m.find('[name="StartDate"]').val() || null,
                EndDate: $m.find('[name="EndDate"]').val() || null,
                Description: $m.find('[name="Description"]').val() || "",
                EntryType: $m.find('[name="EntryType"]').val() || "",
                IsInternal: $m.find('[name="IsInternal"]').val() === "true"
            };
        },
        validate: function (data) {
            if (!data.SocialSecurityNumber) return "Save the employee (with an SSN) first.";
            return null;
        }
    });
    empdb.positions = positions;

    /* ------------------------------------------------------------------
     *  Service History
     * ------------------------------------------------------------------ */
    var services = makeCrudTab({
        resource: "Services",
        idField: "ServiceId",
        tableId: "#empdbServicesTable",
        modalId: "empdbServiceModal",
        addBtnId: "#empdbServiceAdd",
        saveBtnId: "#empdbServiceSave",
        colCount: 6,
        editClass: "empdb-service-edit",
        delClass: "empdb-service-delete",
        addTitle: "Add Service",
        editTitle: "Edit Service",
        confirmText: "Delete this service history entry?",
        savedAdded: "Service added.",
        savedUpdated: "Service updated.",
        deletedText: "Service deleted.",
        rowHtml: function (s) {
            return '<tr data-id="' + s.ServiceId + '">' +
                '<td class="command-icon"><a href="#" class="text-primary empdb-service-edit" title="Edit"><i class="fas fa-edit"></i></a></td>' +
                '<td>' + esc(s.CompanyName) + '</td>' +
                '<td>' + fmtDate(s.HireDate) + '</td>' +
                '<td>' + fmtDate(s.TerminationDate) + '</td>' +
                '<td>' + fmtMoney(s.LastPayRate) + '</td>' +
                '<td class="command-icon"><a href="#" class="text-danger empdb-service-delete" title="Delete"><i class="fas fa-trash"></i></a></td>' +
            '</tr>';
        },
        fillForm: function ($m, s) {
            $m.find('[name="ServiceId"]').val(s ? s.ServiceId : 0);
            $m.find('[name="CompanyName"]').val(s ? s.CompanyName || "" : "");
            $m.find('[name="HireDate"]').val(s ? isoDate(s.HireDate) : "");
            $m.find('[name="TerminationDate"]').val(s ? isoDate(s.TerminationDate) : "");
            $m.find('[name="LastPayRate"]').val(s && s.LastPayRate != null ? s.LastPayRate : "");
        },
        readForm: function ($m) {
            var ctx = getCtx();
            return {
                ServiceId: parseInt($m.find('[name="ServiceId"]').val(), 10) || 0,
                SocialSecurityNumber: ctx.ssn || "",
                CompanyName: $m.find('[name="CompanyName"]').val() || "",
                HireDate: $m.find('[name="HireDate"]').val() || null,
                TerminationDate: $m.find('[name="TerminationDate"]').val() || null,
                LastPayRate: $m.find('[name="LastPayRate"]').val()
                    ? parseFloat($m.find('[name="LastPayRate"]').val()) : null
            };
        },
        validate: function (data) {
            if (!data.SocialSecurityNumber) return "Save the employee (with an SSN) first.";
            return null;
        }
    });
    empdb.services = services;

    /* ------------------------------------------------------------------
     *  Emergency Contacts
     * ------------------------------------------------------------------ */
    var contacts = makeCrudTab({
        resource: "Contacts",
        idField: "ContactId",
        tableId: "#empdbContactsTable",
        modalId: "empdbContactModal",
        addBtnId: "#empdbContactAdd",
        saveBtnId: "#empdbContactSave",
        colCount: 9,
        editClass: "empdb-contact-edit",
        delClass: "empdb-contact-delete",
        addTitle: "Add Emergency Contact",
        editTitle: "Edit Emergency Contact",
        confirmText: "Delete this contact?",
        savedAdded: "Contact added.",
        savedUpdated: "Contact updated.",
        deletedText: "Contact deleted.",
        rowHtml: function (c) {
            return '<tr data-id="' + c.ContactId + '">' +
                '<td class="command-icon"><a href="#" class="text-primary empdb-contact-edit" title="Edit"><i class="fas fa-edit"></i></a></td>' +
                '<td>' + esc(c.FirstName) + '</td>' +
                '<td>' + esc(c.LastName) + '</td>' +
                '<td>' + esc(c.Relationship) + '</td>' +
                '<td>' + esc(phoneMask(c.PhoneHome)) + '</td>' +
                '<td>' + esc(phoneMask(c.PhoneWork)) + '</td>' +
                '<td>' + esc(phoneMask(c.PhoneMobile)) + '</td>' +
                '<td>' + (c.CallOrder == null ? "" : c.CallOrder) + '</td>' +
                '<td class="command-icon"><a href="#" class="text-danger empdb-contact-delete" title="Delete"><i class="fas fa-trash"></i></a></td>' +
            '</tr>';
        },
        fillForm: function ($m, c) {
            $m.find('[name="ContactId"]').val(c ? c.ContactId : 0);
            $m.find('[name="FirstName"]').val(c ? c.FirstName || "" : "");
            $m.find('[name="LastName"]').val(c ? c.LastName || "" : "");
            $m.find('[name="Relationship"]').val(c ? c.Relationship || "" : "");
            $m.find('[name="PhoneHome"]').val(c ? phoneMask(c.PhoneHome) : "");
            $m.find('[name="PhoneWork"]').val(c ? phoneMask(c.PhoneWork) : "");
            $m.find('[name="PhoneMobile"]').val(c ? phoneMask(c.PhoneMobile) : "");
            $m.find('[name="CallOrder"]').val(c && c.CallOrder != null ? c.CallOrder : "");
        },
        readForm: function ($m) {
            var ctx = getCtx();
            return {
                ContactId: parseInt($m.find('[name="ContactId"]').val(), 10) || 0,
                EmployeeId: parseInt(ctx.employeeId, 10) || 0,
                FirstName: $m.find('[name="FirstName"]').val() || "",
                LastName: $m.find('[name="LastName"]').val() || "",
                Relationship: $m.find('[name="Relationship"]').val() || "",
                PhoneHome: $m.find('[name="PhoneHome"]').val() || "",
                PhoneWork: $m.find('[name="PhoneWork"]').val() || "",
                PhoneMobile: $m.find('[name="PhoneMobile"]').val() || "",
                CallOrder: $m.find('[name="CallOrder"]').val()
                    ? parseInt($m.find('[name="CallOrder"]').val(), 10) : null
            };
        },
        validate: function (data) {
            if (data.EmployeeId <= 0) return "Save the employee first.";
            return null;
        }
    });
    empdb.contacts = contacts;

    /* ------------------------------------------------------------------
     *  Groups tab — dual-list (selected / available) with move buttons + DnD
     * ------------------------------------------------------------------ */
    var groups = (function () {
        var $sel, $avail, $saveBtn;
        var loaded = false;

        function listItemHtml(g) {
            return '<li class="empdb-dual-list-item" draggable="true" data-id="' + g.GroupID + '">' +
                esc(g.GroupName) + '</li>';
        }

        function emptyMsg($list, msg) {
            $list.html('<li class="empdb-dual-list-empty text-muted">' + esc(msg) + '</li>');
        }

        function renderList($list, items, emptyText) {
            if (!items || !items.length) {
                emptyMsg($list, emptyText);
                return;
            }
            $list.html(items.map(listItemHtml).join(""));
        }

        function reload() {
            var ctx = getCtx();
            return empdb.api.get("Memberships/ForEmployee?employeeId=" + encodeURIComponent(ctx.employeeId))
                .then(function (state) {
                    renderList($sel, state.Selected, "No groups assigned.");
                    renderList($avail, state.Available, "No groups available.");
                    loaded = true;
                })
                .catch(function (err) { empdb.notifyError("Could not load groups: " + err.message); });
        }

        // Move helpers — operate on jQuery sets (so the same code path handles
        // single-row "selected" moves and bulk "all" moves).
        function move($items, $target) {
            if (!$items || !$items.length) return;
            // Strip the "(empty)" placeholder if it's there.
            $target.find(".empdb-dual-list-empty").remove();
            $items.each(function () { $target.append(this); });
            // Restore placeholders if a list ended up empty.
            ensureEmptyPlaceholders();
        }
        function ensureEmptyPlaceholders() {
            if (!$sel.find(".empdb-dual-list-item").length) emptyMsg($sel, "No groups assigned.");
            if (!$avail.find(".empdb-dual-list-item").length) emptyMsg($avail, "No groups available.");
        }

        function selectedSelected() { return $sel.find(".empdb-dual-list-item.is-selected"); }
        function selectedAvailable() { return $avail.find(".empdb-dual-list-item.is-selected"); }
        function clearSelection() {
            $sel.find(".is-selected").removeClass("is-selected");
            $avail.find(".is-selected").removeClass("is-selected");
        }

        function save() {
            var ctx = getCtx();
            var empId = parseInt(ctx.employeeId, 10) || 0;
            if (empId <= 0) {
                empdb.notifyError("Save the employee first before assigning groups.");
                return;
            }
            var ids = $sel.find(".empdb-dual-list-item").map(function () {
                return parseInt(this.getAttribute("data-id"), 10);
            }).get().filter(function (n) { return n > 0; });

            empdb.api.post("Memberships/Save", { EmployeeId: empId, GroupIds: ids })
                .then(function () { empdb.notifySuccess("Group membership saved."); })
                .catch(function (err) { empdb.notifyError("Save failed: " + err.message); });
        }

        function init() {
            $sel = $("#empdbGroupsSelected");
            $avail = $("#empdbGroupsAvailable");
            $saveBtn = $("#empdbGroupSave");
            if (!$sel.length || !$avail.length) return;

            // Click an item -> toggle .is-selected (with shift/ctrl support).
            $sel.add($avail).on("click", ".empdb-dual-list-item", function (e) {
                if (!e.ctrlKey && !e.metaKey && !e.shiftKey) {
                    $(this).siblings(".empdb-dual-list-item.is-selected").removeClass("is-selected");
                }
                $(this).toggleClass("is-selected");
            });
            // Double-click -> immediate move to other side.
            $sel.on("dblclick", ".empdb-dual-list-item", function () {
                move($(this), $avail);
            });
            $avail.on("dblclick", ".empdb-dual-list-item", function () {
                move($(this), $sel);
            });

            // Buttons
            $(document).on("click", "#empdbGroupAdd", function (e) {
                e.preventDefault();
                move(selectedAvailable(), $sel);
                clearSelection();
            });
            $(document).on("click", "#empdbGroupAddAll", function (e) {
                e.preventDefault();
                move($avail.find(".empdb-dual-list-item"), $sel);
                clearSelection();
            });
            $(document).on("click", "#empdbGroupRemove", function (e) {
                e.preventDefault();
                move(selectedSelected(), $avail);
                clearSelection();
            });
            $(document).on("click", "#empdbGroupRemoveAll", function (e) {
                e.preventDefault();
                move($sel.find(".empdb-dual-list-item"), $avail);
                clearSelection();
            });
            $(document).on("click", "#empdbGroupSave", function (e) {
                e.preventDefault();
                save();
            });

            // HTML5 drag-and-drop. We bind on the two list containers; the
            // dragged element gets a temporary id we read back in `drop`.
            var dragEl = null;
            $sel.add($avail).on("dragstart", ".empdb-dual-list-item", function (e) {
                dragEl = this;
                $(this).addClass("is-dragging");
                try { e.originalEvent.dataTransfer.effectAllowed = "move"; } catch (_) {}
                try { e.originalEvent.dataTransfer.setData("text/plain", this.getAttribute("data-id")); } catch (_) {}
            });
            $sel.add($avail).on("dragend", ".empdb-dual-list-item", function () {
                $(this).removeClass("is-dragging");
                dragEl = null;
            });
            $sel.add($avail).on("dragover", function (e) {
                e.preventDefault();
                $(this).addClass("is-drag-over");
                try { e.originalEvent.dataTransfer.dropEffect = "move"; } catch (_) {}
            });
            $sel.add($avail).on("dragleave", function () {
                $(this).removeClass("is-drag-over");
            });
            $sel.add($avail).on("drop", function (e) {
                e.preventDefault();
                $(this).removeClass("is-drag-over");
                if (!dragEl) return;
                // Only move if it actually changed sides.
                if (this !== dragEl.parentNode) {
                    $(this).find(".empdb-dual-list-empty").remove();
                    this.appendChild(dragEl);
                    ensureEmptyPlaceholders();
                }
            });

            reload();
        }

        return { init: init, reload: reload };
    })();
    empdb.groups = groups;

    /* ------------------------------------------------------------------
     *  Photo tab — drag-drop upload + remove
     * ------------------------------------------------------------------ */
    var photo = (function () {
        var $drop, $file, $img, $empty, $remove;

        function setPreview(url) {
            if (url) {
                $img.attr("src", url + (url.indexOf("?") >= 0 ? "&" : "?") + "v=" + Date.now()).show();
                $empty.hide();
                $remove.show();
            } else {
                $img.attr("src", "").hide();
                $empty.show();
                $remove.hide();
            }
        }

        function upload(file) {
            if (!file) return;
            if (!/^image\//i.test(file.type)) {
                empdb.notifyError("Only image files can be uploaded.");
                return;
            }
            var ctx = getCtx();
            var empId = parseInt(ctx.employeeId, 10) || 0;
            if (empId <= 0) {
                empdb.notifyError("Save the employee first before uploading a photo.");
                return;
            }

            var fd = new FormData();
            fd.append("employeeId", String(empId));
            fd.append("file", file, file.name);

            // We can't go through apiCall() because the body is FormData, not
            // JSON. Build the headers manually so DNN's Web API accepts it.
            $drop.addClass("is-uploading");
            fetch("/DesktopModules/EmployeeDB/API/Photos/Upload", {
                method: "POST",
                credentials: "same-origin",
                headers: {
                    "ModuleId": ctx.moduleId,
                    "TabId": ctx.tabId,
                    "RequestVerificationToken": ctx.verificationToken,
                    "Accept": "application/json"
                },
                body: fd
            }).then(function (resp) {
                $drop.removeClass("is-uploading");
                if (!resp.ok) {
                    return resp.text().then(function (t) { throw new Error(t || (resp.status + " " + resp.statusText)); });
                }
                return resp.json();
            }).then(function (payload) {
                setPreview(payload && payload.Url);
                empdb.notifySuccess("Photo uploaded.");
            }).catch(function (err) {
                $drop.removeClass("is-uploading");
                empdb.notifyError("Upload failed: " + err.message);
            });
        }

        function removePhoto() {
            var ctx = getCtx();
            var empId = parseInt(ctx.employeeId, 10) || 0;
            if (empId <= 0) return;
            empdb.confirmDelete("Remove this photo?").then(function (ok) {
                if (!ok) return;
                empdb.api.del("Photos/" + empId).then(function () {
                    setPreview("");
                    empdb.notifySuccess("Photo removed.");
                }).catch(function (err) { empdb.notifyError("Remove failed: " + err.message); });
            });
        }

        function init() {
            $drop = $("#empdbPhotoDrop");
            $file = $("#empdbPhotoFile");
            $img = $("#empdbPhotoImg");
            $empty = $("#empdbPhotoEmpty");
            $remove = $("#empdbPhotoRemove");
            if (!$drop.length) return;

            // Click drop zone -> open file picker.
            //
            // The <input type="file"> lives INSIDE $drop, so any click we
            // synthesize on $file bubbles back up to $drop. Without the
            // e.target === $file[0] guard, $drop's click handler re-fires
            // for every bubbled click, producing infinite recursion
            // ("Maximum call stack size exceeded"). Native .click() on the
            // raw DOM element also avoids jQuery's event-trigger pipeline
            // and is what reliably pops the OS file dialog.
            $drop.on("click", function (e) {
                if (e.target === $file[0]) return; // ignore the synthetic click bubble
                e.preventDefault();
                $file[0].click();
            });
            $drop.on("keydown", function (e) {
                if (e.key === "Enter" || e.key === " ") {
                    e.preventDefault();
                    $file[0].click();
                }
            });
            $file.on("change", function () {
                if (this.files && this.files[0]) {
                    upload(this.files[0]);
                    this.value = ""; // allow re-upload of same file
                }
            });

            // Drag and drop.
            $drop.on("dragover dragenter", function (e) {
                e.preventDefault();
                e.stopPropagation();
                $drop.addClass("is-drag-over");
            });
            $drop.on("dragleave dragend drop", function (e) {
                e.preventDefault();
                e.stopPropagation();
                $drop.removeClass("is-drag-over");
            });
            $drop.on("drop", function (e) {
                var dt = e.originalEvent.dataTransfer;
                if (dt && dt.files && dt.files.length) upload(dt.files[0]);
            });

            // Remove button.
            $remove.on("click", function (e) {
                e.preventDefault();
                removePhoto();
            });

            // Block accidental browser-wide image drops outside the zone.
            $(document).on("dragover drop", function (e) {
                if (e.target && $(e.target).closest("#empdbPhotoDrop").length) return;
                e.preventDefault();
            });
        }

        return { init: init };
    })();
    empdb.photo = photo;

    /* ------------------------------------------------------------------
     *  Bootstrap on DOM ready
     * ------------------------------------------------------------------ */
    $(function () {
        if (document.getElementById("EmployeeEditForm")) {
            phones.init();
            positions.init();
            services.init();
            contacts.init();
            groups.init();
            photo.init();
        }
    });

    window.empdb = empdb;
})(window, window.jQuery);
