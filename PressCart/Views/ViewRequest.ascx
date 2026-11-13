<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ViewRequest.ascx.cs" Inherits="tjc.Modules.PressCart.ViewRequest" %>

<div class="tjc-presscart-admin">
    <asp:HyperLink ID="lnkBack" runat="server" CssClass="btn btn-secondary text-white mb-3">
        <i class="fa fa-arrow-left" aria-hidden="true"></i>&nbsp;Back to all requests</asp:HyperLink>

    <asp:Panel ID="pnlMissing" runat="server" Visible="false" CssClass="alert alert-warning">
        <i class="fa fa-exclamation-circle" aria-hidden="true"></i>&nbsp;That presentation cart request no longer exists.
    </asp:Panel>

    <asp:Panel ID="pnlRequest" runat="server">
        <h2>
            Request #<asp:Literal ID="ltRequestId" runat="server" />
            <span class="badge <%= StatusBadge %>"><asp:Literal ID="ltStatus" runat="server" /></span>
        </h2>
        <p class="text-muted">
            Submitted <asp:Literal ID="ltSubmitted" runat="server" />.
            <asp:Literal ID="ltStatusChanged" runat="server" />
        </p>

        <fieldset>
            <legend>Requested by</legend>
            <dl class="row">
                <dt class="col-sm-3">Name</dt>
                <dd class="col-sm-9"><asp:Literal ID="ltRequestorName" runat="server" /></dd>
                <dt class="col-sm-3">Email</dt>
                <dd class="col-sm-9"><asp:Literal ID="ltEmail" runat="server" /></dd>
                <dt class="col-sm-3">Phone</dt>
                <dd class="col-sm-9"><asp:Literal ID="ltPhone" runat="server" /></dd>
            </dl>
        </fieldset>

        <fieldset>
            <legend>Contact</legend>
            <dl class="row">
                <dt class="col-sm-3">Name</dt>
                <dd class="col-sm-9"><asp:Literal ID="ltContactName" runat="server" /></dd>
                <dt class="col-sm-3">Email</dt>
                <dd class="col-sm-9"><asp:Literal ID="ltContactEmail" runat="server" /></dd>
                <dt class="col-sm-3">Phone</dt>
                <dd class="col-sm-9"><asp:Literal ID="ltContactPhone" runat="server" /></dd>
            </dl>
        </fieldset>

        <fieldset>
            <legend>Hearing</legend>
            <dl class="row">
                <dt class="col-sm-3">Date</dt>
                <dd class="col-sm-9"><asp:Literal ID="ltCourtDate" runat="server" /></dd>
                <dt class="col-sm-3">Time</dt>
                <dd class="col-sm-9"><asp:Literal ID="ltCourtTime" runat="server" /></dd>
                <dt class="col-sm-3">County</dt>
                <dd class="col-sm-9"><asp:Literal ID="ltCounty" runat="server" /></dd>
                <dt class="col-sm-3">Division / Judge</dt>
                <dd class="col-sm-9"><asp:Literal ID="ltJudge" runat="server" /></dd>
                <dt class="col-sm-3">Courthouse</dt>
                <dd class="col-sm-9"><asp:Literal ID="ltCourthouse" runat="server" /></dd>
                <dt class="col-sm-3">Courtroom</dt>
                <dd class="col-sm-9"><asp:Literal ID="ltCourtroom" runat="server" /></dd>
                <asp:PlaceHolder ID="phTrialLength" runat="server" Visible="false">
                    <dt class="col-sm-3">Days for hearing</dt>
                    <dd class="col-sm-9"><asp:Literal ID="ltTrialLength" runat="server" /></dd>
                </asp:PlaceHolder>
            </dl>
        </fieldset>

        <fieldset>
            <legend>Request</legend>
            <dl class="row">
                <dt class="col-sm-3">Cart type</dt>
                <dd class="col-sm-9"><asp:Literal ID="ltCartType" runat="server" /></dd>
                <dt class="col-sm-3">Training required</dt>
                <dd class="col-sm-9"><asp:Literal ID="ltTraining" runat="server" /></dd>
                <dt class="col-sm-3">Special instructions</dt>
                <dd class="col-sm-9"><asp:Literal ID="ltComments" runat="server" /></dd>
            </dl>
            <asp:Panel ID="pnlEquipment" runat="server" Visible="false">
                <dl class="row">
                    <dt class="col-sm-3">Equipment flagged</dt>
                    <dd class="col-sm-9"><asp:Literal ID="ltEquipment" runat="server" /></dd>
                </dl>
            </asp:Panel>
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
                    <i class="fa fa-trash" aria-hidden="true"></i>&nbsp;Delete request</asp:LinkButton>
            </fieldset>
        </asp:Panel>
    </asp:Panel>
</div>

<script type="text/javascript">
    (function ($) {
        $(document).ready(function () {
            $(".confirm").dnnConfirm({
                text: 'Deleting this request cannot be undone. Are you sure?',
                yesText: 'Yes, delete it',
                noText: 'No',
                title: 'Delete presentation cart request?'
            });
        });
    }(jQuery));
</script>
