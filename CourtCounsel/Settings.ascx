<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Settings.ascx.cs" Inherits="tjc.Modules.CourtCounsel.Settings" %>

<%@ Register TagName="label" TagPrefix="dnn" Src="~/controls/labelcontrol.ascx" %>

<h2 id="dnnSitePanel-BasicSettings" class="dnnFormSectionHead"><a href="" class="dnnSectionExpanded"><%=LocalizeString("BasicSettings")%></a></h2>
<fieldset>
    <div class="dnnFormItem">
        <dnn:label id="lblAdminRole" runat="server" />

        <asp:TextBox ID="txtAdminRole" runat="server" />
    </div>
    <div class="dnnFormItem">
        <dnn:label id="lblDefaultReminderPeriod" runat="server" />

        <asp:TextBox ID="txtDefaultReminderPeriod" runat="server" />
    </div>
    <div class="dnnFormItem">
        <dnn:label id="lblSharePointSiteURL" runat="server" />

        <asp:TextBox ID="txtSharePointSiteURL" runat="server" />
    </div>
</fieldset>
<h2 id="dnnSitePanel-SharePointSettings" class="dnnFormSectionHead"><a href="" class="dnnSectionExpanded"><%=LocalizeString("SharePointSettings")%></a></h2>
<fieldset>
    <div class="dnnFormItem">
        <dnn:label id="lblId" runat="server" />

        <asp:TextBox ID="txtId" runat="server" />
    </div>

    <div class="dnnFormItem">
        <dnn:label id="lblDocumentLibraryName" runat="server" />

        <asp:TextBox ID="txtDocumentLibraryName" runat="server" />
    </div>
    <div class="dnnFormItem">
        <dnn:label id="lblDocumentLibraryURL" runat="server" />

        <asp:TextBox ID="txtDocumentLibraryURL" runat="server" />
    </div>
    <div class="dnnFormItem">
        <dnn:label id="lblDocumentDriveId" runat="server" />

        <asp:TextBox ID="txtDocumentDriveId" runat="server" />
    </div>
    <div class="dnnFormItem">
        <dnn:label id="lblOrderDriveId" runat="server" />

        <asp:TextBox ID="txtOrderDriveId" runat="server" />
    </div>
    <div class="dnnFormItem">
        <dnn:label id="lblOrderPath" runat="server" />

        <asp:TextBox ID="txtOrderPath" runat="server" />
    </div>
    <div class="dnnFormItem">
        <dnn:label id="lblGraphConfig" runat="server" />

        <asp:TextBox ID="txtGraphConfig" runat="server" />
    </div>
   
</fieldset>


