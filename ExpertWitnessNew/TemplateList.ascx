<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TemplateList.ascx.cs" Inherits="tjc.Modules.ExpertWitness.TemplateList" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>

<script type="text/javascript">
    window.__ewCtx = { moduleId: <%= ModuleId %>, tabId: <%= TabId %> };
</script>

<div class="tabs">
    <ul class="nav nav-tabs">
        <li class="nav-item"><a class="nav-link" href="<%=RequestListUrl %>">Requests</a></li>
        <li class="nav-item"><a class="nav-link" href="<%=ExpertListUrl %>">Experts</a></li>
        <li class="nav-item"><a class="nav-link active" href="#evaluations" data-bs-toggle="tab">Evaluation Types</a></li>
        <li class="nav-item"><a class="nav-link" href="<%=TypeListUrl %>">Expert Types</a></li>
        <li class="nav-item"><a class="nav-link" href="<%=LocationListUrl %>">Locations</a></li>
    </ul>
    <div class="tab-content">
        <div id="evaluations" class="tab-pane active">
            <button type="button" id="ewTemplateAdd" class="btn btn-success me-3"><i class="fas fa-plus"></i>&nbsp;Add Evaluation Type</button>
            <table id="tblTemplates" class="table table-striped table-hover ew-admin-table">
                <thead>
                    <tr>
                        <th class="command-item no-sort"></th>
                        <th>ID</th>
                        <th>Evaluation Type</th>
                        <th>Required Expert Types</th>
                        <th class="command-item no-sort"></th>
                    </tr>
                </thead>
                <tbody>
                    <tr><td colspan="5" class="text-center text-muted">Loading&hellip;</td></tr>
                </tbody>
            </table>
        </div>
    </div>
</div>

<div class="modal fade" id="TemplateEditModal" tabindex="-1" aria-hidden="true">
    <div class="modal-dialog modal-lg">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title">Add Evaluation Type</h5>
                <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
            </div>
            <div class="modal-body">
                <input type="hidden" name="TemplateID" value="0" />
                <div class="mb-3">
                    <label>Evaluation Type</label>
                    <input type="text" name="TemplateName" class="form-control" maxlength="200" />
                </div>
                <fieldset class="outline-fieldset">
                    <legend>Add Requirement</legend>
                    <div class="mb-3">
                        <label>Number Required</label>
                        <input type="number" id="ewTemplateNumberRequired" class="form-control" min="1" />
                    </div>
                    <div id="ewTemplateTypes" class="column-2"></div>
                    <p>
                        <button type="button" class="btn btn-outline-primary me-3" id="ewTemplateAddReq">Add Requirement</button>
                    </p>
                </fieldset>
                <table id="tblTemplateReqs" class="table table-striped ew-admin-table">
                    <thead>
                        <tr><th>#</th><th>Expert Types</th><th>Required</th><th class="command-item"></th></tr>
                    </thead>
                    <tbody></tbody>
                </table>
            </div>
            <div class="modal-footer">
                <button type="button" id="ewTemplateSave" class="btn btn-primary me-3">Save</button>
                <button type="button" class="btn btn-secondary me-3" data-bs-dismiss="modal">Cancel</button>
            </div>
        </div>
    </div>
</div>

<dnn:DnnCssInclude runat="server" FilePath="/Resources/Libraries/DataTables/dataTables.bootstrap5.min.css" />
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/DataTables/dataTables.min.js" />
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/DataTables/dataTables.bootstrap5.min.js" />
<dnn:DnnCssInclude runat="server" FilePath="/Resources/Libraries/sweetalert/sweetalert2.min.css" />
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/sweetalert/sweetalert2.all.min.js" />
<dnn:DnnCssInclude runat="server" FilePath="/Resources/Libraries/Noty/noty.min.css" />
<dnn:DnnCssInclude runat="server" FilePath="/Resources/Libraries/Noty/bootstrap-v4.min.css" />
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/Noty/noty.min.js" />
<dnn:DnnCssInclude runat="server" FilePath="~/DesktopModules/tjc.modules/ExpertWitness/module.css" Priority="100" />
<dnn:DnnJsInclude runat="server" FilePath="~/DesktopModules/tjc.modules/ExpertWitness/Scripts/ew-core.js" Priority="200" />
<dnn:DnnJsInclude runat="server" FilePath="~/DesktopModules/tjc.modules/ExpertWitness/Scripts/ew-admin.js" Priority="210" />

