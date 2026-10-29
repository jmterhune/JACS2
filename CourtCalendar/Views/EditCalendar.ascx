<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EditCalendar.ascx.cs" Inherits="tjc.Modules.CourtCalendar.EditCalendar" %>

<div class="cc-admin cc-editcal">
    <asp:Literal ID="ltMessage" runat="server" EnableViewState="false" />

    <div class="cc-toolbar mb-3">
        <asp:HyperLink ID="lnkHome" runat="server" CssClass="btn btn-link"><i class="fas fa-arrow-left" aria-hidden="true"></i>&nbsp;Back to calendars</asp:HyperLink>
    </div>

    <asp:Panel ID="pnlDenied" runat="server" Visible="false">
        <div class="alert alert-warning">You do not have permission to edit this calendar.</div>
    </asp:Panel>

    <asp:Panel ID="pnlMain" runat="server">
        <h2 class="cc-h2"><asp:Literal ID="ltTitle" runat="server" /></h2>

        <%-- Calendar settings --%>
        <div class="card mb-3"><div class="card-body">
            <asp:HiddenField ID="hfCalendarId" runat="server" />
            <div class="row">
                <asp:Panel ID="pnlNewPeriod" runat="server" CssClass="col-12">
                    <div class="row">
                        <div class="col-md-3 mb-2">
                            <label class="form-label" for="<%= ddlMonth.ClientID %>">Month</label>
                            <asp:DropDownList ID="ddlMonth" runat="server" CssClass="form-select" />
                        </div>
                        <div class="col-md-3 mb-2">
                            <label class="form-label" for="<%= ddlYear.ClientID %>">Year</label>
                            <asp:DropDownList ID="ddlYear" runat="server" CssClass="form-select" />
                        </div>
                    </div>
                </asp:Panel>
                <div class="col-md-8 mb-2">
                    <label class="form-label" for="<%= txtTitleOverride.ClientID %>">Title override (optional)</label>
                    <asp:TextBox ID="txtTitleOverride" runat="server" CssClass="form-control" placeholder="e.g. AMENDED October 2026 Felony Div. III" />
                </div>
                <div class="col-md-4 mb-2 d-flex align-items-end">
                    <div class="form-check">
                        <asp:CheckBox ID="chkPublished" runat="server" CssClass="form-check-input" />
                        <label class="form-check-label" for="<%= chkPublished.ClientID %>">Published (visible on public site)</label>
                    </div>
                </div>
                <div class="col-12 mb-2">
                    <label class="form-label" for="<%= txtNotes.ClientID %>">Footer / amendment notes (optional)</label>
                    <asp:TextBox ID="txtNotes" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="2" />
                </div>
            </div>
            <asp:Button ID="btnSaveCalendar" runat="server" CssClass="btn btn-primary" Text="Save calendar" OnClick="btnSaveCalendar_Click" />
            <asp:Button ID="btnSendNotification" runat="server" CssClass="btn btn-outline-primary" Text="Send update notification" CausesValidation="false" OnClick="btnSendNotification_Click" OnClientClick="return confirm('Email all subscribers that this calendar changed?');" />
            <asp:Button ID="btnDeleteCalendar" runat="server" CssClass="btn btn-outline-danger float-end" Text="Delete calendar" CausesValidation="false" OnClick="btnDeleteCalendar_Click" OnClientClick="return confirm('Delete this calendar and all its entries?');" />
        </div></div>

        <asp:Panel ID="pnlChildren" runat="server">
            <%-- Week headers --%>
            <div class="card mb-3"><div class="card-body">
                <div class="d-flex justify-content-between align-items-center mb-2">
                    <h3 class="cc-h3 mb-0">Week row headers</h3>
                    <asp:Button ID="btnAddWeek" runat="server" CssClass="btn btn-sm btn-primary" Text="Add week header" CausesValidation="false" OnClick="btnAddWeek_Click" />
                </div>
                <asp:Panel ID="pnlWeekForm" runat="server" Visible="false" CssClass="cc-inline-form mb-2">
                    <asp:HiddenField ID="hfWeekId" runat="server" />
                    <div class="row">
                        <div class="col-md-3 mb-2">
                            <label class="form-label" for="<%= txtWeekStart.ClientID %>">Week start (Monday)</label>
                            <asp:TextBox ID="txtWeekStart" runat="server" CssClass="form-control" TextMode="Date" />
                        </div>
                        <div class="col-md-4 mb-2">
                            <label class="form-label" for="<%= txtWeekLabel.ClientID %>">Label</label>
                            <asp:TextBox ID="txtWeekLabel" runat="server" CssClass="form-control" placeholder="Week 5 / Trial Week" />
                        </div>
                        <div class="col-md-3 mb-2">
                            <label class="form-label" for="<%= txtWeekSub.ClientID %>">Sub-label (optional)</label>
                            <asp:TextBox ID="txtWeekSub" runat="server" CssClass="form-control" />
                        </div>
                        <div class="col-md-2 mb-2">
                            <label class="form-label" for="<%= txtWeekSort.ClientID %>">Order</label>
                            <asp:TextBox ID="txtWeekSort" runat="server" CssClass="form-control" Text="0" />
                        </div>
                    </div>
                    <asp:Button ID="btnSaveWeek" runat="server" CssClass="btn btn-sm btn-primary" Text="Save" OnClick="btnSaveWeek_Click" />
                    <asp:Button ID="btnCancelWeek" runat="server" CssClass="btn btn-sm btn-secondary" Text="Cancel" CausesValidation="false" OnClick="btnCancelWeek_Click" />
                </asp:Panel>
                <table class="table table-sm">
                    <thead><tr><th>Week start</th><th>Label</th><th>Sub-label</th><th></th></tr></thead>
                    <tbody>
                        <asp:Repeater ID="rptWeeks" runat="server" OnItemCommand="rptWeeks_ItemCommand">
                            <ItemTemplate>
                                <tr>
                                    <td><%# Eval("WeekStartDate", "{0:MM/dd/yyyy}") %></td>
                                    <td><%# Eval("Label") %></td>
                                    <td><%# Eval("SubLabel") %></td>
                                    <td class="text-end">
                                        <asp:LinkButton runat="server" CssClass="btn btn-sm btn-outline-primary" CommandName="edit" CommandArgument='<%# Eval("WeekHeaderId") %>'><i class="fas fa-pen" aria-hidden="true"></i></asp:LinkButton>
                                        <asp:LinkButton runat="server" CssClass="btn btn-sm btn-outline-danger" CommandName="delete" CommandArgument='<%# Eval("WeekHeaderId") %>' OnClientClick="return confirm('Delete this week header?');"><i class="fas fa-trash" aria-hidden="true"></i></asp:LinkButton>
                                    </td>
                                </tr>
                            </ItemTemplate>
                        </asp:Repeater>
                    </tbody>
                </table>
            </div></div>

            <%-- Entries --%>
            <div class="card mb-3"><div class="card-body">
                <div class="d-flex justify-content-between align-items-center mb-2">
                    <h3 class="cc-h3 mb-0">Entries</h3>
                    <asp:Button ID="btnAddEntry" runat="server" CssClass="btn btn-sm btn-primary" Text="Add entry" CausesValidation="false" OnClick="btnAddEntry_Click" />
                </div>
                <asp:Panel ID="pnlEntryForm" runat="server" Visible="false" CssClass="cc-inline-form mb-2">
                    <asp:HiddenField ID="hfEntryId" runat="server" />
                    <div class="row">
                        <div class="col-md-3 mb-2">
                            <label class="form-label" for="<%= txtEntryDate.ClientID %>">Date</label>
                            <asp:TextBox ID="txtEntryDate" runat="server" CssClass="form-control" TextMode="Date" />
                        </div>
                        <div class="col-md-2 mb-2">
                            <label class="form-label" for="<%= txtTimespan.ClientID %>">Time</label>
                            <asp:TextBox ID="txtTimespan" runat="server" CssClass="form-control" placeholder="9:00 / 9:00-5:00" />
                        </div>
                        <div class="col-md-2 mb-2 d-flex align-items-end">
                            <div class="form-check">
                                <asp:CheckBox ID="chkAllDay" runat="server" CssClass="form-check-input" />
                                <label class="form-check-label" for="<%= chkAllDay.ClientID %>">All day</label>
                            </div>
                        </div>
                        <div class="col-md-5 mb-2">
                            <label class="form-label" for="<%= txtShortTitle.ClientID %>">Short title</label>
                            <asp:TextBox ID="txtShortTitle" runat="server" CssClass="form-control" placeholder="Pleas/Motions" />
                        </div>
                        <div class="col-md-6 mb-2">
                            <label class="form-label" for="<%= txtDetail.ClientID %>">Detail (shown in hover popover)</label>
                            <asp:TextBox ID="txtDetail" runat="server" CssClass="form-control" placeholder="SPECIAL SET / BARTHOLOMEW" />
                        </div>
                        <div class="col-md-4 mb-2">
                            <label class="form-label" for="<%= ddlCategory.ClientID %>">Category</label>
                            <asp:DropDownList ID="ddlCategory" runat="server" CssClass="form-select" />
                        </div>
                        <div class="col-md-2 mb-2">
                            <label class="form-label" for="<%= txtEntrySort.ClientID %>">Order</label>
                            <asp:TextBox ID="txtEntrySort" runat="server" CssClass="form-control" Text="0" />
                        </div>
                    </div>
                    <asp:Button ID="btnSaveEntry" runat="server" CssClass="btn btn-sm btn-primary" Text="Save" OnClick="btnSaveEntry_Click" />
                    <asp:Button ID="btnCancelEntry" runat="server" CssClass="btn btn-sm btn-secondary" Text="Cancel" CausesValidation="false" OnClick="btnCancelEntry_Click" />
                </asp:Panel>
                <table class="table table-sm cc-entry-admin">
                    <thead><tr><th>Date</th><th>Time</th><th>Title</th><th>Detail</th><th>Category</th><th></th></tr></thead>
                    <tbody>
                        <asp:Repeater ID="rptEntries" runat="server" OnItemCommand="rptEntries_ItemCommand">
                            <ItemTemplate>
                                <tr class='<%# (bool)Eval("IsStricken") ? "cc-row-stricken" : "" %>'>
                                    <td><%# Eval("EntryDate", "{0:MM/dd}") %></td>
                                    <td><%# (bool)Eval("AllDay") ? "All day" : System.Web.HttpUtility.HtmlEncode((string)Eval("Timespan")) %></td>
                                    <td><%# System.Web.HttpUtility.HtmlEncode((string)Eval("ShortTitle")) %></td>
                                    <td><%# System.Web.HttpUtility.HtmlEncode((string)Eval("Detail")) %></td>
                                    <td><%# CategoryName((int)Eval("CategoryId")) %></td>
                                    <td class="text-end">
                                        <asp:LinkButton runat="server" CssClass="btn btn-sm btn-outline-warning" CommandName="strike" CommandArgument='<%# Eval("EntryId") %>' ToolTip="Strike / unstrike"><i class="fas fa-strikethrough" aria-hidden="true"></i></asp:LinkButton>
                                        <asp:LinkButton runat="server" CssClass="btn btn-sm btn-outline-primary" CommandName="edit" CommandArgument='<%# Eval("EntryId") %>'><i class="fas fa-pen" aria-hidden="true"></i></asp:LinkButton>
                                        <asp:LinkButton runat="server" CssClass="btn btn-sm btn-outline-danger" CommandName="delete" CommandArgument='<%# Eval("EntryId") %>' OnClientClick="return confirm('Delete this entry?');"><i class="fas fa-trash" aria-hidden="true"></i></asp:LinkButton>
                                    </td>
                                </tr>
                            </ItemTemplate>
                        </asp:Repeater>
                    </tbody>
                </table>
            </div></div>
        </asp:Panel>
    </asp:Panel>
</div>
