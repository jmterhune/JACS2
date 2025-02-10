<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DestructionForm.ascx.cs" Inherits="tjc.Modules.RecordDestruction.View" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<%@ Register Assembly="DotNetNuke.Web" Namespace="DotNetNuke.Web.UI.WebControls" TagPrefix="dnn" %>
<div class="tabs">
    <ul class="nav nav-tabs">
        <li class="nav-item active">
            <a class="nav-link" href="#logForm" data-toggle="tab">Record Destruction Log</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=SearchLogUrl %>">Search Log</a>
        </li>
        <asp:PlaceHolder ID="phAdminTabs" runat="server" Visible="false">
            <li class="nav-item">
                <a class="nav-link" href="<%=DepartmentListUrl %>">Departments</a>
            </li>
            <li class="nav-item">
                <a class="nav-link" href="<%=RecordTypeListUrl %>">Record Types</a>
            </li>
            <li class="nav-item">
                <a class="nav-link" href="<%=RetentionPeriodListUrl %>">Retention Periods</a>
            </li>
            <li class="nav-item">
                <a class="nav-link" href="<%=DestructionMethodListUrl %>">Destruction Methods</a>
            </li>
        </asp:PlaceHolder>
    </ul>
    <div class="tab-content">
        <div id="logForm" class="tab-pane active fire-bg">
            <div class="row">
                <div class="col-auto">
                    <div class="form-group">
                        <asp:Label runat="server" AssociatedControlID="txtName" Text="Name" />
                        <asp:TextBox ID="txtName" Enabled="false" ReadOnly="true" runat="server" CssClass="form-control" />
                    </div>
                </div>
                <div class="col-auto">
                    <div class="form-group">
                        <asp:Label runat="server" AssociatedControlID="drpDepartment" Text="Department" />
                        <asp:DropDownList ID="drpDepartment" runat="server" CssClass="form-control" AppendDataBoundItems="True" DataTextField="GroupName"
                            DataValueField="GroupID">
                            <asp:ListItem Value="" Text="&lt; Select Department &gt;" />
                        </asp:DropDownList>
                        <asp:RequiredFieldValidator ID="valDepartment" runat="server" Display="Dynamic" ControlToValidate="drpDepartment" CssClass="label label-danger"
                            ErrorMessage="Department is Required" SetFocusOnError="true"></asp:RequiredFieldValidator>
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="col-auto">
                    <div class="form-group">
                        <asp:Label runat="server" AssociatedControlID="drpRecordType" Text="Record Type" />
                        <asp:DropDownList ID="drpRecordType" runat="server" CssClass="form-control" AppendDataBoundItems="True" DataTextField="Description"
                            DataValueField="RecordTypeID">
                            <asp:ListItem Value="" Text="&lt; Select Record Type &gt;" />
                        </asp:DropDownList>
                        <asp:RequiredFieldValidator ID="valRecordType" runat="server" Display="Dynamic" ControlToValidate="drpRecordType" CssClass="label label-danger"
                            ErrorMessage="Record Type is Required" SetFocusOnError="true"></asp:RequiredFieldValidator>

                    </div>
                </div>
                <div class="col-auto">
                    <div class="form-group">
                        <asp:Label runat="server" AssociatedControlID="drpRetentionPeriod" Text="Retention Period" />
                        <asp:DropDownList ID="drpRetentionPeriod" runat="server" CssClass="form-control" AppendDataBoundItems="True" DataTextField="Description"
                            DataValueField="RetentionPeriodID">
                            <asp:ListItem Value="" Text="&lt; Select Retention Period &gt;" />
                        </asp:DropDownList>
                        <asp:RequiredFieldValidator ID="valRetentionPeriod" runat="server" Display="Dynamic" ControlToValidate="drpRetentionPeriod" CssClass="label label-danger"
                            ErrorMessage="Retention Period is Required" SetFocusOnError="true"></asp:RequiredFieldValidator>

                    </div>
                </div>
                <div class="col-auto">
                    <div class="form-group">
                        <asp:Label runat="server" AssociatedControlID="drpDestructionMethod" Text="Destruction Method" />
                        <asp:DropDownList ID="drpDestructionMethod" runat="server" CssClass="form-control" AppendDataBoundItems="True" DataTextField="Description"
                            DataValueField="DestructionMethodID">
                            <asp:ListItem Value="" Text="&lt; Select Retention Period &gt;" />
                        </asp:DropDownList>
                        <asp:RequiredFieldValidator ID="valDestructionMethod" runat="server" Display="Dynamic" ControlToValidate="drpDestructionMethod" CssClass="label label-danger"
                            ErrorMessage="Destruction Method is Required" SetFocusOnError="true"></asp:RequiredFieldValidator>

                    </div>
                </div>
            </div>
            <div class="row">
                <div class="col-6">
                    <div class="form-group">
                        <asp:Label runat="server" AssociatedControlID="txtDescription" Text="Description" />
                        <asp:TextBox runat="server" ID="txtDescription" MaxLength="2000" TextMode="MultiLine" Rows="4" CssClass="form-control" />
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="txtDescription"
                            Display="Dynamic" SetFocusOnError="true" CssClass="label label-danger" ErrorMessage="Description is Required" />
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="col-auto">
                    <div class="form-group">
                        <asp:Label runat="server" AssociatedControlID="txtName" Text="Year Created" />
                        <asp:TextBox ID="txtYearCreated" runat="server" CssClass="form-control" TextMode="Number" min="2000" />
                        <asp:RequiredFieldValidator ID="valYearCreated" runat="server" Display="Dynamic" ControlToValidate="txtYearCreated" CssClass="label label-danger"
                            ErrorMessage="Year Created is Required" SetFocusOnError="true"></asp:RequiredFieldValidator>

                    </div>
                </div>

                <div class="col-auto">
                    <div class="form-group">
                        <asp:Label runat="server" AssociatedControlID="txtDateDestroyed" Text="Date Destroyed" />
                        <asp:TextBox ID="txtDateDestroyed" runat="server" CssClass="form-control date-picker" />
                        <asp:RequiredFieldValidator ID="valDateDestroyed" runat="server" Display="Dynamic" ControlToValidate="txtDateDestroyed" CssClass="label label-danger"
                            ErrorMessage="Date Destroyed is Required" SetFocusOnError="true"></asp:RequiredFieldValidator>

                    </div>
                </div>
                <div class="col-md-12 mt-3">
                    <asp:Label runat="server" ID="lblUpload" AssociatedControlID="ctlLogFile" Text="Upload File (Optional)" />
                    <asp:FileUpload ID="ctlLogFile" runat="server" ToolTip="Upload Log File" />
                </div>
            </div>
            <hr />
            <p class="mt-3">
                <asp:Button ID="cmdSave" ClientIDMode="Static" runat="server" CssClass="hidden" Text="Destroy Click" OnClick="cmdSave_Click" />
                <button class="btn btn-primary" id="cmdCheck">Destroy</button>
                <asp:HyperLink ID="cmdCancel" runat="server" CssClass="btn btn-secondary" Text="Cancel" />
            </p>
        </div>
    </div>
</div>
<image src="/images/fire.gif" id="imgFire" class="godzilla" style="display: none" />
<dnn:DnnCssInclude runat="server" FilePath="~/Resources/Shared/components/TimePicker/Themes/jquery-ui.min.css" />

<script type="text/javascript">
    (function ($, Sys) {
        $(".date-picker").datepicker();
        $("#cmdCheck").on("click", function (e) {
            e.preventDefault();
            ShowFire();
        });
    }(jQuery, window.Sys));

    function ShowFire() {
        const form = document.querySelector('form');
        if (form.checkValidity()) {
            if (typeof (Page_ClientValidate) == 'function') {
                Page_ClientValidate();
            }
            if (Page_IsValid) {
                $("#imgFire").show();
                setTimeout(function () { $("#cmdSave").click(); }, 1000);
                return true;
            } else {
                return false
            }
        } else {
            form.reportValidity();
            return false;
        }
        return true;
    }
</script>
