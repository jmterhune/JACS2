<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Settings.ascx.cs" Inherits="tjc.Modules.Transcriptionist.Settings" %>

<h2 id="dnnSitePanel-TranscriptionistSettings" class="dnnFormSectionHead"><a href="" class="dnnSectionExpanded">Transcriptionist Admin</a></h2>
<fieldset>
    <div class="dnnFormItem">
        <asp:Label ID="lblExternalUrl" runat="server" AssociatedControlID="txtExternalUrl" Text="Public Transcriptionist List" />
        <asp:TextBox ID="txtExternalUrl" runat="server" MaxLength="500" />
        <asp:CustomValidator ID="valExternalUrl" runat="server" ControlToValidate="txtExternalUrl" ValidateEmptyText="false"
            OnServerValidate="valExternalUrl_ServerValidate" CssClass="dnnFormMessage dnnFormError" Display="Dynamic"
            ErrorMessage="Enter a full address starting with http:// or https://" />
        <div class="dnnFormMessage">Address of the Transcriptionist list page on the public (jud12) site. Shown as a link on the dashboard and list. The site part of this address is also used to link the applicants' uploaded documents. Blank = no link.</div>
    </div>
</fieldset>
