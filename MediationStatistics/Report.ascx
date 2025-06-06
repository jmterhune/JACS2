<%@ control language="C#" autoeventwireup="true" codebehind="Report.ascx.cs" inherits="tjc.Modules.MediationStatistics.Report" %>
<%@ register tagprefix="dnn" namespace="DotNetNuke.Web.Client.ClientResourceManagement" assembly="DotNetNuke.Web.Client" %>
<div id="report-form">
    <div class="row form-group">
        <div class="col-auto">
            <asp:label runat="server" associatedcontrolid="txtStartDate" text="Start Date" />
            <asp:textbox autocompletetype="Disabled" runat="server" id="txtStartDate" maxlength="15" clientidmode="Static" cssclass="form-control datepicker" />
        </div>
        <div class="col-auto">
            <asp:label runat="server" associatedcontrolid="txtEndDate" text="End Date" />
            <asp:textbox autocompletetype="Disabled" runat="server" id="txtEndDate" maxlength="15" clientidmode="Static" cssclass="form-control datepicker" />
        </div>
        <div class="col-auto">
            <asp:label runat="server" associatedcontrolid="drpReport" text="Report" />
            <asp:dropdownlist id="drpReport" runat="server" tooltip="Select Report to Run" autopostback="true" cssclass="form-control" onselectedindexchanged="drpReport_SelectedIndexChanged" clientidmode="Static">
                <asp:listitem text="Compendium" value="0" />
                <asp:listitem text="Fees Owed" value="1" />
                <asp:listitem text="Referral Sources" value="2" />
                <asp:listitem text="Check Stats" value="3" />
                <asp:listitem text="Collected &amp; Paid" value="4" />
                <asp:listitem text="Mediator Stats" value="5" />
            </asp:dropdownlist>
        </div>

    </div>
</div>
<p>
    <asp:linkbutton id="cmdReport" runat="server"
        onclick="cmdReport_Click" cssclass="btn btn-primary btn-lg">
        <i class="fas fa-save"></i>&nbsp;Run Report</asp:linkbutton>
    <asp:hyperlink id="lnkCancel" cssclass="btn btn-secondary btn-lg" runat="server"><i class="fas fa-redo"></i>&nbsp;Exit</asp:hyperlink>
</p>
<div id="Conpendium" runat="server" visible="false">
    <asp:repeater id="rptConpendium" runat="server" onitemdatabound="rptConpendium_ItemDataBound">
        <footertemplate>
            </tbody></table>
        </footertemplate>
        <itemtemplate>
            <asp:literal id="ltHeader" runat="server" />
            <tr>
                <td>
                    <asp:literal id="ltLineNumber" runat="server" />
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
        </itemtemplate>
    </asp:repeater>
</div>
<div id="FeesOwed" runat="server" visible="false">
    <asp:repeater id="rptFeesOwed" runat="server" onitemdatabound="rptFeesOwed_ItemDataBound">
        <itemtemplate>
            <asp:literal id="ltHeader" runat="server" />
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
                    <asp:checkbox id="chkAgreement" runat="server" enabled="false" checked='<%#Eval("FeeAgreement")%>' />
                </td>
                <td>
                    <asp:checkbox id="chkjudgment" runat="server" enabled="false" checked='<%#Eval("Feejudgement")%>' />
                </td>
                <td>
                    <asp:checkbox id="chkWaiver" runat="server" enabled="false" checked='<%#Eval("FeeWaiver")%>' />
                </td>
                <td>
                    <asp:checkbox id="chkOts" runat="server" enabled="false" checked='<%#Eval("OTS")%>' />
                </td>
                <td>
                    <asp:checkbox id="chkP1_FTA" runat="server" enabled="false" checked='<%#Eval("P1_FTA")%>' />
                </td>
                <td>
                    <asp:checkbox id="chkP2_FTA" runat="server" enabled="false" checked='<%#Eval("P2_FTA")%>' />
                </td>
            </tr>
        </itemtemplate>
        <footertemplate>
            </tbody></table>
        </footertemplate>
    </asp:repeater>
</div>
<div id="Referrals" runat="server" visible="false">
    <asp:repeater id="rptReferrals" runat="server">
        <footertemplate>
            </tbody></table>
        </footertemplate>
        <headertemplate>
            <table class="table table-striped">
                <thead>
                    <tr>
                        <th>Referral Source
                        </th>
                    </tr>
                </thead>
                <tbody>
        </headertemplate>
        <itemtemplate>
            <tr>
                <td>
                    <%# Container.DataItem ?? string.Empty%>
                </td>
            </tr>
        </itemtemplate>
    </asp:repeater>
