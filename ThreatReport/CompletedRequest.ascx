<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CompletedRequest.ascx.cs" EnableViewState="false" Inherits="tjc.Modules.ThreatReport.CompleteRequest" %>
<div class="alert alert-success">

    <strong><em class="fa fa-exclamation-circle"></em>Thank You! </strong>Your Incident Report has been recorded.
    <asp:Literal  runat="server"  ID="ltIncidentID"/>
</div>


<p>
            <asp:HyperLink ID="lnkReport" Visible="false" runat="server" CssClass="btn btn-primary btn-lg" ToolTip="Report an Incident"><em class="fa fa-search"></em>&nbsp;View Incidents</asp:HyperLink>

    <asp:HyperLink ID="lnkHome" runat="server" CssClass="btn btn-tertiary btn-lg" ToolTip="Return to Home Page"><em class="fa fa-link"></em>&nbsp;Return to Home Page</asp:HyperLink>
</p>
<script type="text/javascript">
    $(document).ready(function () {
        $("h1 .Head").text("Thank You for your Report");
    });
</script>
