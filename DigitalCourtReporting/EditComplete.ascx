<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EditComplete.ascx.cs" Inherits="tjc.Modules.DigitalCourtReporting.EditComplete" %>
<div id="navigationLinks" class="btn-group mb-2" role="group" aria-label="Button group with nested dropdown">
    <a class="btn btn-primary" id="lnkAccounting" href='<%=AccountingUrl %>'>Accounting</a>
    <a class="btn btn-primary" id="lnkInquiry" href="<%=InquiryUrl %>">Inquiry</a>
    <a class="btn btn-primary" id="lnkDCR" href="<%=DCRUrl %>">
        <abbr title="Digital Court Reporting">DCR</abbr></a>
    <a class="btn btn-primary" id="lnkNotification" href="<%=NotificationUrl %>">Notification</a>
    <a class="btn btn-primary" id="lnkStats" href="<%=StatsUrl %>">Stats</a>
    <a class="btn btn-primary active" id="lnkComplete" href="<%=CompleteUrl %>">Complete</a>
</div>
<div class="heading heading-border heading-middle-border heading-middle-border-center heading-border-lg mb-1">
    <h2>Audio Request Form (Completed)</h2>
</div>
<asp:Button Text="Reopen" runat="server" ID="cmdSubmit" CssClass="btn btn-tertiary" OnClick="cmdSubmit_Click" />
<asp:HyperLink Text="Cancel" runat="server" ID="lnkCancel" CssClass="btn btn-secondary" />
<div>
    <fieldset class="outline-fieldset" id="requestorInformation">
        <legend>Requestor Information</legend>
        <div class="row">
            <div class="col-md-6">
                <div class="mb-1 row">
                    <label for="txtRequestedDate" class="col-sm-4 col-form-label text-end">Requested Date:</label>
                    <div class="col-sm-8">
                        <asp:TextBox ID="txtRequestedDate" ClientIDMode="Static" ReadOnly="true" CssClass="form-control-plaintext" runat="server" />
                    </div>
                </div>
                <div class="mb-1 row">
                    <label for="txtRequestorName" class="col-sm-4 col-form-label text-end">Requestor Name:</label>
                    <div class="col-sm-8">
                        <asp:TextBox ID="txtRequestorName" ClientIDMode="Static" ReadOnly="true" CssClass="form-control-plaintext" runat="server" />
                    </div>
                </div>
                <div class="mb-1 row">
                    <label for="txtAddress" class="col-sm-4 col-form-label text-end">Mailing Address:</label>
                    <div class="col-sm-8">
                        <asp:TextBox ID="txtAddress" ClientIDMode="Static" ReadOnly="true" CssClass="form-control-plaintext" runat="server" />
                    </div>
                </div>
                <div class="mb-1 row">
                    <label for="txtCityStateZip" class="col-sm-4 col-form-label text-end">City, State Zip:</label>
                    <div class="col-sm-8">
                        <asp:TextBox ID="txtCityStateZip" ClientIDMode="Static" ReadOnly="true" CssClass="form-control-plaintext" runat="server" />
                    </div>
                </div>
            </div>
            <div class="col-md-6">
                <div class="mb-1 row">
                    <label for="txtPhone" class="col-sm-4 col-form-label text-end">Phone:</label>
                    <div class="col-sm-8">
                        <asp:TextBox ID="txtPhone" ClientIDMode="Static" ReadOnly="true" CssClass="form-control-plaintext" runat="server" />
                    </div>
                </div>
                <div class="mb-1 row">
                    <label for="txtFax" class="col-sm-4 col-form-label text-end">Fax:</label>
                    <div class="col-sm-8">
                        <asp:TextBox ID="txtFax" ClientIDMode="Static" ReadOnly="true" CssClass="form-control-plaintext" runat="server" />
                    </div>
                </div>
                <div class="mb-1 row">
                    <label for="txtEmail" class="col-sm-4 col-form-label text-end">Email:</label>
                    <div class="col-sm-8">
                        <asp:TextBox ID="txtEmail" ClientIDMode="Static" ReadOnly="true" CssClass="form-control-plaintext" runat="server" />
                    </div>
                </div>
            </div>
        </div>
    </fieldset>
    <fieldset class="outline-fieldset" id="cdChoice">
        <legend>Choice of CD</legend>
        <div class="mb-1 row">
            <label for="txtCdPreference" class="col-sm-auto col-form-label text-end">CD Preference:</label>
            <div class="col">
                <asp:TextBox ID="txtCdPreference" ClientIDMode="Static" ReadOnly="true" CssClass="form-control-plaintext" runat="server" />
            </div>
        </div>
    </fieldset>
    <fieldset class="outline-fieldset" id="proceedingInfo">
        <legend>Proceeding Information</legend>
        <div class="row">
            <div class="col-md-6">
                <div class="mb-1 row">
                    <label for="txtCaseName" class="col-sm-4 col-form-label text-end">Case Name:</label>
                    <div class="col-sm-8">
                        <asp:TextBox ID="txtCaseName" ClientIDMode="Static" ReadOnly="true" CssClass="form-control-plaintext" runat="server" />
                    </div>
                </div>
                <div class="mb-1 row">
                    <label for="txtProceedingDate" class="col-sm-4 col-form-label text-end">Date of Proceeding :</label>
                    <div class="col-sm-8">
                        <asp:TextBox ID="txtProceedingDate" ClientIDMode="Static" ReadOnly="true" CssClass="form-control-plaintext" runat="server" />
                    </div>
                </div>
                <div class="mb-1 row">
                    <label for="txtLocation" class="col-sm-4 col-form-label text-end">Location of Proceeding:</label>
                    <div class="col-sm-8">
                        <asp:TextBox ID="txtLocation" ClientIDMode="Static" ReadOnly="true" CssClass="form-control-plaintext" runat="server" />
                    </div>
                </div>
                <div class="mb-1 row">
                    <label for="txtJurisdiction" class="col-sm-4 col-form-label text-end">Jurisdiction:</label>
                    <div class="col-sm-8">
                        <asp:TextBox ID="txtJurisdiction" ClientIDMode="Static" ReadOnly="true" CssClass="form-control-plaintext" runat="server" />
                    </div>
                </div>
                <div class="mb-1 row">
                    <label for="txtCaseInvolvment" class="col-sm-4 col-form-label text-end">Involvement in Case:</label>
                    <div class="col-sm-8">
                        <asp:TextBox ID="txtCaseInvolvment" ClientIDMode="Static" ReadOnly="true" CssClass="form-control-plaintext" runat="server" />
                    </div>
                </div>
            </div>
            <div class="col-md-6">
                <div class="mb-1 row">
                    <label for="txtCaseNumber" class="col-sm-4 col-form-label text-end">Case Number:</label>
                    <div class="col-sm-8">
                        <asp:TextBox ID="txtCaseNumber" ClientIDMode="Static" ReadOnly="true" CssClass="form-control-plaintext" runat="server" />
                    </div>
                </div>
                <div class="mb-1 row">
                    <label for="txtProceedingTime" class="col-sm-4 col-form-label text-end">Time of Proceeding:</label>
                    <div class="col-sm-8">
                        <asp:TextBox ID="txtProceedingTime" ClientIDMode="Static" ReadOnly="true" CssClass="form-control-plaintext" runat="server" />
                    </div>
                </div>
                <div class="mb-1 row">
                    <label for="txtProceddingType" class="col-sm-4 col-form-label text-end">Type of Proceeding:</label>
                    <div class="col-sm-8">
                        <asp:TextBox ID="txtProceddingType" ClientIDMode="Static" ReadOnly="true" CssClass="form-control-plaintext" runat="server" />
                    </div>
                </div>
                <div class="mb-1 row">
                    <label for="txtPresidingJudge" class="col-sm-4 col-form-label text-end">Presiding Judge/Magistrate:</label>
                    <div class="col-sm-8">
                        <asp:TextBox ID="txtPresidingJudge" ClientIDMode="Static" ReadOnly="true" CssClass="form-control-plaintext" runat="server" />
                    </div>
                </div>
            </div>
        </div>
    </fieldset>
    <fieldset class="outline-fieldset">
        <legend>Special Instructions</legend>
        <asp:Literal ID="ltNotes" runat="server"></asp:Literal>
    </fieldset>
    <fieldset class="outline-fieldset" id="fsAccounting" runat="server">
        <legend>Accounting Information</legend>
        <div class="row">
            <div class="col-md-6">
                <div class="mb-1 row">
                    <label for="txtPaymentReceived" class="col-sm-4 col-form-label text-end">Date Payment Received:</label>
                    <div class="col-sm-8">
                        <asp:TextBox ID="txtPaymentReceived" ClientIDMode="Static" ReadOnly="true" CssClass="form-control-plaintext" runat="server" />
                    </div>
                </div>
                <div class="mb-1 row">
                    <label for="txtCheckMo" class="col-sm-4 col-form-label text-end">Check or MO Number:</label>
                    <div class="col-sm-8">
                        <asp:TextBox ID="txtCheckMo" ClientIDMode="Static" ReadOnly="true" CssClass="form-control-plaintext" runat="server" />
                    </div>
                </div>
            </div>
            <div class="col-md-6">
                <div class="mb-1 row">
                    <label for="txtPaymentAmount" class="col-sm-4 col-form-label text-end">Payment Amount:</label>
                    <div class="col-sm-8">
                        <asp:TextBox ID="txtPaymentAmount" ClientIDMode="Static" ReadOnly="true" CssClass="form-control-plaintext" runat="server" />
                    </div>
                </div>
                <div class="mb-1 row">
                    <label for="txtReceivedBy" class="col-sm-4 col-form-label text-end">Received By:</label>
                    <div class="col-sm-8">
                        <asp:TextBox ID="txtReceivedBy" ClientIDMode="Static" ReadOnly="true" CssClass="form-control-plaintext" runat="server" />
                    </div>
                </div>
            </div>
        </div>
        <div class="form-group">
            <label for="txtAccountingNotes">Notes</label>
            <asp:TextBox ID="txtAccountingNotes" ClientIDMode="Static" TextMode="MultiLine" ReadOnly="true" CssClass="form-control-plaintext border rounded p-1" runat="server" />
        </div>
    </fieldset>
    <fieldset class="outline-fieldset" id="fsDCR" runat="server">
        <legend>DCR Information</legend>
        <div class="row">
            <div class="col-md-4">
                <div class="mb-1 row">
                    <label for="txtJuvenile" class="col-sm-4 col-form-label text-end">Juv. Court Order Attache:</label>
                    <div class="col-sm-8">
                        <asp:TextBox ID="txtJuvenile" ClientIDMode="Static" ReadOnly="true" CssClass="form-control-plaintext" runat="server" />
                    </div>
                </div>
                <div class="mb-1 row">
                    <label for="txtIndigentCertAttach" class="col-sm-4 col-form-label text-end">Indig. - Clerk Cert. Attached:</label>
                    <div class="col-sm-8">
                        <asp:TextBox ID="txtIndigentCertAttach" ClientIDMode="Static" ReadOnly="true" CssClass="form-control-plaintext" runat="server" />
                    </div>
                </div>
                <div class="mb-1 row">
                    <label for="txtCdType" class="col-sm-4 col-form-label text-end">CD Type:</label>
                    <div class="col-sm-8">
                        <asp:TextBox ID="txtCdType" ClientIDMode="Static" ReadOnly="true" CssClass="form-control-plaintext" runat="server" />
                    </div>
                </div>
            </div>
            <div class="col-md-4">
                <div class="mb-1 row">
                    <label for="txtProcessedBy" class="col-sm-4 col-form-label text-end">Processed By:</label>
                    <div class="col-sm-8">
                        <asp:TextBox ID="txtProcessedBy" ClientIDMode="Static" ReadOnly="true" CssClass="form-control-plaintext" runat="server" />
                    </div>
                </div>
                <div class="mb-1 row">
                    <label for="txtTrackingNumber" class="col-sm-4 col-form-label text-end">Tracking Number:</label>
                    <div class="col-sm-8">
                        <asp:TextBox ID="txtTrackingNumber" ClientIDMode="Static" ReadOnly="true" CssClass="form-control-plaintext" runat="server" />
                    </div>
                </div>
                <div class="mb-1 row">
                    <label for="txtDateBurned" class="col-sm-4 col-form-label text-end">Date CD Burned:</label>
                    <div class="col-sm-8">
                        <asp:TextBox ID="txtDateBurned" ClientIDMode="Static" ReadOnly="true" CssClass="form-control-plaintext" runat="server" />
                    </div>
                </div>
            </div>
            <div class="col-md-4">
                <div class="mb-1 row">
                    <label for="txtTotalMinutes" class="col-sm-4 col-form-label text-end">Total Minutes Burned:</label>
                    <div class="col-sm-8">
                        <asp:TextBox ID="txtTotalMinutes" ClientIDMode="Static" ReadOnly="true" CssClass="form-control-plaintext" runat="server" />
                    </div>
                </div>
                <div class="mb-1 row">
                    <label for="txtCDProvided" class="col-sm-4 col-form-label text-end"># CD&#39;s Provided:</label>
                    <div class="col-sm-8">
                        <asp:TextBox ID="txtCDProvided" ClientIDMode="Static" ReadOnly="true" CssClass="form-control-plaintext" runat="server" />
                    </div>
                </div>
            </div>
        </div>
        <div class="form-group">
            <label for="txtNotesDCR">Notes</label>
            <asp:TextBox ID="txtNotesDCR" ClientIDMode="Static" TextMode="MultiLine" ReadOnly="true" CssClass="form-control-plaintext border rounded p-1" runat="server" />
        </div>
    </fieldset>
    <fieldset class="outline-fieldset">
        <legend>Court Administration Call Back Information</legend>
        <div class="row">
            <div class="col-md-6">
                <div class="mb-1 row">
                    <label for="txtNotification" class="col-sm-4 col-form-label text-end">Notification:</label>
                    <div class="col-sm-8">
                        <asp:TextBox ID="txtNotification" ClientIDMode="Static" ReadOnly="true" CssClass="form-control-plaintext" runat="server" />
                    </div>
                </div>
                <div class="mb-1 row">
                    <label for="txtCalledPerson" class="col-sm-4 col-form-label text-end">Person Spoke With:</label>
                    <div class="col-sm-8">
                        <asp:TextBox ID="txtCalledPerson" ClientIDMode="Static" ReadOnly="true" CssClass="form-control-plaintext" runat="server" />
                    </div>
                </div>
                <div class="mb-1 row">
                    <label for="txtDateCalled" class="col-sm-4 col-form-label text-end">Date Called/Mailed/Deliver:</label>
                    <div class="col-sm-8">
                        <asp:TextBox ID="txtDateCalled" ClientIDMode="Static" ReadOnly="true" CssClass="form-control-plaintext" runat="server" />
                    </div>
                </div>
            </div>
            <div class="col-md-6">
                <div class="mb-1 row">
                    <label for="txtRecipient" class="col-sm-4 col-form-label text-end">Picked Up By/Delivered To:</label>
                    <div class="col-sm-8">
                        <asp:TextBox ID="txtRecipient" ClientIDMode="Static" ReadOnly="true" CssClass="form-control-plaintext" runat="server" />
                    </div>
                </div>
                <div class="mb-1 row">
                    <label for="txtCalledBy" class="col-sm-4 col-form-label text-end">Called/Mailed By:</label>
                    <div class="col-sm-8">
                        <asp:TextBox ID="txtCalledBy" ClientIDMode="Static" ReadOnly="true" CssClass="form-control-plaintext" runat="server" />
                    </div>
                </div>
                <div class="mb-1 row">
                    <label for="txtDatePickedUp" class="col-sm-4 col-form-label text-end">Picked Up Date:</label>
                    <div class="col-sm-8">
                        <asp:TextBox ID="txtDatePickedUp" ClientIDMode="Static" ReadOnly="true" CssClass="form-control-plaintext" runat="server" />
                    </div>
                </div>
            </div>
        </div>
        <div class="form-group">
            <label for="txtCourtAdminNotes">Notes</label>
            <asp:TextBox ID="txtCourtAdminNotes" ClientIDMode="Static" TextMode="MultiLine" ReadOnly="true" CssClass="form-control-plaintext border rounded p-1" runat="server" />
        </div>
    </fieldset>
</div>
<script type="text/javascript">
    /*globals jQuery, window, Sys */
    (function ($, Sys) {

        $(document).ready(function () {
            $(".date-picker").on("blur", function (e) {
                var date = $(this).val();
                $(this).val(date.replace(/\.|-/g, "/"));
            });
        });

    }(jQuery, window.Sys));
</script>
