<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Referral.ascx.cs" Inherits="tjc.Modules.JudicialReferral.Referral" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<div typeof="post" id="referral-form">
    <div class="alert alert-warning">
        <strong><em class="fa fa-warning"></em></strong>All fields marked with an asterisk (<em class="text-danger">*</em>) are required and
        must be filled in or this form will not be processed.
    </div>
    <div class="row form-group">
        <div class="col-md-6">
            <asp:Label runat="server" AssociatedControlID="txtCaseSequence" Text="Case Number<em>*</em>" ToolTip="required" />
            <div class="input-group">
                <asp:DropDownList ID="drpCounty" ClientIDMode="Static" runat="server" CssClass="form-control" required="required">
                    <asp:ListItem Text="< Select County >" Value=""></asp:ListItem>
                    <asp:ListItem Text="DeSoto" Value="D"></asp:ListItem>
                    <asp:ListItem Text="Manatee" Value="M"></asp:ListItem>
                    <asp:ListItem Text="Sarasota" Value="S"></asp:ListItem>
                    <asp:ListItem Text="Venice" Value="V"></asp:ListItem>
                </asp:DropDownList>
                <asp:TextBox ID="txtYear" ClientIDMode="Static" runat="server" MaxLength="4" CssClass="form-control col-2"></asp:TextBox>
                <asp:TextBox ID="txtCaseType" ClientIDMode="Static" runat="server" MaxLength="2" CssClass="form-control col-1 upperCase" placeholder="CC"></asp:TextBox>
                <asp:TextBox ID="txtCaseSequence" ClientIDMode="Static" runat="server" MaxLength="25" CssClass="form-control col-3 upperCase" placeholder="000000"></asp:TextBox>
                <div class="input-group-append">
                    <small class="input-group-text form-control" title="County-Year-Case Type-Case Sequence">(Format: C-YYYY-CT-<span id="caseFormat">000000</span>)</small>
                </div>
                <asp:RequiredFieldValidator runat="server" ControlToValidate="drpCounty" ErrorMessage="County is Required"
                    Display="Dynamic" SetFocusOnError="true" CssClass="label label-danger" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtYear"
                    Display="Dynamic" SetFocusOnError="true" CssClass="label label-danger" ErrorMessage="Year is Required" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtCaseType"
                    Display="Dynamic" SetFocusOnError="true" CssClass="label label-danger" ErrorMessage="Case Type is Required" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtCaseSequence"
                    Display="Dynamic" SetFocusOnError="true" CssClass="label label-danger" ErrorMessage="Case Sequence is Required" />
            </div>
        </div>
        <div class="col-md-6">
            <asp:Label runat="server" AssociatedControlID="txtCaseParties" Text="Case Name<em>*</em>" ToolTip="required" />
            <div class="input-group">
                <asp:TextBox ID="txtCaseParties" runat="server" ClientIDMode="Static" MaxLength="2000" CssClass="form-control" placeholder="Party One v. Party Two"></asp:TextBox>
                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtCaseParties"
                    Display="Dynamic" SetFocusOnError="true" CssClass="label label-danger" ErrorMessage="Case Name is Required" />

                <button id="lockFields" class="btn btn-default" title="Click to unlock fields"><i class="fas fa-lock-open"></i></button>
            </div>
            <div class="form-text">"Case Name" format should be first and last name (with <strong class="text-uppercase">no</strong> middle initials) - example "John Doe"</div>
        </div>
    </div>
    <div class="row form-group">
        <div class="col-md-3">
            <asp:Label runat="server" AssociatedControlID="drpJudge" Text="Judge<em>*</em>" ToolTip="required" />
            <asp:DropDownList ID="drpJudge" runat="server" ClientIDMode="Static" CssClass="form-control" required="required">
                <asp:ListItem Text="< Select Judge >" Value=""></asp:ListItem>
            </asp:DropDownList>
            <asp:RequiredFieldValidator runat="server" ControlToValidate="drpJudge" Display="Dynamic"
                CssClass="label label-danger" ErrorMessage="Please Select a Judge" SetFocusOnError="true" />
        </div>
        <div class="col-md-3">
            <asp:Label runat="server" AssociatedControlID="drpCaseType" Text="Select Case Type<em>*</em>" ToolTip="required" />
            <asp:DropDownList ID="drpCaseType" runat="server" ClientIDMode="Static" CssClass="form-control">
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
        <div class="col-md-4">
            <asp:Label runat="server" AssociatedControlID="txtMotionTitle" Text="Motion Title<em>*</em>" ToolTip="required" />
            <asp:TextBox ID="txtMotionTitle" runat="server" ClientIDMode="Static" MaxLength="50" CssClass="form-control"></asp:TextBox>
            <asp:RequiredFieldValidator runat="server" ControlToValidate="txtMotionTitle"
                Display="Dynamic" SetFocusOnError="true" CssClass="label label-danger" ErrorMessage="Motion Title is Required" />
        </div>
        <div class="col-md-2">
            <asp:Label runat="server" AssociatedControlID="txtMotionDate" Text="Motion Date<em>*</em>" ToolTip="required" />
            <asp:TextBox runat="server" ClientIDMode="Static" CssClass="form-control datepicker" ID="txtMotionDate" />
            <asp:RequiredFieldValidator runat="server" ControlToValidate="txtMotionDate"
                Display="Dynamic" SetFocusOnError="true" CssClass="label label-danger" ErrorMessage="Motion Date is Required" />
        </div>
    </div>
    <p class="alert alert-info mt-2"><i class="fa fa-info-circle"></i>Drag and Drop files to the box below. Acceptable file types are .docx, .doc, .xls, .xlsx, and .pdf</p>
    <div class="row">
        <div class="col-md-6">
            <fieldset class="fieldset-border">
                <legend>File Upload</legend>
                <div>
                    <label for="fileselect">Files to upload:</label>
                    <input type="file" id="fileselect" name="fileselect[]" multiple="multiple" />
                    <div id="filedrag">or drop files here</div>
                </div>

            </fieldset>
        </div>
        <div class="col-md-6">
            <div id="messages">
            </div>
            <div id="progress"></div>
        </div>
    </div>
    <asp:HiddenField ID="hdAttachmentIds" runat="server" />
    <div class="mt-3">
        <div class="form-check mb-2">
            <asp:CheckBox ID="chkMotionVacate" ClientIDMode="Static" runat="server" Text="<strong>3.850</strong> Motion to Vacate, Set Aside, or Correct Sentence: Court Counsel will assist with all 3.850 motions. If the Motion is not facially sufficient, a proposed order striking the motion will be provided to the judge. If the Motion is facially sufficient for legal review, the judicial assistant will be prompted to send an Acknowledgment of the Motion to the defendant, copying the State and Clerk. Unless the Court is able to dismiss all claims as legally deficient, the State will be ordered to respond within 60 days." TextAlign="Right" />
        </div>
        <div class="form-check mb-2">
            <asp:CheckBox ID="chkMotionCorrect" ClientIDMode="Static" runat="server" Text="<strong>3.800(b)</strong> Motion to Correct Sentencing Error: The Court must rule on this motion within 60 days or it is deemed denied. Unless directed otherwise by the court below, this motion shall be handled directly by the judge and court counsel need not take any action." TextAlign="Right" />
        </div>
        <div class="form-check mb-2">
            <asp:CheckBox ID="chkMotionDirected" ClientIDMode="Static" runat="server" Text="Unless directed by the court below, these motions shall be handled directly by the presiding judge unless the complexity of the issue warrants further assistance by Court Counsel:" TextAlign="Right" />
        </div>
        <div class="form-check mb-2">
            <asp:CheckBoxList ID="clsMotionList" ClientIDMode="Static" runat="server" CssClass="motion-list list-unstyled" RepeatDirection="Vertical" RepeatLayout="UnorderedList">
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
            <asp:CheckBox ID="chkMotionOther" ClientIDMode="Static" runat="server" Text="All other motions: Court Counsel will assist with all other motions, as referred by the presiding judge." TextAlign="Right" />
        </div>
        <asp:CustomValidator Display="Dynamic" SetFocusOnError="true" CssClass="label label-danger" ID="valMotionCheck" runat="server" ErrorMessage="You Must Select at least one of the four options"
            ClientValidationFunction="MotionCheck"></asp:CustomValidator>
    </div>
    <hr />
    <p class="mt-2">
        <asp:LinkButton ID="cmdSave" runat="server" CssClass="btn btn-primary mr-md" Text="Submit to Judge" OnClick="cmdSave_Click" />
        <asp:HyperLink ID="cmdCancel" runat="server" CssClass="btn btn-secondary" Text="Cancel" />
    </p>

