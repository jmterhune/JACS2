<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Edit.ascx.cs" Inherits="tjc.Modules.ThreatReport.Edit" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<%-- SweetAlert2 + Noty for confirms / toast notifications --%>
<dnn:DnnCssInclude runat="server" FilePath="/Resources/Libraries/sweetalert/sweetalert2.min.css" />
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/sweetalert/sweetalert2.all.min.js" />
<dnn:DnnCssInclude runat="server" FilePath="/Resources/Libraries/Noty/noty.min.css" />
<dnn:DnnCssInclude runat="server" FilePath="/Resources/Libraries/Noty/bootstrap-v4.min.css" />
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/Noty/noty.min.js" />

<div type="post" id="ThreatReport">
    <div class="alert alert-warning">
        <em class="fa fa-warning"></em>All fields marked with an asterisk (<em class="lbl">*</em>) are required and
        must be filled in or this form will not be processed.
    </div>
    <fieldset>
        <legend>Person Making This Report</legend>
        <div class="form-group">
            <div class="row">
                <div class="col-md-6">
                    <asp:Label Text="Person Making Report<em>*</em>" runat="server" AssociatedControlID="txtPersonReporting" />
                    <asp:TextBox ID="txtPersonReporting" runat="server" MaxLength="50" CssClass="form-control" />
                    <asp:RequiredFieldValidator ID="valPersonReporting" runat="server" SetFocusOnError="true" CssClass="label label-danger"
                        Display="Dynamic" ErrorMessage="Person Making Report is Required" ControlToValidate="txtPersonReporting" ValidationGroup="incident" />
                </div>
                <div class="col-md-6">
                    <asp:Label Text="Date of Report<em>*</em>" runat="server" AssociatedControlID="txtDateReported" />
                    <asp:TextBox ID="txtDateReported" runat="server" TextMode="DateTime" MaxLength="50" CssClass="form-control" />
                    <asp:RequiredFieldValidator ID="valDateReported" runat="server" SetFocusOnError="true" CssClass="label label-danger"
                        Display="Dynamic" ErrorMessage="Date is Required" ControlToValidate="txtDateReported" ValidationGroup="incident" />
                    <asp:CompareValidator Display="Dynamic" ID="valDateReportedDateType" runat="server" Type="Date" CssClass="label label-danger" Operator="DataTypeCheck"
                        ControlToValidate="txtDateReported" ErrorMessage="Please enter a valid date." ValidationGroup="incident">
                    </asp:CompareValidator>
                </div>

            </div>
        </div>
        <div class="form-group">
            <div class="row">
                <div class="col-md-4">
                    <asp:Label Text="Phone" runat="server" AssociatedControlID="txtPersonReportingPhone" />
                    <asp:TextBox ID="txtPersonReportingPhone" TextMode="Phone" runat="server" MaxLength="50" CssClass="form-control phone_us" placeholder="(555) 555-5555" />
                </div>
                <div class="col-md-2">
                    <asp:Label Text="Extension" runat="server" AssociatedControlID="txtPersonReportingExtension" />
                    <asp:TextBox ID="txtPersonReportingExtension" runat="server" MaxLength="10" CssClass="form-control" />

                </div>
                <div class="col-md-6">
                    <asp:Label Text="Email" runat="server" AssociatedControlID="txtPersonReportingEmail" />
                    <asp:TextBox ID="txtPersonReportingEmail" runat="server" TextMode="Email" MaxLength="250" CssClass="form-control" />
                </div>

            </div>
        </div>

    </fieldset>
    <fieldset>
        <legend>Location of Incident<em class="text-danger">*</em></legend>
        <div class="form-group">
            <div class="row">
                <div class="col-md-12">
                    <asp:RadioButtonList runat="server" ID="rblLocation" CssClass="dnnFormRadioButtons" RepeatLayout="Table" RepeatColumns="3">
                        <asp:ListItem Text="Manatee County Judicial Center" />
                        <asp:ListItem Text="Manatee County Historical Courthouse" />
                        <asp:ListItem Text="Silvertooth Judicial Center" />
                        <asp:ListItem Text="Sarasota CJC" />
                        <asp:ListItem Text="RLA (Venice Courthouse)" />
                        <asp:ListItem Text="DeSoto County Courthouse" />
                    </asp:RadioButtonList>
                    <asp:RequiredFieldValidator ID="valLocation" runat="server" SetFocusOnError="true" CssClass="label label-danger"
                        Display="Dynamic" ErrorMessage="Location is Required" ControlToValidate="rblLocation" ValidationGroup="incident" />
                </div>
            </div>
        </div>
    </fieldset>
    <fieldset>
        <legend>Nature of Incident</legend>

        <div class="form-group">
            <div class="row">
                <div class="col-md-6">
                    <asp:Label Text="Date of Incident<em>*</em>" runat="server" AssociatedControlID="txtDate" />
                    <asp:TextBox ID="txtDate" runat="server" TextMode="DateTime" MaxLength="50" CssClass="form-control" />
                    <asp:RequiredFieldValidator ID="valDate" runat="server" SetFocusOnError="true" CssClass="label label-danger"
                        Display="Dynamic" ErrorMessage="Date is Required" ControlToValidate="txtDate" ValidationGroup="incident" />
                    <asp:CompareValidator
                        ID="valIsDate" runat="server"
                        Type="Date" CssClass="label label-danger"
                        Operator="DataTypeCheck"
                        ControlToValidate="txtDate"
                        ErrorMessage="Please enter a valid date."
                        ValidationGroup="incident" Display="Dynamic">
                    </asp:CompareValidator>
                </div>

            </div>
        </div>
        <div class="form-group">
            <div class="row">
                <div class="col-md-12">
                    <asp:Label Text="Nature of Incident<em>*</em>" runat="server" AssociatedControlID="rblIncidentNature" />
                    <asp:RadioButtonList runat="server" ID="rblIncidentNature" CssClass="dnnFormRadioButtons" RepeatLayout="Flow" RepeatDirection="Horizontal">
                        <asp:ListItem Text="Threat by mail" />
                        <asp:ListItem Text="Verbal threat live" />
                        <asp:ListItem Text="Verbal threat phone" />
                        <asp:ListItem Text="Physical Altercation" />
                        <asp:ListItem Text="Concealed Weapon" />
                        <asp:ListItem Text="Other" />
                    </asp:RadioButtonList>
                    <asp:RequiredFieldValidator ID="valIncidentNature" runat="server" SetFocusOnError="true" CssClass="label label-danger"
                        Display="Dynamic" ErrorMessage="Nature of Incident is required" ControlToValidate="rblIncidentNature" ValidationGroup="incident" />

                </div>

            </div>
        </div>
        <div class="form-group">
            <div class="row">
                <div class="col-md-12">
                    <asp:Label Text="Briefly describe the incident<em>*</em>" runat="server" AssociatedControlID="txtIncidentDescription" />
                    <asp:TextBox ID="txtIncidentDescription" runat="server" TextMode="MultiLine" Rows="5" CssClass="form-control" />
                    <asp:RequiredFieldValidator ID="valIncidentDescription" runat="server" SetFocusOnError="true" CssClass="label label-danger"
                        Display="Dynamic" ErrorMessage="Description of Incident is Required" ControlToValidate="txtIncidentDescription" ValidationGroup="incident" />

                </div>
            </div>
        </div>
        <div class="form-group">
            <div class="row">
                <div class="col-md-6">
                    <asp:Label Text="If a specific person was targeted, indicate their name" runat="server" AssociatedControlID="txtPersonTargeted" />
                    <asp:TextBox ID="txtPersonTargeted" CssClass="form-control" runat="server" MaxLength="50" />

                </div>
                <div class="col-md-6">
                    <div class="form-control mt-5g">
                        <div class="form-check form-check-inline">
                            <asp:CheckBox Text="Court Employee?" runat="server" ID="chkCourtEmployee" class="check" />
                        </div>
                        <div class="form-check form-check-inline">
                            <asp:CheckBox Text="Target Notified?" runat="server" ID="chkTargetNotified" class="check" />
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </fieldset>
    <fieldset class="mb-4">
        <legend>Suspect's Information, If Known</legend>
        <p class="alert alert-info"><em class="fa fa-info-circle"></em>Use the Add Person button to enter individual details</p>
        <asp:UpdatePanel runat="server" ID="updatePersonsInvolved" RenderMode="Block" UpdateMode="Conditional">
            <ContentTemplate>
                <asp:Repeater ID="rptPersonsInvolved" runat="server" OnItemCommand="rptPersonsInvolved_ItemCommand" OnItemCreated="rptPersonsInvolved_ItemCreated">
                    <HeaderTemplate>
                        <div class="accordion mb-2" id="accordion">
                    </HeaderTemplate>

                    <ItemTemplate>
                        <div class="card card-default">
                            <div class="card-header">
                                <h4 class="card-title">
                                    <a class="accordion-toggle collapsed d-inline-block" data-toggle="collapse" data-parent="#accordion" aria-expanded="false" href="<%#"#Suspect-" + Container.ItemIndex + 1 %>"><%#DataBinder.Eval(Container.DataItem, "FirstName")%> <%#DataBinder.Eval(Container.DataItem, "LastName")%>&nbsp;<small>(click to expand)</small> </a>
                                    <asp:LinkButton ID="cmdDelete" runat="server" CssClass="float-end text-danger" CommandName="delete" OnClientClick="return Jud12ConfirmPostback(this, 'Delete this Person?', 'Delete?');" CommandArgument='<%#Container.ItemIndex.ToString() %>'><i class="fas fa-trash"></i></asp:LinkButton></td>

                                </h4>
                            </div>
                            <div id="<%#"Suspect-" + Container.ItemIndex + 1 %>" class="accordion-body collapse">
                                <div class="card-body container">
                                    <div class="row">
                                        <div class="form-group">

                                            <div class="col-md-3">
                                                <label class="d-block">
                                                    Date of Birth
                                        <input type="text" readonly class="form-control" value="<%#DataBinder.Eval(Container.DataItem, "DateOfBirth", "{0:MM/dd/yyyy}")%>"></label>
                                            </div>
                                            <div class="col-md-3">
                                                <label class="d-block">
                                                    Gender
                                        <input type="text" readonly class="form-control" value="<%#DataBinder.Eval(Container.DataItem, "Gender")%>"></label>
                                            </div>
                                            <div class="col-md-3">
                                                <label class="d-block">
                                                    Race
                                        <input type="text" readonly class="form-control" value="<%#DataBinder.Eval(Container.DataItem, "Race")%>"></label>
                                            </div>
                                            <div class="col-md-3">
                                                <label class="d-block">
                                                    Hair Color
                                        <input type="text" readonly class="form-control" value="<%#DataBinder.Eval(Container.DataItem, "HairColor")%>"></label>
                                            </div>

                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="form-group">
                                            <div class="col-md-2">
                                                <label class="d-block">
                                                    Height
                                        <input type="text" readonly class="form-control" value="<%#DataBinder.Eval(Container.DataItem, "Height")%>"></label>
                                            </div>
                                            <div class="col-md-2">
                                                <label class="d-block">
                                                    Weight
                                        <input type="text" readonly class="form-control" value="<%#DataBinder.Eval(Container.DataItem, "Weight")%>"></label>
                                            </div>
                                            <div class="col-md-3">
                                                <label class="d-block">
                                                    Phone
                                        <input type="text" readonly class="form-control phone_us" value="<%#DataBinder.Eval(Container.DataItem, "Phone")%>"></label>
                                            </div>
                                            <div class="col-md-3">
                                                <label class="d-block">
                                                    Voice (accent, slang, speech)
                                        <input type="text" readonly class="form-control" value="<%#DataBinder.Eval(Container.DataItem, "Voice")%>"></label>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="form-group">
                                            <div class="col-md-12">
                                                <label class="d-block">
                                                    Vehicle Info
                                        <input type="text" readonly class="form-control" value="<%#DataBinder.Eval(Container.DataItem, "Vehicle")%>"></label>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="form-group">
                                            <div class="col-md-12">
                                                <label class="d-block">
                                                    Distinguishing scars/marks/tattoos
                                        <textarea rows="4" readonly class="form-control"><%#DataBinder.Eval(Container.DataItem, "Features")%></textarea></label>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </ItemTemplate>
                    <FooterTemplate>
                        </div>
                    </FooterTemplate>
                </asp:Repeater>
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="cmdSavePerson" />
            </Triggers>
        </asp:UpdatePanel>
        <button type="button" id="cmdAddPerson" class="btn btn-tertiary" data-toggle="modal" data-target="#personEdit"><em class="fa fa-plus"></em>&nbsp;Add Suspect</button>
    </fieldset>
    <hr />
    <fieldset>
        <legend>Actions Taken on Scene</legend>
        <div class="form-group">
            <div class="row">
                <div class="col-md-6">
                    <asp:Label Text="Reported to Law Enforcement on (date)" runat="server" AssociatedControlID="txtDateReportedLeo" />
                    <asp:TextBox ID="txtDateReportedLeo" runat="server" MaxLength="50" CssClass="form-control" />
                </div>
                <div class="col-md-6">
                    <asp:Label Text="Reported By" runat="server" AssociatedControlID="txtPersonReportingLeo" />
                    <asp:TextBox ID="txtPersonReportingLeo" runat="server" MaxLength="50" CssClass="form-control" />

                </div>

            </div>
        </div>
        <div class="form-group">
            <div class="row">
                <div class="col-md-6">
                    <asp:Label Text="Law Enforcement Agency" runat="server" AssociatedControlID="txtAgency" />
                    <asp:TextBox ID="txtAgency" runat="server" MaxLength="50" CssClass="form-control" />
                </div>
                <div class="col-md-6">
                    <asp:Label Text="Case Number" runat="server" AssociatedControlID="txtCaseNumber" />
                    <asp:TextBox ID="txtCaseNumber" runat="server" MaxLength="50" CssClass="form-control" />

                </div>

            </div>
        </div>

        <div class="form-group">
            <div class="row">
                <div class="col-md-12">
                    <asp:Label Text="Other Actions" runat="server" AssociatedControlID="txtActionTaken" />
                    <asp:TextBox ID="txtActionTaken" runat="server" TextMode="MultiLine" CssClass="form-control" Rows="5" />
                </div>
            </div>
        </div>

    </fieldset>
    <fieldset>
        <legend>Attach any Additional Information as Necessary</legend>
        <div class="form-group">
            <div class="row">
                <div class="col-md-12 filebox">
                    <div style="position: relative;">
                        <div id="files-overlay" class="overlay" style="display: none;">
                            <div class="spinner"></div>
                        </div>
                        <div id="upload-wrapper" class="btn btn-tertiary">
                            Select File to Upload
                <asp:FileUpload ID="uplFiles" runat="server" CssClass="file-upload" accept=".pdf,.jpg,.jpeg" onchange='check_extension(this.value);' />
                            <asp:Button ID="cmdAddFile" CssClass="btn btn-primary sr-only" CausesValidation="false" runat="server" Text="Upload Selected File" Enabled="false" />
                            <span id="fileUploadWarning" class="dnnFormMessage dnnFormError" style="display: none; right: auto; left: 25%;">Please Choose File to Upload</span>
                        </div>

                    </div>
                    <div class="file-list-container">

                        <ul id="fileList" class="file-list">
                            <asp:Literal ID="ltFiles" runat="server"></asp:Literal>
                        </ul>

                        <div id="dialog" style="display: none" title="Confirmation Required">
                            <p>Delete the selected file?</p>
                        </div>
                    </div>
                    <span class="info"></span>

                </div>

            </div>
        </div>
    </fieldset>
    <div class="row">
        <div class="col-md-12">
            <asp:Button ID="cmdSubmit" runat="server" OnClick="cmdSubmit_Click" CssClass="btn btn-primary btn-lg" Text="Submit" data-loading-text="Loading..." ValidationGroup="incident" />
            <asp:HyperLink ID="lnkCancel" runat="server" CssClass="btn btn-secondary btn-lg" TabIndex="0" Text="Cancel" />
        </div>
    </div>

