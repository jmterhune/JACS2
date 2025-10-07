<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CourtPermissionView.ascx.cs" Inherits="tjc.Modules.jacs.CourtPermissionView" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<%@ Register Src="Controls/navbar.ascx" TagPrefix="tb" TagName="navbar" %>
<section class="navbar border-0 mb-0 justify-content-start">
    <button class="btn btn-default me-3" id="btnToggleMenu" type="button" data-bs-toggle="collapse" data-bs-target="#sidebarMenu" aria-controls="sidebarMenu" aria-expanded="true" aria-label="Toggle navigation">
        <i class="fa-solid fa-bars"></i>
    </button>
    <h2 class="mb-0">Court Permissions</h2>
</section>
<div class="d-flex">
    <tb:navbar runat="server" ID="navbar" />
    <main class="main flex-grow-1 p-3 pt-0">
        <a id="lnkAdd" class="btn btn-primary me-3" tabindex="-1" href="#" data-bs-toggle="modal" data-bs-target="#CourtPermissionEditModal"><i class="fa fa-plus" aria-hidden="true"></i>&nbsp;Add Court Permission</a>
        <table id="tblCourtPermission" class="table table-striped w-100">
            <thead>
                <tr>
                    <th></th>
                    <th></th>
                    <th>User</th>
                    <th>Judge</th>
                    <th>Active</th>
                    <th>Permission</th>
                    <th></th>
                </tr>
            </thead>
        </table>
    </main>
</div>
<!-- Detail Modal -->
<div class="modal fade" id="CourtPermissionDetailModal" tabindex="-1" aria-labelledby="CourtPermissionDetailModalLabel" aria-hidden="true">
    <div class="modal-dialog modal-lg">
        <div class="modal-content">
            <div id="progress-courtpermission" class="modal-progress" style="display: none;">
                <div class="center-progress">
                    <img alt="" src="/images/loading.gif" />
                </div>
            </div>
            <div class="modal-header">
                <h4 class="modal-title" id="CourtPermissionDetailModalLabel">Court Permission Details</h4>
                <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
            </div>
            <div class="modal-body">
                <table class="table table-striped m-0 p-0 w-100">
                    <tbody>
                        <tr>
                            <td><strong>User:</strong></td>
                            <td><span id="cpUserDisplayName"></span></td>
                        </tr>
                        <tr>
                            <td><strong>Judge:</strong></td>
                            <td><span id="cpJudgeName"></span></td>
                        </tr>
                        <tr>
                            <td><strong>Active:</strong></td>
                            <td><span id="cpActive"></span></td>
                        </tr>
                        <tr>
                            <td><strong>Permission:</strong></td>
                            <td><span id="cpPermission"></span></td>
                        </tr>
                    </tbody>
                </table>
                <input type="hidden" id="hdCourtPermissionId" />
            </div>
            <div class="modal-footer justify-content-around">
                <button type="button" class="btn btn-primary" data-bs-dismiss="modal" data-bs-toggle="modal" data-bs-target="#CourtPermissionEditModal" id="editCourtPermissionBtn"><i class="fas fa-edit me-2"></i>&nbsp;Edit</button>
                <button type="button" id="cmdDelete" class="btn btn-danger" data-bs-dismiss="modal"><i class="fa fa-trash me-2"></i>&nbsp;Delete</button>
                <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Close</button>
            </div>
        </div>
    </div>
