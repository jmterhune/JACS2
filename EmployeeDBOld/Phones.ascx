<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Phones.ascx.cs" Inherits="tjc.Modules.EmployeeDB.Phones" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<div class="tabs">
    <ul class="nav nav-tabs">
        <li class="nav-item">
            <a class="nav-link" href="<%=EmployeeUrl %>"><i class="fas fa-list"></i>&nbsp;Back to  List</a>
        </li>

        <li class="nav-item">
            <asp:HyperLink ID="lnkDetails" runat="server" CssClass="nav-link"><i class="fas fa-user-edit"></i>&nbsp;Details</asp:HyperLink>
        </li>
        <li class="nav-item active">
            <a class="nav-link" href="#phones" data-toggle="tab"><i class="fas fa-phone"></i>&nbsp;Phone Numbers</a>
        </li>
        <li class="nav-item" id="liGroups" runat="server">
            <a class="nav-link" href="<%=DetailUrl%>?g=groups"><i class="fas fa-users"></i>&nbsp;Groups</a>
        </li>
        <li class="nav-item" id="liHistory" runat="server">
            <a class="nav-link" href="<%=EmploymentUrl%>"><i class="fas fa-user-clock"></i>&nbsp;Employment History</a>
        </li>
        <li class="nav-item" id="liEmergencyContacts" runat="server">
            <a class="nav-link" href="<%=EmergencyContactUrl%>"><i class="fas fa-address-book"></i>&nbsp;Emergency Contacts</a>
        </li>
    </ul>
    <div class="tab-content  edit-form">
        <div id="phones" class="tab-pane active">
            <asp:UpdatePanel ID="pnlPhones" runat="server" RenderMode="Block" OnUnload="pnlPhones_Unload">
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
                    <asp:Repeater ID="rptPhones" runat="server" OnItemDataBound="rptPhones_ItemDataBound" OnItemCommand="rptPhones_ItemCommand" OnItemCreated="rptPhones_ItemCreated">
                        <HeaderTemplate>
                            <table id="tblPhones" class="table table-striped">
                                <thead>
                                    <tr>
                                        <th>&nbsp;</th>
                                        <th>Type</th>
                                        <th>Phone</th>
                                        <th>Location</th>

                                        <th>SWN<br />
                                            Call?</th>
                                        <th>SWN<br />
                                            Text?</th>
                                        <th>SWN<br />
                                            Exclude Ext?</th>
                                        <th>SWN<br />
                                            Cascade</th>
                                        <th>&nbsp;</th>
                                        <th>&nbsp;</th>
                                    </tr>
                                </thead>
                                <tbody>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <tr>
                                <td class="command-icon">
                                    <asp:LinkButton ID="cmdEdit" runat="server" CommandName="edit" CausesValidation="false" CommandArgument='<%#DataBinder.Eval(Container.DataItem,"PhoneId").ToString() %>'><i class="fa fa-pencil"></i></asp:LinkButton>
                                </td>
                                <td><%#DataBinder.Eval(Container.DataItem,"PhoneType") %></td>
                                <td><%#DataBinder.Eval(Container.DataItem,"FormattedPhone") %></td>
                                <td><%#DataBinder.Eval(Container.DataItem,"OfficeLocationName") %></td>
                                <td><%#DataBinder.Eval(Container.DataItem,"SwnText").ToString()=="True"?"<i class=\"fas fa-check-square\"></i>":"<i class=\"fas fa-square\"></i>" %></td>
                                <td><%#DataBinder.Eval(Container.DataItem,"SwnCall").ToString()=="True"?"<i class=\"fas fa-check-square\"></i>":"<i class=\"fas fa-square\"></i>" %></td>
                                <td><%#DataBinder.Eval(Container.DataItem,"SwnExcludeExtension").ToString()=="True"?"<i class=\"fas fa-check-square\"></i>":"<i class=\"fas fa-square\"></i>" %></td>
                                <td><%#DataBinder.Eval(Container.DataItem,"PhoneCascade") %></td>
                                <td>
                                    <asp:LinkButton ID="cmdUp" runat="server" ToolTip="Move Up in Cascade Order" CausesValidation="false" CommandName="up" CommandArgument='<%#DataBinder.Eval(Container.DataItem,"PhoneId").ToString() %>'><i class="fas fa-long-arrow-alt-up"></i></asp:LinkButton>
                                    <asp:LinkButton ID="cmdDown" runat="server" ToolTip="Move Down in Cascade Order" CausesValidation="false" CommandName="down" CommandArgument='<%#DataBinder.Eval(Container.DataItem,"PhoneId").ToString() %>'><i class="fas fa-long-arrow-alt-down"></i></asp:LinkButton></td>
                                <td class="command-icon">
                                    <asp:LinkButton ID="cmdDelete" ToolTip="Delete Phone Number" CssClass="confirm" runat="server" CausesValidation="false" CommandName="delete" CommandArgument='<%#DataBinder.Eval(Container.DataItem,"PhoneId").ToString() %>'><i class="fa fa-trash"></i></asp:LinkButton></td>
                            </tr>
                        </ItemTemplate>
                        <FooterTemplate>
                            </tbody></table><hr />
                        </FooterTemplate>
                    </asp:Repeater>
                    <div class="modal fade" id="EditPhoneModal" tabindex="-1" role="dialog" aria-labelledby="EditPhoneModalLabel" aria-hidden="true">
                        <div class="modal-dialog">
                            <div class="modal-content">
                                <div class="modal-header">
                                    <h4 class="modal-title" id="EditPhoneModalLabel">Add / Edit Phone</h4>
                                    <button type="button" class="close" data-bs-dismiss="modal" aria-hidden="true">&times;</button>
                                </div>
                                <div class="modal-body">
                                    <div class="form-group">
                                        <asp:Label runat="server" AssociatedControlID="drpType" Text="Group Type<em>*</em>" ToolTip="required" />
                                        <asp:DropDownList runat="server" ID="drpType" CssClass="form-control" ClientIDMode="Static">
                                            <asp:ListItem Text="Work" />
                                            <asp:ListItem Text="Work Cell" />
                                            <asp:ListItem Text="Mobile" />
                                            <asp:ListItem Text="Home" />
                                            <asp:ListItem Text="Judicial Office" />
                                        </asp:DropDownList>
                                        <asp:RequiredFieldValidator runat="server" ControlToValidate="drpType"
                                            Display="Dynamic" SetFocusOnError="true" CssClass="label label-danger" ErrorMessage="Group Type is Required" />
                                    </div>
                                    <div class="form-group">
                                        <asp:Label runat="server" AssociatedControlID="txtNumber" Text="Phone<em>*</em>" ToolTip="required" />
                                        <asp:TextBox runat="server" CssClass="form-control phone" MaxLength="20" ID="txtNumber" ClientIDMode="Static" />
                                        <asp:RequiredFieldValidator runat="server" ControlToValidate="txtNumber"
                                            Display="Dynamic" SetFocusOnError="true" CssClass="label label-danger" ErrorMessage="Phone Number is Required" />
                                    </div>
                                    <div class="form-group">
                                        <asp:Label runat="server" AssociatedControlID="txtExtension" Text="Extension" />
                                        <asp:TextBox runat="server" CssClass="form-control" MaxLength="10" ID="txtExtension" ClientIDMode="Static" />
                                    </div>
                                    <div class="form-group">
                                        <asp:Label runat="server" AssociatedControlID="drpLocation" Text="Location" />
                                        <asp:DropDownList ID="drpLocation" runat="server" CssClass="form-control" DataTextField="Description" DataValueField="OfficeLocationId" AppendDataBoundItems="true" ClientIDMode="Static">
                                            <asp:ListItem Text="<Select Location>" Value="" />
                                        </asp:DropDownList>
                                    </div>
                                    <div class="form-group">
                                        <asp:Label runat="server" AssociatedControlID="txtCascade" Text="SWN Cascade" />
                                        <asp:TextBox runat="server" CssClass="form-control col-1" MaxLength="1" ID="txtCascade" ClientIDMode="Static" />
                                    </div>
                                    <div class="form-check">
                                        <asp:CheckBox ID="chkSWNCall" runat="server" Text="SWN Call?" ClientIDMode="Static" />
                                    </div>
                                    <div class="form-check">
                                        <asp:CheckBox ID="chkSWNText" runat="server" Text="SWN Text?" ClientIDMode="Static" />
                                    </div>
                                    <div class="form-check">
                                        <asp:CheckBox ID="chkExcludeExt" runat="server" Text="SWN Exclude Ext?" ClientIDMode="Static" />
                                    </div>
                                    <asp:HiddenField ID="hdPhoneId" runat="server" ClientIDMode="Static" />

                                </div>
                                <div class="modal-footer justify-content-between">
                                    <asp:Button OnClientClick="ToggleEditForm(false)" CssClass="btn btn-primary" ID="cmdSave" runat="server" Text="Save" OnClick="cmdSave_Click" />
                                    <button type="button" class="btn btn-default" data-bs-dismiss="modal">Close</button>
                                </div>
                            </div>
                        </div>
                    </div>
                    <p>
                        <asp:LinkButton CssClass="btn btn-dark me-2" runat="server" ID="cmdFixSort" CausesValidation="false" OnClick="cmdFixSort_Click"><i class="fas fa-sort"></i> Fix SWN Phone Sort Order</asp:LinkButton>
                    </p>
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="cmdSave" EventName="Click" />
                    <asp:AsyncPostBackTrigger ControlID="cmdFixSort" EventName="Click" />
                </Triggers>
            </asp:UpdatePanel>
        </div>
    </div>
