<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="LogList.ascx.cs" Inherits="tjc.Modules.ProSeLog.LogList" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<asp:HyperLink ID="lnkManage" Visible="false" CssClass="btn btn-danger mb-3" runat="server">Manage Lists</asp:HyperLink>
<div class="tabs">
    <ul class="nav nav-tabs">
        <li class="nav-item active">
            <a class="nav-link" href="#caseType" data-toggle="tab">Search</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=FormUrl %>">Data Entry</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=StatsUrl %>">Monthly Stats</a>
        </li>
    </ul>
    <div class="tab-content">
        <div id="logList" class="tab-pane active">
            <section class="call-to-action call-to-action-default mb-lg rounded p-3">
                <div class="row form-group">
                    <div class="col-4">
                        <asp:Label runat="server" AssociatedControlID="txtPetitioner" Text="Petitioner" ToolTip="Search By Petitioner" />
                        <asp:TextBox runat="server" CssClass="form-control" MaxLength="50" ID="txtPetitioner" />
                    </div>
                    <div class="col-auto  mt-4">
                        <asp:Button ID="cmdPetitioner" runat="server" CausesValidation="false" CssClass="btn btn-primary" Text="Submit" OnClick="cmdPetitioner_Click" />
                    </div>
                    <div class="col-4">
                        <asp:Label runat="server" AssociatedControlID="txtRespondent" Text="Respondent" ToolTip="Search By Respondent" />
                        <asp:TextBox runat="server" CssClass="form-control" MaxLength="50" ID="txtRespondent" />
                    </div>
                    <div class="col-auto mt-4">
                        <asp:Button ID="cmdRespondent" runat="server" CausesValidation="false" CssClass="btn btn-primary" Text="Submit" OnClick="cmdRespondent_Click" />
                    </div>
                    <div class="col-auto">
                        <asp:Label runat="server" AssociatedControlID="drpCounty" Text="County" ToolTip="Search By County" />
                        <asp:DropDownList runat="server" CssClass="form-control" ID="drpCounty" AppendDataBoundItems="true">
                            <asp:ListItem Value="0">&lt; Filter By Location &gt;</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                </div>
                <div class="row form-group">
                    <div class="col-4">
                        <asp:Label runat="server" AssociatedControlID="txtCaseName" Text="Case Name" ToolTip="Search By Case Name" />
                        <asp:TextBox runat="server" CssClass="form-control" MaxLength="50" ID="txtCaseName" />

                    </div>
                    <div class="col-auto  mt-4">
                        <asp:Button ID="cmdCaseName" runat="server" CausesValidation="false" CssClass="btn btn-primary" Text="Submit" OnClick="cmdCaseName_Click" />
                    </div>
                    <div class="col-4">
                        <asp:Label runat="server" AssociatedControlID="txtCaseNumber" Text="Case Number" ToolTip="Begin typing the Case Number below to retrieve matching numbers" />
                        <asp:TextBox runat="server" CssClass="form-control" ClientIDMode="static" MaxLength="50" ID="txtCaseNumber" />
                        <select id="drpCaseNumbers" class="form-control casenumber-list position-absolute top-20" ></select>
                    </div>
                    <div class="col-auto mt-4">
                        <asp:Button ID="cmdCaseNumber" runat="server" CausesValidation="false" CssClass="btn btn-primary" Text="Submit" OnClick="cmdCaseNumber_Click" />
                    </div>
                </div>
            </section>
            <asp:Literal ID="ltMessage" runat="server" />
            <asp:Repeater ID="rptHistoryList" runat="server" OnItemDataBound="rptHistoryList_ItemDataBound">
                <HeaderTemplate>
                    <table id="tblHistory" class="table table-striped">
                        <thead>
                            <tr>
                                <th>&nbsp;</th>
                                <th>&nbsp;</th>
                                <th>Petitioner</th>
                                <th>Respondent</th>
                                <th>Case Name</th>
                                <th>Case Number</th>
                                <th>Case Type</th>
                                <th>Resolved</th>
                            </tr>
                        </thead>
                        <tbody>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr>
                        <td class="command-item">
                            <asp:HyperLink ID="lnkEdit" ToolTip="Edit this Record" runat="server"><i class="fa fa-pencil"></i></asp:HyperLink>
                        </td>
                        <td class="command-item">
                            <asp:HyperLink ID="lnkView" ToolTip='<%#"View All Records for " + Eval("CaseNumber")%>' runat="server"><i class="fa fa-search"></i></asp:HyperLink>
                        </td>
                        <td><%#Eval("Petitioner")%></td>
                        <td><%#Eval("Respondent")%></td>
                        <td><%#Eval("CaseName")%></td>
                        <td><%#Eval("CaseNumber")%></td>
                        <td><%#Eval("CaseTypeName")%></td>
                        <td><%#DataBinder.Eval(Container.DataItem,"ResolutionDate","{0:MM/dd/yyyy}") %></td>
                    </tr>
                </ItemTemplate>
                <FooterTemplate>
                    </tbody>
                </table>
                </FooterTemplate>
            </asp:Repeater>
        </div>
    </div>
