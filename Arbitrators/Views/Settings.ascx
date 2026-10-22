<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Settings.ascx.cs" Inherits="tjc.Modules.Arbitrators.Views.Settings" %>
<%@ Register TagName="label" TagPrefix="dnn" Src="~/controls/labelcontrol.ascx" %>

<h2 id="dnnSitePanel-BasicSettings" class="dnnFormSectionHead"><a href="" class="dnnSectionExpanded">Notification Settings</a></h2>
<fieldset>
    <div class="dnnFormItem">
        <dnn:Label ID="lblFromEmail" runat="server" Text="Notification From Address" />
        <asp:TextBox ID="txtFromEmail" runat="server" />
    </div>
    <div class="dnnFormItem">
        <dnn:Label ID="lblCcEmail" runat="server" Text="CC Address (optional)" />
        <asp:TextBox ID="txtCcEmail" runat="server" />
    </div>
</fieldset>
