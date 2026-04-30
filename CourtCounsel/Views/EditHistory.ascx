<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EditHistory.ascx.cs" Inherits="tjc.Modules.CourtCounsel.Views.EditHistory" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>

<nav class="navbar navbar-expand-lg navbar-dark bg-dark mb-md rounded">
    <button class="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target="#navbarNav">
        <span class="navbar-toggler-icon"></span>
    </button>
    <div class="collapse navbar-collapse" id="navbarNav">
        <ul class="navbar-nav">
            <li class="nav-item"><a class="nav-link" href="<%=SearchUrl %>"><i class="fas fa-search"></i>&nbsp;Search</a></li>
            <li class="nav-item"><a class="nav-link active" href="<%=DataEntryUrl %>"><i class="fas fa-pencil-alt"></i>&nbsp;Data Entry</a></li>
            <li class="nav-item"><a class="nav-link" href="<%=ReportsUrl %>"><i class="fas fa-chart-bar"></i>&nbsp;Reports</a></li>
            <li class="nav-item"><a class="nav-link" href="<%=DataSheetUrl %>"><i class="fas fa-table"></i>&nbsp;Data Sheet</a></li>
            <li class="nav-item" id="liAdmin" runat="server" visible="false"><a class="nav-link" href="<%=AdminUrl %>"><i class="fa fa-tools"></i>&nbsp;Admin</a></li>
        </ul>
    </div>
</nav>

