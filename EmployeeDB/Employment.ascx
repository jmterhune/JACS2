<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Employment.ascx.cs" Inherits="tjc.Modules.EmployeeDB.Employment" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<div class="tabs">
    <ul class="nav nav-tabs">
        <li class="nav-item">
            <a class="nav-link" href="<%=DetailUrl%>"><i class="fas fa-user-edit"></i>&nbsp;Details</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=PhoneUrl%>" data-toggle="tab"><i class="fas fa-phone"></i>&nbsp;Phone Numbers</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=DetailUrl%>?g=groups"><i class="fas fa-users"></i>&nbsp;Groups</a>
        </li>
        <li class="nav-item active">
            <a class="nav-link" href="<%=EmploymentUrl%>"><i class="fas fa-user-clock"></i>&nbsp;Employment History</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=ContactUrl%>"><i class="fas fa-address-book"></i>&nbsp;Emergency Contacts</a>
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

                    <asp:Repeater ID="rptPositionHistory" runat="server" OnItemCommand="rptPositionHistory_ItemCommand" OnItemCreated="rptPositionHistory_ItemCreated">
                        <HeaderTemplate>
                            <table id="tblPositionHistory" class="table table-striped">
                                <thead>
                                    <tr>
                                        <th>&nbsp;</th>
                                        <th>Type</th>
                                        <th>Position</th>
                                        <th>Internal?</th>
                                        <th>Start Date</th>
                                        <th>End Date</th>
                                        <th>Rating</th>
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
                                <td><%#DataBinder.Eval(Container.DataItem,"EntryType") %></td>
                                <td><%#DataBinder.Eval(Container.DataItem,"Description") %></td>
                                <td><%#DataBinder.Eval(Container.DataItem,"IsInternal").ToString()=="True"?"<i class=\"fas fa-check-square\"></i>":"<i class=\"fas fa-square\"></i>" %></td>
                                <td><%#DataBinder.Eval(Container.DataItem,"StartDate","{0: dd/MM/yyyy}") %></td>
                                <td><%#DataBinder.Eval(Container.DataItem,"EndDate","{0: dd/MM/yyyy}") %></td>
                                <td><%#DataBinder.Eval(Container.DataItem,"Rating").ToString() %></td>

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
                                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                                </div>
                                <div class="modal-body">
                                    <div class="form-group">
                                        <asp:Label runat="server" AssociatedControlID="drpType" Text="Type<em>*</em>" ToolTip="required" />
                                        <asp:DropDownList runat="server" ID="drpType" CssClass="form-control">
                                            <asp:ListItem Text="Transfer" />
                                            <asp:ListItem Text="Promotion" />
                                            <asp:ListItem Text="Other" />
                                        </asp:DropDownList>
                                        <asp:RequiredFieldValidator runat="server" ControlToValidate="drpType"
                                            Display="Dynamic" SetFocusOnError="true" CssClass="label label-danger" ErrorMessage="Type is Required" />
                                    </div>
                                    <div class="form-group">
                                        <asp:Label runat="server" AssociatedControlID="txtPosition" Text="Position<em>*</em>" ToolTip="required" />
                                        <asp:TextBox runat="server" CssClass="form-control" MaxLength="20" ID="txtPosition" />
                                        <asp:RequiredFieldValidator runat="server" ControlToValidate="txtPosition"
                                            Display="Dynamic" SetFocusOnError="true" CssClass="label label-danger" ErrorMessage="Position is Required" />
                                    </div>
                                    <div class="form-group">
                                        <asp:Label runat="server" AssociatedControlID="txtStartDate" Text="Start Date" />
                                        <asp:TextBox runat="server" CssClass="form-control datapicker" ID="txtStartDate" />
                                        <asp:RequiredFieldValidator runat="server" ControlToValidate="txtStartDate"
                                            Display="Dynamic" SetFocusOnError="true" CssClass="label label-danger" ErrorMessage="Start Date is Required" />
                                    </div>
                                    <div class="form-group">
                                        <asp:Label runat="server" AssociatedControlID="txtEndDate" Text="End Date" />
                                        <asp:TextBox runat="server" CssClass="form-control datapicker" ID="txtEndDate" />
                                        <asp:RequiredFieldValidator runat="server" ControlToValidate="txtEndDate"
                                            Display="Dynamic" SetFocusOnError="true" CssClass="label label-danger" ErrorMessage="End Date is Required" />
                                    </div>
                                    <div class="form-group">
                                        <asp:Label runat="server" AssociatedControlID="txtRating" Text="SWN Cascade" />
                                        <asp:TextBox runat="server" CssClass="form-control col-1" MaxLength="1" ID="txtCascade" />
                                    </div>
                                    <div class="form-check">
                                        <asp:CheckBox ID="chkSWNCall" runat="server" Text="SWN Call?" />
                                    </div>
                                    <div class="form-check">
                                        <asp:CheckBox ID="chkSWNText" runat="server" Text="SWN Text?" />
                                    </div>
                                    <div class="form-check">
                                        <asp:CheckBox ID="chkExcludeExt" runat="server" Text="SWN Exclude Ext?" />
                                    </div>
                                </div>
                                <div class="modal-footer">
                                    <asp:HiddenField ID="hdPositionHistoryId" runat="server" />
                                    <asp:Button OnClientClick="ToggleEditForm(false)" CssClass="btn btn-primary" ID="cmdSave" runat="server" Text="Save" OnClick="cmdSave_Click" />
                                    <button type="button" class="btn btn-default" data-dismiss="modal">Close</button>
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
    </div>
</div>
<dnn:dnncssinclude runat="server" filepath="~/Resources/Shared/components/TimePicker/Themes/jquery-ui.min.css" />
<dnn:dnnjsinclude runat="server" filepath="https://cdnjs.cloudflare.com/ajax/libs/jquery.mask/1.14.16/jquery.mask.js" />
<dnn:dnnjsinclude runat="server" filepath="https://cdn.datatables.net/v/bs5/dt-1.13.1/datatables.min.js" />
<dnn:dnncssinclude runat="server" filepath="https://cdn.datatables.net/v/bs5/dt-1.13.1/datatables.min.css" />

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
                { "bSortable": false },
                { "bSortable": true },
                { "bSortable": false },
                { "bSortable": false },
                { "bSortable": false },
                { "bSortable": false },
            ]
        });
        $("#tblPositionHistory_length").prepend('<button class="btn btn-primary btn-lg mr-2" data-toggle="modal" data-target="#EditPhoneModal"><i class="fa fa-plus"></i>&nbsp;Add Phone</button>');
        table.draw();

        $(".confirm").dnnConfirm({

            text: 'Are you sure you wish to delete this Phone?',

            yesText: 'Yes',

            noText: 'No',

            title: 'Delete Phone?'

        });
    }
    function ToggleEditForm(toggleValue) {
        if (toggleValue) {
            $('#EditPositionHistoryModal').modal('show');
        } else {
            if (typeof (Page_ClientValidate) == 'function') {
                Page_ClientValidate();
            }

            if (Page_IsValid) {
                $('#EditPositionHistoryModal').modal('hide');
                $('body').removeClass('modal-open');
                $('.modal-backdrop').remove();
            }
        }

        return true;
    }
</script>
