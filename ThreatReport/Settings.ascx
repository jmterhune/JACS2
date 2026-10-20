<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Settings.ascx.cs" Inherits="tjc.Modules.ThreatReport.Settings" %>

<h2 id="dnnSitePanel-ThreatSettings" class="dnnFormSectionHead"><a href="" class="dnnSectionExpanded">Incident Access and Notifications</a></h2>
<fieldset>
    <div class="dnnFormItem">
        <asp:Label ID="lblViewerRole" runat="server" AssociatedControlID="txtViewerRole" Text="Incident Viewer Role" />
        <asp:TextBox ID="txtViewerRole" runat="server" />
        <div class="dnnFormMessage">Role on this site whose members can view ALL incidents, submit incidents, and are emailed new ones. Everyone else who is signed in sees only their own submissions. Blank = Incident Viewer.</div>
    </div>
    <div class="dnnFormItem">
        <asp:Label ID="lblSubmitterRoles" runat="server" AssociatedControlID="txtSubmitterRoles" Text="Submitter Roles" />
        <asp:TextBox ID="txtSubmitterRoles" runat="server" />
        <div class="dnnFormMessage">Roles on this site (separate several with commas) whose members can submit incidents. They see only their own submissions. Blank = Judges,Judicial Assistant.</div>
    </div>
    <div class="dnnFormItem">
        <asp:Label ID="lblJud12Role" runat="server" AssociatedControlID="txtJud12Role" Text="Jud12 Site Notification Role" />
        <asp:TextBox ID="txtJud12Role" runat="server" />
        <div class="dnnFormMessage">Role on the jud12 (external) site whose members are also emailed new incidents. Blank = Incident Reporter.</div>
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
