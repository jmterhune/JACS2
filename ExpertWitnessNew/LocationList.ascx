<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="LocationList.ascx.cs" Inherits="tjc.Modules.ExpertWitness.LocationList" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>

<script type="text/javascript">
    window.__ewCtx = { moduleId: <%= ModuleId %>, tabId: <%= TabId %> };
</script>

<div class="tabs">
    <ul class="nav nav-tabs">
        <li class="nav-item"><a class="nav-link" href="<%=RequestListUrl %>">Requests</a></li>
        <li class="nav-item"><a class="nav-link" href="<%=ExpertListUrl %>">Experts</a></li>
        <li class="nav-item"><a class="nav-link" href="<%=EvaluationTypeListUrl %>">Evaluation Types</a></li>
        <li class="nav-item"><a class="nav-link" href="<%=TypeListUrl %>">Expert Types</a></li>
        <li class="nav-item"><a class="nav-link active" href="#locations" data-bs-toggle="tab">Locations</a></li>
    </ul>
    <div class="tab-content">
        <div id="locations" class="tab-pane active">
            <button type="button" id="ewLocationAdd" class="btn btn-success"><i class="fas fa-plus"></i>&nbsp;Add Location</button>
            <table id="tblLocations" class="table table-striped table-hover ew-admin-table">
                <thead>
                    <tr>
                        <th class="command-item no-sort"></th>
                        <th>ID</th>
                        <th>Location</th>
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

<div class="modal fade" id="LocationEditModal" tabindex="-1" aria-hidden="true">
    <div class="modal-dialog">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title">Add Location</h5>
                <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
            </div>
            <div class="modal-body">
                <input type="hidden" name="LocationID" value="0" />
                <div class="mb-3">
                    <label>Location:</label>
                    <input type="text" name="LocationName" class="form-control" maxlength="50" />
                </div>
            </div>
            <div class="modal-footer">
                <button type="button" id="ewLocationSave" class="btn btn-primary">Save</button>
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
            resource: "Locations",
            idField: "LocationID",
            tableId: "#tblLocations",
            modalId: "LocationEditModal",
            addBtnId: "#ewLocationAdd",
            saveBtnId: "#ewLocationSave",
            editClass: "ew-location-edit",
            delClass: "ew-location-delete",
            colCount: 4,
            order: [[2, "asc"]],
            addTitle: "Add Location",
            editTitle: "Edit Location",
            addedText: "Location added.",
            updatedText: "Location updated.",
            deletedText: "Location deleted.",
            confirmText: "Are you sure you wish to delete this location?",
            rowHtml: function (l) {
                return '<tr data-id="' + l.LocationID + '">' +
                    '<td class="command-item"><a href="#" class="text-primary ew-location-edit" title="Edit"><i class="fas fa-edit"></i></a></td>' +
                    '<td>' + l.LocationID + '</td>' +
                    '<td>' + window.ew.esc(l.LocationName) + '</td>' +
                    '<td class="command-item"><a href="#" class="text-danger ew-location-delete" title="Delete"><i class="fas fa-trash"></i></a></td>' +
                    '</tr>';
            },
            fillForm: function ($m, l) {
                $m.find('[name="LocationID"]').val(l ? l.LocationID : 0);
                $m.find('[name="LocationName"]').val(l ? l.LocationName : "");
            },
            readForm: function ($m) {
                return {
                    LocationID: parseInt($m.find('[name="LocationID"]').val(), 10) || 0,
                    LocationName: $m.find('[name="LocationName"]').val()
                };
            },
            validate: function (d) {
                return (d.LocationName && d.LocationName.trim()) ? null : "Location name is required.";
            }
        }).init();
    });
</script>