</div>
<dnn:dnncssinclude runat="server" filepath="~/Resources/Shared/components/TimePicker/Themes/jquery-ui.min.css" />
<dnn:dnncssinclude runat="server" filepath="/Resources/Libraries/DataTables/dataTables.bootstrap5.min.css" />
<dnn:dnnjsinclude runat="server" filepath="/Resources/Libraries/jQuery/jquery.mask.js" />
<dnn:dnnjsinclude runat="server" filepath="/Resources/Libraries/DataTables/dataTables.min.js" />
<dnn:dnnjsinclude runat="server" filepath="/Resources/Libraries/DataTables/dataTables.bootstrap5.min.js" />

<script type="text/javascript">
    /*globals jQuery, window, Sys */
    (function ($, Sys) {
        $(document).ready(function () {
            Sys.Application.add_load(function (s, e) { PageInit(); });
            PageInit();
        });
    }(jQuery, window.Sys));
    function PageInit() {
        $('.phone').mask('(000) 000-0000');
        var table = $('#tblPhones').DataTable({
            "order": [[8, "asc"]],
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
                { "bSortable": false },]
        });
        $(".dt-length").prepend('<button onclick="return ClearForm()" class="btn btn-primary btn-sm me-2" data-bs-toggle="modal" data-bs-target="#EditPhoneModal"><i class="fa fa-plus"></i>&nbsp;Add Phone</button>');
        table.on('draw', function () {
            $(".confirm").dnnConfirm({
                text: 'Are you sure you wish to delete this Phone?',
                yesText: 'Yes',
                noText: 'No',
                title: 'Delete Phone?'
            });
        });
        table.draw();
    }
    function ToggleEditForm(toggleValue) {
        if (toggleValue) {
            $('#EditPhoneModal').modal('show');
        } else {
            if (typeof (Page_ClientValidate) == 'function') {
                Page_ClientValidate();
            }
            if (Page_IsValid) {
                $('#EditPhoneModal').modal('hide');
                $('body').removeClass('modal-open');
                $('.modal-backdrop').remove();
            }
        }
        return true;
    }
    function ClearForm() {
        $('#drpType').val("");
        $('#txtNumber').val("");
        $('#txtExtension').val("");
        $('#drpLocation').val("");
        $('#txtCascade').val("");
        $('#chkSWNText').prop("checked", false);
        $('#chkSWNCall').prop("checked", false);
        $('#chkExcludeExt').prop("checked", false);
        $('#hdPhoneId').val("");
        return false;
    }
</script>
