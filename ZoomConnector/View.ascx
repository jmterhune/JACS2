<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="View.ascx.cs" Inherits="tjc.Modules.ZoomConnector.View" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<%-- SweetAlert2 + Noty for confirms / toast notifications --%>
<dnn:DnnCssInclude runat="server" FilePath="/Resources/Libraries/sweetalert/sweetalert2.min.css" />
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/sweetalert/sweetalert2.all.min.js" />
<dnn:DnnCssInclude runat="server" FilePath="/Resources/Libraries/Noty/noty.min.css" />
<dnn:DnnCssInclude runat="server" FilePath="/Resources/Libraries/Noty/bootstrap-v4.min.css" />
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/Noty/noty.min.js" />
<div id="zoomContainer">
        <asp:Literal ID="ltMessages" runat="server"></asp:Literal>
    <div class="form-group">
        <asp:Label runat="server" ID="lblCounty" AssociatedControlID="drpCounty">County</asp:Label>
        <asp:DropDownList ID="drpCounty" runat="server" OnSelectedIndexChanged="drpCounty_SelectedIndexChanged" AutoPostBack="true" CssClass="form-control">
            <asp:ListItem Text="< Select County >" Value="" />
            <asp:ListItem Text="DeSoto" Value="d" />
            <asp:ListItem Text="Manatee" Value="m" />
            <asp:ListItem Text="Sarasota" Value="s" />
        </asp:DropDownList>
        <asp:RequiredFieldValidator ID="valCounty" runat="server" Display="Dynamic" ControlToValidate="drpCounty"
            ErrorMessage="Please Select a County" CssClass="label label-danger" ></asp:RequiredFieldValidator>

    </div>
    <div class="form-group">
        <asp:Label runat="server" ID="lblLocation" AssociatedControlID="drpLocation">Location</asp:Label>
        <asp:DropDownList ID="drpLocation" runat="server" Enabled="false" CssClass="form-control">
            <asp:ListItem Text="< Select Location >" Value="" />
        </asp:DropDownList>
    </div>
    <div class="form-group">
        <asp:Label runat="server" ID="lblMeetingInfo" AssociatedControlID="txtMeetingInfo">Meeting Info <small id="emailHelp" class="text-muted">(Paste Meeting Info in the field below)</small></asp:Label>
        <asp:TextBox ID="txtMeetingInfo" runat="server" Width="500" TextMode="MultiLine" CssClass="form-control" Rows="5" />
        
        <asp:RequiredFieldValidator ID="valMeetingInfo" runat="server" Display="Dynamic" ControlToValidate="txtMeetingInfo"
            ErrorMessage="Please paste in the meeting information" CssClass="label label-danger" ></asp:RequiredFieldValidator>

    </div>
    <asp:LinkButton ID="cmdUpdate" runat="server" CausesValidation="True" CssClass="btn btn-primary dial" Text="Dial" OnClick="cmdUpdate_Click">
    </asp:LinkButton>
    <asp:HyperLink ID="cmdCancel" runat="server" CssClass="btn btn-secondary"
        Text="Cancel">
    </asp:HyperLink>
</div>
<script>
    $(function () {
        $('.dial').click(function (e) {
            if (Page_ClientValidate() == true) {
                $(this).addClass('aspNetDisabled');
                $(this).val('Please Wait...');
            }
        });
    });
</script>