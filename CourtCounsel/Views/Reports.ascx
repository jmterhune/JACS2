<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Reports.ascx.cs" Inherits="tjc.Modules.CourtCounsel.Views.Reports" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>

<nav class="navbar navbar-expand-lg navbar-dark bg-dark mb-md rounded">
    <button class="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target="#navbarNav">
        <span class="navbar-toggler-icon"></span>
    </button>
    <div class="collapse navbar-collapse" id="navbarNav">
        <ul class="navbar-nav">
            <li class="nav-item"><a class="nav-link" href="<%=SearchUrl %>"><i class="fas fa-search"></i>&nbsp;Search</a></li>
            <li class="nav-item"><a class="nav-link" href="<%=DataEntryUrl %>"><i class="fas fa-pencil-alt"></i>&nbsp;Data Entry</a></li>
            <li class="nav-item"><a class="nav-link active" href="<%=ReportsUrl %>"><i class="fas fa-chart-bar"></i>&nbsp;Reports</a></li>
            <li class="nav-item"><a class="nav-link" href="<%=DataSheetUrl %>"><i class="fas fa-table"></i>&nbsp;Data Sheet</a></li>
            <li class="nav-item" id="liAdmin" runat="server" visible="false"><a class="nav-link" href="<%=AdminUrl %>"><i class="fa fa-tools"></i>&nbsp;Admin</a></li>
        </ul>
    </div>
</nav>

<div class="card">
    <div class="card-header">
        <a data-bs-toggle="collapse" href="#reportFiltersCollapse" role="button" aria-expanded="true" aria-controls="reportFiltersCollapse"
           class="text-decoration-none d-flex justify-content-between align-items-center collapse-toggle">
            <strong><i class="fas fa-filter"></i>&nbsp;Report Filters</strong>
            <span class="collapse-indicator">
                <i class="fas fa-minus icon-expanded"></i>
                <i class="fas fa-plus icon-collapsed"></i>
            </span>
        </a>
    </div>
    <div id="reportFiltersCollapse" class="collapse show">
    <div class="card-body p-3">
        <div class="row mb-3">
            <div class="col-md-3">
                <label class="fw-bold">Start Date:</label>
                <asp:TextBox ID="txtStartDate" runat="server" CssClass="form-control" TextMode="Date" />
            </div>
            <div class="col-md-3">
                <label class="fw-bold">End Date:</label>
                <asp:TextBox ID="txtEndDate" runat="server" CssClass="form-control" TextMode="Date" />
            </div>
        </div>
        <div class="row mb-3">
            <div class="col-md-12">
                <label class="fw-bold">Status:</label>
                <asp:RadioButtonList ID="rblStatus" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" CssClass="status-options">
                    <asp:ListItem Text="Active" Value="Active" Selected="True" />
                    <asp:ListItem Text="Inactive" Value="Inactive" />
                    <asp:ListItem Text="Not Completed" Value="NotCompleted" />
                    <asp:ListItem Text="Completed" Value="Completed" />
                    <asp:ListItem Text="All" Value="" />
                </asp:RadioButtonList>
            </div>
        </div>
        <div class="row mb-3">
            <div class="col-md-3">
                <label class="fw-bold">Extended Status:</label>
                <asp:DropDownList ID="drpStatus" runat="server" CssClass="form-control">
                    <asp:ListItem Text="-- All --" Value="" />
                    <asp:ListItem Text="New" Value="New" />
                    <asp:ListItem Text="In Progress" Value="In Progress" />
                    <asp:ListItem Text="Under Review" Value="Under Review" />
                    <asp:ListItem Text="On Hold" Value="On Hold" />
                    <asp:ListItem Text="Completed" Value="Completed" />
                    <asp:ListItem Text="Dismissed" Value="Dismissed" />
                    <asp:ListItem Text="Withdrawn" Value="Withdrawn" />
                </asp:DropDownList>
            </div>
            <div class="col-md-3">
                <label class="fw-bold">County:</label>
                <asp:DropDownList ID="drpCounty" runat="server" CssClass="form-control" />
            </div>
            <div class="col-md-3">
                <label class="fw-bold">Requestor:</label>
                <asp:DropDownList ID="drpRequestor" runat="server" CssClass="form-control" />
            </div>
        </div>
        <div class="row mb-3">
            <div class="col-md-6">
                <label class="fw-bold d-block">Attorney:</label>
                <div class="dropdown attorney-dropdown">
                    <button type="button" id="attorneyToggle" class="form-select text-start" data-bs-toggle="dropdown" data-bs-auto-close="outside" aria-expanded="false">
                        <span id="attorneySelectedLabel">Select from List</span>
                    </button>
                    <div class="dropdown-menu p-2 attorney-dropdown-menu">
                        <input type="text" class="form-control form-control-sm mb-2 attorney-filter" placeholder="Filter attorneys..." autocomplete="off" />
                        <div class="form-check border-bottom pb-1 mb-1">
                            <input type="checkbox" class="form-check-input attorney-select-all" id="chkAttAll" />
                            <label class="form-check-label fw-bold" for="chkAttAll">Select All</label>
                        </div>
                        <asp:CheckBoxList ID="cblAttorneys" runat="server" RepeatLayout="Flow" CssClass="attorney-checklist" />
                        <asp:Panel ID="pnlInactiveAttorneys" runat="server" Visible="false" CssClass="attorney-inactive-section">
                            <div class="dropdown-header text-muted small pt-2 border-top">Inactive</div>
                            <asp:CheckBoxList ID="cblAttorneysInactive" runat="server" RepeatLayout="Flow" CssClass="attorney-checklist attorney-inactive" />
                        </asp:Panel>
                    </div>
                </div>
            </div>
        </div>
        <div class="row mb-3">
            <div class="col-md-12">
                    <asp:CheckBox ID="chkShowDetail" runat="server" Text="Show Detail" CssClass="form-check" />
            </div>
        </div>
        <div class="row">
            <div class="col-md-12">
                <asp:Button ID="cmdSubmit" runat="server" CssClass="btn btn-primary" Text="Submit" OnClick="cmdSubmit_Click" />
                <asp:Button ID="cmdReset" runat="server" CssClass="btn btn-secondary ms-1" Text="Reset" OnClick="cmdReset_Click" />
            </div>
        </div>
    </div>
    </div>
