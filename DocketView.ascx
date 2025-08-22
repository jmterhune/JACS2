<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DocketView.ascx.cs" Inherits="tjc.Modules.jacs.DocketView" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<%@ Register Src="Controls/navbar.ascx" TagPrefix="tb" TagName="navbar" %>

<section class="navbar border-0 mb-0 justify-content-start">
    <button class="btn btn-default me-3" id="btnToggleMenu" type="button" data-bs-toggle="collapse" data-bs-target="#sidebarMenu" aria-controls="sidebarMenu" aria-expanded="true" aria-label="Toggle navigation">
        <i class="fa-solid fa-bars"></i>
    </button>
    <h2 class="mb-0">Docket Report</h2>
</section>

<div class="d-flex">
    <tb:navbar runat="server" ID="navbar" />
    <main class="main flex-grow-1 p-3 pt-0">
        <div class="row">
            <div class="col-md-7 col-sm-12 bold-labels">
                <div class="alert alert-info">
                    <strong class="fs-3"><i class="fas fa-info-circle fs-4"></i>Note</strong><div class="mt-3">This allows a user to print the docket report for a calendar they have access to. </div>
                </div>
                <div class="card">
                    <div class="card-body form-group">
                        <div class="row">
                            <div class="col-md-12">
                                <label>Judge</label>
                                <select id="courtFilter" name="court" class="form-control"></select>
                            </div>
                            <div class="col-md-12">
                                <label>Categories</label>
                                <select id="categoryFilter" name="category" class="form-control categories_select" style="width: 100%">
                                    <option value="0">All</option>
                                </select>
                            </div>
                            <div class="col-md-6">
                                <label for="from">From Date</label>
                                <input type="date" class="form-control" name="from" id="from" value="<%= DateTime.Now.ToString("yyyy-MM-dd") %>" />
                            </div>
                            <div class="col-md-6">
                                <label>To Date</label>
                                <input type="date" class="form-control" name="to" id="to" />
                            </div>
                            <div class="col-md-12">
                                <label class="d-block">Hearings</label>
                                <div class="form-check form-check-inline">
                                    <input type="radio" class="form-check-input" name="hearing" value="all" id="all" checked />
                                    <label class="radio-inline form-check-label font-weight-normal" for="all">All</label>
                                </div>
                                <div class="form-check form-check-inline">
                                    <input type="radio" class="form-check-input" name="hearing" value="addon" id="addon" />
                                    <label class="radio-inline form-check-label font-weight-normal" for="addon">Add On Only</label>
                                </div>
                                <div class="form-check form-check-inline">
                                    <input type="radio" class="form-check-input" name="hearing" value="noaddon" id="noaddon" />
                                    <label class="radio-inline form-check-label font-weight-normal" for="noaddon">No Add On</label>
                                </div>
                            </div>
                            <div class="col-sm-12 mb-0 mt-2">
                                <h6 class="text-primary mt-4">Optional Fields</h6>
                                <hr class="mb-0" />
                            </div>
                            <div class="col-md-12">
                                <label class="d-block"></label>
                                <div class="form-check form-check-inline">
                                    <input type="checkbox" class="form-check-input" id="categoryPrint" name="category_print" value="1" checked />
                                    <label class="form-check-label" for="categoryPrint">Print Category</label>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div id="saveActions" class="form-group">
                    <div class="btn-group" role="group">
                        <button type="button" id="printDocket" class="btn btn-success">
                            <span class="fa fa-print" role="presentation" aria-hidden="true"></span>&nbsp;
                            <span data-value="save_and_back">Print Docket</span>
                        </button>
                    </div>
                    <a href='<%=MainViewUrl %>' class="btn btn-default"><span class="fa fa-ban"></span>&nbsp;Cancel</a>
                </div>
            </div>
        </div>
    </main>
</div>

<dnn:DnnJsInclude runat="server" FilePath="~/Resources/Libraries/select2/js/select2.full.min.js" />
<dnn:DnnCssInclude runat="server" FilePath="~/Resources/Libraries/select2/css/select2.min.css" />
<dnn:DnnCssInclude runat="server" FilePath="~/Resources/Libraries/select2/css/select2-bootstrap-5-theme.min.css" />
<dnn:DnnJsInclude runat="server" FilePath="~/Resources/Libraries/sweetalert/sweetalert2.min.js" />
<dnn:DnnCssInclude runat="server" FilePath="~/Resources/Libraries/sweetalert/sweetalert2.min.css" />
<dnn:DnnJsInclude runat="server" FilePath="~/Resources/Libraries/Noty/noty.min.js" />
<dnn:DnnCssInclude runat="server" FilePath="~/Resources/Libraries/Noty/noty.min.css" />

<dnn:DnnJsInclude runat="server" FilePath="~/DesktopModules/tjc.modules/JACS/js/docket.js" ForceProvider="DnnFormBottomProvider" Priority="101" />

<script>
    var moduleId = <%=ModuleId%>;
    var service = {
        path: "JACS",
        framework: $.ServicesFramework(moduleId)
    };

    (function ($, Sys) {
        $(document).ready(function () {
            try {
                if (typeof DocketController === 'undefined') {
                    console.error('DocketController is not defined. Check if docket.js loaded correctly.');
                    return;
                }
                const docketController = new DocketController({
                    moduleId: moduleId,
                    userId: <%=UserId%>,
                    isAdmin: "<%=IsAdmin%>",
                    adminRole: "<%=AdminRole%>",
                    service: service,
                });
                docketController.init();
            } catch (e) {
                console.error('Error initializing DocketController:', e);
            }
        });
    }(jQuery, window.Sys));
</script>
