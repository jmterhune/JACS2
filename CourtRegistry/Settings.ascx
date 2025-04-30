<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Settings.ascx.cs" Inherits="tjc.Modules.CourtRegistry.Settings" %>

<%@ Register TagName="label" TagPrefix="dnn" Src="~/controls/labelcontrol.ascx" %>

	<h2 id="dnnSitePanel-BasicSettings" class="dnnFormSectionHead"><a href="" class="dnnSectionExpanded"><%=LocalizeString("BasicSettings")%></a></h2>
	<fieldset>
        <div class="dnnFormItem">
            <dnn:Label ID="lblUrl" runat="server" /> 
            <asp:TextBox ID="txtUrl" runat="server" />
        </div>
        <div class="dnnFormItem">
            <dnn:label ID="lblApprover" runat="server" />
            <asp:TextBox ID="txtApprover" runat="server" />
        </div>
    </fieldset>


