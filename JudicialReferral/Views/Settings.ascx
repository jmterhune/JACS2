<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Settings.ascx.cs" Inherits="tjc.Modules.JudicialReferral.Views.Settings" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>

<h2 id="dnnSitePanel-BasicSettings" class="dnnFormSectionHead">
    <a href="" class="dnnSectionExpanded"><%=LocalizeString("BasicSettings")%></a>
</h2>
<fieldset>
    <div class="dnnFormItem">
        <dnn:Label ID="lblJudgeRole" runat="server" ControlName="txtJudgeRole" />
        <asp:TextBox ID="txtJudgeRole" runat="server" />
    </div>
    <div class="dnnFormItem">
        <dnn:Label ID="lblJaRole" runat="server" ControlName="txtJaRole" />
        <asp:TextBox ID="txtJaRole" runat="server" />
    </div>
    <div class="dnnFormItem">
        <dnn:Label ID="lblCounselRole" runat="server" ControlName="txtCounselRole" />
        <asp:TextBox ID="txtCounselRole" runat="server" />
    </div>
    <div class="dnnFormItem">
        <dnn:Label ID="lblCounselEmail" runat="server" ControlName="txtCounselEmail" />
        <asp:TextBox ID="txtCounselEmail" runat="server" />
    </div>
    <div class="dnnFormItem">
        <dnn:Label ID="lblFolder" runat="server" ControlName="txtFolderName" />
        <asp:TextBox ID="txtFolderName" runat="server" />
    </div>
</fieldset>
