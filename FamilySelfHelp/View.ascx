<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="View.ascx.cs" Inherits="tjc.Modules.FamilySelfHelp.View" %>
<div id="LogForm">
    <div class="container">
        <div class="btn-group">
            <asp:HyperLink ID="lnkSearch" CssClass="btn btn-primary active" runat="server">Search</asp:HyperLink>
            <asp:HyperLink ID="lnkDataEntry" CssClass="btn btn-primary" runat="server">Data Entry</asp:HyperLink>
            <asp:HyperLink ID="lnkMerge" CssClass="btn btn-primary" runat="server">Merge Clients</asp:HyperLink>
            <asp:HyperLink ID="lnkReports" CssClass="btn btn-primary" runat="server">Reports</asp:HyperLink>
        </div>
        <div class="row">
            <div class="col-6 d-flex align-items-end">

                <fieldset id="fsDetails" runat="server" visible="false" class="fieldset-bordered">
                    <legend>Client Details</legend>
                    <dl class="data-list">
                        <dt>Client:</dt>
                        <dd>
                            <asp:Literal ID="lblName" runat="server" /></dd>
                        <dt>Client ID:</dt>
                        <dd>
                            <asp:Literal ID="lblNumber" runat="server" /></dd>
                    </dl>
                    <asp:HyperLink ID="lnkEditLink" runat="server" Text="Edit this Client" CssClass="btn btn-tertiary float-end" />
                </fieldset>
            </div>
            <div class="col-6">
                <section class="call-to-action call-to-action-default pt-3 pb-2">
                    <p class="text-start">Begin typing the clients last name below to check for previous log</p>
                    <div class="row form-group">
                        <div class="col text-start">
                            <asp:Label runat="server" AssociatedControlID="txtName" CssClass="fw-bold" Text="Client" ToolTip="Begin typing the clients last name below to check for previous log" />
                            <asp:TextBox runat="server" CssClass="form-control" MaxLength="50" ID="txtName" />
                            <select id="drpClients" class="form-control client-list position-absolute top-20" style="z-index: 1000"></select>
                            <asp:HiddenField ID="hdClientId" ClientIDMode="Static" runat="server" />
                        </div>
                        <div class="col-auto  mt-4">
                            <asp:Button ID="cmdSearch" runat="server" CausesValidation="false" CssClass="btn btn-lg btn-default" Text="Search" OnClick="cmdSearch_Click" />
                        </div>
                    </div>
                </section>
            </div>
        </div>
        <hr />
    </div>
    <asp:Panel ID="pnlDetails" CssClass="ms-2 me-2" runat="server" Visible="false">
        <div id="Details">
            <asp:Repeater ID="rptEvents" runat="server" OnItemCommand="rptEvents_ItemCommand">
                <HeaderTemplate>
                    <h4 class="mb-1">Case Log Events</h4>
                    <table class="table table-striped">
                        <thead>
                            <tr>
                                <th>&nbsp;</th>
                                <th>Date</th>
                                <th>Case Number</th>
                                <th>Division</th>
                                <th>Contact Method</th>
                                <th>Location</th>
                                <th>Case Type(s)</th>
                                <th>Client Type</th>
                                <th>Service(s) Provided</th>
                                <th>Interpreter</th>
                                <th>Appointment</th>
                                <th>Time</th>
                                <th>&nbsp;</th>
                            </tr>
                        </thead>
                        <tbody>
                </HeaderTemplate>
                <FooterTemplate></tbody></table></FooterTemplate>
                <ItemTemplate>
                    <tr>
                        <td class="command-icon">
                            <asp:HyperLink ID="lnkDetail" ToolTip="Edit This Record" CssClass="cmdlink" runat="server" NavigateUrl='<%#EditUrl("lid", Eval("LogId").ToString(), "log")%>'><i class="fas fa-pencil"></i></asp:HyperLink></td>
                        <td>
                            <%#Eval("ServiceDate", "{0:d}")%>
                        </td>
                        <td>
                            <%#Eval("IsNewCase")%></td>
                        <td><%#Eval("Division")%></td>
                        <td><%#Eval("ContactMethod")%></td>
                        <td><%#Eval("Location")%></td>
                        <td><%#Eval("FormattedCaseType")%></td>
                        <td><%#Eval("ClientType")%></td>
                        <td><%#Eval("FormattedServiceProvided")%></td>
                        <td class="text-center"><%#HasInterpreter(Eval("InterpreterProvided").ToString())%></td>
                        <td class="text-center"><%#HasInterpreter(Eval("HasAppointment").ToString())%></td>
                        <td><%#Eval("TimeSpent")%> Hours</td>
                        <td class="command-icon">
                            <asp:LinkButton ID="cmdDelete" CssClass="confirm" CommandName="delete" ToolTip="Delete this Record" CommandArgument='<%# Eval("LogId").ToString()%>' runat="server">
                                            <i title="Delete File" class="fa fa-trash"></i>
                            </asp:LinkButton></td>

                    </tr>
                </ItemTemplate>
            </asp:Repeater>

            <asp:HyperLink ID="lnkNewLog" runat="server" Text="New Case" CssClass="btn btn-primary" />
        </div>
    </asp:Panel>
    <p class="text-center">
        <asp:LinkButton ID="cmdNewClient" runat="server" Text="New Client" CssClass="btn btn-primary" Visible="false" />
    </p>
