<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EEOSetup.ascx.cs" Inherits="tjc.Modules.EmployeeDB.Views.EEOSetup" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>

<dnn:DnnCssInclude runat="server" FilePath="~/DesktopModules/tjc.modules/EmployeeDB/module.css" Priority="100" />

<%-- SweetAlert2 + Noty for confirms / toast notifications --%>
<dnn:DnnCssInclude runat="server" FilePath="/Resources/Libraries/sweetalert/sweetalert2.min.css" />
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/sweetalert/sweetalert2.all.min.js" />
<dnn:DnnCssInclude runat="server" FilePath="/Resources/Libraries/Noty/noty.min.css" />
<dnn:DnnCssInclude runat="server" FilePath="/Resources/Libraries/Noty/bootstrap-v4.min.css" />
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/Noty/noty.min.js" />
<dnn:DnnJsInclude runat="server" FilePath="~/DesktopModules/tjc.modules/EmployeeDB/Scripts/empdb-edit.js" Priority="200" />
<dnn:DnnJsInclude runat="server" FilePath="~/DesktopModules/tjc.modules/EmployeeDB/Scripts/empdb-eeo.js" Priority="220" />

<%-- Page-wide container override: the EEO List has 5 categories x 8
     race/gender slots = 40 numeric columns + Year + Job Category, so the
     default Bootstrap container max-width chokes the table.
     This style block ships only with the EEOSetup view, so the unscoped
     override is effectively page-scoped — same approach the user takes in
     Mediation Statistics (module.css line 31-32). --%>
<style>
    .container,
    body .container,
    main .container {
        max-width: 1920px !important;
        padding-left: 16px;
        padding-right: 16px;
    }
</style>

