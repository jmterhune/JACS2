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
        <div class="dnnFormMessage">Tab ID or URL of the page with the incident list. Used for the link in emails to @jud12.flcourts.org addresses, the confirmation page's link, and the detail page's return link.</div>
    </div>
    <div class="dnnFormItem">
        <asp:Label ID="lblExternalUrl" runat="server" AssociatedControlID="txtExternalUrl" Text="External Incident Link" />
        <asp:TextBox ID="txtExternalUrl" runat="server" />
        <div class="dnnFormMessage">Link to an incident on the jud12 (external) site, with the incident number replaced by {id}. Open an incident on that site, copy its address, and put {id} where its number is. Used for the link in emails to everyone who is not an @jud12.flcourts.org address. Blank = no link for them.</div>
    </div>
    <div class="dnnFormItem">
        <asp:Label ID="lblEditTab" runat="server" AssociatedControlID="txtEditTab" Text="Submit Incident Page" />
        <asp:TextBox ID="txtEditTab" runat="server" />
        <div class="dnnFormMessage">Tab ID or URL of the page with the submit-incident module. Shows the Add Threat button on the incident list.</div>
    </div>
</fieldset>
