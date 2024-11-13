<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Settings.ascx.cs" Inherits="tjc.Modules.HearingLog.Settings" %>
<%@ Register TagName="label" TagPrefix="dnn" Src="~/controls/labelcontrol.ascx" %>
<fieldset>
    <div class="dnnFormItem">
        <dnn:Label ID="lblChiefJudgeRole" runat="server" />
        <asp:DropDownList runat="server" ID="drpRoles">
        </asp:DropDownList>
    </div>
    <div class="dnnFormItem">
        <dnn:Label ID="lblJudgeRole" runat="server" />
        <asp:DropDownList runat="server" ID="drpJudgeRole">
        </asp:DropDownList>
    </div>
    <div class="dnnFormItem">
        <dnn:Label ID="lblJaRole" runat="server" />
        <asp:DropDownList runat="server" ID="drpJaRole">
        </asp:DropDownList>
    </div>
     <div class="dnnFormItem">
     <dnn:Label ID="lblUrl" runat="server" />
    <asp:TextBox ID="txtUrl" runat="server" />
 </div>
</fieldset>
