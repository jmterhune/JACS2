<%@ control language="C#" autoeventwireup="true" codebehind="SurveyReport.ascx.cs" inherits="tjc.Modules.PretrialServices.SurveyReport" %>
<%@ register tagprefix="dnn" namespace="DotNetNuke.Web.Client.ClientResourceManagement" assembly="DotNetNuke.Web.Client" %>
<div class="fullScreenContainer">
    <div class="btn-toolbar" role="toolbar" aria-label="Reporting Toolbar">
        <div class="input-group me-2" aria-label="Select Report Date">
            <div class="input-group-text" id="dvReportDate">Report Date</div>
            <asp:textbox runat="server" id="txtReportDate" cssclass="form-control datepicker" width="110" aria-label="Report Date Selection" aria-describedby="dvReportDate" />
        </div>
        <div class="btn-group me-2" role="group" aria-label="Select Report Type">
            <input type="radio" class="btn-check" name="btnradio" id="reportWeek" autocomplete="off" checked>
            <label class="btn btn-outline-primary mb-0" for="reportWeek">Weekly</label>
            <input type="radio" class="btn-check" name="btnradio" id="reportMonth" autocomplete="off">
            <label class="btn btn-outline-primary mb-0" for="reportMonth">Monthly</label>
            <input type="radio" class="btn-check" name="btnradio" id="reportYear" autocomplete="off">
            <label class="btn btn-outline-primary mb-0" for="reportYear">Annual</label>
        </div>
        <asp:hiddenfield id="hdRportDate" clientidmode="Static" runat="server" value="W" />
        <asp:button text="Submit" id="cmdSubmit" cssclass="btn btn-primary" runat="server" onclick="cmdSubmit_Click" />
    </div>
    <h4>How many cases</h4>
    <ul class="list">
        <li>
            <asp:label id="lblScreened" runat="server">0</asp:label>
            - Screened (SPR packet created)</li>
        <li>
            <asp:label id="lblNotScreened" runat="server">0</asp:label>
            - Not Screened (no SPR packet created)</li>
        <li>
            <asp:label id="lblPlacedSPR" runat="server">0</asp:label>
            - Placed on SPR**</li>
        <li>
            <asp:label id="lblNotPlacedSPR" runat="server">0</asp:label>
            - Not placed on SPR (number of participants in the program)</li>
        <li>
            <asp:label id="lblMisdemeanor" runat="server">0</asp:label>
            - Misdemeanor case</li>
        <li>
            <asp:label id="lblFelony" runat="server">0</asp:label>
            - Felony case (how many charged with MM misdemeanors and CF felonies)</li>
        <li>
            <asp:label id="lblNoBond" runat="server">0</asp:label>
            - Nonsecured (Without bond)</li>
        <li>
            <asp:label id="lblWithBond" runat="server">0</asp:label>
            - Secured (with bond) wants to know who had bonds and who didn’t)</li>
        <li>
            <asp:label id="lblBothBond" runat="server">0</asp:label>
            - Both Secured and Unsecured</li>
        <li>
            <asp:label id="lblRevokedBond" runat="server">0</asp:label>
            - Bond Revoked</li>
        <li>
            <asp:label id="lblUnsuccessfulCompletion" runat="server">0</asp:label>
            - number exiting with an unsuccessful completion</li>
        <li>
            <asp:label id="lblSuccessfulCompletion" runat="server">0</asp:label>
            - number exiting with a successful completion</li>
        <li>
            <asp:label id="lblOtherCompletion" runat="server">0</asp:label>
            - number exiting for other reasons (this one wants us to describe: bonded before 1st, pled at 1st, revoked bonds, revoked SPR, new arrest, deceased, ordered by court)</li>
        <li>
            <asp:label id="lblTotalExiting" runat="server">0</asp:label>
            - total number exiting the program</li>
        <li>
            <asp:label id="lblAverageLengthSPR" runat="server">0</asp:label>
            - average length of time in months on SPR (when we open it and closed it)</li>
        <li>
            <asp:label id="lblFtaSpr" runat="server">0</asp:label>
            - how many FTA (failed to appear while on SPR)</li>
        <li>
            <asp:label id="lblWarrantsFta" runat="server">0</asp:label>
            - warrants issued for FTA</li>
        <li>
            <asp:label id="lblSprRevokedFta" runat="server">0</asp:label>
            - SPR revoked due to FTA</li>
        <li>
            <asp:label id="lblNewArrest" runat="server">0</asp:label>
            - arrested for committing a new offense</li>
        <li>
            <asp:label id="lblReleaseRevokedNewOffense" runat="server">0</asp:label>
            - released revoked due to new offense</li>
        <li>
            <asp:label id="lblNoComplaintsProgramConditions" runat="server">0</asp:label>
            - non-compliant with program conditions</li>
        <li>
            <asp:label id="lblWarrantNonCompliance" runat="server">0</asp:label>
            - warrant issued for non-compliance with program</li>
        <li>
            <asp:label id="lblNumberCarriedOver" runat="server">0</asp:label>
            - how many carried over from the year before</li>
    </ul>
</div>
<dnn:dnncssinclude runat="server" filepath="~/Resources/Shared/components/TimePicker/Themes/jquery-ui.min.css" />

<script type="text/javascript">
    /*globals jQuery, window, Sys */
    (function ($, Sys) {

        $(document).ready(function () {
            $(".datepicker").datepicker();
            $(".toggle-button input").addClass("btn-check");
            $(".toggle-button label").addClass("btn btn-outline-primary");
            if ($("#hdRportDate").val() == "W") {
                $("#reportMonth").prop("checked", false);
                $("#reportYear").prop("checked", false);
                $("#reportWeek").prop("checked", true);
            }
            else if ($("#hdRportDate").val() == "M") {
                $("#reportMonth").prop("checked", true);
                $("#reportYear").prop("checked", false);
                $("#reportWeek").prop("checked", false);
            }
            else {
                $("#reportMonth").prop("checked", false);
                $("#reportYear").prop("checked", true);
                $("#reportWeek").prop("checked", false);
            }
            $("#reportWeek").on("click", function (e) {
                $("#hdRportDate").val("W");
            });
            $("#reportMonth").on("click", function (e) {
                $("#hdRportDate").val("M");
            });
            $("#reportYear").on("click", function (e) {
                $("#hdRportDate").val("Y");
            });
        });

    }(jQuery, window.Sys));

</script>
