<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Holidays.ascx.cs" Inherits="tjc.Modules.JudgeVacation.Holidays" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<%-- SweetAlert2 + Noty for confirms / toast notifications --%>
<dnn:DnnCssInclude runat="server" FilePath="/Resources/Libraries/sweetalert/sweetalert2.min.css" />
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/sweetalert/sweetalert2.all.min.js" />
<dnn:DnnCssInclude runat="server" FilePath="/Resources/Libraries/Noty/noty.min.css" />
<dnn:DnnCssInclude runat="server" FilePath="/Resources/Libraries/Noty/bootstrap-v4.min.css" />
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/Noty/noty.min.js" />

<div class="form-container">
    <div class="row g-3" id="vacation-form">
        <div class="col-auto">
            <asp:Label runat="server" AssociatedControlID="HolidayDatePicker" ResourceKey="HolidayDate" />
            <asp:TextBox runat="server" CssClass="form-control date-picker" ID="HolidayDatePicker" />
        </div>
        <div class="col-auto">
            <asp:Label runat="server" AssociatedControlID="txtDescription" ResourceKey="Description" />
            <asp:TextBox runat="server" ID="txtDescription" CssClass="form-control" MaxLength="50" />
        </div>
    </div>
    <p class="mt-3">
        <asp:LinkButton ID="cmdSave" runat="server" CssClass="btn btn-primary" ResourceKey="Save" OnClick="cmdSave_Click" />
        <asp:HyperLink ID="cmdCancel" runat="server" CssClass="btn btn-secondary" ResourceKey="Cancel" />
    </p>
    <hr />
    <asp:Panel ID="pnlRecords" runat="server">
        <div class="h4 row">Holidays
           <div class="col-auto"> <asp:DropDownList ID="drpYear" CssClass="form-control d-inline-block" runat="server" AutoPostBack="true" OnSelectedIndexChanged="drpYear_SelectedIndexChanged"></asp:DropDownList></div></div>
        <asp:Repeater ID="rptHolidays" runat="server" OnItemCommand="rptHolidays_ItemCommand">
            <HeaderTemplate>
                <table class="table table-striped">
                    <thead>
                        <tr>
                            <th>Date</th>
                            <th>Description</th>
                            <th>&nbsp;</th>
                        </tr>
                    </thead>
                    <tbody>
            </HeaderTemplate>
            <ItemTemplate>
                <tr>
                    <td><%# Eval("HolidayDate", "{0: MM/dd/yyyy}") %></td>
                    <td><%# Eval("Description") %></td>
                    <td class="command-item">
                        <asp:LinkButton CssClass="text-danger" ToolTip="Delete Holiday" runat="server" CommandArgument='<%# Eval("HolidayID") %>' CommandName="delete" OnClientClick="return Jud12ConfirmPostback(this, 'Are you sure you wish to delete this record?', 'Delete?');"><i class="fas fa-trash" aria-hidden="true" title="Delete Holiday"></i></asp:LinkButton></td>
                </tr>
            </ItemTemplate>
            <FooterTemplate>
                </tbody>
        </table>
            </FooterTemplate>
        </asp:Repeater>
    </asp:Panel>
</div>
<dnn:dnncssinclude runat="server" filepath="~/Resources/Shared/components/TimePicker/Themes/jquery-ui.min.css" />
<script type="text/javascript">
    $(".date-picker").datepicker();
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
