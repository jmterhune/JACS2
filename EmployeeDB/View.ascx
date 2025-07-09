<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="View.ascx.cs" Inherits="tjc.Modules.EmployeeDB.View" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<div class="tabs">
    <ul class="nav nav-tabs">
        <li class="nav-item active">
            <a class="nav-link" href="<%=EmployeeUrl%>"><i class="fas fa-id-badge"></i>&nbsp;Employees</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=ContactUrl%>"><i class="fas fa-user"></i>&nbsp;Contacts</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=DepartmentUrl%>"><i class="fas fa-sitemap"></i>&nbsp;Departments</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=JobGroupUrl%>"><i class="fas fa-users"></i>&nbsp;Job Groups</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=JobClassUrl%>"><i class="fas fa-user-tag"></i>&nbsp;Job Classes</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=RaceUrl%>"><i class="fas fa-users-cog"></i>&nbsp;Race</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=CountyUrl%>"><i class="fas fa-map-marked-alt"></i>&nbsp;Counties</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=LocationUrl%>"><i class="fas fa-building"></i>&nbsp;Locations</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=SwnLogUrl%>"><i class="fas fa-exclamation-circle"></i>&nbsp;SWN Interface Log</a>
        </li>
    </ul>
    <div class="tab-content">
        <div id="Employees" class="tab-pane active">
            <asp:UpdatePanel ID="pnlEmployees" runat="server" RenderMode="Block" OnUnload="pnlEmployees_Unload">
                <ContentTemplate>
                    <asp:UpdateProgress ID="upProgressEvent" runat="server">
                        <ProgressTemplate>
                            <div class="modal-progress">
                                <div class="center-progress">
                                    <img alt="" src="/images/loading.gif" />
                                </div>
                            </div>
                        </ProgressTemplate>
                    </asp:UpdateProgress>
                    <asp:Literal ID="ltMessage" runat="server" />
                    <asp:Repeater ID="rptEmployees" runat="server">
                        <HeaderTemplate>
                            <table id="tblEmployees" class="table table-striped">
                                <thead>
                                    <tr>
                                        <th>&nbsp;</th>
                                        <th>Name</th>
                                        <th>Email</th>
                                        <th>Phone</th>
                                        <th>Department</th>
                                        <th>Active</th>
                                        <th class="d-none"></th>
                                    </tr>
                                </thead>
                                <tbody>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <tr>
                                <td class="command-icon">
                                    <asp:HyperLink runat="server" ID="cmdEdit" NavigateUrl='<%#EditUrl("eid",DataBinder.Eval(Container.DataItem,"EmployeeId").ToString(),"Employee") %>' ToolTip="Edit Employee Record"><i class="fa fa-pencil-alt"></i></asp:HyperLink>
                                </td>
                                <td><%#DataBinder.Eval(Container.DataItem,"FullName") %></td>
                                <td><a href='mailto:<%#DataBinder.Eval(Container.DataItem,"Email") %>'><%#DataBinder.Eval(Container.DataItem,"Email") %></a></td>
                                <td><%#DataBinder.Eval(Container.DataItem,"Phones") %></td>
                                <td><%#DataBinder.Eval(Container.DataItem,"Department") %></td>
                                <td><%#DataBinder.Eval(Container.DataItem,"IsActive").ToString()=="True"?"<i class=\"fas fa-check-square\"></i>":"<i class=\"fas fa-square\"></i>" %></td>
                                <td class="d-none"><%#DataBinder.Eval(Container.DataItem,"DepartmentId") %></td>
                            </tr>
                        </ItemTemplate>
                        <FooterTemplate>
                            </tbody></table><hr />
                        </FooterTemplate>
                    </asp:Repeater>
                    <div class="btn-toolbar">
                        <div class="btn-group me-2" role="group" aria-label="Active / Inactive Employees">
                            <div id="swActive" class="input-group ms-md switch btn btn-default">
                                <div class="form-check form-switch">
                                    <asp:CheckBox ID="chkInactiveEmployees" Checked="true" AutoPostBack="true" OnCheckedChanged="chkInactiveEmployees_CheckedChanged" ClientIDMode="Static" runat="server" Text="Active Employees" />
                                </div>
                            </div>
                        </div>
                        <div class="btn-group me-2" role="group" aria-label="Action Toolbar">
                            <asp:HyperLink ID="lnkEeoReport" runat="server" CssClass="btn btn-default"><i class="fas fa-gear"></i> EEO Report Setup</asp:HyperLink>
                            <asp:HyperLink ID="lnkSwnList" runat="server" CssClass="btn btn-default"><i class="fas fa-mobile-screen-button"></i> SWN Export File</asp:HyperLink>
                            <asp:LinkButton ID="cmdAddContacts" runat="server" OnClick="cmdAddContacts_Click" CssClass="btn btn-default"><i class="fas fa-address-book"></i> Show Missing SWN Contacts</asp:LinkButton>
                            <asp:LinkButton ID="cmdSyncAll" OnClick="cmdSyncAll_Click" runat="server" CssClass="btn btn-default confirm"><i class="fas fa-rotate"></i> Sync All Contacts with SWN</asp:LinkButton>
                        </div>
                        <div class="btn-group" role="group" aria-label="Cancel Toolbar">
                            <asp:HyperLink ID="lnkCancel" Visible="false" runat="server" CssClass="btn btn-danger"><i class="fas fa-undo"></i> Cancel</asp:HyperLink>
                        </div>
                    </div>
                    <div class="mt-3 border rounded bg-light text-dark p-2">
                        <div class="row">
                            <div class="col-auto">
                                <asp:Label Text="Current Supervisor" ID="lblOldSupervisor" AssociatedControlID="drpOldSupervisor" runat="server" />
                                <asp:DropDownList runat="server" ID="drpOldSupervisor" CssClass="form-control" DataTextField="DataText" DataValueField="DataValue" AppendDataBoundItems="true">
                                    <asp:ListItem Text="<Select Old Supervisor>" Value="" />
                                </asp:DropDownList>
                            </div>
                            <div class="col-auto">
                                <asp:Label Text="New Supervisor" ID="lblNewSupervisor" AssociatedControlID="drpNewSupervisor" runat="server" />
                                <asp:DropDownList runat="server" ID="drpNewSupervisor" CssClass="form-control" DataTextField="DataText" DataValueField="DataValue" AppendDataBoundItems="true">
                                    <asp:ListItem Text="<Select New Supervisor>" Value="" />
                                </asp:DropDownList>
                            </div>
                            <div class="col-auto">
                                <asp:Button TabIndex="0" CssClass="btn btn-primary mt-3" Text="Swith Supervisors" ID="cmdSwithSupervisor" CausesValidation="false" ValidationGroup="switch" OnClick="cmdSwithSupervisor_Click" runat="server" />
                            </div>
                        </div>
                    </div>
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="cmdAddContacts" EventName="Click" />
                    <asp:AsyncPostBackTrigger ControlID="cmdSyncAll" EventName="Click" />
                    <asp:AsyncPostBackTrigger ControlID="chkInactiveEmployees" EventName="CheckedChanged" />
                </Triggers>
            </asp:UpdatePanel>
        </div>
    </div>
