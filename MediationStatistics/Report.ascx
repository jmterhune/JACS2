<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Report.ascx.cs" Inherits="tjc.Modules.MediationStatistics.Report" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<div id="report-form">
    <div class="row">
        <div class="col-auto">
            <div class="form-group">
                <asp:Label runat="server" AssociatedControlID="txtStartDate" Text="Start Date" />
                <asp:TextBox runat="server" ID="txtStartDate" MaxLength="15" ClientIDMode="Static" CssClass="form-control datepicker" />
            </div>
        </div>

        <div class="col-auto">
            <div class="form-group">
                <asp:Label runat="server" AssociatedControlID="txtEndDate" Text="End Date" />
                <asp:TextBox runat="server" ID="txtEndDate" MaxLength="15" ClientIDMode="Static" CssClass="form-control datepicker" />
            </div>
        </div>
        <div class="col-auto">
            <div class="form-group">
                <asp:Label runat="server" AssociatedControlID="drpReport" Text="Report" />
                <asp:DropDownList ID="drpReport" runat="server" ToolTip="Select Report to Run" CssClass="form-control" ClientIDMode="Static">
                    <asp:ListItem Text="Compendium" Value="0" />
                    <asp:ListItem Text="Fees Owed" Value="1" />
                    <asp:ListItem Text="Referral Sources" Value="2" />
                    <asp:ListItem Text="Check Stats" Value="3" />
                    <asp:ListItem Text="Collected &amp; Paid" Value="4" />
                </asp:DropDownList>
            </div>
        </div>
    </div>
</div>
<p>
    <asp:LinkButton ID="cmdReport" runat="server"
        OnClick="cmdReport_Click" CssClass="btn btn-primary btn-lg"><i class="fas fa-save"></i> Run Report</asp:LinkButton>
    <asp:HyperLink ID="lnkCancel" CssClass="btn btn-secondary btn-lg" runat="server"><i class="fas fa-redo"></i> Exit</asp:HyperLink>
</p>
<div id="Conpendium" runat="server" visible="false">
    <asp:Repeater ID="rptConpendium" runat="server" OnItemDataBound="rptConpendium_ItemDataBound">
        <FooterTemplate>
            </tbody></table>
        </FooterTemplate>
        <ItemTemplate>
            <asp:Literal ID="ltHeader" runat="server" />
            <tr>
                <td>
                    <asp:Literal ID="ltLineNumber" runat="server" />
                </td>
                <td>
                    <%#Eval("question") %>
                </td>
                <%#FormatNumber(Eval("sarasota").ToString(), Eval("sPercent").ToString())%>
                <%#FormatNumber(Eval("manatee").ToString(), Eval("mPercent").ToString())%>
                <%#FormatNumber(Eval("desoto").ToString(), Eval("dPercent").ToString())%>
                <%#FormatNumber(Eval("Southcounty").ToString(), "-1")%>
                <%#FormatNumber(Eval("Northcounty").ToString(), "-1")%>
            </tr>
        </ItemTemplate>
    </asp:Repeater>
</div>
<div id="FeesOwed" runat="server" visible="false">
    <asp:Repeater ID="rptFeesOwed" runat="server" OnItemDataBound="rptFeesOwed_ItemDataBound">
        <ItemTemplate>
            <asp:Literal ID="ltHeader" runat="server" />
            <tr>
                <td>
                    <%#Eval("CaseNumber") %>
                </td>
                <td>
                    <%#Eval("pFirstName") %>&nbsp;<%#Eval("pLastName")%>
                </td>
                <td>
                    <%#Eval("FeeOwed")%>
                </td>
                <td>
                    <%#Eval("aFirstName") %>&nbsp;<%#Eval("aLastName")%>
                </td>
                <td>
                    <%#Eval("Phone") %>
                    <%#Eval("FormattedExtension") %>
                </td>
                <td>
                    <%#Eval("FormattedAddress") %>
                    <%#Eval("FormattedCity") %>
                    <%#Eval("State") %>
                    <%#Eval("Zip")%>
                </td>
                <td>
                    <%#Eval("MediationDate","{0:d}") %>
                </td>
                <td>
                    <asp:CheckBox ID="chkAgreement" runat="server" Enabled="false" Checked='<%#Eval("FeeAgreement")%>' />
                </td>
                <td>
                    <asp:CheckBox ID="chkjudgment" runat="server" Enabled="false" Checked='<%#Eval("Feejudgement")%>' />
                </td>
                <td>
                    <asp:CheckBox ID="chkWaiver" runat="server" Enabled="false" Checked='<%#Eval("FeeWaiver")%>' />
                </td>
                <td>
                    <asp:CheckBox ID="chkOts" runat="server" Enabled="false" Checked='<%#Eval("OTS")%>' />
                </td>
                <td>
                    <asp:CheckBox ID="chkP1_FTA" runat="server" Enabled="false" Checked='<%#Eval("P1_FTA")%>' />
                </td>
                <td>
                    <asp:CheckBox ID="chkP2_FTA" runat="server" Enabled="false" Checked='<%#Eval("P2_FTA")%>' />
                </td>
            </tr>
        </ItemTemplate>
        <FooterTemplate>
            </tbody></table>
        </FooterTemplate>
    </asp:Repeater>
