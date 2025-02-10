<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SurveyReport.ascx.cs" Inherits="tjc.Modules.PretrialServices.SurveyReport" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<div class="fullScreenContainer">
    <div class="btn-toolbar" role="toolbar" aria-label="Reporting Toolbar">
        <div class="input-group me-2" aria-label="Select Report Date">
            <div class="input-group-text" id="dvReportDate">Report Date</div>
            <asp:TextBox runat="server" ID="txtReportDate" CssClass="form-control datepicker" Width="110" aria-label="Report Date Selection" aria-describedby="dvReportDate" />
        </div>
        <div class="btn-group me-2" role="group" aria-label="Select Report Type">
            <input type="radio" class="btn-check" name="btnradio" id="reportWeek" autocomplete="off" checked>
            <label class="btn btn-outline-primary mb-0" for="reportWeek">Weekly</label>
            <input type="radio" class="btn-check" name="btnradio" id="reportYear" autocomplete="off">
            <label class="btn btn-outline-primary mb-0" for="reportYear">Annual</label>
        </div>
        <asp:HiddenField ID="hdRportDate" ClientIDMode="Static" runat="server" Value="W" />
        <asp:Button Text="Submit" ID="cmdSubmit" CssClass="btn btn-primary" runat="server" OnClick="cmdSubmit_Click" />
    </div>
    <h4>How many cases</h4>
    <ul class="list">
        <li>
            <asp:Label ID="lblScreened" runat="server">0</asp:Label> - Screened (SPR packet created)</li>
        <li>
            <asp:Label ID="lblNotScreened" runat="server">0</asp:Label> - Not Screened (no SPR packet created)</li>
        <li>
            <asp:Label ID="lblPlacedSPR" runat="server">0</asp:Label> - Placed on SPR**</li>
        <li>
            <asp:Label ID="lblNotPlacedSPR" runat="server">0</asp:Label> - Not placed on SPR (number of participants in the program)</li>
        <li>
            <asp:Label ID="lblMisdemeanor" runat="server">0</asp:Label> - Misdemeanor case</li>
        <li>
            <asp:Label ID="lblFelony" runat="server">0</asp:Label> - Felony case (how many charged with MM misdemeanors and CF felonies)</li>
        <li>
            <asp:Label ID="lblNoBond" runat="server">0</asp:Label> - Nonsecured (Without bond)</li>
        <li>
            <asp:Label ID="lblWithBond" runat="server">0</asp:Label> - Secured (with bond) wants to know who had bonds and who didn’t)</li>
        <li>
            <asp:Label ID="lblBothBond" runat="server">0</asp:Label> - Both Secured and Unsecured</li>
        <li>
            <asp:Label ID="lblRevokedBond" runat="server">0</asp:Label> - Bond Revoked</li>
        <li>
            <asp:Label ID="lblUnsuccessfulCompletion" runat="server">0</asp:Label> - number exiting with an unsuccessful completion</li>
        <li>
            <asp:Label ID="lblSuccessfulCompletion" runat="server">0</asp:Label> - number exiting with a successful completion</li>
        <li>
            <asp:Label ID="lblOtherCompletion" runat="server">0</asp:Label> - number exiting for other reasons (this one wants us to describe: bonded before 1st, pled at 1st, revoked bonds, revoked SPR, new arrest, deceased, ordered by court)</li>
        <li>
            <asp:Label ID="lblTotalExiting" runat="server">0</asp:Label> - total number exiting the program</li>
        <li>
            <asp:Label ID="lblAverageLengthSPR" runat="server">0</asp:Label> - average length of time in months on SPR (when we open it and closed it)</li>
        <li>
            <asp:Label ID="lblFtaSpr" runat="server">0</asp:Label> - how many FTA (failed to appear while on SPR)</li>
        <li>
            <asp:Label ID="lblWarrantsFta" runat="server">0</asp:Label> - warrants issued for FTA</li>
        <li>
            <asp:Label ID="lblSprRevokedFta" runat="server">0</asp:Label> - SPR revoked due to FTA</li>
        <li>
            <asp:Label ID="lblNewArrest" runat="server">0</asp:Label> - arrested for committing a new offense</li>
        <li>
            <asp:Label ID="lblReleaseRevokedNewOffense" runat="server">0</asp:Label> - released revoked due to new offense</li>
        <li>
            <asp:Label ID="lblNoComplaintsProgramConditions" runat="server">0</asp:Label> - non-compliant with program conditions</li>
        <li>
            <asp:Label ID="lblWarrantNonCompliance" runat="server">0</asp:Label> - warrant issued for non-compliance with program</li>
        <li>
            <asp:Label ID="lblNumberCarriedOver" runat="server">0</asp:Label> - how many carried over from the year before</li>
    </ul>
</div>
<dnn:DnnCssInclude runat="server" FilePath="~/Resources/Shared/components/TimePicker/Themes/jquery-ui.min.css" />

<script type="text/javascript">
    /*globals jQuery, window, Sys */
    (function ($, Sys) {

        $(document).ready(function () {
            $(".datepicker").datepicker();
            $(".toggle-button input").addClass("btn-check");
            $(".toggle-button label").addClass("btn btn-outline-primary");
            if ($("#hdRportDate").val() == "W")
                $("#reportWeek").prop("checked", true);
            else
                $("#reportYear").prop("checked", true);
            $("#reportWeek").on("click", function (e) {
                $("#hdRportDate").val("W");
            });
            $("#reportYear").on("click", function (e) {
                $("#hdRportDate").val("Y");
            });
        });

    }(jQuery, window.Sys));

</script>