</div>
<div id="process-overlay" class="overlay" style="display: none;">
    <div class="spinner"></div>
</div>
<div class="modal fade" id="caseListModal" tabindex="-1" role="dialog" aria-labelledby="caseListModalLabel" aria-hidden="true">
    <div class="modal-dialog">
        <div class="modal-content">
            <div class="modal-header">
                <h4 class="modal-title" id="caseListModalLabel">Matching Case Number Results</h4>
                <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
            </div>
            <div class="modal-body">
                <table id="caseList" class="table table-striped">
                    <thead>
                        <tr>
                            <th class="command-icon">&nbsp;</th>
                            <th>Case Number</th>
                            <th>Case Name</th>
                        </tr>
                    </thead>
                    <tbody id="caseListBody">
                    </tbody>
                </table>
            </div>
            <div class="modal-footer">
                <button type="button" class="btn btn-caseList" data-dismiss="modal">Close</button>
            </div>
        </div>
    </div>
</div>
<dnn:DnnCssInclude runat="server" FilePath="~/Resources/Shared/components/TimePicker/Themes/jquery-ui.min.css" />
<dnn:dnncssinclude runat="server" filepath="/Resources/Libraries/DataTables/dataTables.bootstrap5.min.css" />
<dnn:dnnjsinclude runat="server" filepath="/Resources/Libraries/DataTables/dataTables.min.js" />
<dnn:dnnjsinclude runat="server" filepath="/Resources/Libraries/DataTables/dataTables.bootstrap5.min.js" />
<dnn:dnnjsinclude runat="server" filepath="/Resources/Libraries/jQuery/jquery.mask.js" />
<script type="text/javascript">
    var ccModuleId = <%=ModuleId%>;
    var link = document.getElementById("<%=cmdSave.ClientID %>");
    var totalFiles = 0;
    var county = "";
    document.addEventListener('click', function (e) {
        if (e.target.id === link.id) {
            if (document.getElementById("<%=cmdSave.ClientID %>").disabled)
                e.preventDefault();
        }
    });
    $(document).ready(function () {
        PageInit();
    });
    function PageInit() {
        $("#lockFields").hide();
        $("#lockFields").on("click", function (e) {
            e.preventDefault();
            EnableFields();
        });
        $(".datepicker").datepicker();
        $(".form-check input:checkbox").addClass("form-check-input");
        $(".form-check label").addClass("form-check-label");
        if (typeof (window.FileReader) == 'undefined') {
            alert('Browser does not support HTML5 file uploads!');
        }
        if ($("#txtCaseSequence").val() == "") {
            $("#txtCaseSequence").mask("000000");
        }
        $("#drpCounty").on("change", function () {
            county = $(this).val();
            PreValidateCaseNumber();
        });
        $(".upperCase").on("input", function (evt) {
            $(this).val(function (_, val) {
                return val.toUpperCase();
            });
        });
        $(document).on('click', '.delete-item', function (e) {
            e.preventDefault();
            var aid = $(this).data("aid");
            DeleteFile(aid);
        });
        $("#txtYear").on("blur", function () {
            PreValidateCaseNumber();
        });
        $("#txtCaseType").on("blur", function () {
            PreValidateCaseNumber();
        });
        $("#txtCaseSequence").on("blur", function () {
          var  caseSequence = $(this).val().padStart(6, '0');
            $("#txtCaseSequence").val(caseSequence);
            PreValidateCaseNumber();
        });
        $(document).on("click", ".case-select", function (e) {
            e.preventDefault();
            var dataElement = $(this);
            var obj = { "logId": dataElement.data("logid"), "caseNumber": dataElement.data("casenumber"), "countyId": dataElement.data("countyid"), "description": dataElement.data("desc") };
            PopulateCaseInformation(obj);
            $('#caseListModal').modal('hide');
        });
    }
    function EnableFields() {
        $('#txtCaseSequence').prop("disabled", false);
        $("#txtCaseType").prop("disabled", false);
        $("#drpCounty").prop("disabled", false);
        $("#txtYear").prop("disabled", false);
        $("#txtCaseParties").prop("disabled", false);
        $("#lockFields").hide();

    }
    function DisableButton() {
        $("#cmdSave").prop('disabled', true);
        $("#cmdSave").html("Processing...");
        setTimeout(() => {
            $("#cmdSave").prop('disabled', false);
            $("#cmdSave").html("Save");
        }, "3000");
    }
    window.onbeforeunload = DisableButton;

    function MotionCheck(sender, args) {
        args.IsValid = false;
        var chkMotionVacate = $('#chkMotionVacate').is(':checked');
        var chkMotionCorrect = $('#chkMotionCorrect').is(':checked');
        var chkMotionDirected = $('#chkMotionDirected').is(':checked');
        var chkMotionOther = $('#chkMotionOther').is(':checked');
        if (chkMotionVacate | chkMotionCorrect | chkMotionDirected | chkMotionOther) {
            args.IsValid = true;
            return;
        }
    }
    function DirectedMotionCheck(sender, args) {
        args.IsValid = true;
        var chkMotionDirected = $('#chkMotionDirected').is(':checked');
        var radioButtons = $('#clsMotionList');
        if (chkMotionDirected) {
            var found = radioButtons.find('input:checked');
            if (found.length === 0) {
                args.IsValid = false;
                return;
            }
        }

    }
    const validExtensions = ['pdf', 'xls', 'xlsx', 'docx', 'doc'];

    function $id(id) {
        return document.getElementById(id);
    }

    // output information
    function Output(msg) {
        var m = $id("messages");
        m.innerHTML = msg + m.innerHTML;
    }

    // file drag hover
    function FileDragHover(e) {
        e.stopPropagation();
        e.preventDefault();
        e.target.className = (e.type == "dragover" ? "hover" : "");
    }
    function FileSelectHandler(e) {

        // cancel event and hover styling
        FileDragHover(e);

        // fetch FileList object
        var files = e.target.files || e.dataTransfer.files;

        // process all File objects
        for (var i = 0, f; f = files[i]; i++) {
            if (ValidateFile(f)) {
                // ParseFile(f);
                UploadFile(f);
            }
        }
    }

    function ValidateFile(file) {
        if (CheckExtension(file.name) && CheckFileSize(file)) {
            return true;
        }
        return false;
    }
    function getFileExtension(filename) {
        const match = filename.match(/\.([^.]+)$/);
        return match ? match[1] : null;
    }
    function isExtensionValid(filename) {
        const extension = getFileExtension(filename);
        return extension ? validExtensions.includes(extension.toLowerCase()) : false;
    }
    function CheckExtension(filename) {

        if (isExtensionValid(filename)) {
            return true;
        } else {
            Output("<p class='alert alert-danger'><i class='fa fa-warning'></i> " + filename + " has an invalid file type, please choose a document with a doc, docx, xsl, xslx, or pdf extension!</p>");
            return false;
        }
    }
    function CheckFileSize(file) {
        var size = "<%=MaxRequestLength%>";
        if (file.size <= size) { return true; } else {
            Output("<p class='alert alert-danger'><i class='fa fa-warning'></i> " + file.name + " is larger than the Maximum file size of <%=MaxFileSize%>!</p>");
        }
    }
    // file selection Functions
    //Delete File
    function DeleteFile(aid) {
        // URL of the ASHX handler with a query string parameter
        var url = `<%=TemplateSourceDirectory %>/FileHandler.ashx?aid=${aid}`;

        // Call the ASHX handler using fetch
        fetch(url)
            .then(response => {
                if (!response.ok) {
                    throw new Error('Unable to connect to server');
                }
                return response.text();
            })
            .then(data => {
                var deleteItem = $('[data-aid="' + aid + '"]').parent();
                deleteItem.remove();
            })
            .catch(error => {
                alert('There was a problem when deleting the file:', error);
            });
    }
    // upload files
    function UploadFile(file) {
        var data = new FormData();
        data.append("mid",<%=ModuleId%>);
        data.append("tid",<%=TabId%>);
        data.append(file.name, file);
        var xhr = new XMLHttpRequest();
        if (xhr.upload && file.size <= <%=MaxRequestLength%>) {

            // create progress bar
            var o = $id("progress");
            var progress = o.appendChild(document.createElement("p"));
            progress.appendChild(document.createTextNode(file.name));
            // progress bar
            xhr.upload.addEventListener("progress", function (e) {
                var pc = parseInt(100 - (e.loaded / e.total * 100));
                progress.style.backgroundPosition = pc + "% 0";
            }, false);
            // file received/failed
            xhr.onreadystatechange = function (e) {
                if (xhr.readyState == 4) {
                    progress.className = (xhr.status == 200 ? "success" : "failure");
                    var data = xhr.responseText;
                    var jsonResponse = JSON.parse(data);
                    if (jsonResponse.idList.length > 0) {
                        var newId = String(jsonResponse.idList);
                        if (xhr.status == 200) {
                            var deleteItem = progress.appendChild(document.createElement("i"));
                            deleteItem.className = "fa fa-trash delete-item float-end text-dark mt-1";
                            deleteItem.setAttribute('data-aid', newId);
                        }
                        var oldvalue = $("#<%=hdAttachmentIds.ClientID%>").val();
                        if (oldvalue != "") { newId = oldvalue + "," + newId; }
                        $("#<%=hdAttachmentIds.ClientID%>").val(newId);
                    }
                }
            };
            // start upload
            xhr.open("POST", "<%=TemplateSourceDirectory %>/FileHandler.ashx", true);
            xhr.setRequestHeader("X-FILENAME", file.name);
            xhr.send(data);

        }

    }
    // initialize
    function Init() {

        var fileselect = $id("fileselect"),
            filedrag = $id("filedrag");

        // file select
        fileselect.addEventListener("change", FileSelectHandler, false);

        // is XHR2 available?
        var xhr = new XMLHttpRequest();
        if (xhr.upload) {

            // file drop
            filedrag.addEventListener("dragover", FileDragHover, false);
            filedrag.addEventListener("dragleave", FileDragHover, false);
            filedrag.addEventListener("drop", FileSelectHandler, false);
            filedrag.style.display = "block";
        }

    }
    // call initialization file
    if (window.File && window.FileList && window.FileReader) {
        Init();
    }
    function PreValidateCaseNumber(sender, args) {
        var errorMessage = "";
        var caseCounty = $("#drpCounty").val();
        var caseYear = $("#txtYear").val();
        var caseType = $("#txtCaseType").val();
        var caseSequence = $("#txtCaseSequence").val();
        if (caseCounty != "" && caseCounty != "" && caseYear != "" && caseType != "" && caseSequence != "") {
            $('#process-overlay').show();
            var caseNumber = GetCaseNumber();
            RetrieveLogEntryByCaseNumber(caseNumber);
        }
    }
    function GetCaseNumber() {
        var caseCounty = $("#drpCounty").val();
        var caseYear = $("#txtYear").val();
        var caseType = $("#txtCaseType").val();
        var caseSequence = $("#txtCaseSequence").val();
        return caseCounty + "-" + caseYear + "-" + caseType + "-" + caseSequence;
    }
    function RetrieveLogEntryByCaseNumber(caseNumber) {
        var service = {
            path: "CourtCounsel",
            framework: $.ServicesFramework(ccModuleId)
        }
        service.baseUrl = service.framework.getServiceRoot(service.path);
        var restUrl = `${service.baseUrl}LogEntry/GetLogEntryByCaseNumber/${caseNumber}`;
        try {

            $.ajax({
                url: restUrl,
                beforeSend: service.framework.setModuleHeaders,
                dataType: "json"
            }).done(function (data) {
                if (data) {
                    if (data.length > 0) {
                        if (data.length == 1) {
                            PopulateCaseInformation(data[0]);
                        } else {
                            PopulateCaseList(data);
                        }
                    } else {
                        // ShowAlert("No Search Results", "The selected search criteria did not yeild any results. Please change your search request and try again");//No Case Found
                    }
                }
                else {
                    // ShowAlert("No Search Results", "The selected search criteria did not yeild any results. Please change your search request and try again");//No Case Found
                }
            }).always(function (data) {
                $('#process-overlay').hide();
            });
        } catch (e) {
            ShowAlert("Error Validating Case Number!!", "Unexpected error searching for case number.\n\nMake sure you are logged in and try again");//No Case Found
        }
        return false;
    }
    function PopulateCaseList(data) {
        var tableBody = document.getElementById('caseListBody');
        $('#caseList > tbody > tr').remove();
        data.forEach(function (object) {
            var tr = document.createElement('tr');
            tr.innerHTML = `<td><a class="command-icon case-select" title="Select This Log Entry" data-logId="${object.logId}" data-caseNumber="${object.caseNumber}" data-countyId="${object.countyId}" data-desc="${object.description}"><i class="fa fa-check-circle"></i></a></td><td>${object.caseNumber}</td><td>${object.description}</td>`;
            tableBody.appendChild(tr);
        });
        $('#caseListModal').modal('show');
    }
    function PopulateCaseInformation(data) {
        $("#lockFields").show();
        $('#txtCaseSequence').prop("disabled", true);
        $("#txtCaseType").prop("disabled", true);
        $("#txtYear").prop("disabled", true);
        $("#txtCaseParties").prop("disabled", true);
        const countyValue = data.caseNumber.charAt(0);
        $("#drpCounty").val(countyValue).prop("disabled", true);
        $("#txtCaseParties").val(data.description).prop("disabled", true);
        $("#hdLogId").val(data.logId);
    }
    function ShowAlert(title, text) {
        $('#process-overlay').hide();
        $.dnnAlert({
            okText: 'OK',
            title: title,
            text: text
        });
    }
</script>
