<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Review.ascx.cs" Inherits="tjc.Modules.JudicialReferral.Views.Review" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>

<div class="container-fluid mt-3" style="max-width:1000px">
    <asp:Panel ID="pnlJA" runat="server">
        <h3>Referral Details</h3>
        <div class="card mb-3">
            <div class="card-body">
                <div class="form-row">
                    <div class="form-group col-md-6">
                        <label for="<%=drpJudge.ClientID %>">Judge</label>
                        <asp:DropDownList ID="drpJudge" runat="server" CssClass="form-control">
                            <asp:ListItem Text="&lt; Select Judge &gt;" Value=""></asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div class="form-group col-md-6">
                        <label for="<%=txtCaseNumber.ClientID %>">Case Number</label>
                        <asp:TextBox ID="txtCaseNumber" runat="server" MaxLength="25" CssClass="form-control"></asp:TextBox>
                    </div>
                </div>
                <div class="form-row">
                    <div class="form-group col-md-12">
                        <label for="<%=txtCaseParties.ClientID %>">Case Name</label>
                        <asp:TextBox ID="txtCaseParties" runat="server" CssClass="form-control"></asp:TextBox>
                    </div>
                </div>
                <div class="form-row">
                    <div class="form-group col-md-8">
                        <label for="<%=txtMotionTitle.ClientID %>">Motion Title</label>
                        <asp:TextBox ID="txtMotionTitle" runat="server" CssClass="form-control"></asp:TextBox>
                    </div>
                    <div class="form-group col-md-4">
                        <label for="<%=txtMotionDate.ClientID %>">Motion Date</label>
                        <asp:TextBox ID="txtMotionDate" runat="server" CssClass="form-control" TextMode="SingleLine" type="date" />
                    </div>
                </div>

                <div class="mt-3">
                    <h5><asp:Literal ID="ltAttachments" runat="server" Text="Attachments"></asp:Literal></h5>
                    <asp:Repeater ID="rptFiles" runat="server">
                        <HeaderTemplate>
                            <ul class="list-group">
                        </HeaderTemplate>
                        <ItemTemplate>
                            <li class="list-group-item">
                                <a href='/portals/0/<%# Eval("Path") %>' target="_blank">
                                    <i class="fas fa-file"></i>&nbsp;<%# Eval("FileName") %>
                                </a>
                            </li>
                        </ItemTemplate>
                        <FooterTemplate>
                            </ul>
                        </FooterTemplate>
                    </asp:Repeater>
                </div>
                <asp:HiddenField runat="server" ID="hdIsJudge" Value="0" ClientIDMode="Static" />
            </div>
        </div>
    </asp:Panel>

    <asp:Panel ID="pnlJudge" runat="server" ClientIDMode="Static">
        <h3>Motion Type</h3>
        <div class="card mb-3">
            <div class="card-body">
                <div class="form-inline mb-3">
                    <asp:RadioButtonList ID="rblDivisions" runat="server" RepeatLayout="Flow" RepeatDirection="Horizontal" CssClass="division">
                        <asp:ListItem Text="&nbsp;Criminal&nbsp;" Value="0" />
                        <asp:ListItem Text="&nbsp;Civil&nbsp;" Value="1" />
                        <asp:ListItem Text="&nbsp;Family&nbsp;" Value="2" />
                        <asp:ListItem Text="&nbsp;Appeals&nbsp;" Value="3" />
                    </asp:RadioButtonList>
                </div>

                <div id="dvCriminal">
                    <ul class="list-unstyled">
                        <li class="mb-2">
                            <asp:RadioButton ID="chkStatusOrder" GroupName="criminal" runat="server" Text="&nbsp;Status Order from 2d DCA/Supreme Court" ClientIDMode="Static" />
                            <label class="ml-3">Date Filed:</label>
                            <asp:TextBox runat="server" ID="txtStatusOrderFiled" CssClass="form-control-sm d-inline-block" TextMode="SingleLine" type="date" ClientIDMode="Static" Style="width:auto" />
                            <asp:CustomValidator ID="valStatusOrderFiled" CssClass="text-danger" runat="server" ErrorMessage="Date Filed is Required"
                                ClientValidationFunction="ValidateStatusOrderFiled" Display="Dynamic"></asp:CustomValidator>
                        </li>
                        <li class="mb-2">
                            <asp:RadioButton ID="chkMotionVacate" GroupName="criminal" runat="server" Text="&nbsp;<strong>3.850 Motion</strong>" />
                        </li>
                        <li class="mb-2">
                            <asp:RadioButton ID="chkMotionCorrect" GroupName="criminal" runat="server" Text="&nbsp;<strong>3.800(b) Motion</strong> <small class='text-muted'>(rule in 60 days or deemed denied)</small>" ClientIDMode="Static" />
                            <label class="ml-3">Date Filed:</label>
                            <asp:TextBox runat="server" ID="txtMotionCorrectFiled" CssClass="form-control-sm d-inline-block" TextMode="SingleLine" type="date" ClientIDMode="Static" Style="width:auto" />
                            <asp:CustomValidator ID="valMotionCorrectFiled" CssClass="text-danger" runat="server" ErrorMessage="Date Filed is Required"
                                ClientValidationFunction="ValidateMotionCorrectFiled" Display="Dynamic"></asp:CustomValidator>
                        </li>
                        <li class="mb-2">
                            <asp:RadioButton ID="chkMotionDirected" GroupName="criminal" runat="server" Text="&nbsp;Handled directly by the judge:" ClientIDMode="Static" />
                            <asp:RadioButtonList ID="clsMotionList" runat="server" RepeatColumns="2" RepeatDirection="Vertical" RepeatLayout="Table" CssClass="ml-4">
                                <asp:ListItem Text="Motion to modify or reduce sentence" />
                                <asp:ListItem Text="Motion to modify probation" />
                                <asp:ListItem Text="Speedy trial matters" />
                                <asp:ListItem Text="Motions to appoint appellate counsel" />
                                <asp:ListItem Text="Motions to convert court costs and fines" />
                                <asp:ListItem Text="Pro se pleading by defendant with counsel" />
                                <asp:ListItem Text="Motion to dismiss counsel, or to self-represent" />
                            </asp:RadioButtonList>
                            <asp:CustomValidator ID="valMotionDirected" CssClass="text-danger" runat="server" ErrorMessage="You must select one of the seven options"
                                ClientValidationFunction="DirectedMotionCheck" Display="Dynamic"></asp:CustomValidator>
                        </li>
                        <li class="mb-2">
                            <asp:RadioButton ID="chkOtherPostconviction" GroupName="criminal" runat="server" Text="&nbsp;Other postconviction" ClientIDMode="Static" />
                            <asp:TextBox ID="txtPostconvictionCriminal" MaxLength="50" runat="server" CssClass="form-control-sm d-inline-block" ClientIDMode="Static" />
                            <asp:CustomValidator ID="valPostconvictionCriminal" CssClass="text-danger" runat="server" ErrorMessage="Enter a value for other postconviction"
                                ClientValidationFunction="ValidatePostconvictionCriminal" Display="Dynamic"></asp:CustomValidator>
                        </li>
                        <li class="mb-2">
                            <asp:RadioButton ID="chkPretrialCriminal" GroupName="criminal" runat="server" Text="&nbsp;Pretrial motion" ClientIDMode="Static" />
                            <asp:TextBox ID="txtPretrialCriminal" MaxLength="50" runat="server" CssClass="form-control-sm d-inline-block" ClientIDMode="Static" />
                            <asp:CustomValidator ID="valPretrialCriminal" CssClass="text-danger" runat="server" ErrorMessage="Enter a value for Pretrial motion"
                                ClientValidationFunction="ValidatePretrialCriminal" Display="Dynamic"></asp:CustomValidator>
                        </li>
                        <li class="mb-2">
                            <asp:CheckBox ID="chkResearchCriminal" runat="server" Text="&nbsp;Research/memo only" ClientIDMode="Static" />
                            <asp:TextBox ID="txtResearchCriminal" MaxLength="50" runat="server" CssClass="form-control-sm d-inline-block" ClientIDMode="Static" />
                            <asp:CustomValidator ID="valResearchCriminal" CssClass="text-danger" runat="server" ErrorMessage="Enter a value for Research/memo"
                                ClientValidationFunction="ValidateResearchCriminal" Display="Dynamic"></asp:CustomValidator>
                        </li>
                    </ul>
                    <asp:CustomValidator ID="valMotionCheckCriminal" CssClass="text-danger" runat="server" ErrorMessage="You must select at least one Criminal option"
                        ClientValidationFunction="MotionCheckCriminal" Display="Dynamic"></asp:CustomValidator>
                </div>

                <div id="dvCivil" style="display:none">
                    <ul class="list-unstyled">
                        <li><asp:RadioButton ID="chkDismissCivil" GroupName="civil" runat="server" Text="&nbsp;Motion to Dismiss" /></li>
                        <li><asp:RadioButton ID="chkSummaryJudgementCivil" GroupName="civil" runat="server" Text="&nbsp;Motion for Summary Judgment" /></li>
                        <li><asp:RadioButton ID="chkCompelDiscoveryCivil" GroupName="civil" runat="server" Text="&nbsp;Motion to Compel Discovery" /></li>
                        <li><asp:RadioButton ID="chkAttorneyFeesCivil" GroupName="civil" runat="server" Text="&nbsp;Motion for Attorney Fees" /></li>
                        <li>
                            <asp:RadioButton ID="chkPretrialCivil" GroupName="civil" runat="server" Text="&nbsp;Other pretrial or post trial motion" ClientIDMode="Static" />
                            <asp:TextBox ID="txtPretrialCivil" MaxLength="50" runat="server" CssClass="form-control-sm d-inline-block" ClientIDMode="Static" />
                            <asp:CustomValidator ID="valPretrialCivil" CssClass="text-danger" runat="server" ErrorMessage="Enter a value for other pretrial or post trial motion"
                                ClientValidationFunction="ValidatePretrialCivil" Display="Dynamic"></asp:CustomValidator>
                        </li>
                        <li>
                            <asp:CheckBox ID="chkResearchCivil" runat="server" Text="&nbsp;Research/memo only" ClientIDMode="Static" />
                            <asp:TextBox ID="txtResearchCivil" MaxLength="50" runat="server" CssClass="form-control-sm d-inline-block" ClientIDMode="Static" />
                            <asp:CustomValidator ID="valResearchCivil" CssClass="text-danger" runat="server" ErrorMessage="Enter a value for Research/memo"
                                ClientValidationFunction="ValidateResearchCivil" Display="Dynamic"></asp:CustomValidator>
                        </li>
                    </ul>
                    <asp:CustomValidator ID="valCivilCheck" CssClass="text-danger" runat="server" ErrorMessage="You must select at least one option"
                        ClientValidationFunction="MotionCheckCivil" Display="Dynamic"></asp:CustomValidator>
                </div>

                <div id="dvFamily" style="display:none">
                    <ul class="list-unstyled">
                        <li><asp:CheckBox ID="chkModifyTimeshareFamily" runat="server" Text="&nbsp;Supplemental Petition to Modify Timesharing" /></li>
                        <li><asp:CheckBox ID="chkModifySupportFamily" runat="server" Text="&nbsp;Supplemental Petition to Modify Child Support/Alimony" /></li>
                        <li><asp:CheckBox ID="chkCompelDiscoveryFamily" runat="server" Text="&nbsp;Motion to Compel Discovery" /></li>
                        <li><asp:CheckBox ID="chkAttorneyFeesFamily" runat="server" Text="&nbsp;Motion for Attorney Fees" /></li>
                        <li>
                            <asp:CheckBox ID="chkPretrialFamily" runat="server" Text="&nbsp;Other pretrial or post trial motion" ClientIDMode="Static" />
                            <asp:TextBox ID="txtPretrialFamily" MaxLength="50" runat="server" CssClass="form-control-sm d-inline-block" ClientIDMode="Static" />
                            <asp:CustomValidator ID="valPretrialFamily" CssClass="text-danger" runat="server" ErrorMessage="Enter a value for other pretrial or post trial motion"
                                ClientValidationFunction="ValidatePretrialFamily" Display="Dynamic"></asp:CustomValidator>
                        </li>
                        <li>
                            <asp:CheckBox ID="chkResearchFamily" runat="server" Text="&nbsp;Research - memo only" ClientIDMode="Static" />
                            <asp:TextBox ID="txtResearchFamily" MaxLength="50" runat="server" CssClass="form-control-sm d-inline-block" ClientIDMode="Static" />
                            <asp:CustomValidator ID="valResearchFamily" CssClass="text-danger" runat="server" ErrorMessage="Enter a value for Research/memo"
                                ClientValidationFunction="ValidateResearchFamily" Display="Dynamic"></asp:CustomValidator>
                        </li>
                    </ul>
                    <asp:CustomValidator ID="valCivilMotionCheck" CssClass="text-danger" runat="server" ErrorMessage="You must select at least one option"
                        ClientValidationFunction="MotionCheckFamily" Display="Dynamic"></asp:CustomValidator>
                </div>

                <div id="dvAppeals" style="display:none">
                    <div class="form-group">
                        <label for="txtAppeals">Type of Appeal</label>
                        <asp:TextBox ID="txtAppeals" runat="server" MaxLength="50" CssClass="form-control" ClientIDMode="Static" />
                        <asp:CustomValidator ID="valAppeals" CssClass="text-danger" runat="server" ErrorMessage="Enter a value for Type of Appeal"
                            ClientValidationFunction="ValidateAppeals" Display="Dynamic"></asp:CustomValidator>
                    </div>
                </div>
            </div>
        </div>

        <h3>Judicial Response</h3>
        <div class="card mb-3">
            <div class="card-body">
                <ul class="list-unstyled">
                    <li class="mb-2">
                        <asp:RadioButton ID="chkNo" runat="server" Text="&nbsp;I <strong>do not</strong> seek Court Counsel's assistance in the above titled motion." GroupName="response" />
                    </li>
                    <li class="mb-2">
                        <asp:RadioButton ID="chkYes" GroupName="response" runat="server" Text="&nbsp;I seek Court Counsel's assistance in the above titled motion. See below:" />
                        <asp:RadioButtonList ID="clsResponse" runat="server" RepeatDirection="Vertical" RepeatLayout="Table" CssClass="ml-4">
                            <asp:ListItem Text="The State/Petitioner should be ordered to respond to the Motion." />
                            <asp:ListItem Text="The Motion should be granted." />
                            <asp:ListItem Text="The Motion should be denied." />
                            <asp:ListItem Text="Please have assigned staff attorney contact me to discuss the Motion." />
                            <asp:ListItem Text="Other" Value="other" />
                        </asp:RadioButtonList>
                    </li>
                    <li class="mb-2">
                        <asp:TextBox MaxLength="500" ID="txtOther" runat="server" CssClass="form-control" placeholder="Other response..." />
                        <asp:CustomValidator ID="valYes" CssClass="text-danger" runat="server" ErrorMessage="You must set at least one of the five options" ClientValidationFunction="ValidateYes" Display="Dynamic"></asp:CustomValidator>
                        <asp:CustomValidator ID="valOther" CssClass="text-danger" runat="server" ErrorMessage="Please enter a response for Other" ClientValidationFunction="ValidateOther" Display="Dynamic"></asp:CustomValidator>
                    </li>
                    <li class="mb-2">
                        <asp:CustomValidator ID="valResponse" CssClass="text-danger" runat="server" ErrorMessage="You must accept or reject Court Counsel's assistance"
                            ClientValidationFunction="ValidateResponse" Display="Dynamic"></asp:CustomValidator>
                        <div class="form-group mt-2">
                            <label for="txtRequestedCompletionDate">Requested Completion Date (other than ASAP)</label>
                            <asp:TextBox runat="server" ID="txtRequestedCompletionDate" CssClass="form-control" TextMode="SingleLine" type="date" ClientIDMode="Static" Style="width:auto" />
                            <asp:CustomValidator ID="valRequestedCompletionDate" CssClass="text-danger" runat="server" ErrorMessage="Requested Completion Date Required if seeking Court Counsel Assistance" ClientValidationFunction="ValidateRequestedCompletionDate" Display="Dynamic"></asp:CustomValidator>
                        </div>
                    </li>
                </ul>
            </div>
        </div>
    </asp:Panel>

    <div class="mt-3">
        <asp:LinkButton ID="cmdSave" runat="server" CssClass="btn btn-primary" Text="Save" OnClick="cmdSave_Click" />
        <asp:LinkButton ID="cmdComplete" runat="server" CausesValidation="false" CssClass="btn btn-success" Text="Order Completed?" OnClick="cmdComplete_Click" />
        <asp:HyperLink ID="cmdCancel" runat="server" CssClass="btn btn-secondary" Text="Cancel" />
    </div>
