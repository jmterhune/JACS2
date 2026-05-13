<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Settings.ascx.cs" Inherits="tjc.Modules.ThreatReport.Settings" %>
<%@ Register TagName="label" TagPrefix="dnn" Src="~/controls/labelcontrol.ascx" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<%-- SweetAlert2 + Noty for confirms / toast notifications --%>
<dnn:DnnCssInclude runat="server" FilePath="/Resources/Libraries/sweetalert/sweetalert2.min.css" />
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/sweetalert/sweetalert2.all.min.js" />
<dnn:DnnCssInclude runat="server" FilePath="/Resources/Libraries/Noty/noty.min.css" />
<dnn:DnnCssInclude runat="server" FilePath="/Resources/Libraries/Noty/bootstrap-v4.min.css" />
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/Noty/noty.min.js" />

<h2 id="dnnSitePanel-BasicSettings" class="dnnFormSectionHead"><a href="" class="dnnSectionExpanded"><%=LocalizeString("BasicSettings")%></a></h2>
<fieldset>
    <div class="dnnFormItem">
        <dnn:Label ID="lblAttachmentDirectory" runat="server" />

        <asp:TextBox ID="txtAttachmentDirectory" runat="server" />
    </div>
    <div class="dnnFormItem">
        <dnn:Label ID="lblTabId" runat="server" />

        <asp:TextBox ID="txtTabID" runat="server" />
    </div>
    <div class="dnnFormItem">
        <dnn:Label ID="lblViewTabID" runat="server" />

        <asp:TextBox ID="txtViewTabID" runat="server" />
    </div>
     <div class="dnnFormItem">
            <dnn:Label ID="lblRole" runat="server" ></dnn:Label>
            <asp:DropDownList ID="drpRole" runat="server"></asp:DropDownList>
        </div>
</fieldset>


