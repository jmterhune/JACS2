<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EEOSetup.ascx.cs" Inherits="tjc.Modules.EmployeeDB.Views.EEOSetup" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>

<div class="container-fluid">
    <h3><i class="fas fa-chart-bar"></i>&nbsp;EEO Setup</h3>

    <div class="tabs">
        <ul class="nav nav-tabs" id="eeoTabs" role="tablist">
            <li class="nav-item active">
                <a class="nav-link active" href="#pane-eeo-list" data-bs-toggle="tab" data-toggle="tab">EEO List</a>
            </li>
            <li class="nav-item">
                <a class="nav-link" href="#pane-eeo-review" data-bs-toggle="tab" data-toggle="tab">Review This Year's EEO Data</a>
            </li>
        </ul>

        <div class="tab-content">
            <div class="tab-pane active" id="pane-eeo-list">
                <asp:Repeater ID="rptEeoList" runat="server">
                    <HeaderTemplate>
                        <table id="table-eeo-list" class="table table-striped table-hover" style="width:100%">
                            <thead>
                                <tr>
                                    <th>Job Group</th>
                                    <th>Year</th>
                                    <th>Pop Male</th>
                                    <th>Pop Female</th>
                                    <th>Pop White</th>
                                    <th>Pop Black</th>
                                    <th>Pop Hispanic</th>
                                    <th>Pop Asian</th>
                                    <th>Pop Indian</th>
                                    <th>Pop Other</th>
                                    <th>Hire Male</th>
                                    <th>Hire Female</th>
                                    <th>Promo Male</th>
                                    <th>Promo Female</th>
                                    <th>Transfer Male</th>
                                    <th>Transfer Female</th>
                                    <th>Term Male</th>
                                    <th>Term Female</th>
                                </tr>
                            </thead>
                            <tbody>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <tr>
                            <td><%# Eval("JobGroupName") %></td>
                            <td><%# Eval("Year") %></td>
                            <td><%# Eval("PopulationMale") %></td>
                            <td><%# Eval("PopulationFemale") %></td>
                            <td><%# Eval("PopulationWhite") %></td>
                            <td><%# Eval("PopulationBlack") %></td>
                            <td><%# Eval("PopulationHispanic") %></td>
                            <td><%# Eval("PopulationAsian") %></td>
                            <td><%# Eval("PopulationIndian") %></td>
                            <td><%# Eval("PopulationOther") %></td>
                            <td><%# Eval("HireMale") %></td>
                            <td><%# Eval("HireFemale") %></td>
                            <td><%# Eval("PromoMale") %></td>
                            <td><%# Eval("PromoFemale") %></td>
                            <td><%# Eval("TransferMale") %></td>
                            <td><%# Eval("TransferFemale") %></td>
                            <td><%# Eval("TermMale") %></td>
                            <td><%# Eval("TermFemale") %></td>
                        </tr>
                    </ItemTemplate>
                    <FooterTemplate>
                            </tbody>
                        </table>
                    </FooterTemplate>
                </asp:Repeater>
            </div>

            <div class="tab-pane" id="pane-eeo-review">
                <div class="card">
                    <div class="card-header">
                        <strong><i class="fas fa-calendar"></i>&nbsp;Reporting Window</strong>
                    </div>
                    <div class="card-body">
                        <div class="row">
                            <div class="col-md-3">
                                <label for="<%= dpStartDate.ClientID %>" class="fw-bold">Start Date</label>
                                <asp:TextBox ID="dpStartDate" runat="server" CssClass="form-control" TextMode="SingleLine" type="date" />
                            </div>
                            <div class="col-md-3">
                                <label for="<%= dpEndDate.ClientID %>" class="fw-bold">End Date</label>
                                <asp:TextBox ID="dpEndDate" runat="server" CssClass="form-control" TextMode="SingleLine" type="date" />
                            </div>
                            <div class="col-md-3">
                                <label for="<%= txtYear.ClientID %>" class="fw-bold">Year</label>
                                <asp:TextBox ID="txtYear" runat="server" CssClass="form-control" />
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-12">
                                <asp:LinkButton ID="btnStart" runat="server" CssClass="btn btn-primary" OnClick="btnStart_Click">
                                    <i class="fas fa-calculator"></i>&nbsp;Check EEO Values
                                </asp:LinkButton>
                                <asp:LinkButton ID="btnAccept" runat="server" CssClass="btn btn-success" OnClick="btnAccept_Click" Visible="false">
                                    <i class="fas fa-save"></i>&nbsp;Publish Results
                                </asp:LinkButton>
                            </div>
                        </div>
                    </div>
                </div>

                <asp:Panel ID="pnlPreview" runat="server" Visible="false">
                    <br />
                    <h4>Preview</h4>
                    <asp:Repeater ID="rptPreview" runat="server">
                        <HeaderTemplate>
                            <table id="table-eeo-preview" class="table table-striped" style="width:100%">
                                <thead>
                                    <tr>
                                        <th>Job Group</th>
                                        <th>Pop Male</th>
                                        <th>Pop Female</th>
                                        <th>Pop White</th>
                                        <th>Pop Black</th>
                                        <th>Pop Hispanic</th>
                                        <th>Pop Asian</th>
                                        <th>Pop Indian</th>
                                        <th>Pop Other</th>
                                        <th>Hire M/F</th>
                                        <th>Promo M/F</th>
                                        <th>Term M/F</th>
                                    </tr>
                                </thead>
                                <tbody>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <tr>
                                <td><%# Eval("JobGroupName") %></td>
                                <td><%# Eval("PopulationMale") %></td>
                                <td><%# Eval("PopulationFemale") %></td>
                                <td><%# Eval("PopulationWhite") %></td>
                                <td><%# Eval("PopulationBlack") %></td>
                                <td><%# Eval("PopulationHispanic") %></td>
                                <td><%# Eval("PopulationAsian") %></td>
                                <td><%# Eval("PopulationIndian") %></td>
                                <td><%# Eval("PopulationOther") %></td>
                                <td><%# Eval("HireMale") %>/<%# Eval("HireFemale") %></td>
                                <td><%# Eval("PromoMale") %>/<%# Eval("PromoFemale") %></td>
                                <td><%# Eval("TermMale") %>/<%# Eval("TermFemale") %></td>
                            </tr>
                        </ItemTemplate>
                        <FooterTemplate>
                                </tbody>
                            </table>
                        </FooterTemplate>
                    </asp:Repeater>
                </asp:Panel>
            </div>
        </div>
    </div>
