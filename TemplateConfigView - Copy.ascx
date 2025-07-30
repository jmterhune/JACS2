<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TemplateConfigView.ascx.cs" Inherits="tjc.Modules.jacs.TemplateConfigView" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<%@ Register Src="Controls/navbar.ascx" TagPrefix="tb" TagName="navbar" %>

<section class="navbar border-0 mb-0 justify-content-start">
    <button class="btn btn-default me-3" id="btnToggleMenu" type="button" data-bs-toggle="collapse" data-bs-target="#sidebarMenu" aria-controls="sidebarMenu" aria-expanded="true" aria-label="Toggle navigation">
        <i class="fa-solid fa-bars"></i>
    </button>
    <h2 class="mb-0">Template Configuration</h2>
</section>

<div class="d-flex">
    <tb:navbar runat="server" ID="navbar" />
    <main class="main flex-grow-1 p-3 pt-0">
        <h3 class="mb-2"><span class="text-uppercase"><asp:Literal ID="ltCourtName" runat="server" /></span> - <span class="text-capitalize"><asp:Literal ID="ltTemplateName" runat="server" /></span></h3>
        <h4 class="mb-2"></h4>
        <div class="template-actions d-flex mb-0">
            <div class="actions me-auto">
                <a href="#" class="btn btn-default m-1" id="multiDeleteBtn"><i class="fa fa-lg fa-trash mr-2"></i> Delete Timeslot(s)</a>
                <a href="#" class="btn btn-default m-1" id="multiCopyBtn"><i class="fa fa-lg fa-copy mr-2"></i> Copy Timeslot(s)</a>
            </div>
        </div>
        <div id="calendar"></div>
    </main>
</div>

<!-- Timeslot Modal -->
<div class="modal fade" id="TimeslotModal" tabindex="-1" aria-labelledby="TimeslotModalLabel" aria-hidden="true">
    <div class="modal-dialog modal-lg modal-dialog-centered">
        <div class="modal-content p-3">
            <div id="progress-timeslot" class="modal-progress" style="display: none;">
                <div class="center-progress">
                    <img alt="" src="/images/loading.gif" />
                </div>
            </div>
            <div class="modal-header">
                <h4 class="modal-title" id="TimeslotModalLabel">Create...</h4>
                <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
            </div>
            <ul class="nav nav-tabs" id="myTab" role="tablist">
                <li class="nav-item" id="timeslot-nav">
                    <a class="nav-link active" id="timeslot-tab-link" data-bs-toggle="tab" href="#timeslotTab" role="tab" aria-controls="timeslotTab" aria-selected="true">Timeslot(s)</a>
                </li>
            </ul>
            <div class="modal-body tab-content">
                <div class="tab-pane active" id="timeslotTab" role="tabpanel" aria-labelledby="timeslot-tab">
                    <form id="timeslotForm" data-action="">
                        <input type="hidden" name="court_id" value="<%=CourtId%>" />
                        <input type="hidden" name="template_id" value="<%=TemplateId%>" />
                        <input type="hidden" id="edit_timeslotId" name="id" />
                        <div class="row blocking">
                            <div class="col-md-2">
                                <div class="form-check">
                                    <input type="checkbox" class="form-check-input" id="blocked" name="blocked">
                                    <label class="form-check-label" for="blocked">Block</label>
                                </div>
                            </div>
                            <div class="col-md-2 public_block" style="display: none;">
                                <div class="form-check">
                                    <input type="checkbox" class="form-check-input" id="public_block" name="public_block">
                                    <label class="form-check-label" for="public_block">Public Block</label>
                                </div>
                            </div>
                            <div class="col-md-12 mb-3 block_reason" style="display: none;">
                                <label for="block_reason">Block Reason</label>
                                <input type="text" class="form-control" name="block_reason" id="block_reason">
                                <div class="invalid-feedback">Block Reason is required when Block is checked.</div>
                            </div>
                        </div>
                        <div class="row cattle-call">
                            <div class="col-md-6 mb-3">
                                <label>Calendar Call?</label>
                                <div class="form-check form-check-inline">
                                    <input class="form-check-input" type="radio" name="cattlecall" id="cattlecall_yes" value="1" checked>
                                    <label class="form-check-label" for="cattlecall_yes">Yes (Concurrent)</label>
                                </div>
                                <div class="form-check form-check-inline">
                                    <input class="form-check-input" type="radio" name="cattlecall" id="cattlecall_no" value="0">
                                    <label class="form-check-label" for="cattlecall_no">No (Consecutive)</label>
                                </div>
                            </div>
                        </div>
                        <div class="row time-selection">
                            <div class="col-md-2 mb-3">
                                <label for="timeslot_start">Start Time</label>
                                <div class="input-group date" id="timeslot_start" data-target-input="nearest">
                                    <input type="text" name="timeslot_start" class="form-control datetimepicker-input" data-target="#timeslot_start" required />
                                    <div class="invalid-feedback">Start Time is required.</div>
                                </div>
                                <input type="hidden" name="t_start" id="t_start" />
                            </div>
                            <div class="col-md-2 mb-3">
                                <label for="timeslot_end">End Time</label>
                                <div class="input-group date" id="timeslot_end" data-target-input="nearest">
                                    <input type="text" name="timeslot_end" class="form-control datetimepicker-input" data-target="#timeslot_end" required />
                                    <div class="invalid-feedback">End Time is required.</div>
                                </div>
                                <input type="hidden" name="t_end" id="t_end" />
                            </div>
                            <div class="col-md-3 mb-3">
                                <label for="duration">Duration</label>
                                <select class="form-control" id="duration" name="duration" required>
                                    <option value="">-</option>
                                    <option value="5">5 mins</option>
                                    <option value="10">10 mins</option>
                                    <option value="15">15 mins</option>
                                    <option value="20">20 mins</option>
                                    <option value="30">30 mins</option>
                                    <option value="45">45 mins</option>
                                    <option value="60">1 hour</option>
                                    <option value="90">1.5 hours</option>
                                    <option value="120">2 hours</option>
                                    <option value="150">2.5 hours</option>
                                    <option value="165">2.75 hours</option>
                                    <option value="180">3 hours</option>
                                    <option value="210">3.5 hours</option>
                                    <option value="240">4 hours</option>
                                    <option value="300">5 hours</option>
                                    <option value="360">6 hours</option>
                                    <option value="480">8 hours</option>
                                </select>
                                <div class="invalid-feedback">Duration is required.</div>
                            </div>
                            <div class="col-md-3 mb-3 quantity-group">
                                <label for="quantity">Quantity</label>
                                <input type="number" name="quantity" id="quantity" class="form-control" min="1" required />
                                <div class="invalid-feedback">Quantity must be at least 1.</div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-6 mb-3">
                                <label for="description">Description</label>
                                <input type="text" class="form-control" name="description" id="description">
                                <div class="valid-feedback">Looks good!</div>
                            </div>
                            <div class="col-md-6 mb-3">
                                <label for="category_id">Category</label>
                                <select class="form-control" id="category_id" name="category_id">
                                    <option value=""> - </option>
                                    <!-- Populated via AJAX -->
                                </select>
                            </div>
                        </div>
                        <div class="row restricted">
                            <div class="col-md-12 mb-3">
                                <label for="timeslot_motions">Restricted Motions</label>
                                <select name="timeslot_motions[]" multiple id="timeslot_motions" autocomplete="off">
                                    <!-- Populated via AJAX -->
                                </select>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-6 text-left">
                                <a class="btn btn-danger delete-button" href="#" id="deleteTimeslotBtn" style="display: none;">Delete</a>
                            </div>
                            <div class="col-md-6 text-right">
                                <button type="submit" class="btn btn-primary">Save changes</button>
                            </div>
                        </div>
                    </form>
                </div>
            </div>
        </div>
    </div>
