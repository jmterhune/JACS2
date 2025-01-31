<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EditClient.ascx.cs" Inherits="tjc.Modules.FamilySelfHelp.EditClient" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
        <div class="btn-group mb-2">
            <asp:HyperLink ID="lnkSearch" CssClass="btn btn-primary active" runat="server">Search</asp:HyperLink>
            <asp:HyperLink ID="lnkDataEntry" CssClass="btn btn-primary" runat="server">Data Entry</asp:HyperLink>
            <asp:HyperLink ID="lnkMerge" CssClass="btn btn-primary" Visible="false" runat="server">Merge Clients</asp:HyperLink>
            <asp:HyperLink ID="lnkReports" CssClass="btn btn-primary" Visible="false" runat="server">Reports</asp:HyperLink>
        </div>

<div id="ClientEditForm">
    <div class="form-group row">
        <div class="col-4">
            <asp:Label runat="server" AssociatedControlID="txtLastName" Text="Last Name<em>*</em>" ToolTip="Required" />
            <asp:TextBox runat="server" CssClass="form-control form-control-sm" MaxLength="50" ID="txtLastName" />
            <asp:RequiredFieldValidator runat="server" ControlToValidate="txtLastName"
                Display="Dynamic" SetFocusOnError="true" CssClass="label label-danger" ErrorMessage="Last Name is Required" />
        </div>
        <div class="col-3">
            <asp:Label runat="server" AssociatedControlID="txtFirstName" Text="First Name<em>*</em>" ToolTip="Required" />
            <asp:TextBox runat="server" CssClass="form-control form-control-sm" MaxLength="50" ID="txtFirstName" />
            <asp:RequiredFieldValidator runat="server" ControlToValidate="txtFirstName"
                Display="Dynamic" SetFocusOnError="true" CssClass="label label-danger" ErrorMessage="First Name is Required" />
        </div>
        <div class="col-1">
            <asp:Label runat="server" AssociatedControlID="txtMiddleInitial" Text="<abbr title='Middle Initial'>MI</abbr>" />
            <asp:TextBox runat="server" CssClass="form-control  form-control-sm" MaxLength="1" ID="txtMiddleInitial" />
        </div>
       
    </div>
    <div class="form-group row"> 
        <div class="col-4">
            <asp:Label runat="server" AssociatedControlID="txtEmail" Text="Email" />
            <asp:TextBox runat="server" CssClass="form-control form-control-sm" MaxLength="250" ID="txtEmail" />
            <asp:RegularExpressionValidator Display="Dynamic" SetFocusOnError="true" CssClass="label label-danger" ValidationExpression="^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$" ErrorMessage="Invalid email address" ControlToValidate="txtEmail" runat="server" />
        </div>
        <div class="col-3">
            <asp:Label runat="server" AssociatedControlID="txtPhone" Text="Phone" />
            <asp:TextBox runat="server" CssClass="form-control  form-control-sm phone" MaxLength="50" ID="txtPhone" />
        </div>
    </div>
    <hr />
    <p>
        <asp:Button Text="Submit" ID="cmdSubmit" runat="server" CssClass="btn btn-primary me-2" OnClick="cmdSubmit_Click" />
        <asp:HyperLink ID="lnkCancel" runat="server" CssClass="btn btn-danger me-2" Text="Cancel" />
        <asp:Button Text="Delete" ID="cmdDelete" runat="server" CssClass="btn btn-danger" OnClick="cmdDelete_Click" />
    </p>
</div>
<dnn:dnnjsinclude runat="server" filepath="/Resources/Libraries/jQuery/jquery.mask.js" />

<script type="text/javascript">
    /*globals jQuery, window, Sys */
    (function ($, Sys) {
        $(document).ready(function () {
            $('.phone').mask('(000) 000-0000');
        });
    }(jQuery, window.Sys));
</script>
