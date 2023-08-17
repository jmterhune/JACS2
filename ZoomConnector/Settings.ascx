<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Settings.ascx.cs" Inherits="tjc.Modules.ZoomConnector.Settings" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>

<div class="dnnForm dnnClear">
    <div class="dnnFormItem">
        <dnn:Label ID="lblManateeConnectorIP" runat="server" ControlName="txtManateeConnectorIP" Suffix=":"></dnn:Label>
        <asp:TextBox ID="txtManateeConnectorIP" runat="server"></asp:TextBox>
    </div>
    <div class="dnnFormItem">
        <dnn:Label ID="lblSarasotaConnectorIP" runat="server" ControlName="txtSarasotaConnectorIP" Suffix=":"></dnn:Label>
        <asp:TextBox ID="txtSarasotaConnectorIP" runat="server"></asp:TextBox>
    </div>
    <div class="dnnFormItem">
        <dnn:Label ID="lblDeSotoConnectorIP" runat="server" ControlName="txtDeSotoConnectorIP" Suffix=":"></dnn:Label>
        <asp:TextBox ID="txtDeSotoConnectorIP" runat="server"></asp:TextBox>
    </div>
    <div class="dnnFormItem">
        <dnn:Label ID="lblManatee" runat="server" ControlName="txtManatee" Suffix=":"></dnn:Label>
        <asp:TextBox ID="txtManatee" runat="server" TextMode="MultiLine" Width="250"></asp:TextBox>
    </div>

    <div class="dnnFormItem">
        <dnn:Label ID="lblSarasota" runat="server" ControlName="txtSarasota" Suffix=":"></dnn:Label>
        <asp:TextBox ID="txtSarasota" runat="server" TextMode="MultiLine" Width="250"></asp:TextBox>
    </div>
    <div class="dnnFormItem">
        <dnn:Label ID="lblDeSoto" runat="server" ControlName="txtDeSoto" Suffix=":"></dnn:Label>
        <asp:TextBox ID="txtDeSoto" runat="server" TextMode="MultiLine" Width="250"></asp:TextBox>
    </div>

</div>
