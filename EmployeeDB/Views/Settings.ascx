<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Settings.ascx.cs" Inherits="tjc.Modules.EmployeeDB.Views.Settings" %>

<style>
    /* Normalize field heights to BS4 standard. The DNN/Porto skin sets
       box-sizing: content-box on inputs/selects which combines with padding
       and border to render ~52px tall fields; force border-box so the
       calc(1.5em + 0.75rem + 2px) ≈ 38px height applies as intended. */
    #EmployeeDbSettings .form-control,
    #EmployeeDbSettings .form-select,
    #EmployeeDbSettings select {
        box-sizing: border-box;
        height: calc(1.5em + 0.75rem + 2px);
        padding: .375rem .75rem;
        line-height: 1.5;
    }
</style>

<div id="EmployeeDbSettings" class="container-fluid">
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
            <fieldset class="mb-3">
                <legend class="h6">Save-Notification Email</legend>
                <div class="form-check mb-2">
                    <asp:CheckBox ID="chkNotifyOnSave" runat="server" CssClass="form-check-input" />
                    <label class="form-check-label" for="<%# chkNotifyOnSave.ClientID %>">Email a change summary when an employee is added or updated</label>
                </div>
                <div class="mb-2">
                    <label for="<%# txtNotifyFrom.ClientID %>" class="form-label">From address</label>
                    <asp:TextBox ID="txtNotifyFrom" runat="server" CssClass="form-control" />
                </div>
                <div class="mb-2">
                    <label for="<%# txtNotifyTo.ClientID %>" class="form-label">To address</label>
                    <asp:TextBox ID="txtNotifyTo" runat="server" CssClass="form-control" />
                    <div class="form-text">Multiple addresses can be comma-separated.</div>
                </div>
            </fieldset>
            <%-- The Crisis24 Person/HR feed is configured with web.config
                 appSettings (Crisis24FTP and friends) rather than module
                 settings, because the SFTP credentials belong to the server,
                 not to a module instance. See
                 Components\Helpers\Crisis24Uploader.cs. --%>
        </div>
    </div>
</div>
