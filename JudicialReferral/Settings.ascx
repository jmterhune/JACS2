<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Settings.ascx.cs" Inherits="tjc.Modules.JudicialReferral.Settings" %>
<%@ Register TagName="label" TagPrefix="dnn" Src="~/controls/labelcontrol.ascx" %>

	<h2 id="dnnSitePanel-BasicSettings" class="dnnFormSectionHead"><a href="" class="dnnSectionExpanded"><%=LocalizeString("BasicSettings")%></a></h2>
	<fieldset>
        <div class="dnnFormItem">
            <dnn:Label ID="lblJudgeRole" runat="server" /> 
 
            <asp:TextBox ID="txtJudgeRole" runat="server" />
        </div>
         <div class="dnnFormItem">
            <dnn:Label ID="lblJaRole" runat="server" /> 
            <asp:TextBox ID="txtJaRole" runat="server" />
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="lblCounselRole" runat="server" /> 
            <asp:TextBox ID="txtCounselRole" runat="server" />
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="lblCounselEmail" runat="server" /> 
 
            <asp:TextBox ID="txtCounselEmail" runat="server" />
        </div>
          <div class="dnnFormItem">
            <dnn:Label ID="lblFolder" runat="server" /> 
 
            <asp:TextBox ID="txtFolderName" runat="server" />
        </div>
    </fieldset>


