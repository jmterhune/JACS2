<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Settings.ascx.cs" Inherits="tjc.Modules.Purchasing.Settings" %>
<%@ Register TagName="label" TagPrefix="dnn" Src="~/controls/labelcontrol.ascx" %>

	<h2 id="dnnSitePanel-BasicSettings" class="dnnFormSectionHead"><a href="" class="dnnSectionExpanded"><%=LocalizeString("BasicSettings")%></a></h2>
	<fieldset>
    <div class="dnnFormItem">
        <dnn:Label ID="lblEmails" runat="server" />
        <asp:TextBox ID="txtEmails" runat="server" />
    </div>
    <div class="dnnFormItem">
        <dnn:Label ID="lblAdminRole" runat="server" />
        <asp:TextBox ID="txtAdminRole" runat="server" />
    </div>
    <div class="dnnFormItem">
        <dnn:Label ID="lblAttachmentFolderName" runat="server" />
        <asp:TextBox ID="txtAttachmentFolderName" runat="server" />
    </div>
</fieldset>


