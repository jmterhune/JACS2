<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="View.ascx.cs" Inherits="tjc.Modules.Reports.View" %>
<asp:Panel runat="server" ID="pnlReportList">
    <ul class="list list-icons">
        <li><a href="/12th-Circuit-Services/Human-Resources/Employee-Reports/rid/1"><i class="fas fa-birthday-cake"></i> Birthday Report</a></li>
        <li><a href="/12th-Circuit-Services/Human-Resources/Employee-Reports/rid/2"><i class="fas fa-star"></i> Service Reports</a></li>
        <li><a href="/12th-Circuit-Services/Human-Resources/Employee-Reports/rid/3"><i class="fas fa-user-minus"></i> Termination Report</a></li>
        <li>
            <asp:HyperLink ID="lnkDataCard" runat="server"><i class="fas fa-id-badge"></i> Data Card</asp:HyperLink></li>
        <%-- Employee Reports (replaces HR Excel sheets that used to live
             in EmployeeDB\Documentation\). Each report is its own module
             control under EmployeeReports/ — see Reports.dnn. --%>
        <li>
            <asp:HyperLink ID="lnkDropParticipants" runat="server"><i class="fas fa-hourglass-half"></i> DROP Participants</asp:HyperLink></li>
        <li>
            <asp:HyperLink ID="lnkJaSeniority" runat="server"><i class="fas fa-balance-scale"></i> JA Seniority</asp:HyperLink></li>
        <li>
            <asp:HyperLink ID="lnkStaffAttorneySeniority" runat="server"><i class="fas fa-gavel"></i> Staff Attorney Seniority</asp:HyperLink></li>
        <li>
            <asp:HyperLink ID="lnkCertifiedInterpreter" runat="server"><i class="fas fa-language"></i> Certified Interpreter Seniority</asp:HyperLink></li>
    </ul>
</asp:Panel>
<asp:Panel runat="server" ID="pnlBirthdays" Visible="false">
    <div class="mb-md">
        <div class="btn-group" role="group" aria-label="Search">
            <div id="swBirthMonth" class="input-group">
                <asp:DropDownList ID="drpBirthMonth" runat="server" CssClass="form-control" aria-label="Select Month" ClientIDMode="Static">
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
            </div>
            <div id="swCounty" class="input-group" runat="server">
                <%-- County items past "All Counties" are appended in
                     View.ascx.cs → BindCounties() from tjc_gl_counties so
                     the dropdown carries CountyId values (the SP now needs
                     an int @countyId, not a county-name string). --%>
                <asp:DropDownList ID="drpCounty" runat="server" CssClass="form-control" aria-label="Select County" ClientIDMode="Static">
                    <asp:ListItem Text="All Counties" Value="0" />
                </asp:DropDownList>
            </div>

        </div>
        <asp:Button ID="cmdSubmitBirthReport" OnClick="cmdSubmitBirthReport_Click" ClientIDMode="Static" runat="server" Text="View Report" ToolTip="View Report" CssClass="btn btn-primary" />
    </div>
</asp:Panel>
<asp:Panel runat="server" ID="pnlServiceAward" Visible="false">
    <div class="mb-md">
        <div class="btn-group" role="group" aria-label="Search">
            <div class="input-group"> <asp:TextBox runat="server" ID="txtYear" CssClass="form-control" aria-label="Year" TextMode="Number" Width="70" /></div>
            <div id="swServiceMonth" class="input-group">
                <asp:DropDownList ID="drpServiceMonth" runat="server"  CssClass="form-control" aria-label="Select Month" ClientIDMode="Static">
                    <asp:ListItem Text="All Year" Value="0" />
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
            </div>
            <div id="swReportType" runat="server" class="input-group">
                <asp:DropDownList ID="drpReportType" runat="server" CssClass="form-control" aria-label="Select Report Type" ClientIDMode="Static">
                    <asp:ListItem Text="Service Date" Value="1" />
                    <asp:ListItem Text="Hire Date" Value="0" />
                </asp:DropDownList>
            </div>
        </div>
        <asp:Button ID="cmdSubmitServiceReport" OnClick="cmdSubmitServiceReport_Click" ClientIDMode="Static" runat="server" Text="View Report" ToolTip="View Report" CssClass="btn btn-primary" />
    </div>
