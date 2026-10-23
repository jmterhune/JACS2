<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Edit.ascx.cs" Inherits="tjc.Modules.ProcessServer.Edit" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<div class="tabs">
    <ul class="nav nav-tabs">
        <li class="active">
            <a class="nav-link active" href="#Personal" data-bs-toggle="tab">Application Details</a>
        </li>
        <li>
            <a class="nav-link" href="#Process" data-bs-toggle="tab">Application Status</a>
        </li>
        <li>
            <a class="nav-link" href="#Files" data-bs-toggle="tab">Manage Files</a>
        </li>
    </ul>
    <div class="tab-content">
        <div id="Personal" class="tab-pane active">
            <asp:UpdatePanel ID="UpdatePanel1" runat="server" RenderMode="Block" OnUnload="updateProcessServer_Unload">
                <ContentTemplate>
                    <p class="alert alert-info"><strong>Payment Type:</strong>
                        <asp:Literal ID="ltPaymentType" runat="server" /></p>
                    <div class="mb-3">
                        <div class="row">
                            <div class="col-md-5">
                                <asp:Label ID="lblFirstName" runat="server" AssociatedControlID="txtFirstName" Text="First Name " />
                                <asp:TextBox ID="txtFirstName" CssClass="form-control" MaxLength="25" runat="server"></asp:TextBox>
                            </div>
                            <div class="col-md-2">
                                <asp:Label ID="lblMiddleInitial" runat="server" AssociatedControlID="txtMiddleInitial" Text="Middle Initial" />
                                <asp:TextBox ID="txtMiddleInitial" CssClass="form-control" MaxLength="25" runat="server"></asp:TextBox>
                            </div>
                            <div class="col-md-5">
                                <asp:Label ID="lblLastName" runat="server" AssociatedControlID="txtLastName" Text="Last Name " />
                                <asp:TextBox ID="txtLastName" CssClass="form-control" MaxLength="25" runat="server"></asp:TextBox>
                            </div>
                        </div>
                    </div>
                    <div class="mb-3">
                        <div class="row">
                            <div class="col-md-3">
                                <asp:Label ID="lblCell" runat="server" AssociatedControlID="txtCell" Text="Primary Phone" />
                                <asp:TextBox ID="txtCell" runat="server" MaxLength="15" CssClass="form-control phone-us" placeholder="(555) 555-5555" AutoCompleteType="Cellular" />
                                <asp:CheckBox ID="chkShowPhone" runat="server" Text="Show Internet" CssClass="dnnFormRadioButtons" />
                            </div>
                            <div class="col-md-3">
                                <asp:Label ID="lblPhone" runat="server" AssociatedControlID="txtPhone" Text="Secondary Phone" />
                                <asp:TextBox ID="txtPhone" runat="server" MaxLength="15" CssClass="form-control phone-us" placeholder="(555) 555-5555" AutoCompleteType="HomePhone" />
                            </div>
                            <div class="col-md-6">
                                <asp:Label ID="lblEmail" runat="server" AssociatedControlID="txtEMail" Text="Email Address" />
                                <asp:TextBox ID="txtEMail" runat="server" MaxLength="150" CssClass="form-control" AutoCompleteType="Email" />
                            </div>
                        </div>
                    </div>
                    <div class="mb-3">
                        <div class="row">
                            <div class="col-md-3">
                                <asp:Label ID="lblExpirationDate" runat="server" AssociatedControlID="txtExpirationDate" Text="Bond Expires" />
                                <asp:TextBox ID="txtExpirationDate" CssClass="form-control datepicker" MaxLength="25" runat="server"></asp:TextBox>
                            </div>
                            <div class="col-md-3">
                                <asp:Label ID="lblNumber" runat="server" AssociatedControlID="txtNumber" Text="ID Number" />
                                <asp:TextBox ID="txtNumber" CssClass="form-control" TextMode="Number" MaxLength="25" runat="server"></asp:TextBox>
                            </div>
                            <div class="col-md-3">
                                <asp:Label ID="lblCity" runat="server" AssociatedControlID="txtCity" Text="City" />
                                <asp:TextBox ID="txtCity" runat="server" MaxLength="50" CssClass="form-control" AutoCompleteType="HomeCity" />
                            </div>
                            <div class="col-md-3">
                                <asp:Label ID="lblZip" runat="server" AssociatedControlID="txtZip" Text="Zip" />
                                <asp:TextBox ID="txtZip" runat="server" MaxLength="11" CssClass="form-control" AutoCompleteType="HomeZipCode" />
                            </div>
                        </div>
                    </div>
                    <div class="mb-3">
                        <div class="row">
                            <div class="col-md-12">
                                <asp:Label ID="lblNotes" runat="server" AssociatedControlID="txtNotes" Text="Notes" />
                                <asp:TextBox ID="txtNotes" TextMode="MultiLine" Rows="3" CssClass="form-control" runat="server"></asp:TextBox>
                            </div>
                        </div>
                    </div>
                    <div class="mb-3">
                        <div class="row">
                            <div class="col-md-12">
                                <fieldset class="border d-inline-block ps-2 rounded ">
                                    <legend class="w-auto ps-2 pe-2 fs-6">Website Photo</legend>
                                    <asp:Label ID="lblPhotoFileId" runat="server" CssClass="visually-hidden" Text="Photo" AssociatedControlID="phFileUpload" />
                                    <asp:PlaceHolder ID="phFileUpload" runat="server" />
                                </fieldset>
                            </div>
                        </div>
                    </div>
                    <asp:LinkButton ID="btnSubmit" runat="server" OnClick="btnSubmit_Click" Text="Save" CssClass="btn btn-primary" />
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="btnSubmit" EventName="Click" />
                </Triggers>
            </asp:UpdatePanel>
        </div>
        <div id="Process" class="tab-pane">
            <asp:UpdatePanel ID="UpdatePanel2" runat="server" RenderMode="Block" OnUnload="updateProcessServer_Unload">
                <ContentTemplate>
                    <asp:Literal ID="ltMessage1" runat="server" />

                    <div class="mb-3">
                        <div class="row">
                            <div class="col-md-4">
                                <asp:Label ID="lblStatus" runat="server" AssociatedControlID="drpStatus" Text="Change Status" />
                                <asp:DropDownList runat="server" ID="drpStatus" CssClass="form-control">
                                    <asp:ListItem Text="New" Value="0" />
                                    <asp:ListItem Text="Criminal History Completed" Value="1" />
                                    <asp:ListItem Text="Court Counsel Rejected" Value="-2" />
                                    <asp:ListItem Text="Court Counsel Approved" Value="2" />
                                    <asp:ListItem Text="Chief Judge Rejected" Value="-3" />
                                    <asp:ListItem Text="Chief Judge Approved" Value="3" />
                                    <asp:ListItem Text="Approved" Value="4" />
                                    <asp:ListItem Text="Rejected" Value="-1" />
                                </asp:DropDownList>
                            </div>

                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-4">
                            <asp:LinkButton ID="btnUpdateStatus" runat="server" OnClick="btnUpdateStatus_Click" Text="Update Status" CssClass="btn btn-primary  align-bottom" />
                        </div>
                    </div>
                    <hr />
                    <div class="mb-3">
                        <div class="row">
                            <div class="col-md-12">
                                <asp:Label ID="lblRejectionNotification" runat="server" AssociatedControlID="txtRejectionNotification" Text="Notification to User <small class='text-info'>(Enter text here and then click approve or reject application and the text will be inserted into the notification email)</small>" />
                                <asp:TextBox ID="txtRejectionNotification" CssClass="form-control" TextMode="MultiLine" Rows="3" runat="server"></asp:TextBox>
                            </div>
                        </div>
                    </div>
                    <asp:LinkButton ID="btnApprove" runat="server" OnClick="btnApprove_Click" Text="Approve Application" CssClass="btn btn-primary me-2" />
                    <asp:LinkButton ID="btnReject" runat="server" OnClick="btnReject_Click" Text="Reject Application" CssClass="btn btn-secondary" />
                    <asp:LinkButton ID="btnSendChiefJudge" runat="server" OnClick="btnSendChiefJudge_Click" Text="Send Chief Judge Notification" CssClass="btn btn-quaternary float-end" />
                    <asp:LinkButton ID="btnSendCourtCounsel" runat="server" OnClick="btnSendCourtCounsel_Click" Text="Send Court Counsel Notification" CssClass="btn btn-tertiary float-end me-2" />
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="btnApprove" EventName="Click" />
                    <asp:AsyncPostBackTrigger ControlID="btnReject" EventName="Click" />
                    <asp:AsyncPostBackTrigger ControlID="btnSendChiefJudge" EventName="Click" />
                    <asp:AsyncPostBackTrigger ControlID="btnSendCourtCounsel" EventName="Click" />
                </Triggers>
            </asp:UpdatePanel>
        </div>
        <div id="Files" class="tab-pane">
            <asp:UpdatePanel ID="UpdatePanel3" runat="server" RenderMode="Block" OnUnload="updateProcessServer_Unload">
                <ContentTemplate>

                    <fieldset class="border p-2 pt-0 rounded" style="min-height: 50px">
                        <legend class="w-auto ps-2 pe-2 fs-6">Files</legend>
                        <asp:Repeater ID="rptUploades" runat="server" OnItemDataBound="rptUploades_ItemDataBound" OnItemCommand="rptUploades_ItemCommand" OnItemCreated="rptUploades_ItemCreated">
                            <HeaderTemplate>
                                <table class="table table-striped">
                                    <thead>
                                        <tr>
                                            <th>File Name</th>
                                            <th>Uploaded</th>
                                            <th>Hidden?</th>
                                            <th>&nbsp;</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <tr>
                                    <td>
                                        <asp:HyperLink ID="lnkFile" runat="server"><%#DataBinder.Eval(Container.DataItem,"OriginalFileName") %></asp:HyperLink></td>
                                    <td><%#DataBinder.Eval(Container.DataItem,"DateUploaded","{0:MM/dd/yyyy}") %></td>
                                    <td class="command-icon">
                                        <asp:LinkButton ID="cmdHide" runat="server" CommandName="hide" CommandArgument='<%#DataBinder.Eval(Container.DataItem,"AttachmentID").ToString() %>'>
                                            <i class="fa fa-square" aria-hidden="true">&nbsp;</i>
                                        </asp:LinkButton>
                                    </td>
                                    <td class="command-icon">
                                        <asp:LinkButton ID="cmdDelete" runat="server" CommandName="delete" OnClientClick="return confirm('Delete this File?')" CommandArgument='<%#DataBinder.Eval(Container.DataItem,"AttachmentID").ToString() %>'><i class="fa fa-trash" aria-hidden="true"></i></asp:LinkButton>
                                    </td>
                                </tr>
                            </ItemTemplate>
                            <FooterTemplate>
                                </tbody>
                        </table>
                            </FooterTemplate>
                        </asp:Repeater>
                    </fieldset>
                    <hr />
                    <fieldset class="border p-2 pt-0 rounded">
                        <legend class="w-auto ps-2 pe-2 fs-6">Upload Files</legend>
                        <div class="row">
                            <div class="mb-3">
                                <div class="col-md-4">
                                    <asp:FileUpload ID="fuUpload" runat="server" AllowMultiple="true" />
                                </div>
                                <div class="col-md-4">
                                    <asp:CheckBox ID="chkHide" runat="server" CssClass="dnnFormRadioButtons" Text="Hide from Applicant" />
                                </div>
                            </div>
                        </div>
                        <asp:Button ID="btnSaveFile" runat="server" OnClick="btnSaveFile_Click" Text="Upload Selected File" CssClass="btn btn-primary" />

                    </fieldset>
                </ContentTemplate>
                <Triggers>
                    <asp:PostBackTrigger ControlID="btnSaveFile" />
                </Triggers>
            </asp:UpdatePanel>

        </div>
    </div>
</div>
<p class="text-center">
    <asp:LinkButton ID="btnCancel" runat="server" OnClick="btnCancel_Click" Text="Return to List" CssClass="btn btn-default" />
</p>
<dnn:dnncssinclude runat="server" filepath="~/Resources/Shared/components/TimePicker/Themes/jquery-ui.min.css" />
<dnn:dnnjsinclude runat="server" filepath="/Resources/Libraries/jQuery/jquery.mask.js" />

<script type="text/javascript">
    /*globals jQuery, window, Sys */
    (function ($, Sys) {
        function dnnEditBasicSettings() {
            $(".datepicker").datepicker();
            $(".datepicker").mask('00/00/0000')
        }
        $(document).ready(function () {
            dnnEditBasicSettings();
            Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
                dnnEditBasicSettings();
            });
        });

    }(jQuery, window.Sys));
    function SetTab() {
        $(document).ready(function () {
            $('a[href="#Files"]').tab('show');
        });
    }
    function ShowMessage(message) {
        $.dnnAlert({ okText: 'Ok', text: message });
    }
</script>
