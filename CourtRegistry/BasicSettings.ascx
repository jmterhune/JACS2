<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BasicSettings.ascx.cs" Inherits="tjc.Modules.CourtRegistry.BasicSettings" %>
<div class="tabs">
    <ul class="nav nav-tabs">
        <li class="nav-item">
            <a class="nav-link" href="<%=ApplicationListUrl%>">Applications</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=ManageYearsUrl%>">Manage Fiscal Years</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=ExceptionListUrl%>">Exceptions</a>
        </li>
        <li class="nav-item active">
            <a class="nav-link" href="#basic" data-toggle="tab">Basic Settings</a>
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
        <div id="basic" class="tab-pane active">
            <div class="row form-group">
                <div class="col-md-3">
                    <asp:Label runat="server" AssociatedControlID="drpMonth" Text="Begin Fiscal Year Month" />
                    <asp:DropDownList runat="server" ID="drpMonth" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="drpMonth_SelectedIndexChanged" />
                </div>
                <div class="col-md-3">
                    <asp:Label runat="server" AssociatedControlID="drpDay" Text="Begin Fiscal Year Day" />
                    <asp:DropDownList runat="server" ID="drpDay" CssClass="form-control" />
                </div>
            </div>
            <div class="row form-group">
                <div class="col-md-6">
                    <asp:Label runat="server" AssociatedControlID="txtContactEmail" Text="Contact Email" />
                    <asp:TextBox runat="server" ID="txtContactEmail" CssClass="form-control" MaxLength="250" />
                </div>
                <div class="col-md-6">
                    <asp:Label runat="server" AssociatedControlID="txtUpdateNotificationSendTo" Text="JAC Update Notification Recipients" />
                    <asp:TextBox runat="server" ID="txtUpdateNotificationSendTo" CssClass="form-control" MaxLength="500" />
                </div>
            </div>
            <div class="row form-group">
                <div class="col-md-12">
                    <asp:Label runat="server" AssociatedControlID="txtEditAttorneyNote" Text="Edit Attorney Note" />
                    <asp:TextBox runat="server" ID="txtEditAttorneyNote" CssClass="form-control" TextMode="MultiLine" Rows="4" />
                </div>
            </div>
            <div class="row form-group">
                <div class="col-md-12">
                    <asp:Label runat="server" AssociatedControlID="txtEditApplicationNote" Text="Edit Application Note" />
                    <asp:TextBox runat="server" ID="txtEditApplicationNote" CssClass="form-control" TextMode="MultiLine" Rows="4" />
                </div>
            </div>
            <div class="row form-group">
                <div class="col-md-12">
                    <asp:Label runat="server" AssociatedControlID="txtVerificationNote" Text="Verification Note" />
                    <asp:TextBox runat="server" ID="txtVerificationNote" CssClass="form-control" TextMode="MultiLine" Rows="4" />
                </div>
            </div>
            <div class="row form-group">
                <div class="col-md-12">
                    <asp:Label runat="server" AssociatedControlID="txtApplicationEmail" Text="Application Email Body" />
                    <asp:TextBox runat="server" ID="txtApplicationEmail" CssClass="form-control" TextMode="MultiLine" Rows="4" />
                </div>
            </div>
            <p>
                <asp:Button ID="cmdSave" runat="server" Text="Save Settings" CssClass="btn btn-primary" OnClick="cmdSave_Click" />
                <asp:HyperLink ID="lnkCancel" runat="server" Text="Cancel" CssClass="btn btn-default" />
                <asp:Literal ID="ltMessage" runat="server" />
            </p>
        </div>
    </div>
</div>
