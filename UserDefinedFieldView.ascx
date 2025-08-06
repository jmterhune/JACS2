<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UserDefinedFieldView.ascx.cs" Inherits="tjc.Modules.jacs.UserDefinedFieldView" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<%@ Register Src="Controls/navbar.ascx" TagPrefix="tb" TagName="navbar" %>
<section class="navbar border-0 mb-0 justify-content-start">
    <button class="btn btn-default me-3" id="btnToggleMenu" type="button" data-bs-toggle="collapse" data-bs-target="#sidebarMenu" aria-controls="sidebarMenu" aria-expanded="true" aria-label="Toggle navigation">
        <i class="fa-solid fa-bars"></i>
    </button>
    <h2 class="mb-0"><asp:Literal ID="ltCourtName" runat="server" />'s User Defined Fields </h2>
</section>
<div class="d-flex">
    <tb:navbar runat="server" ID="navbar" />
    <main class="main flex-grow-1 p-3 pt-0">
        <div class="alert alert-info mt-3" role="alert">
            <strong><i class="fas fa-info-circle"></i>&nbsp;Note:</strong>
            This section allows a user to add custom fields to be displayed on the schedule, docket, or attorney scheduling.
        </div>
        <a id="lnkAdd" class="btn btn-primary me-3" tabindex="-1" href="#" data-bs-toggle="modal" data-bs-target="#UserDefinedFieldEditModal"><i class="fa fa-plus" aria-hidden="true"></i>&nbsp;Add User Defined Field</a>
        <table id="tblUserDefinedField" class="table table-striped w-100">
            <thead>
                <tr>
                    <th></th>
                    <th></th>
                    <th>Field Name</th>
                    <th>Field Type</th>
                    <th>Alignment</th>
                    <th>Default Value</th>
                    <th>Required</th>
                    <th>Yes Answer Required</th>
                    <th>Display on Docket</th>
                    <th>Display on Schedule</th>
                    <th>Use in Attorney Scheduling</th>
                    <th></th>
                </tr>
            </thead>
        </table>
    </main>
</div>
<!-- Detail Modal -->
<div class="modal fade" id="UserDefinedFieldDetailModal" tabindex="-1" aria-labelledby="UserDefinedFieldDetailModalLabel" aria-hidden="true">
    <div class="modal-dialog modal-xl">
        <div class="modal-content">
            <div id="progress-userDefinedField" class="modal-progress" style="display: none;">
                <div class="center-progress">
                    <img alt="" src="/images/loading.gif" />
                </div>
            </div>
            <div class="modal-header">
                <h4 class="modal-title" id="UserDefinedFieldDetailModalLabel">User Defined Field Details</h4>
                <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
            </div>
            <div class="modal-body">
                <table class="table table-striped m-0 p-0 w-100">
                    <tbody>
                        <tr>
                            <td><strong>Field Name:</strong></td>
                            <td><span id="fieldName"></span></td>
                        </tr>
                        <tr>
                            <td><strong>Field Type:</strong></td>
                            <td><span id="fieldType"></span></td>
                        </tr>
                        <tr>
                            <td><strong>Alignment:</strong></td>
                            <td><span id="alignment"></span></td>
                        </tr>
                        <tr>
                            <td><strong>Default Value:</strong></td>
                            <td><span id="defaultValue"></span></td>
                        </tr>
                        <tr>
                            <td><strong>Required:</strong></td>
                            <td><span id="required"></span></td>
                        </tr>
                        <tr>
                            <td><strong>Yes Answer Required:</strong></td>
                            <td><span id="yesAnswerRequired"></span></td>
                        </tr>
                        <tr>
                            <td><strong>Display on Docket:</strong></td>
                            <td><span id="displayOnDocket"></span></td>
                        </tr>
                        <tr>
                            <td><strong>Display on Schedule:</strong></td>
                            <td><span id="displayOnSchedule"></span></td>
                        </tr>
                        <tr>
                            <td><strong>Use in Attorney Scheduling:</strong></td>
                            <td><span id="useInAttorneyScheduling"></span></td>
                        </tr>
                    </tbody>
                </table>
                <input type="hidden" id="hdUserDefinedFieldId" />
            </div>
            <div class="modal-footer justify-content-around">
                <button type="button" class="btn btn-primary" data-bs-dismiss="modal" data-bs-toggle="modal" data-bs-target="#UserDefinedFieldEditModal" id="editUserDefinedFieldBtn"><i class="fas fa-edit me-2"></i>&nbsp;Edit</button>
                <button type="button" id="cmdDelete" class="btn btn-danger" data-bs-dismiss="modal"><i class="fa fa-trash me-2"></i>&nbsp;Delete</button>
                <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Close</button>
            </div>
        </div>
    </div>