</div>
<div id="Referrals" runat="server" visible="false">
    <asp:Repeater ID="rptReferrals" runat="server">
        <FooterTemplate>
            </tbody></table>
        </FooterTemplate>
        <HeaderTemplate>
            <table class="table table-striped">
                <thead>
                    <tr>
                        <th>Referral Source
                        </th>
                    </tr>
                </thead>
                <tbody>
        </HeaderTemplate>
        <ItemTemplate>
            <tr>
                <td>
                    <%# Container.DataItem ?? string.Empty%>
                </td>
            </tr>
        </ItemTemplate>
    </asp:Repeater>
</div>
<div id="Checker" runat="server" visible="false">
    <asp:GridView ID="rgChecker" runat="server" CssClass="table table-striped">
        <Columns>
            <asp:BoundField DataField="Region" HeaderText="Region" ReadOnly="True" SortExpression="Region"></asp:BoundField>
            <asp:BoundField DataField="CaseTypeGroup" HeaderText="Case Group" ReadOnly="True" SortExpression="CaseTypeGroup">
                <ItemStyle Wrap="false" />
            </asp:BoundField>
            <asp:BoundField DataField="CaseNumber" HeaderText="Case Number" ReadOnly="True" SortExpression="CaseNumber">
                <ItemStyle Wrap="false" />
            </asp:BoundField>
            <asp:BoundField DataField="partyone" HeaderText="Party One" ReadOnly="True" SortExpression="partyone"></asp:BoundField>
            <asp:BoundField DataField="partytwo" HeaderText="Party Two" ReadOnly="True" SortExpression="partytwo"></asp:BoundField>
            <asp:BoundField DataField="ReferralDate" HeaderText="Referred" ReadOnly="True" SortExpression="ReferralDate"
                DataFormatString="{0:d}"></asp:BoundField>
            <asp:CheckBoxField DataField="MediationHeld" HeaderText="Mediation Held" ReadOnly="True" SortExpression="MediationHeld"></asp:CheckBoxField>
            <asp:BoundField DataField="MediationDate" HeaderText="Mediated" ReadOnly="True" SortExpression="MediationDate"
                DataFormatString="{0:d}"></asp:BoundField>
            <asp:BoundField DataField="Mediator" HeaderText="Mediator" ReadOnly="True" SortExpression="Mediator"></asp:BoundField>
            <asp:CheckBoxField DataField="AgreementReached" HeaderText="Agreement Reached" ReadOnly="True" SortExpression="AgreementReached"></asp:CheckBoxField>
            <asp:BoundField DataField="FeeAmount" HeaderText="Fee Amount" ReadOnly="True" SortExpression="FeeAmount"></asp:BoundField>
            <asp:CheckBoxField DataField="OTS" HeaderText="OTSC" ReadOnly="True" SortExpression="OTS">
                <HeaderStyle Wrap="false" />
            </asp:CheckBoxField>
            <asp:CheckBoxField DataField="FeeWaiver" HeaderText="Fee Waived" ReadOnly="True" SortExpression="FeeWaiver">
                <HeaderStyle Wrap="false" />
            </asp:CheckBoxField>
            <asp:CheckBoxField DataField="P1_FTA" HeaderText="P-FTA" ReadOnly="True" SortExpression="P1_FTA">
                <HeaderStyle Wrap="false" />
            </asp:CheckBoxField>
            <asp:CheckBoxField DataField="P2_FTA" HeaderText="R-FTA" ReadOnly="True" SortExpression="P2_FTA">
                <HeaderStyle Wrap="false" />
            </asp:CheckBoxField>
        </Columns>
    </asp:GridView>
