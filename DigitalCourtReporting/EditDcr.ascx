<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EditDcr.ascx.cs" Inherits="tjc.Modules.DigitalCourtReporting.EditDcr" %>
<div class="btn-group mb-2" role="group" aria-label="Button group with nested dropdown">
    <a class="btn btn-primary" id="lnkAccounting" href='<%=AccountingUrl %>'>Accounting</a>
    <div class="btn-group" role="group">
        <button id="btnGroupDropInquiry" type="button" class="btn btn-primary dropdown-toggle" data-bs-toggle="dropdown" aria-expanded="false">
            Inquiry
        </button>
        <ul class="dropdown-menu" aria-labelledby="btnGroupDropInquiry">
            <li>
                <a class="dropdown-item" id="lnkInquiry" href="<%=InquiryUrl %>">All Counties</a></li>
            <li>
                <a class="dropdown-item" id="lnkInqDesoto" href="<%=InquiryDeSotoUrl %>">Desoto</a></li>
            <li>
                <a class="dropdown-item" id="lnkInqManatee" href="<%=InquiryManateeUrl %>">Manatee</a></li>
            <li>
                <a class="dropdown-item" id="lnkInqSarasota" href="<%=InquirySarasotaUrl %>">Sarasota</a></li>
        </ul>
    </div>
    <div class="btn-group" role="group">
        <button id="btnGroupDropDCR" type="button" class="btn btn-primary dropdown-toggle" data-bs-toggle="dropdown" aria-expanded="false">
            DCR
        </button>
        <ul class="dropdown-menu" aria-labelledby="btnGroupDropDCR">
            <li>
                <a class="dropdown-item" id="lnkDCR" href="<%=DCRUrl %>">All Counties</a></li>
            <li>
                <a class="dropdown-item" id="lnkDCRDesoto" href="<%=DCRDeSotoUrl %>">Desoto</a></li>
            <li>
                <a class="dropdown-item" id="lnkDCRManatee" href="<%=DCRManateeUrl %>">Manatee</a></li>
            <li>
                <a class="dropdown-item" id="lnkDCRSarasota" href="<%=DCRSarasotaUrl %>">Sarasota</a></li>
        </ul>
    </div>
    <a class="btn btn-primary" id="lnkNotification" href="<%=NotificationUrl %>">Notification</a>
    <a class="btn btn-primary" id="lnkStats" href="<%=StatsUrl %>">Stats</a>
    <div class="btn-group" role="group">
        <button id="btnGroupDropComplete" type="button" class="btn btn-primary dropdown-toggle" data-bs-toggle="dropdown" aria-expanded="false">
            Complete
        </button>
        <ul class="dropdown-menu" aria-labelledby="btnGroupDropComplete">
            <li>
                <a class="dropdown-item" id="lnkComplete" href="<%=CompleteUrl %>">All Counties</a></li>
            <li>
                <a class="dropdown-item" id="lnkCompDesoto" href="<%=CompleteDeSotoUrl %>">Desoto</a></li>
            <li>
                <a class="dropdown-item" id="lnkCompManatee" href="<%=CompleteManateeUrl %>">Manatee</a></li>
            <li>
                <a class="dropdown-item" id="lnkCompSarasota" href="<%=CompleteSarasotaUrl %>">Sarasota</a></li>
        </ul>
    </div>
