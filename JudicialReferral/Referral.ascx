<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Referral.ascx.cs" Inherits="tjc.Modules.JudicialReferral.Referral" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<div typeof="post" id="referral-form">
    <div class="alert alert-warning">
        <strong><em class="fa fa-warning"></em></strong>All fields marked with an asterisk (<em class="text-danger">*</em>) are required and
        must be filled in or this form will not be processed.
    </div>
    <div class="row">
        <div class="col-md-6">
            <asp:Label runat="server" AssociatedControlID="drpJudge" Text="Judge<em>*</em>" ToolTip="required" />
            <asp:DropDownList ID="drpJudge" runat="server" CssClass="form-control" required="required">
                <asp:ListItem Text="< Select Judge >" Value=""></asp:ListItem>
            </asp:DropDownList>
            <asp:RequiredFieldValidator runat="server" ControlToValidate="drpJudge" Display="Dynamic"
                CssClass="label label-danger" ErrorMessage="Please Select a Judge" SetFocusOnError="true" />
        </div>
        <div class="col-md-6">
            <asp:Label runat="server" AssociatedControlID="drpCounty" Text="County<em>*</em>" ToolTip="required" />
            <asp:DropDownList ID="drpCounty" runat="server" CssClass="form-control" required="required">
                <asp:ListItem Text="< Select County >" Value=""></asp:ListItem>
                <asp:ListItem Text="DeSoto" Value="D"></asp:ListItem>
                <asp:ListItem Text="Manatee" Value="M"></asp:ListItem>
                <asp:ListItem Text="Sarasota" Value="S"></asp:ListItem>
                <asp:ListItem Text="Venice" Value="V"></asp:ListItem>
            </asp:DropDownList>
            <asp:RequiredFieldValidator runat="server" ControlToValidate="drpCounty" ErrorMessage="County is Required"
                Display="Dynamic" SetFocusOnError="true" CssClass="label label-danger" />
        </div>
    </div>
    <div class="row">
        <div class="col-md-6">
            <asp:Label runat="server" AssociatedControlID="txtCaseNumber" Text="Case Number<em>*</em>" ToolTip="required" />
            <div class="input-group">
                <asp:TextBox ID="txtYear" runat="server" MaxLength="4" CssClass="form-control col-2"></asp:TextBox>
                <asp:TextBox ID="txtCaseType" runat="server" MaxLength="2" CssClass="form-control col-1" placeholder="CC"></asp:TextBox>
                <asp:TextBox ID="txtCaseNumber" runat="server" MaxLength="25" CssClass="form-control col-3" placeholder="012345"></asp:TextBox>
                <div class="input-group-append">
                    <small class="input-group-text form-control">(Format: 2022 CC 012345)</small>
                </div>
                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtYear"
                    Display="Dynamic" SetFocusOnError="true" CssClass="label label-danger" ErrorMessage="Year is Required" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtCaseType"
                    Display="Dynamic" SetFocusOnError="true" CssClass="label label-danger" ErrorMessage="Case Type is Required" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtCaseNumber"
                    Display="Dynamic" SetFocusOnError="true" CssClass="label label-danger" ErrorMessage="Case Number is Required" />
            </div>
        </div>
        <div class="col-md-6">
            <asp:Label runat="server" AssociatedControlID="drpCaseType" Text="Select Case Type<em>*</em>" ToolTip="required" />
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
    <div class="row">
        <div class="col-md-5">
            <asp:Label runat="server" AssociatedControlID="txtCaseParties" Text="Case Name<em>*</em>" ToolTip="required" />
            <asp:TextBox ID="txtCaseParties" runat="server" MaxLength="2000" CssClass="form-control" placeholder="Party One v. Party Two"></asp:TextBox>
            <asp:RequiredFieldValidator runat="server" ControlToValidate="txtCaseParties"
                Display="Dynamic" SetFocusOnError="true" CssClass="label label-danger" ErrorMessage="Case Name is Required" />
        </div>
        <div class="col-md-4">
            <asp:Label runat="server" AssociatedControlID="txtMotionTitle" Text="Motion Title<em>*</em>" ToolTip="required" />
            <asp:TextBox ID="txtMotionTitle" runat="server" MaxLength="50" CssClass="form-control"></asp:TextBox>
            <asp:RequiredFieldValidator runat="server" ControlToValidate="txtMotionTitle"
                Display="Dynamic" SetFocusOnError="true" CssClass="label label-danger" ErrorMessage="Motion Title is Required" />
        </div>
        <div class="col-md-3">

            <asp:Label runat="server" AssociatedControlID="txtMotionDate" Text="Motion Date<em>*</em>" ToolTip="required" />
            <asp:TextBox runat="server" CssClass="form-control datepicker" ID="txtMotionDate" />
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
    <asp:CustomValidator Display="Dynamic" SetFocusOnError="true" CssClass="label label-danger" ID="CustomValidator1" runat="server" ErrorMessage="You must select at least one of the seven options"
        ClientValidationFunction="DirectedMotionCheck"></asp:CustomValidator>

    <div class="mt-3">
        <div class="form-check mb-2">
            <asp:CheckBox ID="chkMotionVacate" runat="server" Text="<strong>3.850</strong> Motion to Vacate, Set Aside, or Correct Sentence: Court Counsel will assist with all 3.850 motions. If the Motion is not facially sufficient, a proposed order striking the motion will be provided to the judge. If the Motion is facially sufficient for legal review, the judicial assistant will be prompted to send an Acknowledgment of the Motion to the defendant, copying the State and Clerk. Unless the Court is able to dismiss all claims as legally deficient, the State will be ordered to respond within 60 days." TextAlign="Right" />
        </div>
        <div class="form-check mb-2">
            <asp:CheckBox ID="chkMotionCorrect" runat="server" Text="<strong>3.800(b)</strong> Motion to Correct Sentencing Error: The Court must rule on this motion within 60 days or it is deemed denied. Unless directed otherwise by the court below, this motion shall be handled directly by the judge and court counsel need not take any action." TextAlign="Right" />
        </div>
        <div class="form-check mb-2">
            <asp:CheckBox ID="chkMotionDirected" runat="server" Text="Unless directed by the court below, these motions shall be handled directly by the presiding judge unless the complexity of the issue warrants further assistance by Court Counsel:" TextAlign="Right" />
        </div>
        <div class="form-check mb-2">
            <asp:CheckBoxList ID="clsMotionList" runat="server" CssClass="motion-list list-unstyled" RepeatDirection="Vertical" RepeatLayout="UnorderedList">
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
    <hr />
    <p class="mt-2">
        <asp:LinkButton ID="cmdSave" runat="server" CssClass="btn btn-primary mr-md" Text="Submit to Judge" OnClick="cmdSave_Click" />
        <asp:HyperLink ID="cmdCancel" runat="server" CssClass="btn btn-secondary" Text="Cancel" />
    </p>

</div>
<dnn:DnnCssInclude runat="server" FilePath="~/Resources/Shared/components/TimePicker/Themes/jquery-ui.min.css" />


<script type="text/javascript">
    var link = document.getElementById("<%=cmdSave.ClientID %>");
    var totalFiles = 0;
    document.addEventListener('click', function (e) {
        if (e.target.id === link.id) {
            if (document.getElementById("<%=cmdSave.ClientID %>").disabled)
                e.preventDefault();
        }
    });
    $(document).ready(function () {
        // this function runs when the page loads to set up the drop area

        // Check if window.fileReader exists to make sure the browser
        // supports file uploads
        $(".datepicker").datepicker();
        $(".form-check input:checkbox").addClass("form-check-input");
        $(".form-check label").addClass("form-check-label");
        if (typeof (window.FileReader) == 'undefined') {
            alert('Browser does not support HTML5 file uploads!');
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
    var extensionHash = {
        '.pdf': 1,
        '.xls': 1,
        '.xlsx': 1,
        '.docx': 1,
        '.doc': 1,
    };
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
    function CheckExtension(filename) {
        var re = /\..+$/;
        var ext = filename.match(re);
        if (extensionHash[ext]) {
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

    // file selection
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
</script>