<div class="mt-3">
    <asp:ValidationSummary ID="valSummary" runat="server" CssClass="alert alert-danger" DisplayMode="BulletList" />
    <asp:HiddenField ID="hdLogId" runat="server" Value="0" />
    <asp:Literal ID="ltSaveMessage" runat="server" />

    <!-- Row 1: Action Date, Case Number, Case Name -->
    <div class="row mb-2">
        <div class="col-md-4 ">
            <label for="<%=txtDateReceived.ClientID %>">Action Date <span class="text-danger">*</span></label>
            <asp:TextBox ID="txtDateReceived" runat="server" CssClass="form-control" TextMode="SingleLine" type="date" />
            <asp:RequiredFieldValidator ID="rfvDateReceived" runat="server" ControlToValidate="txtDateReceived"
                ErrorMessage="Action Date is required." Display="None" />
            <div class="form-text">(future dates will be set to inactive)</div>
        </div>
        <div class="col-md-4">
            <label for="drpCountyLetter">Case Number <span class="text-danger">*</span></label>
            <div class="input-group">
                <asp:DropDownList ID="drpCountyLetter" runat="server" title="County" CssClass="form-control county-letter" ClientIDMode="Static">
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
            </div>
            <small class="form-text" title="County-Year-Case Type-Case Sequence">(Format: C-YYYY-CT-<span id="caseFormat">000000</span>)</small>

            <asp:RequiredFieldValidator runat="server" ControlToValidate="drpCountyLetter" InitialValue="" CssClass="text-danger" Display="Dynamic" ErrorMessage="County is required." />
            <asp:RequiredFieldValidator runat="server" ControlToValidate="txtCaseYear" CssClass="text-danger" Display="Dynamic" ErrorMessage="Case Year is required." />
            <asp:RequiredFieldValidator runat="server" ControlToValidate="txtCaseType" CssClass="text-danger" Display="Dynamic" ErrorMessage="Case Type is required." />
            <asp:RequiredFieldValidator runat="server" ControlToValidate="txtCaseSequence" CssClass="text-danger" Display="Dynamic" ErrorMessage="Case Sequence is required." />
        </div>
        <div class="col-md-4">
            <label for="<%=txtCaseName.ClientID %>">Case Name <span class="text-danger">*</span></label>
            <asp:TextBox ID="txtCaseName" runat="server" CssClass="form-control" MaxLength="100" />
            <asp:RequiredFieldValidator ID="rfvCaseName" runat="server" ControlToValidate="txtCaseName"
                ErrorMessage="Case Name is required." Display="None" />
        </div>
    </div>

    <!-- Row 2: Case Type, Requested By, Responsible/Attorney -->
    <div class="row mb-2">
        <div class="col-md-4">
            <label for="<%=drpCaseType.ClientID %>">Case Type <span class="text-danger">*</span></label>
            <asp:DropDownList ID="drpCaseType" runat="server" CssClass="form-control" />
            <asp:RequiredFieldValidator ID="rfvCaseType" runat="server" ControlToValidate="drpCaseType"
                InitialValue="" ErrorMessage="Case Type is required." Display="None" />
        </div>
        <div class="col-md-4">
            <label for="<%=drpRequestor.ClientID %>">Requested By <span class="text-danger">*</span></label>
            <asp:DropDownList ID="drpRequestor" runat="server" CssClass="form-control" />
            <asp:RequiredFieldValidator ID="rfvRequestor" runat="server" ControlToValidate="drpRequestor"
                InitialValue="" ErrorMessage="Requested By is required." Display="None" />
        </div>
        <div class="col-md-4">
            <label for="<%=drpAttorney.ClientID %>">Responsible / Attorney <span class="text-danger">*</span></label>
            <asp:DropDownList ID="drpAttorney" runat="server" CssClass="form-control" />
            <asp:RequiredFieldValidator ID="rfvAttorney" runat="server" ControlToValidate="drpAttorney"
                InitialValue="" ErrorMessage="Responsible / Attorney is required." Display="None" />
        </div>
    </div>

    <!-- Row 3: Motion Filed, County, Action Taken -->
    <div class="row mb-2">
        <div class="col-md-4">
            <label for="<%=txtMotionFiled.ClientID %>">Motion Filed <span class="text-danger">*</span></label>
            <asp:TextBox ID="txtMotionFiled" runat="server" CssClass="form-control" TextMode="SingleLine" type="date" />
            <asp:RequiredFieldValidator ID="valMotionDate" runat="server" ControlToValidate="txtMotionFiled"
                Display="Dynamic" CssClass="text-danger" ErrorMessage="Date Motion Filed Required" />
        </div>
        <div class="col-md-4">
            <label for="<%=drpCounty.ClientID %>">County <span class="text-danger">*</span></label>
            <asp:DropDownList ID="drpCounty" runat="server" CssClass="form-control" />
            <asp:RequiredFieldValidator ID="valCounty" runat="server" ControlToValidate="drpCounty" InitialValue=""
                Display="Dynamic" CssClass="text-danger" ErrorMessage="Please Select County" />
        </div>
        <div class="col-md-4">
            <label for="<%=drpAction.ClientID %>">Action Taken</label>
            <asp:DropDownList ID="drpAction" runat="server" CssClass="form-control" />
        </div>
    </div>

    <!-- Row 4: Date Completed, Time Spent, Status -->
    <div class="row mb-2">
        <div class="col-md-4">
            <label for="<%=txtDateCompleted.ClientID %>">Date Completed</label>
            <asp:TextBox ID="txtDateCompleted" runat="server" CssClass="form-control" TextMode="SingleLine" type="date" />
        </div>
        <div class="col-md-4">
            <label for="<%=drpTimeSpan.ClientID %>">Time Spent</label>
            <asp:DropDownList ID="drpTimeSpan" runat="server" CssClass="form-control" />
        </div>
        <div class="col-md-4">
            <label for="<%=drpStatus.ClientID %>">Status</label>
            <asp:DropDownList ID="drpStatus" runat="server" CssClass="form-control">
                <asp:ListItem Text="< Select Status >" Value="" />
                <asp:ListItem Text="Admin Task Completed" Value="Admin Task Completed" />
                <asp:ListItem Text="Admin Review Needed" Value="Admin Review Needed" />
                <asp:ListItem Text="Amended Filed" Value="Amended Filed" />
                <asp:ListItem Text="Amended Motion Due" Value="Amended Motion Due" />
                <asp:ListItem Text="Assigned" Value="Assigned" />
                <asp:ListItem Text="Completed" Value="Completed" />
                <asp:ListItem Text="EOT Filed" Value="EOT Filed" />
                <asp:ListItem Text="EOT Granted" Value="EOT Granted" />
                <asp:ListItem Text="Evidentiary Hearing Granted" Value="Evidentiary Hearing Granted" />
                <asp:ListItem Text="Evidentiary Hearing Scheduled" Value="Evidentiary Hearing Scheduled" />
                <asp:ListItem Text="Final Order Due" Value="Final Order Due" />
                <asp:ListItem Text="Follow up needed" Value="Follow up needed" />
                <asp:ListItem Text="Mandamus Petition Filed w/ 2nd" Value="Mandamus Petition Filed w/ 2nd" />
                <asp:ListItem Text="Motion to Hear and Rule filed" Value="Motion to Hear and Rule filed" />
                <asp:ListItem Text="Motion Stricken With Leave to Amend" Value="Motion Stricken With Leave to Amend" />
                <asp:ListItem Text="Motion Under Review" Value="Motion Under Review" />
                <asp:ListItem Text="NOI I filed" Value="NOI I filed" />
                <asp:ListItem Text="NOI II filed" Value="NOI II filed" />
                <asp:ListItem Text="NOI III filed" Value="NOI III filed" />
                <asp:ListItem Text="Non-Final Order Entered" Value="Non-Final Order Entered" />
                <asp:ListItem Text="Order to Show Cause" Value="Order to Show Cause" />
                <asp:ListItem Text="Ordered to Respond" Value="Ordered to Respond" />
                <asp:ListItem Text="Post Conviction Counsel Appointed" Value="Post Conviction Counsel Appointed" />
                <asp:ListItem Text="Proposed Order Submitted" Value="Proposed Order Submitted" />
                <asp:ListItem Text="Response Due" Value="Response Due" />
                <asp:ListItem Text="Response Filed" Value="Response Filed" />
                <asp:ListItem Text="Other" Value="Other" />
                <asp:ListItem Text="Appeals" Value="[[group]]Appeals" />
                <asp:ListItem Text="Fee Due" Value="Fee Due" />
                <asp:ListItem Text="Fee Order Issued" Value="Fee Order Issued" />
                <asp:ListItem Text="Fee Paid" Value="Fee Paid" />
                <asp:ListItem Text="Show Cause Order" Value="Show Cause Order" />
                <asp:ListItem Text="Initial Brief Filed" Value="Initial Brief Filed" />
                <asp:ListItem Text="Initial Brief Due" Value="Initial Brief Due" />
                <asp:ListItem Text="Answer Brief Due" Value="Answer Brief Due" />
                <asp:ListItem Text="Answer Brief Filed" Value="Answer Brief Filed" />
                <asp:ListItem Text="Reply Brief (Optional)" Value="Reply Brief (Optional)" />
                <asp:ListItem Text="Ready for Disposition" Value="Ready for Disposition" />
                <asp:ListItem Text="Commissioners Reports (Belated Appeal)" Value="[[group]]Commissioners Reports (Belated Appeal)" />
                <asp:ListItem Text="Evidentiary Hearing Scheduled " Value="Evidentiary Hearing Scheduled " />
                <asp:ListItem Text="Transcripts Ordered" Value="Transcripts Ordered" />
                <asp:ListItem Text="Transcripts Received &amp; Filed" Value="Transcripts Received &amp; Filed" />
                <asp:ListItem Text="Final Report Filed" Value="Final Report Filed" />
            </asp:DropDownList>
        </div>
    </div>

    <!-- Row 5: Comments -->
    <div class="row mb-2">
        <div class="col-12 ">
            <label for="<%=txtComments.ClientID %>">Comments</label>
            <asp:TextBox ID="txtComments" runat="server" CssClass="form-control" TextMode="MultiLine" MaxLength="8000" Rows="4" />
        </div>
    </div>

    <!-- Row 6: Future Action Date (edit mode only) -->
    <asp:Panel ID="pnlFutureAction" runat="server" Visible="false">
        <div class="row mb-2">
            <div class="col-md-4">
                <label for="<%=txtFutureAction.ClientID %>">Future Action Date</label>
                <asp:TextBox ID="txtFutureAction" runat="server" CssClass="form-control" TextMode="SingleLine" type="date" />
            </div>
        </div>
    </asp:Panel>

    <!-- Buttons -->
    <div class="row">
        <div class="col">
            <asp:Button ID="cmdSave" runat="server" CssClass="btn btn-primary" Text="Save"
                OnClick="cmdSave_Click" UseSubmitBehavior="false"
                OnClientClick="if (typeof Page_ClientValidate === 'function' &amp;&amp; !Page_ClientValidate('')) { return false; } this.disabled = true; this.value = 'Saving…';" />
            <asp:Button ID="cmdCancel" runat="server" CssClass="btn btn-default" Text="Return to List" OnClick="cmdCancel_Click" CausesValidation="false" />
            <asp:Button ID="cmdDelete" runat="server" CssClass="btn btn-danger ms-3" Text="Delete" OnClick="cmdDelete_Click"
                CausesValidation="false" Visible="false" OnClientClick="return confirm('Are you sure you want to delete this record?');" />
        </div>
    </div>
