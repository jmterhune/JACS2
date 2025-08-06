<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CourtExtendView.ascx.cs" Inherits="tjc.Modules.jacs.CourtExtendView" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<%@ Register Src="Controls/navbar.ascx" TagPrefix="tb" TagName="navbar" %>

<section class="navbar border-0 mb-0 justify-content-start">
    <button class="btn btn-default me-3" id="btnToggleMenu" type="button" data-bs-toggle="collapse" data-bs-target="#sidebarMenu" aria-controls="sidebarMenu" aria-expanded="true" aria-label="Toggle navigation">
        <i class="fa-solid fa-bars"></i>Menu
    </button>
    <h2 class="mb-0">Extend Court Calendar</h2>
</section>
<div class="d-flex">
    <tb:navbar runat="server" ID="navbar" />
    <main class="main flex-grow-1 p-3 pt-0">
        <h3 class="mb-2">Court Name: <span class="text-capitalize">
            <asp:Literal ID="ltCourtName" runat="server" /></span></h3>
        <div class="alert alert-info mb-4">
            <i class="fa fa-info-circle"></i><strong>Note:</strong> This will extend the calendar based on the order of the automated templates.
        </div>
        <div class="row">
            <div class="col-md-8">
                <form id="extendForm" action="#" method="post">
                    <asp:HiddenField ID="hdCourtId" runat="server" />
                    <div class="card">
                        <div class="card-body row">
                            <div class="form-group col-md-12">
                                <asp:Literal ID="ltLastTimeslot" runat="server" />
                                <asp:Literal ID="ltLastTemplateTimeslot" runat="server" />
                                <asp:Literal ID="ltLastHearing" runat="server" />
                            </div>
                            <div class="form-group col-md-6">
                                <label for="startTemplate">Starting Template</label>
                                <asp:DropDownList ID="ddlStartTemplate" runat="server" CssClass="form-control" required="required" />
                            </div>
                            <div class="form-group col-md-6 required">
                                <label for="weeks">Weeks to Extend</label>
                                <asp:TextBox ID="txtWeeks" runat="server" CssClass="form-control" TextMode="Number" required="required" />
                            </div>
                            <div class="form-group col-md-6 required">
                                <label for="startDate">Start Date</label>
                                <asp:TextBox ID="txtStartDate" runat="server" CssClass="form-control datepicker" required="required" />
                            </div>
                        </div>
                    </div>
                    <div class="form-group mt-3">
                        <asp:Button ID="btnExtend" runat="server" CssClass="btn btn-success" Text="Extend" OnClientClick="return submitExtendForm();" />
                        <a href="#" class="btn btn-default" id="btnCancel">Cancel</a>
                    </div>
                </form>
            </div>
        </div>
    </main>
</div>

<dnn:DnnCssInclude runat="server" FilePath="/Resources/Libraries/BootstrapDatepicker/bootstrap-datepicker.min.css" />
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/Moment/moment.min.js" />
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/BootstrapDatepicker/bootstrap-datepicker.min.js" />
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/sweetalert/sweetalert2.min.js" />
<dnn:DnnCssInclude runat="server" FilePath="/Resources/Libraries/sweetalert/sweetalert2.min.css" />
<dnn:DnnJsInclude runat="server" FilePath="~/DesktopModules/tjc.modules/JACS/js/courtextend.js" ForceProvider="DnnFormBottomProvider" Priority="103" />

<script>
    var moduleId = <%=ModuleId%>;
    var service = {
        path: "JACS",
        framework: $.ServicesFramework(moduleId)
    };
    (function ($, Sys) {
        $(document).ready(function () {
            try {
                if (typeof CourtExtendController === 'undefined') {
                    console.error('CourtExtendController is not defined.');
                    return;
                }
                const courtExtendController = new CourtExtendController({
                    moduleId: moduleId,
                    userId: <%=UserId%>,
                    isAdmin: "<%=IsAdmin%>",
                    adminRole: 'AdminRole',
                    service: service,
                    courtId: <%=CourtId%>,
                    cancelUrl:"<%=CourtCalendarUrl%>"
                });
                courtExtendController.init();
            } catch (e) {
                console.error('Error initializing CourtExtendController:', e);
            }
        });
    }(jQuery, window.Sys));

</script>
