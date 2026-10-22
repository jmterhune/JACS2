<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ExcludedAttorneyList.ascx.cs" Inherits="tjc.Modules.JacsCaseMaint.ExcludedAttorneyList" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<%-- SweetAlert2 + Noty for confirms / toast notifications --%>
<dnn:DnnCssInclude runat="server" FilePath="/Resources/Libraries/sweetalert/sweetalert2.min.css" />
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/sweetalert/sweetalert2.all.min.js" />
<dnn:DnnCssInclude runat="server" FilePath="/Resources/Libraries/Noty/noty.min.css" />
<dnn:DnnCssInclude runat="server" FilePath="/Resources/Libraries/Noty/bootstrap-v4.min.css" />
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/Noty/noty.min.js" />
<asp:HiddenField ID="hdnActiveTab" runat="server" Value="excluded" />
<ul class="nav nav-tabs mb-3" id="attorneyTabs" role="tablist">
    <li class="nav-item" role="presentation">
        <button class="nav-link" id="excluded-tab" data-tab="excluded" data-bs-toggle="tab" data-bs-target="#excludedPane" type="button" role="tab" aria-controls="excludedPane">Excluded Attorneys</button>
    </li>
    <li class="nav-item" role="presentation">
        <button class="nav-link" id="search-tab" data-tab="search" data-bs-toggle="tab" data-bs-target="#searchPane" type="button" role="tab" aria-controls="searchPane">Attorney Status</button>
    </li>
</ul>
<div class="tab-content">
    <div class="tab-pane fade" id="excludedPane" role="tabpanel" aria-labelledby="excluded-tab">
        <div class="alert alert-info"><i class="fa fa-info-circle"></i>&nbsp;The list below displays the barnumbers that have been excluded from the Florida Bar Import routine.</div>

        <button type="button" class="btn btn-primary" data-bs-toggle="modal" data-bs-target="#formModal">Add Attorney to Exclude </button>
        <hr />

        <asp:Repeater ID="rptAttorneyList" runat="server" OnItemCommand="rptAttorneyList_ItemCommand">
            <HeaderTemplate>
                <table class="table table-striped">
                    <thead>
                        <tr>
                            <th>ID</th>
                            <th>Bar Number</th>
                            <th>Name</th>
                            <th>Email</th>
                            <th>Active</th>
                            <th>&nbsp;</th>
                        </tr>
                    </thead>
                    <tbody>
            </HeaderTemplate>
            <ItemTemplate>
                <tr>
                    <td class="command-icon-container"><%#Eval("RecordId") %></td>
                    <td><%#Eval("barnumber") %></td>
                    <td><%#Eval("NAME") %></td>
                    <td><%#Eval("EMAIL") %></td>
                    <td><%#Eval("ACTIVE") %></td>
                    <td class="command-icon-container">
                        <asp:LinkButton runat="server" CausesValidation="false" ID="cmdDelete" ToolTip="Delete Record" CssClass="command-icon text-danger" OnClientClick="return Jud12ConfirmPostback(this, 'Delete Bar Number?', 'Delete?');" CommandArgument='<%#Eval("RecordId") %>' CommandName="delete"><i class="fas fa-trash"></i></asp:LinkButton>
                    </td>
                </tr>
            </ItemTemplate>
            <FooterTemplate>
                </tbody></table>
            </FooterTemplate>
        </asp:Repeater>
    </div>
    <div class="tab-pane fade" id="searchPane" role="tabpanel" aria-labelledby="search-tab">
        <div class="alert alert-info"><i class="fa fa-info-circle"></i>&nbsp;Search for an attorney by bar number. Use the switch in the Active column to activate or deactivate the attorney in all three JACS databases.</div>
        <asp:Panel ID="pnlSearch" runat="server" DefaultButton="cmdSearch" CssClass="row g-3 align-items-end">
            <div class="col-auto">
                <asp:Label ID="lblSearchBarNumber" CssClass="visually-hidden" AssociatedControlID="txtSearchBarNumber" runat="server" Text="Bar Number"></asp:Label>
                <asp:TextBox ID="txtSearchBarNumber" CssClass="form-control" runat="server" MaxLength="7" placeholder="Bar Number"></asp:TextBox>
            </div>
            <div class="col-auto">
                <asp:Button ID="cmdSearch" OnClick="cmdSearch_Click" runat="server" CausesValidation="false" CssClass="btn btn-primary" Text="Search" />
                <asp:Button ID="cmdClear" OnClick="cmdClear_Click" runat="server" CausesValidation="false" CssClass="btn btn-default" Text="Clear" />
            </div>
        </asp:Panel>
        <hr />
        <asp:Repeater ID="rptSearchResults" runat="server" OnItemDataBound="rptSearchResults_ItemDataBound">
            <HeaderTemplate>
                <table class="table table-striped">
                    <thead>
                        <tr>
                            <th>Bar Number</th>
                            <th>Name</th>
                            <th>Email</th>
                            <th>Phone</th>
                            <th>Active</th>
                        </tr>
                    </thead>
                    <tbody>
            </HeaderTemplate>
            <ItemTemplate>
                <tr>
                    <td><%#Eval("BARNUM") %></td>
                    <td><%#Eval("NAME") %></td>
                    <td><%#Eval("EMAIL") %></td>
                    <td><%#Eval("PHONENUM") %></td>
                    <td>
                        <asp:HiddenField ID="hdnBarNumber" runat="server" Value='<%#Eval("BARNUM") %>' />
                        <div class="form-check form-switch mb-0">
                            <asp:CheckBox ID="chkActive" runat="server" AutoPostBack="true" OnCheckedChanged="chkActive_CheckedChanged" Checked='<%#Eval("IsActive") %>' />
                        </div>
                    </td>
                </tr>
            </ItemTemplate>
            <FooterTemplate>
                </tbody></table>
            </FooterTemplate>
        </asp:Repeater>
        <asp:Panel ID="pnlNoResults" runat="server" Visible="false" CssClass="alert alert-warning">
            No attorney found for bar number <asp:Literal ID="litNoResultsBarNumber" runat="server" />.
        </asp:Panel>
    </div>