</div>

<script type="text/javascript">
    function InitializeOptGroups(selectId) {
        var select = document.getElementById(selectId);
        if (!select) return;

        var options = Array.from(select.options);
        var newSelect = document.createElement('select');
        newSelect.id = select.id;
        newSelect.name = select.name;
        newSelect.className = select.className;
        newSelect.style.cssText = select.style.cssText;

        var currentGroup = null;
        options.forEach(function (opt) {
            if (opt.value === '<') {
                currentGroup = document.createElement('optgroup');
                currentGroup.label = 'Active';
                newSelect.appendChild(currentGroup);
            } else if (opt.value === '>') {
                currentGroup = document.createElement('optgroup');
                currentGroup.label = 'Inactive';
                newSelect.appendChild(currentGroup);
            } else if (opt.value && opt.value.indexOf('[[group]]') === 0) {
                currentGroup = document.createElement('optgroup');
                currentGroup.label = opt.value.substring('[[group]]'.length);
                newSelect.appendChild(currentGroup);
            } else {
                var newOpt = opt.cloneNode(true);
                if (currentGroup) {
                    currentGroup.appendChild(newOpt);
                } else {
                    newSelect.appendChild(newOpt);
                }
            }
        });
        select.parentNode.replaceChild(newSelect, select);
    }

    jQuery(document).ready(function ($) {
        InitializeOptGroups('<%= drpRequestor.ClientID %>');
        InitializeOptGroups('<%= drpAttorney.ClientID %>');
        InitializeOptGroups('<%= drpTimeSpan.ClientID %>');
        InitializeOptGroups('<%= drpStatus.ClientID %>');

        InitCaseNumberWidget();
    });

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
</script>
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/jQuery/jquery.mask.js" />
