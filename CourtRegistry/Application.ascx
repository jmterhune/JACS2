<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Application.ascx.cs" Inherits="tjc.Modules.CourtRegistry.Application" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<div id="Application">
    <asp:Literal ID="ltHeading" runat="server"></asp:Literal>
    <table role="presentation" class="layout p-2 mb-2">
        <tbody>
            <tr>
                <td class="lbl"><strong>Name:</strong></td>
                <td><asp:Literal ID="ltName" runat="server" /></td>
                <td class="lbl"><strong>Bar Number:</strong></td>
                <td><asp:Literal ID="ltBarNumber" runat="server" /></td>
                <td class="lbl"><strong>Law Firm:</strong></td>
                <td><asp:Literal ID="ltFirm" runat="server" /></td>
            </tr>
            <tr>
                <td class="lbl"><strong>Address:</strong></td>
                <td colspan="5"><asp:Literal ID="ltAddress" runat="server" /></td>
            </tr>
            <tr>
                <td class="lbl"><strong>Phone:</strong></td>
                <td><asp:Literal ID="ltPhone" runat="server" /></td>
                <td class="lbl"><strong>Fax:</strong></td>
                <td><asp:Literal ID="ltFax" runat="server" /></td>
                <td class="lbl"><strong>Cell:</strong></td>
                <td><asp:Literal ID="ltCell" runat="server" /></td>
            </tr>
            <tr>
                <td class="lbl"><strong>Email:</strong></td>
                <td><asp:HyperLink ID="lnkEmail" runat="server"></asp:HyperLink></td>
                <td class="lbl"><strong>Languages:</strong></td>
                <td><asp:Literal ID="ltLanguages" runat="server" /></td>
                <td class="lbl"><strong>Years on Registry:</strong></td>
                <td><asp:Literal ID="ltYears" runat="server" /></td>
            </tr>
            <tr>
                <td colspan="6">
                    <p class="mb-0"><strong>How attorney will meet with clients in the "remote" locations:</strong></p>
                    <asp:Literal ID="ltRemoteInfo" runat="server" />
                </td>
            </tr>
            <tr>
                <td colspan="3">
                    <asp:CheckBox Text="Is a Renewal?" ID="chkRenewal" runat="server" Enabled="false" />
                    <asp:CheckBox Text="Signed for Guardianship Cases?" ID="chkGuardian" runat="server" Enabled="false" />
                </td>
                <td colspan="3" class="text-end">
                    <asp:LinkButton runat="server" OnClientClick="return confirm('This will Reject the entire application.\n\nAre You Sure?');" ID="cmdReject" CssClass="btn btn-danger" Text="Reject All" OnClick="cmdReject_Click" />
                </td>
            </tr>
        </tbody>
    </table>
    <p class="legend"><strong>Legend:</strong>
        <span class="badge badge-primary">New</span>
        <span class="badge badge-success">Approved</span>
        <span class="badge badge-warning">Rejected</span>
        <span class="badge badge-dark">Requesting Removal</span>
        <span class="badge badge-danger">Requested Removal but not Approved</span></p>
    <p class="text-danger"><strong>Note:</strong> A checked box normally indicates approval. However, for <span class="badge badge-dark">Requesting Removal</span> and <span class="badge badge-danger">Requested Removal but not Approved</span>, <em>uncheck</em> the box to approve the removal.</p>
    <table role="presentation" class="layout">
        <tbody>
            <tr>
                <td colspan="6" class="p-0">
                    <asp:Table ID="rootTbl" runat="server" CssClass="jacList"></asp:Table>
                </td>
            </tr>
            <tr>
                <td colspan="6">
                    <p><asp:Label ID="lblRejectionText" runat="server" CssClass="bold"><strong>Rejection Reasons</strong></asp:Label></p>
                    <asp:TextBox ID="txtRejectText" Rows="5" runat="server" TextMode="MultiLine" CssClass="form-control"></asp:TextBox>
                </td>
            </tr>
        </tbody>
    </table>
    <hr />
    <p>
        <asp:LinkButton runat="server" ID="cmdSave" CssClass="btn btn-primary" OnClientClick="return doSubmit(this);" Text="Approve" OnClick="cmdSave_Click" />
        <asp:HyperLink runat="server" ID="lnkCancel" CssClass="btn btn-default" Text="Return to List" NavigateUrl="/" />
    </p>
</div>
<script type="text/javascript">
    function doSubmit(btn) {
        if (typeof (Page_ClientValidate) == 'function' && Page_ClientValidate() == false) {
            return false;
        }
        btn.setAttribute("disabled", "disabled");
        btn.classList.add("isDisabled");
        btn.innerText = 'Processing ...';
        return true;
    }
</script>
