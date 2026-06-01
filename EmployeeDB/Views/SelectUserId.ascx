<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SelectUserId.ascx.cs" Inherits="tjc.Modules.EmployeeDB.Views.SelectUserId" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>

<div class="container-fluid">
    <h3><i class="fas fa-user-tag"></i>&nbsp;Associate DNN User with Employee</h3>

    <div class="card">
        <div class="card-body">
            <asp:Panel ID="pnlEmployee" runat="server">
                <p>
                    Select the DNN User that should be linked to the employee
                    <strong><asp:Literal ID="ltEmployeeName" runat="server" /></strong>.
                </p>
            </asp:Panel>

            <div class="row">
                <div class="col-md-6">
                    <label for="<%= drpUsers.ClientID %>" class="fw-bold">User</label>
                    <asp:DropDownList ID="drpUsers" runat="server" CssClass="form-control">
                        <asp:ListItem Text="&lt; Select User &gt;" Value="" />
                    </asp:DropDownList>
                </div>
            </div>

            <div class="row">
                <div class="col-12">
                    <asp:LinkButton ID="cmdSave" runat="server" CssClass="btn btn-primary" OnClick="cmdSave_Click">
                        <i class="fas fa-save"></i>&nbsp;Save
                    </asp:LinkButton>
                    <asp:LinkButton ID="cmdCancel" runat="server" CssClass="btn btn-secondary" OnClick="cmdCancel_Click">
                        <i class="fas fa-times"></i>&nbsp;Cancel
                    </asp:LinkButton>
                </div>
            </div>
        </div>
    </div>

    <asp:Panel ID="pnlError" runat="server" Visible="false">
        <br />
        <div class="alert alert-danger" role="alert">
            <asp:Literal ID="ltError" runat="server" />
        </div>
    </asp:Panel>
</div>