<script type="text/javascript">
    jQuery(function () {
        var ew = window.ew;
        if (!ew || !ew.makeAdminTab) return;

        var typeNameById = {};
        var reqs = []; // requirements being built: { numberRequired, typeIds[], typeNames[] }

        function buildTypeChecks(rows) {
            typeNameById = {};
            $("#ewTemplateTypes").html((rows || []).map(function (t) {
                typeNameById[t.TypeID] = t.TypeName;
                return '<div class="form-check">' +
                    '<input class="form-check-input ew-tr-type" type="checkbox" value="' + t.TypeID + '" id="ewtr_' + t.TypeID + '">' +
                    '<label class="form-check-label" for="ewtr_' + t.TypeID + '">' + ew.esc(t.TypeName) + '</label></div>';
            }).join(""));
        }
        function clearBuilder() {
            $("#ewTemplateNumberRequired").val("");
            $("#ewTemplateTypes .ew-tr-type").prop("checked", false);
        }
        function renderReqs() {
            var $b = $("#tblTemplateReqs tbody");
            if (!reqs.length) {
                $b.html('<tr><td colspan="4" class="text-center text-muted">No requirements added.</td></tr>');
                return;
            }
            $b.html(reqs.map(function (r, i) {
                return '<tr><td>' + (i + 1) + '</td>' +
                    '<td>' + ew.esc(r.typeNames.join(", ")) + '</td>' +
                    '<td>' + r.numberRequired + '</td>' +
                    '<td class="command-item"><a href="#" class="text-danger ew-tr-del" data-i="' + i + '" title="Remove"><i class="fas fa-trash"></i></a></td></tr>';
            }).join(""));
        }

        ew.api.get("Types/All").then(function (types) {
            buildTypeChecks(types);
            renderReqs();

            // Add a requirement to the in-progress list.
            $(document).on("click", "#ewTemplateAddReq", function (e) {
                e.preventDefault();
                var num = parseInt($("#ewTemplateNumberRequired").val(), 10);
                var ids = $("#ewTemplateTypes .ew-tr-type:checked").map(function () { return parseInt(this.value, 10); }).get();
                if (!num || num < 1) { ew.notifyError("Enter a valid Number Required."); return; }
                if (!ids.length) { ew.notifyError("Select at least one expert type."); return; }
                reqs.push({ numberRequired: num, typeIds: ids, typeNames: ids.map(function (id) { return typeNameById[id] || id; }) });
                renderReqs();
                clearBuilder();
            });
            // Remove a requirement.
            $(document).on("click", ".ew-tr-del", function (e) {
                e.preventDefault();
                reqs.splice($(this).data("i"), 1);
                renderReqs();
            });

            ew.makeAdminTab({
                resource: "Templates",
                idField: "TemplateID",
                tableId: "#tblTemplates",
                modalId: "TemplateEditModal",
                addBtnId: "#ewTemplateAdd",
                saveBtnId: "#ewTemplateSave",
                editClass: "ew-template-edit",
                delClass: "ew-template-delete",
                colCount: 5,
                order: [[2, "asc"]],
                addTitle: "Add Evaluation Type",
                editTitle: "Edit Evaluation Type",
                addedText: "Evaluation type added.",
                updatedText: "Evaluation type updated.",
                deletedText: "Evaluation type deleted.",
                confirmText: "Are you sure you wish to delete this evaluation type?",
                rowHtml: function (t) {
                    return '<tr data-id="' + t.TemplateID + '">' +
                        '<td class="command-item"><a href="#" class="text-primary ew-template-edit" title="Edit"><i class="fas fa-edit"></i></a></td>' +
                        '<td>' + t.TemplateID + '</td>' +
                        '<td>' + ew.esc(t.TemplateName) + '</td>' +
                        '<td>' + ew.esc(t.TypesRequired) + '</td>' +
                        '<td class="command-item"><a href="#" class="text-danger ew-template-delete" title="Delete"><i class="fas fa-trash"></i></a></td>' +
                        '</tr>';
                },
                fillForm: function ($m, t) {
                    $m.find('[name="TemplateID"]').val(t ? t.TemplateID : 0);
                    $m.find('[name="TemplateName"]').val(t ? t.TemplateName : "");
                    reqs = (t && t.Requirements ? t.Requirements : []).map(function (r) {
                        var ids = r.TypeIDs || [];
                        return { numberRequired: r.NumberRequired, typeIds: ids, typeNames: ids.map(function (id) { return typeNameById[id] || id; }) };
                    });
                    renderReqs();
                    clearBuilder();
                },
                readForm: function ($m) {
                    return {
                        TemplateID: parseInt($m.find('[name="TemplateID"]').val(), 10) || 0,
                        TemplateName: $m.find('[name="TemplateName"]').val(),
                        Requirements: reqs.map(function (r, i) {
                            return { Sequence: i + 1, NumberRequired: r.numberRequired, TypeIDs: r.typeIds };
                        })
                    };
                },
                validate: function (d) {
                    if (!d.TemplateName || !d.TemplateName.trim()) return "Evaluation type name is required.";
                    if (!d.Requirements.length) return "Add at least one requirement.";
                    return null;
                }
            }).init();
        }).catch(function (err) { ew.notifyError("Load failed: " + err.message); });
    });
</script>
