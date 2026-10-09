<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Settings.ascx.cs" Inherits="tjc.Modules.FileManager.Views.Settings" %>
<%@ Register TagName="label" TagPrefix="dnn" Src="~/controls/labelcontrol.ascx" %>

<h2 id="dnnSitePanel-BasicSettings" class="dnnFormSectionHead"><a href="" class="dnnSectionExpanded">File Share</a></h2>
<fieldset>
    <div class="dnnFormItem">
        <dnn:Label ID="lblShareKey" runat="server" Text="Share key" />
        <asp:TextBox ID="txtShareKey" runat="server" MaxLength="50" />
        <asp:RegularExpressionValidator ID="valShareKey" runat="server" ControlToValidate="txtShareKey"
            ValidationExpression="^[A-Za-z0-9_\-]+$" ErrorMessage="Use letters, numbers, - and _ only." CssClass="dnnFormMessage dnnFormError" Display="Dynamic" />
    </div>
    <div class="dnnFormItem">
        <asp:Label ID="lblStatus" runat="server" CssClass="dnnFormMessage" />
    </div>
</fieldset>
