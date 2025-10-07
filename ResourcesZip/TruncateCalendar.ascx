<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TruncateCalendar.ascx.cs" Inherits="tjc.Modules.jacs.TruncateCalendar" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<%@ Register Src="Controls/navbar.ascx" TagPrefix="tb" TagName="navbar" %>

<section class="navbar border-0 mb-0 justify-content-start">
    <button class="btn btn-default me-3" id="btnToggleMenu" type="button" data-bs-toggle="collapse" data-bs-target="#sidebarMenu" aria-controls="sidebarMenu" aria-expanded="true" aria-label="Toggle navigation">
        <i class="fa-solid fa-bars"></i>Menu
    </button>
    <h2 class="mb-0">Truncate Court Calendar</h2>
</section>
<div class="d-flex">
    <tb:navbar runat="server" ID="navbar" />
    <main class="main flex-grow-1 p-3 pt-0">
        <h3 class="mb-2">Court Name: <span class="text-capitalize">
            <asp:Literal ID="ltCourtName" runat="server" ClientIDMode="Static" /></span></h3>
        <div class="alert alert-info mb-4">
            <i class="fa fa-info-circle"></i><strong>Note:</strong> Truncating the calendar is an irreversible action. All scheduled hearings will be canceled and corresponding notifications will be sent to attorneys and the JA (if option is enabled).
        </div>
        <div class="row">
            <div class="col-md-8">
                <asp:HiddenField ID="hdCourtId" runat="server" ClientIDMode="Static" />
                <div class="card">
                    <div class="card-body row">
                        <div class="col-md-12">
                            <asp:Literal ID="ltLastTimeslot" runat="server" ClientIDMode="Static" />
                        </div>
                        <div class="col-auto">
                            <label for="truncateDate">Truncate Date</label>
                            <asp:TextBox ID="txtTruncateDate" runat="server" CssClass="form-control datepicker" required="required" ClientIDMode="Static" />
                        </div>
                    </div>
                    <div class="row mt-3">
                        <div class="col-md-6">
                            <asp:RadioButtonList ID="ddlFilter" runat="server" CssClass="form-control" ClientIDMode="Static">
                                <asp:ListItem Value="all" Selected="True">Truncate all timeslots from date forward</asp:ListItem>
                                <asp:ListItem Value="hearings">Preserve scheduled hearings only</asp:ListItem>
                                <asp:ListItem Value="templates">Preserve blocked non-template timeslots only</asp:ListItem>
                                <asp:ListItem Value="both">Preserve both hearings and blocked non-template timeslots</asp:ListItem>
                            </asp:RadioButtonList>

                        </div>
                    </div>
                </div>
            </div>
            <div class="mt-3">
                <button type="button" class="btn btn-primary" id="btnTruncate"><i class="fa fa-lg fa-calendar-minus"></i>Truncate</button>
                <a href="#" class="btn btn-default" id="btnCancel"><i class="fa fa-ban"></i>Cancel</a>
            </div>
        </div>
    </main>
</div>

<dnn:DnnCssInclude runat="server" FilePath="/Resources/Libraries/Bootstrap/bootstrap-datepicker.min.css" />
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/Moment/moment.min.js" />
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/Bootstrap/bootstrap-datepicker.min.js" />
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/sweetalert/sweetalert2.min.js" />
<dnn:DnnCssInclude runat="server" FilePath="/Resources/Libraries/sweetalert/sweetalert2.min.css" />
<dnn:DnnJsInclude runat="server" FilePath="~/DesktopModules/tjc.modules/JACS/js/truncateCalendar.js" ForceProvider="DnnFormBottomProvider" Priority="103" />

<script>
    var moduleId = <%=ModuleId%>;
    var service = {
        path: "JACS",
        framework: $.ServicesFramework(moduleId)
    };

    (function ($, Sys) {
        $(document).ready(function () {
            try {
                if (typeof CourtTruncateController === 'undefined') {
                    console.error('CourtTruncateController is not defined.');
                    return;
                }
                const courtTruncateController = new CourtTruncateController({
                    moduleId: moduleId,
                    userId: <%=UserId%>,
                    isAdmin: "<%=IsAdmin%>",
                    adminRole: 'AdminRole',
                    service: service,
                    courtId: <%=CourtId%>,
                    cancelUrl:"<%=CourtCalendarUrl%>"
                });
                courtTruncateController.init();
            } catch (e) {
                console.error('Error initializing CourtTruncateController:', e);
            }
        });
    }(jQuery, window.Sys));
</script>
