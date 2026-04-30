<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DataSheet.ascx.cs" Inherits="tjc.Modules.CourtCounsel.Views.DataSheet" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>

<nav class="navbar navbar-expand-lg navbar-dark bg-dark mb-md rounded">
    <button class="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target="#navbarNav">
        <span class="navbar-toggler-icon"></span>
    </button>
    <div class="collapse navbar-collapse" id="navbarNav">
        <ul class="navbar-nav">
            <li class="nav-item"><a class="nav-link" href="<%=SearchUrl %>"><i class="fas fa-search"></i>&nbsp;Search</a></li>
            <li class="nav-item"><a class="nav-link" href="<%=DataEntryUrl %>"><i class="fas fa-pencil-alt"></i>&nbsp;Data Entry</a></li>
            <li class="nav-item"><a class="nav-link" href="<%=ReportsUrl %>"><i class="fas fa-chart-bar"></i>&nbsp;Reports</a></li>
            <li class="nav-item"><a class="nav-link active" href="<%=DataSheetUrl %>"><i class="fas fa-table"></i>&nbsp;Data Sheet</a></li>
            <li class="nav-item" id="liAdmin" runat="server" visible="false"><a class="nav-link" href="<%=AdminUrl %>"><i class="fa fa-tools"></i>&nbsp;Admin</a></li>
        </ul>
    </div>
</nav>

<div class="datasheet-full-width">
    <div class="mb-3">
        <div class="row">
            <div class="col-md-3">
                <label class="fw-bold">Filter by Attorney:</label>
                <div class="d-block dropdown attorney-dropdown">
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
            <div class="col-md-3">
                <label for="<%=txtDateReceivedFrom.ClientID %>" class="fw-bold">Date Received &ge;</label>
                <asp:TextBox ID="txtDateReceivedFrom" runat="server" CssClass="form-control" TextMode="Date" />
            </div>
            <div class="col-md-3">
                <label for="<%=drpRequestedBy.ClientID %>" class="fw-bold">Requested By:</label>
                <asp:DropDownList ID="drpRequestedBy" runat="server" CssClass="form-select" />
            </div>
            <div class="col-md-3">
                <label for="<%=drpCompletedFilter.ClientID %>" class="fw-bold">Status:</label>
                <asp:DropDownList ID="drpCompletedFilter" runat="server" CssClass="form-select">
                    <asp:ListItem Value="include" Text="Include Completed" Selected="True" />
                    <asp:ListItem Value="exclude" Text="Does Not Include Completed" />
                </asp:DropDownList>
            </div>
        </div>
        <div class="row mt-2">
            <div class="col-md-12">
                <asp:Button ID="cmdFilter" runat="server" CssClass="btn btn-primary" Text="Filter" OnClick="cmdFilter_Click" />
                <asp:Button ID="cmdClear" runat="server" CssClass="btn btn-secondary" Text="Clear" OnClick="cmdClear_Click" />
                <asp:Button ID="cmdExport" runat="server" CssClass="btn btn-success" Text="Export to Excel" OnClick="cmdExport_Click" CausesValidation="false" />
            </div>
        </div>
    </div>

    <asp:UpdatePanel ID="upSheet" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <div class="table-responsive">
                <table id="sheet-table" class="table table-striped table-bordered table-hover">
                    <thead>
                        <tr>
                            <th>Case Name</th>
                            <th>Case Type</th>
                            <th>Case Number</th>
                            <th>Date Received</th>
                            <th>Motion Filed</th>
                            <th>Requested By</th>
                            <th>Responsible</th>
                            <th>Action</th>
                            <th>Completed</th>
                            <th>Status</th>
                        </tr>
                    </thead>
                    <tbody>
                        <asp:Repeater ID="rptSheet" runat="server">
                            <ItemTemplate>
                                <tr>
                                    <td><%#Eval("PartyName") %></td>
                                    <td><%#Eval("CaseType") %></td>
                                    <td>
                                        <a href='<%#EditUrl("cn",Eval("CaseNumber").ToString(),"CaseHistory") %>'>
                                            <%#Eval("CaseNumber") %>
                                    </a>
                                    </td>
                                    <td><%#Eval("DateReceived", "{0:d}") %></td>
                                    <td><%#Eval("MotionFiled", "{0:d}") %></td>
                                    <td><%#Eval("RequestedBy") %></td>
                                    <td><%#Eval("Responsible") %></td>
                                    <td><%#Eval("Action") %></td>
                                    <td><%#Eval("DateCompleted", "{0:d}") %></td>
                                    <td><%#Eval("StatusName") %></td>
                                </tr>
                            </ItemTemplate>
                        </asp:Repeater>
                    </tbody>
                </table>
            </div>

            <div class="row align-items-center">
                <div class="col-md-3">
                    <label for="<%=drpPageSize.ClientID %>" class="form-label">Rows per page:</label>
                    <asp:DropDownList ID="drpPageSize" runat="server" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="drpPageSize_SelectedIndexChanged">
                        <asp:ListItem Value="25" Text="25" />
                        <asp:ListItem Value="50" Text="50" Selected="True" />
                        <asp:ListItem Value="100" Text="100" />
                        <asp:ListItem Value="250" Text="250" />
                    </asp:DropDownList>
                </div>
                <div class="col-md-6 text-center">
                    <asp:LinkButton ID="cmdFirst" runat="server" CssClass="btn btn-outline-secondary" CausesValidation="false" OnClick="cmdFirst_Click"><i class="fas fa-angle-double-left"></i></asp:LinkButton>
                    <asp:LinkButton ID="cmdPrev" runat="server" CssClass="btn btn-outline-secondary" CausesValidation="false" OnClick="cmdPrev_Click"><i class="fas fa-angle-left"></i></asp:LinkButton>
                    <asp:Literal ID="lblPageInfo" runat="server" />
                    <asp:LinkButton ID="cmdNext" runat="server" CssClass="btn btn-outline-secondary" CausesValidation="false" OnClick="cmdNext_Click"><i class="fas fa-angle-right"></i></asp:LinkButton>
                    <asp:LinkButton ID="cmdLast" runat="server" CssClass="btn btn-outline-secondary" CausesValidation="false" OnClick="cmdLast_Click"><i class="fas fa-angle-double-right"></i></asp:LinkButton>
                </div>
                <div class="col-md-3 text-end">
                    <asp:Literal ID="lblTotal" runat="server" />
                </div>
            </div>
        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="cmdFilter" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="cmdClear" EventName="Click" />
        </Triggers>
    </asp:UpdatePanel>
