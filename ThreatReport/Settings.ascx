<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Settings.ascx.cs" Inherits="tjc.Modules.ThreatReport.Settings" %>

<h2 id="dnnSitePanel-ThreatNotifications" class="dnnFormSectionHead"><a href="" class="dnnSectionExpanded">Incident Notifications</a></h2>
<fieldset>
    <div class="dnnFormItem">
        <asp:Label ID="lblViewerRole" runat="server" AssociatedControlID="txtViewerRole" Text="Intranet Notification Role" />
        <asp:TextBox ID="txtViewerRole" runat="server" />
        <div class="dnnFormMessage">Role on this (intranet) site whose members receive incident notifications. Leave blank for Incident Viewer.</div>
    </div>
    <div class="dnnFormItem">
        <asp:Label ID="lblJud12Role" runat="server" AssociatedControlID="txtJud12Role" Text="Jud12 Site Notification Role" />
        <asp:TextBox ID="txtJud12Role" runat="server" />
        <div class="dnnFormMessage">Role on the jud12 (external) site whose members also receive incident notifications. Leave blank for Incident Reporter.</div>
    </div>
</fieldset>
