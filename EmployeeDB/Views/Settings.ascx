<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Settings.ascx.cs" Inherits="tjc.Modules.EmployeeDB.Views.Settings" %>

<div class="container-fluid">
    <div class="row">
        <div class="col-md-8">
            <div class="mb-3">
                <label for="<%# txtReportUrl.ClientID %>" class="form-label">Report Module Tab ID</label>
                <asp:TextBox ID="txtReportUrl" runat="server" CssClass="form-control" />
                <div class="form-text">Tab ID for the reports module.</div>
            </div>
            <div class="mb-3">
                <label for="<%# drpHrAdminRole.ClientID %>" class="form-label">HR Admin Role</label>
                <asp:DropDownList ID="drpHrAdminRole" runat="server" CssClass="form-select" />
                <div class="form-text">Portal role granted HR administrative access to the Employee Database module.</div>
            </div>
        </div>
    </div>
</div>
