<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Settings.ascx.cs" Inherits="tjc.Modules.DigitalCourtReporting.Settings" %>
<%@ Register TagName="label" TagPrefix="dnn" Src="~/controls/labelcontrol.ascx" %>

<h2 id="dnnSitePanel-BasicSettings" class="dnnFormSectionHead"><a href="" class="dnnSectionExpanded"><%=LocalizeString("BasicSettings")%></a></h2>
<fieldset>
    <div class="dnnFormItem">
        <dnn:Label ID="lblDeSotoReportingEmail" runat="server" />
        <asp:TextBox ID="txtDeSotoReportingEmail" runat="server" />
    </div>
    <div class="dnnFormItem">
        <dnn:Label ID="lblManateeReportingEmail" runat="server" />
        <asp:TextBox ID="txtManateeReportingEmail" runat="server" />
    </div>
    <div class="dnnFormItem">
        <dnn:Label ID="lblSarasotaReportingEmail" runat="server" />
        <asp:TextBox ID="txtSarasotaReportingEmail" runat="server" />
    </div>
    <div class="dnnFormItem">
        <dnn:Label ID="lblAdminRole" runat="server" />
        <asp:DropDownList runat="server" ID="drpAdminRole">
        </asp:DropDownList>
    </div>
</fieldset>


