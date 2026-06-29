<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ExpertList.ascx.cs" Inherits="tjc.Modules.ExpertWitness.ExpertList" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>

<script type="text/javascript">
    window.__ewCtx = { moduleId: <%= ModuleId %>, tabId: <%= TabId %> };
</script>

<div class="tabs">
    <ul class="nav nav-tabs">
        <li class="nav-item"><a class="nav-link" href="<%=RequestListUrl %>">Requests</a></li>
        <li class="nav-item"><a class="nav-link active" href="#experts" data-bs-toggle="tab">Experts</a></li>
        <li class="nav-item"><a class="nav-link" href="<%=EvaluationTypeListUrl %>">Evaluation Types</a></li>
        <li class="nav-item"><a class="nav-link" href="<%=TypeListUrl %>">Expert Types</a></li>
        <li class="nav-item"><a class="nav-link" href="<%=LocationListUrl %>">Locations</a></li>
    </ul>
    <div class="tab-content">
        <div id="experts" class="tab-pane active">
            <button type="button" id="ewExpertAdd" class="btn btn-success"><i class="fas fa-plus"></i>&nbsp;Add Expert</button>
            <table id="tblExperts" class="table table-striped table-hover ew-admin-table">
                <thead>
                    <tr>
                        <th class="command-item no-sort"></th>
                        <th>ID</th>
                        <th>Expert</th>
                        <th>Field of Expertise</th>
                        <th>Locations</th>
                        <th>Contract Ends</th>
                        <th class="no-sort">Comments</th>
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