</div>
<div id="Checker" runat="server" visible="false">
    <asp:gridview id="rgChecker" runat="server" cssclass="table table-striped rgChecker" clientidmode="Static">
        <columns>
            <asp:boundfield datafield="Region" headertext="Region" readonly="True" sortexpression="Region"></asp:boundfield>
            <asp:boundfield datafield="CaseTypeGroup" headertext="Case Group" readonly="True" sortexpression="CaseTypeGroup">
                <itemstyle wrap="false" />
            </asp:boundfield>
            <asp:boundfield datafield="CaseNumber" headertext="Case Number" readonly="True" sortexpression="CaseNumber">
                <itemstyle wrap="false" />
            </asp:boundfield>
            <asp:boundfield datafield="partyone" headertext="Party One" readonly="True" sortexpression="partyone"></asp:boundfield>
            <asp:boundfield datafield="partytwo" headertext="Party Two" readonly="True" sortexpression="partytwo"></asp:boundfield>
            <asp:boundfield datafield="ReferralDate" headertext="Referred" readonly="True" sortexpression="ReferralDate"
                dataformatstring="{0:d}"></asp:boundfield>
            <asp:checkboxfield datafield="MediationHeld" headertext="Mediation Held" readonly="True" sortexpression="MediationHeld"></asp:checkboxfield>
            <asp:boundfield datafield="MediationDate" headertext="Mediated" readonly="True" sortexpression="MediationDate"
                dataformatstring="{0:d}"></asp:boundfield>
            <asp:boundfield datafield="Mediator" headertext="Mediator" readonly="True" sortexpression="Mediator"></asp:boundfield>
            <asp:checkboxfield datafield="AgreementReached" headertext="Agreement Reached" readonly="True" sortexpression="AgreementReached"></asp:checkboxfield>
            <asp:boundfield datafield="FeeAmount" headertext="Fee Amount" readonly="True" sortexpression="FeeAmount"></asp:boundfield>
            <asp:checkboxfield datafield="OTS" headertext="OTSC" readonly="True" sortexpression="OTS">
                <headerstyle wrap="false" />
            </asp:checkboxfield>
            <asp:checkboxfield datafield="FeeWaiver" headertext="Fee Waived" readonly="True" sortexpression="FeeWaiver">
                <headerstyle wrap="false" />
            </asp:checkboxfield>
            <asp:checkboxfield datafield="P1_FTA" headertext="P-FTA" readonly="True" sortexpression="P1_FTA">
                <headerstyle wrap="false" />
            </asp:checkboxfield>
            <asp:checkboxfield datafield="P2_FTA" headertext="R-FTA" readonly="True" sortexpression="P2_FTA">
                <headerstyle wrap="false" />
            </asp:checkboxfield>
        </columns>
    </asp:gridview>
    <button class="btn btn-primary" onclick="exportGridViewToExcel('rgChecker')"><i class="fas fa-file-excel"></i>&nbsp;Export to Excel</button>
</div>
<div id="CollectedPaid" runat="server" visible="false">
    <fieldset class="outline-fieldset">
        <legend>Family</legend><strong>Mediations Held:&nbsp;</strong><asp:label
            id="lblMediationHeld_f" runat="server" /><br />
        <strong>Total fees collected:&nbsp; ($0-$50)</strong>&nbsp;<asp:label id="lblFeeCollect60_f"
            runat="server" />&nbsp;&nbsp;<strong>($50-$100)</strong>&nbsp;<asp:label id="lblFeeCollect120_f"
                runat="server" />&nbsp;&nbsp;<strong>(Indigent)</strong>&nbsp;<asp:label id="lblFeeCollectIndigent_f"
                    runat="server" />&nbsp;<strong>(Fee Waived)</strong>
        <asp:label id="lblFamilyPaidWaived" runat="server" />
        <br />
        <strong>Total fees owed when session was held:&nbsp; ($0-$50)</strong>&nbsp;<asp:label
            id="lblFeeOwedHeld60_f" runat="server" />&nbsp;&nbsp;<strong>($50-$100)</strong>&nbsp;<asp:label
                id="lblFeeOwedHeld120_f" runat="server" />&nbsp;&nbsp;<strong>(Indigent)</strong>&nbsp;<asp:label
                    id="lblFeeOwedHeldIndigent_f" runat="server" />&nbsp;<strong>(Fee Waived)</strong>
        <asp:label id="lblFamilyOwedWaived" runat="server" />
        <br />
        <strong>Total fees owed when session was not held (FTA):&nbsp; ($0-$50)</strong>&nbsp;<asp:label
            id="lblFeeOwedNH60_f" runat="server" />&nbsp;&nbsp;<strong>($50-$100)</strong>&nbsp;<asp:label
                id="lblFeeOwedNH120_f" runat="server" />&nbsp;&nbsp;<strong>(Indigent)</strong>&nbsp;<asp:label
                    id="lblFeeOwedIndigentNH_f" runat="server" />&nbsp;<strong>(Fee Waived)</strong>
        <asp:label id="lblFamilyOwedWaivedFTA" runat="server" />
        <br />
    </fieldset>
    <fieldset class="outline-fieldset">
        <legend>County</legend><strong>Mediations Held:&nbsp;</strong>&nbsp;<asp:label
            id="lblMediationHeld_c" runat="server" /><br />
        <strong>Total fees collected:&nbsp;</strong><asp:label id="lblFeeCollect60_c"
            runat="server" />&nbsp;&nbsp;<strong>(Indigent)&nbsp;</strong><asp:label id="lblFeeCollectIndigent_c"
                runat="server" />&nbsp;<strong>(Fee Waived)</strong>
        <asp:label id="lblCountyPaidWaived" runat="server" />
        <br />
        <strong>Total fees owed when session was held:&nbsp;</strong>&nbsp;<asp:label
            id="lblFeeOwedHeld60_c" runat="server" />&nbsp;&nbsp;<strong>(Indigent)&nbsp;</strong><asp:label id="lblFeeOwedHeldIndigent_c"
                runat="server" />&nbsp;<strong>(Fee Waived)</strong>
        <asp:label id="lblCountyOwedWaived" runat="server" />
        <br />
        <strong>Total fees owed when session was not held (FTA):&nbsp;</strong>&nbsp;<asp:label
            id="lblFeeOwedNH60_c" runat="server" />&nbsp;&nbsp;<strong>(Indigent)&nbsp;</strong><asp:label id="lblFeeOwedNHIndigent_c"
                runat="server" />&nbsp;<strong>(Fee Waived)</strong>
        <asp:label id="lblCountyOwedWaivedFTA" runat="server" />
        <br />
    </fieldset>
