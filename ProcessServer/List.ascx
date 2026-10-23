<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="List.ascx.cs" Inherits="tjc.Modules.ProcessServer.List" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<div class="tabs">
    <ul class="nav nav-tabs">
        <li class="active">
            <a class="nav-link active" href="#Apps" data-bs-toggle="tab">Applications</a>
        </li>
        <li>
            <a class="nav-link" href="#Payments" data-bs-toggle="tab">Credit Card Payments</a>
        </li>
    </ul>
    <div class="tab-content">
        <div id="Apps" class="tab-pane active">
            <asp:UpdatePanel ID="updateProcessServer" runat="server" RenderMode="Block" OnUnload="updateProcessServer_Unload">
                <ContentTemplate>
                    <div class="row g-3 align-items-center border border-dark rounded-3 bg-dark mb-3 mt-2">
                        <div class="col-auto mb-2 mt-2">
                            <asp:Label ID="lblYear" runat="server" AssociatedControlID="drpYear" CssClass="col-form-label fw-bold text-white" Text="Select Year" />
                        </div>
                        <div class="col-auto mb-2 mt-2">
                            <asp:DropDownList ID="drpYear" runat="server" CssClass="form-control" OnSelectedIndexChanged="drpYear_SelectedIndexChanged" AutoPostBack="true">
                            </asp:DropDownList>
                        </div>
                    </div>
                    <asp:Repeater ID="rptProcessServers" runat="server" OnItemCommand="rptProcessServers_ItemCommand" OnItemCreated="rptProcessServers_ItemCreated" OnItemDataBound="rptProcessServers_ItemDataBound">
                        <HeaderTemplate>
                            <table id="processServers" summary="Process Server List" class="table table-striped">
                                <thead>
                                    <tr>
                                        <th>&nbsp;</th>
                                        <th>&nbsp;</th>
                                        <th data-bs-toggle="tooltip" title="Process Server Card ID">#
                                        </th>
                                        <th>Year</th>
                                        <th>Name
                                        </th>
                                        <th>Email</th>
                                        <th>Username</th>
                                        <th>Modified</th>
                                        <th>Expiration</th>
                                        <th data-bs-toggle="tooltip" class="command-icon" title="Is Renewal Application">RN</th>
                                        <th data-bs-toggle="tooltip" class="command-icon" title="Payment Received">$</th>
                                        <th data-bs-toggle="tooltip" class="command-icon" title="Criminal History Completed">CH</th>
                                        <th data-bs-toggle="tooltip" class="command-icon" title="Status">Stat</th>
                                        <th>&nbsp;</th>
                                    </tr>
                                </thead>
                                <tbody>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <tr>
                                <td class="command-icon">
                                    <asp:HyperLink ID="lnkSearch" runat="server" CssClass="command"><i class="fas fa-search" aria-hidden="true"></i></asp:HyperLink></a>
                                </td>
                                <td class="command-icon">
                                    <asp:HyperLink ID="lnkEdit" runat="server" CssClass="command"><i class="fas fa-pen" aria-hidden="true"></i></asp:HyperLink></a>
                                </td>
                                <td>
                                    <%#DataBinder.Eval(Container.DataItem, "ProcessServerNumber")%>
                                </td>
                                <td>
                                    <%#DataBinder.Eval(Container.DataItem, "ApplicationYear")%>
                                </td>
                                <td class="nowrap">

                                    <%#DataBinder.Eval(Container.DataItem, "LastName")%>, <%#DataBinder.Eval(Container.DataItem, "FirstName")%>
                                </td>
                                <td>
                                    <a href='mailto:<%#DataBinder.Eval(Container.DataItem, "Email")%>'>
                                        <%#DataBinder.Eval(Container.DataItem, "Email")%></a>
                                </td>
                                <td><%#GetUserName(DataBinder.Eval(Container.DataItem, "UserID").ToString())%></td>
                                <td><%#DataBinder.Eval(Container.DataItem, "LastModifiedOnDate","{0:MM/dd/yyyy}")%></td>
                                <td>
                                    <asp:Literal ID="ltExpirationDate" runat="server"></asp:Literal>
                                </td>
                                <td class="command-icon">
                                    <asp:LinkButton ID="cmdRenewal" runat="server" OnClientClick="return confirm('Are You Sure?')" CommandName="SetRenewal" CommandArgument='<%#DataBinder.Eval(Container.DataItem,"ProcessServerID").ToString() %>'>
                                <i class="fa fa-square" aria-hidden="true">&nbsp;</i>
                                    </asp:LinkButton>
                                </td>
                                <td class="command-icon">
                                    <asp:LinkButton ID="cmdPaid" runat="server" OnClientClick="return confirm('Are You Sure?')" CommandName="SetPaid" CommandArgument='<%#DataBinder.Eval(Container.DataItem,"ProcessServerID").ToString() %>'>
                                <i class="fa fa-square" aria-hidden="true">&nbsp;</i>
                                    </asp:LinkButton>
                                </td>
                                <td class="command-icon">
                                    <asp:LinkButton ID="cmdCHCompleted" runat="server" CommandName="SetCHCompleted" CommandArgument='<%#DataBinder.Eval(Container.DataItem,"ProcessServerID").ToString() %>'>
                                <i class="fa fa-square" aria-hidden="true">&nbsp;</i>
                                    </asp:LinkButton>
                                </td>
                                <td>
                                    <asp:Literal ID="ltStatus" runat="server" /></td>
                                <td class="command-icon">
                                    <asp:LinkButton ID="cmdDelete" runat="server" Visible="false" CommandName="delete" OnClientClick="return confirm('Delete this Application and Associated Files?')" CommandArgument='<%#DataBinder.Eval(Container.DataItem,"ProcessServerID").ToString() %>'><i class="fa fa-trash" aria-hidden="true"></i></asp:LinkButton>
                                </td>
                            </tr>
                        </ItemTemplate>
                        <FooterTemplate>
                            </tbody></table>
                        </FooterTemplate>
                    </asp:Repeater>
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="drpYear" EventName="SelectedIndexChanged" />
                </Triggers>
            </asp:UpdatePanel>
        </div>
        <div id="Payments" class="tab-pane">
            <asp:UpdatePanel runat="server">
                <ContentTemplate>
                    <div class="alert alert-warning"><i class="fa fa-warning" aria-hidden="true"></i>Records in red represent cancelled or failed transactions.</div>
                    <div class="row g-3">
                        <div class="col-sm-2">
                            <asp:Label ID="lblStartDate" runat="server" AssociatedControlID="txtStartDate" CssClass="visually-hidden" Text="Begin Date" />
                            <asp:TextBox ID="txtStartDate" runat="server" MaxLength="20" CssClass="form-control datepicker" />
                        </div>
                        <div class="col-sm-2">
                            <asp:Label ID="lblEndDate" runat="server" AssociatedControlID="txtEndDate" CssClass="visually-hidden" Text="End Date" />
                            <asp:TextBox ID="txtEndDate" runat="server" MaxLength="20" CssClass="form-control datepicker" />
                        </div>
                        <div class="col-sm-2">
                            <asp:Button ID="cmdSubmit" runat="server" OnClick="cmdSubmit_Click" CssClass="btn btn-primary mb-2" Text="Filter by Date Range" />
                        </div>
                    </div>
                    <asp:Repeater runat="server" ID="rptTransactions" EnableViewState="false" OnItemDataBound="rptTransactions_ItemDataBound">
                        <HeaderTemplate>
                            <table id="transactions" summary="List of Transactions" class="table table-striped">
                                <thead>
                                    <tr>
                                        <th>Customer</th>
                                        <th>Application Id</th>
                                        <th>OrderId</th>
                                        <th>Email</th>
                                        <th>Amount</th>
                                        <th>Date</th>
                                    </tr>

                                </thead>
                                <tbody>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <tr id="tRow" runat="server">
                                <td><%#DataBinder.Eval(Container.DataItem, "CustomerName")%></td>
                                <td><%#DataBinder.Eval(Container.DataItem, "ProcessServerID")%></td>
                                <td><%#DataBinder.Eval(Container.DataItem, "OrderId")%></td>
                                <td><%#DataBinder.Eval(Container.DataItem, "EmailAddress")%></td>
                                <td><%#DataBinder.Eval(Container.DataItem, "PaymentAmount","{0:C}")%></td>
                                <td><%#DataBinder.Eval(Container.DataItem, "PaymentDate","{0:MM/dd/yyyy HH:mm}")%></td>

                            </tr>
                        </ItemTemplate>
                        <FooterTemplate>
                            </tbody>
        </table>
                        </FooterTemplate>
                    </asp:Repeater>

                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="cmdSubmit" EventName="Click" />
                </Triggers>

            </asp:UpdatePanel>

        </div>
    </div>
