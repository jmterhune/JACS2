<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Settings.ascx.cs" Inherits="tjc.Modules.JCID.Settings" %>

<h2 id="dnnSitePanel-JcidAdminSettings" class="dnnFormSectionHead"><a href="" class="dnnSectionExpanded">JCID Admin</a></h2>
<fieldset>
    <div class="dnnFormItem">
        <asp:Label ID="lblExternalUrl" runat="server" AssociatedControlID="txtExternalUrl" Text="Public JCID Application Page" />
        <asp:TextBox ID="txtExternalUrl" runat="server" MaxLength="500" />
        <asp:CustomValidator ID="valExternalUrl" runat="server" ControlToValidate="txtExternalUrl" ValidateEmptyText="false"
            OnServerValidate="valExternalUrl_ServerValidate" CssClass="dnnFormMessage dnnFormError" Display="Dynamic"
            ErrorMessage="Enter a full address starting with http:// or https://" />
        <div class="dnnFormMessage">Address of the JCID application page on the public (jud12) site. The site part of this address links the applicant photos; it also builds the copy-able payment link and the reject-correction link emailed to applicants. Blank = those links are hidden / omitted.</div>
    </div>
</fieldset>
