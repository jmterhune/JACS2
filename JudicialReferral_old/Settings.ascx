<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Settings.ascx.cs" Inherits="tjc.Modules.JudicialReferral.Settings" %>
<%@ Register TagName="label" TagPrefix="dnn" Src="~/controls/labelcontrol.ascx" %>

<h2 id="dnnSitePanel-BasicSettings" class="dnnFormSectionHead"><a href="" class="dnnSectionExpanded"><%=LocalizeString("BasicSettings")%></a></h2>
<fieldset>
    <div class="dnnFormItem">
        <dnn:Label ID="lblJudgeRole" runat="server" />
        <asp:DropDownList ID="drpJudgeRole" runat="server" CssClass="form-control">
            <asp:ListItem Text="< Select Judge Role >" Value=""></asp:ListItem>
        </asp:DropDownList>
    </div>
    <div class="dnnFormItem">
        <dnn:Label ID="lblJaRole" runat="server" />
        <asp:DropDownList ID="drpJaRole" runat="server" CssClass="form-control">
            <asp:ListItem Text="< Select JA Role >" Value=""></asp:ListItem>
        </asp:DropDownList>
    </div>
    <div class="dnnFormItem">
        <dnn:Label ID="lblCounselRole" runat="server" />
        <asp:DropDownList ID="drpCourtCounsel" runat="server" CssClass="form-control">
            <asp:ListItem Text="< Select Court Counsel >" Value=""></asp:ListItem>
        </asp:DropDownList>
    </div>
    <div class="dnnFormItem">
        <dnn:Label ID="lblCounselEmail" runat="server" />
        <asp:TextBox ID="txtCounselEmail" runat="server" />
    </div>
     <div class="dnnFormItem">
     <dnn:Label ID="lblCourtCounselRefferalUrl" runat="server" />
     <asp:TextBox ID="txtCourtCounselRefferalUrl" runat="server" />
 </div>
    <div class="dnnFormItem">
        <dnn:Label ID="lblFolder" runat="server" />
        <asp:TextBox ID="txtFolderName" runat="server" />
    </div>
     <div class="dnnFormItem">
     <dnn:Label ID="lblModuleId" runat="server" />
     <asp:TextBox ID="txtModuleId" runat="server" />
 </div>
</fieldset>