</div>

<script type="text/javascript">
    var selectedDivision = "0";
    var isJudge = false;

    function PageInit() {
        jQuery(document).ready(function ($) {
            isJudge = $('#hdIsJudge').val() == "1";
            var checked = $('#<%=rblDivisions.ClientID %> input:checked').val();
            selectedDivision = checked || "0";
            SwitchDivisions(selectedDivision);

            $('#<%=rblDivisions.ClientID %> input').change(function () {
                selectedDivision = $(this).val();
                SwitchDivisions(selectedDivision);
            });

            var yesChecked = $('#<%=chkYes.ClientID%>').is(':checked');
            if (!yesChecked) {
                $('#<%=clsResponse.ClientID%> input').prop('checked', false).prop('disabled', true);
            }

            if (!$('#<%=clsResponse.ClientID%>_4').is(':checked')) {
                $('#<%=txtOther.ClientID%>').hide();
            }

            $('#<%=clsResponse.ClientID%> input').click(function () {
                if ($('#<%=clsResponse.ClientID%>_4').is(':checked')) {
                    $('#<%=txtOther.ClientID%>').show();
                } else {
                    $('#<%=txtOther.ClientID%>').val('').hide();
                }
            });

            $("input[type='radio'][name$='response']").click(function () {
                if ($('#<%=chkYes.ClientID%>').is(':checked')) {
                    $('#<%=clsResponse.ClientID%> input').prop('disabled', false);
                } else {
                    $('#<%=clsResponse.ClientID%> input').prop('checked', false).prop('disabled', true);
                }
            });
        });
    }
    PageInit();

    function SwitchDivisions(division) {
        var sections = { "0": "#dvCriminal", "1": "#dvCivil", "2": "#dvFamily", "3": "#dvAppeals" };
        for (var key in sections) {
            if (key === division) jQuery(sections[key]).show(); else jQuery(sections[key]).hide();
        }
    }

    function ValidateRequestedCompletionDate(sender, args) {
        args.IsValid = true;
        var date = jQuery('#txtRequestedCompletionDate').val();
        var chkYes = jQuery('#<%=chkYes.ClientID%>').is(':checked');
        if (chkYes && (!date || date.length === 0)) args.IsValid = false;
    }
    function ValidateResponse(sender, args) {
        args.IsValid = true;
        if (isJudge) {
            args.IsValid = false;
            if (jQuery('#<%=chkYes.ClientID%>').is(':checked') || jQuery('#<%=chkNo.ClientID%>').is(':checked')) {
                args.IsValid = true;
            }
        }
    }
    function ValidateOther(sender, args) {
        args.IsValid = true;
        var otherChecked = jQuery('#<%=clsResponse.ClientID%>_4').is(':checked');
        var other = jQuery('#<%=txtOther.ClientID%>').val();
        if (otherChecked && (!other || other.length === 0)) args.IsValid = false;
    }
    function ValidateYes(sender, args) {
        args.IsValid = true;
        var chkYes = jQuery('#<%=chkYes.ClientID%>').is(':checked');
        if (chkYes && isJudge) {
            var found = jQuery('#<%=clsResponse.ClientID%>').find('input:checked');
            if (found.length === 0) args.IsValid = false;
        }
    }
    function ValidateStatusOrderFiled(sender, args) {
        args.IsValid = true;
        if (selectedDivision == "0" && isJudge) {
            var date = jQuery('#txtStatusOrderFiled').val();
            if (jQuery('#chkStatusOrder').is(':checked') && (!date || date.length === 0)) args.IsValid = false;
        }
    }
    function ValidateMotionCorrectFiled(sender, args) {
        args.IsValid = true;
        if (selectedDivision == "0" && isJudge) {
            var date = jQuery('#txtMotionCorrectFiled').val();
            if (jQuery('#chkMotionCorrect').is(':checked') && (!date || date.length === 0)) args.IsValid = false;
        }
    }
    function ValidatePostconvictionCriminal(sender, args) {
        args.IsValid = true;
        if (selectedDivision == "0" && isJudge) {
            var text = jQuery('#txtPostconvictionCriminal').val();
            if (jQuery('#chkOtherPostconviction').is(':checked') && (!text || text.length === 0)) args.IsValid = false;
        }
    }
    function ValidatePretrialCriminal(sender, args) {
        args.IsValid = true;
        if (selectedDivision == "0" && isJudge) {
            var text = jQuery('#txtPretrialCriminal').val();
            if (jQuery('#chkPretrialCriminal').is(':checked') && (!text || text.length === 0)) args.IsValid = false;
        }
    }
    function ValidateResearchCriminal(sender, args) {
        args.IsValid = true;
        if (selectedDivision == "0" && isJudge) {
            var text = jQuery('#txtResearchCriminal').val();
            if (jQuery('#chkResearchCriminal').is(':checked') && (!text || text.length === 0)) args.IsValid = false;
        }
    }
    function DirectedMotionCheck(sender, args) {
        args.IsValid = true;
        if (selectedDivision == "0" && isJudge) {
            if (jQuery('#chkMotionDirected').is(':checked')) {
                var found = jQuery('#<%=clsMotionList.ClientID%>').find('input:checked');
                if (found.length === 0) args.IsValid = false;
            }
        }
    }
    function MotionCheckCriminal(sender, args) {
        args.IsValid = true;
        if (selectedDivision == "0" && isJudge) {
            args.IsValid = (
                jQuery('#chkStatusOrder').is(':checked') ||
                jQuery('#<%=chkMotionVacate.ClientID%>').is(':checked') ||
                jQuery('#chkMotionCorrect').is(':checked') ||
                jQuery('#chkMotionDirected').is(':checked') ||
                jQuery('#chkOtherPostconviction').is(':checked') ||
                jQuery('#chkPretrialCriminal').is(':checked') ||
                jQuery('#chkResearchCriminal').is(':checked')
            );
        }
    }
    function ValidatePretrialCivil(sender, args) {
        args.IsValid = true;
        if (selectedDivision == "1" && isJudge) {
            var text = jQuery('#txtPretrialCivil').val();
            if (jQuery('#chkPretrialCivil').is(':checked') && (!text || text.length === 0)) args.IsValid = false;
        }
    }
    function ValidateResearchCivil(sender, args) {
        args.IsValid = true;
        if (selectedDivision == "1" && isJudge) {
            var text = jQuery('#txtResearchCivil').val();
            if (jQuery('#chkResearchCivil').is(':checked') && (!text || text.length === 0)) args.IsValid = false;
        }
    }
    function MotionCheckCivil(sender, args) {
        args.IsValid = true;
        if (selectedDivision == "1" && isJudge) {
            args.IsValid = (
                jQuery('#<%=chkDismissCivil.ClientID%>').is(':checked') ||
                jQuery('#<%=chkSummaryJudgementCivil.ClientID%>').is(':checked') ||
                jQuery('#<%=chkCompelDiscoveryCivil.ClientID%>').is(':checked') ||
                jQuery('#<%=chkAttorneyFeesCivil.ClientID%>').is(':checked') ||
                jQuery('#chkPretrialCivil').is(':checked') ||
                jQuery('#chkResearchCivil').is(':checked')
            );
        }
    }
    function ValidatePretrialFamily(sender, args) {
        args.IsValid = true;
        if (selectedDivision == "2" && isJudge) {
            var text = jQuery('#txtPretrialFamily').val();
            if (jQuery('#chkPretrialFamily').is(':checked') && (!text || text.length === 0)) args.IsValid = false;
        }
    }
    function ValidateResearchFamily(sender, args) {
        args.IsValid = true;
        if (selectedDivision == "2" && isJudge) {
            var text = jQuery('#txtResearchFamily').val();
            if (jQuery('#chkResearchFamily').is(':checked') && (!text || text.length === 0)) args.IsValid = false;
        }
    }
    function MotionCheckFamily(sender, args) {
        args.IsValid = true;
        if (selectedDivision == "2" && isJudge) {
            args.IsValid = (
                jQuery('#<%=chkModifyTimeshareFamily.ClientID%>').is(':checked') ||
                jQuery('#<%=chkModifySupportFamily.ClientID%>').is(':checked') ||
                jQuery('#<%=chkCompelDiscoveryFamily.ClientID%>').is(':checked') ||
                jQuery('#<%=chkAttorneyFeesFamily.ClientID%>').is(':checked') ||
                jQuery('#chkPretrialFamily').is(':checked') ||
                jQuery('#chkResearchFamily').is(':checked')
            );
        }
    }
    function ValidateAppeals(sender, args) {
        args.IsValid = true;
        if (selectedDivision == "3" && isJudge) {
            var text = jQuery('#txtAppeals').val();
            if (!text || text.length === 0) args.IsValid = false;
        }
    }
</script>
