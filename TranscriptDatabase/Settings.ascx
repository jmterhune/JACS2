<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Settings.ascx.cs" Inherits="tjc.Modules.TranscriptDatabase.Settings" %>
<%@ Register TagName="label" TagPrefix="dnn" Src="~/controls/labelcontrol.ascx" %>

<h2 id="dnnSitePanel-BasicSettings" class="dnnFormSectionHead"><a href="" class="dnnSectionExpanded"><%=LocalizeString("BasicSettings")%></a></h2>
<fieldset>
    <div class="dnnFormItem">
        <dnn:Label ID="lblAdminRole" runat="server" />
        <asp:TextBox ID="txtAdminRole" runat="server" />
    </div>
     <div class="dnnFormItem">
     <dnn:Label ID="lblCourtReporterRole" runat="server" />
     <asp:DropDownList runat="server" ID="drpCourtReporterRole">
     </asp:DropDownList>
 </div>
    <div class="dnnFormItem">
        <dnn:Label ID="lblUploadFolderName" runat="server" />
        <asp:TextBox ID="txtUploadFolderName" runat="server" />
    </div>
</fieldset>


