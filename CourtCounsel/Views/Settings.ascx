<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Settings.ascx.cs" Inherits="tjc.Modules.CourtCounsel.Views.Settings" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<%@ Register TagPrefix="dnn" TagName="label" Src="~/controls/LabelControl.ascx" %>

<div class="dnnFormItem">
    <dnn:label ID="lblAdminRole" runat="server" ControlName="drpAdminRole" Text="Admin Role:" HelpText="Select the role that will have administrative access to this module." />
    <asp:DropDownList ID="drpAdminRole" runat="server" CssClass="form-control" />
</div>
<div class="dnnFormItem">
    <dnn:label ID="lblTemplate" runat="server" ControlName="txtTemplate" Text="Template:" HelpText="Enter the default template text for new entries." />
    <asp:TextBox ID="txtTemplate" runat="server" CssClass="form-control" TextMode="MultiLine" MaxLength="2000" Rows="10" />
</div>