</div>
<div id="personEdit" class="modal fade" tabindex="-1" role="dialog" aria-labelledby="PersonLabel" aria-hidden="true">
    <div class="modal-dialog modal-lg">
        <div class="modal-content">
            <div class="modal-header">
                <h4 class="modal-title" id="PersonLabel">Add Suspect</h4>
            </div>
            <div class="modal-body">
                <div class="form-group">
                    <div class="row">
                        <div class="col-md-4">
                            <asp:Label Text="First Name" runat="server" AssociatedControlID="txtFirstName" />
                            <asp:TextBox ID="txtFirstName" runat="server" MaxLength="50" CssClass="form-control" />
                        </div>
                        <div class="col-md-5">
                            <asp:Label Text="Last Name" runat="server" AssociatedControlID="txtLastName" />
                            <asp:TextBox ID="txtLastName" runat="server" MaxLength="50" CssClass="form-control" />
                        </div>
                        <div class="col-md-3">
                            <asp:Label Text="Birth Date" runat="server" AssociatedControlID="txtDOB" />
                            <asp:TextBox ID="txtDOB" runat="server" MaxLength="50" CssClass="form-control" TextMode="Date" />
                            <asp:CompareValidator ID="valIsDOB" runat="server" Type="Date" CssClass="label label-danger"
                                Operator="DataTypeCheck" ControlToValidate="txtDOB" Display="Dynamic"
                                ErrorMessage="Please enter a valid date." ValidationGroup="person">
                            </asp:CompareValidator>
                        </div>

                    </div>
                </div>
                <div class="form-group">
                    <div class="row">
                        <div class="col-md-3">
                            <asp:Label Text="Phone" runat="server" AssociatedControlID="txtPhonePerson" />
                            <asp:TextBox ID="txtPhonePerson" TextMode="Phone" runat="server" MaxLength="50" CssClass="form-control phone_us" placeholder="(555) 555-5555" />

                        </div>

                        <div class="col-md-4">
                            <asp:Label Text="Gender" runat="server" AssociatedControlID="drpGender" />
                            <asp:DropDownList runat="server" ID="drpGender" CssClass="form-control">
                                <asp:ListItem Text="< Select Gender>" Value="" />
                                <asp:ListItem Text="Male" />
                                <asp:ListItem Text="Female" />
                            </asp:DropDownList>
                        </div>
                        <div class="col-md-5">
                            <asp:Label Text="Race" runat="server" AssociatedControlID="drpRace" />
                            <asp:DropDownList runat="server" ID="drpRace" CssClass="form-control">
                                <asp:ListItem Text="< Select Race>" Value="" />
                                <asp:ListItem Text="Afican American Not Hispanic" />
                                <asp:ListItem Text="Asian / Pacific Islander" />
                                <asp:ListItem Text="Hispanic" />
                                <asp:ListItem Text="Native American / Alaskan" />
                                <asp:ListItem Text="White Not Hispanic" />
                            </asp:DropDownList>
                        </div>

                    </div>
                </div>
                <div class="form-group">
                    <div class="row">
                        <div class="col-md-2">
                            <asp:Label Text="Height" runat="server" AssociatedControlID="txtHeight" />
                            <asp:TextBox ID="txtHeight" runat="server" MaxLength="50" CssClass="form-control" />
                        </div>
                        <div class="col-md-2">
                            <asp:Label Text="Weight" runat="server" AssociatedControlID="txtWeight" />
                            <asp:TextBox ID="txtWeight" runat="server" MaxLength="50" CssClass="form-control" />
                        </div>
                        <div class="col-md-3">
                            <asp:Label Text="Hair Color" runat="server" AssociatedControlID="txtHairColor" />
                            <asp:TextBox ID="txtHairColor" runat="server" MaxLength="50" CssClass="form-control" />
                        </div>

                        <div class="col-md-5">
                            <asp:Label Text="Voice (accent, slang, speech)" runat="server" AssociatedControlID="txtVoice" />
                            <asp:TextBox ID="txtVoice" runat="server" MaxLength="100" CssClass="form-control" />
                        </div>

                    </div>
                </div>
                <div class="form-group">
                    <div class="row">
                        <div class="col-md-12">
                            <asp:Label Text="Distinguishing scars/marks/tattoos" runat="server" AssociatedControlID="txtFeatures" />
                            <asp:TextBox ID="txtFeatures" runat="server" TextMode="MultiLine" CssClass="form-control" Rows="3" />
                        </div>
                    </div>
                </div>
                <div class="form-group">
                    <div class="row">
                        <div class="col-md-12">
                            <asp:Label Text="Vehicle Info" runat="server" AssociatedControlID="txtVehicle" />
                            <asp:TextBox ID="txtVehicle" runat="server" MaxLength="100" CssClass="form-control" />
                        </div>

                    </div>
                </div>

            </div>
            <div class="modal-footer">
                <asp:LinkButton ID="cmdSavePerson" OnClick="cmdSavePerson_Click" runat="server" Text="Save Person" OnClientClick="CloseDialog();" CssClass="btn btn-primary" />
                <button type="button" id="cmdCancelPerson" tabindex="0" class="btn btn-secondary" data-dismiss="modal">Cancel</button>
            </div>
        </div>
    </div>
