<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SubmissionList.ascx.cs" Inherits="tjc.Modules.CDSPAdmin.Views.SubmissionList" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>

<%-- DataTables (DNN shared) --%>
<dnn:DnnCssInclude runat="server" FilePath="/Resources/Libraries/DataTables/dataTables.bootstrap5.min.css" />
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/DataTables/dataTables.min.js" />
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/DataTables/dataTables.bootstrap5.min.js" />
<%-- SweetAlert2 (DNN shared) for confirms / toasts --%>
<dnn:DnnCssInclude runat="server" FilePath="/Resources/Libraries/sweetalert/sweetalert2.min.css" />
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/sweetalert/sweetalert2.all.min.js" />
<%-- Module behavior --%>
<dnn:DnnJsInclude runat="server" FilePath="~/DesktopModules/tjc.modules/CDSPAdmin/Scripts/cdsp-list.js" Priority="200" />

<div class="container-fluid cdsp-list">
    <h3><i class="fas fa-inbox"></i>&nbsp;CDSP Submissions</h3>

    <%-- Web API context for the JS layer. The __RequestVerificationToken hidden
         field is injected by ServicesFramework.RequestAjaxAntiForgerySupport in
         Page_Load; cdsp-list.js reads it and sends it on every AJAX call. --%>
    <script type="text/javascript">
        window.__cdspCtx = {
            moduleId: <%= ModuleId %>,
            tabId: <%= TabId %>,
            serviceRoot: '<%= ResolveUrl("~/DesktopModules/CDSPAdmin/API/") %>'
        };
    </script>

    <div class="form-check form-switch cdsp-toggle">
        <input class="form-check-input" type="checkbox" role="switch" id="cdspShowCompleted" />
        <label class="form-check-label" for="cdspShowCompleted">Show completed submissions</label>
    </div>

    <asp:Repeater ID="rptSubmissions" runat="server">
        <HeaderTemplate>
            <table id="tblSubmissions" class="table table-striped table-bordered table-hover cdsp-table">
                <thead>
                    <tr>
                        <th class="no-sort cdsp-col-icon"></th>
                        <th>Submitted</th>
                        <th>Division</th>
                        <th>County</th>
                        <th>Complainant</th>
                        <th>Phone</th>
                        <th>Email</th>
                        <th>Status</th>
                        <th class="no-sort cdsp-col-icon"></th>
                    </tr>
                </thead>
                <tbody>
        </HeaderTemplate>
        <ItemTemplate>
            <tr data-id='<%# Eval("SubmissionID") %>' data-completed='<%# ((bool)Eval("Completed")) ? "1" : "0" %>'>
                <td class="cdsp-col-icon">
                    <a href="#" class="cdsp-icon-btn cdsp-view text-primary" data-id='<%# Eval("SubmissionID") %>' title="View details"><i class="fas fa-search"></i></a>
                </td>
                <td data-order='<%# Eval("CreatedDate", "{0:yyyyMMddHHmmss}") %>'><%# Eval("CreatedDate", "{0:MM/dd/yyyy}") %></td>
                <td><%# Server.HtmlEncode(Convert.ToString(Eval("Division"))) %></td>
                <td><%# Server.HtmlEncode(Convert.ToString(Eval("County"))) %></td>
                <td><%# Server.HtmlEncode(Convert.ToString(Eval("ComplainantName"))) %></td>
                <td><%# Server.HtmlEncode(Convert.ToString(Eval("Phone"))) %></td>
                <td><%# Server.HtmlEncode(Convert.ToString(Eval("Email"))) %></td>
                <td class="cdsp-status">
                    <%# ((bool)Eval("Completed"))
                        ? "<span class=\"badge bg-success\">Completed</span>"
                        : "<span class=\"badge bg-warning text-dark\">Open</span>" %>
                </td>
                <td class="cdsp-col-icon">
                    <a href="#" class="cdsp-icon-btn cdsp-toggle" data-id='<%# Eval("SubmissionID") %>' title="Toggle completed">
                        <i class='<%# ((bool)Eval("Completed")) ? "fas fa-check-square text-success" : "far fa-square text-muted" %>'></i>
                    </a>
                </td>
            </tr>
        </ItemTemplate>
        <FooterTemplate>
                </tbody>
            </table>
        </FooterTemplate>
    </asp:Repeater>
</div>

<%-- Detail modal (populated via AJAX) --%>
<div class="modal fade cdsp-modal" id="cdspDetailModal" tabindex="-1" aria-hidden="true" aria-labelledby="cdspDetailTitle">
    <div class="modal-dialog modal-xl modal-dialog-scrollable">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title" id="cdspDetailTitle"><i class="fas fa-file-alt"></i>&nbsp;Submission Detail</h5>
                <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
            </div>
            <div class="modal-body cdsp-detail" id="cdspDetailBody"></div>
            <div class="modal-footer">
                <button type="button" class="btn cdsp-modal-toggle" id="cdspModalToggle"></button>
                <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Close</button>
            </div>
        </div>
    </div>
</div>
