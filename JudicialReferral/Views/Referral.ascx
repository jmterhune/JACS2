<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Referral.ascx.cs" Inherits="tjc.Modules.JudicialReferral.Views.Referral" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>

<div class="container-fluid">
    <h3>New Judicial Referral</h3>
    <asp:ValidationSummary ID="valSummary" runat="server" CssClass="alert alert-danger" DisplayMode="BulletList" />

            <div class="row mb-3">
                <div class="col-12 col-lg-4 mb-3 mb-lg-0">
                    <label for="<%=drpJudge.ClientID %>">Select Judge <span class="text-danger">*</span></label>
                    <asp:DropDownList ID="drpJudge" runat="server" CssClass="form-control">
                        <asp:ListItem Text="&lt; Select Judge &gt;" Value=""></asp:ListItem>
                    </asp:DropDownList>
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="drpJudge" InitialValue=""
                        CssClass="text-danger" ErrorMessage="Please Select a Judge" Display="Dynamic" />
                </div>
                <div class="col-12 col-lg-8">
                    <label for="drpCountyLetter">Case Number <span class="text-danger">*</span></label>
                    <div class="input-group">
                        <asp:DropDownList ID="drpCountyLetter" runat="server" title="County (D=DeSoto, M=Manatee, S=Sarasota, V=Venice)" CssClass="form-control county-letter" ClientIDMode="Static">
                            <asp:ListItem Text="" Value=""></asp:ListItem>
                            <asp:ListItem Text="D" Value="D" title="DeSoto"></asp:ListItem>
                            <asp:ListItem Text="M" Value="M" title="Manatee"></asp:ListItem>
                            <asp:ListItem Text="S" Value="S" title="Sarasota"></asp:ListItem>
                            <asp:ListItem Text="V" Value="V" title="Venice"></asp:ListItem>
                        </asp:DropDownList>
                        <asp:TextBox ID="txtCaseYear" title="Year" runat="server" MaxLength="4" CssClass="form-control case-year" placeholder="YYYY" ClientIDMode="Static"></asp:TextBox>
                        <asp:TextBox ID="txtCaseType" title="Case Type" runat="server" MaxLength="2" CssClass="form-control upperCase case-type" placeholder="CT" ClientIDMode="Static"></asp:TextBox>
                        <asp:TextBox ID="txtCaseSequence" title="Case Sequence" runat="server" MaxLength="6" CssClass="form-control upperCase case-sequence" placeholder="000000" ClientIDMode="Static"></asp:TextBox>
                        <asp:TextBox ID="txtDefendantSuffix" title="Defendant Suffix" runat="server" MaxLength="10" CssClass="form-control upperCase defendant-suffix" ClientIDMode="Static"></asp:TextBox>
                        <small class="input-group-text" title="County-Year-Case Type-Case Sequence">(Format: C-YYYY-CT-<span id="caseFormat">000000</span>)</small>
                    </div>
                    <small class="form-text text-muted d-block">
                        First field is the <strong>County</strong>, then Year, Case Type, Case Sequence, and (for CF cases) Defendant Suffix.
                    </small>
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="drpCountyLetter" InitialValue="" CssClass="text-danger" Display="Dynamic" ErrorMessage="County is Required" />
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="txtCaseYear" CssClass="text-danger" Display="Dynamic" ErrorMessage="Case Year is Required" />
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="txtCaseType" CssClass="text-danger" Display="Dynamic" ErrorMessage="Case Type is Required" />
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="txtCaseSequence" CssClass="text-danger" Display="Dynamic" ErrorMessage="Case Sequence is Required" />
                </div>
            </div>

            <div class="row mb-3">
                <div class="col-12 col-lg-6 mb-3 mb-lg-0">
                    <label for="<%=txtCaseParties.ClientID %>">Case Name <span class="text-danger">*</span></label>
                    <asp:TextBox ID="txtCaseParties" runat="server" MaxLength="2000" CssClass="form-control" placeholder="Party One v. Party Two"></asp:TextBox>
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="txtCaseParties" CssClass="text-danger" ErrorMessage="Case Name is Required" Display="None" />
                </div>
                <div class="col-12 col-md-8 col-lg-4 mb-3 mb-md-0">
                    <label for="<%=txtMotionTitle.ClientID %>">Motion Title <span class="text-danger">*</span></label>
                    <asp:TextBox ID="txtMotionTitle" runat="server" MaxLength="50" CssClass="form-control"></asp:TextBox>
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="txtMotionTitle" CssClass="text-danger" ErrorMessage="Motion Title is Required" Display="None" />
                </div>
                <div class="col-12 col-md-4 col-lg-2">
                    <label for="<%=txtMotionDate.ClientID %>">Motion Date <span class="text-danger">*</span></label>
                    <asp:TextBox ID="txtMotionDate" runat="server" CssClass="form-control" TextMode="SingleLine" type="date" />
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="txtMotionDate" CssClass="text-danger" ErrorMessage="Motion Date is Required" Display="None" />
                </div>
            </div>

            <div class="row">
                <div class="col-md-12">
                    <label for="fuAttachments">Attachments</label>
                    <p class="text-muted small">Acceptable file types: .docx, .doc, .xls, .xlsx, .pdf</p>
                    <asp:FileUpload ID="fuAttachments" runat="server" AllowMultiple="true" CssClass="form-control-file" accept=".docx,.doc,.xls,.xlsx,.pdf" />
                    <asp:CustomValidator ID="valUpload" runat="server" CssClass="text-danger"
                        ClientValidationFunction="validateUpload" ErrorMessage="Please Attach File" Display="None" />
                    <asp:HiddenField ID="hdJudge" runat="server" ClientIDMode="Static" />
                </div>
            </div>
    <hr />
            <div class="row">
                <div class="col">
                    <asp:LinkButton ID="cmdSave" runat="server" CssClass="btn btn-primary" Text="Submit to Judge" OnClick="cmdSave_Click" />
                    <asp:HyperLink ID="cmdCancel" runat="server" CssClass="btn btn-secondary" Text="Cancel" />
                </div>
            </div>
