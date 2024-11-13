<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Settings.ascx.cs" Inherits="tjc.Modules.JudgeVacation.Settings" %>
<%@ Register TagName="label" TagPrefix="dnn" Src="~/controls/labelcontrol.ascx" %>

<h2 id="dnnSitePanel-BasicSettings" class="dnnFormSectionHead"><a href="" class="dnnSectionExpanded"><%=LocalizeString("BasicSettings")%></a></h2>
<fieldset>
    <div class="dnnFormItem">
        <dnn:Label ID="lblEmailto" runat="server" />
        <asp:TextBox ID="txtEmailTo" runat="server" />
    </div>
    <div class="dnnFormItem">
        <dnn:label ID="lblReportingRole" runat="server" />
        <asp:DropDownList runat="server" ID="drpReportingRole">
        </asp:DropDownList>
    </div>
    <div class="dnnFormItem">
    <dnn:label ID="lblJudgeRole" runat="server" />
        <asp:DropDownList runat="server" ID="drpJudges">
        </asp:DropDownList>
</div>
</fieldset>


