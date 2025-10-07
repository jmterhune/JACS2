<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="JudgeView.ascx.cs" Inherits="tjc.Modules.jacs.JudgeView" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<%@ Register Src="Controls/navbar.ascx" TagPrefix="tb" TagName="navbar" %>
<section class="navbar border-0 mb-0 justify-content-start">
    <button class="btn btn-default me-3" id="btnToggleMenu" type="button" data-bs-toggle="collapse" data-bs-target="#sidebarMenu" aria-controls="sidebarMenu" aria-expanded="true" aria-label="Toggle navigation">
        <i class="fa-solid fa-bars"></i>
    </button>
    <h2 class="mb-0">Judges</h2>
</section>
<div class="d-flex">
    <tb:navbar runat="server" ID="navbar" />
    <main class="main flex-grow-1 p-3 pt-0">
        <a id="lnkAdd" class="btn btn-primary me-3" tabindex="-1" href="#" data-bs-toggle="modal" data-bs-target="#JudgeEditModal"><i class="fa fa-plus" aria-hidden="true"></i>&nbsp;Add Judge</a>
        <table id="tblJudge" class="table table-striped w-100">
            <thead>
                <tr>
                    <th></th>
                    <th></th>
                    <th>Name</th>
                    <th>Phone</th>
                    <th>Court</th>
                    <th>Title</th>
                    <th></th>
                </tr>
            </thead>
        </table>
    </main>
</div>
<!-- Detail Modal -->
<div class="modal fade" id="JudgeDetailModal" tabindex="-1" aria-labelledby="JudgeDetailModalLabel" aria-hidden="true">
    <div class="modal-dialog modal-lg">
        <div class="modal-content">
            <div id="progress-judge" class="modal-progress" style="display: none;">
                <div class="center-progress">
                    <img alt="" src="/images/loading.gif" />
                </div>
            </div>
            <div class="modal-header">
                <h4 class="modal-title" id="JudgeDetailModalLabel">Judge Details</h4>
                <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
            </div>
            <div class="modal-body">
                <table class="table table-striped m-0 p-0 w-100">
                    <tbody>
                        <tr>
                            <td><strong>Name:</strong></td>
                            <td><span id="judgeName"></span></td>
                        </tr>
                        <tr>
                            <td><strong>Phone:</strong></td>
                            <td><span id="judgePhone"></span></td>
                        </tr>
                        <tr>
                            <td><strong>Court:</strong></td>
                            <td><span id="judgeCourt"></span></td>
                        </tr>
                        <tr>
                            <td><strong>Title:</strong></td>
                            <td><span id="judgeTitle"></span></td>
                        </tr>
                    </tbody>
                </table>
                <input type="hidden" id="hdJudgeId" />
            </div>
            <div class="modal-footer justify-content-around">
                <button type="button" class="btn btn-primary" data-bs-dismiss="modal" data-bs-toggle="modal" data-bs-target="#JudgeEditModal" id="editJudgeBtn"><i class="fas fa-edit me-2"></i>&nbsp;Edit</button>
                <button type="button" id="cmdDelete" class="btn btn-danger" data-bs-dismiss="modal"><i class="fa fa-trash me-2"></i>&nbsp;Delete</button>
                <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Close</button>
            </div>
        </div>
    </div>
