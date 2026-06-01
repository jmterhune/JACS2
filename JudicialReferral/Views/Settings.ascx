<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Settings.ascx.cs" Inherits="tjc.Modules.JudicialReferral.Views.Settings" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>

<h2 id="dnnSitePanel-BasicSettings" class="dnnFormSectionHead">
    <a href="" class="dnnSectionExpanded"><%=LocalizeString("BasicSettings")%></a>
</h2>
<fieldset>
    <div class="dnnFormItem">
        <dnn:Label ID="lblJudgeRole" runat="server" ControlName="drpJudgeRole" />
        <asp:DropDownList ID="drpJudgeRole" runat="server" CssClass="form-select" />
    </div>
    <div class="dnnFormItem">
        <dnn:Label ID="lblJaRole" runat="server" ControlName="drpJaRole" />
        <asp:DropDownList ID="drpJaRole" runat="server" CssClass="form-select" />
    </div>
    <div class="dnnFormItem">
        <dnn:Label ID="lblCounselRole" runat="server" ControlName="drpCounselRole" />
        <asp:DropDownList ID="drpCounselRole" runat="server" CssClass="form-select" />
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
