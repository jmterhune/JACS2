<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SwnList.ascx.cs" Inherits="tjc.Modules.EmployeeDB.Views.SwnList" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>

<div class="container-fluid">
    <h3><i class="fas fa-file-export"></i>&nbsp;Send Word Now Export</h3>

    <div class="card">
        <div class="card-body">
            <p class="card-text">
                This report produces the pipe-delimited contact file that can be uploaded into the
                Send Word Now (SWN) portal to bulk-import all active employees, their group memberships,
                phones, and email addresses.
            </p>
            <p class="card-text">
                Click <strong>Download</strong> below to generate the file.
            </p>

            <asp:LinkButton ID="cmdDownload" runat="server" CssClass="btn btn-primary" OnClick="cmdDownload_Click">
                <i class="fas fa-download"></i>&nbsp;Download
            </asp:LinkButton>
        </div>
    </div>

    <asp:Panel ID="pnlMessage" runat="server" Visible="false">
        <br />
        <div class="alert alert-info" role="alert">
            <asp:Literal ID="ltMessage" runat="server" />
        </div>
    </asp:Panel>
</div>
