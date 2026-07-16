<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EditAccounting.ascx.cs" Inherits="tjc.Modules.DigitalCourtReporting.EditAccounting" %>
<div id="navigationLinks" class="btn-group mb-2" role="group" aria-label="Button group with nested dropdown">
    <a class="btn btn-primary active" id="lnkAccounting" href='<%=AccountingUrl %>'>Accounting</a>
    <a class="btn btn-primary" id="lnkInquiry" href="<%=InquiryUrl %>">Inquiry</a>
    <a class="btn btn-primary" id="lnkDCR" href="<%=DCRUrl %>">
        <abbr title="Digital Court Reporting">DCR</abbr></a>
    <a class="btn btn-primary" id="lnkNotification" href="<%=NotificationUrl %>">Notification</a>
    <a class="btn btn-primary" id="lnkStats" href="<%=StatsUrl %>">Stats</a>
    <a class="btn btn-primary" id="lnkComplete" href="<%=CompleteUrl %>">Complete</a>
</div>
<div class="heading heading-border heading-middle-border heading-middle-border-center heading-border-lg mb-1">
    <h2>Audio Request Form (Accounting)</h2>
</div>

<p class="mb-0 text-end">All fields marked with <em class="text-danger">*</em> are required and must be filled in or this form will not be processed.</p>
<div>
    <fieldset class="outline-fieldset mt-0" id="requestorInformation">
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
        <legend>Accounting Information</legend>
        <div class="row form-group">
            <div class="col-auto">
                <label for="txtPaymentReceived">Date Payment Received<em class="text-danger">*</em></label>
                <asp:TextBox AutoCompleteType="Disabled" ID="txtPaymentReceived" ClientIDMode="Static" runat="server" CssClass="form-control date-picker" MaxLength="15"></asp:TextBox>
                <asp:RequiredFieldValidator Display="Dynamic" SetFocusOnError="true" CssClass="label label-danger"
                    ErrorMessage="Received Date Required" ControlToValidate="txtPaymentReceived" runat="server" />
            </div>
            <div class="col-auto">
                <label for="txtCheckMo">Check or MO Number<em class="text-danger">*</em></label>
                <asp:TextBox AutoCompleteType="Disabled" ID="txtCheckMo" ClientIDMode="Static" runat="server" CssClass="form-control" MaxLength="30"></asp:TextBox>
                <asp:RequiredFieldValidator Display="Dynamic" SetFocusOnError="true" CssClass="label label-danger"
                    ErrorMessage="Check Number Required" ControlToValidate="txtCheckMo" runat="server" />
            </div>
            <div class="col-auto">
                <label for="txtPaymentAmount">Payment Amount<em class="text-danger">*</em></label>
                <asp:TextBox AutoCompleteType="Disabled" ID="txtPaymentAmount" step="0.01" min="0" TextMode="Number" ClientIDMode="Static" runat="server" CssClass="form-control" MaxLength="25"></asp:TextBox>
                <asp:RequiredFieldValidator Display="Dynamic" SetFocusOnError="true" CssClass="label label-danger"
                    ErrorMessage="Payment Amount Required" ControlToValidate="txtPaymentAmount" runat="server" />
            </div>
            <div class="col-md-3">
                <label for="txtReceivedBy">Received By<em class="text-danger">*</em></label>
                <asp:TextBox AutoCompleteType="Disabled" ID="txtReceivedBy" ClientIDMode="Static" runat="server" CssClass="form-control" MaxLength="50"></asp:TextBox>
                <asp:RequiredFieldValidator Display="Dynamic" SetFocusOnError="true" CssClass="label label-danger"
                    ErrorMessage="Received By Required" ControlToValidate="txtReceivedBy" runat="server" />
            </div>
        </div>
        <div class="form-group">
                <label for="txtAccountingNotes">Notes</label>
                <asp:TextBox AutoCompleteType="Disabled" ID="txtAccountingNotes" ClientIDMode="Static" runat="server" TextMode="MultiLine" CssClass="form-control" MaxLength="750"></asp:TextBox>
        </div>
    </fieldset>
    <hr />
    <asp:Button Text="Submit" runat="server" ID="cmdSubmit" CssClass="btn btn-primary" OnClick="cmdSubmit_Click" />
    <asp:HyperLink ID="lnkCancel" runat="server" CssClass="btn btn-secondary ms-2">Cancel</asp:HyperLink>
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
