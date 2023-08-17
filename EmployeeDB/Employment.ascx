<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Employment.ascx.cs" Inherits="tjc.Modules.EmployeeDB.Employment" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<asp:Literal ID="ltMessage" Visible="false" runat="server"><div class="alert alert-warning"><i class="fa fa-warning"></i> {0}</div></asp:Literal>
<div class="tabs">
    <ul class="nav nav-tabs">
        <li class="nav-item">
            <a class="nav-link" href="<%=EmployeeUrl %>"><i class="fas fa-list"></i>&nbsp;Back to  List</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=DetailUrl%>"><i class="fas fa-user-edit"></i>&nbsp;Details</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=PhoneUrl%>"><i class="fas fa-phone"></i>&nbsp;Phone Numbers</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=DetailUrl%>?g=groups"><i class="fas fa-users"></i>&nbsp;Groups</a>
        </li>
        <li class="nav-item active">
            <a class="nav-link" href="<%=EmploymentUrl%>"><i class="fas fa-user-clock"></i>&nbsp;Employment History</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=EmergencyContactUrl%>"><i class="fas fa-address-book"></i>&nbsp;Emergency Contacts</a>
        </li>
    </ul>
    <div class="tab-content edit-form">
        <div id="positionHistory" class="tab-pane active">
            <asp:UpdatePanel ID="pnlPositionHistory" runat="server" RenderMode="Block" OnUnload="pnlPositionHistory_Unload">
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
                    <h2 class="mb-1">Position History</h2>
                    <asp:Repeater ID="rptPositionHistory" runat="server" OnItemCommand="rptPositionHistory_ItemCommand" OnItemCreated="rptPositionHistory_ItemCreated">
                        <HeaderTemplate>
                            <table id="tblPositionHistory" class="table table-striped">
                                <thead>
                                    <tr>
                                        <th>&nbsp;</th>
                                        <th>Position</th>
                                        <th>Type</th>
                                        <th>Internal?</th>
                                        <th>Start Date</th>
                                        <th>End Date</th>
                                        <th>&nbsp;</th>
                                    </tr>
                                </thead>
                                <tbody>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <tr>
                                <td class="command-icon">
                                    <asp:LinkButton ID="cmdEdit" runat="server" CommandName="edit" CausesValidation="false" CommandArgument='<%#DataBinder.Eval(Container.DataItem,"PositionId").ToString() %>'><i class="fa fa-pencil"></i></asp:LinkButton>
                                </td>
                                <td><%#DataBinder.Eval(Container.DataItem,"Description") %></td>
                                <td><span data-original-title='<%#DataBinder.Eval(Container.DataItem,"EntryName") %>' data-plugin-tooltip="tooltip"><%#DataBinder.Eval(Container.DataItem,"EntryType") %></span></td>
                                <td><%#DataBinder.Eval(Container.DataItem,"IsInternal").ToString()=="True"?"<i class=\"fas fa-check-square\"></i>":"<i class=\"fas fa-square\"></i>" %></td>
                                <td><%#DataBinder.Eval(Container.DataItem,"StartDate","{0: MM/dd/yyyy}") %></td>
                                <td><%#DataBinder.Eval(Container.DataItem,"EndDate","{0: MM/dd/yyyy}") %></td>
                                <td class="command-icon">
                                    <asp:LinkButton ID="cmdDelete" CssClass="confirm" runat="server" CausesValidation="false" CommandName="delete" CommandArgument='<%#DataBinder.Eval(Container.DataItem,"PositionId").ToString() %>'><i class="fa fa-trash"></i></asp:LinkButton></td>
                            </tr>
                        </ItemTemplate>
                        <FooterTemplate>
                            </tbody></table><hr />
                        </FooterTemplate>
                    </asp:Repeater>
                    <div class="modal fade" id="EditPositionHistoryModal" tabindex="-1" role="dialog" aria-labelledby="EditPositionHistoryModalLabel" aria-hidden="true">
                        <div class="modal-dialog">
                            <div class="modal-content">
                                <div class="modal-header">
                                    <h4 class="modal-title" id="EditPositionHistoryModalLabel">Add / Edit Position History</h4>
                                    <button type="button" class="close" data-bs-dismiss="modal" aria-hidden="true">&times;</button>
                                </div>
                                <div class="modal-body">
                                    <div class="form-group">
                                        <asp:Label runat="server" AssociatedControlID="drpType" Text="Type<em>*</em>" ToolTip="required" />
                                        <asp:DropDownList runat="server" ID="drpType" CssClass="form-control"  ClientIDMode="Static">
                                            <asp:ListItem Text="<Select Option>" Value="" />
                                            <asp:ListItem Text="Transfer" Value="T" />
                                            <asp:ListItem Text="Promotion" Value="P" />
                                            <asp:ListItem Text="Other" Value="O" />
                                        </asp:DropDownList>
                                        <asp:RequiredFieldValidator runat="server" ValidationGroup="Position" ControlToValidate="drpType"
                                            Display="Dynamic" SetFocusOnError="true" CssClass="label label-danger" ErrorMessage="Employment Type is Required" />
                                    </div>
                                    <div class="form-group">
                                        <asp:Label runat="server" AssociatedControlID="txtPosition" Text="Position<em>*</em>" ToolTip="required" />
                                        <asp:TextBox runat="server" CssClass="form-control" MaxLength="20" ID="txtPosition"  ClientIDMode="Static" />
                                        <asp:RequiredFieldValidator runat="server" ValidationGroup="Position" ControlToValidate="txtPosition"
                                            Display="Dynamic" SetFocusOnError="true" CssClass="label label-danger" ErrorMessage="Position is Required" />
                                    </div>
                                    <div class="form-group">
                                        <asp:Label runat="server" AssociatedControlID="drpExternal" Text="Internal/External<em>*</em>" ToolTip="required" />
                                        <asp:DropDownList runat="server" ID="drpExternal" CssClass="form-control"  ClientIDMode="Static">
                                            <asp:ListItem Text="<Select Option>" Value="" />
                                            <asp:ListItem Text="Internal" />
                                            <asp:ListItem Text="External" />
                                        </asp:DropDownList>
                                        <asp:RequiredFieldValidator runat="server" ValidationGroup="Position" ControlToValidate="drpExternal"
                                            Display="Dynamic" SetFocusOnError="true" CssClass="label label-danger" ErrorMessage="Please Select Internal or External" />
                                    </div>
                                    <div class="form-group">
                                        <asp:Label runat="server" AssociatedControlID="txtStartDate" Text="Start Date" />
                                        <asp:TextBox runat="server" CssClass="form-control datepicker" ID="txtStartDate"  ClientIDMode="Static"/>
                                        <asp:RequiredFieldValidator runat="server" ValidationGroup="Position" ControlToValidate="txtStartDate"
                                            Display="Dynamic" SetFocusOnError="true" CssClass="label label-danger" ErrorMessage="Start Date is Required" />
                                    </div>
                                    <div class="form-group">
                                        <asp:Label runat="server" AssociatedControlID="txtEndDate" Text="End Date" />
                                        <asp:TextBox runat="server" CssClass="form-control datepicker" ID="txtEndDate"  ClientIDMode="Static"/>
                                        <asp:RequiredFieldValidator runat="server" ValidationGroup="Position" ControlToValidate="txtEndDate"
                                            Display="Dynamic" SetFocusOnError="true" CssClass="label label-danger" ErrorMessage="End Date is Required" />
                                    </div>
                                </div>
                                <div class="modal-footer">
                                    <asp:HiddenField ID="hdPositionHistoryId" runat="server"  ClientIDMode="Static"/>
                                    <asp:Button OnClientClick="TogglePositionForm(false)" ValidationGroup="Position" CssClass="btn btn-primary" ID="cmdSave" runat="server" Text="Save" OnClick="cmdSave_Click" />
                                    <button type="button" class="btn btn-default" data-bs-dismiss="modal">Close</button>
                                </div>
                            </div>
                        </div>
                    </div>
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="cmdSave" EventName="Click" />
                </Triggers>

            </asp:UpdatePanel>

        </div>
        <div id="serviceHistory" class="tab-pane active">
            <asp:UpdatePanel ID="pnlServiceHistory" runat="server" RenderMode="Block" OnUnload="pnlServiceHistory_Unload">
                <ContentTemplate>
                    <asp:UpdateProgress ID="upProgressEvent2" runat="server">
                        <ProgressTemplate>
                            <div class="modal-progress">
                                <div class="center-progress">
                                    <img alt="" src="/images/loading.gif" />
                                </div>
                            </div>
                        </ProgressTemplate>
                    </asp:UpdateProgress>
                    <h2 class="mb-1">Service History</h2>
                    <asp:Repeater ID="rptServiceHistory" runat="server" OnItemCommand="rptServiceHistory_ItemCommand" OnItemCreated="rptServiceHistory_ItemCreated">
                        <HeaderTemplate>
                            <table id="tblServiceHistory" class="table table-striped">
                                <thead>
                                    <tr>
                                        <th>&nbsp;</th>
                                        <th>Company</th>
                                        <th>Hire Date</th>
                                        <th>Termination Date</th>
                                        <th>Last Pay Rate</th>
                                        <th>&nbsp;</th>
                                    </tr>
                                </thead>
                                <tbody>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <tr>
                                <td class="command-icon">
                                    <asp:LinkButton ID="cmdEdit" runat="server" CommandName="edit" CausesValidation="false" CommandArgument='<%#DataBinder.Eval(Container.DataItem,"ServiceId").ToString() %>'><i class="fa fa-pencil"></i></asp:LinkButton>
                                </td>
                                <td><%#DataBinder.Eval(Container.DataItem,"CompanyName") %></td>
                                <td><%#DataBinder.Eval(Container.DataItem,"HireDate","{0: MM/dd/yyyy}") %></td>
                                <td><%#DataBinder.Eval(Container.DataItem,"TerminationDate","{0: MM/dd/yyyy}") %></td>
                                <td><%#DataBinder.Eval(Container.DataItem,"LastPayRate","{0:C}") %></td>
                                <td class="command-icon">
                                    <asp:LinkButton ID="cmdDelete" CssClass="confirm" runat="server" CausesValidation="false" CommandName="delete" CommandArgument='<%#DataBinder.Eval(Container.DataItem,"ServiceId").ToString() %>'><i class="fa fa-trash"></i></asp:LinkButton></td>
                            </tr>
                        </ItemTemplate>
                        <FooterTemplate>
                            </tbody></table><hr />
                        </FooterTemplate>
                    </asp:Repeater>
                    <div class="modal fade" id="EditServiceHistoryModal" tabindex="-1" role="dialog" aria-labelledby="EditServiceHistoryModalLabel" aria-hidden="true">
                        <div class="modal-dialog">
                            <div class="modal-content">
                                <div class="modal-header">
                                    <h4 class="modal-title" id="EditServiceHistoryModalLabel">Add / Edit Service History</h4>
                                    <button type="button" class="close" data-bs-dismiss="modal" aria-hidden="true">&times;</button>
                                </div>
                                <div class="modal-body">
                                    <div class="form-group">
                                        <asp:Label runat="server" AssociatedControlID="txtCompany" Text="Company<em>*</em>" ToolTip="required" />
                                        <asp:TextBox runat="server" CssClass="form-control" MaxLength="200" ID="txtCompany" ClientIDMode="Static" />
                                        <asp:RequiredFieldValidator runat="server" ValidationGroup="Service" ControlToValidate="txtCompany"
                                            Display="Dynamic" SetFocusOnError="true" CssClass="label label-danger" ErrorMessage="Company Name is Required" />
                                    </div>
                                    <div class="form-group">
                                        <asp:Label runat="server" AssociatedControlID="txtHireDate" Text="Hire Date" />
                                        <asp:TextBox runat="server" CssClass="form-control datepicker" ID="txtHireDate" ClientIDMode="Static" />
                                    </div>
                                    <div class="form-group">
                                        <asp:Label runat="server" AssociatedControlID="txtTerminationDate" Text="Termination Date" />
                                        <asp:TextBox runat="server" CssClass="form-control datepicker" ID="txtTerminationDate"  ClientIDMode="Static"/>
                                    </div>
                                    <div class="form-group">
                                        <asp:Label runat="server" AssociatedControlID="txtLastPayRate" Text="Last Pay Rate" />
                                        <asp:TextBox runat="server" CssClass="form-control money" MaxLength="10" ID="txtLastPayRate"  ClientIDMode="Static"/>
                                    </div>

                                </div>
                                <div class="modal-footer">
                                    <asp:HiddenField ID="hdServiceHistoryId" runat="server"  ClientIDMode="Static"/>
                                    <asp:Button OnClientClick="ToggleServiceForm(false)" ValidationGroup="Service" CssClass="btn btn-primary" ID="cmdSaveService" runat="server" Text="Save" OnClick="cmdSaveService_Click" />
                                    <button type="button" class="btn btn-default" data-bs-dismiss="modal">Close</button>
                                </div>
                            </div>
                        </div>
                    </div>
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="cmdSaveService" EventName="Click" />
                </Triggers>

            </asp:UpdatePanel>
        </div>
    </div>
