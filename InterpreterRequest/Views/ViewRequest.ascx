<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ViewRequest.ascx.cs" Inherits="tjc.Modules.InterpreterRequest.ViewRequest" %>

<div class="tjc-interpreter-admin">
    <asp:HyperLink ID="lnkBack" runat="server" CssClass="btn btn-secondary text-white mb-3">
        <i class="fa fa-arrow-left" aria-hidden="true"></i>&nbsp;Back to all requests</asp:HyperLink>

    <asp:Panel ID="pnlMissing" runat="server" Visible="false" CssClass="alert alert-warning">
        <i class="fa fa-exclamation-circle" aria-hidden="true"></i>&nbsp;That interpreter request no longer exists.
    </asp:Panel>

    <asp:Panel ID="pnlRequest" runat="server">
        <h2>
            Request #<asp:Literal ID="ltRequestId" runat="server" />
            <span class="badge <%= StatusBadge %>"><asp:Literal ID="ltStatus" runat="server" /></span>
        </h2>
        <p class="text-muted">
            Submitted <asp:Literal ID="ltSubmitted" runat="server" /> on the
            <asp:Literal ID="ltFormLanguage" runat="server" /> form.
            <asp:Literal ID="ltStatusChanged" runat="server" />
        </p>

        <fieldset>
            <legend>Interpreter language</legend>
            <dl class="row">
                <dt class="col-sm-3">Language</dt>
                <dd class="col-sm-9"><asp:Literal ID="ltLanguage" runat="server" /></dd>
            </dl>
        </fieldset>

        <fieldset>
            <legend>Case information</legend>
            <dl class="row">
                <dt class="col-sm-3">Case number</dt>
                <dd class="col-sm-9"><asp:Literal ID="ltCaseNumber" runat="server" /></dd>
                <dt class="col-sm-3">Person needing interpreter</dt>
                <dd class="col-sm-9"><asp:Literal ID="ltPersonNeeding" runat="server" /></dd>
                <dt class="col-sm-3">Name</dt>
                <dd class="col-sm-9"><asp:Literal ID="ltPersonName" runat="server" /></dd>
                <dt class="col-sm-3">Type of event</dt>
                <dd class="col-sm-9"><asp:Literal ID="ltEventType" runat="server" /></dd>
            </dl>
        </fieldset>

        <fieldset>
            <legend>Date and time needed</legend>
            <dl class="row">
                <dt class="col-sm-3">Date</dt>
                <dd class="col-sm-9"><asp:Literal ID="ltDateNeeded" runat="server" /></dd>
                <dt class="col-sm-3">Time</dt>
                <dd class="col-sm-9"><asp:Literal ID="ltTimeNeeded" runat="server" /></dd>
                <dt class="col-sm-3">Estimated length</dt>
                <dd class="col-sm-9"><asp:Literal ID="ltDuration" runat="server" /></dd>
            </dl>
        </fieldset>

        <fieldset>
            <legend>Location of appearance</legend>
            <dl class="row">
                <dt class="col-sm-3">County</dt>
                <dd class="col-sm-9"><asp:Literal ID="ltCounty" runat="server" /></dd>
                <dt class="col-sm-3">Courtroom</dt>
                <dd class="col-sm-9"><asp:Literal ID="ltCourtroom" runat="server" /></dd>
                <dt class="col-sm-3">Appearance method</dt>
                <dd class="col-sm-9"><asp:Literal ID="ltAppearanceMethod" runat="server" /></dd>
                <dt class="col-sm-3">Presiding judge</dt>
                <dd class="col-sm-9"><asp:Literal ID="ltJudge" runat="server" /></dd>
            </dl>
        </fieldset>

        <fieldset>
            <legend>Person requesting the interpreter</legend>
            <dl class="row">
                <dt class="col-sm-3">Name</dt>
                <dd class="col-sm-9"><asp:Literal ID="ltRequestorName" runat="server" /></dd>
                <dt class="col-sm-3">Phone</dt>
                <dd class="col-sm-9"><asp:Literal ID="ltRequestorPhone" runat="server" /></dd>
                <dt class="col-sm-3">Email</dt>
                <dd class="col-sm-9"><asp:Literal ID="ltRequestorEmail" runat="server" /></dd>
                <dt class="col-sm-3">Requestor is the attorney</dt>
                <dd class="col-sm-9"><asp:Literal ID="ltIsAttorney" runat="server" /></dd>
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
                title: 'Delete interpreter request?'
            });
        });
    }(jQuery));
</script>
