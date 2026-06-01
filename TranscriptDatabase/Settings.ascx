<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Settings.ascx.cs" Inherits="tjc.Modules.TranscriptDatabase.Settings" %>
<%@ Register TagName="label" TagPrefix="dnn" Src="~/controls/labelcontrol.ascx" %>

<h2 id="dnnSitePanel-BasicSettings" class="dnnFormSectionHead"><a href="" class="dnnSectionExpanded"><%=LocalizeString("BasicSettings")%></a></h2>
<fieldset>
    <div class="dnnFormItem">
        <dnn:Label ID="lblAdminRole" runat="server" />
        <asp:DropDownList runat="server" ID="drpAdminRole">
        </asp:DropDownList>
    </div>
    <div class="dnnFormItem">
        <dnn:Label ID="lblCourtReporterRole" runat="server" />
        <asp:DropDownList runat="server" ID="drpCourtReporterRole">
        </asp:DropDownList>
    </div>
    <div class="dnnFormItem">
    <dnn:Label ID="lblCourtReporterIntakeRole" runat="server" />
    <asp:DropDownList runat="server" ID="drpCourtReporterIntake">
    </asp:DropDownList>
</div>
    <div class="dnnFormItem">
        <dnn:Label ID="lblUploadFormFolder" runat="server" />
        <asp:TextBox AutoCompleteType="Disabled" ID="txtUploadFormFolder" runat="server" />
    </div>
     <div class="dnnFormItem">
     <dnn:Label ID="lblUploadFileFolder" runat="server" />
     <asp:TextBox AutoCompleteType="Disabled" ID="txtUploadFileFolder" runat="server" />
 </div>
</fieldset>