</div>
<script>
    var moduleId = <%=ModuleId%>;

    $(function () {
        $('.client-list').hide();
        $("#<%=txtName.ClientID%>").on("keyup", function () {
            var text = $(this).val();
            FillClients(text);
            $('.client-list').show();
            $('#hdClientId').val("");
        });
        $('#drpClients').on("change", function () {
            $("#<%=txtName.ClientID%>").val($(this).find("option:selected").text());
            $('#hdClientId').val($(this).find("option:selected").val());
            $('.client-list').hide();
        });
        $('#drpClients').on("click", function () {
            if ($(this).length == 1) {
                $('#drpClients').attr("size", 0);
                $("#<%=txtName.ClientID%>").val($(this).find("option:selected").text());
                $('.client-list').hide();
                $('#hdClientId').val($(this).find("option:selected").val());
            }
        });
        $(".confirm").dnnConfirm({
            text: 'Are you sure you wish to delete this Record?',
            yesText: 'Yes',
            noText: 'No',
            title: 'Delete Record?'
        });
    });
    function FindTextInClientList(textValue) {
        $('#drpClients option:contains("")');
    }
    function FillClients(name) {
        var service = {
            path: "tjc.Modules/FamilySelfHelp",
            framework: $.ServicesFramework(moduleId)
        }
        service.baseUrl = service.framework.getServiceRoot(service.path);
        var restUrl = "/DesktopModules/tjc.Modules/FamilySelfHelp/api/ClientName/client/" + name;
        var choices = '';
        drpClients = $('#drpClients');
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
                            choices += '<option value="' + c.value + '">' + c.text + '</option>';
                        }
                        drpClients.html(choices);

                    } else {
                        ShowAlert("No Matching Client Found", "No Client exits for the name entered.<br />Please enter a different name or contact the site administrator.");
                    }
                }
                else {
                    ShowAlert("No Matching Client Found", "No Client exits for the name entered.<br />Please enter a different name or contact the site administrator.");
                }
            }).always(function (data) {
                if (count > 25)
                    count = 25;
                if (count == 1)
                    count = 0;
                $('#drpClients').attr("size", count);
            });

        } catch (e) {
            ShowAlert("Error!", "Error retrieving client names.<br />Please refresh the page and try again.");
        }
        if ($("#hdClientId").val() != "")
            $('#drpClients').val($("#hdClientId").val());
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
