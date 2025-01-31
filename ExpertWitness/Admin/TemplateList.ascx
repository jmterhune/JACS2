<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TemplateList.ascx.cs" Inherits="tjc.Modules.ExpertWitness.TemplateList" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>

<div class="tabs">
    <ul class="nav nav-tabs">
        <li class="nav-item">
            <a class="nav-link" href="<%=RequestListUrl %>">Requests</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=ExpertListUrl %>">Experts</a>
        </li>
        <li class="nav-item active">
            <a class="nav-link" href="#evalutions" data-toggle="tab">Evaluation Types</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=TypeListUrl %>">Expert Types</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=LocationListUrl %>">Locations</a>
        </li>
    </ul>
    <div class="tab-content">
        <div id="evalutions" class="tab-pane active">
            <asp:UpdatePanel ID="pnlTemplates" runat="server" RenderMode="Block" OnUnload="pnlTemplates_Unload">
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
                    <asp:Repeater ID="rptTemplate" runat="server" OnItemCreated="rptTemplate_ItemCreated" OnItemCommand="rptTemplate_ItemCommand">
                        <HeaderTemplate>
                            <table id="tblTemplate" class="table table-striped">
                                <thead>
                                    <tr>
                                        <th>&nbsp;</th>
                                        <th>ID</th>
                                        <th>Evaluation Type</th>
                                        <th>Required Expert Types</th>
                                        <th>&nbsp;</th>
                                    </tr>
                                </thead>
                                <tbody>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <tr>
                                <td class="command-item">
                                    <asp:LinkButton ID="cmdEdit" runat="server" CommandName="edit" CausesValidation="false" CommandArgument='<%#DataBinder.Eval(Container.DataItem,"TemplateId").ToString() %>'><i class="fa fa-pencil"></i></asp:LinkButton>
                                <td><%#Eval("TemplateID")%></td>
                                <td><%#Eval("TemplateName")%></td>
                                <td><%#Eval("TypesRequired")%></td>
                                <td class="command-item">
                                    <asp:LinkButton ID="cmdDelete" CssClass="confirm" runat="server" CausesValidation="false" CommandName="delete" CommandArgument='<%#DataBinder.Eval(Container.DataItem,"TemplateId").ToString() %>'><i class="fa fa-trash"></i></asp:LinkButton>
                                </td>
                            </tr>
                        </ItemTemplate>
                        <FooterTemplate>
                            </tbody>
                    </table>
                        </FooterTemplate>
                    </asp:Repeater>
                    <div class="modal fade" id="EditTemplateModal" tabindex="-1" role="dialog" aria-labelledby="EditTemplateModalLabel" aria-hidden="true">
                        <div class="modal-dialog">
                            <div class="modal-content">
                                <div class="modal-header">
                                    <h4 class="modal-title" id="EditTemplateModalLabel">Add / Edit Template</h4>
                                    <button type="button" class="close" data-bs-dismiss="modal" aria-hidden="true">&times;</button>
                                </div>
                                <div class="modal-body">
                                    <div class="form-group">
                                        <asp:Label runat="server" AssociatedControlID="txtTemplateName" Text="Template" />
                                        <asp:TextBox runat="server" ClientIDMode="Static" CssClass="form-control" MaxLength="200" ID="txtTemplateName" />
                                        <asp:RequiredFieldValidator ValidationGroup="Template" runat="server" ControlToValidate="txtTemplateName" Display="Dynamic" SetFocusOnError="true" CssClass="label label-danger" ErrorMessage="Template Name is Required" />
                                    </div>
                                    <fieldset class="outline-fieldset">
                                        <legend>Add Requirements</legend>
                                        <div class="form-group">
                                            <asp:Label runat="server" AssociatedControlID="txtNumberRequired" Text="Number Required" />
                                            <asp:TextBox runat="server" ClientIDMode="Static" CssClass="form-control" TextMode="Number" ID="txtNumberRequired" />
                                            <asp:RequiredFieldValidator ValidationGroup="Type" runat="server" ControlToValidate="txtNumberRequired" Display="Dynamic" SetFocusOnError="true" CssClass="label label-danger" ErrorMessage="Number Required is Required" />
                                        </div>
                                        <asp:CheckBoxList ID="clsTemplateRequirements" ClientIDMode="Static" runat="server" RepeatDirection="Vertical" CssClass="radio-button-list form-check form-switch column-2 evaluation-types" RepeatLayout="UnorderedList">
                                        </asp:CheckBoxList>
                                        <asp:CustomValidator runat="server" ValidationGroup="Type" ID="valTemplateRequirements" Display="Dynamic" CssClass="label label-danger" ClientValidationFunction="ValidateTemplateRequirements" ErrorMessage="Please Select At least one Expert Type" />
                                        <hr />
                                        <p>
                                            <button type="button" class="btn btn-tertiary" id="cmdAddRequirement">Add Requirement</button>
                                        </p>
                                    </fieldset>
                                    <asp:HiddenField ID="hdTemplateId" ClientIDMode="Static" runat="server" />
                                    <asp:HiddenField ID="hdRequirements" ClientIDMode="Static" runat="server" />
                                    <asp:Repeater ID="rptTemplateTypes" runat="server">
                                        <HeaderTemplate>
                                            <table id="tblTypes" class="table table-striped">
                                                <thead>
                                                    <tr>
                                                        <th>#</th>
                                                        <th>Expert Types</th>
                                                        <th>Required</th>
                                                        <th>&nbsp;</th>
                                                    </tr>
                                                </thead>
                                                <tbody>
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <tr data-seq="<%#Eval("Sequence")%>" data-type="<%#Eval("TypeNames")%>" data-required="<%#Eval("NumberRequired")%>">
                                                <td><%#Eval("Sequence")%></td>
                                                <td><%#Eval("TypeNames")%></td>
                                                <td><%#Eval("NumberRequired")%></td>
                                                <td class="command-item">
                                                    <a class="deleteRow" data-index="<%#Container.ItemIndex %>" tabindex="0" role="button" aria-pressed="false"><i class="fa fa-trash"></i></a>
                                                </td>
                                            </tr>
                                        </ItemTemplate>
                                        <FooterTemplate>
                                            </tbody>
                                            </table>
                                        </FooterTemplate>
                                    </asp:Repeater>
                                </div>
                                <div class="modal-footer justify-content-between">
                                    <asp:Button OnClientClick="ToggleEditForm(false)" ValidationGroup="Template" CssClass="btn btn-primary" ID="cmdSave" runat="server" Text="Save" OnClick="cmdSave_Click" />
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
    let templateRequirements = [];
    (function ($, Sys) {

        $(document).ready(function () {
            Sys.Application.add_load(function (s, e) { PageInit(); });
            PageInit();
        });

    }(jQuery, window.Sys));

    function PageInit() {
        var table = $('#tblTemplate').DataTable({
            "order": [[1, "asc"]],
            "oLanguage": {
                "sSearch": "Filter by Text"
            },
            "aoColumns": [
                { "bSortable": false },
                { "bSortable": true },
                { "bSortable": true },
                { "bSortable": false },
                { "bSortable": false },],
            autoWidth: true,
        });
        $("#tblTemplate_length").prepend('<button onclick="return ClearForm()" class="btn btn-primary btn-sm me-2" data-bs-toggle="modal" data-bs-target="#EditTemplateModal"><i class="fa fa-plus"></i>&nbsp;Add Template</button>');
        table.on('draw', function () {
            $(".confirm").dnnConfirm({
                text: 'Are you sure you wish to Delete the selected Template?',
                yesText: 'Yes',
                noText: 'No',
                title: 'Delete Template?'
            });
        });
        table.draw();
        $("#tblTypes tbody tr").each(function () {
            const seq = $(this).data("seq");
            const required = $(this).data("required");
            const templateTypes = [];
            const templateTypeNames = $(this).data("type").split(';');
            templateTypeNames.forEach((labelText) => {
                const type = { typeName: labelText, typeId: GetCheckboxValueByLabel(labelText) }
                templateTypes.push(type);
            });
            const templateRequirement = {
                numberRequired: required,
                types: templateTypes,
                sequence: seq
            };
            templateRequirements.push(templateRequirement);
            $("#hdRequirements").val(JSON.stringify(templateRequirements));
        });
        $('#cmdAddRequirement').on('click', function (e) {
            e.preventDefault();
            var isValid = Page_ClientValidate("Type");
            if (isValid) {
                const numberRequired = $('#txtNumberRequired').val();
                const templateTypes = $('#clsTemplateRequirements input[type="checkbox"]:checked').map(function () {
                    const type = { typeName: $(this).next("label").text(), typeId: $(this).val() }
                    return type;
                }).get();
                const templateRequirement = {
                    numberRequired: parseInt(numberRequired, 10),
                    types: templateTypes,
                    sequence: templateRequirements.length + 1
                };
                templateRequirements.push(templateRequirement);
                $("#hdRequirements").val(JSON.stringify(templateRequirements));
                PopulateTable();
                ClearTypeForm();
            }
        });
        $("#EditTemplateModal").on('click', '.deleteRow', function (e) {
            e.preventDefault();
            const indexToDelete = $(this).data('index');
            DeleteRow(indexToDelete);
        });
    }
    function GetCheckboxValueByLabel(labelText) {
        const label = $("label").filter(function () {
            return $(this).text().trim() == labelText;
        });
        const checkbox = label.prev("input[type='checkbox']");
        return checkbox.length ? checkbox.val() : null;
    }
    function ToggleEditForm(toggleValue) {
        if (toggleValue) {
            $('#EditTemplateModal').modal('show');
        } else {
            var isValid = Page_ClientValidate("Template");
            if (isValid) {
                $('#EditTemplateModal').modal('hide');
                $('body').removeClass('modal-open');
                $('.modal-backdrop').remove();
            }
        }
        return true;
    }
    function ClearTypeForm() {
        $('#txtNumberRequired').val("");
        $('#clsTemplateRequirements input[type="checkbox"]').prop('checked', false);
        return false;
    }
    function ClearForm() {
        templateRequirements = [];
        const tableBody = $('#tblTypes tbody');
        tableBody.empty(); // Clear the table body
        $('#txtTemplateName').val("");
        $('#hdRequirements').val("");
        $('#hdTemplateId').val("");
        return false;
    }
    function DeleteRow(index) {
        templateRequirements.splice(index, 1);
        $("#hdRequirements").val(JSON.stringify(templateRequirements));
        PopulateTable();
    }
    function PopulateTable() {
        const tableBody = $('#tblTypes tbody');
        tableBody.empty(); // Clear the table body
        templateRequirements.forEach((obj, index) => {
            const row = `<tr>
                    <td>${index + 1}</td>
                    <td>${obj.types.map(type => type.typeName).join(';')}</td>
                    <td>${obj.numberRequired}</td>
                    <td><a class="deleteRow" data-index="${index}" tabindex="0" role="button" aria-pressed="false"><i class="fa fa-trash"></i></a></td>
                </tr>`;
            tableBody.append(row);
        });
    }
    //Validations
    function ValidateTemplateRequirements(source, args) {
        var clsTemplateRequirements = document.getElementById('clsTemplateRequirements');
        var chkListinputs = clsTemplateRequirements.getElementsByTagName("input");
        for (var i = 0; i < chkListinputs.length; i++) {
            if (chkListinputs[i].checked) {
                args.IsValid = true;
                return;
            }
        }
        args.IsValid = false;
    }
</script>

