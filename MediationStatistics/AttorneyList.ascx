<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AttorneyList.ascx.cs" Inherits="tjc.Modules.MediationStatistics.AttorneyList" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>

<div class="tabs">
    <ul class="nav nav-tabs">
        <li class="nav-item">
            <a class="nav-link" href="<%=CaseListUrl %>"><i class="fas fa-list"></i>&nbsp;Back to List</a>
        </li>
        <li class="nav-item active">
            <a class="nav-link" href="#attorneys" data-toggle="tab">Attorneys</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=MediatorListUrl %>">Mediators</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=RegionListUrl %>">Regions</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=GroupListUrl %>">Case Type Groups</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=CaseTypeListUrl %>">Case Types</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=AppearanceListUrl %>">Appearance Values</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=IssueListUrl %>">Issues</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="#<%=ActionListUrl %>">Stage of Action Items</a>
        </li>
    </ul>
    <div class="tab-content">
        <div id="attorneys" class="tab-pane active">
            <asp:UpdatePanel ID="pnlAttorneys" runat="server" RenderMode="Block" OnUnload="pnlAttorneys_Unload">
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
                    <asp:Repeater ID="rptAttorney" runat="server" OnItemCreated="rptAttorney_ItemCreated" OnItemCommand="rptAttorney_ItemCommand">
                        <HeaderTemplate>
                            <table id="tblAttorney" class="table table-striped">
                                <thead>
                                    <tr>
                                        <th>&nbsp;</th>
                                        <th>Name</th>
                                        <th>Firm</th>
                                        <th>Phone</th>
                                        <th>Email</th>
                                        <th>Address</th>
                                        <th>City</th>
                                        <th>State</th>
                                        <th>Zip</th>
                                        <th>&nbsp;</th>
                                    </tr>
                                </thead>
                                <tbody>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <tr>
                                <td class="command-item">
                                    <asp:LinkButton ID="cmdEdit" runat="server" CssClass="text-primary" CommandName="edit" CausesValidation="false" CommandArgument='<%#DataBinder.Eval(Container.DataItem,"AttorneyId").ToString() %>'><i class="fas fa-edit"></i></asp:LinkButton>
                                <td><%#Eval("FullName")%></td>
                                <td><%#Eval("Firm") %></td>
                                <td><%#Eval("FormattedPhone") %></td>
                                <td><a href='mailto:<%#Eval("Email") %>'><%#Eval("Email") %></a></td>
                                <td><%#Eval("Address") %></td>
                                <td><%#Eval("City") %></td>
                                <td><%#Eval("State") %></td>
                                <td><%#Eval("Zip") %></td>
                                <td class="command-item">
                                    <asp:LinkButton ID="cmdDelete" CssClass="text-danger confirm" runat="server" CausesValidation="false" CommandName="delete" CommandArgument='<%#DataBinder.Eval(Container.DataItem,"AttorneyId").ToString() %>'><i class="fas fa-trash"></i></asp:LinkButton>
                                </td>
                            </tr>
                        </ItemTemplate>
                        <FooterTemplate>
                            </tbody>
                    </table>
                        </FooterTemplate>
                    </asp:Repeater>
                    <div class="modal fade" id="EditAttorneyModal" tabindex="-1" role="dialog" aria-labelledby="EditAttorneyModalLabel" aria-hidden="true">
                        <div class="modal-dialog modal-lg">
                            <div class="modal-content">
                                <div class="modal-header">
                                    <h4 class="modal-title" id="EditAttorneyModalLabel">Add / Edit Attorney</h4>
                                    <button type="button" class="close" data-bs-dismiss="modal" aria-hidden="true">&times;</button>
                                </div>
                                <div class="modal-body">
                                    <div class="row form-group">
                                        <div class="col-3">
                                            <asp:Label runat="server" AssociatedControlID="txtFirstName" Text="First Name" />
                                            <asp:TextBox AutoCompleteType="Disabled" runat="server" ClientIDMode="Static" CssClass="form-control" MaxLength="50" ID="txtFirstName" />
                                        </div>
                                        <div class="col-4">
                                            <asp:Label runat="server" AssociatedControlID="txtLastName" Text="Last Name" />
                                            <asp:TextBox AutoCompleteType="Disabled" runat="server" ClientIDMode="Static" CssClass="form-control" MaxLength="50" ID="txtLastName" />
                                        </div>
                                        <div class="col-5">
                                            <asp:Label runat="server" AssociatedControlID="txtFirm" Text="Firm" />
                                            <asp:TextBox AutoCompleteType="Disabled" runat="server" ClientIDMode="Static" CssClass="form-control" MaxLength="50" ID="txtFirm" />
                                        </div>
                                    </div>
                                    <div class="row form-group">
                                        <div class="col-4">
                                            <asp:Label runat="server" AssociatedControlID="txtPhone" Text="Phone" />
                                            <asp:TextBox AutoCompleteType="Disabled" runat="server" ClientIDMode="Static" CssClass="form-control phone" MaxLength="15" ID="txtPhone" />
                                        </div>
                                        <div class="col-3">
                                            <asp:Label runat="server" AssociatedControlID="txtExtension" Text="Extension" />
                                            <asp:TextBox AutoCompleteType="Disabled" runat="server" ClientIDMode="Static" CssClass="form-control" MaxLength="10" ID="txtExtension" />
                                        </div>
                                        <div class="col-5">
                                            <asp:Label runat="server" AssociatedControlID="txtEmail" Text="Email" />
                                            <asp:TextBox AutoCompleteType="Disabled" runat="server" ClientIDMode="Static" CssClass="form-control" MaxLength="250" ID="txtEmail" />
                                        </div>
                                    </div>
                                    <div class="row form-group">
                                        <div class="col-12">
                                            <asp:Label runat="server" AssociatedControlID="txtAddress" Text="Address" />
                                            <asp:TextBox AutoCompleteType="Disabled" runat="server" ClientIDMode="Static" CssClass="form-control" MaxLength="150" ID="txtAddress" />
                                        </div>
                                    </div>
                                    <div class="row form-group">
                                        <div class="col-5">
                                            <asp:Label runat="server" AssociatedControlID="txtCity" Text="City" />
                                            <asp:TextBox AutoCompleteType="Disabled" runat="server" ClientIDMode="Static" CssClass="form-control" MaxLength="50" ID="txtCity" />
                                        </div>
                                        <div class="col-3">
                                            <asp:Label runat="server" AssociatedControlID="drpState" Text="State" />
                                            <asp:DropDownList ID="drpState" ClientIDMode="Static" runat="server" CssClass="form-control">
                                                <asp:ListItem Value="" Text="" />
                                                <asp:ListItem Value="AL" Text="Alabama" />
                                                <asp:ListItem Value="AK" Text="Alaska" />
                                                <asp:ListItem Value="AZ" Text="Arizona" />
                                                <asp:ListItem Value="AR" Text="Arkansas" />
                                                <asp:ListItem Value="CA" Text="California" />
                                                <asp:ListItem Value="CO" Text="Colorado" />
                                                <asp:ListItem Value="CT" Text="Connecticut" />
                                                <asp:ListItem Value="DE" Text="Delaware" />
                                                <asp:ListItem Value="DC" Text="District of Columbia" />
                                                <asp:ListItem Value="FL" Text="Florida" />
                                                <asp:ListItem Value="GA" Text="Georgia" />
                                                <asp:ListItem Value="HI" Text="Hawaii" />
                                                <asp:ListItem Value="ID" Text="Idaho" />
                                                <asp:ListItem Value="IL" Text="Illinois" />
                                                <asp:ListItem Value="IN" Text="Indiana" />
                                                <asp:ListItem Value="IA" Text="Iowa" />
                                                <asp:ListItem Value="KS" Text="Kansas" />
                                                <asp:ListItem Value="KY" Text="Kentucky" />
                                                <asp:ListItem Value="LA" Text="Louisiana" />
                                                <asp:ListItem Value="ME" Text="Maine" />
                                                <asp:ListItem Value="MD" Text="Maryland" />
                                                <asp:ListItem Value="MA" Text="Massachusetts" />
                                                <asp:ListItem Value="MI" Text="Michigan" />
                                                <asp:ListItem Value="MN" Text="Minnesota" />
                                                <asp:ListItem Value="MS" Text="Mississippi" />
                                                <asp:ListItem Value="MO" Text="Missouri" />
                                                <asp:ListItem Value="MT" Text="Montana" />
                                                <asp:ListItem Value="NE" Text="Nebraska" />
                                                <asp:ListItem Value="NV" Text="Nevada" />
                                                <asp:ListItem Value="NH" Text="New Hampshire" />
                                                <asp:ListItem Value="NJ" Text="New Jersey" />
                                                <asp:ListItem Value="NM" Text="New Mexico" />
                                                <asp:ListItem Value="NY" Text="New York" />
                                                <asp:ListItem Value="NC" Text="North Carolina" />
                                                <asp:ListItem Value="ND" Text="North Dakota" />
                                                <asp:ListItem Value="OH" Text="Ohio" />
                                                <asp:ListItem Value="OK" Text="Oklahoma" />
                                                <asp:ListItem Value="OR" Text="Oregon" />
                                                <asp:ListItem Value="PA" Text="Pennsylvania" />
                                                <asp:ListItem Value="RI" Text="Rhode Island" />
                                                <asp:ListItem Value="SC" Text="South Carolina" />
                                                <asp:ListItem Value="SD" Text="South Dakota" />
                                                <asp:ListItem Value="TN" Text="Tennessee" />
                                                <asp:ListItem Value="TX" Text="Texas" />
                                                <asp:ListItem Value="UT" Text="Utah" />
                                                <asp:ListItem Value="VT" Text="Vermont" />
                                                <asp:ListItem Value="VA" Text="Virginia" />
                                                <asp:ListItem Value="WA" Text="Washington" />
                                                <asp:ListItem Value="WV" Text="West Virginia" />
                                                <asp:ListItem Value="WI" Text="Wisconsin" />
                                                <asp:ListItem Value="WY" Text="Wyoming" />
                                            </asp:DropDownList>
                                        </div>
                                        <div class="col-4">
                                            <asp:Label runat="server" AssociatedControlID="txtZip" Text="Zip" />
                                            <asp:TextBox AutoCompleteType="Disabled" runat="server" ClientIDMode="Static" CssClass="form-control" MaxLength="50" ID="txtZip" />
                                        </div>
                                    </div>
                                    <asp:HiddenField ID="hdAttorneyId" ClientIDMode="Static" runat="server" />
                                </div>
                                <div class="modal-footer justify-content-between">
                                    <asp:Button OnClientClick="ToggleEditForm(false)" CssClass="btn btn-primary" ID="cmdSave" runat="server" Text="Save" OnClick="cmdSave_Click" />
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
        <div id="regions" class="tab-pane">
        </div>
        <div id="groups" class="tab-pane">
        </div>
        <div id="types" class="tab-pane">
        </div>
        <div id="appearances" class="tab-pane">
        </div>
        <div id="issues" class="tab-pane">
        </div>
        <div id="actions" class="tab-pane">
        </div>
        <div id="type-group" class="tab-pane">
        </div>
        <div id="appearance-group" class="tab-pane">
        </div>
        <div id="issue-group" class="tab-pane">
        </div>
    </div>