<div class="container empdb-eeo-wide">

    <%-- DNN Web API context (TabId/ModuleId/AntiForgery token) for the JS layer. --%>
    <script type="text/javascript">
        window.__empdbCtx = {
            tabId: <%= TabId %>,
            moduleId: <%= ModuleId %>
        };
    </script>

    <h3><i class="fas fa-chart-bar"></i>&nbsp;EEO Setup</h3>

    <div class="tabs">
        <ul class="nav nav-tabs" id="eeoTabs" role="tablist">
            <li class="nav-item active">
                <a class="nav-link active" href="#pane-eeo-list" data-bs-toggle="tab" data-toggle="tab">EEO List</a>
            </li>
            <li class="nav-item">
                <a class="nav-link" href="#pane-eeo-review" data-bs-toggle="tab" data-toggle="tab">Review This Year's EEO Data</a>
            </li>
        </ul>

        <div class="tab-content">
            <div class="tab-pane active" id="pane-eeo-list">
                <button type="button" id="empdbEeoAdd" class="btn btn-success me-3">
                    <i class="fas fa-plus"></i>&nbsp;Add EEO Row
                </button>
                <%-- Compact column codes from the legacy view:
                       A = Population, C = Hire, D = Promotion, E = Transfer, F = Term
                       M = Male, F = Female, W = White, B = Black, A = Asian,
                       H = Hispanic, O = Other, I = Native American (Indian)
                     Hover any header for the full description. --%>
                <table id="table-eeo-list" class="table table-striped table-hover table-sm empdb-eeo-table" style="width:100%">
                    <thead>
                        <tr>
                            <th class="command-item no-sort"></th>
                            <th>Job Category</th>
                            <th>Year</th>
                            <th title="Population Male">A<br />M</th>
                            <th title="Population Female">A<br />F</th>
                            <th title="Population White">A<br />W</th>
                            <th title="Population Black">A<br />B</th>
                            <th title="Population Asian">A<br />A</th>
                            <th title="Population Hispanic">A<br />H</th>
                            <th title="Population Other">A<br />O</th>
                            <th title="Population Native American">A<br />I</th>
                            <th title="Hire Male">C<br />M</th>
                            <th title="Hire Female">C<br />F</th>
                            <th title="Hire White">C<br />W</th>
                            <th title="Hire Black">C<br />B</th>
                            <th title="Hire Asian">C<br />A</th>
                            <th title="Hire Hispanic">C<br />H</th>
                            <th title="Hire Other">C<br />O</th>
                            <th title="Hire Native American">C<br />I</th>
                            <th title="Promotions Male">D<br />M</th>
                            <th title="Promotions Female">D<br />F</th>
                            <th title="Promotions White">D<br />W</th>
                            <th title="Promotions Black">D<br />B</th>
                            <th title="Promotions Asian">D<br />A</th>
                            <th title="Promotions Hispanic">D<br />H</th>
                            <th title="Promotions Other">D<br />O</th>
                            <th title="Promotions Native American">D<br />I</th>
                            <th title="Transfers Male">E<br />M</th>
                            <th title="Transfers Female">E<br />F</th>
                            <th title="Transfers White">E<br />W</th>
                            <th title="Transfers Black">E<br />B</th>
                            <th title="Transfers Asian">E<br />A</th>
                            <th title="Transfers Hispanic">E<br />H</th>
                            <th title="Transfers Other">E<br />O</th>
                            <th title="Transfers Native American">E<br />I</th>
                            <th title="Terminations Male">F<br />M</th>
                            <th title="Terminations Female">F<br />F</th>
                            <th title="Terminations White">F<br />W</th>
                            <th title="Terminations Black">F<br />B</th>
                            <th title="Terminations Asian">F<br />A</th>
                            <th title="Terminations Hispanic">F<br />H</th>
                            <th title="Terminations Other">F<br />O</th>
                            <th title="Terminations Native American">F<br />I</th>
                            <th class="command-item no-sort"></th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr><td colspan="44" class="text-muted text-center">Loading…</td></tr>
                    </tbody>
                </table>
            </div>

            <div class="tab-pane" id="pane-eeo-review">
                <%-- Reporting Window: Bootstrap collapse panel. Header acts
                     as the toggle (caret icon flips via CSS based on
                     aria-expanded). Padded body, margin above the action
                     buttons. --%>
                <div class="card mt-3 empdb-reporting-window">
                    <div class="card-header empdb-reporting-window-header collapse-toggle"
                         role="button"
                         data-bs-toggle="collapse"
                         data-bs-target="#empdbReportingWindowBody"
                         aria-expanded="true"
                         aria-controls="empdbReportingWindowBody">
                        <strong><i class="fas fa-calendar"></i>&nbsp;Reporting Window</strong>
                        <span class="empdb-reporting-window-caret">
                            <i class="fas fa-chevron-up icon-expanded"></i>
                            <i class="fas fa-chevron-down icon-collapsed"></i>
                        </span>
                    </div>
                    <div id="empdbReportingWindowBody" class="collapse show">
                        <div class="card-body p-4">
                            <div class="row">
                                <div class="col-md-3">
                                    <label for="<%= dpStartDate.ClientID %>" class="fw-bold">Start Date</label>
                                    <asp:TextBox ID="dpStartDate" runat="server" CssClass="form-control" TextMode="SingleLine" type="date" />
                                </div>
                                <div class="col-md-3">
                                    <label for="<%= dpEndDate.ClientID %>" class="fw-bold">End Date</label>
                                    <asp:TextBox ID="dpEndDate" runat="server" CssClass="form-control" TextMode="SingleLine" type="date" />
                                </div>
                                <div class="col-md-3">
                                    <label for="<%= txtYear.ClientID %>" class="fw-bold">Year</label>
                                    <asp:TextBox ID="txtYear" runat="server" CssClass="form-control" />
                                </div>
                            </div>
                            <div class="row mt-4">
                                <div class="col-12">
                                    <asp:LinkButton ID="btnStart" runat="server" CssClass="btn btn-primary" OnClick="btnStart_Click">
                                        <i class="fas fa-calculator"></i>&nbsp;Check EEO Values
                                    </asp:LinkButton>
                                    <asp:LinkButton ID="btnAccept" runat="server" CssClass="btn btn-success ms-2" OnClick="btnAccept_Click" Visible="false">
                                        <i class="fas fa-save"></i>&nbsp;Publish Results
                                    </asp:LinkButton>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                <asp:Panel ID="pnlPreview" runat="server" Visible="false">
                    <br />
                    <h4>Preview</h4>
                    <asp:Repeater ID="rptPreview" runat="server">
                        <HeaderTemplate>
                            <table id="table-eeo-preview" class="table table-striped empdb-eeo-table" style="width:100%">
                                <thead>
                                    <tr>
                                        <th>Job Category</th>
                                        <th title="Population Male">A<br />M</th>
                                        <th title="Population Female">A<br />F</th>
                                        <th title="Population White">A<br />W</th>
                                        <th title="Population Black">A<br />B</th>
                                        <th title="Population Hispanic">A<br />H</th>
                                        <th title="Population Asian">A<br />A</th>
                                        <th title="Population Native American">A<br />I</th>
                                        <th title="Population Other">A<br />O</th>
                                        <th title="Hires (Male / Female)">C<br />M&#8209;F</th>
                                        <th title="Promotions (Male / Female)">D<br />M&#8209;F</th>
                                        <th title="Terminations (Male / Female)">F<br />M&#8209;F</th>
                                    </tr>
                                </thead>
                                <tbody>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <tr>
                                <td><%# Eval("JobGroupName") %></td>
                                <td><%# Eval("PopulationMale") %></td>
                                <td><%# Eval("PopulationFemale") %></td>
                                <td><%# Eval("PopulationWhite") %></td>
                                <td><%# Eval("PopulationBlack") %></td>
                                <td><%# Eval("PopulationHispanic") %></td>
                                <td><%# Eval("PopulationAsian") %></td>
                                <td><%# Eval("PopulationIndian") %></td>
                                <td><%# Eval("PopulationOther") %></td>
                                <td><%# Eval("HireMale") %>/<%# Eval("HireFemale") %></td>
                                <td><%# Eval("PromoMale") %>/<%# Eval("PromoFemale") %></td>
                                <td><%# Eval("TermMale") %>/<%# Eval("TermFemale") %></td>
                            </tr>
                        </ItemTemplate>
                        <FooterTemplate>
                                </tbody>
                            </table>
                        </FooterTemplate>
                    </asp:Repeater>
                </asp:Panel>
            </div>
        </div>
    </div>

    <%-- Add / Edit EEO modal — wide, with the 35 numeric inputs grouped into
         5 categories (Population / Hire / Promotion / Transfer / Termination).
         Each category has 8 race-or-gender columns; the Indian (Native
         American) column is appended after the legacy seven so existing
         habits aren't disturbed. --%>
    <div class="modal fade" id="EeoEditModal" tabindex="-1" aria-hidden="true">
        <div class="modal-dialog modal-xl">
            <div class="modal-content">
                <div class="modal-header"><h5 class="modal-title">EEO Row</h5><button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button></div>
                <div class="modal-body">
                    <input type="hidden" name="EeoId" value="0" />
                    <div class="row mb-3">
                        <div class="col-md-6">
                            <label>Job Category:</label>
                            <select name="JobGroupId" class="form-control">
                                <option value=""></option>
                                <%= GetJobGroupOptions() %>
                            </select>
                        </div>
                        <div class="col-md-3">
                            <label>Year:</label>
                            <input type="number" name="Year" class="form-control" min="1900" max="2999" step="1" />
                        </div>
                    </div>

                    <%-- One <fieldset> per category. The eight inputs per row
                         each carry a name like Population_M / Hire_F etc.
                         The JS readForm/fillForm map these to the model's
                         PopulationMale / HireFemale property names. --%>
                    <fieldset class="empdb-eeo-fieldset mb-3">
                        <legend class="empdb-eeo-legend">Population</legend>
                        <div class="row">
                            <div class="col"><label>Male</label><input type="number" name="Population_M" class="form-control" min="0" step="1" /></div>
                            <div class="col"><label>Female</label><input type="number" name="Population_F" class="form-control" min="0" step="1" /></div>
                            <div class="col"><label>White</label><input type="number" name="Population_W" class="form-control" min="0" step="1" /></div>
                            <div class="col"><label>Black</label><input type="number" name="Population_B" class="form-control" min="0" step="1" /></div>
                            <div class="col"><label>Asian</label><input type="number" name="Population_A" class="form-control" min="0" step="1" /></div>
                            <div class="col"><label>Hispanic</label><input type="number" name="Population_H" class="form-control" min="0" step="1" /></div>
                            <div class="col"><label>Other</label><input type="number" name="Population_O" class="form-control" min="0" step="1" /></div>
                            <div class="col"><label title="Native American">Indian</label><input type="number" name="Population_I" class="form-control" min="0" step="1" /></div>
                        </div>
                    </fieldset>

                    <fieldset class="empdb-eeo-fieldset mb-3">
                        <legend class="empdb-eeo-legend">Hires</legend>
                        <div class="row">
                            <div class="col"><label>Male</label><input type="number" name="Hire_M" class="form-control" min="0" step="1" /></div>
                            <div class="col"><label>Female</label><input type="number" name="Hire_F" class="form-control" min="0" step="1" /></div>
                            <div class="col"><label>White</label><input type="number" name="Hire_W" class="form-control" min="0" step="1" /></div>
                            <div class="col"><label>Black</label><input type="number" name="Hire_B" class="form-control" min="0" step="1" /></div>
                            <div class="col"><label>Asian</label><input type="number" name="Hire_A" class="form-control" min="0" step="1" /></div>
                            <div class="col"><label>Hispanic</label><input type="number" name="Hire_H" class="form-control" min="0" step="1" /></div>
                            <div class="col"><label>Other</label><input type="number" name="Hire_O" class="form-control" min="0" step="1" /></div>
                            <div class="col"><label title="Native American">Indian</label><input type="number" name="Hire_I" class="form-control" min="0" step="1" /></div>
                        </div>
                    </fieldset>

                    <fieldset class="empdb-eeo-fieldset mb-3">
                        <legend class="empdb-eeo-legend">Promotions</legend>
                        <div class="row">
                            <div class="col"><label>Male</label><input type="number" name="Promo_M" class="form-control" min="0" step="1" /></div>
                            <div class="col"><label>Female</label><input type="number" name="Promo_F" class="form-control" min="0" step="1" /></div>
                            <div class="col"><label>White</label><input type="number" name="Promo_W" class="form-control" min="0" step="1" /></div>
                            <div class="col"><label>Black</label><input type="number" name="Promo_B" class="form-control" min="0" step="1" /></div>
                            <div class="col"><label>Asian</label><input type="number" name="Promo_A" class="form-control" min="0" step="1" /></div>
                            <div class="col"><label>Hispanic</label><input type="number" name="Promo_H" class="form-control" min="0" step="1" /></div>
                            <div class="col"><label>Other</label><input type="number" name="Promo_O" class="form-control" min="0" step="1" /></div>
                            <div class="col"><label title="Native American">Indian</label><input type="number" name="Promo_I" class="form-control" min="0" step="1" /></div>
                        </div>
                    </fieldset>

                    <fieldset class="empdb-eeo-fieldset mb-3">
                        <legend class="empdb-eeo-legend">Transfers</legend>
                        <div class="row">
                            <div class="col"><label>Male</label><input type="number" name="Transfer_M" class="form-control" min="0" step="1" /></div>
                            <div class="col"><label>Female</label><input type="number" name="Transfer_F" class="form-control" min="0" step="1" /></div>
                            <div class="col"><label>White</label><input type="number" name="Transfer_W" class="form-control" min="0" step="1" /></div>
                            <div class="col"><label>Black</label><input type="number" name="Transfer_B" class="form-control" min="0" step="1" /></div>
                            <div class="col"><label>Asian</label><input type="number" name="Transfer_A" class="form-control" min="0" step="1" /></div>
                            <div class="col"><label>Hispanic</label><input type="number" name="Transfer_H" class="form-control" min="0" step="1" /></div>
                            <div class="col"><label>Other</label><input type="number" name="Transfer_O" class="form-control" min="0" step="1" /></div>
                            <div class="col"><label title="Native American">Indian</label><input type="number" name="Transfer_I" class="form-control" min="0" step="1" /></div>
                        </div>
                    </fieldset>

                    <fieldset class="empdb-eeo-fieldset mb-3">
                        <legend class="empdb-eeo-legend">Terminations</legend>
                        <div class="row">
                            <div class="col"><label>Male</label><input type="number" name="Term_M" class="form-control" min="0" step="1" /></div>
                            <div class="col"><label>Female</label><input type="number" name="Term_F" class="form-control" min="0" step="1" /></div>
                            <div class="col"><label>White</label><input type="number" name="Term_W" class="form-control" min="0" step="1" /></div>
                            <div class="col"><label>Black</label><input type="number" name="Term_B" class="form-control" min="0" step="1" /></div>
                            <div class="col"><label>Asian</label><input type="number" name="Term_A" class="form-control" min="0" step="1" /></div>
                            <div class="col"><label>Hispanic</label><input type="number" name="Term_H" class="form-control" min="0" step="1" /></div>
                            <div class="col"><label>Other</label><input type="number" name="Term_O" class="form-control" min="0" step="1" /></div>
                            <div class="col"><label title="Native American">Indian</label><input type="number" name="Term_I" class="form-control" min="0" step="1" /></div>
                        </div>
                    </fieldset>
                </div>
                <div class="modal-footer">
                    <button type="button" id="empdbEeoSave" class="btn btn-primary">Save</button>
                    <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancel</button>
                </div>
            </div>
        </div>
    </div>
