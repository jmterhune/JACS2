<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="View.ascx.cs" Inherits="tjc.Modules.MediationStatistics.View" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<div class="tabs">
    <ul class="nav nav-tabs">
        <li class="nav-item active">
            <a class="nav-link" href="#caseSearch" data-toggle="tab">Add / Search Cases</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=ReportUrl%>">Reports</a>
        </li>
        <li class="nav-item" id="lists" style="display: hidden">
            <a class="nav-link" href="<%=AttorneyListUrl%>">Manage Lists</a>
        </li>
    </ul>
    <div class="tab-content pb-0">
        <div id="caseSearch" class="tab-pane active">
            <div class="toggle toggle-quaternary mb-0" data-plugin-toggle="toggle">
                <section class="toggle active">
                    <button class="toggle-heading" type="button">Expand for Search / Add Case</button>
                    <div class="toggle-content">
                        <asp:ValidationSummary ValidationGroup="CaseSearch" CssClass="med_ErrorList" ID="valSummaryCaseSearch"
                            runat="server" DisplayMode="BulletList" HeaderText="Please Correct the Following Issues" />
                        <asp:ValidationSummary ValidationGroup="CaseNew" CssClass="med_ErrorList" ID="valSummaryCaseNew"
                            runat="server" DisplayMode="BulletList" HeaderText="When Creating a New Case" />

                        <fieldset class="outline-fieldset">
                            <legend>Case Style Options</legend>
                            <div class="form-group">
                                <div class="row">
                                    <div class="col-md-4">
                                        <asp:Label runat="server" AssociatedControlID="drpRegion" Text="Region" />
                                        <asp:DropDownList runat="server" CssClass="form-control" ID="drpRegion" ClientIDMode="Static" AppendDataBoundItems="true">
                                            <asp:ListItem Text="< Select Region >" Value="" />
                                        </asp:DropDownList>
                                        <asp:RequiredFieldValidator ValidationGroup="CaseSearch" ID="valRegion" Display="Dynamic"
                                            SetFocusOnError="true" CssClass="label label-danger" ControlToValidate="drpRegion" runat="server" ErrorMessage="Region is Required"></asp:RequiredFieldValidator>
                                    </div>
                                    <div class="col-md-4">
                                        <asp:Label runat="server" AssociatedControlID="drpGroup" Text="Case Type Group" />
                                        <asp:DropDownList runat="server" CssClass="form-control" ID="drpGroup" ClientIDMode="Static">
                                            <asp:ListItem Text="< Select Type >" Value="" />
                                        </asp:DropDownList>
                                        <asp:RequiredFieldValidator ValidationGroup="CaseSearch" ID="valCaseType" Display="Dynamic"
                                            SetFocusOnError="true" CssClass="label label-danger" ControlToValidate="drpGroup" runat="server" ErrorMessage="Case Type is required"></asp:RequiredFieldValidator>
                                    </div>
                                </div>
                            </div>
                            <div class="form-group">
                                <div class="row">
                                    <div class="col-md-6">
                                        <asp:Label runat="server" AssociatedControlID="txtCaseYear" Text="Case Number" />
                                        <div class="input-group">
                                            <asp:TextBox AutoCompleteType="Disabled" ID="txtCaseYear" title="Year" runat="server" MaxLength="4" CssClass="form-control year-field" placeholder="YYYY" ClientIDMode="Static"></asp:TextBox>
                                            <asp:TextBox AutoCompleteType="Disabled" ID="txtCaseType" title="Case Type" runat="server" MaxLength="2" CssClass="form-control upperCase case-code-field" placeholder="CC" ClientIDMode="Static"></asp:TextBox>
                                            <asp:TextBox AutoCompleteType="Disabled" ID="txtCaseSequence" title="Case Sequence" runat="server" MaxLength="6" CssClass="form-control upperCase" placeholder="000000" ClientIDMode="Static"></asp:TextBox>
                                            <asp:TextBox AutoCompleteType="Disabled" ID="txtSuffix" title="Suffix" runat="server" MaxLength="4" CssClass="form-control upperCase case-code-field" ClientIDMode="Static"></asp:TextBox>
                                            <div class="input-group-append">
                                                <small class="input-group-text form-control" title="Year - Case Type - Case Sequence - Suffix">(Format: YYYY-CC-000000-NC)</small>
                                            </div>
                                        </div>

                                    </div>
                                    <div class="col-md-6">
                                        <asp:Label runat="server" AssociatedControlID="drpCountyLetter" Text="CDSP Number" />
                                        <div class="input-group">
                                            <asp:DropDownList runat="server" CssClass="form-control" ID="drpCDSPType" ClientIDMode="Static">
                                                <asp:ListItem Text="< Select Type >" Value="" />
                                                <asp:ListItem Text="CDSP" />
                                                <asp:ListItem Text="CDSPF" />
                                            </asp:DropDownList>
                                            <asp:TextBox AutoCompleteType="Disabled" ID="txtCDSPYear" title="Year" runat="server" MaxLength="4" CssClass="form-control year-field" placeholder="YYYY" ClientIDMode="Static"></asp:TextBox>
                                            <asp:TextBox AutoCompleteType="Disabled" ID="txtCDSPNumber" title="Case Type" runat="server" MaxLength="3" CssClass="form-control upperCase" placeholder="000" ClientIDMode="Static"></asp:TextBox>
                                            <asp:DropDownList ID="drpCountyLetter" runat="server" title="County" CssClass="form-control location-field" ClientIDMode="Static">
                                                <asp:ListItem Text="" Value=""></asp:ListItem>
                                                <asp:ListItem Text="D" Value="D" title="DeSoto"></asp:ListItem>
                                                <asp:ListItem Text="M" Value="M" title="Manatee"></asp:ListItem>
                                                <asp:ListItem Text="S" Value="S" title="Sarasota"></asp:ListItem>
                                                <asp:ListItem Text="V" Value="V" title="Venice"></asp:ListItem>
                                            </asp:DropDownList>
                                            <div class="input-group-append">
                                                <small class="input-group-text form-control" title="Type - Year - Number - Location">(Format: CDSP-YYYY-000-C)</small>
                                            </div>
                                        </div>

                                    </div>
                                </div>
                            </div>
                            <asp:CustomValidator ID="valCustom" runat="server" ErrorMessage="You must enter a Case Number or CDSP Number."
                                ClientValidationFunction="CheckCaseNumber" Display="Dynamic"
                                CssClass="label label-danger" ValidationGroup="CaseNew"></asp:CustomValidator>

                        </fieldset>
                        <fieldset class="outline-fieldset">
                            <legend>Other Search Options</legend>
                            <div class="form-group">
                                <div class="row">
                                    <div class="col-md-4">
                                        <asp:Label runat="server" AssociatedControlID="txtLastName" Text="Last Name" />
                                        <asp:TextBox AutoCompleteType="Disabled" runat="server" ID="txtLastName" MaxLength="50" ClientIDMode="Static" CssClass="form-control" />
                                    </div>
                                    <div class="col-md-4">
                                        <asp:Label runat="server" AssociatedControlID="txtFirstName" Text="First Name" />
                                        <asp:TextBox AutoCompleteType="Disabled" runat="server" ID="txtFirstName" MaxLength="50" ClientIDMode="Static" CssClass="form-control" />
                                    </div>
                                    <div class="col-md-4">
                                        <asp:Label runat="server" AssociatedControlID="txtBusinessName" Text="Business Name" />
                                        <asp:TextBox AutoCompleteType="Disabled" runat="server" ID="txtBusinessName" MaxLength="50" ClientIDMode="Static" CssClass="form-control" />
                                    </div>
                                </div>
                            </div>
                        </fieldset>
                        <p>
                            <button type="button" class="btn btn-primary me-2" id="cmdSearch">Search</button>
                            <asp:HyperLink runat="server" ID="lnkReset" CssClass="btn btn-secondary">Reset</asp:HyperLink>
                            <asp:Button Text="Add Case" OnClientClick="return fnJSOnFormSubmit()" CssClass="btn btn-success float-end" ValidationGroup="CaseNew" ID="cmdAddCase" OnClick="cmdAddCase_Click" runat="server" />
                        </p>
                    </div>
                </section>
            </div>
            <asp:Literal ID="ltMessage" runat="server"></asp:Literal>
        </div>
    </div>