</asp:Panel>
<asp:Panel runat="server" ID="pnlTerminationReport" Visible="false">
    <%-- All validators + the submit button are in the "TerminationReport"
         ValidationGroup so they only fire on this panel's button click (and
         never on the Birthday / Service buttons in the sibling panels). --%>
    <div class="mb-md">
        <div class="btn-group" role="group" aria-label="Search">
            <div id="swStartDate" class="input-group">
                <asp:TextBox runat="server" CssClass="form-control" TextMode="date" ID="txtStartDate" ClientIDMode="Static" />
                <asp:RequiredFieldValidator runat="server" ID="rfvStartDate"
                    ControlToValidate="txtStartDate"
                    ValidationGroup="TerminationReport"
                    Display="Dynamic" CssClass="text-danger ms-2"
                    ErrorMessage="Start date is required." />
            </div>
            <div id="swEndDate" class="input-group">
                <asp:TextBox runat="server" CssClass="form-control" TextMode="date" ID="txtEndDate" ClientIDMode="Static" />
                <asp:RequiredFieldValidator runat="server" ID="rfvEndDate"
                    ControlToValidate="txtEndDate"
                    ValidationGroup="TerminationReport"
                    Display="Dynamic" CssClass="text-danger ms-2"
                    ErrorMessage="End date is required." />
                <%-- Operator=LessThanEqual + ControlToValidate=txtStartDate +
                     ControlToCompare=txtEndDate enforces start <= end. The
                     validator no-ops while either field is empty so it
                     doesn't double-fire with the RequiredFieldValidators. --%>
                <asp:CompareValidator runat="server" ID="cvDateRange"
                    ControlToValidate="txtStartDate"
                    ControlToCompare="txtEndDate"
                    Type="Date" Operator="LessThanEqual"
                    ValidationGroup="TerminationReport"
                    Display="Dynamic" CssClass="text-danger ms-2"
                    ErrorMessage="Start date must be on or before end date." />
            </div>
        </div>
        <asp:Button ID="cmdTerminationReport" OnClick="cmdTerminationReport_Click" ClientIDMode="Static"
                    ValidationGroup="TerminationReport"
                    runat="server" Text="View Report" ToolTip="View Report" CssClass="btn btn-primary" />
    </div>
</asp:Panel>
<h2>
    <asp:Literal ID="ltReportTitle" runat="server" />
</h2>
<asp:HiddenField  id="hdTitle" runat="server" Value="Employee Reports" ClientIDMode="Static" />
<asp:GridView ID="grdReport" GridLines="None" OnRowDataBound="OnRowDataBound" CssClass="table table-striped" runat="server" AutoGenerateColumns="true" AllowSorting="false" AllowPaging="false" ClientIDMode="Static"></asp:GridView>
<asp:HyperLink CssClass="btn btn-primary" ID="lnkReport" runat="server" Text="Return to Report List" /> 
<%-- Client-side grid enhancement (sort / filter / export) via DataTables.
     Libraries are the site-wide copies under /Resources/Libraries/DataTables.
     Load order matters: core -> BS5 styling -> jszip + pdfmake (the export
     dependencies) -> Buttons core -> Buttons BS5 -> Buttons HTML5 (excel/pdf). --%>
