<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="LogEdit.ascx.cs" Inherits="tjc.Modules.CourtCounsel.LogEdit" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<asp:Literal ID="ltMessage" runat="server" />

<div typeof="post" id="assignment-form">
    <fieldset>
        <legend>Add / Edit Log Entry</legend>
        <nav class="navbar navbar-expand-lg navbar-dark bg-dark mb-md">
            <button class="navbar-toggler" type="button" data-toggle="collapse" data-target="#navbarNav" aria-controls="navbarNav" aria-expanded="false" aria-label="Toggle navigation">
                <span class="navbar-toggler-icon"></span>
            </button>
            <div class="collapse navbar-collapse" id="navbarNav">
                <ul class="navbar-nav">

                    <li class="nav-item">
                        <asp:HyperLink CssClass="nav-link" ID="lnkSearch" runat="server"><i class="fas fa-search"></i>&nbsp;Search</asp:HyperLink>
                    </li>
                    <li class="active nav-item">
                        <a class="nav-link" href="<%=EditUrl("logEdit") %>"><i class="fas fa-pencil-alt"></i>&nbsp;Data Entry</a>
                    </li>
                    <li class="nav-item">
                        <a class="nav-link" href="<%=EditUrl("reports") %>"><i class="fas fa-chart-bar"></i>&nbsp;Reports</a>
                    </li>
                    <li class="nav-item" id="li1" runat="server" visible="false">
                        <a class="nav-link" href="<%=EditUrl("admin") %>"><i class="fa fa-tools"></i>&nbsp;Admin</a>
                    </li>
                    <li class="nav-item">
                        <a class="nav-link" href="<%=SharePointSiteURL %>"><i class="fas fa-home"></i>&nbsp;Team Site</a>
                    </li>
                </ul>

            </div>
        </nav>
        <div class="row form-group">
            <div class="col-md-5">
                <asp:Label runat="server" AssociatedControlID="drpCountyLetter" Text="Case Number<em>*</em>" ToolTip="required" />
                <div class="input-group">
                    <asp:DropDownList ID="drpCountyLetter" runat="server" title="County" CssClass="form-control" ClientIDMode="Static">
                        <asp:ListItem Text="" Value=""></asp:ListItem>
                        <asp:ListItem Text="D" Value="D" title="DeSoto"></asp:ListItem>
                        <asp:ListItem Text="M" Value="M" title="Manatee"></asp:ListItem>
                        <asp:ListItem Text="S" Value="S" title="Sarasota"></asp:ListItem>
                        <asp:ListItem Text="V" Value="V" title="Venice"></asp:ListItem>
                    </asp:DropDownList>
                    <asp:TextBox ID="txtCaseYear" title="Year" runat="server" MaxLength="4" CssClass="form-control" placeholder="YYYY" ClientIDMode="Static"></asp:TextBox>
                    <asp:TextBox ID="txtCaseType" title="Case Type" runat="server" MaxLength="2" CssClass="form-control upperCase" placeholder="CT" ClientIDMode="Static"></asp:TextBox>
                    <asp:TextBox ID="txtCaseSequence" title="Case Sequence" runat="server" MaxLength="25" CssClass="form-control upperCase" placeholder="000000" ClientIDMode="Static"></asp:TextBox>
                    <div class="input-group-append">
                        <small class="input-group-text form-control" title="County - Year - Case Type - Case Sequence">(Format: C-YYYY-CT-<span id="caseFormat">000000</span>)</small>
                    </div>
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="drpCountyLetter"
                        Display="Dynamic" SetFocusOnError="true" CssClass="label label-danger" ErrorMessage="County is Required" />
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="txtCaseYear"
                        Display="Dynamic" SetFocusOnError="true" CssClass="label label-danger" ErrorMessage="Case Year is Required" />
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="txtCaseType"
                        Display="Dynamic" SetFocusOnError="true" CssClass="label label-danger" ErrorMessage="Case Type is Required" />
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="txtCaseSequence"
                        Display="Dynamic" SetFocusOnError="true" CssClass="label label-danger" ErrorMessage="Case Sequence is Required" />
                    <asp:CustomValidator ID="valCaseNumber" runat="server" SetFocusOnError="true" CssClass="label label-danger" ControlToValidate="txtCaseSequence"
                        Display="Dynamic" ErrorMessage="Invalid Case Number. Please Review Format Requirements" OnServerValidate="valCaseNumber_ServerValidate" ClientValidationFunction="ValidateCaseNumber">
                    </asp:CustomValidator>
                </div>
            </div>
            <div class="col-md-5">
                <asp:Label runat="server" AssociatedControlID="txtCaseName" Text="Case Name<em>*</em>" ToolTip="required" />
                <asp:TextBox ID="txtCaseName" runat="server" MaxLength="100" CssClass="form-control" placeholder="Party One v. Party Two" ClientIDMode="Static"></asp:TextBox>
                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtCaseName"
                    Display="Dynamic" SetFocusOnError="true" CssClass="label label-danger" ErrorMessage="Case Name is Required" />

            </div>
            <div class="col-md-2">
                <asp:Label runat="server" AssociatedControlID="drpCounty" Text="County<em>*</em>" ToolTip="required" />
                <asp:DropDownList ID="drpCounty" runat="server" CssClass="form-control" required="required" AppendDataBoundItems="true" ClientIDMode="Static">
                    <asp:ListItem Text="< Select Option >" Value=""></asp:ListItem>
                </asp:DropDownList>
                <asp:RequiredFieldValidator runat="server" ControlToValidate="drpCounty" ErrorMessage="County is Required"
                    Display="Dynamic" SetFocusOnError="true" CssClass="label label-danger" />
            </div>
        </div>
        <div class="row  form-group">
            <div class="col-auto">
                <asp:Label runat="server" AssociatedControlID="drpCaseType" Text="Select Case Type<em>*</em>" ToolTip="required" />
                <asp:DropDownList ID="drpCaseType" runat="server" CssClass="form-control auto-size">
                </asp:DropDownList>
                <asp:RequiredFieldValidator runat="server" ControlToValidate="drpCaseType"
                    Display="Dynamic" SetFocusOnError="true" CssClass="label label-danger" ErrorMessage="Please Select the Case Type" />
            </div>
            <div class="col-auto">
                <asp:Label runat="server" AssociatedControlID="txtAssignedDate" Text="Assigned Date<em>*</em>" ToolTip="required" />
                <asp:TextBox runat="server" CssClass="form-control auto-size datepicker"  ClientIDMode="Static" ID="txtAssignedDate" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtAssignedDate"
                    Display="Dynamic" SetFocusOnError="true" CssClass="label label-danger" ErrorMessage="Assigned Date is Required" />
            </div>
            <div class="col-auto">

                <asp:Label runat="server" AssociatedControlID="txtMotionFiled" Text="Motion Filed<em>*</em>" ToolTip="required" />
                <asp:TextBox runat="server" CssClass="form-control auto-size datepicker"  ID="txtMotionFiled" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtMotionFiled"
                    Display="Dynamic" SetFocusOnError="true" CssClass="label label-danger" ErrorMessage="Motion Filed is Required" />
            </div>
            <div class="col-auto">
                <asp:Label runat="server" AssociatedControlID="drpRequestedBy" Text="Requested By<em>*</em>" ToolTip="required" />
                <asp:DropDownList ID="drpRequestedBy" runat="server" CssClass="form-control auto-size" required="required" ClientIDMode="Static">
                    <asp:ListItem Text="< Select Option >" Value=""></asp:ListItem>
                </asp:DropDownList>
                <asp:RequiredFieldValidator runat="server" ControlToValidate="drpRequestedBy" ErrorMessage="Requested by is Required"
                    Display="Dynamic" SetFocusOnError="true" CssClass="label label-danger" />

            </div>
            <div class="col-auto">
                <asp:Label runat="server" AssociatedControlID="drpResponsible" Text="Responsible<em>*</em>" ToolTip="required" />
                <asp:DropDownList ID="drpResponsible" runat="server" CssClass="form-control auto-size" required="required" ClientIDMode="Static">
                    <asp:ListItem Text="< Select Option >" Value=""></asp:ListItem>
                </asp:DropDownList>
                <asp:RequiredFieldValidator runat="server" ControlToValidate="drpResponsible" ErrorMessage="Responsible is Required"
                    Display="Dynamic" SetFocusOnError="true" CssClass="label label-danger" />
            </div>
        </div>
        <div class="row  form-group">

            <div class="col-auto">
                <asp:Label runat="server" AssociatedControlID="drpActionTaken" Text="Action Taken" />
                <asp:DropDownList ID="drpActionTaken" runat="server" CssClass="form-control auto-size">
                </asp:DropDownList>

            </div>
            <div class="col-auto">
                <asp:Label runat="server" AssociatedControlID="drpTimeSpent" Text="Time Spent" />
                <asp:DropDownList ID="drpTimeSpent" runat="server" CssClass="form-control auto-size">
                    <asp:ListItem Text="< Select Time Spent >" Value=""></asp:ListItem>
                </asp:DropDownList>

            </div>
            <div class="col-auto">
                <asp:Label runat="server" AssociatedControlID="txtDateCompleted" Text="Date Completed Filed" />
                <asp:TextBox runat="server" CssClass="form-control auto-size datepicker"  ID="txtDateCompleted" />
            </div>
            <div class="col-auto">
                <asp:Label runat="server" AssociatedControlID="drpStatus" Text="Status" />
                <asp:DropDownList ID="drpStatus" runat="server" CssClass="form-control auto-size" ClientIDMode="Static">
                    <asp:ListItem Text="< Select Option >" Value=""></asp:ListItem>
                </asp:DropDownList>
            </div>
        </div>
        <div class="row  form-group">
            <div class="col-md-12">
                <asp:Label runat="server" AssociatedControlID="txtComments" Text="Comments" />
                <asp:TextBox runat="server" CssClass="form-control" ID="txtComments" TextMode="MultiLine" Rows="4" />

            </div>
        </div>
        <div class="form-check mb-2">
            <asp:CheckBox Text="Prevent Judge Reassignment" ID="chkReassign" Visible="false" runat="server" />
        </div>
        <asp:HiddenField ID="hdLogId" runat="server" ClientIDMode="Static" />
    </fieldset>
    <p>
        <asp:Button ID="cmdSave" runat="server" CssClass="btn btn-primary" Text="Submit" OnClick="cmdSave_Click" />
        <asp:HyperLink ID="lnkCancel" Text="Cancel" CssClass="btn btn-default" runat="server" />
        <button type="button" id="cmdChangeCaseInfo" class="btn btn-tertiary pull-right">Change Case Info</button>
        <asp:HiddenField ID="hdCaseInfoChanged" runat="server" ClientIDMode="Static" />
    </p>
