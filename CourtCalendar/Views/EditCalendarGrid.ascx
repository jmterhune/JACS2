<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EditCalendarGrid.ascx.cs" Inherits="tjc.Modules.CourtCalendar.EditCalendarGrid" %>

<div class="cc-admin cc-editgrid">
    <div class="cc-toolbar mb-3">
        <asp:HyperLink ID="lnkHome" runat="server" CssClass="btn btn-outline-secondary"><i class="fas fa-arrow-left" aria-hidden="true"></i>&nbsp;Back to calendars</asp:HyperLink>
        <asp:HyperLink ID="lnkList" runat="server" CssClass="btn btn-outline-secondary"><i class="fas fa-list" aria-hidden="true"></i>&nbsp;List view</asp:HyperLink>
    </div>

    <asp:Panel ID="pnlDenied" runat="server" Visible="false">
        <div class="alert alert-warning">You do not have permission to edit this calendar.</div>
    </asp:Panel>

    <asp:Panel ID="pnlMain" runat="server">
        <h2 class="cc-h2"><asp:Literal ID="ltTitle" runat="server" /></h2>
        <p class="text-muted cc-g-help">Click a day to add an entry. Drag an entry to reorder it, or drag it onto another day to move it. Hold <strong>Shift</strong> while dragging to copy it.</p>
        <div class="cc-g-message" role="status" aria-live="polite"></div>
        <asp:Literal ID="ltGrid" runat="server" EnableViewState="false" />
    </asp:Panel>

    <%-- Lightweight entry modal (no Bootstrap JS dependency) --%>
    <div id="ccgModal_<%= ModuleId %>" class="ccg-modal" style="display:none;" role="dialog" aria-modal="true" aria-label="Entry">
        <div class="ccg-modal-dialog">
            <div class="ccg-modal-head">
                <strong class="ccg-m-title">Add entry</strong>
                <button type="button" class="btn-close ccg-close" aria-label="Close"></button>
            </div>
            <div class="ccg-modal-body">
                <input type="hidden" class="ccg-f-id" value="0" />
                <div class="row">
                    <div class="col-md-5 mb-2">
                        <label class="form-label">Date</label>
                        <input type="date" class="form-control ccg-f-date" />
                    </div>
                    <div class="col-md-4 mb-2">
                        <label class="form-label">Time</label>
                        <input type="text" class="form-control ccg-f-time" placeholder="9:00 / 9:00-5:00" />
                    </div>
                    <div class="col-md-3 mb-2 d-flex align-items-end">
                        <div class="form-check">
                            <input type="checkbox" class="form-check-input ccg-f-allday" id="ccgAllDay_<%= ModuleId %>" />
                            <label class="form-check-label" for="ccgAllDay_<%= ModuleId %>">All day</label>
                        </div>
                    </div>
                    <div class="col-md-8 mb-2">
                        <label class="form-label">Short title</label>
                        <input type="text" class="form-control ccg-f-title" placeholder="Pleas/Motions" />
                        <small class="form-text text-muted">Please avoid ALL CAPS &ndash; screen readers may read capitalized words letter by letter. Use Bold for emphasis.</small>
                    </div>
                    <div class="col-md-4 mb-2 d-flex align-items-end">
                        <div class="form-check me-3">
                            <input type="checkbox" class="form-check-input ccg-f-bold" id="ccgBold_<%= ModuleId %>" />
                            <label class="form-check-label" for="ccgBold_<%= ModuleId %>"><strong>Bold</strong></label>
                        </div>
                        <div class="form-check">
                            <input type="checkbox" class="form-check-input ccg-f-italic" id="ccgItalic_<%= ModuleId %>" />
                            <label class="form-check-label" for="ccgItalic_<%= ModuleId %>"><em>Italic</em></label>
                        </div>
                    </div>
                    <div class="col-md-8 mb-2">
                        <label class="form-label">Detail (hover popover on the public site)</label>
                        <input type="text" class="form-control ccg-f-detail" placeholder="SPECIAL SET / BARTHOLOMEW" />
                    </div>
                    <div class="col-md-4 mb-2">
                        <label class="form-label">Category</label>
                        <select class="form-select ccg-f-cat"><asp:Literal ID="ltCatOptions" runat="server" EnableViewState="false" /></select>
                    </div>
                </div>
            </div>
            <div class="ccg-modal-foot">
                <button type="button" class="btn btn-primary ccg-save">Save</button>
                <button type="button" class="btn btn-outline-warning ccg-strike" style="display:none;">Strike / unstrike</button>
                <button type="button" class="btn btn-outline-danger ccg-delete" style="display:none;">Delete</button>
                <button type="button" class="btn btn-secondary ccg-close">Cancel</button>
            </div>
        </div>
    </div>
</div>

