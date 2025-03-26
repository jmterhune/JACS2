<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FormList.ascx.cs" Inherits="tjc.Modules.TranscriptDatabase.FormList" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<div class="tabs">
    <ul class="nav nav-tabs">
        <li class="nav-item">
            <a class="nav-link" href="<%=DesignationListUrl%>">Designations</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=CalendartUrl%>">Calendar</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=AttorneyListUrl%>">Attorneys</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=NamesListUrl%>">Names</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=OfficeListUrl%>">Offices</a>
        </li>
        <li class="nav-item active">
            <a class="nav-link" href="#forms" data-toggle="tab">Forms</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=HearingListUrl%>">Hearing Types</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=ReportListUrl%>">Reporting</a>
        </li>
    </ul>
    <div class="tab-content pb-0">
        <div id="forms" class="tab-pane active">
            <asp:UpdatePanel ID="pnlForms" runat="server" RenderMode="Block" OnUnload="pnlForms_Unload">
                <ContentTemplate>
                    <asp:UpdateProgress ID="upProgressEvent" runat="server">
                        <ProgressTemplate>
                            <div class="modal-progress">
                                <div class="center-progress">
                                    <img alt="" src="/images/loading.gif" />
                                </div>
                            </div>
                        </ProgressTemplate>
                    </asp:UpdateProgress>
                    <asp:Literal ID="ltMessage" runat="server" />
                    <asp:Repeater ID="rptForm" runat="server" OnItemCreated="rptForms_ItemCreated" OnItemCommand="rptForms_ItemCommand">
                        <HeaderTemplate>
                            <table id="tblForm" class="table table-striped">
                                <thead>
                                    <tr>
                                        <th>&nbsp;</th>
                                        <th>Form Type</th>
                                        <th>File Name</th>
                                        <th>&nbsp;</th>
                                    </tr>
                                </thead>
                                <tbody>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <tr>
                                <td class="command-item">
                                    <asp:LinkButton ID="cmdEdit" runat="server" CommandName="edit" CausesValidation="false" CommandArgument='<%#DataBinder.Eval(Container.DataItem,"FormID").ToString() %>'><i class="fa fa-pencil"></i></asp:LinkButton>
                                <td><%#Eval("FormText")%></td>
                                <td>
                                    <asp:HyperLink ID="lnkfile" runat="server" NavigateUrl='<%#Eval("filepath")%>' Text='<%#Eval("fileName")%>' /></td>
                                <td class="command-item">
                                    <asp:LinkButton ID="cmdDelete" CssClass="confirm" runat="server" CausesValidation="false" CommandName="delete" CommandArgument='<%#DataBinder.Eval(Container.DataItem,"FormID").ToString() %>'><i class="fa fa-trash"></i></asp:LinkButton>
                                </td>
                            </tr>
                        </ItemTemplate>
                        <FooterTemplate>
                            </tbody>
                            </table>
                        </FooterTemplate>
                    </asp:Repeater>
                    <asp:HiddenField ClientIDMode="Static" ID="hdFileId" runat="server" />
                    <div class="modal fade" id="EditFormModal" tabindex="-1" role="dialog" aria-labelledby="EditFormModalLabel" aria-hidden="true">
                        <div class="modal-dialog">
                            <div class="modal-content">
                                <div class="modal-header">
                                    <h4 class="modal-title" id="EditFormModalLabel">Add / Edit Form</h4>
                                    <button type="button" class="close" data-bs-dismiss="modal" aria-hidden="true">&times;</button>
                                </div>
                                <div class="modal-body">
                                    <div class="form-group">
                                        <asp:Label runat="server" AssociatedControlID="drpFileType" Text="Form Type" />
                                        <asp:DropDownList runat="server" ID="drpFileType" CssClass="form-control">
                                        </asp:DropDownList>
                                        <asp:RequiredFieldValidator Display="Dynamic" SetFocusOnError="true" ValidationGroup="Form" CssClass="label label-danger"
                                            ErrorMessage="Form Type Is Required" ControlToValidate="drpFileType" runat="server" />
                                    </div>
                                    <div class="form-group clearfix">
                                        <asp:Label ID="lblupload" runat="server" AssociatedControlID="uplFile" Text="Upload Attachments<em>*</em>" />
                                        <div style="position: relative;">
                                            <div id="upload-overlay" class="overlay" style="display: none;">
                                                <div class="spinner"></div>
                                            </div>
                                            <asp:FileUpload ID="uplFile" runat="server" ToolTip="Select File to Upload" AllowMultiple="false" ClientIDMode="Static" CssClass="fileUpload" accept=".pdf,.doc,.docx,.xls,.xlsx" />
                                            <span id="fileUploadWarning" style="display: none" class="label label-danger upload-warning">Please Choose File to Upload</span>
                                            <asp:CustomValidator ID="valUpload" Display="Dynamic" ValidationGroup="Form" runat="server" CssClass="label label-danger" ClientValidationFunction="validateUpload"
                                                ErrorMessage="Please select a file" OnServerValidate="valUpload_ServerValidate"></asp:CustomValidator>
                                            <span id="uploadInfo"></span>
                                        </div>
                                        <div class="form-group">
                                            <asp:Label ID="lblLink" runat="server" AssociatedControlID="lnkFormUrl" Text="" />
                                            <asp:HyperLink ID="lnkFormUrl" ClientIDMode="Static" runat="server" />
                                        </div>
                                    </div>
                                    <asp:HiddenField ID="hdFormId" ClientIDMode="Static" runat="server" />
                                </div>
                                <div class="modal-footer justify-content-between">
                                    <asp:Button OnClientClick="ToggleEditForm(false)" CssClass="btn btn-primary" ID="cmdSave" ValidationGroup="Form" runat="server" Text="Save" OnClick="cmdSave_Click" />
                                    <button type="button" class="btn btn-default" data-bs-dismiss="modal">Close</button>
                                </div>
                            </div>
                        </div>
                    </div>
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="cmdSave" EventName="Click" />
                </Triggers>
            </asp:UpdatePanel>

        </div>
    </div>