</div>

<div class="modal fade" id="reassignModal" tabindex="-1" role="dialog" aria-labelledby="reassignModalLabel" aria-hidden="true">
    <div class="modal-dialog">
        <div class="modal-content">
            <div class="modal-header">
                <h4 class="modal-title" id="reassignModalLabel">Reassignment Reason</h4>
                <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
            </div>
            <div class="modal-body">
                <div class="form-group">
                    <asp:Label runat="server" AssociatedControlID="txtReason" Text="Reason for Judge Reassingment" />
                    <asp:TextBox runat="server" CssClass="form-control" ID="txtReason" TextMode="MultiLine" Rows="4" ClientIDMode="Static" />

                </div>
            </div>
            <div class="modal-footer">
                <button type="button" class="btn btn-reassign" data-dismiss="modal">Close</button>
            </div>
        </div>
    </div>
</div>
<div class="modal fade" id="caseListModal" tabindex="-1" role="dialog" aria-labelledby="caseListModalLabel" aria-hidden="true">
    <div class="modal-dialog">
        <div class="modal-content">
            <div class="modal-header">
                <h4 class="modal-title" id="caseListModalLabel">Matching Case Number Results</h4>
                <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
            </div>
            <div class="modal-body">
                <table id="caseList" class="table table-striped">
                    <thead>
                        <tr>
                            <th class="command-icon">&nbsp;</th>
                            <th>Case Number</th>
                            <th>Case Name</th>
                        </tr>
                    </thead>
                    <tbody id="caseListBody">
                    </tbody>
                </table>
            </div>
            <div class="modal-footer">
                <button type="button" class="btn btn-caseList" data-dismiss="modal">Close</button>
            </div>
        </div>
    </div>
