<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Settings.ascx.cs" Inherits="tjc.Modules.ProcessServer.Settings" %>

<h2 id="dnnSitePanel-ProcessServerSettings" class="dnnFormSectionHead"><a href="" class="dnnSectionExpanded">Process Server Admin</a></h2>
<fieldset>
    <div class="dnnFormItem">
        <asp:Label ID="lblAdminRole" runat="server" AssociatedControlID="txtAdminRole" Text="Admin Role" />
        <asp:TextBox ID="txtAdminRole" runat="server" MaxLength="250" />
        <div class="dnnFormMessage">Role allowed to manage applications and payments. Blank defaults to "Process Server Admin".</div>
    </div>
    <div class="dnnFormItem">
        <asp:Label ID="lblChiefJudgeRole" runat="server" AssociatedControlID="txtChiefJudgeRole" Text="Chief Judge Role" />
        <asp:TextBox ID="txtChiefJudgeRole" runat="server" MaxLength="250" />
        <div class="dnnFormMessage">Role for the Chief Judge approval step. Blank defaults to "Chief Judge".</div>
    </div>
    <div class="dnnFormItem">
        <asp:Label ID="lblCourtCounselRole" runat="server" AssociatedControlID="txtCourtCounselRole" Text="Court Counsel Role" />
        <asp:TextBox ID="txtCourtCounselRole" runat="server" MaxLength="250" />
        <div class="dnnFormMessage">Role for the Court Counsel approval step. Blank defaults to "Process Server Counsel".</div>
    </div>
    <div class="dnnFormItem">
        <asp:Label ID="lblNotificationEmail" runat="server" AssociatedControlID="txtNotificationEmail" Text="Notification Email" />
        <asp:TextBox ID="txtNotificationEmail" runat="server" MaxLength="250" />
        <div class="dnnFormMessage">From address for approve / reject emails. Blank defaults to "process@jud12.flcourts.org".</div>
    </div>
    <div class="dnnFormItem">
        <asp:Label ID="lblEnrollmentStart" runat="server" AssociatedControlID="txtEnrollmentStart" Text="Enrollment Period Start" />
        <asp:TextBox ID="txtEnrollmentStart" TextMode="Date" runat="server" />
        <div class="dnnFormMessage">Only the month and day are used; the window repeats each year. Determines the application year.</div>
    </div>
    <div class="dnnFormItem">
        <asp:Label ID="lblEnrollmentEnd" runat="server" AssociatedControlID="txtEnrollmentEnd" Text="Enrollment Period End" />
        <asp:TextBox ID="txtEnrollmentEnd" TextMode="Date" runat="server" />
    </div>
</fieldset>
