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

<h2 id="dnnSitePanel-ExternalApp" class="dnnFormSectionHead"><a href="" class="dnnSectionExpanded">External Application</a></h2>
<fieldset>
    <div class="dnnFormItem">
        <dnn:Label ID="lblExternalUrl" runat="server" Text="External App URL (optional)" />
        <asp:TextBox ID="txtExternalUrl" runat="server" Width="400px" placeholder="https://www.jud12.flcourts.org/..." />
        <asp:CustomValidator ID="valExternalUrl" runat="server" ControlToValidate="txtExternalUrl" Display="Dynamic" CssClass="dnnFormMessage dnnFormError"
            ErrorMessage="Enter a full http:// or https:// URL." OnServerValidate="valExternalUrl_ServerValidate" />
        <div class="dnnFormMessage">Address of the Arbitrators list page on the public (jud12) site. Shown as a link on the dashboard, list and review pages. Blank = no link.</div>
    </div>
</fieldset>
