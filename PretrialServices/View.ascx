<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="View.ascx.cs" Inherits="tjc.Modules.PretrialServices.View" %>
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
            <div class="input-group-text" id="lblReportType">Report Type:</div>
            <asp:DropDownList ID="drpReportType" ClientIDMode="Static" CausesValidation="false" runat="server" CssClass="form-control input-item-lg">
                <asp:ListItem Value="0">Daily</asp:ListItem>
                <asp:ListItem Value="1">Weekly</asp:ListItem>
                <asp:ListItem Value="2">Monthly</asp:ListItem>
                <asp:ListItem Value="3">Yearly</asp:ListItem>
            </asp:DropDownList>
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
            <asp:Panel runat="server" ID="pnlIntakeForm" CssClass="intake-form mx-auto" Enabled="false">
                <div class="alert alert-info"><i class="fa fa-info-circle"></i>This section is only active on the 7<sup>th</sup>, 14<sup>th</sup>, 21<sup>st</sup>, 28<sup>th</sup>, or last day of the month</div>
                <h4>Week End Intake Log</h4>
                <asp:Literal ID="ltMessage" runat="server" Visible="false"><div class="alert alert-{0} alert-dismissible fade show" role="alert"><i class="fa fa-{1}"></i>&nbsp;{2} <button type="button" class="close" data-dismiss="alert" aria-label="Close">
    <span aria-hidden="true">&times;</span>
  </button></div></asp:Literal>

                <div class="form-group row">
                    <div class="col-4">
                        <asp:Label runat="server" AssociatedControlID="txtInterviewed" Text="Interviewed" />
                        <asp:TextBox runat="server" ClientIDMode="Static" CssClass="form-control" TextMode="Number" MaxLength="10" ID="txtInterviewed" />
                    </div>
                    <div class="col-4">
                        <asp:Label runat="server" AssociatedControlID="txtAssessed" Text="Assessed" />
                        <asp:TextBox runat="server" ClientIDMode="Static" CssClass="form-control" TextMode="Number" MaxLength="10" ID="txtAssessed" />

                    </div>
                    <div class="col-4">
                        <asp:Label runat="server" AssociatedControlID="txtPtrRecommended" Text="PTR Recommended" />
                        <asp:TextBox runat="server" ClientIDMode="Static" CssClass="form-control" TextMode="Number" MaxLength="10" ID="txtPtrRecommended" />
                    </div>
                </div>
                <div class="form-group row">
                    <div class="col-4">
                        <asp:Label runat="server" AssociatedControlID="txtPtrOrdered" Text="PTR Ordered" />
                        <asp:TextBox runat="server" ClientIDMode="Static" CssClass="form-control" TextMode="Number" MaxLength="10" ID="txtPtrOrdered" />
                    </div>

                    <div class="col-4">
                        <asp:Label runat="server" AssociatedControlID="txtIndigentAssessed" Text="Indigent Assessed" />
                        <asp:TextBox runat="server" ClientIDMode="Static" CssClass="form-control" TextMode="Number" MaxLength="10" ID="txtIndigentAssessed" />
                    </div>
                    <div class="col-4">
                        <asp:Label runat="server" AssociatedControlID="txtPtrNotRecommended" Text="PTR Not Recommended" />
                        <asp:TextBox runat="server" ClientIDMode="Static" CssClass="form-control" TextMode="Number" MaxLength="10" ID="txtPtrNotRecommended" />
                        <asp:HiddenField ID="hdLogId" ClientIDMode="Static" runat="server" />
                    </div>
                </div>
                <hr />
                <p>
                    <asp:Button CssClass="btn btn-primary me-2" ID="cmdUpdate" ValidationGroup="intake" runat="server" Text="Save" OnClick="cmdUpdate_Click" />
                    <button type="button" id="cmdCancel" class="btn btn-default me-2 confirm">Cancel</button>
                    <asp:Button CssClass="btn btn-secondary float-end" ID="cmdDeleteIntake" runat="server" Text="Delete" OnClick="cmdDeleteIntake_Click" />

                </p>
                <div id="dialog" title="Delete Record?">
                    Are you sure you wish to delete this Record?
                </div>
            </asp:Panel>
            <div class="modal fade" id="EditDefendantsInProgramModal" tabindex="-1" role="dialog" aria-labelledby="EditDefendantsInProgramModalLabel" aria-hidden="true">
                <div class="modal-dialog modal-lg">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h4 class="modal-title" id="EditDefendantsInProgramModalLabel">Add / Edit Record</h4>
                            <asp:Button OnClientClick="DismissModal()" CausesValidation="False" CssClass="btn-close me-2" aria-hidden="true" ID="cmdClose2" runat="server" Text="&times;" OnClick="cmdClose_Click" />
                        </div>
                        <div class="modal-body">
                            <div class="form-group row">
                                <div class="col-6">
                                    <asp:Label runat="server" AssociatedControlID="txtName" Text="Defendant Name<em>*</em>" ToolTip="required" />
                                    <asp:TextBox runat="server" CssClass="form-control" MaxLength="100" ID="txtName" />
                                    <asp:RequiredFieldValidator runat="server" ValidationGroup="defendant" ControlToValidate="txtName"
                                        Display="Dynamic" SetFocusOnError="true" CssClass="label label-danger" ErrorMessage="Defendant Name is Required" />
                                </div>
                                <div class="col-6">
                                    <asp:Label runat="server" AssociatedControlID="txtFTADate" Text="<abbr title='Failure to Appear'>FTA</abbr> Date" />
                                    <asp:TextBox runat="server" CssClass="form-control datepicker" MaxLength="50" ID="txtFTADate" />
                                </div>
                            </div>
                            <div class="form-group row">
                                <div class="col-6">
                                    <asp:Label runat="server" AssociatedControlID="txtCaseNumber" Text="Case Number<em>*</em>" ToolTip="required" />
                                    <asp:TextBox runat="server" CssClass="form-control" MaxLength="200" ID="txtCaseNumber" />
                                    <asp:RequiredFieldValidator runat="server" ControlToValidate="txtCaseNumber"
                                        Display="Dynamic" SetFocusOnError="true" ValidationGroup="defendant" CssClass="label label-danger" ErrorMessage="Case Number is Required" />
                                </div>
                                <div class="col-6">
                                    <asp:Label runat="server" AssociatedControlID="txtCompletionDate" Text="Completion Date" />
                                    <asp:TextBox runat="server" CssClass="form-control datepicker completion-date" MaxLength="50" ID="txtCompletionDate" />
                                    <asp:CustomValidator ID="valCompletionDate" Display="Dynamic" ValidationGroup="defendant" CssClass="label label-danger" ClientValidationFunction="ValidateCompletionDate" runat="server" ErrorMessage="Complete all Yes/No questions below when entering the completion date"></asp:CustomValidator>
                                </div>
                            </div>
                            <div class="form-group">
                                <asp:Label runat="server" AssociatedControlID="txtCharges" Text="Arrest Charges<em>*</em>" />
                                <asp:TextBox runat="server" TextMode="MultiLine" Rows="3" CssClass="form-control" ID="txtCharges" />
                                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtCharges"
                                    Display="Dynamic" SetFocusOnError="true" ValidationGroup="defendant" CssClass="label label-danger" ErrorMessage="Arrest Charges are Required" />
                            </div>
                            <div class="form-group row">
                                <div class="col-6">
                                    <asp:Label runat="server" AssociatedControlID="txtCourtAppearances" Text="Court Appearances" />
                                    <asp:TextBox runat="server" CssClass="form-control" TextMode="Number" MaxLength="10" ID="txtCourtAppearances" />
                                </div>
                                <div class="col-6">
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
                            <div class="row">
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
                            <div class="form-group mt-3 ps-3 pe-3">
                                <div class="row align-items-center">
                                    <div class="col-6">
                                        <asp:Label runat="server" CssClass="col-form-label" AssociatedControlID="rblFtaArrestHearing" Text="FTA Arrest Any hearing?" />
                                        <asp:RadioButtonList ID="rblFtaArrestHearing" runat="server" RepeatLayout="Flow" CssClass="radio-button-list fta-hearing" RepeatDirection="Horizontal">
                                            <asp:ListItem Text="Yes" Value="1" />
                                            <asp:ListItem Text="No" Value="0" />
                                        </asp:RadioButtonList>
                                    </div>
                                    <div class="col-6">
                                        <asp:Label runat="server"  CssClass="col-form-label" AssociatedControlID="rblBwOrdered" Text="Bench Warrant Ordered?" />
                                        <asp:RadioButtonList ID="rblBwOrdered" runat="server" RepeatLayout="Flow" CssClass="radio-button-list bw-ordered" RepeatDirection="Horizontal">
                                            <asp:ListItem Text="Yes" Value="1" />
                                            <asp:ListItem Text="No" Value="0" />
                                        </asp:RadioButtonList>
                                    </div>


                                    <div class="col-6">
                                        <asp:Label runat="server"  CssClass="col-form-label" AssociatedControlID="rblCompletion" Text="Successful Completion?" />
                                        <asp:RadioButtonList ID="rblCompletion" runat="server" RepeatLayout="Flow" CssClass="radio-button-list completion" RepeatDirection="Horizontal">
                                            <asp:ListItem Text="Yes" Value="1" />
                                            <asp:ListItem Text="No" Value="0" />
                                        </asp:RadioButtonList>

                                    </div>
                                    <div class="col-6">
                                        <asp:Label runat="server"  CssClass="col-form-label" AssociatedControlID="rblIndigent" Text="Indigent?" />
                                        <asp:RadioButtonList ID="rblIndigent" runat="server" RepeatLayout="Flow" CssClass="radio-button-list indigent" RepeatDirection="Horizontal">
                                            <asp:ListItem Text="Yes" Value="1" />
                                            <asp:ListItem Text="No" Value="0" />
                                        </asp:RadioButtonList>
                                    </div>
                                    <div class="form-check col-6 ms-2">
                                        <asp:CheckBox ID="chkRevoked" runat="server" Text="Revoked?" />
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="modal-footer">
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
            <asp:AsyncPostBackTrigger ControlID="cmdUpdate" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="cmdSearch" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="cmdClose" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="cmdClose2" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="drpYear" EventName="SelectedIndexChanged" />
            <asp:AsyncPostBackTrigger ControlID="drpMonth" EventName="SelectedIndexChanged" />
            <asp:AsyncPostBackTrigger ControlID="drpDay" EventName="SelectedIndexChanged" />
            <asp:AsyncPostBackTrigger ControlID="cmdDeleteIntake" EventName="Click" />
        </Triggers>

    </asp:UpdatePanel>

