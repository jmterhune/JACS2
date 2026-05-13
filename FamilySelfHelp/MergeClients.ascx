<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MergeClients.ascx.cs" Inherits="tjc.Modules.FamilySelfHelp.MergeClients" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<%-- SweetAlert2 + Noty for confirms / toast notifications --%>
<dnn:DnnCssInclude runat="server" FilePath="/Resources/Libraries/sweetalert/sweetalert2.min.css" />
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/sweetalert/sweetalert2.all.min.js" />
<dnn:DnnCssInclude runat="server" FilePath="/Resources/Libraries/Noty/noty.min.css" />
<dnn:DnnCssInclude runat="server" FilePath="/Resources/Libraries/Noty/bootstrap-v4.min.css" />
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/Noty/noty.min.js" />
<div id="LogForm">
    <div class="container">
        <div class="btn-group mb-2">
            <asp:HyperLink ID="lnkSearch" CssClass="btn btn-primary" runat="server">Search</asp:HyperLink>
            <asp:HyperLink ID="lnkDataEntry" CssClass="btn btn-primary" runat="server">Data Entry</asp:HyperLink>
            <asp:HyperLink ID="lnkMerge" CssClass="btn btn-primary active" runat="server">Merge Clients</asp:HyperLink>
            <asp:HyperLink ID="lnkReports" CssClass="btn btn-primary" runat="server">Reports</asp:HyperLink>
        </div>
        <div class="row">
            <div class="col-7">
                <fieldset>
                    <legend>Clients</legend>
                    <asp:Repeater ID="rptClients" runat="server" OnItemCommand="rptClients_ItemCommand">
                        <HeaderTemplate>
                            <table class="table table-striped">
                                <thead>
                                    <tr>
                                        <th>Select Client</th>
                                        <th>Last Name</th>
                                        <th>First Name</th>
                                        <th>&nbsp;</th>
                                    </tr>
                                </thead>
                                <tbody>
                        </HeaderTemplate>

                        <ItemTemplate>
                            <tr>
                                <td>
                                    <asp:LinkButton ID="cmdMerge" ClientIDMode="Static" runat="server" CssClass="btn btn-default" CommandArgument='<%#Eval("ClientId").ToString()%>'
                                        CommandName="merge" OnClientClick="return Jud12ConfirmPostback(this, 'Merge all clients in this list to this client record?', 'Confirm');"><i class="fas fa-user-friends"></i> Merge into this Client</asp:LinkButton>
                                </td>
                                <td>
                                    <%#Eval("LastName")%></td>
                                <td>
                                    <%#Eval("FirstName")%></td>
                                <td>
                                    <asp:LinkButton ID="cmdRemove" ToolTip="Remove Client from Merge" ClientIDMode="Static" runat="server" CssClass="text-danger" CommandArgument='<%#Eval("ClientId").ToString()%>'
                                        CommandName="remove" OnClientClick="return Jud12ConfirmPostback(this, 'Remove this Client from the Merge?', 'Confirm');"><i class="fas fa-trash"></i></asp:LinkButton>
                                </td>
                            </tr>
                            <tr class="cases">
                                <td>&nbsp;</td>
                                <td colspan="3"><strong>Service Dates</strong>
                                    <%#GetCaseNumbers(Eval("ClientId").ToString())%></td>

                            </tr>
                        </ItemTemplate>
                        <SeparatorTemplate>
                            <tr>
                                <td colspan="4">&nbsp;</td>
                            </tr>
                        </SeparatorTemplate>

                        <FooterTemplate></tbody></table></FooterTemplate>
                    </asp:Repeater>
                </fieldset>

            </div>
            <div class="col-5">
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
                            <asp:Button ID="cmdClient" runat="server" CausesValidation="false" CssClass="btn btn-primary" Text="Add Client" OnClick="cmdClient_Click" />
                        </div>
                    </div>
                </section>
            </div>
        </div>
        <hr />
    </div>
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
        $("#cmdMerge").not('[data-swal-bound]').attr('data-swal-bound', '1').on('click', function (e) {
            e.preventDefault();
            var href = this.href || '';
            Swal.fire({
                title: 'Merge Clients?',
                text: 'Merge all clients in this list to this client record?',
                icon: 'warning',
                showCancelButton: true,
                confirmButtonText: 'Yes',
                cancelButtonText: 'No',
                confirmButtonColor: '#d33'
            }).then(function (r) {
                if (r.isConfirmed) {
                    var m = href.match(/__doPostBack\(['"]([^'"]+)['"],\s*['"]([^'"]*)['"]\)/);
                    if (m && typeof __doPostBack === 'function') __doPostBack(m[1], m[2]);
                }
            });
        });
    });
    function FindTextInClientList(textValue) {
        $('#drpClients option:contains(')
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
        return false;
    }
    function ShowAlert(title, text) {
        Swal.fire({ title: title, html: text, icon: 'info', confirmButtonText: 'OK' });
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
