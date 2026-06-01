<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Settings.ascx.cs" Inherits="tjc.Modules.ExpertWitness.Settings" %>
<%@ Register TagName="label" TagPrefix="dnn" Src="~/controls/labelcontrol.ascx" %>

<h2 id="dnnSitePanel-BasicSettings" class="dnnFormSectionHead"><a href="" class="dnnSectionExpanded"><%=LocalizeString("BasicSettings")%></a></h2>
<fieldset>

        <div class="dnnFormItem">
        <dnn:label id="lblAdminRole" controlname="drpAdminRole" runat="server" Suffix=":" />
        <asp:DropDownList ID="drpAdminRole" runat="server" CssClass="form-control">
            <asp:ListItem Text="< Select Admin Role >" Value=""></asp:ListItem>
        </asp:DropDownList>
    </div>

</fieldset>


