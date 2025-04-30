<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EditInquiry.ascx.cs" Inherits="tjc.Modules.DigitalCourtReporting.EditInquiry" %>
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
    <h2>Reporting Inquiry</h2>
</div>
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
    </fieldset>
    <hr />
    <asp:Button Text="Send to DCR Queue" runat="server" ID="cmdSubmit" CssClass="btn btn-primary" OnClick="cmdSubmit_Click" />
    <asp:Button Text="Send to Reporter" runat="server" ID="cmdEmail" CssClass="btn btn-tertiary" OnClick="cmdEmail_Click" />
</div>