</div>
<dnn:dnncssinclude runat="server" filepath="/Resources/Libraries/DataTables/dataTables.bootstrap5.min.css" />
<dnn:dnnjsinclude runat="server" filepath="/Resources/Libraries/DataTables/dataTables.min.js" />
<dnn:dnnjsinclude runat="server" filepath="/Resources/Libraries/DataTables/dataTables.bootstrap5.min.js" />
<dnn:dnnjsinclude runat="server" filepath="/Resources/Libraries/jQuery/moment.min.js" />
<dnn:dnnjsinclude runat="server" filepath="/Resources/Libraries/DataTables/datetime-moment.js" />


<script type="text/javascript">

    /*globals jQuery, window, Sys */
    (function ($, Sys) {

        $(document).ready(function () {
            bindDataTable();
            Sys.WebForms.PageRequestManager.getInstance().add_endRequest(bindDataTable); // bind data table on every UpdatePanel refresh
        });

    }(jQuery, window.Sys));

    function bindDataTable() {
        // create datatables
        $('[data-bs-toggle="tooltip"]').tooltip()
        $.fn.dataTable.moment('MM/DD/YYYY');
        var table = $('#processServers').DataTable({
            "aoColumns": [
                { "bSortable": false },
                { "bSortable": false },
                { "bSortable": false },
                null,
                null,
                null,
                { "bSortable": false },
                null,
                null,
                { "bSortable": false },
                { "bSortable": false },
                { "bSortable": false },
                { "bSortable": false },
                { "bSortable": false },

            ],
            "oLanguage": {
                "sSearch": "Filter by Text"
            }, columnDefs: [
                {
                    targets: [7, 8],
                    render: function (data, type, row) {
                        if (type === 'sort') {
                            return data.replace(/.*? /, '');
                        }
                        return data;
                    }
                }
            ],
            "lengthMenu": [[25, 50, 100, -1], [25, 50, 100, "All"]],
            "bStateSave": true
        });
        var table2 = $('#transactions').DataTable({
            "order": [[0, "desc"]],
            "oLanguage": {

                "sSearch": "Filter by Text"

            }
        });
    }

</script>
