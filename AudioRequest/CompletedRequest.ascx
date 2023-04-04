<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CompletedRequest.ascx.cs" EnableViewState="false" Inherits="tjc.Modules.AudioRequest.CompleteRequest" %>
<div class="alert alert-success fade in nomargin" id="pcResponse" runat="server">
    <strong><em class="fa fa-exclamation-circle"></em>Please Remember</strong>, after the court order has been signed by the presiding
            circuit court judge, the order should be faxed or scanned and emailed by the requestor
            to the applicable Digital Court Recording Office.
</div>
<div class="alert alert-success fade in nomargin" id="saResponse" runat="server" visible="false">
    <strong><em class="fa fa-exclamation-circle"></em>IMPORTANT </strong>You will be notified when your audio has been uploaded and instructions will be provided on how to access the audio.<br />
    If a CD was requested, it will be processed and inter-officed in 3-5 business days.
</div>
<%--<div class="alert alert-success fade in nomargin" id="trResponse" runat="server" visible="false">
    <strong><em class="fa fa-exclamation-circle"></em>Thank you for your request </strong>  It is being processed; you will be contacted for further information..
</div>--%>

<div id="message" runat="server">
    <div class="alert alert-default">

        <strong>Sarasota and DeSoto County Digital Court Recording Office </strong>
        <p class="ml-sm">
            <strong><em class="fa fa-fax"></em>Fax:</strong> (941) 861-7924<br />
            <strong><em class="fa fa-envelope"></em>Email:</strong> <a href="mailto:dcrgrpsar@jud12.flcourts.org">dcrgrpsar@jud12.flcourts.org</a>
        </p>
    </div>
    <div class="alert alert-default">
        <strong>Manatee and DeSoto County Digital Court Recording Office </strong>
        <p class="ml-sm">
            <strong><em class="fa fa-fax"></em>Fax:</strong> (941) 749-3692<br />
            <strong><em class="fa fa-envelope"></em>Email:</strong> <a href="mailto:dcrgrpman@jud12.flcourts.org">dcrgrpman@jud12.flcourts.org</a>
        </p>
    </div>
    <h4>Obtain Orders at the Following Links</h4>
    <ul>
        <li><a href="/Portals/0/Documents/Programs/DCR/Motion-Order-Copy-Juv-Proceeding.pdf" title="Opens in New Window">MOTION REQUESTING AUTHORIZATION FOR COPY OF DIGITAL RECORDING</a></li>
        <li><a href="/Portals/0/Documents/Programs/DCR/Order-Copy-Juv-Proceeding.pdf" title="Opens in New Window">ORDER AUTHORIZING COPY OF DIGITAL RECORDING</a></li>
    </ul>
</div>
<p>
    <a class="btn btn-primary btn-lg" href="/Programs/Court-Reporting-Recording" title="Return to Court Reporting">Return to Court Reporting</a>
</p>
<script type="text/javascript">
    $(document).ready(function () {
        $("h1 .Head").text("Thank You for your Request");
    });
</script>