</div>
<dnn:dnncssinclude runat="server" filepath="~/Resources/Shared/components/TimePicker/Themes/jquery-ui.min.css" />
<dnn:dnnjsinclude runat="server" filepath="https://cdnjs.cloudflare.com/ajax/libs/jquery.mask/1.14.16/jquery.mask.js" />
<dnn:dnnjsinclude runat="server" filepath="https://cdn.datatables.net/1.13.4/js/jquery.dataTables.min.js" />
<dnn:dnnjsinclude runat="server" filepath="https://cdn.datatables.net/1.13.4/js/dataTables.bootstrap5.min.js" />
<dnn:dnncssinclude runat="server" filepath="https://cdn.datatables.net/1.13.4/css/dataTables.bootstrap5.min.css" />

<script type="text/javascript">
    /*globals jQuery, window, Sys */
    (function ($, Sys) {

        $(document).ready(function () {
            Sys.Application.add_load(function (s, e) { PageInit(); });
            PageInit();
        });

    }(jQuery, window.Sys));
    function PageInit() {
        $(".datepicker").datepicker();
        var table = $('#tblPositionHistory').DataTable({
            "order": [[1, "asc"]],
            "oLanguage": {
                "sSearch": "Filter by Text"
            }, "aoColumns": [
                { "bSortable": false },
                { "bSortable": true },
                { "bSortable": true },
                { "bSortable": true },
                { "bSortable": false },
                { "bSortable": false },
                { "bSortable": false },
            ]
        });
        $("#tblPositionHistory_length").prepend('<button onclick="return ClearForm(1)" class="btn btn-primary btn-sm me-2" data-bs-toggle="modal" data-bs-target="#EditPositionHistoryModal"><i class="fa fa-plus"></i>&nbsp;Add Postion History</button>');
        table.draw();

        var table2 = $('#tblServiceHistory').DataTable({
            "order": [[1, "asc"]],
            "oLanguage": {
                "sSearch": "Filter by Text"
            }, "aoColumns": [
                { "bSortable": false },
                { "bSortable": true },
                { "bSortable": true },
                { "bSortable": true },
                { "bSortable": false },
                { "bSortable": false },
            ]
        });
        $("#tblServiceHistory_length").prepend('<button onclick="return ClearForm(0)" class="btn btn-primary btn-sm me-2" data-bs-toggle="modal" data-bs-target="#EditServiceHistoryModal"><i class="fa fa-plus"></i>&nbsp;Add Service History</button>');
        table2.draw();

        $(".confirm").dnnConfirm({
            text: 'Are you sure you wish to delete this Item?',
            yesText: 'Yes',
            noText: 'No',
            title: 'Delete Item?'
        });
    }
    function TogglePositionForm(toggleValue) {
        if (toggleValue) {
            $('#EditPositionHistoryModal').modal('show');
        } else {
            if (typeof (Page_ClientValidate) == 'function') {
                Page_ClientValidate("Position");
            }

            if (Page_IsValid) {
                $('#EditPositionHistoryModal').modal('hide');
                $('body').removeClass('modal-open');
                $('.modal-backdrop').remove();
            }
        }
        return true;
    }
    function ToggleServiceForm(toggleValue) {
        if (toggleValue) {
            $('#EditServiceHistoryModal').modal('show');
        } else {
            if (typeof (Page_ClientValidate) == 'function') {
                Page_ClientValidate("Service");
            }

            if (Page_IsValid) {
                $('#EditServiceHistoryModal').modal('hide');
                $('body').removeClass('modal-open');
                $('.modal-backdrop').remove();
            }
        }
        return true;
    }
    function ClearForm(form) {
        if (form == 0) {
            $('#txtHireDate').val("");
            $('#txtTerminationDate').val("");
            $('#txtLastPayRate').val("");
            $('#txtCompany').val("");
            $('#hdServiceHistoryId').val("");
        } else {
            $('#drpType').val("");
            $('#txtPosition').val("");
            $('#drpExternal').val("");
            $('#txtStartDate').val("");
            $('#txtEndDate').val("");
            $('#hdPositionHistoryId').val("");
        }
        return false;
    }
</script>