</div>
<!-- Edit Modal -->
<div class="modal fade" id="UserDefinedFieldEditModal" tabindex="-1" aria-labelledby="UserDefinedFieldEditModalLabel" aria-hidden="true">
    <div class="modal-dialog modal-xl">
        <div class="modal-content">
            <div id="edit_progress-userDefinedField" class="modal-progress" style="display: none;">
                <div class="center-progress">
                    <img alt="" src="/images/loading.gif" />
                </div>
            </div>
            <div class="modal-header">
                <h4 class="modal-title" id="UserDefinedFieldEditModalLabel">Edit User Defined Field</h4>
                <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
            </div>
            <div class="modal-body">
                <div class="container-fluid">
                    <asp:HiddenField ID="edit_hdCourtId" ClientIDMode="Static" runat="server" />
                    <input type="hidden" id="edit_hdUserDefinedFieldId">
                    <div class="form-group">
                        <div class="row">
                            <div class="col-md-12">
                                <label>Field Name<em>*</em></label>
                                <input type="text" id="edit_fieldName" class="form-control" required>
                                <div class="invalid-feedback">User Defined Field Name is Required.</div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-6">
                                <label>Field Type</label>
                                <select id="edit_fieldType" class="form-control">
                                    <option value="text">Text</option>
                                    <option value="number">Number</option>
                                    <option value="date">Date</option>
                                    <option value="yes_no">Yes/No</option>
                                </select>
                            </div>
                            <div class="col-md-6">
                                <label>Alignment</label>
                                <select id="edit_alignment" class="form-control">
                                    <option value="left">Left</option>
                                    <option value="center">Center</option>
                                    <option value="right">Right</option>
                                </select>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-6">
                                <label>Default Value</label>
                                <input type="text" id="edit_defaultValue" class="form-control">
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-4">
                                <label>Required</label>
                                <input type="checkbox" id="edit_required">
                            </div>
                            <div class="col-md-4">
                                <label>'Yes' Answer Required</label>
                                <input type="checkbox" id="edit_yesAnswerRequired" disabled>
                            </div>
                            <div class="col-md-4">
                                <label>Display on Docket</label>
                                <input type="checkbox" id="edit_displayOnDocket">
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-4">
                                <label>Display on Schedule</label>
                                <input type="checkbox" id="edit_displayOnSchedule">
                            </div>
                            <div class="col-md-4">
                                <label>Use in Attorney Scheduling</label>
                                <input type="checkbox" id="edit_useInAttorneyScheduling">
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
<dnn:DnnJsInclude runat="server" FilePath="~/DesktopModules/tjc.modules/JACS/js/userDefinedField.js" ForceProvider="DnnFormBottomProvider" Priority="101" />
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
                if (typeof UserDefinedFieldController === 'undefined') {
                    console.error('UserDefinedFieldController is not defined. Check if Script(userDefinedField.js) loaded correctly.');
                    return;
                }
                const userDefinedFieldController = new UserDefinedFieldController({
                    moduleId: moduleId,
                    userId: <%=UserId%>,
                    isAdmin: "<%=IsAdmin%>",
                    adminRole: "<%=AdminRole%>",
                    service: service,
                    currentPage: 0,
                    pageSize: 25,
                    recordCount: 0,
                    sortColumnIndex: 3,
                    sortDirection: "asc"
                });
                userDefinedFieldController.init();
            } catch (e) {
                console.error('Error initializing UserDefinedFieldController:', e);
            }
        });
    }(jQuery, window.Sys));
</script>