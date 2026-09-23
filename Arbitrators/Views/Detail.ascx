<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Detail.ascx.cs" Inherits="tjc.Modules.Arbitrators.Views.Detail" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<%-- SweetAlert2 + Noty for confirms / toast notifications --%>
<dnn:DnnCssInclude runat="server" FilePath="/Resources/Libraries/sweetalert/sweetalert2.min.css" />
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/sweetalert/sweetalert2.all.min.js" />
<dnn:DnnCssInclude runat="server" FilePath="/Resources/Libraries/Noty/noty.min.css" />
<dnn:DnnCssInclude runat="server" FilePath="/Resources/Libraries/Noty/bootstrap-v4.min.css" />
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/Noty/noty.min.js" />

<div id="ArbitratorDetail" class="container-fluid">
    <asp:Panel ID="pnlNotFound" runat="server" Visible="false" CssClass="alert alert-danger">
        Unable to find the requested application. <asp:HyperLink runat="server" ID="lnkNotFoundBack" Text="Return to List" />
    </asp:Panel>

    <asp:Panel ID="pnlApplication" runat="server">
        <asp:Literal ID="ltHeading" runat="server" />

        <div class="row mb-3">
            <div class="col-12 col-lg-4">
                <label>Address</label>
                <p><asp:Literal ID="ltAddress" runat="server" /></p>
            </div>
            <div class="col-12 col-lg-4">
                <label>Phone / Fax</label>
                <p><asp:Literal ID="ltPhone" runat="server" /></p>
            </div>
            <div class="col-12 col-lg-4">
                <label>Email</label>
                <p><asp:HyperLink ID="lnkEmail" runat="server" /></p>
            </div>
        </div>

        <div class="row mb-3">
            <div class="col-12 col-lg-6">
                <label>Website</label>
                <p><asp:Literal ID="ltWebsite" runat="server" /></p>
            </div>
            <div class="col-12 col-lg-6">
                <label>Preferred Areas</label>
                <p><asp:Literal ID="ltPreferredAreas" runat="server" /></p>
            </div>
        </div>

        <div class="row mb-3">
            <div class="col-12">
                <asp:CheckBox ID="chkBarMember" runat="server" Text="Florida Bar Member" Enabled="false" />
                &nbsp;&nbsp;
                <asp:CheckBox ID="chkCertifiedMediator" runat="server" Text="Certified Mediator" Enabled="false" />
                &nbsp;&nbsp;
                <asp:CheckBox ID="chkCompletedTraining" runat="server" Text="Completed Arbitration Training" Enabled="false" />
            </div>
        </div>

        <div class="row mb-3">
            <div class="col-12">
                <label>Experience Statement</label>
                <p class="white-space-pre"><asp:Literal ID="ltExperience" runat="server" /></p>
            </div>
        </div>

        <div class="row mb-3">
            <div class="col-12 col-lg-4">
                <label>Signed</label>
                <p><asp:Literal ID="ltSigned" runat="server" /></p>
            </div>
            <div class="col-12 col-lg-4">
                <label>Submitted</label>
                <p><asp:Literal ID="ltSubmitted" runat="server" /></p>
            </div>
            <div class="col-12 col-lg-4">
                <label>Submitted From IP</label>
                <p><asp:Literal ID="ltSubmittedIP" runat="server" /></p>
            </div>
        </div>

        <div class="row mb-3">
            <div class="col-12">
                <h5>Attachments</h5>
                <asp:Repeater ID="rptAttachments" runat="server">
                    <HeaderTemplate><ul class="list-group"></HeaderTemplate>
                    <ItemTemplate>
                        <li class="list-group-item">
                            <a href='<%# ResolveUrl("~/DesktopModules/tjc.modules/Arbitrators/Handlers/AttachmentViewer.ashx") %>?id=<%# Eval("AttachmentID") %>' target="_blank">
                                <i class="fas fa-paperclip"></i>&nbsp;<%# Server.HtmlEncode(Convert.ToString(Eval("FileName"))) %>
                                <small class="text-muted">(<%# Eval("AttachmentType") %>)</small>
                            </a>
                        </li>
                    </ItemTemplate>
                    <FooterTemplate></ul></FooterTemplate>
                </asp:Repeater>
                <asp:Literal ID="ltNoAttachments" runat="server" Text="No attachments." Visible="false" />
            </div>
        </div>

        <hr />
        <div class="row mb-3">
            <div class="col-12">
                <label for="<%=txtStatusNotes.ClientID %>">Review Notes <small class="text-muted">(included in the applicant's notification email, e.g. the reason for denial)</small></label>
                <asp:TextBox ID="txtStatusNotes" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="4" />
            </div>
        </div>

        <div>
            <asp:LinkButton ID="cmdApprove" runat="server" CssClass="btn btn-success" Text="Approve" OnClientClick="return Jud12ConfirmPostback(this, 'This will approve the application and email the applicant.', 'Approve Application?');" OnClick="cmdApprove_Click" />
            <asp:LinkButton ID="cmdDeny" runat="server" CssClass="btn btn-danger" Text="Deny" OnClientClick="return Jud12ConfirmPostback(this, 'This will deny the application and email the applicant.', 'Deny Application?');" OnClick="cmdDeny_Click" />
            <asp:HyperLink ID="lnkCancel" runat="server" CssClass="btn btn-secondary" Text="Return to List" />
        </div>
    </asp:Panel>
</div>

<script type="text/javascript">
    function Jud12ConfirmPostback(btn, msg, title) {
        if (!window.Swal) { return window.confirm(msg); }
        if (btn && btn.dataset && btn.dataset.jud12Confirmed === '1') {
            btn.dataset.jud12Confirmed = '';
            return true;
        }
        Swal.fire({
            title: title || 'Confirm', text: msg, icon: 'warning',
            showCancelButton: true, confirmButtonText: 'Yes', cancelButtonText: 'No',
            confirmButtonColor: '#d33'
        }).then(function (r) {
            if (!r.isConfirmed) return;
            var href = btn.href || '';
            var m = href.match(/__doPostBack\(['"]([^'"]+)['"],\s*['"]([^'"]*)['"]\)/);
            if (m && typeof __doPostBack === 'function') {
                __doPostBack(m[1], m[2]);
            } else if (btn && typeof btn.click === 'function') {
                btn.dataset.jud12Confirmed = '1';
                btn.click();
            }
        });
        return false;
    }
</script>
