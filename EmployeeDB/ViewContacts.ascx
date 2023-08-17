<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ViewContacts.ascx.cs" Inherits="tjc.Modules.EmployeeDB.ViewContacts" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<div class="tabs">
    <ul class="nav nav-tabs">
        <li class="nav-item">
            <a class="nav-link" href="<%=EmployeeUrl%>"><i class="fas fa-user"></i>&nbsp;Employees</a>
        </li>
        <li class="nav-item active">
            <a class="nav-link" href="<%=ContactUrl%>"><i class="fas fa-id-badge"></i>&nbsp;Contacts</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=DepartmentUrl%>"><i class="fas fa-sitemap"></i>&nbsp;Departments</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=JobGroupUrl%>"><i class="fas fa-users"></i>&nbsp;Job Groups</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=JobClassUrl%>"><i class="fas fa-user-tag"></i>&nbsp;Job Classes</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=RaceUrl%>"><i class="fas fa-users-cog"></i>&nbsp;Race</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=CountyUrl%>"><i class="fas fa-map-marked-alt"></i>&nbsp;Counties</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=LocationUrl%>"><i class="fas fa-building"></i>&nbsp;Locations</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=SwnLogUrl%>"><i class="fas fa-exclamation-circle"></i>&nbsp;SWN Interface Log</a>
        </li>
    </ul>
    <div class="tab-content">
        <div id="Contacts" class="tab-pane active">
            <asp:UpdatePanel ID="pnlContacts" runat="server" RenderMode="Block" OnUnload="pnlContacts_Unload">
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
                    <asp:Repeater ID="rptContacts" runat="server" OnItemCommand="rptContacts_ItemCommand" OnItemCreated="rptContacts_ItemCreated">
                        <HeaderTemplate>
                            <table id="tblContacts" class="table table-striped">
                                <thead>
                                    <tr>
                                        <th>&nbsp;</th>
                                        <th>Name</th>
                                        <th>Title</th>
                                        <th>Email</th>
                                        <th>Active</th>
                                        <th>&nbsp;</th>
                                    </tr>
                                </thead>
                                <tbody>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <tr>
                                <td class="command-icon">
                                    <asp:HyperLink runat="server" ID="cmdEdit" NavigateUrl='<%#EditUrl("eid",DataBinder.Eval(Container.DataItem,"EmployeeId").ToString(),"EditContact") %>' ToolTip="Edit Employee Record"><i class="fa fa-pencil-alt"></i></asp:HyperLink>
                                </td>
                                <td><%#DataBinder.Eval(Container.DataItem,"FullName") %></td>
                                <td><a href='mailto:<%#DataBinder.Eval(Container.DataItem,"Email") %>'><%#DataBinder.Eval(Container.DataItem,"Email") %></a></td>
                                <td><%#DataBinder.Eval(Container.DataItem,"JobTitle") %></td>
                                <td><%#DataBinder.Eval(Container.DataItem,"IsActive").ToString()=="True"?"<i class=\"fas fa-check-square\"></i>":"<i class=\"fas fa-square\"></i>" %></td>
                                <td class="command-icon">
                                    <asp:LinkButton ID="cmdDelete" CssClass="confirm" runat="server" CausesValidation="false" CommandName="delete" CommandArgument='<%#DataBinder.Eval(Container.DataItem,"EmployeeId").ToString() %>'><i class="fa fa-trash"></i></asp:LinkButton>
                                </td>
                            </tr>
                        </ItemTemplate>
                        <FooterTemplate>
                            </tbody></table><hr />
                        </FooterTemplate>
                    </asp:Repeater>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </div>
</div>
<dnn:dnnjsinclude runat="server" filepath="https://cdn.datatables.net/1.13.4/js/jquery.dataTables.min.js" />
<dnn:dnnjsinclude runat="server" filepath="https://cdn.datatables.net/1.13.4/js/dataTables.bootstrap5.min.js" />
<dnn:dnncssinclude runat="server" filepath="https://cdn.datatables.net/1.13.4/css/dataTables.bootstrap5.min.css" />

<script type="text/javascript">
    (function ($, Sys) {
        $(document).ready(function () {
            Sys.Application.add_load(function (s, e) { PageInit(); });
            PageInit();
        });
    }(jQuery, window.Sys));

    function PageInit() {
        var table = $('#tblContacts').DataTable({
            "order": [[1, "asc"]],
            "oLanguage": {
                "sSearch": "Filter by Text"
            },
            "aoColumns": [
                { "bSortable": false },
                { "bSortable": true },
                { "bSortable": true },
                { "bSortable": false },
                { "bSortable": false },
                { "bSortable": false },]
        });
        $(".confirm").dnnConfirm({
            text: 'Are you sure you wish to Delete this Contact? ',
            yesText: 'Yes',
            noText: 'No',
            title: 'Delete Contact?'
        });

        $("#tblContacts_length").prepend("<a class='btn btn-primary btn-sm me-2' href='<%=ContactDetailUrl%>'><i class='fas fa-plus'></i> Add Contact</a>");
        table.draw();
    }
</script>
