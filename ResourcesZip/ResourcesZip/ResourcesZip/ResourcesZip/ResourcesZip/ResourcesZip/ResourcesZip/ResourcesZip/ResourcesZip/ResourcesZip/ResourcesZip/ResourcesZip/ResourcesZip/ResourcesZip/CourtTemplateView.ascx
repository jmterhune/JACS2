<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CourtTemplateView.ascx.cs" Inherits="tjc.Modules.jacs.CourtTemplateView" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<%@ Register Src="Controls/navbar.ascx" TagPrefix="tb" TagName="navbar" %>
<section class="navbar border-0 mb-0 justify-content-start">
    <button class="btn btn-default me-3" id="btnToggleMenu" type="button" data-bs-toggle="collapse" data-bs-target="#sidebarMenu" aria-controls="sidebarMenu" aria-expanded="true" aria-label="Toggle navigation">
        <i class="fa-solid fa-bars"></i>
    </button>
    <h2 class="mb-0">Court Templates</h2>
</section>
<div class="d-flex">
    <tb:navbar runat="server" ID="navbar" />
    <main class="main flex-grow-1 p-3 pt-0">
        <a id="lnkAdd" class="btn btn-primary me-3" tabindex="-1" href="#" data-bs-toggle="modal" data-bs-target="#CourtTemplateEditModal"><i class="fa fa-plus" aria-hidden="true"></i>&nbsp;Add Court Template</a>
        <table id="tblCourtTemplate" class="table table-striped w-100">
            <thead>
                <tr>
                    <th></th>
                    <th></th>
                    <th>Template Name</th>
                    <th>Judge Name</th>
                    <th colspan="3">Actions</th>
                </tr>
            </thead>
        </table>
    </main>
</div>
<!-- Detail Modal -->
<div class="modal fade" id="CourtTemplateDetailModal" tabindex="-1" aria-labelledby="CourtTemplateDetailModalLabel" aria-hidden="true">
    <div class="modal-dialog modal-lg">
        <div class="modal-content">
            <div id="progress-courttemplate" class="modal-progress" style="display: none;">
                <div class="center-progress">
                    <img alt="" src="/images/loading.gif" />
                </div>
            </div>
            <div class="modal-header">
                <h4 class="modal-title" id="CourtTemplateDetailModalLabel">Court Template Details</h4>
                <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
            </div>
            <div class="modal-body">
                <table class="table table-striped m-0 p-0 w-100">
                    <tbody>
                        <tr>
                            <td><strong>Template Name:</strong></td>
                            <td><span id="ctName"></span></td>
                        </tr>
                        <tr>
                            <td><strong>Judge:</strong></td>
                            <td><span id="ctCourt"></span></td>
                        </tr>
                    </tbody>
                </table>
                <input type="hidden" id="hdCourtTemplateId" />
            </div>
            <div class="modal-footer justify-content-around">
                <button type="button" class="btn btn-primary" data-bs-dismiss="modal" data-bs-toggle="modal" data-bs-target="#CourtTemplateEditModal" id="editCourtTemplateBtn"><i class="fas fa-edit me-2"></i>&nbsp;Edit</button>
                <button type="button" id="cmdDelete" class="btn btn-danger" data-bs-dismiss="modal"><i class="fa fa-trash me-2"></i>&nbsp;Delete</button>
                <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Close</button>
            </div>
        </div>
    </div>
</div>
<!-- Edit Modal -->
<div class="modal fade" id="CourtTemplateEditModal" tabindex="-1" aria-labelledby="CourtTemplateEditModalLabel" aria-hidden="true">
    <div class="modal-dialog modal-lg">
        <div class="modal-content">
            <div id="edit_progress-courttemplate" class="modal-progress" style="display: none;">
                <div class="center-progress">
                    <img alt="" src="/images/loading.gif" />
                </div>
            </div>
            <div class="modal-header">
                <h4 class="modal-title" id="CourtTemplateEditModalLabel">Edit Court Template</h4>
                <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
            </div>
            <div class="modal-body">
                <div class="container-fluid">
                    <input type="hidden" id="edit_hdCourtTemplateId">
                    <div class="form-group">
                        <div class="row">
                            <div class="col-md-6">
                                <label>Template Name<em>*</em></label>
                                <input type="text" id="edit_ctName" class="form-control" required>
                                <div class="invalid-feedback" id="edit_name-error">Template Name is Required.</div>
                            </div>
                            <div class="col-md-6">
                                <label>Judge<em>*</em></label>
                                <select id="edit_ctCourt" class="form-control" required>
                                </select>
                                <div class="invalid-feedback" id="edit_ctCourt-error">Judge is Required.</div>
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
<dnn:DnnJsInclude runat="server" FilePath="~/DesktopModules/tjc.modules/JACS/js/courttemplate.js" ForceProvider="DnnFormBottomProvider" Priority="101" />
<dnn:DnnCssInclude runat="server" FilePath="~/Resources/Libraries/DataTables/dataTables.bootstrap5.min.css" />
<dnn:DnnJsInclude runat="server" FilePath="~/Resources/Libraries/DataTables/datatables.min.js" />
<dnn:DnnJsInclude runat="server" FilePath="~/Resources/Libraries/DataTables/dataTables.bootstrap5.min.js" />
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/sweetalert/sweetalert2.min.js" />
<dnn:DnnCssInclude runat="server" FilePath="/Resources/Libraries/sweetalert/sweetalert2.min.css" />
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/Noty/noty.min.js" />
<dnn:DnnCssInclude runat="server" FilePath="/Resources/Libraries/Noty/noty.min.css" />
<script>
    var moduleId = <%=ModuleId%>;
    var service = {
        path: "JACS",
        framework: $.ServicesFramework(moduleId)
    };

    (function ($, Sys) {
        $(document).ready(function () {
            try {
                if (typeof CourtTemplateController === 'undefined') {
                    console.error('CourtTemplateController is not defined. Check if Script(courttemplate.js) loaded correctly.');
                    return;
                }
                const courtTemplateController = new CourtTemplateController({
                    moduleId: moduleId,
                    userId: <%=UserId%>,
                    isAdmin: "<%=IsAdmin%>",
                    adminRole: "<%=AdminRole%>",
                    service: service,
                    currentPage: 0,
                    pageSize: 25,
                    recordCount: 0,
                    sortColumnIndex: 3,
                    sortDirection: "asc",
                    templateConfigUrl: "<%=TemplateConfigUrl%>",
                });
                courtTemplateController.init();
            } catch (e) {
                console.error('Error initializing CourtTemplateController:', e);
            }
        });
    }(jQuery, window.Sys));
</script>
