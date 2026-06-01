<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UpdateCaseName.ascx.cs" Inherits="tjc.Modules.CourtCounsel.Views.UpdateCaseName" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>

<nav class="navbar navbar-expand-lg navbar-dark bg-dark mb-md rounded">
    <button class="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target="#navbarNav">
        <span class="navbar-toggler-icon"></span>
    </button>
    <div class="collapse navbar-collapse" id="navbarNav">
        <ul class="navbar-nav">
            <li class="nav-item"><a class="nav-link" href="<%=SearchUrl %>"><i class="fas fa-search"></i>&nbsp;Search</a></li>
            <li class="nav-item"><a class="nav-link" href="<%=DataEntryUrl %>"><i class="fas fa-pencil-alt"></i>&nbsp;Data Entry</a></li>
            <li class="nav-item"><a class="nav-link" href="<%=ReportsUrl %>"><i class="fas fa-chart-bar"></i>&nbsp;Reports</a></li>
            <li class="nav-item"><a class="nav-link" href="<%=DataSheetUrl %>"><i class="fas fa-table"></i>&nbsp;Data Sheet</a></li>
            <li class="nav-item" id="liAdmin" runat="server" visible="false"><a class="nav-link" href="<%=AdminUrl %>"><i class="fa fa-tools"></i>&nbsp;Admin</a></li>
        </ul>
    </div>
</nav>

<div class="container-fluid mt-3">
    <h4>Update Case Name</h4>

    <div class="row mb-3">
        <div class="col-md-4">
            <div class="input-group">
                <asp:TextBox ID="txtCaseNumber" runat="server" CssClass="form-control" placeholder="Enter Case Number..." />
                <asp:Button ID="cmdFind" runat="server" CssClass="btn btn-primary" Text="Find" OnClick="cmdFind_Click" />
            </div>
        </div>
    </div>

    <asp:Panel ID="pnlResults" runat="server" Visible="false">
        <div class="table-responsive mb-3">
            <table class="table table-striped table-bordered">
                <thead>
                    <tr>
                        <th>Case Number</th>
                        <th>Party Name</th>
                    </tr>
                </thead>
                <tbody>
                    <asp:Repeater ID="rptResults" runat="server">
                        <ItemTemplate>
                            <tr>
                                <td><%#Eval("CaseNumber") %></td>
                                <td><%#Eval("PartyName") %></td>
                            </tr>
                        </ItemTemplate>
                    </asp:Repeater>
                </tbody>
            </table>
        </div>

        <div class="row mb-3">
            <div class="col-md-4">
                <label class="fw-bold">New Case Name:</label>
                <asp:TextBox ID="txtNewCaseName" runat="server" CssClass="form-control" MaxLength="200" />
            </div>
            <div class="col-md-2 align-self-end">
                <asp:Button ID="cmdUpdate" runat="server" CssClass="btn btn-success" Text="Update" OnClick="cmdUpdate_Click" />
            </div>
        </div>
    </asp:Panel>

    <asp:Panel ID="pnlMessage" runat="server" Visible="false" CssClass="alert alert-success">
        <asp:Literal ID="ltMessage" runat="server" />
    </asp:Panel>
</div>

