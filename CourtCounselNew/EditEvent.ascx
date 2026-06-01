<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EditEvent.ascx.cs" Inherits="tjc.Modules.CourtCounsel.EditEvent" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<div class="dnnForm dnnEditBasicSettings" id="dnnEditBasicSettings">
    <div class="dnnFormExpandContent dnnRight "><a href=""><%=LocalizeString("ExpandAll")%></a></div>

    <div class="form-group">
        <asp:Label runat="server" AssociatedControlID="txtStartDate" Text="Start Date<em>*</em>" ToolTip="required" />
        <asp:TextBox runat="server" CssClass="form-control" TextMode="date" ID="txtStartDate" ClientIDMode="Static" ValidationGroup="event" />
        <asp:RequiredFieldValidator runat="server" ControlToValidate="txtStartDate" ValidationGroup="event"
            Display="Dynamic" SetFocusOnError="true" CssClass="label label-danger" ErrorMessage="Start Date is Required" />
    </div>
    <div class="form-group">
        <asp:Label runat="server" AssociatedControlID="txtSubject" Text="Subject<em>*</em>" />
        <asp:TextBox runat="server" CssClass="form-control" ID="txtSubject" ClientIDMode="Static" ValidationGroup="event" />
        <asp:RequiredFieldValidator runat="server" ControlToValidate="txtSubject" ValidationGroup="event"
            Display="Dynamic" SetFocusOnError="true" CssClass="label label-danger" ErrorMessage="Subject is Required" />

    </div>
    <div class="form-group">
        <asp:Label runat="server" AssociatedControlID="txtBody" Text="Body" />
        <asp:TextBox runat="server" CssClass="form-control" ID="txtBody" TextMode="MultiLine" Rows="4" ClientIDMode="Static" />

    </div>
    <div class="form-group">
        <asp:Label runat="server" AssociatedControlID="txtReminderDays" Text="Reminder in days before event<em>*</em>" />
        <asp:TextBox runat="server" CssClass="form-control" ID="txtReminderDays" TextMode="Number" ClientIDMode="Static" />
        <asp:RequiredFieldValidator runat="server" ControlToValidate="txtReminderDays" ValidationGroup="event"
            Display="Dynamic" SetFocusOnError="true" CssClass="label label-danger" ErrorMessage="Reminder Days is Required" />
    </div>
    <asp:HiddenField runat="server" ID="hdExternalId" ClientIDMode="Static"></asp:HiddenField>
</div>

<asp:LinkButton ID="btnSubmit" runat="server"
    OnClick="btnSubmit_Click" resourcekey="btnSubmit" CssClass="btn btn-primary pull-left" />
<asp:LinkButton ID="btnCancel" runat="server"
    OnClick="btnCancel_Click" resourcekey="btnCancel" CssClass="btn btn-default" />
<dnn:dnncssinclude runat="server" filepath="~/Resources/Shared/components/TimePicker/Themes/jquery-ui.min.css" />

<script type="text/javascript">
    /*globals jQuery, window, Sys */
    (function ($, Sys) {
        function dnnEditBasicSettings() {
            $(".datepicker").datepicker();
            $('#dnnEditBasicSettings').dnnPanels();
            $('#dnnEditBasicSettings .dnnFormExpandContent a').dnnExpandAll({ expandText: '<%=Localization.GetString("ExpandAll", LocalResourceFile)%>', collapseText: '<%=Localization.GetString("CollapseAll", LocalResourceFile)%>', targetArea: '#dnnEditBasicSettings' });
        }

        $(document).ready(function () {
            dnnEditBasicSettings();
            Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
                dnnEditBasicSettings();
            });
        });

    }(jQuery, window.Sys));
</script>
