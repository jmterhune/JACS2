<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ApiConfgView.ascx.cs" Inherits="tjc.Modules.jacs.ApiConfig" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<%@ Register Src="Controls/navbar.ascx" TagPrefix="tb" TagName="navbar" %>
<section class="navbar border-0 mb-0 justify-content-start">
    <button class="btn btn-default me-3" id="btnToggleMenu" type="button" data-bs-toggle="collapse" data-bs-target="#sidebarMenu" aria-controls="sidebarMenu" aria-expanded="true" aria-label="Toggle navigation">
        <i class="fa-solid fa-bars" aria-hidden="true"></i>
    </button>
    <h2 class="mb-0">API Interface Configuration</h2>
</section>
<div class="d-flex">
    <tb:navbar runat="server" ID="navbar" />
    <main class="main flex-grow-1 p-3 pt-0">
        <a id="lnkAdd" class="btn btn-primary me-3" tabindex="-1" href="#" data-bs-toggle="modal" data-bs-target="#ApiEditModal"><i class="fa fa-plus" aria-hidden="true"></i>&nbsp;Add API Endpoint</a>
        <table id="tblApi" class="table table-striped w-100">
            <thead>
                <tr>
                    <th></th>
                    <th>Endpoint Url</th>
                    <th>Action Performed</th>
                    <th>County</th>
                    <th></th>
                </tr>
            </thead>
        </table>
    </main>
</div>
<!-- Edit Modal -->
<div class="modal fade" id="ApiEditModal" tabindex="-1" aria-labelledby="ApiEditModalLabel" aria-hidden="true">
    <div class="modal-dialog modal-lg">
        <div class="modal-content">
            <div id="edit_progress_api" class="modal-progress" style="display: none;">
                <div class="center-progress">
                    <img alt="" src="/images/loading.gif" />
                </div>
            </div>
            <div class="modal-header">
                <h4 class="modal-title" id="ApiEditModalLabel">Edit Endpoint</h4>
                <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
            </div>
            <div class="modal-body">
                <div class="container-fluid">
                    <input type="hidden" id="edit_hdApiId">
                    <div class="form-group">
                        <div class="row">
                            <div class="col-md-4">
                                <label>County<span class="text-danger">*</span></label>
                                <select id="edit_county" class="form-control">
                                    <option value="">Select County</option>
                                </select>
                                <div class="invalid-feedback" >Please select a County.</div>
                            </div>
                            <div class="col-md-4">
                                <label>Action Performed<span class="text-danger">*</span></label>
                                <select id="edit_type" class="form-control">
                                    <option value="">Select Action</option>
                                </select>
                                 <div class="invalid-feedback" >Please select an Action Type.</div>
                            </div>
                            <div class="col-md-12">
                                <label>Endpoint URL<span class="text-danger">*</span></label>
                                <input type="text" id="edit_end_point" class="form-control" required>
                                <div class="invalid-feedback">Endpoint URL is required</div>
                            </div>
                            
                        </div>
                    </div>
                </div>
            </div>
            <div class="modal-footer justify-content-between">
                <button type="button" class="btn btn-success" id="edit_cmdSave">
                    <i class="fas fa-save" aria-hidden="true"></i>&nbsp;Save
                </button>
                <button type="button" class="btn btn-danger" id="edit_cmdDelete">
                    <i class="fas fa-trash" aria-hidden="true"></i>&nbsp;Delete
                </button>
                <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Close</button>
            </div>
        </div>
    </div>
</div>
<dnn:DnnJsInclude runat="server" FilePath="~/DesktopModules/tjc.modules/JACS/js/imask.js" ForceProvider="DnnFormBottomProvider" />
<dnn:DnnJsInclude runat="server" FilePath="~/DesktopModules/tjc.modules/JACS/js/jacs.js" ForceProvider="DnnFormBottomProvider" Priority="100" />
<dnn:DnnJsInclude runat="server" FilePath="~/DesktopModules/tjc.modules/JACS/js/api.js" ForceProvider="DnnFormBottomProvider" Priority="101" />
<dnn:DnnCssInclude runat="server" FilePath="~/Resources/Shared/components/TimePicker/Themes/jquery-ui.min.css" />
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
                if (typeof ApiConfigController === 'undefined') {
                    console.error('ApiConfigController is not defined. Check if Script(api.js) loaded correctly.');
                    return;
                }
                const apiConfigController = new ApiConfigController({
                    moduleId: moduleId,
                    userId: <%=UserId%>,
                    isAdmin: "<%=IsAdmin%>",
                    adminRole: "<%=AdminRole%>",
                    service: service,
                    currentPage: 0,
                    pageSize: 25,
                    recordCount: 0,
                    sortColumnIndex: 0,
                    sortDirection: "asc"
                });
                apiConfigController.init();
            } catch (e) {
                console.error('Error initializing ApiConfigController:', e);
            }
        });
    }(jQuery, window.Sys));
</script>