</div>
<div class="modal fade" id="formModal" tabindex="-1" role="dialog" aria-labelledby="formModalLabel" aria-hidden="true">
    <div class="modal-dialog">
        <div class="modal-content">
            <div class="modal-header">
                <h4 class="modal-title" id="formModalLabel">Add Bar Number to Exclude</h4>
                <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
            </div>
            <div class="modal-body">
                <div class="mt-lg">
                    <div class="row">
                        <div class="col-sm-12">
                            <asp:Label ID="lblBarnumber" runat="server" Text="Bar Number" AssociatedControlID="txtBarNumber"></asp:Label>
                            <asp:TextBox runat="server" ID="txtBarNumber" CssClass="form-control" MaxLength="10" />
                            <asp:RequiredFieldValidator ErrorMessage="Bar Number is Required" ControlToValidate="txtBarNumber" ValidationGroup="ExcludeAttorney" CssClass="label label-danger" Display="Dynamic" runat="server" />
                        </div>
                    </div>
                </div>
            </div>
            <div class="modal-footer justify-content-between">
                <asp:Button ID="cmdSave" CssClass="btn btn-primary" runat="server" OnClick="cmdSave_Click" ValidationGroup="ExcludeAttorney" Text="Save" />
                <button type="button" class="btn btn-default" data-bs-dismiss="modal">Close</button>
            </div>
        </div>
    </div>
</div>
<script type="text/javascript">
    (function () {
        // Keep the selected tab across postbacks
        var hidden = document.getElementById('<%= hdnActiveTab.ClientID %>');
        var tabs = document.querySelectorAll('#attorneyTabs [data-tab]');
        var active = document.querySelector('#attorneyTabs [data-tab="' + (hidden.value || 'excluded') + '"]') || tabs[0];
        tabs.forEach(function (tab) {
            tab.addEventListener('shown.bs.tab', function () { hidden.value = tab.getAttribute('data-tab'); });
        });
        if (window.bootstrap && bootstrap.Tab) {
            bootstrap.Tab.getOrCreateInstance(active).show();
        } else {
            active.classList.add('active');
            document.querySelector(active.getAttribute('data-bs-target')).classList.add('show', 'active');
        }
    })();

    function Jud12ConfirmPostback(btn, msg, title) {
        if (!window.Swal) { return window.confirm(msg); }
        if (btn && btn.dataset && btn.dataset.jud12Confirmed === '1') {
            btn.dataset.jud12Confirmed = '';
            return true;
        }
        Swal.fire({
            title: title || 'Confirm', text: msg, icon: 'warning',
            showCancelButton: true, confirmButtonText: 'Yes', cancelButtonText: 'No',
            confirmButtonColor: '#d33'
        }).then(function (r) {
            if (!r.isConfirmed) return;
            var href = btn.href || '';
            var m = href.match(/__doPostBack\(['"]([^'"]+)['"],\s*['"]([^'"]*)['"]\)/);
            if (m && typeof __doPostBack === 'function') {
                __doPostBack(m[1], m[2]);
            } else if (btn && btn.tagName === 'INPUT' && (btn.type === 'submit' || btn.type === 'button')) {
                btn.dataset.jud12Confirmed = '1';
                btn.click();
            } else if (btn && typeof btn.click === 'function') {
                btn.dataset.jud12Confirmed = '1';
                btn.click();
            }
        });
        return false;
    }
</script>
