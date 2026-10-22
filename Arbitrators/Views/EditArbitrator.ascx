<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EditArbitrator.ascx.cs" Inherits="tjc.Modules.Arbitrators.Views.EditArbitrator" %>

<div id="ArbitratorEdit" class="container-fluid">
    <asp:Panel ID="pnlNotFound" runat="server" Visible="false" CssClass="alert alert-danger">
        Unable to find the requested record. <asp:HyperLink runat="server" ID="lnkNotFoundBack" Text="Return to List" />
    </asp:Panel>

    <asp:Panel ID="pnlNotAuthorized" runat="server" Visible="false" CssClass="alert alert-danger">
        You do not have permission to edit arbitrator records. <asp:HyperLink runat="server" ID="lnkNotAuthorizedBack" Text="Return to List" />
    </asp:Panel>

    <asp:Panel ID="pnlEdit" runat="server">
        <asp:Literal ID="ltHeading" runat="server" />

        <asp:Panel ID="pnlErrors" runat="server" Visible="false" CssClass="alert alert-danger" role="alert">
            <asp:Literal ID="ltErrors" runat="server" />
        </asp:Panel>

        <div class="row mb-3">
            <div class="col-12 col-lg-3">
                <label for="<%=txtFirstName.ClientID %>" class="form-label">First Name <span class="text-danger" aria-hidden="true">*</span></label>
                <asp:TextBox ID="txtFirstName" runat="server" CssClass="form-control" MaxLength="100" />
            </div>
            <div class="col-12 col-lg-3">
                <label for="<%=txtMiddleName.ClientID %>" class="form-label">Middle Name</label>
                <asp:TextBox ID="txtMiddleName" runat="server" CssClass="form-control" MaxLength="100" />
            </div>
            <div class="col-12 col-lg-4">
                <label for="<%=txtLastName.ClientID %>" class="form-label">Last Name <span class="text-danger" aria-hidden="true">*</span></label>
                <asp:TextBox ID="txtLastName" runat="server" CssClass="form-control" MaxLength="100" />
            </div>
            <div class="col-12 col-lg-2">
                <label for="<%=txtSuffix.ClientID %>" class="form-label">Suffix</label>
                <asp:TextBox ID="txtSuffix" runat="server" CssClass="form-control" MaxLength="20" />
            </div>
        </div>

        <div class="row mb-3">
            <div class="col-12">
                <label for="<%=txtAddress.ClientID %>" class="form-label">Address <span class="text-danger" aria-hidden="true">*</span></label>
                <asp:TextBox ID="txtAddress" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="2" MaxLength="500" />
            </div>
        </div>

        <div class="row mb-3">
            <div class="col-12 col-lg-3">
                <label for="<%=txtPhone.ClientID %>" class="form-label">Phone <span class="text-danger" aria-hidden="true">*</span></label>
                <asp:TextBox ID="txtPhone" runat="server" CssClass="form-control" MaxLength="30" />
            </div>
            <div class="col-12 col-lg-3">
                <label for="<%=txtFax.ClientID %>" class="form-label">Fax</label>
                <asp:TextBox ID="txtFax" runat="server" CssClass="form-control" MaxLength="30" />
            </div>
            <div class="col-12 col-lg-6">
                <label for="<%=txtEmail.ClientID %>" class="form-label">Email <span class="text-danger" aria-hidden="true">*</span></label>
                <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" MaxLength="256" />
            </div>
        </div>

        <div class="row mb-3">
            <div class="col-12">
                <label for="<%=txtWebsite.ClientID %>" class="form-label">Website</label>
                <asp:TextBox ID="txtWebsite" runat="server" CssClass="form-control" MaxLength="300" />
            </div>
        </div>

        <div class="row mb-3">
            <div class="col-12">
                <label for="<%=txtPreferredAreas.ClientID %>" class="form-label">Preferred Areas <span class="text-danger" aria-hidden="true">*</span></label>
                <asp:TextBox ID="txtPreferredAreas" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="3" MaxLength="500" />
            </div>
        </div>

        <div class="row mb-3">
            <div class="col-12">
                <asp:CheckBox ID="chkBarMember" runat="server" Text="Florida Bar Member" CssClass="form-check form-check-inline" />
                <asp:CheckBox ID="chkCertifiedMediator" runat="server" Text="Certified Mediator" CssClass="form-check form-check-inline" />
                <asp:CheckBox ID="chkCompletedTraining" runat="server" Text="Completed Arbitration Training" CssClass="form-check form-check-inline" />
            </div>
        </div>

        <div class="row mb-3">
            <div class="col-12">
                <label for="<%=txtExperience.ClientID %>" class="form-label">Experience Statement</label>
                <asp:TextBox ID="txtExperience" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="5" />
            </div>
        </div>

        <hr />
        <div class="row mb-3">
            <div class="col-12 col-lg-3">
                <label for="<%=ddlStatus.ClientID %>" class="form-label">Status</label>
                <asp:DropDownList ID="ddlStatus" runat="server" CssClass="form-control" />
                <small class="text-muted">Only the Approved status appears on the public list. No email is sent from this page.</small>
            </div>
            <div class="col-12 col-lg-9">
                <label for="<%=txtStatusNotes.ClientID %>" class="form-label">Review Notes</label>
                <asp:TextBox ID="txtStatusNotes" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="3" />
            </div>
        </div>

        <div>
            <asp:LinkButton ID="cmdSave" runat="server" CssClass="btn btn-primary" Text="Save" OnClick="cmdSave_Click" />
            <asp:HyperLink ID="lnkCancel" runat="server" CssClass="btn btn-secondary" Text="Cancel" />
        </div>
    </asp:Panel>
</div>
