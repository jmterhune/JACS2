<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EditHistory.ascx.cs" Inherits="tjc.Modules.CourtCounsel.Views.EditHistory" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>

<nav class="navbar navbar-expand-lg navbar-dark bg-dark mb-md">
    <button class="navbar-toggler" type="button" data-toggle="collapse" data-target="#navbarNav">
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

<div class="container-fluid mt-3">
    <asp:ValidationSummary ID="valSummary" runat="server" CssClass="alert alert-danger" DisplayMode="BulletList" />

    <!-- Row 1: Action Date, Case Number, Case Name -->
    <div class="form-row mb-3">
        <div class="form-group col-md-4">
            <label for="<%=txtDateReceived.ClientID %>">Action Date <span class="text-danger">*</span></label>
            <asp:TextBox ID="txtDateReceived" runat="server" CssClass="form-control" TextMode="SingleLine" type="date" />
            <asp:RequiredFieldValidator ID="rfvDateReceived" runat="server" ControlToValidate="txtDateReceived"
                ErrorMessage="Action Date is required." Display="None" />
        </div>
        <div class="form-group col-md-4">
            <label for="<%=txtCaseNumber.ClientID %>">Case Number <span class="text-danger">*</span></label>
            <asp:TextBox ID="txtCaseNumber" runat="server" CssClass="form-control" MaxLength="18" />
            <asp:RequiredFieldValidator ID="rfvCaseNumber" runat="server" ControlToValidate="txtCaseNumber"
                ErrorMessage="Case Number is required." Display="None" />
            <asp:RegularExpressionValidator ID="revCaseNumber" runat="server" ControlToValidate="txtCaseNumber"
                ValidationExpression="^[DMSV]-\d{4}-[A-Z]{2}-\d{6}$"
                ErrorMessage="Case Number must be in the format: D-2024-FL-000001" Display="None" />
        </div>
        <div class="form-group col-md-4">
            <label for="<%=txtCaseName.ClientID %>">Case Name <span class="text-danger">*</span></label>
            <asp:TextBox ID="txtCaseName" runat="server" CssClass="form-control" MaxLength="100" />
            <asp:RequiredFieldValidator ID="rfvCaseName" runat="server" ControlToValidate="txtCaseName"
                ErrorMessage="Case Name is required." Display="None" />
        </div>
    </div>

    <!-- Row 2: Case Type, Requested By, Responsible/Attorney -->
    <div class="form-row mb-3">
        <div class="form-group col-md-4">
            <label for="<%=drpCaseType.ClientID %>">Case Type <span class="text-danger">*</span></label>
            <asp:DropDownList ID="drpCaseType" runat="server" CssClass="form-control" />
            <asp:RequiredFieldValidator ID="rfvCaseType" runat="server" ControlToValidate="drpCaseType"
                InitialValue="" ErrorMessage="Case Type is required." Display="None" />
        </div>
        <div class="form-group col-md-4">
            <label for="<%=drpRequestor.ClientID %>">Requested By <span class="text-danger">*</span></label>
            <asp:DropDownList ID="drpRequestor" runat="server" CssClass="form-control" />
            <asp:RequiredFieldValidator ID="rfvRequestor" runat="server" ControlToValidate="drpRequestor"
                InitialValue="" ErrorMessage="Requested By is required." Display="None" />
        </div>
        <div class="form-group col-md-4">
            <label for="<%=drpAttorney.ClientID %>">Responsible / Attorney <span class="text-danger">*</span></label>
            <asp:DropDownList ID="drpAttorney" runat="server" CssClass="form-control" />
            <asp:RequiredFieldValidator ID="rfvAttorney" runat="server" ControlToValidate="drpAttorney"
                InitialValue="" ErrorMessage="Responsible / Attorney is required." Display="None" />
        </div>
    </div>

    <!-- Row 3: Motion Filed, County, Action Taken -->
    <div class="form-row mb-3">
        <div class="form-group col-md-4">
            <label for="<%=txtMotionFiled.ClientID %>">Motion Filed</label>
            <asp:TextBox ID="txtMotionFiled" runat="server" CssClass="form-control" TextMode="SingleLine" type="date" />
        </div>
        <div class="form-group col-md-4">
            <label for="<%=drpCounty.ClientID %>">County</label>
            <asp:DropDownList ID="drpCounty" runat="server" CssClass="form-control" />
        </div>
        <div class="form-group col-md-4">
            <label for="<%=drpAction.ClientID %>">Action Taken</label>
            <asp:DropDownList ID="drpAction" runat="server" CssClass="form-control" />
        </div>
    </div>

    <!-- Row 4: Date Completed, Time Spent, Status -->
    <div class="form-row mb-3">
        <div class="form-group col-md-4">
            <label for="<%=txtDateCompleted.ClientID %>">Date Completed</label>
            <asp:TextBox ID="txtDateCompleted" runat="server" CssClass="form-control" TextMode="SingleLine" type="date" />
        </div>
        <div class="form-group col-md-4">
            <label for="<%=drpTimeSpan.ClientID %>">Time Spent</label>
            <asp:DropDownList ID="drpTimeSpan" runat="server" CssClass="form-control" />
        </div>
        <div class="form-group col-md-4">
            <label for="<%=drpStatus.ClientID %>">Status</label>
            <asp:DropDownList ID="drpStatus" runat="server" CssClass="form-control">
                <asp:ListItem Text="" Value="" />
                <asp:ListItem Text="Advisement" Value="Advisement" />
                <asp:ListItem Text="Appeal Filed" Value="Appeal Filed" />
                <asp:ListItem Text="Appeal Pending" Value="Appeal Pending" />
                <asp:ListItem Text="Awaiting Entry of Order" Value="Awaiting Entry of Order" />
                <asp:ListItem Text="Briefs" Value="Briefs" />
                <asp:ListItem Text="Case Closed" Value="Case Closed" />
                <asp:ListItem Text="Comp/Awaiting" Value="Comp/Awaiting" />
                <asp:ListItem Text="Compliance" Value="Compliance" />
                <asp:ListItem Text="Compliance-Partial" Value="Compliance-Partial" />
                <asp:ListItem Text="Continued" Value="Continued" />
                <asp:ListItem Text="Decision Pending" Value="Decision Pending" />
                <asp:ListItem Text="Denied" Value="Denied" />
                <asp:ListItem Text="Dismissed" Value="Dismissed" />
                <asp:ListItem Text="Evidentiary Hearing Set" Value="Evidentiary Hearing Set" />
                <asp:ListItem Text="Filed" Value="Filed" />
                <asp:ListItem Text="Granted" Value="Granted" />
                <asp:ListItem Text="Hearing" Value="Hearing" />
                <asp:ListItem Text="Hearing Set" Value="Hearing Set" />
                <asp:ListItem Text="Mediation" Value="Mediation" />
                <asp:ListItem Text="Monitoring" Value="Monitoring" />
                <asp:ListItem Text="Motion Pending" Value="Motion Pending" />
                <asp:ListItem Text="Need to Schedule" Value="Need to Schedule" />
                <asp:ListItem Text="Need to Set Hearing" Value="Need to Set Hearing" />
                <asp:ListItem Text="New Filing" Value="New Filing" />
                <asp:ListItem Text="No Action Required" Value="No Action Required" />
                <asp:ListItem Text="No Action Taken" Value="No Action Taken" />
                <asp:ListItem Text="On Hold" Value="On Hold" />
                <asp:ListItem Text="Order Drafted" Value="Order Drafted" />
                <asp:ListItem Text="Order Entered" Value="Order Entered" />
                <asp:ListItem Text="Order Signed" Value="Order Signed" />
                <asp:ListItem Text="Oral Argument Set" Value="Oral Argument Set" />
                <asp:ListItem Text="Pending" Value="Pending" />
                <asp:ListItem Text="Referred To GA" Value="Referred To GA" />
                <asp:ListItem Text="Remand" Value="Remand" />
                <asp:ListItem Text="Report Filed" Value="Report Filed" />
                <asp:ListItem Text="Research" Value="Research" />
                <asp:ListItem Text="Review" Value="Review" />
                <asp:ListItem Text="Ruling" Value="Ruling" />
                <asp:ListItem Text="Scheduling" Value="Scheduling" />
                <asp:ListItem Text="Under Advisement" Value="Under Advisement" />
                <asp:ListItem Text="Voluntary Dismissal" Value="Voluntary Dismissal" />
                <asp:ListItem Text="Withdrawn" Value="Withdrawn" />
            </asp:DropDownList>
        </div>
    </div>

    <!-- Row 5: Comments -->
    <div class="form-row mb-3">
        <div class="form-group col-12">
            <label for="<%=txtComments.ClientID %>">Comments</label>
            <asp:TextBox ID="txtComments" runat="server" CssClass="form-control" TextMode="MultiLine" MaxLength="8000" Rows="4" />
        </div>
    </div>

    <!-- Row 6: Future Action Date (edit mode only) -->
    <asp:Panel ID="pnlFutureAction" runat="server" Visible="false">
        <div class="form-row mb-3">
            <div class="form-group col-md-4">
                <label for="<%=txtFutureAction.ClientID %>">Future Action Date</label>
                <asp:TextBox ID="txtFutureAction" runat="server" CssClass="form-control" TextMode="SingleLine" type="date" />
            </div>
        </div>
    </asp:Panel>

    <!-- Buttons -->
    <div class="form-row mb-3">
        <div class="col">
            <asp:Button ID="cmdSave" runat="server" CssClass="btn btn-primary" Text="Save" OnClick="cmdSave_Click" />
            <asp:Button ID="cmdCancel" runat="server" CssClass="btn btn-default" Text="Cancel" OnClick="cmdCancel_Click" CausesValidation="false" />
            <asp:Button ID="cmdDelete" runat="server" CssClass="btn btn-danger ml-3" Text="Delete" OnClick="cmdDelete_Click"
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
    });
</script>