<script src="https://cdnjs.cloudflare.com/ajax/libs/Sortable/1.15.2/Sortable.min.js"></script>
<script type="text/javascript">
(function ($) {
    var moduleId = <%= ModuleId %>;
    var calendarId = <%= GridCalendarId %>;
    var cats = <asp:Literal ID="ltCatMap" runat="server" EnableViewState="false" />;

    $(function () {
        if (!calendarId) { return; }
        var sf = (typeof $.ServicesFramework === "function") ? $.ServicesFramework(moduleId) : null;
        if (!sf) { return; }
        var serviceRoot = sf.getServiceRoot("CourtCalendar") + "CalendarAdmin/";

        var $root = $("#ccgModal_" + moduleId).closest(".cc-editgrid");
        var grid = $root.find(".cc-g-grid").get(0);
        var $modal = $("#ccgModal_" + moduleId);
        var $msg = $root.find(".cc-g-message");
        var entries = {};          // entryId -> entry object
        var lastDrag = 0;          // timestamp of the last drag end, to suppress click

        function esc(s) { return $("<div>").text(s == null ? "" : s).html(); }

        function note(text, isError) {
            $msg.text(text || "").toggleClass("cc-g-error", !!isError);
            if (text) { setTimeout(function () { $msg.text(""); }, 4000); }
        }

        function post(action, data) {
            return $.ajax({
                url: serviceRoot + action, type: "POST",
                data: JSON.stringify(data), contentType: "application/json",
                beforeSend: sf.setModuleHeaders
            });
        }

        function chipHtml(e) {
            var cat = cats[String(e.categoryId)] || cats["0"];
            var hasColor = cat && cat.color;
            var style = hasColor ? ' style="background-color:' + cat.color + ';border-left-color:' + cat.color + ';"' : '';
            var title = esc(e.shortTitle);
            if (e.titleBold) { title = "<strong>" + title + "</strong>"; }
            if (e.titleItalic) { title = "<em>" + title + "</em>"; }
            var time = (!e.allDay && e.timespan) ? '<span class="cc-g-time">' + esc(e.timespan) + '</span> ' : '';
            var cls = "cc-g-entry" + (e.isStricken ? " cc-g-stricken" : "") + (hasColor ? "" : " cc-g-entry-default");
            return '<li class="' + cls + '"' + style + ' data-id="' + e.entryId + '" tabindex="0">' +
                time + '<span class="cc-g-title">' + title + '</span></li>';
        }

        function listIds(ul) {
            return $(ul).children("li.cc-g-entry").map(function () {
                return parseInt(this.getAttribute("data-id"), 10);
            }).get().filter(function (n) { return n > 0; });
        }

        function render() {
            $(grid).find("ul.cc-g-list").empty();
            Object.keys(entries).map(function (k) { return entries[k]; })
                .sort(function (a, b) {
                    if (a.entryDate !== b.entryDate) { return a.entryDate < b.entryDate ? -1 : 1; }
                    return (a.sortOrder - b.sortOrder) || (a.entryId - b.entryId);
                })
                .forEach(function (e) {
                    var ul = grid.querySelector('ul.cc-g-list[data-date="' + e.entryDate + '"]');
                    if (ul) { ul.insertAdjacentHTML("beforeend", chipHtml(e)); }
                });
        }

        function loadEntries() {
            return $.ajax({
                url: serviceRoot + "Entries", type: "GET", data: { id: calendarId },
                beforeSend: sf.setModuleHeaders
            }).done(function (list) {
                entries = {};
                (list || []).forEach(function (e) { entries[e.entryId] = e; });
                render();
            }).fail(function () { note("Could not load entries.", true); });
        }

        // ---- Drag and drop ----
        function initSortables() {
            $(grid).find("ul.cc-g-list").each(function () {
                Sortable.create(this, {
                    group: "ccg-" + moduleId,
                    animation: 120,
                    draggable: ".cc-g-entry",
                    ghostClass: "cc-g-ghost",
                    onEnd: onDragEnd
                });
            });
        }

        function onDragEnd(evt) {
            lastDrag = Date.now();
            var id = parseInt(evt.item.getAttribute("data-id"), 10);
            var fromDate = evt.from.getAttribute("data-date");
            var toDate = evt.to.getAttribute("data-date");
            var shift = evt.originalEvent && evt.originalEvent.shiftKey;

            if (fromDate === toDate) {
                post("ReorderDay", { calendarId: calendarId, date: toDate, entryIds: listIds(evt.to) })
                    .fail(function () { note("Could not save order.", true); loadEntries(); });
                return;
            }
            if (shift) {
                // Copy: undo the visual move, then create a dated copy on the target.
                var ref = evt.from.children[evt.oldIndex] || null;
                evt.from.insertBefore(evt.item, ref);
                post("CopyEntry", { entryId: id, date: toDate, sortOrder: evt.newIndex })
                    .done(function () { note("Entry copied."); loadEntries(); })
                    .fail(function () { note("Could not copy entry.", true); loadEntries(); });
                return;
            }
            // Move: the DOM already reflects the move; persist date + order of both days.
            post("MoveEntry", { entryId: id, date: toDate, sortOrder: evt.newIndex })
                .done(function () {
                    post("ReorderDay", { calendarId: calendarId, date: toDate, entryIds: listIds(evt.to) });
                    if (entries[id]) { entries[id].entryDate = toDate; }
                })
                .fail(function () { note("Could not move entry.", true); loadEntries(); });
        }

        // ---- Modal ----
        function openModal(title) {
            $modal.find(".ccg-m-title").text(title);
            $modal.css("display", "flex");
        }
        function closeModal() { $modal.css("display", "none"); }

        function openAdd(date) {
            $modal.find(".ccg-f-id").val("0");
            $modal.find(".ccg-f-date").val(date || "");
            $modal.find(".ccg-f-time").val("");
            $modal.find(".ccg-f-allday").prop("checked", false);
            $modal.find(".ccg-f-title").val("");
            $modal.find(".ccg-f-bold").prop("checked", false);
            $modal.find(".ccg-f-italic").prop("checked", false);
            $modal.find(".ccg-f-detail").val("");
            $modal.find(".ccg-f-cat").val("0");
            $modal.find(".ccg-delete, .ccg-strike").hide();
            openModal("Add entry");
            $modal.find(".ccg-f-title").focus();
        }

        function openEdit(id) {
            var e = entries[id];
            if (!e) { return; }
            $modal.find(".ccg-f-id").val(e.entryId);
            $modal.find(".ccg-f-date").val(e.entryDate);
            $modal.find(".ccg-f-time").val(e.timespan || "");
            $modal.find(".ccg-f-allday").prop("checked", !!e.allDay);
            $modal.find(".ccg-f-title").val(e.shortTitle || "");
            $modal.find(".ccg-f-bold").prop("checked", !!e.titleBold);
            $modal.find(".ccg-f-italic").prop("checked", !!e.titleItalic);
            $modal.find(".ccg-f-detail").val(e.detail || "");
            $modal.find(".ccg-f-cat").val(String(e.categoryId));
            $modal.find(".ccg-strike").text(e.isStricken ? "Unstrike" : "Strike").show();
            $modal.find(".ccg-delete").show();
            openModal("Edit entry");
            $modal.find(".ccg-f-title").focus();
        }

        function collect() {
            return {
                entryId: parseInt($modal.find(".ccg-f-id").val(), 10) || 0,
                calendarId: calendarId,
                date: $modal.find(".ccg-f-date").val(),
                timespan: $modal.find(".ccg-f-time").val(),
                allDay: $modal.find(".ccg-f-allday").is(":checked"),
                shortTitle: $modal.find(".ccg-f-title").val(),
                titleBold: $modal.find(".ccg-f-bold").is(":checked"),
                titleItalic: $modal.find(".ccg-f-italic").is(":checked"),
                detail: $modal.find(".ccg-f-detail").val(),
                categoryId: parseInt($modal.find(".ccg-f-cat").val(), 10) || 0,
                sortOrder: 9999
            };
        }

        function save() {
            var dto = collect();
            if (!dto.date) { note("Enter a date.", true); return; }
            if (!dto.shortTitle || !dto.shortTitle.trim()) { note("Short title is required.", true); return; }
            var action = dto.entryId > 0 ? "SaveEntry" : "AddEntry";
            post(action, dto)
                .done(function () { closeModal(); note("Entry saved."); loadEntries(); })
                .fail(function () { note("Could not save the entry.", true); });
        }

        // ---- Events ----
        $(grid).on("click", ".cc-g-add", function (e) {
            e.stopPropagation();
            openAdd(this.getAttribute("data-date"));
        });
        $(grid).on("click", ".cc-g-entry", function () {
            if (Date.now() - lastDrag < 300) { return; }   // ignore the click that ends a drag
            openEdit(parseInt(this.getAttribute("data-id"), 10));
        });
        $(grid).on("keydown", ".cc-g-entry", function (ev) {
            if (ev.key === "Enter") { openEdit(parseInt(this.getAttribute("data-id"), 10)); }
        });

        $modal.on("click", ".ccg-save", save);
        $modal.on("click", ".ccg-close", closeModal);
        $modal.on("click", ".ccg-delete", function () {
            var id = parseInt($modal.find(".ccg-f-id").val(), 10) || 0;
            if (id <= 0) { return; }
            if (!confirm("Delete this entry?")) { return; }
            post("DeleteEntry", { entryId: id })
                .done(function () { closeModal(); note("Entry deleted."); loadEntries(); })
                .fail(function () { note("Could not delete the entry.", true); });
        });
        $modal.on("click", ".ccg-strike", function () {
            var id = parseInt($modal.find(".ccg-f-id").val(), 10) || 0;
            if (id <= 0 || !entries[id]) { return; }
            post("SetStricken", { entryId: id, stricken: !entries[id].isStricken })
                .done(function () { closeModal(); loadEntries(); })
                .fail(function () { note("Could not update the entry.", true); });
        });
        $modal.on("click", function (e) { if (e.target === this) { closeModal(); } });
        $(document).on("keydown", function (e) { if (e.key === "Escape") { closeModal(); } });

        if (typeof Sortable !== "undefined") { initSortables(); }
        loadEntries();
    });
}(jQuery));
</script>
