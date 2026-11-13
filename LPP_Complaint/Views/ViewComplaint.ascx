<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ViewComplaint.ascx.cs" Inherits="tjc.Modules.LPP_Complaint.ViewComplaint" %>

<div class="tjc-lpp-admin">
    <asp:HyperLink ID="lnkBack" runat="server" CssClass="btn btn-secondary text-white mb-3">
        <i class="fa fa-arrow-left" aria-hidden="true"></i>&nbsp;Back to all complaints</asp:HyperLink>

    <asp:Panel ID="pnlMissing" runat="server" Visible="false" CssClass="alert alert-warning">
        <i class="fa fa-exclamation-circle" aria-hidden="true"></i>&nbsp;That complaint no longer exists.
    </asp:Panel>

    <asp:Panel ID="pnlComplaint" runat="server">
        <h2>
            Complaint #<asp:Literal ID="ltComplaintId" runat="server" />
            <span class="badge <%= StatusBadge %>"><asp:Literal ID="ltStatus" runat="server" /></span>
        </h2>
        <p class="text-muted">
            Submitted <asp:Literal ID="ltSubmitted" runat="server" />.
            <asp:Literal ID="ltStatusChanged" runat="server" />
        </p>

        <fieldset>
            <legend>Referring party</legend>
            <dl class="row">
                <dt class="col-sm-3">Name</dt>
                <dd class="col-sm-9"><asp:Literal ID="ltReferrerName" runat="server" /></dd>
                <dt class="col-sm-3">Email</dt>
                <dd class="col-sm-9"><asp:Literal ID="ltReferrerEmail" runat="server" /></dd>
                <dt class="col-sm-3">Address</dt>
                <dd class="col-sm-9"><asp:Literal ID="ltReferrerAddress" runat="server" /></dd>
                <dt class="col-sm-3">Telephone</dt>
                <dd class="col-sm-9"><asp:Literal ID="ltReferrerPhone" runat="server" /></dd>
                <dt class="col-sm-3">Relationship with the attorney</dt>
                <dd class="col-sm-9"><asp:Literal ID="ltRelationship" runat="server" /></dd>
            </dl>
        </fieldset>

        <fieldset>
            <legend>Attorney being referred</legend>
            <dl class="row">
                <dt class="col-sm-3">Name</dt>
                <dd class="col-sm-9"><asp:Literal ID="ltAttorneyName" runat="server" /></dd>
                <dt class="col-sm-3">Bar number</dt>
                <dd class="col-sm-9"><asp:Literal ID="ltBarNumber" runat="server" /></dd>
                <dt class="col-sm-3">Address</dt>
                <dd class="col-sm-9"><asp:Literal ID="ltAttorneyAddress" runat="server" /></dd>
                <dt class="col-sm-3">Telephone</dt>
                <dd class="col-sm-9"><asp:Literal ID="ltAttorneyPhone" runat="server" /></dd>
                <dt class="col-sm-3">Email</dt>
                <dd class="col-sm-9"><asp:Literal ID="ltAttorneyEmail" runat="server" /></dd>
            </dl>
        </fieldset>

        <fieldset>
            <legend>Nature of the incident</legend>
            <asp:Panel ID="pnlNoNatures" runat="server" Visible="false" CssClass="text-muted">
                Nothing was ticked on the form.
            </asp:Panel>
            <asp:Repeater ID="rptNatures" runat="server">
                <HeaderTemplate><ul></HeaderTemplate>
                <ItemTemplate><li><%# Server.HtmlEncode(Container.DataItem.ToString()) %></li></ItemTemplate>
                <FooterTemplate></ul></FooterTemplate>
            </asp:Repeater>
        </fieldset>

        <fieldset>
            <legend>Alleged violation</legend>
            <dl class="row">
                <dt class="col-sm-3">Date</dt>
                <dd class="col-sm-9"><asp:Literal ID="ltViolationDate" runat="server" /></dd>
                <dt class="col-sm-3">County</dt>
                <dd class="col-sm-9"><asp:Literal ID="ltCounty" runat="server" /></dd>
                <dt class="col-sm-3">Incident</dt>
                <dd class="col-sm-9"><asp:Literal ID="ltIncident" runat="server" /></dd>
                <dt class="col-sm-3">Signed</dt>
                <dd class="col-sm-9"><asp:Literal ID="ltSigned" runat="server" /></dd>
            </dl>
        </fieldset>

        <asp:Panel ID="pnlManage" runat="server">
            <fieldset>
                <legend>Handling</legend>
                <div class="mb-3 row">
                    <div class="col-md-4">
                        <asp:Label ID="lblStatus" runat="server" AssociatedControlID="drpStatus" Text="Status" />
                        <asp:DropDownList ID="drpStatus" runat="server" CssClass="form-control" />
                    </div>
                </div>
                <div class="mb-3">
                    <asp:Label ID="lblNotes" runat="server" AssociatedControlID="txtNotes" Text="Notes" />
                    <asp:TextBox ID="txtNotes" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="4" />
                </div>
                <asp:Button ID="cmdSaveStatus" runat="server" CssClass="btn btn-primary" Text="Save"
                    OnClick="cmdSaveStatus_Click" />
                <asp:LinkButton ID="cmdDelete" runat="server" CssClass="btn btn-danger text-white confirm float-end"
                    CausesValidation="false" OnClick="cmdDelete_Click">
                    <i class="fa fa-trash" aria-hidden="true"></i>&nbsp;Delete complaint</asp:LinkButton>
            </fieldset>
        </asp:Panel>
    </asp:Panel>
</div>

<script type="text/javascript">
    (function ($) {
        $(document).ready(function () {
            $(".confirm").dnnConfirm({
                text: 'Deleting this complaint cannot be undone. Are you sure?',
                yesText: 'Yes, delete it',
                noText: 'No',
                title: 'Delete complaint?'
            });
        });
    }(jQuery));
</script>
