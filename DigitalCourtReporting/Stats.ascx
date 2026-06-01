<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Stats.ascx.cs" Inherits="tjc.Modules.DigitalCourtReporting.Stats" %>
<div id="navigationLinks" class="btn-group mb-2" role="group" aria-label="Button group with nested dropdown">
    <a class="btn btn-primary " id="lnkAccounting" href='<%=AccountingUrl %>'>Accounting</a>
    <a class="btn btn-primary" id="lnkInquiry" href="<%=InquiryUrl %>">Inquiry</a>
    <a class="btn btn-primary" id="lnkDCR" href="<%=DCRUrl %>">
        <abbr title="Digital Court Reporting">DCR</abbr></a>
    <a class="btn btn-primary" id="lnkNotification" href="<%=NotificationUrl %>">Notification</a>
    <a class="btn btn-primary active" id="lnkStats" href="<%=StatsUrl %>">Stats</a>
    <a class="btn btn-primary" id="lnkComplete" href="<%=CompleteUrl %>">Complete</a>
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
