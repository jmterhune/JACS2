<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DeSotoView.ascx.cs" Inherits="tjc.Modules.PretrialServices.DeSotoView" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<div class="fullScreenContainer">
    <div class="float-end fw-bold lead">
        <span id="current-intake-date" class="badge rounded-pill bg-danger fs-5"></span>
    </div>
    <div class="btn-toolbar mb-3" role="toolbar" aria-label="Filter Records">
        <div id="dateFilter" class="input-group me-3" role="group" aria-label="Date group">
            <button id="btnSearchType" type="button" class="btn btn-primary dropdown-toggle" data-toggle="dropdown" aria-expanded="false">
                Search Type
            </button>
            <ul class="dropdown-menu" aria-labelledby="btnSearchType">
                <li><a class="dropdown-item" href="#" onclick="SetSearchType('0',event)">Date</a></li>
                <li><a class="dropdown-item" href="#" onclick="SetSearchType('1',event)">Defendant Name</a></li>
                <li><a class="dropdown-item" href="#" onclick="SetSearchType('2',event)">Case Number</a></li>
            </ul>

            <div class="input-group-text" id="lblYear">Year:</div>
            <asp:DropDownList ID="drpYear" ClientIDMode="Static" CausesValidation="false" runat="server" AutoPostBack="true" CssClass="form-control input-item-lg" OnSelectedIndexChanged="drpYear_SelectedIndexChanged">
            </asp:DropDownList>
            <div class="input-group-text" id="lblMonth">Month:</div>
            <asp:DropDownList ID="drpMonth" ClientIDMode="Static" CausesValidation="false" AutoPostBack="true" runat="server" OnSelectedIndexChanged="drpMonth_SelectedIndexChanged" CssClass="form-control input-item-lg">
                <asp:ListItem Text="January" Value="1" />
                <asp:ListItem Text="February" Value="2" />
                <asp:ListItem Text="March" Value="3" />
                <asp:ListItem Text="April" Value="4" />
                <asp:ListItem Text="May" Value="5" />
                <asp:ListItem Text="June" Value="6" />
                <asp:ListItem Text="July" Value="7" />
                <asp:ListItem Text="August" Value="8" />
                <asp:ListItem Text="September" Value="9" />
                <asp:ListItem Text="October" Value="10" />
                <asp:ListItem Text="November" Value="11" />
                <asp:ListItem Text="December" Value="12" />
            </asp:DropDownList>
            <div class="input-group-text" id="lblDay">Day:</div>
            <asp:DropDownList ID="drpDay" ClientIDMode="Static" CausesValidation="false" AutoPostBack="true" CssClass="form-control" runat="server" OnSelectedIndexChanged="drpDay_SelectedIndexChanged">
            </asp:DropDownList>
            <asp:TextBox runat="server" CausesValidation="false" ID="txtSearchText" MaxLength="50" CssClass="form-control search-text" />
            <asp:Button ID="cmdSearch" ClientIDMode="Static" CausesValidation="false" CssClass="btn btn-primary" runat="server" Text="Search" OnClick="cmdSearch_Click" />
        </div>

        <div id="report" class="input-group" role="group" aria-label="Report Group">
            <button id="btnReportType" type="button" class="btn btn-primary dropdown-toggle" data-toggle="dropdown" aria-expanded="false">
                Report Type
            </button>
            <ul class="dropdown-menu" aria-labelledby="btnReportType">
                <li><a class="dropdown-item" href="#" onclick="SetReportType('0',event)">Daily</a></li>
                <li><a class="dropdown-item" href="#" onclick="SetReportType('1',event)">Weekly</a></li>
                <li><a class="dropdown-item" href="#" onclick="SetReportType('2',event)">Monthly</a></li>
                <li><a class="dropdown-item" href="#" onclick="SetReportType('3',event)">Yearly</a></li>
                <li><a class="dropdown-item" href="#" onclick="SetReportType('4',event)">Survey</a></li>

            </ul>
            <asp:Button ID="cmdReport" CausesValidation="false" CssClass="btn btn-quaternary" runat="server" Text="Get Report" OnClick="cmdReport_Click" />
        </div>
    </div>


    <asp:UpdatePanel ID="pnlDefendantsInProgram" ViewStateMode="Enabled" runat="server" RenderMode="Block" OnUnload="pnlDefendantsInProgram_Unload">
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

            <asp:Repeater ID="rptDefendantsInProgram" runat="server" OnItemCommand="rptDefendantsInProgram_ItemCommand" OnItemCreated="rptDefendantsInProgram_ItemCreated">
                <HeaderTemplate>
                    <table id="tblDefendantsInProgram" class="table table-striped">
                        <thead>
                            <tr>
                                <th>&nbsp;</th>
                                 <th>Intake Date</th>
                                <th>Defendant</th>
                                <th>Case Number</th>
                                <th>Charges</th>
                                <th class="text-center">Indigent</th>
                                <th class="text-center">
                                    <abbr title="Felony Conviction Dangerous">FCD</abbr></th>
                                <th class="text-center">
                                    <abbr title="Felony Conviction Non-Dangerous">FCND</abbr></th>
                                <th class="text-center">
                                    <abbr title="Misdemeanor Conviction Dangerous">MCD</abbr></th>
                                <th class="text-center">
                                    <abbr title="Misdemeanor Conviction Non-Dangerous">MCND</abbr></th>
                                <th>
                                    <abbr title="Failure to Appear">FTA</abbr>
                                    Date</th>
                                <th class="text-center">Court Appearances</th>
                                <th class="text-center">
                                    <abbr title="Bench Warrant Ordered">BWO</abbr>?</th>
                                <th class="text-center">Compliance</th>
                                <th class="text-center">Revoked?</th>
                                <th>Completion</th>
                                <th class="text-center">Days
                                    <abbr title="Supervised Release">SPR</abbr></th>
                                <th>&nbsp;</th>
                            </tr>
                        </thead>
                        <tbody>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr>
                        <td class="command-icon">
                            <asp:LinkButton ID="cmdEdit" runat="server" CommandName="edit" CausesValidation="false" CommandArgument='<%#DataBinder.Eval(Container.DataItem,"ItemId").ToString() %>'><i class="fa fa-pencil"></i></asp:LinkButton>
                        </td>
                         <td><%#DataBinder.Eval(Container.DataItem,"FormattedIntakeDate") %></td>
                        <td><%#DataBinder.Eval(Container.DataItem,"DefendantName") %></td>
                        <td><%#DataBinder.Eval(Container.DataItem,"CaseNumber") %></td>
                        <td><%#DataBinder.Eval(Container.DataItem,"ArrestCharges") %></td>
                        <td class="text-center"><%#DataBinder.Eval(Container.DataItem,"FormattedIndigent") %></td>
                        <td class="text-center"><%#DataBinder.Eval(Container.DataItem,"FcDangerous") %></td>
                        <td class="text-center"><%#DataBinder.Eval(Container.DataItem,"FcNonDangerous") %></td>
                        <td class="text-center"><%#DataBinder.Eval(Container.DataItem,"McDangerous") %></td>
                        <td class="text-center"><%#DataBinder.Eval(Container.DataItem,"McNonDangerous") %></td>
                        <td><%#DataBinder.Eval(Container.DataItem,"FormattedFTADate") %></td>
                        <td class="text-center"><%#DataBinder.Eval(Container.DataItem,"CourtAppearances") %></td>
                        <td class="text-center"><%#DataBinder.Eval(Container.DataItem,"FormattedBwOrdered") %></td>
                        <td class="text-center"><%#DataBinder.Eval(Container.DataItem,"NonCompArrestViolation") %></td>
                        <td class="text-center"><%#DataBinder.Eval(Container.DataItem,"IsRevoked").ToString()=="True"?"<i class=\"fas fa-check-square\"></i>":"<i class=\"fas fa-square\"></i>" %></td>
                        <td><%#DataBinder.Eval(Container.DataItem,"FormattedCompletion") %></td>
                        <td class="text-center"><%#DataBinder.Eval(Container.DataItem,"DaysSpr") %></td>
                        <td class="command-icon">
                            <asp:LinkButton ID="cmdDelete" CssClass="confirm" runat="server" CausesValidation="false" CommandName="delete" CommandArgument='<%#DataBinder.Eval(Container.DataItem,"ItemId").ToString() %>'><i class="fa fa-trash"></i></asp:LinkButton></td>
                    </tr>
                </ItemTemplate>
                <FooterTemplate>
                    </tbody></table><hr />
                </FooterTemplate>
            </asp:Repeater>
            <asp:HiddenField ID="hdIntakeDate" runat="server" ClientIDMode="Static" />
            <asp:HiddenField ID="hdSearchType" runat="server" ClientIDMode="Static" Value="0" />
            <asp:HiddenField ID="hdReportType" runat="server" ClientIDMode="Static" Value="0" />
            <div class="modal fade" id="EditDefendantsInProgramModal" tabindex="-1" role="dialog" aria-labelledby="EditDefendantsInProgramModalLabel" aria-hidden="true">
                <div class="modal-dialog modal-lg">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h4 class="modal-title" id="EditDefendantsInProgramModalLabel">Add / Edit Record</h4>
                            <asp:Button OnClientClick="DismissModal()" CausesValidation="False" CssClass="btn-close me-2" aria-hidden="true" ID="cmdClose2" runat="server" Text="&times;" OnClick="cmdClose_Click" />
                        </div>
                        <div class="modal-body pb-0">
                            <div class="form-group row">
                                <div class="col-2">
                                    <asp:Label runat="server" AssociatedControlID="txtIntakeDate" Text="Intake Date" />
                                    <asp:TextBox runat="server" CssClass="form-control datepicker" MaxLength="50" ID="txtIntakeDate" />
                                    <asp:RequiredFieldValidator runat="server" ValidationGroup="defendant" ControlToValidate="txtIntakeDate"
                                        Display="Dynamic" SetFocusOnError="true" CssClass="label label-danger" ErrorMessage="Intake Date is Required" />
                                </div>

                                <div class="col-5">
                                    <asp:Label runat="server" AssociatedControlID="txtName" Text="Defendant Name<em>*</em>" ToolTip="required" />
                                    <asp:TextBox runat="server" CssClass="form-control" MaxLength="100" ID="txtName" />
                                    <asp:RequiredFieldValidator runat="server" ValidationGroup="defendant" ControlToValidate="txtName"
                                        Display="Dynamic" SetFocusOnError="true" CssClass="label label-danger" ErrorMessage="Defendant Name is Required" />
                                </div>
                                <div class="col-2">
                                    <asp:Label runat="server" AssociatedControlID="txtFTADate" Text="<abbr title='Failure to Appear'>FTA</abbr> Date" />
                                    <asp:TextBox runat="server" CssClass="form-control datepicker" MaxLength="50" ID="txtFTADate" />
                                </div>
                                <div class="col-3">
                                    <asp:Label runat="server" AssociatedControlID="txtCompletionDate" Text="Completion Date" />
                                    <asp:TextBox runat="server" CssClass="form-control datepicker completion-date" MaxLength="50" ID="txtCompletionDate" />
                                </div>
                            </div>
                            <div class="form-group row">
                                <div class="col-4">
                                    <asp:Label runat="server" AssociatedControlID="txtCaseNumber" Text="Case Number<em>*</em>" ToolTip="required" />
                                    <asp:TextBox runat="server" CssClass="form-control" MaxLength="200" ID="txtCaseNumber" />
                                    <asp:RequiredFieldValidator runat="server" ControlToValidate="txtCaseNumber"
                                        Display="Dynamic" SetFocusOnError="true" ValidationGroup="defendant" CssClass="label label-danger" ErrorMessage="Case Number is Required" />
                                </div>
                                <div class="col-auto">
                                    <asp:Label runat="server" AssociatedControlID="txtCourtAppearances" Text="Court Appearances" />
                                    <asp:TextBox runat="server" CssClass="form-control" TextMode="Number" MaxLength="10" ID="txtCourtAppearances" />
                                </div>
                                <div class="col-auto">
                                    <asp:Label runat="server" AssociatedControlID="drpNewArrest" Text="Non-Comp New Arrest / Tech Violations" />
                                    <asp:DropDownList ID="drpNewArrest" runat="server" CssClass="form-control">
                                        <asp:ListItem Text="< Select Value >" Value="" />
                                        <asp:ListItem Text="New Arrest" Value="New Arrest" />
                                        <asp:ListItem Text="Violation Calls" Value="Viol Calls" />
                                        <asp:ListItem Text="Contact" Value="Contact" />
                                        <asp:ListItem Text="Other" Value="Other" />
                                        <asp:ListItem Text="UA" Value="UA" />
                                    </asp:DropDownList>
                                </div>
                            </div>
                            <div class="row form-group">
                                <div class="col-12 mb-2">
                                    <asp:Label runat="server" AssociatedControlID="txtCharges" Text="Arrest Charges<em>*</em>" />
                                    <asp:TextBox runat="server" TextMode="MultiLine" Rows="2" CssClass="form-control" ID="txtCharges" />
                                    <asp:RequiredFieldValidator runat="server" ControlToValidate="txtCharges"
                                        Display="Dynamic" SetFocusOnError="true" ValidationGroup="defendant" CssClass="label label-danger" ErrorMessage="Arrest Charges are Required" />
                                </div>
                                <div class="col-6">
                                    <fieldset class="form-fieldset">
                                        <legend>Felony Convictions</legend>
                                        <div class="form-group row">
                                            <div class="col-6">
                                                <asp:Label runat="server" AssociatedControlID="txtfcDanger" Text="Dangerous" />
                                                <asp:TextBox runat="server" TextMode="Number" CssClass="form-control" ID="txtfcDanger" />
                                            </div>
                                            <div class="col-6">
                                                <asp:Label runat="server" AssociatedControlID="txtfcNonDanger" Text="Non-Dangerous" />
                                                <asp:TextBox runat="server" TextMode="Number" CssClass="form-control" ID="txtfcNonDanger" />
                                            </div>
                                        </div>
                                    </fieldset>
                                </div>
                                <div class="col-6">
                                    <fieldset class="form-fieldset">
                                        <legend>Misdemeanor Convictions</legend>
                                        <div class="form-group row">
                                            <div class="col-6">
                                                <asp:Label runat="server" AssociatedControlID="txtmcDanger" Text="Dangerous" />
                                                <asp:TextBox runat="server" TextMode="Number" CssClass="form-control" ID="txtmcDanger" />
                                            </div>
                                            <div class="col-6">
                                                <asp:Label runat="server" AssociatedControlID="txtmcNonDanger" Text="Non-Dangerous" />
                                                <asp:TextBox runat="server" TextMode="Number" CssClass="form-control" ID="txtmcNonDanger" />
                                            </div>
                                        </div>
                                    </fieldset>
                                </div>
                            </div>
                            <div class="form-group mt-3">
                                <div class="row">
                                    <div class="col-4">
                                        <asp:Label runat="server" AssociatedControlID="drpCompletion" Text="Completion" />
                                        <asp:DropDownList ID="drpCompletion" runat="server" CssClass="form-control completion">
                                            <asp:ListItem Text="< Select Option >" Value="-1" />
                                            <asp:ListItem Text="Successfull" Value="1" />
                                            <asp:ListItem Text="Not Successfull" Value="0" />
                                            <asp:ListItem Text="Other" Value="2" />
                                        </asp:DropDownList>
                                    </div>
                                    <div class="col-4">
                                        <asp:Label runat="server" AssociatedControlID="drpCaseType" Text="Felony or Misd. Case" />
                                        <asp:DropDownList ID="drpCaseType" runat="server" CssClass="form-control">
                                            <asp:ListItem Text="< Select Option >" Value="-1" />
                                            <asp:ListItem Text="Felony" Value="1" />
                                            <asp:ListItem Text="Misdemeanor" Value="0" />
                                        </asp:DropDownList>
                                    </div>
                                    <div class="col-4">
                                        <asp:Label runat="server" AssociatedControlID="drpBondType" Text="Bond(s)?" />
                                        <asp:DropDownList ID="drpBondType" runat="server" CssClass="form-control">
                                            <asp:ListItem Text="< Select Option >" Value="-1" />
                                            <asp:ListItem Text="Secured" Value="1" />
                                            <asp:ListItem Text="Non-Secured" Value="0" />
                                            <asp:ListItem Text="Both" Value="2" />
                                            <asp:ListItem Text="Revoked" Value="3" />
                                        </asp:DropDownList>
                                    </div>
                                </div>
                                <div class="row mt-2">
                                    <div class="col-auto">
                                        <asp:Label runat="server" AssociatedControlID="drpNonCompliance" Text="Participant Non-Compliance" />
                                        <asp:DropDownList ID="drpNonCompliance" runat="server" CssClass="form-control">
                                            <asp:ListItem Text="< Select Option >" Value="-1" />
                                            <asp:ListItem Text="Failure to Appear" Value="0" />
                                            <asp:ListItem Text="Warrants Issued for Failure to Appear" Value="1" />
                                            <asp:ListItem Text="Release Revoked due to Failure to Appear" Value="2" />
                                            <asp:ListItem Text="Arrested for New Offense" Value="3" />
                                            <asp:ListItem Text="Release Revoked due to New Offense" Value="4" />
                                            <asp:ListItem Text="Non-Compliant with SPR Conditions" Value="5" />
                                            <asp:ListItem Text="Warrant Issued for Non-Compliance with SPR Conditions" Value="6" />
                                        </asp:DropDownList>
                                    </div>
                                </div>
                                <div class="row p-3 pb-0">
                                    <div class="col-3  form-check">
                                        <asp:CheckBox ID="chkFtaArrestHearing" CssClass="ftaHearing" runat="server" Text="<abbr title='Failure to Appear'>FTA</abbr> Arrest?" />
                                    </div>
                                    <div class="col-3 form-check">
                                        <asp:CheckBox ID="chkBwOrdered" CssClass="bw-ordered" runat="server" Text="Bench Warrant?" />
                                    </div>
                                    <div class="col-3 form-check">
                                        <asp:CheckBox ID="chkIndigent" CssClass="indigent" runat="server" Text="Indigent?" />
                                    </div>
                                    <div class="col-3 form-check">
                                        <asp:CheckBox ID="chkCaseScreened" runat="server" Text="Case Screened?" />
                                    </div>
                                    <div class="col-3 form-check">
                                        <asp:CheckBox ID="chkPlaced" runat="server" Text="Placed in Program?" />
                                    </div>
                                    <div class="col-3 form-check">
                                        <asp:CheckBox ID="chkRevoked" runat="server" Text="Revoked?" />
                                    </div>
                                    <div class="col-3 form-check">
                                        <asp:CheckBox ID="chkInterviewed" runat="server" Text="Interviewed?" />
                                    </div>
                                    <div class="col-3 form-check">
                                        <asp:CheckBox ID="chkAssessed" runat="server" Text="Assessed?" />
                                    </div>
                                    <div class="col-3 form-check">
                                        <asp:CheckBox ID="chkIndigentAssessed" runat="server" Text="Ingident Assessed?" />
                                    </div>
                                    <div class="col-3 form-check">
                                        <asp:CheckBox ID="chkPtrOrdered" runat="server" Text="<abbr title='Pretrial Release'>PTR</abbr> Ordered?" />
                                    </div>
                                    <div class="col-3 form-check">
                                        <asp:CheckBox ID="chkPtrRecommended" runat="server" Text="<abbr title='Pretrial Release'>PTR</abbr> Recommended?" />
                                    </div>
                                    <div class="col-3 form-check">
                                        <asp:CheckBox ID="chkPtrNotRecommended" runat="server" Text="<abbr title='Pretrial Release'>PTR</abbr> Not Recommended?" />
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="modal-footer justify-content-between">
                            <asp:HiddenField ID="hdItemId" runat="server" />
                            <asp:Button OnClientClick="ToggleEditForm(false)" ValidationGroup="defendant" CssClass="btn btn-primary" ID="cmdSave" runat="server" Text="Save" OnClick="cmdSave_Click" />
                            <asp:Button OnClientClick="DismissModal()" CausesValidation="False" CssClass="btn btn-default" ID="cmdClose" runat="server" Text="Close" OnClick="cmdClose_Click" />
                        </div>
                    </div>
                </div>
            </div>
        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="cmdSave" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="cmdReport" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="cmdSearch" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="cmdClose" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="cmdClose2" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="drpYear" EventName="SelectedIndexChanged" />
            <asp:AsyncPostBackTrigger ControlID="drpMonth" EventName="SelectedIndexChanged" />
            <asp:AsyncPostBackTrigger ControlID="drpDay" EventName="SelectedIndexChanged" />
        </Triggers>

    </asp:UpdatePanel>

