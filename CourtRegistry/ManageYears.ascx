<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ManageYears.ascx.cs" Inherits="tjc.Modules.CourtRegistry.ManageYears" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<%@ Register TagPrefix="dnn" TagName="TextEditor" Src="~/controls/TextEditor.ascx" %>
<dnn:dnnjsinclude runat="server" filepath="~/DesktopModules/tjc.modules/CourtRegistry/Scripts/registry-ui.js" />
<div class="tabs">
    <ul class="nav nav-tabs">
        <li class="nav-item">
            <a class="nav-link" href="<%=ApplicationListUrl%>">Applications</a>
        </li>
        <li class="nav-item active">
            <a class="nav-link" href="#fiscal-years" data-toggle="tab">Manage Fiscal Years</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=ExceptionListUrl%>">Exceptions</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=BasicSettingsUrl%>">Basic Settings</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=AttorneyListUrl%>">Attorneys</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=JacCodeListUrl%>">JAC Codes</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=UpdateJacCodeUrl%>">Update JAC</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=LocationListUrl%>">Locations</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=CaseTypeListUrl%>">Case Types</a>
        </li>
    </ul>
    <div class="tab-content">
        <div id="fiscal-years" class="tab-pane active">
            <asp:UpdatePanel ID="pnlPeriods" runat="server" RenderMode="Block" OnUnload="pnlPeriods_Unload">
                <ContentTemplate>
                    <asp:UpdateProgress ID="upProgressEvent" runat="server">
                        <ProgressTemplate>
                            <div class="modal-progress">
                                <div class="center-progress">
                                    <img alt="" src="/images/loading.gif" />
                                </div>
                            </div>
                        </ProgressTemplate>
                    </asp:UpdateProgress>
                    <div class="alert alert-info">
                        <i class="fas fa-info-circle"></i>The table below displays the Application Periods currently active.
                The Modification Deadline determines the date at which changes to applications will no longer be accepted.
                To stop new applications from being submitted uncheck the Accepting New Applications box.
                    </div>
                    <button class="btn btn-primary" id="cmdNew" data-toggle="modal" data-target="#addRecord"><i class="fas fa-plus"></i>&nbsp;Add New Record</button>
