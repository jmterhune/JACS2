<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="StampDetail.ascx.cs" Inherits="tjc.Modules.Purchasing.StampDetail" %>
<%@ Register TagPrefix="dnn" TagName="label" Src="~/controls/LabelControl.ascx" %>
<div class="stamp-form-container">
    <div class="row" id="referral-form">
        <div class="col-md-4">
            <asp:Label runat="server" CssClass="form-label" AssociatedControlID="txtRequestor" Text="Requester Name" />
            <asp:TextBox ID="txtRequestor" runat="server" MaxLength="100" CssClass="form-control"></asp:TextBox>
            <asp:RequiredFieldValidator runat="server" ControlToValidate="txtRequestor"
                CssClass="label label-danger" ErrorMessage="Requester is Required" />
        </div>
        <div class="col-md-4">
            <asp:Label runat="server" CssClass="form-label" AssociatedControlID="txtPhone" Text="Phone" />
            <asp:TextBox ID="txtPhone" runat="server" MaxLength="20" CssClass="form-control"></asp:TextBox>
            <asp:RequiredFieldValidator runat="server" ControlToValidate="txtPhone"
                CssClass="label label-danger" ErrorMessage="Phone is Required" />
        </div>
        <div class="col-md-4">
            <asp:Label runat="server" CssClass="form-label" AssociatedControlID="txtEmailAddress" Text="Email Address" />
            <asp:TextBox ID="txtEmailAddress" runat="server" MaxLength="250" CssClass="form-control"></asp:TextBox>
            <asp:RequiredFieldValidator runat="server" ControlToValidate="txtEmailAddress"
                CssClass="label label-danger" ErrorMessage="Email Address is Required" />
        </div>
        <div class="col-md-4">
            <asp:Label runat="server" CssClass="form-label" AssociatedControlID="drpLocation" Text="Delivery Location" />
            <asp:DropDownList ID="drpLocation" runat="server" CssClass="form-control">
                <asp:ListItem Text="< Select Location >" Value=""></asp:ListItem>
                <asp:ListItem Text="CJC"></asp:ListItem>
                <asp:ListItem Text="DeSoto"></asp:ListItem>
                <asp:ListItem Text="Manatee"></asp:ListItem>
                <asp:ListItem Text="Sarasota"></asp:ListItem>
                <asp:ListItem Text="Venice"></asp:ListItem>
                <asp:ListItem Text="1751 Mound Street"></asp:ListItem>
            </asp:DropDownList>
            <asp:RequiredFieldValidator runat="server" ControlToValidate="drpLocation"
                CssClass="label label-danger" ErrorMessage="Please Select a Delivery Location" />
        </div>
        <div class="col-md-4">
            <asp:Label runat="server" CssClass="form-label" AssociatedControlID="txtConsumerName" Text="Who is the Stamp for?" />
            <asp:TextBox ID="txtConsumerName" runat="server" MaxLength="100" CssClass="form-control"></asp:TextBox>
            <asp:RequiredFieldValidator runat="server" ControlToValidate="txtConsumerName"
                CssClass="label label-danger" ErrorMessage="Phone is Required" />
        </div>
        <div class="col-md-4">
            <asp:Label runat="server" CssClass="form-label" AssociatedControlID="drpStampType" Text="Type of Stamp" />
            <asp:DropDownList ID="drpStampType" runat="server" CssClass="form-control">
                <asp:ListItem Text="< Select Type >" Value=""></asp:ListItem>
                <asp:ListItem Text="conforming"></asp:ListItem>
                <asp:ListItem Text="**signature"></asp:ListItem>
                <asp:ListItem Text="other"></asp:ListItem>
            </asp:DropDownList>
            <asp:RequiredFieldValidator runat="server" ControlToValidate="drpStampType"
                CssClass="label label-danger" ErrorMessage="Please Select a Type" />
        </div>
        <div class="col-md-4">
            <asp:Label runat="server" CssClass="form-label" AssociatedControlID="drpFontStyle" Text="Font Style" />
            <asp:DropDownList ID="drpFontStyle" runat="server" CssClass="form-control">
                <asp:ListItem Text="< Select Font Style >" Value=""></asp:ListItem>
                <asp:ListItem Text="Arial" style="font-family: Arial; font-size: 2em"></asp:ListItem>
                <asp:ListItem Text="Arial Narrow" style="font-family: Arial Narrow; font-size: 2em"></asp:ListItem>
                <asp:ListItem Text="Calibri" style="font-family: Calibri; font-size: 2em"></asp:ListItem>
            </asp:DropDownList>
            <asp:RequiredFieldValidator runat="server" ControlToValidate="drpFontStyle"
                CssClass="label label-danger" ErrorMessage="Font Style is Required" />
        </div>
        <div class="col-md-3">
            <asp:Label runat="server" CssClass="form-label" AssociatedControlID="drpInkColor" Text="Ink Color" />
            <asp:DropDownList ID="drpInkColor" runat="server" CssClass="form-control">
                <asp:ListItem Text="< Select Ink Color >" Value=""></asp:ListItem>
                <asp:ListItem Text="Black"></asp:ListItem>
                <asp:ListItem Text="Blue"></asp:ListItem>
                <asp:ListItem Text="Red"></asp:ListItem>
            </asp:DropDownList>
            <asp:RequiredFieldValidator runat="server" ControlToValidate="drpInkColor"
                CssClass="label label-danger" ErrorMessage="Ink Color is Required" />
        </div>
        <div class="col-md-2">
            <asp:Label runat="server" CssClass="form-label" AssociatedControlID="txtFontSize" Text="Font Size" />
            <asp:TextBox ID="txtFontSize" runat="server" MaxLength="20" CssClass="form-control"></asp:TextBox>
            <asp:RequiredFieldValidator runat="server" ControlToValidate="txtFontSize"
                CssClass="label label-danger" ErrorMessage="Font Size is Required" />
        </div>
        <div class="col-md-2">
            <asp:Label runat="server" CssClass="form-label" AssociatedControlID="txtQuantity" Text="Quantity" />
            <asp:TextBox ID="txtQuantity" runat="server" MaxLength="50" CssClass="form-control"></asp:TextBox>
            <asp:RequiredFieldValidator runat="server" ControlToValidate="txtQuantity"
                CssClass="label label-danger" ErrorMessage="Quantity is Required" />
        </div>
        <div class="col-md-6">
            <asp:Label runat="server" CssClass="form-label" AssociatedControlID="txtSample" Text="Enter Text to Appear on Stamp" />
            <asp:TextBox ID="txtSample" runat="server" MaxLength="2000" TextMode="MultiLine" Rows="4" CssClass="form-control text-center"></asp:TextBox>
            <asp:RequiredFieldValidator runat="server" ControlToValidate="txtSample"
                CssClass="label label-danger" ErrorMessage="Sample is Required" />
        </div>
        <div class="col-md-6">
            <asp:Label runat="server" CssClass="form-label" Text="Sample of the Stamp Text" />
            <asp:Literal ID="ltSample" runat="server"></asp:Literal>
        </div>
    </div>
    <div class="row mt-2 mb-3">
        <div class="col-md-6">
            <asp:Label runat="server" CssClass="form-label" AssociatedControlID="txtInstructions" Text="Additional Information for Purchasing" />
            <asp:TextBox ID="txtInstructions" CssClass="form-control" runat="server" MaxLength="2000" TextMode="MultiLine" Rows="5"></asp:TextBox>
        </div>
        <div class="col-md-6">
            <asp:Label runat="server" CssClass="form-label" AssociatedControlID="txtRejectionNotice" Text="Rejection Reason" />
            <asp:TextBox ID="txtRejectionNotice" CssClass="form-control" runat="server" MaxLength="2000" TextMode="MultiLine" Rows="5"></asp:TextBox>
        </div>
    </div>
    <div class="attachment-container">
        <asp:Literal ID="ltAttachments"  runat="server" />
    </div>
    <hr />
    <p>
        <asp:LinkButton ID="cmdSave" runat="server" CssClass="btn btn-primary" Text="Save" OnClick="cmdSave_Click" />
        <asp:HyperLink ID="cmdCancel" runat="server" CssClass="btn btn-secondary" Text="Cancel" />
        <asp:LinkButton ID="cmdReject" runat="server" CssClass="btn btn-default reject" Text="Reject" OnClick="cmdReject_Click" />
    </p>
</div>
