<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TypeList.ascx.cs" Inherits="tjc.Modules.ExpertWitness.TypeList" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>

<script type="text/javascript">
    window.__ewCtx = { moduleId: <%= ModuleId %>, tabId: <%= TabId %> };
</script>

<div class="tabs">
    <ul class="nav nav-tabs">
        <li class="nav-item"><a class="nav-link" href="<%=RequestListUrl %>">Requests</a></li>
        <li class="nav-item"><a class="nav-link" href="<%=ExpertListUrl %>">Experts</a></li>
        <li class="nav-item"><a class="nav-link" href="<%=EvaluationTypeListUrl %>">Evaluation Types</a></li>
        <li class="nav-item"><a class="nav-link active" href="#types" data-bs-toggle="tab">Expert Types</a></li>
        <li class="nav-item"><a class="nav-link" href="<%=LocationListUrl %>">Locations</a></li>
    </ul>
    <div class="tab-content">
        <div id="types" class="tab-pane active">
            <button type="button" id="ewTypeAdd" class="btn btn-success"><i class="fas fa-plus"></i>&nbsp;Add Type</button>
            <table id="tblTypes" class="table table-striped table-hover ew-admin-table">
                <thead>
                    <tr>
                        <th class="command-item no-sort"></th>
                        <th>ID</th>
                        <th>Type</th>
                        <th class="command-item no-sort"></th>
                    </tr>
                </thead>
                <tbody>
                    <tr><td colspan="4" class="text-center text-muted">Loading&hellip;</td></tr>
                </tbody>
            </table>
        </div>
    </div>
</div>

<div class="modal fade" id="TypeEditModal" tabindex="-1" aria-hidden="true">
    <div class="modal-dialog">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title">Add Type</h5>
                <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
            </div>
            <div class="modal-body">
                <input type="hidden" name="TypeID" value="0" />
                <div class="mb-3">
                    <label>Type:</label>
                    <input type="text" name="TypeName" class="form-control" maxlength="50" />
                </div>
            </div>
            <div class="modal-footer">
                <button type="button" id="ewTypeSave" class="btn btn-primary">Save</button>
                <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancel</button>
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
        if (!window.ew || !window.ew.makeAdminTab) return;
        window.ew.makeAdminTab({
            resource: "Types",
            idField: "TypeID",
            tableId: "#tblTypes",
            modalId: "TypeEditModal",
            addBtnId: "#ewTypeAdd",
            saveBtnId: "#ewTypeSave",
            editClass: "ew-type-edit",
            delClass: "ew-type-delete",
            colCount: 4,
            order: [[2, "asc"]],
            addTitle: "Add Type",
            editTitle: "Edit Type",
            addedText: "Type added.",
            updatedText: "Type updated.",
            deletedText: "Type deleted.",
            confirmText: "Are you sure you wish to delete this expert type?",
            rowHtml: function (t) {
                return '<tr data-id="' + t.TypeID + '">' +
                    '<td class="command-item"><a href="#" class="text-primary ew-type-edit" title="Edit"><i class="fas fa-edit"></i></a></td>' +
                    '<td>' + t.TypeID + '</td>' +
                    '<td>' + window.ew.esc(t.TypeName) + '</td>' +
                    '<td class="command-item"><a href="#" class="text-danger ew-type-delete" title="Delete"><i class="fas fa-trash"></i></a></td>' +
                    '</tr>';
            },
            fillForm: function ($m, t) {
                $m.find('[name="TypeID"]').val(t ? t.TypeID : 0);
                $m.find('[name="TypeName"]').val(t ? t.TypeName : "");
            },
            readForm: function ($m) {
                return {
                    TypeID: parseInt($m.find('[name="TypeID"]').val(), 10) || 0,
                    TypeName: $m.find('[name="TypeName"]').val()
                };
            },
            validate: function (d) {
                return (d.TypeName && d.TypeName.trim()) ? null : "Type name is required.";
            }
        }).init();
    });
</script>
