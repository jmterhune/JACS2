<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Settings.ascx.cs" Inherits="tjc.Modules.ThreatReport.Settings" %>

<h2 id="dnnSitePanel-ThreatSettings" class="dnnFormSectionHead"><a href="" class="dnnSectionExpanded">Incident Access and Notifications</a></h2>
<fieldset>
    <div class="dnnFormItem">
        <asp:Label ID="lblViewerRole" runat="server" AssociatedControlID="ddlViewerRole" Text="Incident Viewer Role" />
        <asp:DropDownList ID="ddlViewerRole" runat="server" />
        <div class="dnnFormMessage">Members of this role on this site can view ALL incidents. Everyone else who is signed in sees only their own submissions.</div>
    </div>
    <div class="dnnFormItem">
        <asp:Label ID="lblJud12Role" runat="server" AssociatedControlID="ddlJud12Role" Text="Jud12 Site Notification Role" />
        <asp:DropDownList ID="ddlJud12Role" runat="server" />
        <div class="dnnFormMessage">Members of this role on the jud12 (external) site are emailed new incidents submitted here. This site's own Incident Viewer role is not emailed, so keep the same people in both roles.</div>
    </div>
    <div class="dnnFormItem">
        <asp:Label ID="lblViewTab" runat="server" AssociatedControlID="txtViewTab" Text="Incident List Page" />
        <asp:TextBox ID="txtViewTab" runat="server" />
        <div class="dnnFormMessage">Tab ID or URL of the page with the incident list. Used for the links in emails and the confirmation page, and by the detail page's return link.</div>
    </div>
    <div class="dnnFormItem">
        <asp:Label ID="lblEditTab" runat="server" AssociatedControlID="txtEditTab" Text="Submit Incident Page" />
        <asp:TextBox ID="txtEditTab" runat="server" />
        <div class="dnnFormMessage">Tab ID or URL of the page with the submit-incident module. Shows the Add Threat button on the incident list.</div>
    </div>
</fieldset>