</div>
<div class="heading heading-border heading-middle-border heading-middle-border-center heading-border-lg mb-1">
    <h2>Audio Request Form (DCR)</h2>
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
    </fieldset>    <fieldset class="outline-fieldset" id="fsDCR" runat="server">
        <legend>DCR Information</legend>
        <div class="row form-group">
            <div class="col-md-4">
                <label for="rblCourOrderAttach">Juv. Court Order Attached<em class="text-danger">*</em></label>
                <asp:RadioButtonList ID="rblCourOrderAttach" ClientIDMode="Static" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" CssClass="radio-button-list">
                    <asp:ListItem>Yes</asp:ListItem>
                    <asp:ListItem>N/A</asp:ListItem>
                </asp:RadioButtonList>
                <asp:RequiredFieldValidator Display="Dynamic" SetFocusOnError="true" CssClass="label label-danger"
                    ErrorMessage="Required" ControlToValidate="rblCourOrderAttach" runat="server" />
            </div>
            <div class="col-4">
                <label for="rblClerkCertAttach">Indig. - Clerk Cert. Attached<em class="text-danger">*</em></label>
                <asp:RadioButtonList ID="rblClerkCertAttach" ClientIDMode="Static" runat="server" RepeatDirection="Horizontal"
                    RepeatLayout="Flow" CssClass="radio-button-list">
                    <asp:ListItem>Yes</asp:ListItem>
                    <asp:ListItem>N/A</asp:ListItem>
                </asp:RadioButtonList>
                <asp:RequiredFieldValidator Display="Dynamic" SetFocusOnError="true" CssClass="label label-danger"
                    ErrorMessage="Required" ControlToValidate="rblClerkCertAttach" runat="server" />
            </div>
            <div class="col-md-4">
                <label for="rblCDType">CD Type<em class="text-danger">*</em></label>
                <asp:RadioButtonList ID="rblCDType" ClientIDMode="Static" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" CssClass="radio-button-list">
                    <asp:ListItem>PC</asp:ListItem>
                    <asp:ListItem>Audio Upload</asp:ListItem>
                </asp:RadioButtonList>
                <asp:RequiredFieldValidator Display="Dynamic" SetFocusOnError="true" CssClass="label label-danger"
                    ErrorMessage="Required" ControlToValidate="rblCDType" runat="server" />
            </div>
        </div>
        <div class="row form-group">
            <div class="col-md-4">
                <label for="txtReceivedBy">Processed By<em class="text-danger">*</em></label>
                <asp:TextBox AutoCompleteType="Disabled" ID="txtProcessedBy" ClientIDMode="Static" runat="server" CssClass="form-control" MaxLength="25"></asp:TextBox>
                <asp:RequiredFieldValidator Display="Dynamic" SetFocusOnError="true" CssClass="label label-danger"
                    ErrorMessage="Processed By Required" ControlToValidate="txtProcessedBy" runat="server" />
            </div>
            <div class="col-md-4">
                <label for="txtTrackingNumber">Tracking Number<em class="text-danger">*</em></label>
                <asp:TextBox AutoCompleteType="Disabled" ID="txtTrackingNumber" ClientIDMode="Static" runat="server" CssClass="form-control" MaxLength="25"></asp:TextBox>
                <asp:RequiredFieldValidator Display="Dynamic" SetFocusOnError="true" CssClass="label label-danger"
                    ErrorMessage="Tracking Number Required" ControlToValidate="txtTrackingNumber" runat="server" />
            </div>
            <div class="col-md-4">
                <label for="txtDateBurned">Date CD Burned<em class="text-danger">*</em></label>
                <asp:TextBox AutoCompleteType="Disabled" ID="txtDateBurned" ClientIDMode="Static" runat="server" CssClass="form-control date-picker" MaxLength="25"></asp:TextBox>
                <asp:RequiredFieldValidator Display="Dynamic" SetFocusOnError="true" CssClass="label label-danger"
                    ErrorMessage="Date Burned Required" ControlToValidate="txtDateBurned" runat="server" />
            </div>
        </div>
        <div class="row form-group">
            <div class="col-md-4">
                <label for="txtTotalMinutes">Total Minutes Burned<em class="text-danger">*</em></label>
                <asp:TextBox AutoCompleteType="Disabled" ID="txtTotalMinutes" ClientIDMode="Static" TextMode="Number" min="0" step="10" runat="server" CssClass="form-control"></asp:TextBox>
                <asp:RequiredFieldValidator Display="Dynamic" SetFocusOnError="true" CssClass="label label-danger"
                    ErrorMessage="Total Minutes Required" ControlToValidate="txtTotalMinutes" runat="server" />
            </div>
            <div class="col-md-4">
                <label for="txtCdsProvided">Total Minutes Burned<em class="text-danger">*</em></label>
                <asp:TextBox AutoCompleteType="Disabled" ID="txtCdsProvided" ClientIDMode="Static" TextMode="Number" min="0" step="10" runat="server" CssClass="form-control"></asp:TextBox>
                <asp:RequiredFieldValidator Display="Dynamic" SetFocusOnError="true" CssClass="label label-danger"
                    ErrorMessage="Number of CDs Required" ControlToValidate="txtCdsProvided" runat="server" />
            </div>
        </div>
        <div class="form-group">
            <label for="txtDCRNotes">Notes</label>
            <asp:TextBox textmode="MultiLine" ID="txtDCRNotes" ClientIDMode="Static" runat="server" CssClass="form-control"></asp:TextBox>
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