</div>

<div class="mt-4">
    <asp:Literal ID="ltResults" runat="server" />
</div>

<script type="text/javascript">
    (function ($) {
        function InitFormCheckStyling() {
            // Bootstrap 5 expects form-check-input / form-check-label on inner elements.
            // asp:CheckBox + asp:RadioButtonList render bare <input>/<label> pairs, so promote classes on render.
            $(".form-check input").addClass("form-check-input");
            $(".form-check label").addClass("form-check-label");
            $(".status-options input").addClass("form-check-input me-1");
            $(".status-options label").addClass("form-check-label me-3");
        }

        function InitAttorneyDropdown() {
            var $root = $(".attorney-dropdown");
            if (!$root.length) return;

            // Prevent clicks inside the menu from closing the dropdown.
            $root.find(".dropdown-menu").on("click", function (e) { e.stopPropagation(); });

            var $label = $("#attorneySelectedLabel");
            var $selectAll = $root.find(".attorney-select-all");
            var $filter = $root.find(".attorney-filter");

            // All selectable (active) checkboxes; inactive ones are disabled and not counted in the label.
            function $activeBoxes() { return $root.find(".attorney-checklist:not(.attorney-inactive) input[type=checkbox]"); }
            function $allBoxes() { return $root.find(".attorney-checklist input[type=checkbox]"); }

            function UpdateLabel() {
                var $active = $activeBoxes();
                var selected = $active.filter(":checked").map(function () {
                    return $(this).next("label").text().trim();
                }).get();
                var $inactiveSelected = $root.find(".attorney-inactive input[type=checkbox]:checked");
                $inactiveSelected.each(function () {
                    selected.push($(this).next("label").text().trim());
                });

                if (selected.length === 0) {
                    $label.text("Select from List");
                } else if (selected.length === $active.length) {
                    $label.text("All attorneys");
                } else if (selected.length <= 3) {
                    $label.text(selected.join(", "));
                } else {
                    $label.text(selected.length + " attorneys selected");
                }

                // Sync the Select-All checkbox state
                var visibleActive = $active.filter(":visible");
                var visibleActiveChecked = visibleActive.filter(":checked");
                $selectAll.prop("checked", visibleActive.length > 0 && visibleActive.length === visibleActiveChecked.length);
                $selectAll.prop("indeterminate", visibleActiveChecked.length > 0 && visibleActiveChecked.length < visibleActive.length);
            }

            $selectAll.on("click", function () {
                var checked = this.checked;
                $activeBoxes().filter(":visible").prop("checked", checked);
                UpdateLabel();
            });

            $allBoxes().on("change", UpdateLabel);

            $filter.on("input", function () {
                var term = $(this).val().toLowerCase();
                $root.find(".attorney-checklist label").each(function () {
                    var $label = $(this);
                    var $row = $label.parent(); // CheckBoxList wraps each item in a span
                    var match = $label.text().toLowerCase().indexOf(term) !== -1;
                    $row.toggle(match);
                });
                UpdateLabel();
            });

            UpdateLabel();
        }

        function InitReportsPage() {
            InitFormCheckStyling();
            InitAttorneyDropdown();
        }

        $(document).ready(InitReportsPage);
        if (typeof Sys !== "undefined") {
            Sys.WebForms.PageRequestManager.getInstance().add_endRequest(InitReportsPage);
        }
    }(jQuery));
</script>