</div>
<div class="modal fade" id="statusModal" tabindex="-1" role="dialog" aria-labelledby="statusModalLabel" aria-hidden="true">
    <div class="modal-dialog">
        <div class="modal-content">
            <div class="modal-header">
                <h4 class="modal-title">Status Change</h4>
                <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>

            </div>
            <div class="modal-body">
                <p>The selected status should be assigned a future date. Unless you are fixing an incorrect status, please change the assigned date. </p>
            </div>
            <div class="modal-footer">
                <button type="button" class="btn btn-default" data-dismiss="modal">Close</button>
            </div>
        </div>
    </div>
</div>
<dnn:dnnjsinclude runat="server" filepath="https://cdn.datatables.net/1.12.1/js/jquery.dataTables.min.js" />
<dnn:dnnjsinclude runat="server" filepath="https://cdn.datatables.net/1.12.1/js/dataTables.bootstrap5.min.js" />
<dnn:dnncssinclude runat="server" filepath="https://cdn.datatables.net/1.12.1/css/dataTables.bootstrap5.min.css" />
<dnn:dnncssinclude runat="server" filepath="~/Resources/Shared/components/TimePicker/Themes/jquery-ui.min.css" />
<dnn:dnnjsinclude runat="server" filepath="https://cdnjs.cloudflare.com/ajax/libs/jquery.mask/1.14.16/jquery.mask.js" />



