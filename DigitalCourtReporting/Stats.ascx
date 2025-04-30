<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Stats.ascx.cs" Inherits="tjc.Modules.DigitalCourtReporting.Stats" %>
<div class="btn-group mb-2" role="group" aria-label="Button group with nested dropdown">
    <a class="btn btn-primary" id="lnkAccounting" href='<%=AccountingUrl %>'>Accounting</a>
    <div class="btn-group" role="group">
        <button id="btnGroupDropInquiry" type="button" class="btn btn-primary dropdown-toggle" data-bs-toggle="dropdown" aria-expanded="false">
            Inquiry
        </button>
        <ul class="dropdown-menu" aria-labelledby="btnGroupDropInquiry">
            <li>
                <a class="dropdown-item" id="lnkInquiry" href="<%=InquiryUrl %>">All Counties</a></li>
            <li>
                <a class="dropdown-item" id="lnkInqDesoto" href="<%=InquiryDeSotoUrl %>">Desoto</a></li>
            <li>
                <a class="dropdown-item" id="lnkInqManatee" href="<%=InquiryManateeUrl %>">Manatee</a></li>
            <li>
                <a class="dropdown-item" id="lnkInqSarasota" href="<%=InquirySarasotaUrl %>">Sarasota</a></li>
        </ul>
    </div>
    <div class="btn-group" role="group">
        <button id="btnGroupDropDCR" type="button" class="btn btn-primary dropdown-toggle" data-bs-toggle="dropdown" aria-expanded="false">
            DCR
        </button>
        <ul class="dropdown-menu" aria-labelledby="btnGroupDropDCR">
            <li>
                <a class="dropdown-item" id="lnkDCR" href="<%=DCRUrl %>">All Counties</a></li>
            <li>
                <a class="dropdown-item" id="lnkDCRDesoto" href="<%=DCRDeSotoUrl %>">Desoto</a></li>
            <li>
                <a class="dropdown-item" id="lnkDCRManatee" href="<%=DCRManateeUrl %>">Manatee</a></li>
            <li>
                <a class="dropdown-item" id="lnkDCRSarasota" href="<%=DCRSarasotaUrl %>">Sarasota</a></li>
        </ul>
    </div>
    <a class="btn btn-primary" id="lnkNotification" href="<%=NotificationUrl %>">Notification</a>
    <a class="btn btn-primary" id="lnkStats" href="<%=StatsUrl %>">Stats</a>
    <div class="btn-group" role="group">
        <button id="btnGroupDropComplete" type="button" class="btn btn-primary dropdown-toggle" data-bs-toggle="dropdown" aria-expanded="false">
            Complete
        </button>
        <ul class="dropdown-menu" aria-labelledby="btnGroupDropComplete">
            <li>
                <a class="dropdown-item" id="lnkComplete" href="<%=CompleteUrl %>">All Counties</a></li>
            <li>
                <a class="dropdown-item" id="lnkCompDesoto" href="<%=CompleteDeSotoUrl %>">Desoto</a></li>
            <li>
                <a class="dropdown-item" id="lnkCompManatee" href="<%=CompleteManateeUrl %>">Manatee</a></li>
            <li>
                <a class="dropdown-item" id="lnkCompSarasota" href="<%=CompleteSarasotaUrl %>">Sarasota</a></li>
        </ul>
    </div>
</div>
<div class="heading heading-border heading-middle-border heading-middle-border-center heading-border-lg mb-1">
    <h2>Court Reporting Statistical Report</h2>
</div>
<div class="btn-group" id="SearchCriteria" role="group" aria-label="Search">
    <div id="swSearchCriteria" class="input-group">
        <div class="input-group-prepend">
            <span class="input-group-text bg-dark text-white text-nowrap">Statistics Criteria:</span>
        </div>
        <asp:TextBox AutoCompleteType="Disabled" ID="txtCriteriaStartDate" ClientIDMode="Static" placeholder="mm/dd/yyy" runat="server" CssClass="form-control date-picker" MaxLength="15"></asp:TextBox>
        <asp:TextBox AutoCompleteType="Disabled" ID="txtCriteriaEndDate" ClientIDMode="Static" placeholder="mm/dd/yyy" runat="server" CssClass="form-control date-picker" MaxLength="15"></asp:TextBox>
        <asp:DropDownList runat="server" ID="drpCriteriaCounty" CssClass="form-control" AppendDataBoundItems="true" ClientIDMode="Static">
            <asp:ListItem Text="< Select County >" Value="-1" />
        </asp:DropDownList>
    </div>
    <asp:Button Text="Submit" CssClass="btn btn-dark" ClientIDMode="Static" ID="cmdSearch" OnClick="cmdSearch_Click" runat="server" />
</div>
<div>
    <table id="stats" class="table table-striped">
        <thead>
            <tr>
                <th>Media Provided (CD or Audio)</th>
                <th width="30">#</th>
                <th width="30">Min Burned</th>
            </tr>
        </thead>
        <tbody>
            <asp:Repeater ID="rptStats" runat="server">
                <ItemTemplate>
                    <tr>
                        <th><%#Eval("heading")%></th>
                        <td><%#Eval("TotalNumber")%></td>
                        <td><%#Eval("minburned")%></td>
                    </tr>
                </ItemTemplate>
            </asp:Repeater>
            <tr class="total">
                <th>Total</th>
                <td>
                    <asp:Label ID="lblTotal" runat="server" /></td>
                <td>
                    <asp:Label ID="lblMinTotal" runat="server" /></td>
            </tr>
            <tr class="utp">
                <th>Unable to Process</th>
                <td>
                    <asp:Label ID="lblUTP" runat="server" /></td>
                <td>&nbsp;</td>
            </tr>
        </tbody>
    </table>
</div>
<script type="text/javascript">
    /*globals jQuery, window, Sys */
    (function ($, Sys) {

        $(document).ready(function () {
            $(".date-picker").on("blur", function (e) {
                var date = $(this).val();
                $(this).val(date.replace(/\.|-/g, "/"));
            });
        });

    }(jQuery, window.Sys));
</script>