</div>
<dnn:dnncssinclude runat="server" filepath="~/Resources/Libraries/jQuery-UI/01_13_02/Themes/jquery-ui.css" />
<dnn:dnnjsInclude runat="server" FilePath="https://cdn.datatables.net/v/bs5/dt-1.13.1/datatables.min.js" />
<dnn:dnncssInclude runat="server" FilePath="https://cdn.datatables.net/v/bs5/dt-1.13.1/datatables.min.css" />


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
            "order": [[1, "asc"]],
            "oLanguage": {
                "sSearch": "Filter by Text"
            },
        });

        $("#tblDefendantsInProgram_length").prepend('<button class="btn btn-primary btn-lg me-2" data-bs-toggle="modal" data-bs-target="#EditDefendantsInProgramModal"><i class="fa fa-plus"></i>&nbsp;Add New Record</button>');
        table.draw();

        $(".confirm").dnnConfirm({

            text: 'Are you sure you wish to delete this Record?',

            yesText: 'Yes',

            noText: 'No',

            title: 'Delete Record?'

        });

        $("#<%=cmdDeleteIntake.ClientID%>").click(function (e) {
            e.preventDefault();
            $("#dialog").dialog({
                resizable: false,
                height: "auto",
                width: 400,
                modal: true,
                buttons: {
                    "Confirm": function () {
                        __doPostBack('<%= cmdDeleteIntake.UniqueID %>', '');
                        $(this).dialog("close");
                    },
                    Cancel: function () {
                        e.preventDefault();
                        $(this).dialog("close");
                    }
                }
            });
            $("#dialog").dialog("open");
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
                   // __doPostBack('<%= cmdDeleteIntake.UniqueID %>', '');
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
    function ClearIntakeForm() {
        if ($("#hdLogId").val() != "") {
            $("#txtInterviewed").val("");
            $("#txtAssessed").val("");
            $("#txtPtrRecommended").val("");
            $("#txtPtrOrdered").val("");
            $("#txtIndigentAssessed").val("");
            $("#txtPtrNotRecommended").val("");
            $("#hdLogId").val("");
        }
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
    function ValidateCompletionDate(sender, args) {
        args.IsValid = true;
        var indigent = $('.indigent input:checked').val();
        var bwordered = $('.bw-ordered input:checked').val();
        var completion = $('.completion input:checked').val();
        var ftaHearing = $('.fta-hearing input:checked').val();
        var completionDate = $('.completion-date').val();
        if (completionDate != "" & (indigent == undefined | bwordered == undefined | completion == undefined | ftaHearing == undefined)) {
            args.IsValid = false;
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