</div>

<!-- Styles and Scripts -->
<dnn:DnnJsInclude runat="server" FilePath="~/DesktopModules/tjc.modules/JACS/js/jacs.js" ForceProvider="DnnFormBottomProvider" Priority="100" />
<dnn:DnnJsInclude runat="server" FilePath="~/DesktopModules/tjc.modules/JACS/js/templateConfig.js" ForceProvider="DnnFormBottomProvider" Priority="101" />
<dnn:DnnCssInclude runat="server" FilePath="/Resources/Libraries/TomSelect/tom-select.default.min.css" />
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/TomSelect/tom-select.complete.min.js" />
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/moment/moment.min.js" />
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/fullcalendar/dist/index.global.min.js" />
<dnn:DnnCssInclude runat="server" FilePath="/Resources/Libraries/sweetalert/sweetalert2.min.css" />
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/sweetalert/sweetalert2.min.js" />
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/Noty/noty.min.js" />
<dnn:DnnCssInclude runat="server" FilePath="/Resources/Libraries/Noty/noty.min.css" />
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/tempusdominus/tempusdominus-bootstrap-4.min.js" />
<dnn:DnnCssInclude runat="server" FilePath="/Resources/Libraries/tempusdominus/tempusdominus-bootstrap-4.min.css" />


<script type="text/javascript">
    var moduleId = <%=ModuleId%>;
    var service = {
        path: "JACS",
        framework: $.ServicesFramework(moduleId)
    };

    $(document).ready(function () {
        try {
            if (typeof TemplateConfigController === 'undefined') {
                console.error('TemplateConfigController is not defined. Check if templateConfig.js loaded correctly.');
                return;
            }
            const templateConfigController = new TemplateConfigController({
                moduleId: moduleId,
                userId: <%=UserId%>,
                isAdmin: "<%=IsAdmin%>",
                adminRole: "<%=AdminRole%>",
                service: service,
                templateId: <%=TemplateId%>,
                courtId: <%=CourtId%>,
            });
            templateConfigController.init();
        } catch (e) {
            console.error('Error initializing TemplateConfigController:', e);
        }
    });
</script>