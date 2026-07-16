<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RequestList.ascx.cs" Inherits="tjc.Modules.ExpertWitness.RequestList" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>

<script type="text/javascript">
    window.__ewCtx = { moduleId: <%= ModuleId %>, tabId: <%= TabId %> };
</script>

<div class="tabs">
    <ul class="nav nav-tabs">
        <li class="nav-item"><a class="nav-link active" href="#requests" data-bs-toggle="tab">Requests</a></li>
        <li class="nav-item"><a class="nav-link" href="<%=ExpertListUrl %>">Experts</a></li>
        <li class="nav-item"><a class="nav-link" href="<%=EvaluationTypeListUrl %>">Evaluation Types</a></li>
        <li class="nav-item"><a class="nav-link" href="<%=TypeListUrl %>">Expert Types</a></li>
        <li class="nav-item"><a class="nav-link" href="<%=LocationListUrl %>">Locations</a></li>
    </ul>
    <div class="tab-content">
        <div id="requests" class="tab-pane active">
            <table id="tblRequests" class="table table-striped table-hover ew-admin-table">
                <thead>
                    <tr>
                        <th class="command-item no-sort"></th>
                        <th class="ew-id-col">ID</th>
                        <th>Case Number</th>
                        <th>Evaluation Type</th>
                        <th>Location</th>
                        <th>Submitted By</th>
                        <th>Date Submitted</th>
                        <th class="command-item no-sort"></th>
                    </tr>
                </thead>
                <tbody>
                    <tr><td colspan="8" class="text-center text-muted">Loading&hellip;</td></tr>
                </tbody>
            </table>
        </div>
    </div>
</div>

<div class="modal fade" id="RequestViewModal" tabindex="-1" aria-hidden="true">
    <div class="modal-dialog modal-lg">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title">View Request</h5>
                <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
            </div>
            <div class="modal-body">
                <div class="row mb-3">
                    <div class="col-md-6">
                        <label>Case Number</label>
                        <div class="form-control-plaintext" data-field="CaseNumber"></div>
                    </div>
                    <div class="col-md-6">
                        <label>Location</label>
                        <div class="form-control-plaintext" data-field="LocationName"></div>
                    </div>
                </div>
                <div class="row mb-3">
                    <div class="col-md-12">
                        <label>Evaluation Type</label>
                        <div class="form-control-plaintext" data-field="TemplateName"></div>
                    </div>
                </div>
                <h6>Requirements</h6>
                <ul data-field="Requirements"></ul>
                <h6>Experts Selected</h6>
                <table class="table table-striped ew-admin-table">
                    <thead>
                        <tr><th>Requirement #</th><th>Expert Name</th></tr>
                    </thead>
                    <tbody data-field="Experts"></tbody>
                </table>
            </div>
            <div class="modal-footer">
                <button type="button" class="btn btn-secondary me-3" data-bs-dismiss="modal">Close</button>
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

        function fmtDate(d) {
            if (!d) return "";
            var dt = new Date(d);
            if (isNaN(dt.getTime())) return ew.esc(String(d));
            var mm = ("0" + (dt.getMonth() + 1)).slice(-2), dd = ("0" + dt.getDate()).slice(-2);
            return mm + "/" + dd + "/" + dt.getFullYear();
        }

        ew.makeAdminTab({
            resource: "Requests",
            tableId: "#tblRequests",
            modalId: "RequestViewModal",
            viewClass: "ew-request-view",
            delClass: "ew-request-delete",
            colCount: 8,
            order: [[1, "desc"]],
            deletedText: "Request deleted.",
            confirmText: "Are you sure you wish to delete this request?",
            rowHtml: function (r) {
                return '<tr data-id="' + r.RequestID + '">' +
                    '<td class="command-item"><a href="#" class="text-primary ew-request-view" title="View"><i class="fas fa-search"></i></a></td>' +
                    '<td>' + r.RequestID + '</td>' +
                    '<td>' + ew.esc(r.CaseNumber) + '</td>' +
                    '<td>' + ew.esc(r.TemplateName) + '</td>' +
                    '<td>' + ew.esc(r.LocationName) + '</td>' +
                    '<td>' + ew.esc(r.CreatedBy) + '</td>' +
                    '<td>' + fmtDate(r.CreatedDate) + '</td>' +
                    '<td class="command-item"><a href="#" class="text-danger ew-request-delete" title="Delete"><i class="fas fa-trash"></i></a></td>' +
                    '</tr>';
            },
            onView: function (item, $m) {
                $m.find('[data-field="CaseNumber"]').text(item.CaseNumber || "");
                $m.find('[data-field="LocationName"]').text(item.LocationName || "");
                $m.find('[data-field="TemplateName"]').text(item.TemplateName || "");
                var reqs = (item.Requirements || []).map(function (r) {
                    return '<li><strong>Requirement #' + r.Sequence + ':</strong> Select [' + r.NumberRequired + '] ' + ew.esc(r.Types) + '</li>';
                }).join("");
                $m.find('[data-field="Requirements"]').html(reqs || '<li class="text-muted">None</li>');
                var exps = (item.Experts || []).map(function (x) {
                    return '<tr><td>' + x.Sequence + '</td><td>' + ew.esc(x.Description) + '</td></tr>';
                }).join("");
                $m.find('[data-field="Experts"]').html(exps || '<tr><td colspan="2" class="text-muted">None</td></tr>');
            }
        }).init();
    });
</script>