<div class="modal fade" id="ExpertEditModal" tabindex="-1" aria-hidden="true">
    <div class="modal-dialog modal-lg">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title">Add Expert</h5>
                <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
            </div>
            <div class="modal-body">
                <input type="hidden" name="ExpertID" value="0" />
                <div class="row mb-3">
                    <div class="col-md-8">
                        <label>Expert</label>
                        <input type="text" name="Description" class="form-control" maxlength="50" />
                    </div>
                    <div class="col-md-4">
                        <label>Contract Ends</label>
                        <input type="date" name="ContractEnds" class="form-control" />
                    </div>
                </div>
                <div class="row mb-3">
                    <div class="col-md-6">
                        <label>Email</label>
                        <input type="email" name="Email" class="form-control" maxlength="255" />
                    </div>
                    <div class="col-md-6">
                        <label>Phone</label>
                        <input type="text" name="Phone" class="form-control" maxlength="50" />
                    </div>
                </div>
                <fieldset class="outline-fieldset">
                    <legend>Locations</legend>
                    <div id="ewExpertLocations" class="column-4"></div>
                </fieldset>
                <fieldset class="outline-fieldset">
                    <legend>Types</legend>
                    <div id="ewExpertTypes" class="column-4"></div>
                </fieldset>
                <fieldset class="outline-fieldset">
                    <legend>Evaluation Types</legend>
                    <div id="ewExpertTemplates"></div>
                </fieldset>
                <div class="mb-3">
                    <label>Comments</label>
                    <textarea name="Comments" rows="4" class="form-control"></textarea>
                </div>
            </div>
            <div class="modal-footer">
                <button type="button" id="ewExpertSave" class="btn btn-primary">Save</button>
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
        var ew = window.ew;
        if (!ew || !ew.makeAdminTab) return;

        function fmtDate(d) {
            if (!d) return "";
            var dt = new Date(d);
            if (isNaN(dt.getTime())) return ew.esc(String(d));
            var mm = ("0" + (dt.getMonth() + 1)).slice(-2), dd = ("0" + dt.getDate()).slice(-2);
            return mm + "/" + dd + "/" + dt.getFullYear();
        }
        function buildChecks(sel, rows, idF, nameF, cls) {
            $(sel).html((rows || []).map(function (r) {
                var id = r[idF];
                return '<div class="form-check">' +
                    '<input class="form-check-input ' + cls + '" type="checkbox" value="' + id + '" id="' + cls + '_' + id + '">' +
                    '<label class="form-check-label" for="' + cls + '_' + id + '">' + ew.esc(r[nameF]) + '</label></div>';
            }).join(""));
        }
        function checkedVals($m, sel) {
            return $m.find(sel + ':checked').map(function () { return parseInt(this.value, 10); }).get();
        }

        // Load the three lookup lists, render the modal checkboxes, then wire the tab.
        Promise.all([ew.api.get("Locations/All"), ew.api.get("Types/All"), ew.api.get("Templates/All")]).then(function (res) {
            buildChecks("#ewExpertLocations", res[0], "LocationID", "LocationName", "ew-loc-check");
            buildChecks("#ewExpertTypes", res[1], "TypeID", "TypeName", "ew-type-check");
            buildChecks("#ewExpertTemplates", res[2], "TemplateID", "TemplateName", "ew-tmpl-check");

            ew.makeAdminTab({
                resource: "Experts",
                idField: "ExpertID",
                tableId: "#tblExperts",
                modalId: "ExpertEditModal",
                addBtnId: "#ewExpertAdd",
                saveBtnId: "#ewExpertSave",
                editClass: "ew-expert-edit",
                delClass: "ew-expert-delete",
                colCount: 8,
                order: [[2, "asc"]],
                addTitle: "Add Expert",
                editTitle: "Edit Expert",
                addedText: "Expert added.",
                updatedText: "Expert updated.",
                deletedText: "Expert deleted.",
                confirmText: "Are you sure you wish to delete this expert?",
                rowHtml: function (x) {
                    var cmt = x.Comments ? '<i class="fas fa-comment-alt" title="' + ew.esc(x.Comments) + '"></i>' : "";
                    return '<tr data-id="' + x.ExpertID + '">' +
                        '<td class="command-item"><a href="#" class="text-primary ew-expert-edit" title="Edit"><i class="fas fa-edit"></i></a></td>' +
                        '<td>' + x.ExpertID + '</td>' +
                        '<td>' + ew.esc(x.Description) + '</td>' +
                        '<td>' + ew.esc(x.TypeDisplay) + '</td>' +
                        '<td>' + ew.esc(x.LocationDisplay) + '</td>' +
                        '<td>' + fmtDate(x.ContractEnds) + '</td>' +
                        '<td class="text-center">' + cmt + '</td>' +
                        '<td class="command-item"><a href="#" class="text-danger ew-expert-delete" title="Delete"><i class="fas fa-trash"></i></a></td>' +
                        '</tr>';
                },
                fillForm: function ($m, x) {
                    $m.find('[name="ExpertID"]').val(x ? x.ExpertID : 0);
                    $m.find('[name="Description"]').val(x ? x.Description : "");
                    $m.find('[name="Comments"]').val(x ? (x.Comments || "") : "");
                    $m.find('[name="Email"]').val(x ? (x.Email || "") : "");
                    $m.find('[name="Phone"]').val(x ? (x.Phone || "") : "");
                    $m.find('[name="ContractEnds"]').val(x && x.ContractEnds ? String(x.ContractEnds).slice(0, 10) : "");
                    $m.find(".ew-loc-check, .ew-type-check, .ew-tmpl-check").prop("checked", false);
                    if (x) {
                        (x.LocationIDs || []).forEach(function (id) { $m.find('.ew-loc-check[value="' + id + '"]').prop("checked", true); });
                        (x.TypeIDs || []).forEach(function (id) { $m.find('.ew-type-check[value="' + id + '"]').prop("checked", true); });
                        (x.TemplateIDs || []).forEach(function (id) { $m.find('.ew-tmpl-check[value="' + id + '"]').prop("checked", true); });
                    }
                },
                readForm: function ($m) {
                    var ce = $m.find('[name="ContractEnds"]').val();
                    return {
                        ExpertID: parseInt($m.find('[name="ExpertID"]').val(), 10) || 0,
                        Description: $m.find('[name="Description"]').val(),
                        ContractEnds: ce ? ce : null,
                        Comments: $m.find('[name="Comments"]').val(),
                        Email: $m.find('[name="Email"]').val(),
                        Phone: $m.find('[name="Phone"]').val(),
                        LocationIDs: checkedVals($m, ".ew-loc-check"),
                        TypeIDs: checkedVals($m, ".ew-type-check"),
                        TemplateIDs: checkedVals($m, ".ew-tmpl-check")
                    };
                },
                validate: function (d) {
                    return (d.Description && d.Description.trim()) ? null : "Expert name is required.";
                }
            }).init();
        }).catch(function (err) { ew.notifyError("Load failed: " + err.message); });
    });
</script>
