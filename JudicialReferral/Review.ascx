<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Review.ascx.cs" Inherits="tjc.Modules.JudicialReferral.Review" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>

<div typeof="post" id="review-form">
    <asp:Panel ID="pnlJA" runat="server">
        <fieldset>
            <div class="row">
                <div class="form-group">
                    <div class="col-md-6">
                        <asp:Label runat="server" AssociatedControlID="drpJudge" Text="Judge" />
                        <asp:DropDownList ID="drpJudge" runat="server" CssClass="form-control">
                            <asp:ListItem Text="< Select Judge >" Value=""></asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div class="col-md-6">
                        <asp:Label runat="server" AssociatedControlID="txtCaseNumber" Text="Case Number" />
                        <asp:TextBox ID="txtCaseNumber" runat="server" MaxLength="25" CssClass="form-control"></asp:TextBox>
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="form-group">
                    <div class="col-md-6">
                        <asp:Label runat="server" AssociatedControlID="txtCaseParties" Text="Case Name" />
                        <asp:TextBox ID="txtCaseParties" runat="server" MaxLength="2000" CssClass="form-control" placeholder="Party One v. Party Two"></asp:TextBox>
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="txtCaseParties"
                            Display="Dynamic" SetFocusOnError="true" CssClass="label label-danger" ErrorMessage="Case Name is Required" />

                    </div>
                    <div class="col-md-6">
                        <asp:Label runat="server" AssociatedControlID="drpCaseType" Text="Select Case Type" />
                        <asp:DropDownList ID="drpCaseType" runat="server" CssClass="form-control">
                            <asp:ListItem Text="< Select Case Type >" Value=""></asp:ListItem>
                            <asp:ListItem Text="Appeal" Value="Appeal"></asp:ListItem>
                            <asp:ListItem Text="Circuit Civil" Value="Civil"></asp:ListItem>
                            <asp:ListItem Text="County Civil" Value="County Civil"></asp:ListItem>
                            <asp:ListItem Text="County Criminal" Value="County Criminal"></asp:ListItem>
                            <asp:ListItem Text="Family" Value="Family"></asp:ListItem>
                            <asp:ListItem Text="Felony" Value="Felony"></asp:ListItem>
                            <asp:ListItem Text="Jimmy Ryce" Value="Jimmy Ryce"></asp:ListItem>
                            <asp:ListItem Text="Probate/Guardianship" Value="Probate/Guardianship"></asp:ListItem>
                        </asp:DropDownList>
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="drpCaseType"
                            Display="Dynamic" SetFocusOnError="true" CssClass="label label-danger" ErrorMessage="Please Select the Case Type" />
                    </div>

                </div>
            </div>
            <div class="row">
                <div class="form-group">
                    <div class="col-md-6">
                        <asp:Label runat="server" AssociatedControlID="txtMotionTitle" Text="Motion Title" />
                        <asp:TextBox ID="txtMotionTitle" runat="server" MaxLength="50" CssClass="form-control"></asp:TextBox>
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="txtMotionTitle"
                            Display="Dynamic" SetFocusOnError="true" CssClass="label label-danger" ErrorMessage="Motion Title is Required" />
                    </div>

                    <div class="col-md-6">

                        <asp:Label runat="server" AssociatedControlID="txtMotionDate" Text="Motion Date" />
                        <asp:TextBox runat="server" CssClass="form-control datepicker" ID="txtMotionDate" />
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="txtMotionDate"
                            Display="Dynamic" SetFocusOnError="true" CssClass="label label-danger" ErrorMessage="Motion Date is Required" />
                    </div>
                </div>
            </div>
        </fieldset>
        <fieldset>
            <div class="attachment-container">
                <h5>
                    <asp:Literal runat="server" ID="ltAttachments">Attachment(s)</asp:Literal>
                </h5>
                <asp:Repeater ID="rptFiles" runat="server">
                    <HeaderTemplate>
                        <div class="attachment-list">
                            <ul>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <li><a href='<%# Eval("Path") %>'><%# Eval("FileName") %></a></li>
                    </ItemTemplate>
                    <FooterTemplate>
                        </ul></div>
                    </FooterTemplate>
                </asp:Repeater>
            </div>
            <div>
                <div class="form-check mb-2">
                    <asp:CheckBox ID="chkMotionVacate" runat="server" Text="<strong>3.850</strong> Motion to Vacate, Set Aside, or Correct Sentence: Court Counsel will assist with all 3.850 motions. If the Motion is not facially sufficient, a proposed order striking the motion will be provided to the judge. If the Motion is facially sufficient for legal review, the judicial assistant will be prompted to send an Acknowledgment of the Motion to the defendant, copying the State and Clerk. Unless the Court is able to dismiss all claims as legally deficient, the State will be ordered to respond within 60 days." TextAlign="Right" />
                </div>
                <div class="form-check mb-2">
                    <asp:CheckBox ID="chkMotionCorrect" runat="server" Text="<strong>3.800(b)</strong> Motion to Correct Sentencing Error: The Court must rule on this motion within 60 days or it is deemed denied. Unless directed otherwise by the court below, this motion shall be handled directly by the judge and court counsel need not take any action." TextAlign="Right" />
                </div>
                <div class="form-check mb-2">
                    <asp:CheckBox ID="chkMotionDirected" runat="server" Text="Unless directed by the court below, these motions shall be handled directly by the presiding judge unless the complexity of the issue warrants further assistance by Court Counsel:" TextAlign="Right" />
                    <asp:CheckBoxList ID="clsMotionList" runat="server" CssClass="motion-list" RepeatDirection="Vertical" RepeatLayout="UnorderedList">
                        <asp:ListItem Text="Motion to modify or reduce sentence" />
                        <asp:ListItem Text="Motion to modify probation" />
                        <asp:ListItem Text="Speedy trial matters" />
                        <asp:ListItem Text="Motions to appoint appellate counsel" />
                        <asp:ListItem Text="Motions to convert court costs and fines" />
                        <asp:ListItem Text="Pro se pleading by defendant with counsel" />
                        <asp:ListItem Text="Motion to dismiss counsel, or to self-represent" />
                    </asp:CheckBoxList>
                    <asp:CustomValidator Display="Dynamic" SetFocusOnError="true" CssClass="label label-danger" ID="valMotionDirected" runat="server" ErrorMessage="You must select at least one of the seven options"
                        ClientValidationFunction="DirectedMotionCheck"></asp:CustomValidator>

                </div>
                <div class="form-check">
                    <asp:CheckBox ID="chkMotionOther" runat="server" Text="All other motions: Court Counsel will assist with all other motions, as referred by the presiding judge." TextAlign="Right" />
                </div>
                <asp:CustomValidator Display="Dynamic" SetFocusOnError="true" CssClass="label label-danger" ID="valMotionCheck" runat="server" ErrorMessage="You Must Select at least one of the four options"
                    ClientValidationFunction="MotionCheck"></asp:CustomValidator>
            </div>
        </fieldset>
        <hr />
        <asp:HiddenField runat="server" ID="hdIsJa" Value="0" />
    </asp:Panel>
    <asp:Panel ID="pnlJudge" runat="server">
        <h4 class="mt-0">Judicial Response</h4>
        <fieldset>
            <div class="form-check mb-2">
                <asp:CheckBox ID="chkNo" runat="server" Text="I <strong>do not</strong> seek Court Counsel’s assistance in the above titled motion." TextAlign="Right" />
            </div>
            <div class="form-check">
                <asp:CheckBox ID="chkYes" runat="server" Text="I seek Court Counsel’s assistance in the above titled motion. See below:" TextAlign="Right" />
                <asp:CheckBoxList ID="clsResponse" runat="server" CssClass="motion-list" RepeatDirection="Vertical" RepeatLayout="UnorderedList">
                    <asp:ListItem Text="The State/Petitioner should be ordered to respond to the Motion." />
                    <asp:ListItem Text="The Motion should be granted." />
                    <asp:ListItem Text="The Motion should be denied." />
                    <asp:ListItem Text="Please have assigned staff attorney contact me to discuss the Motion." />
                    <asp:ListItem Text="Other" Value="other" />
                </asp:CheckBoxList>
                <asp:TextBox MaxLength="500" CssClass="form-control" ID="txtOther" runat="server"></asp:TextBox>
                <asp:CustomValidator Display="Dynamic" SetFocusOnError="true" CssClass="label label-danger" ID="valYes" runat="server" ErrorMessage="<br />You Must set at least one of the five options" ClientValidationFunction="ValidateYes"></asp:CustomValidator>
                <asp:CustomValidator Display="Dynamic" SetFocusOnError="true" CssClass="label label-danger" ID="valOther" runat="server" ErrorMessage="<br />Please enter a reponse for other" ClientValidationFunction="ValidateOther"></asp:CustomValidator>

            </div>

            <asp:CustomValidator Display="Dynamic" SetFocusOnError="true" CssClass="label label-danger" ID="valResponse" runat="server" ErrorMessage="You Must accept or reject Court Counsel's assistance, but not both"
                ClientValidationFunction="ValidateResponse"></asp:CustomValidator>

        </fieldset>
    </asp:Panel>
    <hr />
    <p>
        <asp:LinkButton ID="cmdSave" runat="server" CssClass="btn btn-primary mr-md" Text="Save" OnClick="cmdSave_Click" />
        <asp:LinkButton ID="cmdComplete" runat="server" CausesValidation="false" CssClass="btn btn-primary mr-md" Text="Order Completed?" OnClick="cmdComplete_Click" />&nbsp;

        <asp:HyperLink ID="cmdCancel" runat="server" CssClass="btn btn-secondary" Text="Cancel" />
    </p>

