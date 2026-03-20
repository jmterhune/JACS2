<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CourtroomView.ascx.cs" Inherits="tjc.Modules.jacs.CourtroomView" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<%@ Register Src="Controls/navbar.ascx" TagPrefix="tb" TagName="navbar" %>
<section class="navbar border-0 mb-0 justify-content-start">
    <button class="btn btn-default me-3" id="btnToggleMenu" type="button" data-bs-toggle="collapse" data-bs-target="#sidebarMenu" aria-controls="sidebarMenu" aria-expanded="true" aria-label="Toggle navigation">
        <i class="fa-solid fa-bars"></i>
    </button>
    <h2 class="mb-0">Court Rooms</h2>
</section>
<div class="d-flex">
    <tb:navbar runat="server" ID="navbar" />
    <main class="main flex-grow-1 p-3 pt-0">
        <a id="lnkAdd" class="btn btn-primary me-3" tabindex="-1" href="#" data-bs-toggle="modal" data-bs-target="#CourtroomEditModal"><i class="fa fa-plus" aria-hidden="true"></i>&nbsp;Add Courtroom</a>
        <table id="tblCourtroom" class="table table-striped w-100">
            <thead>
                <tr>
                    <th></th>
                    <th>Description</th>
                    <th></th>
                    <th></th>
                </tr>
            </thead>
        </table>
    </main>
</div>
<!-- Detail Modal -->
<div class="modal fade" id="CourtroomDetailModal" tabindex="-1" aria-labelledby="CourtroomDetailModalLabel" aria-hidden="true">
    <div class="modal-dialog modal-lg">
        <div class="modal-content">
            <div id="progress-courtroom" class="modal-progress" style="display: none;">
                <div class="center-progress">
                    <img alt="" src="/images/loading.gif" />
                </div>
            </div>
            <div class="modal-header">
                <h4 class="modal-title" id="CourtroomDetailModalLabel">Courtroom Details</h4>
                <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
            </div>
            <div class="modal-body">
                <table class="table table-striped m-0 p-0 w-100">
                    <tbody>
                        <tr>
                            <td><strong>Description:</strong></td>
                            <td><span id="courtroomDescription"></span></td>
                        </tr>
                    </tbody>
                </table>
                <input type="hidden" id="hdCourtroomId" />
            </div>
            <div class="modal-footer justify-content-around">
                <button type="button" class="btn btn-primary" data-bs-dismiss="modal" data-bs-toggle="modal" data-bs-target="#CourtroomEditModal" id="editCourtroomBtn"><i class="fas fa-edit me-2"></i>&nbsp;Edit</button>
                <button type="button" id="cmdDelete" class="btn btn-danger" data-bs-dismiss="modal"><i class="fa fa-trash me-2"></i>&nbsp;Delete</button>
                <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Close</button>
            </div>
        </div>
    </div>
</div>
<!-- Edit Modal -->
<div class="modal fade" id="CourtroomEditModal" tabindex="-1" aria-labelledby="CourtroomEditModalLabel" aria-hidden="true">
    <div class="modal-dialog modal-lg">
        <div class="modal-content">
            <div id="edit_progress-courtroom" class="modal-progress" style="display: none;">
                <div class="center-progress">
                    <img alt="" src="/images/loading.gif" />
                </div>
            </div>
            <div class="modal-header">
                <h4 class="modal-title" id="CourtroomEditModalLabel">Edit Courtroom</h4>
                <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
            </div>
            <div class="modal-body">
                <div class="container-fluid">
                    <input type="hidden" id="edit_hdCourtroomId">
                    <div class="form-group">
                        <div class="row">
                            <div class="col-md-6">
                                <label>Description<em>*</em></label>
                                <input type="text" id="edit_courtroomDescription" class="form-control" required>
                                <div class="invalid-feedback" id="edit_description-error">Courtroom Description is Required.</div>
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
<!-- Cross Reference Modal -->
<div class="modal fade" id="CourtroomXrefModal" tabindex="-1" aria-labelledby="CourtroomXrefModalLabel" aria-hidden="true">
    <div class="modal-dialog modal-lg">
        <div class="modal-content">
            <div id="xref_progress_courtroom" class="modal-progress" style="display: none;">
                <div class="center-progress">
                    <img alt="" src="/images/loading.gif">
                </div>
            </div>
            <div class="modal-header">
                <h4 id="xrefCourtroomHeader">Managing cross-references for: <span id="xrefSelectedCourtroomName" class="fw-bold"></span>
                </h4>
                <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
            </div>
            <div class="modal-body">
              <input type="hidden" id="hdXrefCourtroomId" />  
                <div class="row mb-3">
                    <div class="col-md-6">
                        <label for="xref_county">County<em>*</em></label>
                        <select id="xref_county" class="form-select">
                            <option value="">Select County</option>
                        </select>
                        <div id="xref_county_error" class="invalid-feedback">County is required.</div>
                    </div>
                    <div class="col-md-6">
                        <label for="xref_clerkCourtroom">Clerk Courtroom<em>*</em></label>
                        <select id="xref_clerkCourtroom" class="form-select" disabled>
                            <option value="">Select Clerk Courtroom</option>
                        </select>
                        <div id="xref_clerkCourtroom_error" class="invalid-feedback">Clerk Courtroom is required.</div>
                        <div id="clerkCourtroomHelp" class="form-text mb-0">Select the Courtroom from the Clerk's Courtroom List</div>
                    </div>
                </div>
                <button type="button" class="btn btn-success" id="xref_cmdSaveReference">
                    <i class="fas fa-save"></i>Save Reference
                </button>
                <table id="tblCourtroomXref" class="table table-striped w-100 mt-4">
                    <thead>
                        <tr>
                            <th>Clerk Courtroom ID</th>
                            <th>Clerk Courtroom</th>
                            <th>County</th>
                            <th></th>
                        </tr>
                    </thead>
                </table>
            </div>
            <div class="modal-footer justify-content-between">
                <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Close</button>
            </div>
        </div>
    </div>
</div>

<dnn:DnnJsInclude runat="server" FilePath="~/DesktopModules/tjc.modules/JACS/js/jacs.js" ForceProvider="DnnFormBottomProvider" Priority="100" />
<dnn:DnnJsInclude runat="server" FilePath="~/DesktopModules/tjc.modules/JACS/js/courtroom.js" ForceProvider="DnnFormBottomProvider" Priority="101" />
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
                if (typeof CourtroomController === 'undefined') {
                    console.error('CourtroomController is not defined. Check if Script(courtroom.js) loaded correctly.');
                    return;
                }
                const courtroomController = new CourtroomController({
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
                courtroomController.init();
            } catch (e) {
                console.error('Error initializing CourtroomController:', e);
            }
        });
    }(jQuery, window.Sys));
</script>