</div>
<dnn:dnncssinclude runat="server" filepath="/Resources/Libraries/DataTables/dataTables.bootstrap5.min.css" />
<dnn:dnnjsinclude runat="server" filepath="/Resources/Libraries/jquery/jquery.mask.js" />
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
        $('.phone').mask('(000) 000-0000');
        var table = $('#tblAttorney').DataTable({
            "order": [[1, "asc"]],
            "oLanguage": {
                "sSearch": "Filter by Text"
            },
            "aoColumns": [
                { "bSortable": false },
                { "bSortable": true },
                { "bSortable": true },
                { "bSortable": false },
                { "bSortable": false },
                { "bSortable": false },
                { "bSortable": false },
                { "bSortable": false },
                { "bSortable": false },
                { "bSortable": false },],
            autoWidth: true,
        });
        $(".dt-length").prepend('<button onclick="return ClearForm()" class="btn btn-primary btn-sm me-2" data-bs-toggle="modal" data-bs-target="#EditAttorneyModal"><i class="fa fa-plus"></i>&nbsp;Add Attorney</button>');
        table.on('draw', function () {
            $(".confirm").dnnConfirm({
                text: 'Are you sure you wish to Delete the selected Attorney?',
                yesText: 'Yes',
                noText: 'No',
                title: 'Delete Attorney?'
            });
        });
        table.draw();
    }
    function ToggleEditForm(toggleValue) {
        if (toggleValue) {
            $('#EditAttorneyModal').modal('show');
        } else {
            $('#EditAttorneyModal').modal('hide');
            $('body').removeClass('modal-open');
            $('.modal-backdrop').remove();
        }
        return true;
    }
    function ClearForm() {
        $('#drpState').val("");
        $('#txtPhone').val("");
        $('#txtExtension').val("");
        $('#txtFirstName').val("");
        $('#txtLastName').val("");
        $('#txtEmail').val("");
        $('#txtFirm').val("");
        $('#txtAddress').val("");
        $('#txtCity').val("");
        $('#txtZip').val("");
        $('#hdAttorneyId').val("");
        return false;
    }
</script>