</div>

<script type="text/javascript">
    (function ($) {
        function InitAttorneyDropdown() {
            var $root = $(".attorney-dropdown");
            if (!$root.length) return;

            $root.find(".dropdown-menu").on("click", function (e) { e.stopPropagation(); });

            var $label = $("#attorneySelectedLabel");
            var $selectAll = $root.find(".attorney-select-all");
            var $filter = $root.find(".attorney-filter");

            function $activeBoxes() { return $root.find(".attorney-checklist:not(.attorney-inactive) input[type=checkbox]"); }
            function $allBoxes() { return $root.find(".attorney-checklist input[type=checkbox]"); }

            function UpdateLabel() {
                var $active = $activeBoxes();
                var selected = $active.filter(":checked").map(function () { return $(this).next("label").text().trim(); }).get();
                $root.find(".attorney-inactive input[type=checkbox]:checked").each(function () {
                    selected.push($(this).next("label").text().trim());
                });

                if (selected.length === 0) { $label.text("Select from List"); }
                else if (selected.length === $active.length) { $label.text("All attorneys"); }
                else if (selected.length <= 3) { $label.text(selected.join(", ")); }
                else { $label.text(selected.length + " attorneys selected"); }

                var visibleActive = $active.filter(":visible");
                var visibleActiveChecked = visibleActive.filter(":checked");
                $selectAll.prop("checked", visibleActive.length > 0 && visibleActive.length === visibleActiveChecked.length);
                $selectAll.prop("indeterminate", visibleActiveChecked.length > 0 && visibleActiveChecked.length < visibleActive.length);
            }

            $selectAll.on("click", function () {
                $activeBoxes().filter(":visible").prop("checked", this.checked);
                UpdateLabel();
            });

            $allBoxes().on("change", UpdateLabel);

            $filter.on("input", function () {
                var term = $(this).val().toLowerCase();
                $root.find(".attorney-checklist label").each(function () {
                    var $label = $(this);
                    var $row = $label.parent();
                    $row.toggle($label.text().toLowerCase().indexOf(term) !== -1);
                });
                UpdateLabel();
            });

            UpdateLabel();
        }

        $(document).ready(InitAttorneyDropdown);
        if (typeof Sys !== "undefined") {
            Sys.WebForms.PageRequestManager.getInstance().add_endRequest(InitAttorneyDropdown);
        }
    }(jQuery));
</script>
