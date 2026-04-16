<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Referral.ascx.cs" Inherits="tjc.Modules.JudicialReferral.Views.Referral" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>

<div class="container-fluid mt-3" style="max-width:900px">
    <h3>New Judicial Referral</h3>
    <asp:ValidationSummary ID="valSummary" runat="server" CssClass="alert alert-danger" DisplayMode="BulletList" />

    <div class="card">
        <div class="card-body">
            <div class="form-row mb-3">
                <div class="form-group col-md-6">
                    <label for="<%=drpJudge.ClientID %>">Select Judge <span class="text-danger">*</span></label>
                    <asp:DropDownList ID="drpJudge" runat="server" CssClass="form-control">
                        <asp:ListItem Text="&lt; Select Judge &gt;" Value=""></asp:ListItem>
                    </asp:DropDownList>
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="drpJudge" InitialValue=""
                        CssClass="text-danger" ErrorMessage="Please Select a Judge" Display="None" />
                </div>
                <div class="form-group col-md-6">
                    <label for="<%=drpCounty.ClientID %>">County <span class="text-danger">*</span></label>
                    <asp:DropDownList ID="drpCounty" runat="server" CssClass="form-control">
                        <asp:ListItem Text="&lt; Select County &gt;" Value=""></asp:ListItem>
                        <asp:ListItem Text="DeSoto" Value="D"></asp:ListItem>
                        <asp:ListItem Text="Manatee" Value="M"></asp:ListItem>
                        <asp:ListItem Text="Sarasota" Value="S"></asp:ListItem>
                        <asp:ListItem Text="Venice" Value="V"></asp:ListItem>
                    </asp:DropDownList>
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="drpCounty" InitialValue=""
                        CssClass="text-danger" ErrorMessage="County is Required" Display="None" />
                </div>
            </div>

            <div class="form-row mb-3">
                <div class="form-group col-md-12">
                    <label>Case Number <span class="text-danger">*</span> <small class="text-muted">(Format: 2022 CC 012345)</small></label>
                    <div class="form-inline">
                        <asp:TextBox ID="txtYear" runat="server" MaxLength="4" CssClass="form-control mr-2" Style="width:70px" placeholder="YYYY"></asp:TextBox>
                        <asp:TextBox ID="txtCaseType" runat="server" MaxLength="2" CssClass="form-control mr-2" Style="width:60px" placeholder="CC"></asp:TextBox>
                        <asp:TextBox ID="txtCaseNumber" runat="server" MaxLength="25" CssClass="form-control" Style="width:140px" placeholder="012345"></asp:TextBox>
                    </div>
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="txtYear" CssClass="text-danger" ErrorMessage="Year is Required" Display="None" />
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="txtCaseType" CssClass="text-danger" ErrorMessage="Case Type is Required" Display="None" />
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="txtCaseNumber" CssClass="text-danger" ErrorMessage="Case Number is Required" Display="None" />
                </div>
            </div>

            <div class="form-row mb-3">
                <div class="form-group col-md-12">
                    <label for="<%=txtCaseParties.ClientID %>">Case Name <span class="text-danger">*</span></label>
                    <asp:TextBox ID="txtCaseParties" runat="server" MaxLength="2000" CssClass="form-control" placeholder="Party One v. Party Two"></asp:TextBox>
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="txtCaseParties" CssClass="text-danger" ErrorMessage="Case Name is Required" Display="None" />
                </div>
            </div>

            <div class="form-row mb-3">
                <div class="form-group col-md-8">
                    <label for="<%=txtMotionTitle.ClientID %>">Motion Title <span class="text-danger">*</span></label>
                    <asp:TextBox ID="txtMotionTitle" runat="server" MaxLength="50" CssClass="form-control"></asp:TextBox>
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="txtMotionTitle" CssClass="text-danger" ErrorMessage="Motion Title is Required" Display="None" />
                </div>
                <div class="form-group col-md-4">
                    <label for="<%=txtMotionDate.ClientID %>">Motion Date <span class="text-danger">*</span></label>
                    <asp:TextBox ID="txtMotionDate" runat="server" CssClass="form-control" TextMode="SingleLine" type="date" />
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="txtMotionDate" CssClass="text-danger" ErrorMessage="Motion Date is Required" Display="None" />
                </div>
            </div>

            <div class="form-row mb-3">
                <div class="form-group col-md-12">
                    <label for="fuAttachments">Attachments</label>
                    <p class="text-muted small">Acceptable file types: .docx, .doc, .xls, .xlsx, .pdf</p>
                    <asp:FileUpload ID="fuAttachments" runat="server" AllowMultiple="true" CssClass="form-control-file" accept=".docx,.doc,.xls,.xlsx,.pdf" />
                    <asp:CustomValidator ID="valUpload" runat="server" CssClass="text-danger"
                        ClientValidationFunction="validateUpload" ErrorMessage="Please Attach File" Display="None" />
                    <asp:HiddenField ID="hdJudge" runat="server" ClientIDMode="Static" />
                </div>
            </div>

            <div class="form-row">
                <div class="col">
                    <asp:LinkButton ID="cmdSave" runat="server" CssClass="btn btn-primary" Text="Submit to Judge" OnClick="cmdSave_Click" />
                    <asp:HyperLink ID="cmdCancel" runat="server" CssClass="btn btn-secondary" Text="Cancel" />
                </div>
            </div>
        </div>
    </div>
</div>

<script type="text/javascript">
    function validateUpload(source, args) {
        args.IsValid = true;
        if (document.getElementById("hdJudge").value == "1") {
            args.IsValid = true;
            return;
        }
        var fu = document.getElementById('<%= fuAttachments.ClientID %>');
        if (fu && fu.files && fu.files.length > 0) {
            args.IsValid = true;
        } else {
            args.IsValid = false;
        }
    }
</script>
