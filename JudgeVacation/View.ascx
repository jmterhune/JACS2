<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="View.ascx.cs" Inherits="tjc.Modules.JudgeVacation.View" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<%-- SweetAlert2 + Noty for confirms / toast notifications --%>
<dnn:DnnCssInclude runat="server" FilePath="/Resources/Libraries/sweetalert/sweetalert2.min.css" />
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/sweetalert/sweetalert2.all.min.js" />
<dnn:DnnCssInclude runat="server" FilePath="/Resources/Libraries/Noty/noty.min.css" />
<dnn:DnnCssInclude runat="server" FilePath="/Resources/Libraries/Noty/bootstrap-v4.min.css" />
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/Noty/noty.min.js" />
<div class="form-container">
    <asp:HyperLink ID="lnkReports" runat="server" CssClass="btn btn-primary me-2 mb-2" Visible="false"><i class="fa-solid fa-chart-line" aria-hidden="true"></i>&nbsp;View Reports</asp:HyperLink>
    <asp:HyperLink ID="lnkHolidays" runat="server" CssClass="btn btn-tertiary mb-2" Visible="false"><i class="fa-solid fa-calendar-days"  aria-hidden="true"></i>&nbsp;Manage Holidays</asp:HyperLink>
    <div class="alert alert-info"><i class="fa fa-info-circle" aria-hidden="true"></i><strong>Please Note:</strong> This information is strictly for reporting purposes and is not integrated with Outlook or any other calendar source.</div>
    <div class="row g-3" id="vacation-form">

        <div class="col-auto">
            <asp:Label runat="server" AssociatedControlID="StartDatePicker" ResourceKey="StartDate" />
            <asp:TextBox runat="server" CssClass="form-control date-picker" ID="StartDatePicker" />
            <asp:RequiredFieldValidator runat="server" ControlToValidate="StartDatePicker"
                Display="Dynamic" SetFocusOnError="true" CssClass="label label-danger" ErrorMessage="Start Date is Required" />
        </div>
        <div class="col-auto">
            <asp:Label runat="server" AssociatedControlID="EndDatePicker" ResourceKey="EndDate" />
            <asp:TextBox runat="server" CssClass="form-control date-picker" ID="EndDatePicker" />
            <asp:RequiredFieldValidator runat="server" ControlToValidate="EndDatePicker"
                Display="Dynamic" SetFocusOnError="true" CssClass="label label-danger" ErrorMessage="End Date is Required" />
        </div>
        <div class="col-auto pt-4">
            <asp:Button ID="cmdSave" runat="server" CssClass="btn btn-primary mt-1" ResourceKey="Save" OnClick="cmdSave_Click" />
            <asp:Button ID="cmdUpdate" Visible="false" runat="server" CssClass="ms-1 mt-1 btn btn-primary" ResourceKey="Update" OnClick="cmdUpdate_Click" />
        </div>
    </div>
    <hr />
    <asp:Panel ID="pnlRecords" runat="server">
        <div class="h4 row">
            Vacation Records for
            <div class="col-auto">
                <asp:DropDownList ID="drpYear" CssClass="form-control d-inline-block" runat="server" AutoPostBack="true" OnSelectedIndexChanged="drpYear_SelectedIndexChanged"></asp:DropDownList></div>
        </div>
        <asp:Repeater ID="rptVacationDays" runat="server" OnItemCommand="rptVacationDays_ItemCommand" OnItemDataBound="rptVacationDays_ItemDataBound">
            <HeaderTemplate>
                <table class="table table-striped">
                    <thead>
                        <tr>
                            <th>&nbsp;</th>
                            <th>Start Date</th>
                            <th>End Date</th>
                            <th>Vacation Days Used</th>
                            <th>&nbsp;</th>
                        </tr>
                    </thead>
                    <tbody>
            </HeaderTemplate>
            <ItemTemplate>
                <tr>
                    <td class="command-item">
                        <asp:HyperLink data-toggle="tooltip" ID="lnkEdit" CssClass="text-primary" ToolTip="Edit Record" runat="server"><i title="Edit Record" class="fas fa-edit" aria-hidden="true"></i></asp:HyperLink>
                    </td>
                    <td><%# Eval("StartDate", "{0: MM/dd/yyyy}") %></td>
                    <td><%# Eval("EndDate", "{0: MM/dd/yyyy}") %></td>
                    <td><%# Eval("VacationDays") %></td>
                    <td class="command-item">
                        <asp:LinkButton data-toggle="tooltip" CssClass="text-danger" ToolTip="Delete Record" CausesValidation="false" runat="server" CommandArgument='<%# Eval("CalendarId") %>' CommandName="delete" OnClientClick="return Jud12ConfirmPostback(this, 'Are you sure you wish to delete this record?', 'Delete?');"><i title="Delete Record" class="fas fa-trash" aria-hidden="true"></i></asp:LinkButton></td>
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
    var link = document.getElementById("<%=cmdSave.ClientID %>");
    document.addEventListener('click', function (e) {
        if (e.target.id === link.id) {
            if (document.getElementById("<%=cmdSave.ClientID %>").disabled)
                e.preventDefault();
        }
    });
    function DisableButton() {
        document.getElementById("<%=cmdSave.ClientID %>").disabled = true;
        document.getElementById("<%=cmdSave.ClientID %>").innerHTML = "Processing...";
        setTimeout(() => {
            document.getElementById("<%=cmdSave.ClientID %>").disabled = false;
            document.getElementById("<%=cmdSave.ClientID %>").innerHTML = "Save";
        }, "3000");
    }
    window.onbeforeunload = DisableButton;

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