</div>
<div class="ms-2 me-3">
<table id="tblCases" class="table table-striped">
    <thead>
        <tr>
            <th>&nbsp;</th>
            <th>Case Number</th>
            <th>Region</th>
            <th>Case Type</th>
            <th>Party One</th>
            <th>Party Two</th>
            <th>Created</th>
            <th>&nbsp;</th>
            <th>&nbsp;</th>
        </tr>
    </thead>
</table>

</div>
<dnn:dnncssinclude runat="server" filepath="~/Resources/Shared/components/TimePicker/Themes/jquery-ui.min.css" />
<dnn:dnncssinclude runat="server" filepath="/Resources/Libraries/DataTables/dataTables.bootstrap5.min.css" />
<dnn:dnnjsinclude runat="server" filepath="/Resources/Libraries/DataTables/dataTables.min.js" />
<dnn:dnnjsinclude runat="server" filepath="/Resources/Libraries/DataTables/dataTables.bootstrap5.min.js" />

<script type="text/javascript">
    var moduleId = <%=ModuleId%>;
    var regionId = null;
    var groupId = null;
    var caseNumber = null;
    var cdspNumber = null;
    var lastName = null;
    var firstName = null;
    var businessName = null;
    var pageSize = 25;
    var recordCount = 0;
    var sortDirection = "desc";
    var sortColumnIndex = 6;
    var isAdmin = "<%=isAdminUser%>";
    var currentPage = 0;
    //GetLocalStorage();
    if (!groupId)
        groupId = 0;
    if (!regionId)
        regionId = 0;
    (function ($, Sys) {
        $(document).ready(function () {
            PageInit();
        });
    }(jQuery, window.Sys));

    // Execute a function when the user presses a key on the keyboard
    document.addEventListener("keypress", function (event) {
        // If the user presses the "Enter" key on the keyboard
        if (event.key === "Enter") {
            // Cancel the default action, if needed
            event.preventDefault();
            // Trigger the button element with a click
            document.getElementById("cmdSearch").click();
        }
    });
    function PageInit() {

        var restUrl = `/DesktopModules/tjc.Modules/Mediation/api/CaseListItem/GetCaseListItems/${recordCount}`;
        var deleteUrl = "/DesktopModules/tjc.Modules/Mediation/api/CaseListItem/Delete/";
        var caseTable = $('#tblCases').DataTable({
            "searching": false,
            autoWidth: true,
            ajax: {
                url: restUrl,
                type: "GET",
                datatype: 'json',
                data(data) {
                    data.region = regionId;
                    data.group = groupId;
                    data.firstName = firstName;
                    data.lastName = lastName;
                    data.businessName = businessName;
                    data.caseNumber = caseNumber;
                    data.cdspNumber = cdspNumber;
                    delete data.columns;
                },
            },
            columns: [{
                data: "caseid", render: function (data, type, row, meta) {
                    var url = "<%=EditUrl("CDSP")%>";
                    url = url.replace("CDSP", row.groupname);
                    return `<a title="Edit Record" onclick="SetCaseId(${data})" href="${url}/cid/${data}"><i class="fas fa-search"></i></a>`;
                }, className: "command-item", orderable: false
            },
                { data: "listnumber" },
                { data: "region" },
                { data: "group" },
                { data: "partyone" },
                { data: "partytwo" },
                { data: "createddate" },
                {
                    data: "comments", render: function (data, type, row, meta) {
                        if (isAdmin == "true")
                            return data == '' ? '' : '<i class="fas fa-comment-alt" data-html="true" title="' + data + '" data-toggle="tooltip" ></i>';
                        return '';
                    }, className: "command-item", orderable: false
                },
                {
                    data: "caseid", render: function (data, type, row, meta) {
                        if (isAdmin == "true")
                            return '<a class="delete confirm" aria-role="button" title="Delete Record" data-caseid="' + data + '" href="#""><i class="fas fa-trash"></i></a>';
                        return '';
                    }, className: "command-item", orderable: false
                },
            ],
            language: {
                emptyTable: "No Records Available.",
                zeroRecords: "No records match the search criteria you entered."
            },
            order: [[sortColumnIndex, sortDirection]],
            serverSide: true,
            process: true,
            lengthMenu: [[25, 50, 100], [25, 50, 100]],
            pageLength: pageSize,
            displayStart: currentPage * pageSize,
        });
        caseTable.on('draw', function () {
            $('[data-toggle="tooltip"]').tooltip();
            $(".confirm").on("click", function (e) {
                e.preventDefault();
                var caseid = $(this).data("caseid");
                $.dnnConfirm({
                    text: 'Are you sure you wish to delete this Case?',
                    yesText: 'Yes',
                    noText: 'No',
                    title: 'Delete Case?',
                    callbackTrue: function () {
                        deleteCase(caseid);
                    }
                });
                function deleteCase(caseId) {
                    $.ajax({
                        url: "/DesktopModules/tjc.Modules/Mediation/Services/api/CaseListItem/DeleteCase/" + caseId,
                        type: 'DELETE',
                        success: function (result) {
                            caseTable.draw();
                        },
                        error: function (error) {
                            alert(error);
                        }
                    });
                }
            });
        });
        $.fn.dataTable.ext.errMode = () => alert('Error while loading the table data. Please refresh');
        caseTable.on('order.dt', function () {
            // This will show: "Ordering on column 1 (asc)", for example
            var order = caseTable.order();
            //    localStorage.setItem('mediation.sortDirection', order[0][1]);
            //    localStorage.setItem('mediation.sortColumnIndex', order[0][0]);
        });
        caseTable.on('page.dt', function () {
            var info = caseTable.page.info();
            //localStorage.setItem('mediation.currentPageIndex', info.page);
        });
        caseTable.on('length.dt', function (e, settings, len) {
            // localStorage.setItem('mediation.pageSize', len);
        });
        $("#drpRegion").on("change", function () {
            regionId = null;
            if ($(this).val().length > 0)
                regionId = $(this).val();
            //    localStorage.setItem('mediation.regionId', $(this).val());
        });
        //$("#txtCaseSequence").on("blur", function () {
        //    var number = $("#txtCaseSequence").val();
        //    if (number.length > 0)
        //        number = number.toString().padStart(6, '0');
        //    $("#txtCaseSequence").val(number);
        //});
        $("#drpGroup").on("change", function () {
            groupId = null;
            if ($(this).val().length > 0)
                groupId = $(this).val();
            //    localStorage.setItem('mediation.groupId', $(this).val());
        });
        $("#cmdSearch").on("click", function (e) {
            e.preventDefault();
            caseNumber = GetCaseNumber();
            cdspNumber = GetCDSPNumber();
            lastName = $("#txtLastName").val();
            firstName = $("#txtFirstName").val();
            businessName = $("#txtBusinessName").val();
            caseTable.draw();
        });

        var IsAdmin = '<%=IsAdmin%>';
        if (IsAdmin === 'True') { $("#lists").show(); }
        $(".date-picker").datepicker();
        InitializeCaseTypeGroups();
    }
    function DeleteCase(e, caseId) {
        e.preventDefault();
        $.ajax({
            url: "/DesktopModules/tjc.Modules/Mediation/api/CaseListItem/DeleteCase/" + caseId,
            type: 'DELETE',
            success: function (result) {
                caseTable.draw();
            },
            error: function (error) {
                alert(error);
            }
        });
    }
    function InitializeCaseTypeGroups() {
        var $select = $("#drpGroup");
        var currentSelection = $select.val();
        var optGroup;
        $("#drpGroup option").each(function () {
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
                    $("<option>" + $(this).text() + "</option>").attr("value", $(this).val()).appendTo(optGroup);
                } else {
                    $("<option>" + $(this).text() + "</option>").attr("value", $(this).val()).appendTo($select);
                }
            }
            $(this).remove();
        });
        $select.val(currentSelection);
    }
    function CheckCaseNumber(sender, args) {
        var fYear = document.getElementById("<%=txtCaseYear.ClientID %>").value;
        var fCode = document.getElementById("<%=txtCaseType.ClientID %>").value;
        var fNumber = document.getElementById("<%=txtCaseSequence.ClientID %>").value;
        var isCaseNumber = true;
        var isCDSPNumber = true;
        if (fYear == "" || fCode == "" || fNumber == "") {
            isCaseNumber = false;
        }
        var fCType = $("#drpCDSPType").val();
        var fCYear = document.getElementById("<%=txtCDSPYear.ClientID %>").value;
        var fCNumber = document.getElementById("<%=txtCDSPNumber.ClientID %>").value;
        var fCLocation = $("#drpCDSPLocation").val();
        if (fCType == "" || fCYear == "" || fCNumber == "" || fCLocation == "") {
            isCDSPNumber = false;
        }
        if (isCaseNumber == false && isCDSPNumber == false) {
            args.IsValid = false;
            return;
        }

        args.IsValid = true;
    }
    function fnJSOnFormSubmit(e) {
        var isGrpOneValid = Page_ClientValidate("CaseSearch");
        var isGrpTwoValid = Page_ClientValidate("CaseNew");

        var i;
        for (i = 0; i < Page_Validators.length; i++) {
            ValidatorValidate(Page_Validators[i]); //this forces validation in all groups
        }

        //display all summaries.
        for (i = 0; i < Page_ValidationSummaries.length; i++) {
            summary = Page_ValidationSummaries[i];
            //does this summary need to be displayed?
            if (fnJSDisplaySummary(summary.validationGroup)) {
                summary.style.display = ""; //"none"; "inline";
            }
        }

        if (isGrpOneValid && isGrpTwoValid)

            return true; //postback only when BOTH validations pass.
        else
            return false;
    }
    function fnJSDisplaySummary(valGrp) {
        var rtnVal = false;
        for (i = 0; i < Page_Validators.length; i++) {
            if (Page_Validators[i].validationGroup == valGrp) {
                if (!Page_Validators[i].isvalid) { //at least one is not valid.
                    rtnVal = true;
                    break; //exit for-loop, we are done.
                }
            }
        }
        return rtnVal;
    }
    function GetCaseNumber() {
        var year = $("#txtCaseYear").val();
        var code = $("#txtCaseType").val();
        var number = $("#txtCaseSequence").val();
        var suffix = $("#txtSuffix").val();
        var caseNumber = null;
        if (year.length == 2)
            year = "20" + year;
        if (number.length > 0)
            number = number.toString().padStart(6, '0');
        if (year && code && number)
            caseNumber = `${year} ${code.toUpperCase()} ${number} ${suffix.toUpperCase()}`;
        return caseNumber;
    }
    function GetCDSPNumber() {
        var type = $("#drpCDSPType").val();
        var year = $("#txtCDSPYear").val();
        var number = $("#txtCDSPNumber").val();
        var location = $("#drpCountyLetter").val();
        var cdspNumber = "";
        if (type.length > 0) { cdspNumber += type + "-"; } else { return null; }
        if (year.length > 0) { cdspNumber += year + "-"; } else { return null; }
        if (number.length > 0) { cdspNumber += number + "-"; } else { return null; }
        if (location.length > 0)
            cdspNumber += location;
        if (cdspNumber != null && cdspNumber.endsWith("-"))
            cdspNumber = cdspNumber.slice(0, -1);
        return cdspNumber;
    }
    function SetCaseId(caseId) {
        // localStorage.setItem('mediation.caseId', caseId);
    }
    function GetLocalStorage() {
        storageRegionId = localStorage.getItem('mediation.regionId');
        storageGroupId = localStorage.getItem('mediation.groupId');
        storageCurrentPage = localStorage.getItem('mediation.currentPageIndex');
        storagePageSize = localStorage.getItem('mediation.pageSize');
        storageSortDirection = localStorage.getItem('mediation.sortDirection');
        storageSortColumnIndex = localStorage.getItem('mediation.sortColumnIndex');
        if (storageRegionId != null && storageRegionId != undefined) {
            regionId = storageRegionId;
            $("#drpRegion").val(regionId);
        }
        if (storageGroupId != null && storageGroupId != undefined) {
            groupId = storageGroupId;
            $("#drpGroup").val(groupId);
        }
        if (storageCurrentPage != null && storageCurrentPage != undefined)
            currentPage = storageCurrentPage;
        if (storagePageSize != null && storagePageSize != undefined)
            pageSize = storagePageSize;
        if (storageSortDirection != null && storageSortDirection != undefined)
            sortDirection = storageSortDirection;
        if (storageSortColumnIndex != null && storageSortColumnIndex != undefined)
            sortColumnIndex = storageSortColumnIndex;
    }
    function ShowAlert(title, text) {
        $.dnnAlert({
            okText: 'OK',
            title: title,
            text: text
        });
    }
</script>