</div>
<script>
    $(document).ready(function () {
        $(".form-check input:checkbox").addClass("form-check-input");
        $(".form-check label").addClass("form-check-label");

        if ($("#<%=clsResponse.ClientID%>_4").is(':checked') === false) {
            $("#<%=txtOther.ClientID%>").hide();
        }
        $("#<%=clsResponse.ClientID%>_4").click(function () {
            if ($(this).is(':checked')) {
                $("#<%=txtOther.ClientID%>").show();

            } else {
                $("#<%=txtOther.ClientID%>").val('');
                $("#<%=txtOther.ClientID%>").hide();
            }
        });
    });
    var link = document.getElementById("<%=cmdSave.ClientID %>");
    document.addEventListener('click', function (e) {
        if (e.target.id === link.id) {
            if (document.getElementById("<%=cmdSave.ClientID %>").disabled)
                e.preventDefault();
        }
    });
    function DisableButton() {
        document.getElementById("<%=cmdSave.ClientID %>").disabled = true;
        document.getElementById("<%=cmdSave.ClientID %>").innerHTML = "Processing...";
        setTimeout(() => {
            document.getElementById("<%=cmdSave.ClientID %>").disabled = false;
            document.getElementById("<%=cmdSave.ClientID %>").innerHTML = "Save";
        }, "3000");
    }
    window.onbeforeunload = DisableButton;

    function ValidateResponse(sender, args) {
        args.IsValid = false;
        if ($('#<%=hdIsJa.ClientID%>').val() == "1") {
            args.IsValid = true;
            return;
        }
        var chkYes = $('#<%=chkYes.ClientID%>').is(':checked');
        var chkNo = $('#<%=chkNo.ClientID%>').is(':checked');
        if (chkYes & chkNo) {
            args.IsValid = false;
            alert("You Cannot choose both 'I do not seek' and 'I seek' options.");
            return;
        }
        if (chkYes | chkNo) {
            args.IsValid = true;
            return;
        }
    }
    function ValidateOther(sender, args) {
        args.IsValid = true;
        var chkYes = $('#<%=clsResponse.ClientID%>_4').is(':checked');
        var other = $('#<%=txtOther.ClientID%>').val();
        if (chkYes & other.length === 0) {
            args.IsValid = false;
            return;
        }
    }
    function ValidateYes(sender, args) {
        args.IsValid = true;
        var chkYes = $('#<%=chkYes.ClientID%>').is(':checked');
        var radioButtons = $('#<%=clsResponse.ClientID%>');
        if (chkYes) {
            var found = radioButtons.find('input:checked');
            if (found.length === 0) {
                args.IsValid = false;
                return;
            }
        }

    }
    function MotionCheck(sender, args) {
        args.IsValid = false;
        var chkMotionVacate = $('#<%=chkMotionVacate.ClientID%>').is(':checked');
        var chkMotionCorrect = $('#<%=chkMotionCorrect.ClientID%>').is(':checked');
        var chkMotionDirected = $('#<%=chkMotionDirected.ClientID%>').is(':checked');
        var chkMotionOther = $('#<%=chkMotionOther.ClientID%>').is(':checked');
        if (chkMotionVacate | chkMotionCorrect | chkMotionDirected | chkMotionOther) {
            args.IsValid = true;
            return;
        }
    }
    function DirectedMotionCheck(sender, args) {
        args.IsValid = true;
        var chkMotionDirected = $('#<%=chkMotionDirected.ClientID%>').is(':checked');
        var radioButtons = $('#<%=clsMotionList.ClientID%>');
        if (chkMotionDirected) {
            var found = radioButtons.find('input:checked');
            if (found.length === 0) {
                args.IsValid = false;
                return;
            }
        }

    }

</script>
