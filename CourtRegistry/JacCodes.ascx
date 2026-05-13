<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="JacCodes.ascx.cs" Inherits="tjc.Modules.CourtRegistry.JacCodes" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<div class="tabs">
    <ul class="nav nav-tabs">
        <li class="nav-item">
            <a class="nav-link" href="<%=ApplicationListUrl%>">Applications</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=ManageYearsUrl%>">Manage Fiscal Years</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=ExceptionListUrl%>">Exceptions</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=BasicSettingsUrl%>">Basic Settings</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=AttorneyListUrl%>">Attorneys</a>
        </li>
        <li class="nav-item active">
            <a class="nav-link" href="#jacCodes" data-toggle="tab">JAC Codes</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=UpdateJacCodeUrl%>">Update JAC</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=LocationListUrl%>">Locations</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=CaseTypeListUrl%>">Case Types</a>
        </li>
    </ul>
    <div class="tab-content pb-0">
        <div id="jacCodes" class="tab-pane active">
            <div class="alert alert-info"><i class="fas fa-info-circle"></i>&nbsp;Use the Update JAC tab to add, modify, or remove codes.</div>
            <asp:Repeater ID="rptJacCodes" runat="server">
                <HeaderTemplate>
                    <table id="tblJacCodes" class="table table-striped">
                        <thead>
                            <tr>
                                <th>Code</th>
                                <th>Case Type</th>
                                <th>Category</th>
                                <th>Active</th>
                            </tr>
                        </thead>
                        <tbody>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr>
                        <td><%#Eval("JacCodeID") %></td>
                        <td><%#Eval("CaseTypeName") %></td>
                        <td><%#Eval("Category") %></td>
                        <td><%#Convert.ToBoolean(Eval("Active")) ? "<i class='fas fa-square-check'></i>" : "<i class='fas fa-square'></i>" %></td>
                    </tr>
                </ItemTemplate>
                <FooterTemplate>
                    </tbody></table>
                </FooterTemplate>
            </asp:Repeater>
        </div>
    </div>
</div>
<dnn:dnncssinclude runat="server" filepath="/Resources/Libraries/DataTables/dataTables.bootstrap5.min.css" />
<dnn:dnnjsinclude runat="server" filepath="/Resources/Libraries/DataTables/dataTables.min.js" />
<dnn:dnnjsinclude runat="server" filepath="/Resources/Libraries/DataTables/dataTables.bootstrap5.min.js" />
<script type="text/javascript">
    (function ($, Sys) {
        $(document).ready(function () {
            $('#tblJacCodes').DataTable({
                "order": [[0, "asc"]],
                pageLength: 25,
                lengthMenu: [[25, 50, 100, -1], [25, 50, 100, "All"]]
            });
        });
    }(jQuery, window.Sys));
</script>