</div>
<!-- Edit Modal -->
<div class="modal fade" id="CourtPermissionEditModal" tabindex="-1" aria-labelledby="CourtPermissionEditModalLabel" aria-hidden="true">
    <div class="modal-dialog modal-lg">
        <div class="modal-content">
            <div id="edit_progress-courtpermission" class="modal-progress" style="display: none;">
                <div class="center-progress">
                    <img alt="" src="/images/loading.gif" />
                </div>
            </div>
            <div class="modal-header">
                <h4 class="modal-title" id="CourtPermissionEditModalLabel">Edit Court Permission</h4>
                <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
            </div>
            <div class="modal-body">
                <div class="container-fluid">
                    <input type="hidden" id="edit_hdCourtPermissionId">
                    <div class="form-group">
                        <div class="row">
                            <div class="col-md-6">
                                <label>User<em>*</em></label>
                                <select id="edit_cpUser" class="form-control" required>
                                    <option value="">Select User</option>
                                </select>
                                <div class="invalid-feedback">User is required.</div>
                            </div>
                            <div class="col-md-6">
                                <label>Judge<em>*</em></label>
                                <select id="edit_cpJudge" class="form-control" required>
                                    <option value="">Select Judge</option>
                                </select>
                                <div class="invalid-feedback">Judge is required.</div>
                            </div>
                        </div>
                        <div class="row mt-3">
                            <div class="col-md-6">
                                <label>Active<em>*</em></label>
                                <div>
                                    <div class="form-check form-check-inline">
                                        <input class="form-check-input" type="radio" name="cpActive" id="cpActiveTrue" value="true" required>
                                        <label class="form-check-label" for="cpActiveTrue">Yes</label>
                                    </div>
                                    <div class="form-check form-check-inline">
                                        <input class="form-check-input" type="radio" name="cpActive" id="cpActiveFalse" value="false">
                                        <label class="form-check-label" for="cpActiveFalse">No</label>
                                    </div>
                                </div>
                                <div class="invalid-feedback">Active status is required.</div>
                            </div>
                            <div class="col-md-6">
                                <label>Calendar Permissions</label>
                                <select id="edit_cpEditable" class="form-control">
                                    <option value="true">View and Edit</option>
                                    <option value="false">View</option>
                                </select>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="modal-footer justify-content-between">
                <button type="button" class="btn btn-success" id="edit_cmdSave">
                    <i class="fas fa-save" aria-hidden="true"></i>&nbsp;Save
                </button>
                <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Close</button>
            </div>
        </div>
    </div>
</div>
<dnn:DnnJsInclude runat="server" FilePath="~/DesktopModules/tjc.modules/JACS/js/jacs.js" ForceProvider="DnnFormBottomProvider" Priority="100" />
<dnn:DnnJsInclude runat="server" FilePath="~/DesktopModules/tjc.modules/JACS/js/courtpermission.js" ForceProvider="DnnFormBottomProvider" Priority="101" />
<dnn:DnnCssInclude runat="server" FilePath="~/Resources/Libraries/DataTables/dataTables.bootstrap5.min.css" />
<dnn:DnnJsInclude runat="server" FilePath="~/Resources/Libraries/DataTables/datatables.min.js" />
<dnn:DnnJsInclude runat="server" FilePath="~/Resources/Libraries/DataTables/dataTables.bootstrap5.min.js" />
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/sweetalert/sweetalert2.min.js" />
<dnn:DnnCssInclude runat="server" FilePath="/Resources/Libraries/sweetalert/sweetalert2.min.css" />
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/Noty/noty.min.js" />
<dnn:DnnCssInclude runat="server" FilePath="/Resources/Libraries/Noty/noty.min.css" /><script>
    var moduleId = <%=ModuleId%>;
    var service = {
        path: "JACS",
        framework: $.ServicesFramework(moduleId)
    };

    (function ($, Sys) {
        $(document).ready(function () {
            try {
                if (typeof CourtPermissionController === 'undefined') {
                    console.error('CourtPermissionController is not defined. Check if Script(courtpermission.js) loaded correctly.');
                    return;
                }
                const courtPermissionController = new CourtPermissionController({
                    moduleId: moduleId,
                    userId: <%=UserId%>,
                    isAdmin: "<%=IsAdmin%>",
                    adminRole: "<%=AdminRole%>",
                    service: service,
                    currentPage: 0,
                    pageSize: 25,
                    recordCount: 0,
                    sortColumnIndex: 2,
                    sortDirection: "asc"
                });
                courtPermissionController.init();
            } catch (e) {
                console.error('Error initializing CourtPermissionController:', e);
            }
        });
    }(jQuery, window.Sys));
</script>