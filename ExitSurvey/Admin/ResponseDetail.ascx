<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ResponseDetail.ascx.cs" Inherits="tjc.Modules.ExitSurvey.Admin.ResponseDetail" %>

<div class="container-fluid exit-survey exit-survey-detail">

    <asp:PlaceHolder ID="plhMessage" runat="server" EnableViewState="false"></asp:PlaceHolder>

    <div class="mb-2 d-print-none">
        <asp:HyperLink ID="lnkBack" runat="server" CssClass="btn btn-secondary" Text="&larr; Back to Results" />
        <asp:Button ID="cmdPrint" runat="server" CssClass="btn btn-primary" OnClientClick="printExitSurvey(); return false;" Text="Print / Save as PDF" />
    </div>

    <asp:Panel ID="pnlDetail" runat="server">

        <h4 class="exit-survey-print-title">Twelfth Judicial Circuit Employee Exit Survey</h4>

        <dl class="row exit-survey-meta">
            <dt class="col-sm-3">Submitted</dt>
            <dd class="col-sm-9"><asp:Literal ID="ltSubmitted" runat="server" /></dd>
            <dt class="col-sm-3">Submitted By</dt>
            <dd class="col-sm-9"><asp:Literal ID="ltSubmittedBy" runat="server" /></dd>
            <dt class="col-sm-3">Share With Supervisor</dt>
            <dd class="col-sm-9"><asp:Literal ID="ltShare" runat="server" /></dd>
        </dl>

        <h5 class="exit-survey-question">1. General conditions of the job</h5>
        <asp:Repeater ID="rptGeneral" runat="server">
            <HeaderTemplate><table class="table exit-survey-results"><tbody></HeaderTemplate>
            <ItemTemplate>
                <tr>
                    <td class="exit-survey-item"><%# Server.HtmlEncode((string)Eval("ItemLabel")) %></td>
                    <td><%# Eval("RatingDisplay") %></td>
                </tr>
            </ItemTemplate>
            <FooterTemplate></tbody></table></FooterTemplate>
        </asp:Repeater>
        <dl id="pnlQ1Other" runat="server" visible="false" class="row">
            <dt class="col-sm-3">Other</dt>
            <dd class="col-sm-9"><asp:Literal ID="ltQ1Other" runat="server" /></dd>
        </dl>

        <h5 class="exit-survey-question">2. Advantages the new employer offers</h5>
        <asp:Repeater ID="rptAdvantages" runat="server">
            <HeaderTemplate><ul></HeaderTemplate>
            <ItemTemplate><li><%# Server.HtmlEncode((string)Eval("ReasonLabel")) %></li></ItemTemplate>
            <FooterTemplate></ul></FooterTemplate>
        </asp:Repeater>
        <dl id="pnlQ2Other" runat="server" visible="false" class="row">
            <dt class="col-sm-3">Other</dt>
            <dd class="col-sm-9"><asp:Literal ID="ltQ2Other" runat="server" /></dd>
        </dl>

        <h5 class="exit-survey-question">3. Have you accepted another position?</h5>
        <p><asp:Literal ID="ltQ3" runat="server" /></p>

        <h5 class="exit-survey-question">4. Type of employer</h5>
        <p><asp:Literal ID="ltQ4" runat="server" /></p>

        <h5 class="exit-survey-question">5. What influenced you to leave</h5>
        <asp:Repeater ID="rptReasons" runat="server">
            <HeaderTemplate><ul></HeaderTemplate>
            <ItemTemplate><li><%# Server.HtmlEncode((string)Eval("ReasonLabel")) %></li></ItemTemplate>
            <FooterTemplate></ul></FooterTemplate>
        </asp:Repeater>
        <dl id="pnlQ5Other" runat="server" visible="false" class="row">
            <dt class="col-sm-3">Other</dt>
            <dd class="col-sm-9"><asp:Literal ID="ltQ5Other" runat="server" /></dd>
        </dl>

        <h5 class="exit-survey-question">6. Supervision received</h5>
        <asp:Repeater ID="rptSupervision" runat="server">
            <HeaderTemplate><table class="table exit-survey-results"><tbody></HeaderTemplate>
            <ItemTemplate>
                <tr>
                    <td class="exit-survey-item"><%# Server.HtmlEncode((string)Eval("ItemLabel")) %></td>
                    <td><%# Eval("RatingDisplay") %></td>
                </tr>
            </ItemTemplate>
            <FooterTemplate></tbody></table></FooterTemplate>
        </asp:Repeater>

        <h5 class="exit-survey-question">7. Adequate training and/or resources</h5>
        <p><asp:Literal ID="ltQ7" runat="server" /></p>

        <h5 class="exit-survey-question">8. What could make the Circuit a better place to work</h5>
        <p><asp:Literal ID="ltQ8" runat="server" /></p>

        <h5 class="exit-survey-question">9. Would you consider working here again?</h5>
        <p><asp:Literal ID="ltQ9" runat="server" /></p>

        <h5 class="exit-survey-section">Personal Info</h5>
        <dl class="row exit-survey-meta">
            <dt class="col-sm-3">Name</dt>
            <dd class="col-sm-9"><asp:Literal ID="ltName" runat="server" /></dd>
            <dt class="col-sm-3">Most recent position title</dt>
            <dd class="col-sm-9"><asp:Literal ID="ltPositionTitle" runat="server" /></dd>
            <dt class="col-sm-3">Name of Supervisor</dt>
            <dd class="col-sm-9"><asp:Literal ID="ltSupervisorName" runat="server" /></dd>
            <dt class="col-sm-3">Supervisor's Title</dt>
            <dd class="col-sm-9"><asp:Literal ID="ltSupervisorTitle" runat="server" /></dd>
            <dt class="col-sm-3">Month/Year hired</dt>
            <dd class="col-sm-9"><asp:Literal ID="ltHired" runat="server" /></dd>
            <dt class="col-sm-3">Month/Year separated</dt>
            <dd class="col-sm-9"><asp:Literal ID="ltSeparated" runat="server" /></dd>
            <dt class="col-sm-3">Date completed</dt>
            <dd class="col-sm-9"><asp:Literal ID="ltDateCompleted" runat="server" /></dd>
        </dl>

    </asp:Panel>