</div>
<dnn:dnncssinclude runat="server" filepath="~/Resources/Libraries/jQuery-UI/01_13_02/Themes/jquery-ui.css" />
<dnn:dnnjsInclude runat="server" FilePath="/Resources/Libraries/Datatables/datatables.min.js" />
<dnn:dnncssInclude runat="server" FilePath="/Resources/Libraries/Datatables/datatables.min.css" />


<script type="text/javascript">
    /*globals jQuery, window, Sys */
    (function ($, Sys) {

        $(document).ready(function () {
            SetSearchType(null, null);
            Sys.Application.add_load(function (s, e) { PageInit(); });
            PageInit();
        });

    }(jQuery, window.Sys));

    function PageInit() {
        $("#dialog").dialog({
            autoOpen: false,
            modal: true
        });
        $(".datepicker").datepicker();
        $(".form-check input").addClass("form-check-input");
        $(".form-check label").addClass("form-check-label");
        $("#cmdCancel").on("click", function (e) {
            e.preventDefault();
            ClearIntakeForm();
        });
        SetIntakeBadge();
        var table = $('#tblDefendantsInProgram').DataTable({
            "order": [[2, "asc"]],
            "oLanguage": {
                "sSearch": "Filter by Text"
            },
        });

        $("#tblDefendantsInProgram_length").prepend('<button class="btn btn-primary me-2" data-bs-toggle="modal" data-bs-target="#EditDefendantsInProgramModal"><i class="fa fa-plus"></i>&nbsp;Add New Record</button>');
        table.draw();

        $(".confirm").dnnConfirm({
            text: 'Are you sure you wish to delete this Record?',
            yesText: 'Yes',
            noText: 'No',
            title: 'Delete Record?'
        });
    }
    function ConfirmDelete(e) {
        e.preventDefault();
        $("#dialog").dialog({
            resizable: false,
            height: "auto",
            width: 400,
            modal: true,
            buttons: {
                "Confirm": function () {
                    $(this).dialog("close");
                    return true;
                },
                Cancel: function () {
                    $(this).dialog("close");
                    return false;
                }
            }
        });
    }
    function SetIntakeBadge() {
        var year = $("#drpYear").val();
        var month = $("#drpMonth").val();
        var day = $("#drpDay").val();
        $("#current-intake-date").text("Selected Intake Date: " + month + "/" + day + "/" + year);
    }
    function DismissModal() {
        $('#EditDefendantsInProgramModal').modal('hide');
        $('body').removeClass('modal-open');
        $('.modal-backdrop').remove();
    }
    function ToggleEditForm(toggleValue) {
        if (toggleValue) {
            $('#EditDefendantsInProgramModal').modal('show');
        } else {
            if (typeof (Page_ClientValidate) == 'function') {
                Page_ClientValidate();
            }

            if (Page_IsValid) {
                $('#EditDefendantsInProgramModal').modal('hide');
                $('body').removeClass('modal-open');
                $('.modal-backdrop').remove();
            }
        }
        return true;
    }

    function SetSearchType(searchType, e) {
        var firstTime = false;
        if (e != null) {
            e.preventDefault();
        }
        if (searchType === null) {
            searchType = $("#hdSearchType").val();
            firstTime = true;
        }
        else {
            $("#hdSearchType").val(searchType);
        }
        switch (searchType) {
            case "0":
                $("#btnSearchType").text("Search by Date");
                ToggleVisibility(0);
                if (!firstTime) {
                    $('#cmdSearch').trigger('click');
                }
                break;
            case "1":
                $("#btnSearchType").text("Search by Defendant Name");
                ToggleVisibility(1);
                break;
            case "2":
                $("#btnSearchType").text("Search by Case Number");
                ToggleVisibility(1);
                break;

            default:
        }
    }
    function SetReportType(reportType, e) {
        var firstTime = false;
        if (e != null) {
            e.preventDefault();
        }
        if (reportType === null) {
            reportType = $("#hdReportType").val();
            firstTime = true;
        }
        else {
            $("#hdReportType").val(reportType);
        }
        switch (reportType) {
            case "0":
                $("#btnReportType").text("Daily Report");
                if (!firstTime) {
                    $('#cmdReport').trigger('click');
                }
                break;
            case "1":
                $("#btnReportType").text("Weekly Report");
                break;
            case "2":
                $("#btnReportType").text("Monthly Report");
                break;
            case "3":
                $("#btnReportType").text("Yearly Report");
                break;
            case "4":
                $("#btnReportType").text("Survey Report");
                break;
            default:
        }
    }


    function ToggleVisibility(fType) {
        if (fType == 0) {
            $("#lblYear").fadeIn();
            $("#drpYear").fadeIn();
            $("#lblMonth").fadeIn();
            $("#drpMonth").fadeIn();
            $("#lblDay").fadeIn();
            $("#drpDay").fadeIn();
            $("#report").fadeIn();
            $("#cmdSearch").fadeOut();
            $(".search-text").fadeOut();
        }
        else {
            $("#lblYear").fadeOut();
            $("#drpYear").fadeOut();
            $("#lblMonth").fadeOut();
            $("#drpMonth").fadeOut();
            $("#lblDay").fadeOut();
            $("#drpDay").fadeOut();
            $("#report").fadeOut();
            $("#cmdSearch").fadeIn();
            $(".search-text").fadeIn();
        }
    }

</script>
