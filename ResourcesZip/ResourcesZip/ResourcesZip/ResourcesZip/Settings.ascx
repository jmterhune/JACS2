<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Settings.ascx.cs" Inherits="tjc.Modules.jacs.Settings" %>
<%@ Register TagName="label" TagPrefix="dnn" Src="~/controls/labelcontrol.ascx" %>
<h2 id="dnnSitePanel-BasicSettings" class="dnnFormSectionHead"><a href="" class="dnnSectionExpanded"><%=LocalizeString("BasicSettings")%></a></h2>
<fieldset>
    <div class="dnnFormItem">
        <dnn:Label ID="lblAdminRole" runat="server" />
        <asp:DropDownList runat="server" ID="drpAdminRole">
        </asp:DropDownList>
    </div>
    <div class="dnnFormItem">
        <dnn:Label ID="lblJudgeRole" runat="server" />
        <asp:DropDownList runat="server" ID="drpJugeRole">
        </asp:DropDownList>
    </div>
    <div class="dnnFormItem">
        <dnn:label ID="lblJaRole" runat="server" />
        <asp:DropDownList runat="server" ID="drpJaRole">
        </asp:DropDownList>
    </div>
    <div class="dnnFormItem">
        <dnn:label ID="lblJacsUserRole" runat="server" />
        <asp:DropDownList runat="server" ID="drpJacsUserRole">
        </asp:DropDownList>
    </div>
    <div class="dnnFormItem">
        <dnn:Label ID="lblQuickRef" runat="server" />
        <asp:TextBox AutoCompleteType="Disabled" ID="txtQuickRefUrl" runat="server" />
    </div>
</fieldset>