</div>
<div id="CollectedPaid" runat="server" visible="false">
    <fieldset class="outline-fieldset">
        <legend>Family</legend><strong>Mediations Held:&nbsp;</strong><asp:Label
            ID="lblMediationHeld_f" runat="server" /><br />
        <strong>Total fees collected:&nbsp; ($0-$50)</strong>&nbsp;<asp:Label ID="lblFeeCollect60_f"
            runat="server" />&nbsp;&nbsp;<strong>($50-$100)</strong>&nbsp;<asp:Label ID="lblFeeCollect120_f"
                runat="server" />&nbsp;&nbsp;<strong>(Indigent)</strong>&nbsp;<asp:Label ID="lblFeeCollectIndigent_f"
                    runat="server" />&nbsp;<strong>(Fee Waived)</strong>
        <asp:Label ID="lblFamilyPaidWaived" runat="server" /><br />
        <strong>Total fees owed when session was held:&nbsp; ($0-$50)</strong>&nbsp;<asp:Label
            ID="lblFeeOwedHeld60_f" runat="server" />&nbsp;&nbsp;<strong>($50-$100)</strong>&nbsp;<asp:Label
                ID="lblFeeOwedHeld120_f" runat="server" />&nbsp;&nbsp;<strong>(Indigent)</strong>&nbsp;<asp:Label
                    ID="lblFeeOwedHeldIndigent_f" runat="server" />&nbsp;<strong>(Fee Waived)</strong>
        <asp:Label ID="lblFamilyOwedWaived" runat="server" /><br />
        <strong>Total fees owed when session was not held (FTA):&nbsp; ($0-$50)</strong>&nbsp;<asp:Label
            ID="lblFeeOwedNH60_f" runat="server" />&nbsp;&nbsp;<strong>($50-$100)</strong>&nbsp;<asp:Label
                ID="lblFeeOwedNH120_f" runat="server" />&nbsp;&nbsp;<strong>(Indigent)</strong>&nbsp;<asp:Label
                    ID="lblFeeOwedIndigentNH_f" runat="server" />&nbsp;<strong>(Fee Waived)</strong>
        <asp:Label ID="lblFamilyOwedWaivedFTA" runat="server" /><br />
    </fieldset>
    <fieldset class="outline-fieldset">
        <legend>County</legend><strong>Mediations Held:&nbsp;</strong>&nbsp;<asp:Label
            ID="lblMediationHeld_c" runat="server" /><br />
        <strong>Total fees collected:&nbsp;</strong><asp:Label ID="lblFeeCollect60_c"
            runat="server" />&nbsp;&nbsp;<strong>(Indigent)&nbsp;</strong><asp:Label ID="lblFeeCollectIndigent_c"
                runat="server" />&nbsp;<strong>(Fee Waived)</strong>
        <asp:Label ID="lblCountyPaidWaived" runat="server" /><br />
        <strong>Total fees owed when session was held:&nbsp;</strong>&nbsp;<asp:Label
            ID="lblFeeOwedHeld60_c" runat="server" />&nbsp;&nbsp;<strong>(Indigent)&nbsp;</strong><asp:Label ID="lblFeeOwedHeldIndigent_c"
                runat="server" />&nbsp;<strong>(Fee Waived)</strong>
        <asp:Label ID="lblCountyOwedWaived" runat="server" /><br />
        <strong>Total fees owed when session was not held (FTA):&nbsp;</strong>&nbsp;<asp:Label
            ID="lblFeeOwedNH60_c" runat="server" />&nbsp;&nbsp;<strong>(Indigent)&nbsp;</strong><asp:Label ID="lblFeeOwedNHIndigent_c"
                runat="server" />&nbsp;<strong>(Fee Waived)</strong>
        <asp:Label ID="lblCountyOwedWaivedFTA" runat="server" /><br />
    </fieldset>
</div>
<dnn:dnncssinclude runat="server" filepath="~/Resources/Shared/components/TimePicker/Themes/jquery-ui.min.css" />

<script type="text/javascript">

    /*globals jQuery, window, Sys */
    (function ($, Sys) {
        $(document).ready(function () {
            PageInit();
            Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
                PageInit();
            });
        });

    }(jQuery, window.Sys));
    function PageInit() {
        $(".datepicker").datepicker();
    }

</script>
