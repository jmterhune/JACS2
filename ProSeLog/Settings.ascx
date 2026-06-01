<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Settings.ascx.cs" Inherits="tjc.Modules.ProSeLog.Settings" %>
<%@ Register TagName="label" TagPrefix="dnn" Src="~/controls/labelcontrol.ascx" %>

	<h2 id="dnnSitePanel-BasicSettings" class="dnnFormSectionHead"><a href="" class="dnnSectionExpanded"><%=LocalizeString("BasicSettings")%></a></h2>
	<fieldset>
           <div class="dnnFormItem">
        <dnn:label id="lblAdminRole" runat="server" controlname="drpAdminRole" suffix=":"></dnn:label>
        <asp:DropDownList ID="drpAdminRole" runat="server"></asp:DropDownList>
    </div>

    </fieldset>
