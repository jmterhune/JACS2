<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Withdrawals.ascx.cs" Inherits="tjc.Modules.CourtRegistry.Withdrawals" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<dnn:dnnjsinclude runat="server" filepath="~/DesktopModules/tjc.modules/CourtRegistry/Scripts/registry-ui.js" />
<div class="tabs">
    <ul class="nav nav-tabs">
        <li class="nav-item">
            <a class="nav-link" href="<%=ApplicationListUrl%>">Applications</a>
        </li>
        <li class="nav-item active">
            <a class="nav-link" href="#withdrawals" data-bs-toggle="tab">Withdrawals</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=ManageYearsUrl%>">Manage Fiscal Years</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=ExceptionListUrl%>">Exceptions</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=BasicSettingsUrl%>">Basic Settings</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=AttorneyListUrl%>">Attorneys</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=JacCodeListUrl%>">JAC Codes</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=UpdateJacCodeUrl%>">Update JAC</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=LocationListUrl%>">Locations</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=CaseTypeListUrl%>">Case Types</a>
        </li>
    </ul>
    <div class="tab-content pb-0">
        <div id="withdrawals" class="tab-pane active">
            <asp:UpdatePanel ID="pnlWithdrawals" runat="server" RenderMode="Block">
                <ContentTemplate>
                    <div class="bg-dark text-white border-dark rounded p-2 mb-2">
                        <div class="row align-items-center">
                            <div class="col-auto pt-1"><strong>Show:</strong></div>
                            <div class="col-auto">
                                <asp:DropDownList runat="server" ID="drpStatus" CssClass="form-control" AutoPostBack="true"
                                    OnSelectedIndexChanged="drpStatus_SelectedIndexChanged" />
                            </div>
                            <div class="col text-end">
                                <asp:Literal ID="ltPendingCount" runat="server" />
                            </div>
                        </div>
                    </div>

                    <asp:Literal ID="ltMessage" runat="server" />
                    <asp:Literal ID="ltModalScript" runat="server" EnableViewState="false" />

                    <p class="text-muted small">
                        Approving a request takes the attorney off the registry for future appointments: their
                        application for the fiscal year of the request and any later year is archived, so they
                        drop out of the JAC export and the registry reports. Nothing is deleted &mdash; the
                        application stays as the record of what they were approved for.
                    </p>

                    <asp:Repeater ID="rptWithdrawals" runat="server" OnItemCommand="rptWithdrawals_ItemCommand">
                        <HeaderTemplate>
                            <table class="table table-striped">
                                <thead>
                                    <tr>
                                        <th>&nbsp;</th>
                                        <th>ID</th>
                                        <th>Attorney</th>
                                        <th>Bar Number</th>
                                        <th>Application</th>
                                        <th>Period</th>
                                        <th>Requested</th>
                                        <th>Reviewed</th>
                                        <th>Status</th>
                                    </tr>
                                </thead>
                                <tbody>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <tr>
                                <td class="command-item">
                                    <asp:LinkButton ID="lnkReview" runat="server" CausesValidation="false" CssClass="text-primary"
                                        CommandName="review" CommandArgument='<%#Eval("WithdrawalId") %>'
                                        ToolTip="Review this request"><i class="fas fa-search"></i></asp:LinkButton>
                                </td>
                                <td><%#Eval("WithdrawalId") %></td>
                                <td><%#Eval("AttorneyName") %></td>
                                <td><%#Eval("BarNumber") %></td>
                                <td>
                                    <%# Convert.ToInt32(Eval("ApplicationId") ?? 0) > 0
                                        ? "<a href=\"" + ApplicationUrl + "/aid/" + Eval("ApplicationId") + "\">#" + Eval("ApplicationId") + "</a>"
                                        : "None" %>
                                </td>
                                <td><%#Eval("PeriodYear") %></td>
                                <td><%#Eval("RequestedDisplay") %></td>
                                <td><%#Eval("ReviewedDisplay") %></td>
                                <td><span class="<%#Eval("StatusCss") %>"><%#Eval("StatusName") %></span></td>
                            </tr>
                        </ItemTemplate>
                        <FooterTemplate>
                            </tbody></table>
                        </FooterTemplate>
                    </asp:Repeater>
                    <asp:Panel ID="pnlEmpty" runat="server" Visible="false" CssClass="alert alert-info">
                        No withdrawal requests match this filter.
                    </asp:Panel>

                    <div class="modal fade" id="withdrawalModal" tabindex="-1" role="dialog" aria-labelledby="withdrawalModalLabel" aria-hidden="true">
                        <div class="modal-dialog modal-lg modal-dialog-scrollable">
                            <div class="modal-content">
                                <div class="modal-header">
                                    <h4 class="modal-title" id="withdrawalModalLabel">Registry Withdrawal Request</h4>
                                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                                </div>
                                <div class="modal-body">
                                    <asp:HiddenField ID="hdWithdrawalId" runat="server" />
                                    <table role="presentation" class="layout p-2 mb-2">
                                        <tbody>
                                            <tr>
                                                <td class="lbl"><strong>Attorney:</strong></td>
                                                <td><asp:Literal ID="ltName" runat="server" /></td>
                                                <td class="lbl"><strong>Bar Number:</strong></td>
                                                <td><asp:Literal ID="ltBarNumber" runat="server" /></td>
                                            </tr>
                                            <tr>
                                                <td class="lbl"><strong>Requested:</strong></td>
                                                <td><asp:Literal ID="ltRequested" runat="server" /></td>
                                                <td class="lbl"><strong>Application:</strong></td>
                                                <td><asp:Literal ID="ltApplication" runat="server" /></td>
                                            </tr>
                                            <tr>
                                                <td class="lbl"><strong>Signed:</strong></td>
                                                <td><asp:Literal ID="ltSignature" runat="server" /></td>
                                                <td class="lbl"><strong>Acknowledged:</strong></td>
                                                <td><asp:Literal ID="ltAcknowledged" runat="server" /></td>
                                            </tr>
                                            <tr>
                                                <td class="lbl"><strong>Reason:</strong></td>
                                                <td colspan="3"><asp:Literal ID="ltReason" runat="server" /></td>
                                            </tr>
                                        </tbody>
                                    </table>

                                    <p class="mb-1"><strong>Case categories held on this application</strong></p>
                                    <asp:Literal ID="ltJacCodes" runat="server" />

                                    <asp:Panel ID="pnlDecision" runat="server">
                                        <div class="row mb-3">
                                            <div class="col-md-5">
                                                <asp:Label runat="server" AssociatedControlID="txtEffectiveDate" Text="Removal effective date" />
                                                <asp:TextBox runat="server" ID="txtEffectiveDate" CssClass="form-control" TextMode="Date" />
                                            </div>
                                        </div>
                                        <div class="row mb-3">
                                            <div class="col-md-12">
                                                <asp:Label runat="server" AssociatedControlID="txtReviewNote"
                                                    Text="Note to the attorney (required when denying)" />
                                                <asp:TextBox runat="server" ID="txtReviewNote" TextMode="MultiLine" Rows="3" CssClass="form-control" />
                                            </div>
                                        </div>
                                    </asp:Panel>
                                    <asp:Panel ID="pnlReviewed" runat="server" Visible="false" CssClass="alert alert-secondary">
                                        <asp:Literal ID="ltReviewedSummary" runat="server" />
                                    </asp:Panel>
                                </div>
                                <div class="modal-footer justify-content-between">
                                    <div>
                                        <asp:Button ID="cmdApprove" runat="server" Visible="false" CssClass="btn btn-danger" OnClick="cmdApprove_Click"
                                            Text="Approve removal"
                                            OnClientClick="return confirm('Approve this removal request? The attorney comes off the registry for future appointments and their application is archived.');" />
                                        <asp:Button ID="cmdDeny" runat="server" Visible="false" CssClass="btn btn-warning" OnClick="cmdDeny_Click"
                                            Text="Deny request"
                                            OnClientClick="return confirm('Deny this request? The attorney stays on the registry and will be emailed your note.');" />
                                    </div>
                                    <button type="button" class="btn btn-default" data-bs-dismiss="modal">Close</button>
                                </div>
                            </div>
                        </div>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </div>
</div>
<script type="text/javascript">
    (function () {
        function cleanupOrphanModals() {
            if (document.querySelectorAll('.modal.show').length === 0) {
                document.querySelectorAll('.modal-backdrop').forEach(function (b) { b.remove(); });
                document.body.classList.remove('modal-open');
                document.body.style.overflow = '';
                document.body.style.paddingRight = '';
            }
        }
        if (window.Sys && Sys.WebForms) {
            Sys.WebForms.PageRequestManager.getInstance().add_endRequest(cleanupOrphanModals);
        }
    })();
</script>
