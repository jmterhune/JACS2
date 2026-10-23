<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Settings.ascx.cs" Inherits="tjc.Modules.ProcessServer.Settings" %>

<h2 id="dnnSitePanel-ProcessServerSettings" class="dnnFormSectionHead"><a href="" class="dnnSectionExpanded">Process Server Admin</a></h2>
<fieldset>
    <div class="dnnFormItem">
        <asp:Label ID="lblAdminRole" runat="server" AssociatedControlID="drpAdminRole" Text="Admin Role" />
        <asp:DropDownList ID="drpAdminRole" runat="server" CssClass="form-control" />
        <div class="dnnFormMessage">Role allowed to manage applications and payments. Leave unselected to use the default "Process Server Admin".</div>
    </div>
    <div class="dnnFormItem">
        <asp:Label ID="lblChiefJudgeRole" runat="server" AssociatedControlID="drpChiefJudgeRole" Text="Chief Judge Role" />
        <asp:DropDownList ID="drpChiefJudgeRole" runat="server" CssClass="form-control" />
        <div class="dnnFormMessage">Role for the Chief Judge approval step. Leave unselected to use the default "Chief Judge".</div>
    </div>
    <div class="dnnFormItem">
        <asp:Label ID="lblCourtCounselRole" runat="server" AssociatedControlID="drpCourtCounselRole" Text="Court Counsel Role" />
        <asp:DropDownList ID="drpCourtCounselRole" runat="server" CssClass="form-control" />
        <div class="dnnFormMessage">Role for the Court Counsel approval step. Leave unselected to use the default "Process Server Counsel".</div>
    </div>
    <div class="dnnFormItem">
        <asp:Label ID="lblNotificationEmail" runat="server" AssociatedControlID="txtNotificationEmail" Text="Notification Email" />
        <asp:TextBox ID="txtNotificationEmail" runat="server" MaxLength="250" />
        <div class="dnnFormMessage">From address for approve / reject emails. Blank defaults to "process@jud12.flcourts.org".</div>
    </div>
    <div class="dnnFormItem">
        <asp:Label ID="lblExternalUrl" runat="server" AssociatedControlID="txtExternalUrl" Text="Applicant Application Link" />
        <asp:TextBox ID="txtExternalUrl" runat="server" MaxLength="500" />
        <asp:CustomValidator ID="valExternalUrl" runat="server" ControlToValidate="txtExternalUrl" ValidateEmptyText="false"
            OnServerValidate="valExternalUrl_ServerValidate" CssClass="dnnFormMessage dnnFormError" Display="Dynamic"
            ErrorMessage="Enter a full address starting with http:// or https://" />
        <div class="dnnFormMessage">Base address of the Process Server Application page on the public (jud12) site, including the module id, e.g. https://jud12.flcourts.org/Services/Certified-Process-Servers/Process-Server-Application/mid/1398. The approve / reject emails add a "View your application" link to it, using the new or renewal page automatically based on the application. Blank = no link.</div>
    </div>
</fieldset>