</div>

<script type="text/javascript">
    function InitEeoTables() {
        jQuery(document).ready(function ($) {
            if ($.fn.DataTable && !$.fn.DataTable.isDataTable('#table-eeo-list')) {
                $('#table-eeo-list').DataTable({
                    "order": [[1, "desc"], [0, "asc"]],
                    "pageLength": 25,
                    "scrollX": true
                });
            }
            if ($.fn.DataTable && $('#table-eeo-preview').length && !$.fn.DataTable.isDataTable('#table-eeo-preview')) {
                $('#table-eeo-preview').DataTable({
                    "order": [[0, "asc"]],
                    "pageLength": 25,
                    "scrollX": true
                });
            }
        });
    }
    InitEeoTables();

    if (typeof Sys !== 'undefined') {
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
            if (jQuery.fn.DataTable) {
                if (jQuery.fn.DataTable.isDataTable('#table-eeo-list')) jQuery('#table-eeo-list').DataTable().destroy();
                if (jQuery.fn.DataTable.isDataTable('#table-eeo-preview')) jQuery('#table-eeo-preview').DataTable().destroy();
            }
            InitEeoTables();
        });
    }
</script>

<dnn:DnnCssInclude runat="server" FilePath="/Resources/Libraries/DataTables/dataTables.bootstrap5.min.css" />
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/DataTables/dataTables.min.js" />
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/DataTables/dataTables.bootstrap5.min.js" />