</div>
<div id="MediatorStats" runat="server" visible="false">
    <hr />
    <div class="row form-group">
        <div class="col-auto">
            <asp:label runat="server" id="lblMediatoryType" associatedcontrolid="drpMediatorType" text="Mediator Type" visible="false" />
            <asp:dropdownlist id="drpMediatorType" runat="server" cssclass="form-control" visible="false" clientidmode="Static">
                <asp:listitem text="< Select Mediator Type >" value=""></asp:listitem>
                <asp:listitem text="Contracted" value="Contracted" />
                <asp:listitem text="Staff" value="Staff" />
                <asp:listitem text="Volunteer" value="Volunteer" />
            </asp:dropdownlist>
        </div>
        <div class="col-auto">
            <asp:label runat="server" id="lblMediator" associatedcontrolid="drpMediator" text="Mediator" visible="false" />
            <asp:dropdownlist id="drpMediator" runat="server" cssclass="form-control" clientidmode="Static" visible="false" appenddatabounditems="true">
                <asp:listitem text="< Select Mediator >" value=""></asp:listitem>
            </asp:dropdownlist>
        </div>

        <div class="col-auto">
            <asp:button cssclass="btn btn-primary" id="cmdMediatorStat" text="Submit" runat="server" onclick="cmdMediatorStat_Click" />
        </div>
    </div>
    <hr />
    <h3>Statistics by Mediator Type</h3>
    <asp:repeater id="rptMediatorTypeCounts" runat="server">
        <headertemplate>
            <table id="tblMediatorTypeCounts" class="table table-striped">
                <thead>
                    <tr>
                        <th>Mediator Type</th>
                        <th>Location</th>
                        <th># Mediations</th>
                        <th># Agreements</th>
                    </tr>
                </thead>
                <tbody>
        </headertemplate>
        <itemtemplate>
            <tr>
                <td>
                    <%#Eval("MediatorType") %>
                </td>
                <td>
                    <%#Eval("Region") %>
                </td>
                <td><%#Eval("Held") %></td>
                <td><%#Eval("Signed") %></td>
            </tr>
        </itemtemplate>
        <footertemplate>
            </tbody></table>
        </footertemplate>
    </asp:repeater>
        <button class="btn btn-primary mb-4" onclick="exportGridViewToExcel('tblMediatorTypeCounts')"><i class="fas fa-file-excel"></i>&nbsp;Export to Excel</button>

    <h3>Statistics by Mediator</h3>
    <asp:repeater id="rptMediatorCounts" runat="server">
        <headertemplate>
            <table id="tblMediatorCounts" class="table table-striped">
                <thead>
                    <tr>
                        <th>Mediator</th>
                        <th>Location</th>
                        <th># Mediations</th>
                        <th># Agreements</th>
                    </tr>
                </thead>
                <tbody>
        </headertemplate>
        <itemtemplate>
            <tr>
                <td>
                    <%#Eval("MediatorName") %>
                </td>
                <td>
                    <%#Eval("Region") %>
                </td>
                <td><%#Eval("Held") %></td>
                <td><%#Eval("Signed") %></td>
            </tr>
        </itemtemplate>
        <footertemplate>
            </tbody></table>
        </footertemplate>
    </asp:repeater>
            <button class="btn btn-primary" onclick="exportGridViewToExcel('tblMediatorCounts')"><i class="fas fa-file-excel"></i>&nbsp;Export to Excel</button>

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
    function exportGridViewToExcel(gridId) {
        var table = document.getElementById(gridId);
        var html = table.outerHTML;
        var dataType = 'application/vnd.ms-excel';
        var blob = new Blob(['\ufeff', html], { type: dataType });

        var url = URL.createObjectURL(blob);
        var a = document.createElement('a');
        a.href = url;
        a.download = 'GridViewExport.xls';
        document.body.appendChild(a);
        a.click();
        document.body.removeChild(a);
    }
</script>