</div>

<script type="text/javascript">
    // Print only the survey detail: clone pnlDetail into a hidden iframe that
    // carries the page's stylesheets, so the DNN skin/chrome is left out and the
    // printout starts cleanly at the top (1in margin from @page in module.css).
    function printExitSurvey() {
        var src = document.getElementById("<%= pnlDetail.ClientID %>");
        if (!src) { window.print(); return; }

        var heads = "";
        var nodes = document.querySelectorAll('link[rel="stylesheet"], style');
        for (var i = 0; i < nodes.length; i++) heads += nodes[i].outerHTML;

        var frame = document.createElement("iframe");
        frame.setAttribute("aria-hidden", "true");
        frame.style.position = "fixed";
        frame.style.right = "0";
        frame.style.bottom = "0";
        frame.style.width = "0";
        frame.style.height = "0";
        frame.style.border = "0";
        document.body.appendChild(frame);

        var doc = frame.contentWindow.document;
        doc.open();
        doc.write('<!DOCTYPE html><html><head><meta charset="utf-8">' + heads +
            '<style>@page{margin:1in;}html,body{margin:0;padding:0;background:#fff;}</style></head>' +
            '<body><div class="container-fluid exit-survey exit-survey-detail exit-survey-print">' +
            src.innerHTML + '</div></body></html>');
        doc.close();

        var iw = frame.contentWindow;
        // Give the copied stylesheets a moment to load before printing.
        setTimeout(function () {
            iw.focus();
            iw.print();
            setTimeout(function () { if (frame.parentNode) frame.parentNode.removeChild(frame); }, 1000);
        }, 500);
    }
</script>
