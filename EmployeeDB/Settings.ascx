<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Settings.ascx.cs" Inherits="tjc.Modules.EmployeeDB.Settings" %>
<%@ Register TagName="label" TagPrefix="dnn" Src="~/controls/labelcontrol.ascx" %>

<h2 id="dnnSitePanel-BasicSettings" class="dnnFormSectionHead"><a href="" class="dnnSectionExpanded"><%=LocalizeString("BasicSettings")%></a></h2>
<fieldset>
    <div class="dnnFormItem">
        <dnn:label id="lblSupervisor" runat="server" controlname="drpSupervisor" suffix=":"></dnn:label>
        <asp:DropDownList ID="drpSupervisor" runat="server"></asp:DropDownList>
    </div>
    <div class="dnnFormItem">
        <dnn:label id="lblUsername" runat="server" controlname="txtUsername" suffix=":"></dnn:label>
        <asp:TextBox ID="txtUsername" runat="server" />
    </div>
    <div class="dnnFormItem">
        <dnn:label id="lblPassword" runat="server" controlname="txtPassword" suffix=":"></dnn:label>
        <asp:TextBox ID="txtPassword" TextMode="Password" runat="server" />
    </div>
    <div class="dnnFormItem">
        <dnn:label id="lblSubscriptionKey" runat="server" controlname="txtSubscriptionKey" suffix=":"></dnn:label>
        <asp:TextBox ID="txtSubscriptionKey" runat="server" />
    </div>
    <div class="dnnFormItem">
        <dnn:label id="lbServiceIdentifier" runat="server" controlname="txtServiceIdentifier" suffix=":"></dnn:label>
        <asp:TextBox ID="txtServiceIdentifier" runat="server" />
    </div>
    <div class="dnnFormItem">
        <dnn:label id="lblBaseUrl" runat="server" controlname="txtBaseUrl" suffix=":"></dnn:label>
        <asp:TextBox ID="txtBaseUrl" runat="server" />
    </div>
</fieldset>


