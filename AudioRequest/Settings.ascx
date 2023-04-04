<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Settings.ascx.cs" Inherits="tjc.Modules.AudioRequest.Settings" %> 
<%@ Register TagName="label" TagPrefix="dnn" Src="~/controls/labelcontrol.ascx" %>

	<h2 id="dnnSitePanel-BasicSettings" class="dnnFormSectionHead"><a href="" class="dnnSectionExpanded"><%=LocalizeString("BasicSettings")%></a></h2>
	<fieldset>
        <div class="dnnFormItem">
            <dnn:Label ID="lblEmail" runat="server" /> 
 
            <asp:TextBox ID="txtEmail" runat="server" />
        </div>
        <div class="dnnFormItem">
            <dnn:label ID="lblEmail2" runat="server" />
            <asp:TextBox ID="txtEmail2" runat="server" />
        </div>
		 <div class="dnnFormItem">
            <dnn:label ID="lblSA" runat="server" />
            <asp:Checkbox ID="chkSA" runat="server" /><em>Audio Request Only</em>
        </div>
        <div class="dnnFormItem">
            <dnn:label ID="lblReportingPage" runat="server" />
            <asp:TextBox ID="txtReportingPage" runat="server" />
        </div>

    </fieldset>