<script>
    var originalJudge = "";
    var moduleId = <%=ModuleId%>;
    (function ($, Sys) {
        $(document).ready(function () {

            Sys.Application.add_load(function (s, e) { PageInit(); });
            PageInit();
        });
    }(jQuery, window.Sys));
    function parseDate(s) {
        var b = s.split(/\D/);
        return new Date(b[0], --b[1], b[2]);
    }
    function PageInit() {
        originalJudge = $("#drpRequestedBy").val();
        $(".datepicker").datepicker();
        $(".form-check input:checkbox").addClass("form-check-input");
        $(".form-check label").addClass("form-check-label");
        if ($("#txtCaseSequence").val() == "") {
            $("#txtCaseSequence").mask("000000");
        }
        $(".fade-alert").fadeTo(2000, 500).slideUp(500, function () {
            $(".fade-alert").slideUp(500);
        });
        $("#txtAssignedDate").on("blur", function () {
            var assignedDate = $(this).val()
            if (assignedDate != "") {
                $("#txtStartDate").val(assignedDate);
            }
        });
        $("#drpStatus").on("change", function () {
            var assignedDate = parseDate($("#txtAssignedDate").val());
            var pending = $(this).find(':selected').data('pending');
            if (pending == 1 && Date.now() >= assignedDate) {
                $("#statusModal").modal("show");
            }
        });
        $("#cmdChangeCaseInfo").on("click", function (e) {
            e.preventDefault();
            if ($("#drpCountyLetter").disable) {
                $(this).html("Change Case Info");
            } else {
                $(this).html("Cancel Case Change");
            }
            $("#drpCountyLetter").prop("disabled", (i, v) => !v);
            $("#txtCaseYear").prop("disabled", (i, v) => !v);
            $("#txtCaseType").prop("disabled", (i, v) => !v);
            $("#txtCaseSequence").prop("disabled", (i, v) => !v);
            $("#drpCounty").prop("disabled", (i, v) => !v);
            $("#txtCaseName").prop("disabled", (i, v) => !v);
            $("#hdCaseInfoChanged").val("1");
        });
        $("#cmdAddEvent").on("click", function (e) {
            $("#txtStartDate").val('');
            $("#txtSubject").val("");
            $("#txtBody").val('');
            $("#txtReminderDays").val('10');
            $("#hdExternalId").val('');
        });
        $("#txtCaseType").on("input", function (e) {
            var caseType = $(this).val();
            MaskCaseSequence(caseType);
        });
        $("#drpRequestedBy").on("change", function () {
            if ($(this).val() != originalJudge && originalJudge != "") {
                $("#reassignModal").modal("show");
            }
        });
        $("#drpCountyLetter").on("change", function () {
            var caseType = $("#txtCaseType").val();
            if (caseType.length > 1)
                MaskCaseSequence(caseType);
        });
        $(".upperCase").on("input", function (evt) {
            $(this).val(function (_, val) {
                return val.toUpperCase();
            });
        });
        InitializeStatusDropDown();
        InitializeResponsibleDropDown();
        InitializeRequestedByDropDown();
        var table = $("#log-list").DataTable({

            "order": [[3, "desc"]],
            "oLanguage": {

                "sSearch": "Filter by Text"

            },
        });
    }
    function PopulateCaseInformation(data) {
        $('#txtCaseSequence').prop("disabled", true);
        $("#txtCaseType").prop("disabled", true);
        $("#drpCountyLetter").prop("disabled", true);
        $("#txtCaseYear").prop("disabled", true);
        $("#drpCounty").val(data.countyId).prop("disabled", true);
        $("#txtCaseName").val(data.description).prop("disabled", true);
        $("#hdLogId").val(data.logId);
    }

    function MaskCaseSequence(caseType) {
        var location = $("#drpCountyLetter").find(":selected").val();
        if (caseType.toUpperCase() == "CF") {
            if (location.toUpperCase() == "S") {
                $("#txtCaseSequence").mask("000000-0000");
                $("#caseFormat").text("000000-0000");
                $("#txtCaseSequence").attr("placeholder", "000000-0000");
            } else {
                $("#txtCaseSequence").mask("YYYYYY-AA", {
                    "translation": {
                        A: { pattern: /[A-Za-z]/ },
                        Y: { pattern: /[0-9]/ }
                    }
                });
                $("#caseFormat").text("000000-AA");
                $("#txtCaseSequence").attr("placeholder", "000000-AA");
            }
        } else {
            $("#txtCaseSequence").mask("000000");
            $("#caseFormat").text("000000");
            $("#txtCaseSequence").attr("placeholder", "000000");
        }
    }
    function GetCaseNumber() {
        var caseCounty = $("#drpCountyLetter").val();
        var caseYear = $("#txtCaseYear").val();
        var caseType = $("#txtCaseType").val();
        var caseSequence = $("#txtCaseSequence").val();
        return caseCounty + "-" + caseYear + "-" + caseType + "-" + caseSequence;
    }
    function InitializeStatusDropDown() {
        var $select = $("#drpStatus");
        var currentSelection = $select.val();
        var optGroup;
        $("#drpStatus option").each(function () {
            if ($(this).val() == "<") {
                /* Opener */
                optGroup = $("<optGroup>").attr("label", $(this).text());
            } else if ($(this).val() == ">") {
                /* Closer */
                $("</optGroup>").appendTo(optGroup);
                optGroup.appendTo($select);
                optGroup = null;
            } else {
                /* Normal Item */
                if (optGroup) {
                    $("<option class='inactive' data-pending='" + $(this).data("pending") + "'>" + $(this).text() + "</option>").attr("value", $(this).val()).appendTo(optGroup);
                } else {
                    $("<option data-pending='" + $(this).data("pending") + "'>" + $(this).text() + "</option>").attr("value", $(this).val()).appendTo($select);
                }
            }
            $(this).remove();
        });
        $select.val(currentSelection);
    }
    function InitializeResponsibleDropDown() {
        var $select = $("#drpResponsible");
        var currentSelection = $select.val();
        var optGroup;
        $("#drpResponsible option").each(function () {
            if ($(this).val() == "<") {
                /* Opener */
                optGroup = $("<optGroup>").attr("label", $(this).text());
            } else if ($(this).val() == ">") {
                /* Closer */
                $("</optGroup>").appendTo(optGroup);
                optGroup.appendTo($select);
                optGroup = null;
            } else {
                /* Normal Item */
                if (optGroup) {
                    $("<option class='inactive'>" + $(this).text() + "</option>").attr("value", $(this).val()).appendTo(optGroup);
                } else {
                    $("<option>" + $(this).text() + "</option>").attr("value", $(this).val()).appendTo($select);
                }
            }
            $(this).remove();
        });
        $select.val(currentSelection);
    }
    function InitializeRequestedByDropDown() {
        var $select = $("#drpRequestedBy");
        var currentSelection = $select.val();
        var optGroup;
        $("#drpRequestedBy option").each(function () {
            if ($(this).val() == "<") {
                /* Opener */
                optGroup = $("<optGroup>").attr("label", $(this).text());
            } else if ($(this).val() == ">") {
                /* Closer */
                $("</optGroup>").appendTo(optGroup);
                optGroup.appendTo($select);
                optGroup = null;
            } else {
                /* Normal Item */
                if (optGroup) {
                    $("<option class='inactive'>" + $(this).text() + "</option>").attr("value", $(this).val()).appendTo(optGroup);
                } else {
                    $("<option>" + $(this).text() + "</option>").attr("value", $(this).val()).appendTo($select);
                }
            }
            $(this).remove();
        });
        $select.val(currentSelection);
    }
    //Validators
    function ValidateCaseNumber(sender, args) {
        var isValid = $("#txtCaseSequence").prop('disabled');
        if (isValid) {
            args.IsValid = true;
            return;
        }
        var errorMessage = "";
        var caseNumber = GetCaseNumber();
        if (caseNumber === "" || caseNumber === null) {
            isValid = false
            errorMessage = "Case Number is Required"
        } else if (caseNumber.length < 16) {
            isValid = false
            errorMessage =
                "Case Number must be 16 characters in the format (C-YYYY-CT-000000)"
        } else if (
            caseNumber.startsWith("S") &&
            caseNumber.indexOf("CF") > 1 && caseNumber.length < 21
        ) {
            isValid = false
            errorMessage =
                "Case Number must include party sequence for CF cases in the format (C-YYYY-CT-000000-0000)"
        } else if (
            !caseNumber.startsWith("S") &&
            caseNumber.indexOf("CF") > 1 &&
            caseNumber.length < 19
        ) {
            isValid = false
            errorMessage =
                "Case Number must include party sequence for CF cases in the format (C-YYYY-CT-000000-XX)"
        }
        if (errorMessage == "") { isValid = true; }
        sender.innerHTML = errorMessage;
        args.IsValid = isValid;
        var logId = $("#hdLogId").val()
        if (isValid && logId.length === 0)
            RetrieveLogEntryByCaseNumber(caseNumber);
    }

    function RetrieveLogEntryByCaseNumber(caseNumber) {
        var service = {
            path: "CourtCounsel",
            framework: $.ServicesFramework(moduleId)
        }
        service.baseUrl = service.framework.getServiceRoot(service.path);
        var restUrl = service.baseUrl + "LogEntry/GetLogEntryByCaseNumber/" + caseNumber;
        try {

            $.ajax({
                url: restUrl,
                beforeSend: service.framework.setModuleHeaders,
                dataType: "json"
            }).done(function (data) {
                if (data) {
                    if (data.length > 0) {
                        if (data.length == 1) {
                            PopulateCaseInformation(data[0]);
                        } else {
                            PopulateCaseList(data);
                        }
                    } else {
                        //No Case Found
                    }
                }
                else {
                    //no data
                }
            }).always(function (data) {
                //close spinner
            });
        } catch (e) {
            alert("Unexpected error searching for case number.\n\nMake sure you are logged in and try again.");
        }
        return false;
    }
    function PopulateCaseList(data) {
        var tableBody = document.getElementById('caseListBody');
        $('#caseList > tbody > tr').remove();
        data.forEach(function (object) {
            var tr = document.createElement('tr');
            tr.innerHTML = `<td><a class="command-icon" title="Select This Log Entry" onclick="SelectCase(${object.logId},${object.countyId},'${object.description}',event)"><i class="fa fa-check-circle"></i></a></td><td>${object.caseNumber}</td><td>${object.description}</td>`;
            tableBody.appendChild(tr);
        });
        $('#caseListModal').modal('show');
    }
    function SelectCase(logid, countyId, caseName, e) {
        e.preventDefault();
        var obj = { "logId": logid, "countyId": countyId, "description": caseName };
        PopulateCaseInformation(obj);
        $('#caseListModal').modal('hide');
    }
</script>
