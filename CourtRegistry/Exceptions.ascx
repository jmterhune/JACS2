<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Exceptions.ascx.cs" Inherits="tjc.Modules.CourtRegistry.JacExceptions" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<dnn:dnnjsinclude runat="server" filepath="~/DesktopModules/tjc.modules/CourtRegistry/Scripts/registry-ui.js" />
<div class="tabs">
    <ul class="nav nav-tabs">
        <li class="nav-item">
            <a class="nav-link" href="<%=ApplicationListUrl%>">Applications</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=ManageYearsUrl%>">Manage Fiscal Years</a>
        </li>
        <li class="nav-item active">
            <a class="nav-link" href="#exceptions" data-toggle="tab">Exceptions</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=BasicSettingsUrl%>">Basic Settings</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=AttorneyListUrl%>">Attorneys</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=JacCodeListUrl%>">JAC Codes</a>
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
        <asp:UpdatePanel ID="pnlExceptions" runat="server" RenderMode="Block" OnUnload="pnlExceptions_Unload">
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
                <div id="exceptions" class="tab-pane active">
                    <div class="p-3 mb-4 bg-light text-dark border rounded">
                        <div class="row form-group">
                            <div class="col-auto">
                                <asp:Label AssociatedControlID="drpPeriod" Text="Select Period" runat="server" />
                                <asp:DropDownList runat="server" ClientIDMode="Static" ID="drpPeriod" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="drpPeriod_SelectedIndexChanged">
                                </asp:DropDownList>
                            </div>
                            <div class="col-auto">
                                <asp:Label AssociatedControlID="drpCategory" ClientIDMode="Static" Text="Case Type" runat="server" />
                                <asp:DropDownList AppendDataBoundItems="true" runat="server" ID="drpCategory" ClientIDMode="Static" OnSelectedIndexChanged="drpCategory_SelectedIndexChanged" AutoPostBack="true" CssClass="form-control">
                                    <asp:ListItem Text="ALL" Value="" />
                                </asp:DropDownList>
                            </div>
                            <div class="col-4">
                                <asp:Label AssociatedControlID="drpCode" Text="JAC Code" runat="server" />
                                <asp:DropDownList runat="server" AppendDataBoundItems="true" ID="drpCode" ClientIDMode="Static" CssClass="form-control">
                                    <asp:ListItem Text="ALL" Value="" />
                                </asp:DropDownList>
                            </div>
                            <div class="col-auto">
                                <asp:Label AssociatedControlID="drpLocation" Text="Location" runat="server" />
                                <asp:DropDownList runat="server" ID="drpLocation" AppendDataBoundItems="true" CssClass="form-control" ClientIDMode="Static">
                                    <asp:ListItem Text="ALL" Value="" />
                                </asp:DropDownList>
                            </div>
                        </div>
                        <div class="row form-group checkbox">
                            <div class="col-auto">
                                <asp:CheckBox ID="chkExclude" Text="Exclude?" ClientIDMode="Static" TextAlign="Right" runat="server" />
                            </div>
                            <div class="col-auto checkbox">
                                <asp:CheckBox ID="chkRenewal" Text="Renewals Only?" ClientIDMode="Static" TextAlign="Right" runat="server" />
                                <asp:CustomValidator ID="valCheckedOne" SetFocusOnError="true" Display="Dynamic" CssClass="label label-danger" runat="server" ErrorMessage="You must select Exclude or Renewals Only" ClientValidationFunction="Validate_CheckedOne" OnServerValidate="valCheckedOne_ServerValidate"></asp:CustomValidator>
                            </div>
                        </div>
                        <hr class="mt-0" />
                        <p class="m-0">
                            <asp:Button ID="cmdAddCodes" ClientIDMode="Static" Text="Add Codes to Exclude" CssClass="btn btn-primary me-2" runat="server" OnClick="cmdAddCodes_Click" />
                            <asp:HyperLink ID="lnkReturn" CssClass="btn btn-default" runat="server" Text="Return to Main Menu" />
                            <asp:Button ID="cmdClearExtensions" OnClientClick="return confirm('This will clear all exclusions below.<br /> Are you sure?');" CausesValidation="false" ClientIDMode="Static" Text="Clear all Extensions" CssClass="btn btn-danger float-end confirm-clear" runat="server" OnClick="cmdClearExtensions_Click" />
                        </p>
                    </div>
                </div>
                <asp:Repeater ID="rptExclusions" runat="server" OnItemCommand="rptExclusions_ItemCommand">
                    <HeaderTemplate>
                        <table id="tblCodes" class="table table-striped">
                            <thead>
                                <tr>
                                    <th>Fiscal Year</th>
                                    <th>JAC Code</th>
                                    <th>Location</th>
                                    <th>Exclude?</th>
                                    <th>Only Renewals?</th>
                                    <th>&nbsp;</th>
                                </tr>
                            </thead>
                            <tbody>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <tr>
                            <td><%#Eval("Period")%></td>
                            <td><%#Eval("JacCodeID")%></td>
                            <td><%#Eval("LocationName")%></td>
                            <td class="command-item"><%#Convert.ToBoolean(Eval("Exclude"))?"<i class='fas fa-square-check'></i>":"<i class='fas fa-square'></i>"%></td>
                            <td class="command-item"><%#Convert.ToBoolean(Eval("OnlyRenewals"))?"<i class='fas fa-square-check'></i>":"<i class='fas fa-square'></i>"%></td>
                            <td class="command-item">
                                <asp:LinkButton ID="cmdDelete" runat="server" CausesValidation="false" OnClientClick="return Registry.confirmDelete(this,'Exception');" CommandArgument='<%#Eval("JacCodeid")+"|"+Eval("LocationID")+"|"+Eval("Year")%>' CommandName="delete" CssClass="delete text-danger"><i class="fas fa-trash"></i></asp:LinkButton></td>
                        </tr>
                    </ItemTemplate>
                    <FooterTemplate>
                        </tbody>
                </table>
                    </FooterTemplate>
                </asp:Repeater>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</div>

<dnn:dnncssinclude runat="server" filepath="~/Resources/Shared/components/TimePicker/Themes/jquery-ui.min.css" />
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
        var tblCodes = $('#tblCodes').DataTable({
            "order": [[1, "asc"]],
            "oLanguage": {
                "sSearch": "Filter by Text"
            },
            "aoColumns": [
                { "bSortable": true },
                { "bSortable": true },
                { "bSortable": true },
                { "bSortable": true },
                { "bSortable": true },
                { "bSortable": false },
            ],
            autoWidth: true,
        });
    }
    function Validate_CheckedOne(sender, args) {
        args.IsValid = false;
        if ($("#chkExclude").is(":checked") || $("#chkRenewal.ClientID").is(":checked")) {
            args.IsValid = true;
        }
    }

    function ShowAlert(title, text) {
        $.dnnAlert({
            okText: 'OK',
            title: title,
            text: text
        });
    }
</script>
