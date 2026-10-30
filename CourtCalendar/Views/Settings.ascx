<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Settings.ascx.cs" Inherits="tjc.Modules.CourtCalendar.Settings" %>

<div class="cc-admin dnnForm">
    <div class="dnnFormItem mb-2">
        <label class="form-label" for="<%= ddlJudgeRole.ClientID %>">Judge role</label>
        <asp:DropDownList ID="ddlJudgeRole" runat="server" CssClass="form-select" />
        <small class="form-text text-muted">Users in this role appear in the judge picker on the Manage Judges screen.</small>
    </div>
    <div class="dnnFormItem mb-2">
        <label class="form-label" for="<%= ddlJaRole.ClientID %>">Judicial assistant role</label>
        <asp:DropDownList ID="ddlJaRole" runat="server" CssClass="form-select" />
        <small class="form-text text-muted">Users in this role appear in the JA picker on the Manage Judges screen.</small>
    </div>
</div>
