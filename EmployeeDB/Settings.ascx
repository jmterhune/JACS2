<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Settings.ascx.cs" Inherits="tjc.Modules.EmployeeDB.Settings" %>  
<%@ Register TagName="label" TagPrefix="dnn" Src="~/controls/labelcontrol.ascx" %>

	<h2 id="dnnSitePanel-BasicSettings" class="dnnFormSectionHead"><a href="" class="dnnSectionExpanded"><%=LocalizeString("BasicSettings")%></a></h2>
	<fieldset>
        <div class="dnnFormItem">
            <dnn:label id="lblSupervisor" runat="server" controlname="drpSupervisor" suffix=":"></dnn:label>
            <asp:DropDownList ID="drpSupervisor" runat="server"></asp:DropDownList>
        </div>
    </fieldset>