</div>

<dnn:dnnjsinclude runat="server" filepath="/Resources/Libraries/DataTables/jquery.dataTables.min.js" />
<dnn:dnnjsinclude runat="server" filepath="/Resources/Libraries/DataTables/dataTables.bootstrap5.min.js" />
<dnn:dnncssinclude runat="server" filepath="/Resources/Libraries/DataTables/dataTables.bootstrap5.min.css" />
<script type="text/javascript">
    var moduleId = <%=this.ModuleId%>;
    (function ($, Sys) {

        $(document).ready(function () {
            Sys.Application.add_load(function (s, e) { PageInit(); });
            PageInit();
        });

    }(jQuery, window.Sys));

    function PageInit() {

        $('.casenumber-list').hide();
        $("#txtCaseNumber").on("keyup", function () {
            var text = $(this).val();
            GetCaseNumbers(text);
            $('.casenumber-list').show();
        });
        $('#drpCaseNumbers').on("change", function () {
            $("#txtCaseNumber").val($(this).find("option:selected").text());
            $('.casenumber-list').hide();
        });
        $('#drpCaseNumbers').on("click", function () {
            if ($(this).length == 1) {
                $('#drpCaseNumbers').attr("size", 0);
                $("#txtCaseNumber").val($(this).find("option:selected").text());
                $('.casenumber-list').hide();
            }
        });

        var table = $('#tblHistory').DataTable({
            "order": [[1, "asc"]],
            "oLanguage": {
                "sSearch": "Filter by Text"
            },
            "aoColumns": [
                { "bSortable": false },
                { "bSortable": false },
                { "bSortable": true },
                { "bSortable": true },
                { "bSortable": true },
                { "bSortable": true },
                { "bSortable": true },
                { "bSortable": false },],
            autoWidth: true,
        });
    }
    function GetCaseNumbers(caseNumber) {
        var service = {
            path: "ProSeLog",
            framework: $.ServicesFramework(moduleId)
        }
        service.baseUrl = service.framework.getServiceRoot(service.path);
        var restUrl = `${service.baseUrl}CaseNumber/GetCaseNumbers/${caseNumber}`;
        var choices = '';
        drpCaseNumbers = $('#drpCaseNumbers');
        var count = 0;
        try {
            $.ajax({
                type: "GET",
                dataType: "json",
                url: restUrl,
            }).done(function (data) {
                if (data) {
                    if (data.length > 0) {
                        count = data.length;
                        for (var i = 0; i < data.length; i++) {
                            c = data[i];
                            choices += '<option>' + c.text + '</option>';
                        }
                        drpCaseNumbers.html(choices);

                    } else {
                        ShowAlert("No Matching Case Numbers Found", "No Case Number exits for the value entered.<br />Please enter a different value or contact the Help Desk.");
                    }
                }
                else {
                    ShowAlert("No Matching Case Numbers Found", "No Case Number exits for the value entered.<br />Please enter a different value or contact the Help Desk.");
                }
            }).always(function (data) {
                if (count > 25)
                    count = 25;
                if (count == 1)
                    count = 0;
                $('#drpCaseNumbers').attr("size", count);
            });

        } catch (e) {
            ShowAlert("Error!", "Error retrieving Case Numbers.<br />Please refresh the page and try again.");
        }
        return false;
    }
    function ShowAlert(title, text) {
        $.dnnAlert({
            okText: 'OK',
            title: title,
            text: text
        });
    }

</script>