</div>

<script type="text/javascript">
    // The Review tab still does a postback, so we re-init the preview table
    // after each async update so DataTables stays fresh. The main #table-eeo-list
    // is fully API-driven and managed by empdb-eeo.js.
    function InitEeoPreviewTable() {
        jQuery(document).ready(function ($) {
            if ($.fn.DataTable && $('#table-eeo-preview').length && !$.fn.DataTable.isDataTable('#table-eeo-preview')) {
                $('#table-eeo-preview').DataTable({
                    "order": [[0, "asc"]],
                    "pageLength": 25,
                    "scrollX": true
                });
            }
        });
    }
    InitEeoPreviewTable();

    if (typeof Sys !== 'undefined') {
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
            if (jQuery.fn.DataTable && jQuery.fn.DataTable.isDataTable('#table-eeo-preview')) {
                jQuery('#table-eeo-preview').DataTable().destroy();
            }
            InitEeoPreviewTable();
        });
    }

    // ---- Tab persistence across postbacks ----
    // The EEO List tab is API-driven, but the Review tab still does full
    // ASP.NET postbacks (Check EEO Values / Publish Results LinkButtons).
    // After each postback the page re-renders with the EEO List tab marked
    // active server-side, which yanks the user away from the Review tab
    // they were just on. Track the current tab in sessionStorage and
    // restore it on every page load so the user stays where they were.
    (function ($) {
        var EEO_TAB_KEY = "empdbEeoActiveTab";

        $(document).off("click.eeoTab").on(
            "click.eeoTab",
            "#eeoTabs .nav-link[data-toggle=tab], #eeoTabs .nav-link[data-bs-toggle=tab]",
            function () {
                var href = this.getAttribute("href");
                if (href) { try { sessionStorage.setItem(EEO_TAB_KEY, href); } catch (e) {} }
            }
        );

        function restoreActiveTab() {
            var href;
            try { href = sessionStorage.getItem(EEO_TAB_KEY); } catch (e) { return; }
            if (!href || href === "#pane-eeo-list") return;   // default — nothing to do

            var $link = $('#eeoTabs .nav-link[href="' + href + '"]');
            var $pane = $(href);
            if (!$link.length || !$pane.length) return;
            if ($pane.hasClass("active")) return;             // already showing it

            // Use jQuery's .tab() (works in both BS4 and BS5 via the bundled
            // jquery plugin) to switch tabs cleanly.
            if ($.fn.tab) { $link.tab("show"); return; }

            // Fallback: swap the active classes manually.
            $('#eeoTabs .nav-link.active, #eeoTabs .nav-item.active').removeClass("active");
            $('.tab-content > .tab-pane.active').removeClass("active show");
            $link.addClass("active").closest(".nav-item").addClass("active");
            $pane.addClass("active show");
        }

        $(restoreActiveTab);
        if (typeof Sys !== 'undefined') {
            Sys.WebForms.PageRequestManager.getInstance().add_endRequest(restoreActiveTab);
        }
    })(jQuery);
</script>

<dnn:DnnCssInclude runat="server" FilePath="/Resources/Libraries/DataTables/dataTables.bootstrap5.min.css" />
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/DataTables/dataTables.min.js" />
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/DataTables/dataTables.bootstrap5.min.js" />