</div>
<!-- Edit Modal -->
<div class="modal fade" id="JudgeEditModal" tabindex="-1" aria-labelledby="JudgeEditModalLabel" aria-hidden="true">
    <div class="modal-dialog modal-lg">
        <div class="modal-content">
            <div id="edit_progress_judge" class="modal-progress" style="display: none;">
                <div class="center-progress">
                    <img alt="" src="/images/loading.gif" />
                </div>
            </div>
            <div class="modal-header">
                <h4 class="modal-title" id="JudgeEditModalLabel">Edit Judge</h4>
                <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
            </div>
            <div class="modal-body">
                <div class="container-fluid">
                    <input type="hidden" id="edit_hdJudgeId">
                    <div class="form-group">
                        <div class="row">
                            <div class="col-md-8">
                                <label>Name<em>*</em></label>
                                <select id="edit_judgeName" class="form-control" required>
                                    <option value="">Select Judge</option>
                                </select>
                                <input type="text" id="edit_judgeNameText" class="form-control" style="display: none;" required> 
                                <div class="invalid-feedback" id="edit_judgeName_error">Judge is Required.</div>
                                <div id="judgeHelp" class="form-text mb-0">Judges must have an existing account on this site and must be added to the Judge Role before assignment can be made.</div>
                               
                            </div>

                            <div class="col-md-4">
                                <label>Title</label>
                                <select id="edit_judgeTitle" class="form-control">
                                    <option value="Judge">Judge</option>
                                    <option value="Mediator">Mediator</option>
                                    <option value="Magistrate">Magistrate</option>
                                    <option value="Case Manager">Case Manager</option>
                                    <option value="Hearing Officer">Hearing Officer</option>
                                </select>
                            </div>

                        </div>
                        <div class="row">
                            <div class="col-md-8">
                                <label>Court</label>
                                <select id="edit_judgeCourt" class="form-control" aria-describedby="courtHelp">
                                    <option value="">Select Court</option>
                                    <!-- Courts will be populated dynamically -->
                                </select>
                                <div id="courtHelp" class="form-text mb-0">Only unassigned Courts appear in the list</div>

                            </div>
                            <div class="col-md-4">
                                <label>Phone</label>
                                <input type="text" id="edit_judgePhone" class="form-control phone">
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
<dnn:DnnJsInclude runat="server" FilePath="~/DesktopModules/tjc.modules/JACS/js/imask.js" ForceProvider="DnnFormBottomProvider" />
<dnn:DnnJsInclude runat="server" FilePath="~/DesktopModules/tjc.modules/JACS/js/jacs.js" ForceProvider="DnnFormBottomProvider" Priority="100" />
<dnn:DnnJsInclude runat="server" FilePath="~/DesktopModules/tjc.modules/JACS/js/judge.js" ForceProvider="DnnFormBottomProvider" Priority="101" />
<dnn:DnnCssInclude runat="server" FilePath="~/Resources/Libraries/DataTables/dataTables.bootstrap5.min.css" />
<dnn:DnnJsInclude runat="server" FilePath="~/Resources/Libraries/DataTables/datatables.min.js" />
<dnn:DnnJsInclude runat="server" FilePath="~/Resources/Libraries/DataTables/dataTables.bootstrap5.min.js" />
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/sweetalert/sweetalert2.min.js" />
<dnn:DnnCssInclude runat="server" FilePath="/Resources/Libraries/sweetalert/sweetalert2.min.css" />
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/Noty/noty.min.js" />
<dnn:DnnCssInclude runat="server" FilePath="/Resources/Libraries/Noty/noty.min.css" />

<script>
    var moduleId = <%=ModuleId%>;
    var portalId = <%=PortalId%>;
    var service = {
        path: "JACS",
        framework: $.ServicesFramework(moduleId)
    };

    (function ($, Sys) {
        $(document).ready(function () {
                const element = document.getElementById('edit_judgePhone');
                const maskOptions = {
                    mask: '(000) 000-0000'
                };
                const mask = IMask(element, maskOptions);
            try {
                if (typeof JudgeController === 'undefined') {
                    console.error('JudgeController is not defined. Check if Script(judge.js) loaded correctly.');
                    return;
                }
                const judgeController = new JudgeController({
                    moduleId: moduleId,
                    userId: <%=UserId%>,
                    isAdmin: "<%=IsAdmin%>",
                    adminRole: "<%=AdminRole%>",
                    judgeRole: "<%=JudgeRole%>",
                    portalId: portalId,
                    service: service,
                    currentPage: 0,
                    pageSize: 25,
                    recordCount: 0,
                    sortColumnIndex: 2,
                    sortDirection: "asc"
                });
                judgeController.init();
            } catch (e) {
                console.error('Error initializing JudgeController:', e);
            }
        });
    }(jQuery, window.Sys));
</script>