<asp:Literal ID="ltModalScript" runat="server" EnableViewState="false" />
                    <asp:Repeater runat="server" ID="rptYears" OnItemCommand="rptYears_ItemCommand">
                        <HeaderTemplate>
                            <table class="table table-striped">
                                <thead>
                                    <tr>
                                        <th>&nbsp;</th>
                                        <th>Year Period Ends</th>
                                        <th>Modification Deadline</th>
                                        <th>Accepting New Applications</th>
                                        <th>&nbsp;</th>
                                    </tr>
                                </thead>
                                <tbody>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <tr>
                                <td class="command-item">
                                    <asp:LinkButton ID="lnkEditYear" runat="server" CausesValidation="false" CommandName="edit" CommandArgument='<%#Eval("ApplicationYear") %>' CssClass="text-primary" ToolTip="Edit Record"><i class="fas fa-edit"></i></asp:LinkButton>
                                </td>
                                <td>
                                    <%#Eval("ApplicationYear") %>
                                </td>
                                <td>
                                    <%#Eval("ModificationDeadline","{0:MM/dd/yyyy}") %>
                                </td>
                                <td>
                                    <%#Boolean.Parse(Eval("AcceptingNewApplications").ToString())==true? "<i class='fas fa-square-check'></i>":"<i class='fas fa-square'></i>" %>
                                </td>
                                <td>
                                    <asp:LinkButton ID="lnkDelete" runat="server" CausesValidation="false" OnClientClick="return Registry.confirmDelete(this,'Fiscal Year');" CommandName="delete" CommandArgument='<%#Eval("ApplicationYear").ToString() %>' CssClass="command-item text-danger"><i class="fas fa-trash"></i></asp:LinkButton>
                                </td>
                            </tr>
                        </ItemTemplate>
                        <FooterTemplate>
                            </tbody>
                    </table>
                        </FooterTemplate>
                    </asp:Repeater>
                    <div class="modal fade" id="addRecord" tabindex="-1" role="dialog" aria-labelledby="addRecordModalLabel" aria-hidden="true">
                        <div class="modal-dialog">
                            <div class="modal-content">
                                <div class="modal-header">
                                    <h4 class="modal-title" id="addRecordModalLabel">Add Fiscal Year</h4>
                                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                                </div>
                                <div class="modal-body">
                                    <div class="row">
                                        <div class="col-auto">
                                            <asp:Label runat="server" AssociatedControlID="txtYearPeriodEnds" Text="Year Period Ends" />
                                            <asp:TextBox runat="server" CssClass="form-control" ID="txtYearPeriodEnds" />
                                            <asp:RequiredFieldValidator runat="server" ControlToValidate="txtYearPeriodEnds" ValidationGroup="new"
                                                Display="Dynamic" SetFocusOnError="true" CssClass="label label-danger" ErrorMessage="Year Period Ends is Required" />
                                        </div>
                                        <div class="col-auto">
                                            <asp:Label runat="server" AssociatedControlID="txtModificationDeadline" Text="Modification Deadline" />
                                            <asp:TextBox runat="server" CssClass="form-control date-picker" ID="txtModificationDeadline" />
                                            <asp:RequiredFieldValidator runat="server" ControlToValidate="txtModificationDeadline" ValidationGroup="new"
                                                Display="Dynamic" SetFocusOnError="true" CssClass="label label-danger" ErrorMessage="Modification Deadline is Required" />
                                        </div>
                                        <div class="col-auto pt-3">
                                            <asp:CheckBox Text="Accepting New Applications" ClientIDMode="Static" ID="chkAcceptingApplications" CssClass="form-check" runat="server" />
                                        </div>
                                    </div>
                                </div>
                                <div class="modal-footer justify-content-between">
                                    <asp:Button Text="Add Record" ID="cmdAddRecord" runat="server" CssClass="btn btn-primary" ValidationGroup="new" OnClick="cmdAddRecord_Click" />
                                    <button type="button" class="btn btn-default" data-dismiss="modal">Close</button>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-4">
                            <fieldset class="outline-fieldset">
                                <legend>JAC Report for Clerks and Judges</legend>
                                <div class="row  form-group">
                                    <div class="col-auto">
                                        <asp:Label runat="server" AssociatedControlID="drpLocations" Text="Select Location" />
                                        <asp:DropDownList ID="drpLocations" CssClass="form-control" DataTextField="LocationName" DataValueField="LocationId" runat="server">
                                        </asp:DropDownList>
                                    </div>
                                    <div class="col-auto">
                                        <asp:Label runat="server" AssociatedControlID="drpYear" Text="Select Fiscal Year" />
                                        <asp:DropDownList ID="drpYear" DataValueField="ApplicationYear" DataTextField="PeriodYear" CssClass="form-control" runat="server">
                                        </asp:DropDownList>
                                    </div>

                                </div>
                                <asp:Button ID="cmdViewRegistry" CssClass="btn btn-primary" Text="View JAC Registry" OnClick="cmdViewRegistry_Click" runat="server" />

                            </fieldset>
                        </div>
                        <div class="col-4">
                            <fieldset class="outline-fieldset">
                                <legend>JAC Code Count Report</legend>
                                <div class="row form-group">
                                    <div class="col-auto">
                                        <asp:Label runat="server" AssociatedControlID="drpExportYear" Text="Select Fiscal Year" />
                                        <asp:DropDownList ID="drpExportYear" DataValueField="ApplicationYear" DataTextField="PeriodYear" CssClass="form-control" runat="server">
                                        </asp:DropDownList>
                                    </div>
                                </div>
                                <asp:Button ID="cmdJACReport" CssClass="btn btn-primary" Text="View JAC Code Counts" OnClick="cmdJACReport_Click" runat="server" />
                            </fieldset>
                        </div>
                        <div class="col-4">
                            <fieldset class="outline-fieldset">
                                <legend>Send Customize Emails to Attorneys</legend>
                                <button class="btn btn-primary" data-toggle="modal" data-target="#emailModal"><i class="fas fa-email"></i>Create Email</button>
                            </fieldset>
                        </div>
                    </div>
                    <div class="modal fade" id="emailModal" tabindex="-1" role="dialog" aria-labelledby="emailModalLabel" aria-hidden="true">
                        <div class="modal-dialog modal-lg">
                            <div class="modal-content">
                                <div class="modal-header">
                                    <h4 class="modal-title" id="emailModalLabel">Send Bulk Emails</h4>
                                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                                </div>
                                <div class="modal-body">
                                    <asp:Literal ID="ltMessage" runat="server" />
                                    <div class="row">
                                        <div class="col-auto">
                                            <asp:Label runat="server" AssociatedControlID="txtEmailSubject" Text="Subject" />
                                            <asp:TextBox runat="server" CssClass="form-control" ID="txtEmailSubject" />
                                            <asp:RequiredFieldValidator runat="server" ControlToValidate="txtEmailSubject" ValidationGroup="email"
                                                Display="Dynamic" SetFocusOnError="true" CssClass="label label-danger" ErrorMessage="Subject is Required" />
                                        </div>
                                    </div>
                                    <div>
                                        <asp:Label runat="server" AssociatedControlID="txtBody" Text="Message Body" />
                                        <dnn:texteditor id="txtBody" runat="server" height="500" width="100%" />
                                        <asp:CustomValidator ID="valBody" ClientValidationFunction="ValidateEmailBody" runat="server" ControlToValidate="txtBody" ValidationGroup="email"
                                            Display="Dynamic" SetFocusOnError="true" CssClass="label label-danger" ErrorMessage="Email Message is Required">
                                        </asp:CustomValidator>
                                    </div>
                                    <div class="row">
                                        <div class="col-auto">
                                            <asp:Label runat="server" AssociatedControlID="drpEmailYear" Text="Select Fiscal Year" />
                                            <asp:DropDownList ID="drpEmailYear" DataValueField="ApplicationYear" DataTextField="PeriodYear" CssClass="form-control" runat="server">
                                            </asp:DropDownList>
                                        </div>
                                        <div class="col-auto">
                                            <asp:Label runat="server" AssociatedControlID="drpEmailYear" Text="Send To" />
                                            <asp:DropDownList ID="drpAttorneys" CssClass="form-control" runat="server">
                                                <asp:ListItem Value="1">All Attorneys</asp:ListItem>
                                                <asp:ListItem Value="2">Selected Fiscal Period</asp:ListItem>
                                            </asp:DropDownList>
                                        </div>
                                    </div>
                                    <p>
                                    </p>
                                </div>
                                <div class="modal-footer justify-content-between">
                                    <asp:Button ID="cmdSend" CssClass="btn btn-primary" Text="Send" runat="server" OnClick="cmdSend_Click" />
                                    <button type="button" class="btn btn-default" data-dismiss="modal">Close</button>
                                </div>
                            </div>
                        </div>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </div>
</div>
<dnn:dnncssinclude runat="server" filepath="~/Resources/Shared/components/TimePicker/Themes/jquery-ui.min.css" />

<script type="text/javascript">
    (function ($, Sys) {
        $(document).ready(function () {
            PageInit();
        });
        if (Sys && Sys.WebForms) {
            Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
                PageInit();
                if (document.querySelectorAll('.modal.show').length === 0) {
                    document.querySelectorAll('.modal-backdrop').forEach(function (b) { b.remove(); });
                    document.body.classList.remove('modal-open');
                    document.body.style.overflow = '';
                    document.body.style.paddingRight = '';
                }
            });
        }
    }(jQuery, window.Sys));

    function PageInit() {
        $(".form-check input").addClass("form-check-input");
        $(".form-check label").addClass("form-check-label");
        $(".date-picker").datepicker();
    }
    function ShowAlert(title, text) {
        $.dnnAlert({
            okText: 'OK',
            title: title,
            text: text
        });
    }
    function ValidateEmailBody(source, arguments) {

        arguments.IsValid = true;

    }
</script>
