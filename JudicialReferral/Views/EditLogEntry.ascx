<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EditLogEntry.ascx.cs" Inherits="tjc.Modules.JudicialReferral.Views.EditLogEntry" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>

<div class="container-fluid form-narrow">
    <h3>Copy to Court Counsel Log</h3>
    <asp:ValidationSummary ID="valSummary" runat="server" CssClass="alert alert-danger" DisplayMode="BulletList" />
            <div class="row mb-3">
                <div class="col-md-6">
                    <label for="<%=txtReceived.ClientID %>">Action Date <span class="text-danger">*</span></label>
                    <asp:TextBox ID="txtReceived" runat="server" CssClass="form-control" TextMode="SingleLine" type="date" />
                    <asp:RequiredFieldValidator ID="valReceived" runat="server" Display="None" ControlToValidate="txtReceived"
                        ErrorMessage="Date Received Required"></asp:RequiredFieldValidator>
                    <small class="form-text text-muted">(future dates will be set to inactive)</small>
                </div>
                <div class="col-md-6">
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
                    <small class="form-text text-muted" title="County-Year-Case Type-Case Sequence">(Format: C-YYYY-CT-<span id="caseFormat">000000</span>)</small>

                </div>
            </div>

            <div class="row mb-3">
                <div class="col-md-6">
                    <label for="<%=txtCaseName.ClientID %>">Case Name <span class="text-danger">*</span></label>
                    <asp:TextBox ID="txtCaseName" runat="server" CssClass="form-control" />
                    <asp:RequiredFieldValidator ID="valCaseName" runat="server" Display="None" ControlToValidate="txtCaseName"
                        ErrorMessage="Case Name Required"></asp:RequiredFieldValidator>
                </div>
                <div class="col-md-6">
                    <label for="<%=drpCaseType.ClientID %>">Case Type <span class="text-danger">*</span></label>
                    <asp:DropDownList ID="drpCaseType" runat="server" CssClass="form-control">
                        <asp:ListItem Value="" Text="&lt; Select Case Type &gt;" />
                    </asp:DropDownList>
                    <asp:RequiredFieldValidator ID="valCaseType" runat="server" Display="None" ControlToValidate="drpCaseType"
                        InitialValue="" ErrorMessage="Please Select a Case Type"></asp:RequiredFieldValidator>
                </div>
            </div>

            <div class="row mb-3">
                <div class="col-md-6">
                    <label for="<%=txtJudge.ClientID %>">Referred by Judge</label>
                    <asp:TextBox ID="txtJudge" ReadOnly="true" Enabled="false" runat="server" CssClass="form-control" />
                </div>
                <div class="col-md-6">
                    <label for="<%=drpRequestor.ClientID %>">Requested By <span class="text-danger">*</span></label>
                    <asp:DropDownList ID="drpRequestor" runat="server" CssClass="form-control">
                        <asp:ListItem Value="" Text="&lt; Select Requested By &gt;" />
                    </asp:DropDownList>
                    <asp:RequiredFieldValidator ID="valRequestedBy" runat="server" Display="None"
                        ControlToValidate="drpRequestor" InitialValue="" ErrorMessage="Please Select Requested By"></asp:RequiredFieldValidator>
                </div>
            </div>

            <div class="row mb-3">
                <div class="col-md-6">
                    <label for="<%=drpAttorney.ClientID %>">Responsible <span class="text-danger">*</span></label>
                    <asp:DropDownList ID="drpAttorney" runat="server" CssClass="form-control">
                        <asp:ListItem Value="" Text="&lt; Select Responsible &gt;" />
                    </asp:DropDownList>
                    <asp:RequiredFieldValidator ID="valResponsible" runat="server" Display="None"
                        ControlToValidate="drpAttorney" InitialValue="" ErrorMessage="Please Select Responsible"></asp:RequiredFieldValidator>
                </div>
                <div class="col-md-6">
                    <label for="<%=txtMotionFiled.ClientID %>">Motion Filed <span class="text-danger">*</span></label>
                    <asp:TextBox ID="txtMotionFiled" runat="server" CssClass="form-control" TextMode="SingleLine" type="date" />
                    <asp:RequiredFieldValidator ID="valMotionDate" runat="server" Display="None"
                        ControlToValidate="txtMotionFiled" ErrorMessage="Date Motion Filed Required"></asp:RequiredFieldValidator>
                </div>
            </div>

            <div class="row mb-3">
                <div class="col-md-4">
                    <label for="<%=drpCounty.ClientID %>">County <span class="text-danger">*</span></label>
                    <asp:DropDownList ID="drpCounty" runat="server" CssClass="form-control">
                        <asp:ListItem Value="" Text="&lt; Select County &gt;" />
                    </asp:DropDownList>
                    <asp:RequiredFieldValidator ID="valCounty" runat="server" Display="None"
                        ControlToValidate="drpCounty" InitialValue="" ErrorMessage="Please Select County"></asp:RequiredFieldValidator>
                </div>
                <div class="col-md-4">
                    <label for="<%=drpAction.ClientID %>">Action Taken</label>
                    <asp:DropDownList ID="drpAction" runat="server" CssClass="form-control">
                        <asp:ListItem Value="" Text="&lt; Select Action Taken &gt;" />
                    </asp:DropDownList>
                </div>
                <div class="col-md-4">
                    <label for="<%=txtDateCompleted.ClientID %>">Date Completed</label>
                    <asp:TextBox ID="txtDateCompleted" runat="server" CssClass="form-control" TextMode="SingleLine" type="date" />
                </div>
            </div>

            <div class="row mb-3">
                <div class="col-md-6">
                    <label for="<%=drpTimeSpan.ClientID %>">Time Spent</label>
                    <asp:DropDownList ID="drpTimeSpan" runat="server" CssClass="form-control">
                        <asp:ListItem Value="" Text="&lt; Select Time Spent &gt;" />
                    </asp:DropDownList>
                </div>
                <div class="col-md-6">
                    <label for="<%=drpStatus.ClientID %>">Status</label>
                    <asp:DropDownList ID="drpStatus" runat="server" CssClass="form-control">
                        <asp:ListItem Value="" Text="&lt; Select Status &gt;" />
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

            <div class="mb-3">
                <h5>Attachments</h5>
                <asp:Repeater ID="rptFiles" runat="server">
                    <HeaderTemplate><ul class="list-group"></HeaderTemplate>
                    <ItemTemplate>
                        <li class="list-group-item">
                            <a href='/portals/0/<%# Eval("Path") %>' target="_blank">
                                <i class="fas fa-file"></i>&nbsp;<%# Eval("FileName") %>
                            </a>
                        </li>
                    </ItemTemplate>
                    <FooterTemplate></ul></FooterTemplate>
                </asp:Repeater>
            </div>

            <div class="row">
                <div class="col-md-12">
                    <label for="<%=txtComments.ClientID %>">Comments</label>
                    <asp:TextBox ID="txtComments" runat="server" MaxLength="750" CssClass="form-control"
                        TextMode="MultiLine" Rows="5" />
                </div>
            </div>
    <hr />
            <div class="row">
                <div class="col">
                    <asp:LinkButton ID="cmdUpdate" runat="server" Text="Copy" CssClass="btn btn-primary" OnClick="cmdUpdate_Click" />
                    <asp:HyperLink ID="cmdCancel" runat="server" CssClass="btn btn-secondary" Text="Cancel" />
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
            if (opt.value && opt.value.indexOf('[[group]]') === 0) {
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
    (function ($) {
        $(document).ready(function () {
            InitializeOptGroups('<%= drpStatus.ClientID %>');
            $(".upperCase").on("input", function () {
                $(this).val($(this).val().toUpperCase());
            });
            if ($("#txtCaseSequence").val() === "") {
                $("#txtCaseSequence").mask("000000");
            }
            if ($("#txtCaseType").val() !== "CF") {
                $("#txtDefendantSuffix").hide();
            } else {
                MaskCaseSequence("CF");
            }
            $("#txtCaseType").on("input", function () {
                var ct = $(this).val();
                if (ct.toUpperCase() === "CF") {
                    $("#txtDefendantSuffix").show();
                } else {
                    $("#txtDefendantSuffix").val("").hide();
                }
                MaskCaseSequence(ct);
            });
            $("#drpCountyLetter").on("change", function () {
                MaskCaseSequence($("#txtCaseType").val());
            });
        });
    }(jQuery));

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
