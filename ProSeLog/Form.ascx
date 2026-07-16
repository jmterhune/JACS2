<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Form.ascx.cs" Inherits="tjc.Modules.ProSeLog.Form" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<asp:HyperLink ID="lnkManage" Visible="false" CssClass="btn btn-danger mb-3" runat="server">Manage Lists</asp:HyperLink>
<div class="tabs">
    <ul class="nav nav-tabs">
        <li class="nav-item">
            <a class="nav-link" href="<%=LogListUrl %>"><i class="fas fa-search"></i>&nbsp;Search</a>
        </li>
        <li class="nav-item active">
            <a class="nav-link" href="#formEntry" data-toggle="tab">Data Entry</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=StatsUrl %>">Monthly Stats</a>
        </li>
    </ul>
    <div class="tab-content">
        <div id="form" class="tab-pane active">
            <asp:Literal ID="ltMessage" runat="server" />
            <div class="row form-group">
                <div class="col-auto">
                    <asp:Label runat="server" AssociatedControlID="drpMonths" Text="Month / Year" />
                    <div class="input-group">
                        <asp:DropDownList ID="drpMonths" runat="server" CssClass="form-control" />
                        <asp:DropDownList ID="drpYear" runat="server" CssClass="form-control"  style="min-width:70px"/>
                    </div>
                </div>
                <div class="col-auto">
                    <asp:Label runat="server" AssociatedControlID="txtPetitioner" Text="Petitioner" />
                    <asp:TextBox ID="txtPetitioner" runat="server" MaxLength="50" CssClass="form-control" />
                </div>
                <div class="col-auto">
                    <asp:Label runat="server" AssociatedControlID="txtRespondent" Text="Respondent" />
                    <asp:TextBox ID="txtRespondent" runat="server" MaxLength="50" CssClass="form-control" />
                </div>
                <div class="col-3">
                    <asp:Label runat="server" AssociatedControlID="txtCaseName" Text="Case Name<em>*</em>" ToolTip="required" />
                    <asp:TextBox ID="txtCaseName" runat="server" MaxLength="50" CssClass="form-control" placeholder="Party One v. Party Two" ClientIDMode="Static"></asp:TextBox>
                </div>
            </div>
            <div class="row form-group">
                <div class="col-5">
                    <asp:Label runat="server" AssociatedControlID="drpCountyLetter" Text="Case Number<em>*</em>" ToolTip="required" />
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
                        <asp:TextBox ID="txtCaseSequence" title="Case Sequence" runat="server" MaxLength="25" CssClass="form-control upperCase case-sequence" placeholder="000000" ClientIDMode="Static"></asp:TextBox>
                        <asp:TextBox ID="txtDefendantSuffix" title="Defendant Suffix" runat="server" MaxLength="10" CssClass="form-control upperCase" ClientIDMode="Static"></asp:TextBox>
                        <div class="input-group-append">
                            <small class="input-group-text form-control" title="County-Year-Case Type-Case Sequence">(Format: C-YYYY-CT-<span id="caseFormat">000000</span>)</small>
                        </div>
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="drpCountyLetter"
                            Display="Dynamic" SetFocusOnError="true" CssClass="label label-danger" ErrorMessage="County is Required" />
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="txtCaseYear"
                            Display="Dynamic" SetFocusOnError="true" CssClass="label label-danger" ErrorMessage="Case Year is Required" />
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="txtCaseType"
                            Display="Dynamic" SetFocusOnError="true" CssClass="label label-danger" ErrorMessage="Case Type is Required" />
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="txtCaseSequence"
                            Display="Dynamic" SetFocusOnError="true" CssClass="label label-danger" ErrorMessage="Case Sequence is Required" />
                    </div>
                </div>
                <div class="col-auto">
                    <asp:Label runat="server" AssociatedControlID="txtPhone" Text="Daytime Phone" />
                    <asp:TextBox runat="server" ClientIDMode="Static" CssClass="form-control phone" MaxLength="15" ID="txtPhone" />
                </div>
                <div class="col-auto">
                    <asp:Label runat="server" AssociatedControlID="drpLocation" Text="Office Location" />
                    <asp:DropDownList ID="drpLocation" runat="server" AppendDataBoundItems="true" CssClass="form-control"
                        DataTextField="CountyName" DataValueField="CountyId">
                        <asp:ListItem Text="&lt; Select Office &gt;" Value="" />
                    </asp:DropDownList>
                </div>
            </div>
            <div class="row form-group">
                <div class="col-auto">
                    <asp:Label runat="server" AssociatedControlID="drpCaseType" Text="Case Type" />
                    <asp:DropDownList ID="drpCaseType" runat="server" AppendDataBoundItems="true" CssClass="form-control"
                        DataTextField="CaseTypeName" DataValueField="CaseTypeID">
                        <asp:ListItem Text="&lt; What Type of Case? &gt;" Value="" />
                    </asp:DropDownList>
                </div>
                <div class="col-auto">
                    <asp:Label runat="server" AssociatedControlID="drpInitialContact" Text="Initial Contact" />
                    <asp:DropDownList ID="drpInitialContact" runat="server" AppendDataBoundItems="true" CssClass="form-control"
                        DataTextField="ContactName" DataValueField="ContactID">
                        <asp:ListItem Text="&lt; How Were you Contacted? &gt;" Value="" />
                    </asp:DropDownList>
                </div>
            </div>
            <fieldset class="outline-fieldset mb-2">
                <legend class="Head">Initial Resolution</legend>
                <ul class="radio-button-list column-4">
                    <li>
                        <asp:CheckBox ID="chkNeedsLetter" runat="server" Text="Needs Letter" /></li>
                    <li>
                        <asp:CheckBox ID="chkProvidedForms" runat="server" Text="Provided Forms" /></li>
                    <li>
                        <asp:CheckBox ID="chkAssistedForm" runat="server" Text="Assisted w/ Forms" /></li>
                    <li>
                        <asp:CheckBox ID="chkAssistedProcedures" runat="server" Text="Assisted w/ Procedures" /></li>
                    <li>
                        <asp:CheckBox ID="chkSetFinalHearing" runat="server" Text="Set Final Hearing" /></li>
                    <li>
                        <asp:CheckBox ID="chkSetOtherHearing" runat="server" Text="Set Other Hearing" /></li>
                    <li>
                        <asp:CheckBox ID="chkReferralOther" runat="server" Text="Referral Other" /></li>
                    <li>
                        <asp:CheckBox ID="chkReferralGmMag" runat="server" Text="Referral GM/MAG" /></li>
                    <li>
                        <asp:CheckBox ID="chkPreparedOrder" runat="server" Text="Prepared Order" /></li>
                    <li>
                        <asp:CheckBox ID="chkOther" runat="server" Text="Other" /></li>
                    <li>
                        <asp:CheckBox ID="chkAppointedPro" runat="server" Text="Appointed Professional" /></li>
                </ul>
            </fieldset>
            <div class="row form-group">
                <div class="col-auto">
                    <asp:Label runat="server" AssociatedControlID="txtResolutionDate" Text="Resolution Date" />
                    <asp:TextBox runat="server" ID="txtResolutionDate" MaxLength="15" ClientIDMode="Static" CssClass="form-control datepicker" />
                </div>
            </div>
            <asp:HiddenField ID="hdReceivedDate" runat="server" />
            <hr />
            <p>
                <asp:Button ID="cmdUpdate" runat="server" Text="Update" CssClass="btn btn-primary" OnClick="cmdUpdate_Click" />
            </p>
        </div>
    </div>
</div>
<dnn:DnnCssInclude runat="server" FilePath="~/Resources/Shared/components/TimePicker/Themes/jquery-ui.min.css" />
<dnn:dnnjsinclude runat="server" filepath="/Resources/Libraries/jQuery/jquery.mask.js" />

<script type="text/javascript">
    (function ($, Sys) {

        $(document).ready(function () {
            $(".datepicker").datepicker();
            $('.phone').mask('(000) 000-0000');
            Sys.Application.add_load(function (s, e) { PageInit(); });
            PageInit();
        });

    }(jQuery, window.Sys));

    function PageInit() {

    }
</script>