</div>
<dnn:dnncssinclude runat="server" filepath="/Resources/Libraries/DataTables/dataTables.bootstrap5.min.css" />
<dnn:dnnjsinclude runat="server" filepath="/Resources/Libraries/DataTables/dataTables.min.js" />
<dnn:dnnjsinclude runat="server" filepath="/Resources/Libraries/DataTables/dataTables.bootstrap5.min.js" />

<script type="text/javascript">
    (function ($, Sys) {

        $(document).ready(function () {
            Sys.Application.add_load(function (s, e) { PageInit(); });
            PageInit();
        });

    }(jQuery, window.Sys));

    function PageInit() {
        var table = $('#tblEmployees').DataTable({

            "order": [[1, "asc"]],
            "oLanguage": {
                "sSearch": "Filter by Text"
            },
            "aoColumns": [
                { "bSortable": false },
                { "bSortable": true },
                { "bSortable": false },
                { "bSortable": false },
                { "bSortable": true },
                { "bSortable": false },
                { "bSortable": false },],
            autoWidth: true,
        });

        $(".confirm").dnnConfirm({
            text: 'Are you sure you wish to Sync All Users? This process may take several minutes.',
            yesText: 'Yes',
            noText: 'No',
            title: 'Sync All Contacts with SWN?'
        });
        $("#tblEmployees_filter").prepend("<%=DepartmentFilterHtml%>");
        $("#tblEmployees_length").prepend(" <a class='btn btn-primary btn-sm me-2' href='<%=DetailUrl%>'><i class='fas fa-plus'></i> Add Employee</a>");
        table.draw();

        $('#drpfilter').change(function () {
            table.draw();
        });

    }
    $.fn.dataTable.ext.search.push(
        function (settings, data, dataIndex) {
            var selectedValue = $("#drpfilter option:selected").val();
            var msdsSearch = parseInt(selectedValue);
            var msdsValue = parseFloat(data[7]) || 0; // use data for the section column
            if (msdsSearch == -1) {
                return true;
            }

            if (msdsSearch == msdsValue) {
                return true;
            }

            return false;
        }
    );
</script>