</div>

<script type="text/javascript">
    function validateUpload(source, args) {
        args.IsValid = false;
        if (document.getElementById("hdJudge").value == "1") {
            args.IsValid = true;
            return;
        }
        var fu = document.getElementById('<%= fuAttachments.ClientID %>');
        if (!fu || !fu.files || fu.files.length === 0) return;

        // Match production's Telerik RadUpload extension check.
        var allowed = ['.docx', '.doc', '.xls', '.xlsx', '.pdf'];
        for (var i = 0; i < fu.files.length; i++) {
            var name = (fu.files[i].name || '').toLowerCase();
            var idx = name.lastIndexOf('.');
            if (idx < 0) return;
            var ext = name.substring(idx);
            if (allowed.indexOf(ext) === -1) return;
        }
        args.IsValid = true;
    }

    (function ($) {
        $(document).ready(function () {
            InitCaseNumberWidget();
        });
    }(jQuery));

    function InitCaseNumberWidget() {
        // Force any field tagged .upperCase to keep its actual value upper case
        jQuery(".upperCase").each(function () {
            var $el = jQuery(this);
            $el.val(($el.val() || "").toUpperCase());
        });
        jQuery(".upperCase").on("input change paste keyup", function () {
            var $el = jQuery(this);
            var v = $el.val() || "";
            var u = v.toUpperCase();
            if (v !== u) $el.val(u);
        });

        if (jQuery("#txtCaseSequence").val() === "") {
            jQuery("#txtCaseSequence").mask("000000");
        } else {
            PadCaseSequence();
        }
        // Auto-pad to 6 chars when the user leaves the sequence field
        jQuery("#txtCaseSequence").on("blur", PadCaseSequence);

        if (jQuery("#txtCaseType").val() !== "CF") {
            jQuery("#txtDefendantSuffix").hide();
        } else {
            MaskCaseSequence("CF");
        }
        // Force initial value upper-case (in case the loaded value is lower)
        jQuery("#txtCaseType").val((jQuery("#txtCaseType").val() || "").toUpperCase());
        jQuery("#txtCaseType").on("input blur", function () {
            var ct = (jQuery(this).val() || "").toUpperCase();
            jQuery(this).val(ct);
            if (ct === "CF") {
                jQuery("#txtDefendantSuffix").show();
            } else {
                jQuery("#txtDefendantSuffix").val("").hide();
            }
            MaskCaseSequence(ct);
        });
        jQuery("#drpCountyLetter").on("change", function () {
            MaskCaseSequence(jQuery("#txtCaseType").val());
        });
    }

    function PadCaseSequence() {
        var $el = jQuery("#txtCaseSequence");
        var raw = ($el.val() || "").replace(/\D/g, "");
        if (raw.length === 0) return;
        while (raw.length < 6) raw = "0" + raw;
        if (raw.length > 6) raw = raw.substring(raw.length - 6);
        $el.val(raw);
    }

    function MaskCaseSequence(caseType) {
        var loc = jQuery("#drpCountyLetter").val();
        jQuery("#txtCaseSequence").mask("000000");
        jQuery("#txtCaseSequence").attr("placeholder", "000000");
        if ((caseType || "").toUpperCase() === "CF") {
            if (loc === "S" || loc === "V") {
                jQuery("#txtDefendantSuffix").mask("0000");
                jQuery("#caseFormat").text("000000-0000");
                if (!jQuery("#txtDefendantSuffix").val()) jQuery("#txtDefendantSuffix").attr("placeholder", "0000");
            } else {
                jQuery("#txtDefendantSuffix").mask("AA", { translation: { A: { pattern: /[A-Za-z]/ } } });
                jQuery("#caseFormat").text("000000-AA");
                if (!jQuery("#txtDefendantSuffix").val()) jQuery("#txtDefendantSuffix").attr("placeholder", "AA");
            }
        } else {
            jQuery("#caseFormat").text("000000");
        }
    }

    function GetCaseNumber() {
        var c = jQuery("#drpCountyLetter").val() || "";
        var y = jQuery("#txtCaseYear").val() || "";
        var t = (jQuery("#txtCaseType").val() || "").toUpperCase();
        var s = jQuery("#txtCaseSequence").val() || "";
        var d = jQuery("#txtDefendantSuffix").val() || "";
        return c + "-" + y + "-" + t + "-" + s + (d ? "-" + d : "");
    }
</script>
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/jQuery/jquery.mask.js" />
