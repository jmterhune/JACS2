<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FeeScheduleList.ascx.cs" Inherits="tjc.Modules.NewMediationStats.FeeScheduleList" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>

<div class="tabs">
    <ul class="nav nav-tabs">
        <li class="nav-item">
            <a class="nav-link" href="<%=CaseListUrl %>"><i class="fas fa-list"></i>&nbsp;Back to List</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=AttorneyListUrl %>">Attorneys</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=MediatorListUrl %>">Mediators</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=RegionListUrl %>">Regions</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=GroupListUrl %>">Case Type Groups</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=CaseTypeListUrl %>">Case Types</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=AppearanceListUrl %>">Appearance Values</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=IssueListUrl %>">Issues</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=ActionListUrl %>">Stage of Action Items</a>
        </li>
        <li class="nav-item active">
            <a class="nav-link" href="#feeSchedules" data-bs-toggle="tab">Fee Schedules</a>
        </li>
    </ul>
    <div class="tab-content">
        <div id="feeSchedules" class="tab-pane active">
            <asp:UpdatePanel ID="pnlFeeSchedules" runat="server" RenderMode="Block" OnUnload="pnlFeeSchedules_Unload">
                <ContentTemplate>
                    <asp:UpdateProgress ID="upProgressEvent" runat="server">
                        <ProgressTemplate>
                            <div class="modal-progress">
                                <div class="center-progress">
                                    <img alt="" src="/images/loading.gif" />
                                </div>
                            </div>
                        </ProgressTemplate>
                    </asp:UpdateProgress>
                    <asp:Literal ID="ltMessage" runat="server" />
                    <p class="text-muted mt-3">
                        A fee schedule applies to every session whose mediation date is on or after its effective date, until the next schedule's
                        effective date. A schedule with no effective date is not in effect. Entry forms and reports pick the schedule from the
                        mediation date, so changing a date changes which schedule existing sessions are reported under.
                    </p>
                    <asp:Repeater ID="rptSchedule" runat="server" OnItemCreated="rptSchedule_ItemCreated" OnItemCommand="rptSchedule_ItemCommand">
                        <HeaderTemplate>
                            <table id="tblFeeSchedule" class="table table-striped">
                                <thead>
                                    <tr>
                                        <th>&nbsp;</th>
                                        <th>Fee Schedule</th>
                                        <th>Effective Date</th>
                                        <th>Status</th>
                                        <th>Notes</th>
                                    </tr>
                                </thead>
                                <tbody>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <tr>
                                <td class="command-item">
                                    <asp:LinkButton ID="cmdEdit" runat="server" CssClass="text-primary" CommandName="edit" CausesValidation="false" CommandArgument='<%#Eval("FeeScheduleId").ToString() %>'><i class="fas fa-edit"></i></asp:LinkButton>
                                </td>
                                <td><%#Server.HtmlEncode((string)Eval("Name"))%></td>
                                <td><%#Eval("EffectiveText")%></td>
                                <td><%#Eval("Status")%></td>
                                <td><%#Server.HtmlEncode((string)Eval("Notes") ?? "")%></td>
                            </tr>
                        </ItemTemplate>
                        <FooterTemplate>
                            </tbody>
                    </table>
                        </FooterTemplate>
                    </asp:Repeater>

                    <h5 class="mt-4">Tiers in each schedule</h5>
                    <asp:Repeater ID="rptTiers" runat="server">
                        <HeaderTemplate>
                            <table class="table table-sm table-striped">
                                <thead>
                                    <tr>
                                        <th>Fee Schedule</th>
                                        <th>Case Group</th>
                                        <th>Used For</th>
                                        <th>Stored Value</th>
                                        <th>Shown As</th>
                                        <th class="text-end">Fee</th>
                                    </tr>
                                </thead>
                                <tbody>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <tr>
                                <td><%#Server.HtmlEncode((string)Eval("ScheduleName"))%></td>
                                <td><%#Eval("GroupName")%></td>
                                <td><%#Eval("TypeName")%></td>
                                <td><%#Server.HtmlEncode((string)Eval("Value"))%></td>
                                <td><%#Server.HtmlEncode((string)Eval("Label"))%></td>
                                <td class="text-end"><%#Eval("AmountText")%></td>
                            </tr>
                        </ItemTemplate>
                        <FooterTemplate>
                            </tbody>
                    </table>
                        </FooterTemplate>
                    </asp:Repeater>

                    <div class="modal fade" id="EditFeeScheduleModal" tabindex="-1" role="dialog" aria-labelledby="EditFeeScheduleModalLabel" aria-hidden="true">
                        <div class="modal-dialog">
                            <div class="modal-content">
                                <div class="modal-header">
                                    <h4 class="modal-title" id="EditFeeScheduleModalLabel">Edit Fee Schedule</h4>
                                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                                </div>
                                <div class="modal-body">
                                    <div class="mb-3">
                                        <asp:Label runat="server" AssociatedControlID="txtName" Text="Name" />
                                        <asp:TextBox AutoCompleteType="Disabled" runat="server" ClientIDMode="Static" CssClass="form-control" MaxLength="100" ID="txtName" />
                                    </div>
                                    <div class="mb-3">
                                        <asp:Label runat="server" AssociatedControlID="txtEffectiveDate" Text="Effective Date (leave blank until the law takes effect)" />
                                        <asp:TextBox AutoCompleteType="Disabled" runat="server" ClientIDMode="Static" TextMode="Date" CssClass="form-control" MaxLength="15" ID="txtEffectiveDate" />
                                    </div>
                                    <div class="mb-3">
                                        <asp:Label runat="server" AssociatedControlID="txtNotes" Text="Notes" />
                                        <asp:TextBox AutoCompleteType="Disabled" runat="server" ClientIDMode="Static" TextMode="MultiLine" Rows="4" CssClass="form-control" MaxLength="500" ID="txtNotes" />
                                    </div>
                                    <asp:HiddenField ID="hdFeeScheduleId" ClientIDMode="Static" runat="server" />
                                </div>
                                <div class="modal-footer justify-content-between">
                                    <asp:Button OnClientClick="ToggleEditForm(false)" CssClass="btn btn-primary" ID="cmdSave" runat="server" Text="Save" OnClick="cmdSave_Click" />
                                    <button type="button" class="btn btn-default" data-bs-dismiss="modal">Close</button>
                                </div>
                            </div>
                        </div>
                    </div>
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="cmdSave" EventName="Click" />
                </Triggers>
            </asp:UpdatePanel>
        </div>
    </div>
</div>

<script type="text/javascript">
    function ToggleEditForm(toggleValue) {
        if (toggleValue) {
            bootstrap.Modal.getOrCreateInstance(document.getElementById('EditFeeScheduleModal')).show();
        } else {
            bootstrap.Modal.getOrCreateInstance(document.getElementById('EditFeeScheduleModal')).hide();
            $('body').removeClass('modal-open');
            $('.modal-backdrop').remove();
        }
        return true;
    }
</script>
