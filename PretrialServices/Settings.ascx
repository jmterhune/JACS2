<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Settings.ascx.cs" Inherits="tjc.Modules.PretrialServices.Settings" %>
<%@ Register TagName="label" TagPrefix="dnn" Src="~/controls/labelcontrol.ascx" %>

<h2 id="dnnSitePanel-BasicSettings" class="dnnFormSectionHead"><a href="" class="dnnSectionExpanded"><%=LocalizeString("BasicSettings")%></a></h2>
<fieldset>
    <div class="dnnFormItem">
        <dnn:Label ID="lblCounty" runat="server" />
        <asp:DropDownList runat="server" ID="drpCounty" AppendDataBoundItems="true">
            <asp:ListItem Text="Select County" Value="" />
        </asp:DropDownList>
    </div>
    <div class="dnnFormItem">
        <dnn:Label ID="lblReportUrl" runat="server" />
        <asp:TextBox runat="server" ID="txtReportUrl" />  
    </div>
</fieldset>


