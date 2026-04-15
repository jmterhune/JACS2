<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Reports.ascx.cs" Inherits="tjc.Modules.CourtCounsel.Views.Reports" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>

<nav class="navbar navbar-expand-lg navbar-dark bg-dark mb-md">
    <button class="navbar-toggler" type="button" data-toggle="collapse" data-target="#navbarNav">
        <span class="navbar-toggler-icon"></span>
    </button>
    <div class="collapse navbar-collapse" id="navbarNav">
        <ul class="navbar-nav">
            <li class="nav-item"><a class="nav-link" href="<%=SearchUrl %>"><i class="fas fa-search"></i>&nbsp;Search</a></li>
            <li class="nav-item"><a class="nav-link" href="<%=DataEntryUrl %>"><i class="fas fa-pencil-alt"></i>&nbsp;Data Entry</a></li>
            <li class="nav-item"><a class="nav-link active" href="<%=ReportsUrl %>"><i class="fas fa-chart-bar"></i>&nbsp;Reports</a></li>
            <li class="nav-item"><a class="nav-link" href="<%=DataSheetUrl %>"><i class="fas fa-table"></i>&nbsp;Data Sheet</a></li>
            <li class="nav-item" id="liAdmin" runat="server" visible="false"><a class="nav-link" href="<%=AdminUrl %>"><i class="fa fa-tools"></i>&nbsp;Admin</a></li>
        </ul>
    </div>
</nav>

<div class="container-fluid mt-3">
    <div class="card">
        <div class="card-header"><strong>Report Filters</strong></div>
        <div class="card-body">
            <div class="row mb-3">
                <div class="col-md-3">
                    <label class="font-weight-bold">Start Date:</label>
                    <asp:TextBox ID="txtStartDate" runat="server" CssClass="form-control" TextMode="Date" />
                </div>
                <div class="col-md-3">
                    <label class="font-weight-bold">End Date:</label>
                    <asp:TextBox ID="txtEndDate" runat="server" CssClass="form-control" TextMode="Date" />
                </div>
            </div>
            <div class="row mb-3">
                <div class="col-md-12">
                    <label class="font-weight-bold">Status:</label>
                    <asp:RadioButtonList ID="rblStatus" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" CssClass="form-check-inline">
                        <asp:ListItem Text="Active" Value="Active" Selected="True" />
                        <asp:ListItem Text="Inactive" Value="Inactive" />
                        <asp:ListItem Text="Not Completed" Value="NotCompleted" />
                        <asp:ListItem Text="Completed" Value="Completed" />
                        <asp:ListItem Text="All" Value="" />
                    </asp:RadioButtonList>
                </div>
            </div>
            <div class="row mb-3">
                <div class="col-md-3">
                    <label class="font-weight-bold">Extended Status:</label>
                    <asp:DropDownList ID="drpStatus" runat="server" CssClass="form-control">
                        <asp:ListItem Text="-- All --" Value="" />
                        <asp:ListItem Text="New" Value="New" />
                        <asp:ListItem Text="In Progress" Value="In Progress" />
                        <asp:ListItem Text="Under Review" Value="Under Review" />
                        <asp:ListItem Text="On Hold" Value="On Hold" />
                        <asp:ListItem Text="Completed" Value="Completed" />
                        <asp:ListItem Text="Dismissed" Value="Dismissed" />
                        <asp:ListItem Text="Withdrawn" Value="Withdrawn" />
                    </asp:DropDownList>
                </div>
                <div class="col-md-3">
                    <label class="font-weight-bold">County:</label>
                    <asp:DropDownList ID="drpCounty" runat="server" CssClass="form-control" />
                </div>
                <div class="col-md-3">
                    <label class="font-weight-bold">Requestor:</label>
                    <asp:DropDownList ID="drpRequestor" runat="server" CssClass="form-control" />
                </div>
            </div>
            <div class="row mb-3">
                <div class="col-md-12">
                    <label class="font-weight-bold">Attorney:</label>
                    <div class="form-check form-check-inline">
                        <input type="checkbox" class="form-check-input" id="chkSelectAllAttorneys" onclick="toggleAllAttorneys(this);" />
                        <label class="form-check-label" for="chkSelectAllAttorneys"><strong>Select All</strong></label>
                    </div>
                    <asp:CheckBoxList ID="cblAttorneys" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" CssClass="form-check-inline" />
                </div>
            </div>
            <div class="row mb-3">
                <div class="col-md-12">
                    <div class="form-check">
                        <asp:CheckBox ID="chkShowDetail" runat="server" CssClass="form-check-input" />
                        <label class="form-check-label font-weight-bold">Show Detail</label>
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="col-md-12">
                    <asp:Button ID="cmdSubmit" runat="server" CssClass="btn btn-primary" Text="Submit" OnClick="cmdSubmit_Click" />
                    <asp:Button ID="cmdReset" runat="server" CssClass="btn btn-secondary ml-1" Text="Reset" OnClick="cmdReset_Click" />
                </div>
            </div>
        </div>
    </div>

    <div class="mt-4">
        <asp:Literal ID="ltResults" runat="server" />
    </div>
</div>

<script type="text/javascript">
    function toggleAllAttorneys(source) {
        var checkboxes = document.querySelectorAll('[id*="cblAttorneys"] input[type="checkbox"]');
        for (var i = 0; i < checkboxes.length; i++) {
            checkboxes[i].checked = source.checked;
        }
    }
</script>

<dnn:DnnCssInclude runat="server" FilePath="~/DesktopModules/CourtCounsel/Styles/module.css" />
