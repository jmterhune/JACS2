<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Case.ascx.cs" Inherits="tjc.Modules.ProSeLog.Case" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<asp:HyperLink ID="lnkManage" Visible="false" CssClass="btn btn-danger mb-3" runat="server">Manage Lists</asp:HyperLink>
<div class="tabs">
    <ul class="nav nav-tabs">
        <li class="nav-item">
            <a class="nav-link" href="<%=LogListUrl %>"><i class="fas fa-search"></i>&nbsp;Search</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=FormUrl %>">Data Entry</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="<%=StatsUrl %>">Monthly Stats</a>
        </li>
    </ul>
    <div class="tab-content">
        <div id="caseDetail" class="tab-pane active">
            <asp:Literal ID="ltMessage" runat="server" />
            <div id="ProseForm">
                <div class="bg-light border rounded mb-3 p-3">
                    <div class="row">
                        <div class="col-auto">
                            <div class="row">
                                    <label for='txtCaseNumber' class="col col-form-label fw-bold text-end">Case Number:</label>
                                <div class="col-auto">
                                    <asp:TextBox ID="txtCaseNumber" ClientIDMode="Static" CssClass="form-control-plaintext" runat="server" />
                                </div>
                            </div>
                            <div class="row">
                                    <label for='txtCaseName' class="col col-form-label fw-bold text-end">Case Name:</label>
                                <div class="col-auto">
                                    <asp:TextBox ID="txtCaseName" ClientIDMode="Static" CssClass="form-control-plaintext" runat="server" />
                                </div>
                            </div>
                        </div>
                        <div class="col-auto">
                            <div class="row">
                                    <label for='txtPetitioner' class="col col-form-label fw-bold text-end">Petitioner:</label>
                                <div class="col-auto">
                                    <asp:TextBox ID="txtPetitioner" ClientIDMode="Static" CssClass="form-control-plaintext" runat="server" />
                                </div>
                            </div>
                            <div class="row">
                                    <label for='txtRespondent' class="col col-form-label fw-bold text-end">Respondent:</label>
                                <div class="col-auto">
                                    <asp:TextBox ID="txtRespondent" ClientIDMode="Static" CssClass="form-control-plaintext" runat="server" />
                                </div>
                            </div>
                        </div>
                        <div class="col-auto">
                            <div class="row">
                                    <label for='txtCaseType' class="col col-form-label fw-bold text-end">Case Type:</label>
                                <div class="col-auto">
                                    <asp:TextBox ID="txtCaseTypeName" ClientIDMode="Static" CssClass="form-control-plaintext" runat="server" />
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <asp:Repeater ID="rptHistoryList" runat="server">
                <HeaderTemplate>
                    <table id="tblHistory" class="table table-striped">
                        <thead>
                            <tr>
                                <th>&nbsp;</th>
                                <th>Received</th>
                                <th>Action Taken</th>
                                <th>Completed</th>
                            </tr>
                        </thead>
                        <tbody>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr>
                        <td class="command-item">
                            <asp:HyperLink ID="lnkView" runat="server" NavigateUrl='<%#EditUrl("hid",Eval("HistoryID").ToString(),"form") %>'><i class="fa fa-pencil"></i></asp:HyperLink>
                        </td>
                        <td><%#DataBinder.Eval(Container.DataItem,"ReceivedDate","{0:MM/dd/yyyy}") %></td>
                        <td>
                            <%#Convert.ToBoolean(Eval("NeedsLetter")) ? "Needs Letter, ": ""%>
                            <%#Convert.ToBoolean(Eval("ProvidedForms")) ? "Provided Forms, ": ""%>
                            <%#Convert.ToBoolean(Eval("AssistedForms")) ? "Assisted w/ Forms, ": ""%>
                            <%#Convert.ToBoolean(Eval("AssistedProcedures")) ? "Assisted w/ Procedures, ": ""%>
                            <%#Convert.ToBoolean(Eval("SetFinalHearing")) ? "Set Final Hearing, ": ""%>
                            <%#Convert.ToBoolean(Eval("SetOtherHearing")) ? "Set Other Hearing, ": ""%>
                            <%#Convert.ToBoolean(Eval("ReferralOther")) ? "Referral Other, ": ""%>
                            <%#Convert.ToBoolean(Eval("ReferralGmMag")) ? "Referral GM/MAG, ": ""%>
                            <%#Convert.ToBoolean(Eval("PreparedOrder")) ? "Prepared Order, ": ""%>
                            <%#Convert.ToBoolean(Eval("Other")) ? "Other, ": ""%>
                            <%#Convert.ToBoolean(Eval("AppointedPro")) ? "Appointed Professional, ": ""%>
                        </td>
                        <td><%#DataBinder.Eval(Container.DataItem,"Resolution").ToString()=="True"?"<i class=\"fas fa-check-square\"></i>":"<i class=\"fas fa-square\"></i>" %></td>
                    </tr>
                </ItemTemplate>
                <FooterTemplate>
                    </tbody>
                </table>
                </FooterTemplate>
            </asp:Repeater>
            <hr />
            <p>
                <asp:HyperLink ID="lnkNewProject" runat="server" Text="New Project" CssClass="btn btn-primary" />
            </p>
        </div>
    </div>
</div>
</div>
<dnn:dnncssinclude runat="server" filepath="/Resources/Libraries/DataTables/dataTables.bootstrap5.min.css" />
<dnn:dnnjsinclude runat="server" filepath="/Resources/Libraries/DataTables/dataTables.min.js" />
<dnn:dnnjsinclude runat="server" filepath="/Resources/Libraries/DataTables/dataTables.bootstrap5.min.js" />
<script type="text/javascript">
    (function ($, Sys) {

        $(document).ready(function () {
            Sys.Application.add_load(function (s, e) { PageInit(); });
            PageInit();
        });
    }(jQuery, window.Sys));

    function PageInit() {
        var table = $('#tblHistory').DataTable({
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
    }
</script>