</div>
<asp:HiddenField ID="hdFileIds" runat="server" />
<asp:HiddenField ID="hdIncidentId" runat="server" />


<dnn:DnnCssInclude runat="server" FilePath="~/Resources/Shared/components/TimePicker/Themes/jquery-ui.min.css" />
<dnn:DnnCssInclude runat="server" FilePath="/Resources/Shared/components/TimePicker/jquery.timepicker.min.css" />
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Shared/components/TimePicker/jquery.timepicker.min.js" />
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/jQuery/jquery.mask.js" />


<script type="text/javascript">
    /*globals jQuery, window, Sys */
    (function ($, Sys) {

        $(document).ready(function () {
            $("#<%=txtDate.ClientID%>").datepicker();
            $("#<%=txtDateReported.ClientID%>").datepicker();
            $("#<%=txtDateReportedLeo.ClientID%>").datepicker();

            $(".check label").addClass("form-check-label");
            $(".check input").addClass("form-check-input");
            $('.phone_us').mask('(000) 000-0000');

            $("#cmdAddPerson").click(function () {
                $('#<%=txtFirstName.ClientID%>').val('');
                $('#<%=txtLastName.ClientID%>').val('');
                $('#<%=txtDOB.ClientID%>').val('');
                $('#<%=txtHairColor.ClientID%>').val('');
                $('#<%=txtHeight.ClientID%>').val('');
                $('#<%=txtWeight.ClientID%>').val('');
                $('#<%=txtFeatures.ClientID%>').val('');
                $('#<%=drpGender.ClientID%>').val('');
                $('#<%=drpRace.ClientID%>').val('');
                $('#<%=txtDOB.ClientID%>').val('');
                $('#<%=txtVehicle.ClientID%>').val('');
                $('#<%=txtVoice.ClientID%>').val('');
                $('#<%=txtPhonePerson.ClientID%>').val('');
            });
            $("#upload-wrapper").click(function (evt) {
                $("#files-overlay").show();
            });
            $("#<%=cmdAddFile.ClientID%>").click(function (evt) {
                var incidentId = "<%=hdIncidentId.Value.ToString()%>";
                var upload = $("#<%=uplFiles.ClientID%>");
                var fileUpload = $("#<%=uplFiles.ClientID%>").get(0);
                var files = fileUpload.files;
                if (files.length == 0) {
                    $(".info").html("<span class='text-danger'>Please Choose a File!</span>");
                    return false;
                }
                var filename = files[0].name;
                var data = new FormData();
                data.append(filename, files[0]);
                data.append("incidentId", incidentId);
                data.append("moduleId", <%=ModuleId%>);
                var options = {};
                options.url = "/ThreatUploadHandler.att";
                options.type = "POST";
                options.data = data;
                options.contentType = false;
                options.processData = false;
                options.success = function (result) {
                    $("#files-overlay").hide();
                    if (result.fileId > 0) {
                        var fileIdList = $("#<%=hdFileIds.ClientID%>").val();
                        if (fileIdList.length == 0) {
                            $("#<%=hdFileIds.ClientID%>").val(result.fileId);
                        } else {
                            $("#<%=hdFileIds.ClientID%>").val(fileIdList + "," + result.fileId);
                        }
                        $("#fileList").append("<li data-incidentId='" + incidentId + "' data-fileId='" + result.fileId + "'><span class='file-name'>" + filename + "</span>&nbsp;<a class='text-danger' onclick=\"ConfirmDelete('" + result.fileId + "')\"><i class='fas fa-trash'></i></a></li>");
                        WriteMessage(false, "File Captured");
                    } else {
                        WriteMessage(true, result.error);
                    }
                };
                options.error = function (err) {
                    new Noty({ text: 'Unexpected Error Occurred. Please Try Again', type: 'error', timeout: 5000, layout: 'topRight', theme: 'mint' }).show();
                    setTimeout(function () {
                        $("#files-overlay").hide();
                        $(".info").html('');
                    }, 1000);
                };
                $.ajax(options);
                evt.preventDefault();
            });
        });
    }(jQuery, window.Sys));
    function CloseDialog() {
        $('#personEdit').modal('hide');
        return true;
    }
    var extensionHash = {
        '.pdf': 1,
        '.jpg': 1,
        '.jpeg': 1,
    };

    function check_extension(filename) {
        var re = /\..+$/;
        var ext = filename.match(re);
        var submitEl = document.getElementById('<%=cmdAddFile.ClientID%>');
        if (extensionHash[ext]) {
            $(".info").html("");
            $("#<%=cmdAddFile.ClientID%>").trigger("click");
            submitEl.disabled = false;
            return true;
        } else {
            $(".info").html("<span class='text-danger'>Invalid File Type, please choose a document with a pdf, jpg, or jpeg extension!</span>");
            submitEl.disabled = true;
            return false;
        }
    }

    function ConfirmDelete(fileId) {
        $("#dialog").dialog({
            buttons: [
                {
                    text: "Delete",
                    "class": 'dnnPrimaryAction',
                    click: function () {
                        DeleteFile(fileId);
                        $(this).dialog("close");
                    }
                },
                {
                    text: "Cancel",
                    "class": 'dnnSecondaryAction dnnConfirmCancel',
                    click: function () {
                        $(this).dialog("close");
                    }
                }
            ],
            close: function () {
                $(this).dialog("close");
            }
        });

        $("#dialog").dialog("open");
    }
    function DeleteFile(fileId) {
        var data = new FormData();
        data.append("fileId", fileId);
        var options = {};
        options.url = "/ThreatUploadHandler.att";
        options.type = "POST";
        options.data = data;
        options.contentType = false;
        options.processData = false;
        options.success = function (result) {
            if (result.fileId > 0) {
                var listItem = $("li[data-fileid='" + result.fileId + "']");
                listItem.remove();
                var fileList = "";
                $('li[data-fileid]').each(function () {
                    var id = $(this).data("fileid");
                    if (fileList == "") {
                        fileList = id;
                    } else {
                        fileList = fileList + "," + id;
                    }
                });
                $("#<%=hdFileIds.ClientID%>").val(fileList);
                var fileCount = $(".file-list li").length;
                WriteMessage(false, result.error);
            } else {
                WriteMessage(true, result.error);
            }
        };
        options.error = function (err) { new Noty({ text: 'Unexpected Error Occurred. Please Try Again', type: 'error', timeout: 5000, layout: 'topRight', theme: 'mint' }).show(); };
        $.ajax(options);

        return false;
    }
    function WriteMessage(isError, message) {
        if (isError) {
            $(".info").html("<span class='text-danger'>" + message + "</span>");
        } else {
            $("#fileAttachmentWarning").fadeOut();
            $(".info").html("<span class='text-success'>" + message + "</span>");
        }
    }
    function Jud12ConfirmPostback(btn, msg, title) {
        if (!window.Swal) { return window.confirm(msg); }
        if (btn && btn.dataset && btn.dataset.jud12Confirmed === '1') {
            btn.dataset.jud12Confirmed = '';
            return true;
        }
        Swal.fire({
            title: title || 'Confirm', text: msg, icon: 'warning',
            showCancelButton: true, confirmButtonText: 'Yes', cancelButtonText: 'No',
            confirmButtonColor: '#d33'
        }).then(function (r) {
            if (!r.isConfirmed) return;
            var href = btn.href || '';
            var m = href.match(/__doPostBack\(['"]([^'"]+)['"],\s*['"]([^'"]*)['"]\)/);
            if (m && typeof __doPostBack === 'function') {
                __doPostBack(m[1], m[2]);
            } else if (btn && btn.tagName === 'INPUT' && (btn.type === 'submit' || btn.type === 'button')) {
                btn.dataset.jud12Confirmed = '1';
                btn.click();
            } else if (btn && typeof btn.click === 'function') {
                btn.dataset.jud12Confirmed = '1';
                btn.click();
            }
        });
        return false;
    }
</script>
