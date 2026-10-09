<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CompletedRequest.ascx.cs" EnableViewState="false" Inherits="tjc.Modules.ThreatReport.CompleteRequest" %>
<div class="alert alert-success">

    <strong><i class="fa fa-exclamation-circle" aria-hidden="true"></i>Thank You! </strong>Your Incident Report has been recorded.
    <asp:Literal  runat="server"  ID="ltIncidentID"/>
</div>


<p>
            <asp:HyperLink ID="lnkReport" Visible="false" runat="server" CssClass="btn btn-primary btn-lg" ToolTip="Report an Incident"><i class="fa fa-search" aria-hidden="true"></i>&nbsp;View Incidents</asp:HyperLink>

    <asp:HyperLink ID="lnkHome" runat="server" CssClass="btn btn-tertiary btn-lg" ToolTip="Return to Home Page"><i class="fa fa-link" aria-hidden="true"></i>&nbsp;Return to Home Page</asp:HyperLink>
</p>
<script type="text/javascript">
    $(document).ready(function () {
        $("h1 .Head").text("Thank You for your Report");
    });
</script>