<link rel="stylesheet" href="/Resources/Libraries/DataTables/dataTables.bootstrap5.min.css" />
<link rel="stylesheet" href="/Resources/Libraries/DataTables/buttons.bootstrap5.min.css" />
<script src="/Resources/Libraries/DataTables/dataTables.js"></script>
<script src="/Resources/Libraries/DataTables/dataTables.bootstrap5.min.js"></script>
<script src="/Resources/Libraries/DataTables/jszip.min.js"></script>
<script src="/Resources/Libraries/DataTables/pdfmake.min.js"></script>
<script src="/Resources/Libraries/DataTables/vfs_fonts.js"></script>
<script src="/Resources/Libraries/DataTables/dataTables.buttons.min.js"></script>
<script src="/Resources/Libraries/DataTables/buttons.bootstrap5.min.js"></script>
<script src="/Resources/Libraries/DataTables/buttons.html5.min.js"></script>
<script>
    $(function () {
        var title = $("#hdTitle").val();
        $(".page-top-info h1").html(title);

        var grid = document.getElementById("grdReport");
        if (!grid || !$.fn || !$.fn.dataTable) return;
        var $grid = $(grid);
        // Only enhance a populated grid (header + at least one data row). An
        // empty report renders no <table>, so this no-ops there too.
        if ($grid.find("thead th").length === 0 || $grid.find("tbody tr").length === 0) return;
        if ($.fn.dataTable.isDataTable(grid)) return;

        // Date columns must sort chronologically, not as text ("12/01/2019"
        // would otherwise sort before "01/05/2020"). A column qualifies when
        // every non-empty cell looks like "MM/dd/yyyy" or "Month dd" (the
        // Service/Termination and Birthday formats). We attach epoch-ms sort
        // data and leave the displayed text alone so exports stay friendly.
        var dateRe = /^(\d{1,2}\/\d{1,2}\/\d{4}|[A-Za-z]+ \d{1,2})$/;
        var columnDefs = [];
        var colCount = $grid.find("thead th").length;
        for (var c = 0; c < colCount; c++) {
            var seen = 0, allDates = true;
            $grid.find("tbody tr").each(function () {
                var txt = $.trim($(this).children().eq(c).text());
                if (txt === "") return;
                seen++;
                if (!dateRe.test(txt) || isNaN(Date.parse(txt))) allDates = false;
            });
            if (seen > 0 && allDates) {
                columnDefs.push({
                    targets: c,
                    render: function (data, type) {
                        if (type === "sort" || type === "type") {
                            var t = Date.parse(data);
                            return isNaN(t) ? 0 : t;
                        }
                        return data;
                    }
                });
            }
        }

        // The Service report's first column is "State or County" (rendered as
        // a multi-line header). Find it so we can offer a dropdown filter; the
        // other reports have no such column and simply skip it.
        var stateCountyCol = -1;
        $grid.find("thead th").each(function (i) {
            var h = $(this).text().toLowerCase();
            if (stateCountyCol === -1 && (h.indexOf("county") !== -1 || h.indexOf("state") !== -1)) {
                stateCountyCol = i;
            }
        });

        // Flatten the multi-line "State<br>or<br>County" / "Years<br>of<br>Service"
        // headers to clean single-line text for the Excel / PDF exports.
        var exportOptions = {
            columns: ":visible",
            format: {
                header: function (data) {
                    return $("<div>").html(String(data).replace(/<br\s*\/?>/gi, " ")).text().replace(/\s+/g, " ").trim();
                }
            }
        };
        var docTitle = function () { return $("#hdTitle").val() || "Employee Report"; };

        $grid.DataTable({
            paging: false,   // match the old GridView (AllowPaging=false): show every row
            order: [],       // keep the server ORDER BY (LastName, FirstName)
            columnDefs: columnDefs,
            layout: {
                topStart: {
                    buttons: [{
                        extend: "collection",
                        text: '<i class="fas fa-download"></i> Export',
                        buttons: [
                            { extend: "excelHtml5", text: '<i class="fas fa-file-excel"></i> Excel', title: docTitle, exportOptions: exportOptions },
                            { extend: "pdfHtml5", text: '<i class="fas fa-file-pdf"></i> PDF', orientation: "landscape", pageSize: "LETTER", title: docTitle, exportOptions: exportOptions }
                        ]
                    }]
                },
                topEnd: "search"
            },
            initComplete: function () {
                if (stateCountyCol < 0) return;
                var dt = this.api();
                var column = dt.column(stateCountyCol);
                var $wrap = $('<label class="dt-sc-filter"><span class="dt-sc-filter-label">State / County:</span></label>');
                var $select = $('<select class="form-select form-select-sm"><option value="">All</option></select>');
                var values = {};
                column.data().each(function (d) {
                    var v = $.trim($("<div>").html(d == null ? "" : String(d)).text());
                    if (v !== "") values[v] = true;
                });
                Object.keys(values).sort().forEach(function (v) {
                    $select.append($("<option>").val(v).text(v));
                });
                $select.on("change", function () {
                    var val = $(this).val();
                    column.search(val ? "^" + $.fn.dataTable.util.escapeRegex(val) + "$" : "", { regex: true, smart: false }).draw();
                });
                $wrap.append($select);
                var $btns = $(dt.buttons().container());
                if ($btns.length) { $btns.parent().append($wrap); }
                else { $(dt.table().container()).prepend($wrap); }
            }
        });
    });
</script>