</div>

<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/DataTables/jquery.dataTables.min.js" />
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/DataTables/dataTables.bootstrap5.min.js" />
<dnn:DnnCssInclude runat="server" FilePath="/Resources/Libraries/DataTables/dataTables.bootstrap5.min.css" />

<script type="text/javascript">
    const uploadHandler = "<%=FormUploadHandler%>";
    const moduleId = <%=ModuleId%>;
    const tabId = <%=TabId%>;
    const isAdmin = "<%=IsAdmin%>";
    var extensionHash = {
        'pdf': 1,
        'doc': 1,
        'docx': 1,
        'xls': 1,
        'xlsx': 1,
    };
    (function ($, Sys) {
        $(document).ready(function () {
            Sys.Application.add_load(function (s, e) { PageInit(); });
            PageInit();
        });
    }(jQuery, window.Sys));
    function PageInit() {
        $("#EditFormModal").on("click", "#uplFile", function (e) {
            $("#upload-overlay").show();
        });
        $("#EditFormModal").on("change", "#uplFile", function (e) {
            check_extension($(this).val());
        });
        var table = $('#tblForm').DataTable({
            "order": [[1, "asc"]],
            "oLanguage": {
                "sSearch": "Filter by Text"
            },
            "aoColumns": [
                { "bSortable": false },
                { "bSortable": true },
                { "bSortable": false },
                { "bSortable": false },],
            autoWidth: true,
        });
        $("#tblForm_length").prepend('<button onclick="return ClearForm()" class="btn btn-primary btn-sm me-2" data-bs-toggle="modal" data-bs-target="#EditFormModal"><i class="fa fa-plus"></i>&nbsp;Add Form</button>');
        table.on('draw', function () {
            $(".confirm").dnnConfirm({
                text: 'Are you sure you wish to Delete the selected Form?',
                yesText: 'Yes',
                noText: 'No',
                title: 'Delete Form?'
            });
        });
        table.draw();
    }
    function ToggleEditForm(toggleValue) {
        if (toggleValue) {
            $('#EditFormModal').modal('show');
        } else {
            $('#EditFormModal').modal('hide');
            $('body').removeClass('modal-open');
            $('.modal-backdrop').remove();
        }
        return true;
    }
    function ClearForm() {
        $('#drpFileType').val("");
        $('#hdFormId').val("");
        return false;
    }
    function WriteAttachmentMessage(filename) {
        if (filename == "") {
            $("#uploadInfo").html("<span class='text-danger'>Unable to upload file. Please make sure the file is in an allowed format.</span>");
        } else {
            $("#fileAttachmentWarning").fadeOut();
            $("#uploadInfo").html("<div class='alert alert-warning mt-3'><i class='fas fa-file-arrow-up'></i> File Uploaded! Click Save to add to Database.</div>");
        }
        var upload = $("#uplFile");
        var html = upload.parent().html();
        upload.parent().html(html.replace(filename, "File Selected"));
    }
    function HandleUpload() {
        $("#upload-overlay").show();
        var upload = $("#uplFile");
        if (upload.is(':enabled')) {
            var fileUpload = $("#uplFile").get(0);
            var file = fileUpload.files[0];
            if (file.length == 0) {
                $("#uploadInfo").html("<span class='text-danger'>Please Choose a File!</span>");
                return false;
            }
            var filename = file.name;
            var data = new FormData();
            data.append(filename, file);
            data.append("mid",moduleId);
            data.append("tabId",tabId);
            var options = {};
            options.url = uploadHandler;
            options.type = "POST";
            options.data = data;
            options.contentType = false;
            options.processData = false;
            options.success = function (fileId) {
                $("#upload-overlay").hide();
                $("#hdFileId").val(fileId);
                $("#lnkFormUrl").attr("href", "");
                $("#lnkFormUrl").text("");
                WriteAttachmentMessage(filename);
            };
            options.error = function (err) {
                alert(err.statusText);
                setTimeout(function () {
                    $("#upload-overlay").hide();
                    $("#uploadInfo").html('');
                }, 1000);
            };
            $.ajax(options);
        }
    }

    function check_extension(filename) {
        var ext = filename.split('.').pop().toLowerCase();
        if (extensionHash[ext]) {
            $("#uploadInfo").html("");
            HandleUpload();
            return true;
        } else {
            $("#uploadInfo").html("<span class='text-danger'>Invalid File Type, please choose an allowed file type!</span>");
            return false;
        }
    }

    function ShowAlert(title, text) {
        $.dnnAlert({
            okText: 'OK',
            title: title,
            text: text
        });
    }
</script>
