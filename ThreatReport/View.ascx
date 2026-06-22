<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="View.ascx.cs" Inherits="tjc.Modules.ThreatReport.View" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>

<asp:Repeater ID="rptIncidentList" runat="server">
    <HeaderTemplate>
        <table class="table table-striped" id="incidents">
            <thead><tr>
                <th>&nbsp;</th>
                <th>Incident ID</th>
                <th>Location</th>
                <th>Incident Type</th>
                <th>Description</th>
                <th>Reported By</th>
                <th>Date</th></tr>
            </thead>
            <tbody>
    </HeaderTemplate>

    <ItemTemplate>
        <tr>
            <td>
                <asp:HyperLink NavigateUrl='<%# EditUrl("id",DataBinder.Eval(Container.DataItem,"IncidentID").ToString(),"incident") %>' runat="server"><em class="fa fa-search"></em></asp:HyperLink></td>
            <td><%#DataBinder.Eval(Container.DataItem,"IncidentID").ToString() %> </td>
            <td><%#DataBinder.Eval(Container.DataItem,"Location").ToString() %> </td>

            <td><%#DataBinder.Eval(Container.DataItem,"NatureOfIncident").ToString() %> </td>
            <td><%#DataBinder.Eval(Container.DataItem,"Description").ToString() %> </td>

            <td><%#DataBinder.Eval(Container.DataItem,"ReportedBy").ToString() %> </td>
            <td><%#DataBinder.Eval(Container.DataItem,"DateOfIncident","{0:MM/dd/yyyy}") %> </td>

        </tr>
    </ItemTemplate>
    <FooterTemplate>
        </tbody></table>
    </FooterTemplate>
</asp:Repeater>

<dnn:dnncssInclude runat="server" FilePath="/Resources/Libraries/DataTables/dataTables.bootstrap5.min.css" />
<dnn:dnnjsInclude runat="server" FilePath="/Resources/Libraries/DataTables/dataTables.min.js" />
<dnn:dnnjsInclude runat="server" FilePath="/Resources/Libraries/DataTables/dataTables.bootstrap5.min.js" />
<script type="text/javascript">

    $(document).ready(function () {
        var incidents = $('#incidents').DataTable({
            "aoColumns": [
                { "bSortable": false },
                null,
                null,
                null,
                  { "bSortable": false },
                null,
                null,
            ],
            "bStateSave": true
        });

    });
</script>
