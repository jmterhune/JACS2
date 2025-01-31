<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Library.ascx.cs" Inherits="tjc.Modules.CourtCounsel.Library" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<nav class="navbar navbar-expand-lg navbar-dark bg-dark mb-md">
    <button class="navbar-toggler" type="button" data-toggle="collapse" data-target="#navbarNav" aria-controls="navbarNav" aria-expanded="false" aria-label="Toggle navigation">
        <span class="navbar-toggler-icon"></span>
    </button>
    <div class="collapse navbar-collapse" id="navbarNav">
        <ul class="navbar-nav">

            <li class="active nav-item">
                <asp:HyperLink CssClass="nav-link" ID="lnkSearch" runat="server"><i class="fas fa-search"></i>&nbsp;Search</asp:HyperLink>
            </li>
            <li class="nav-item">
                <a class="nav-link" href="<%=EditUrl("logEdit") %>"><i class="fas fa-pencil-alt"></i>&nbsp;Data Entry</a>
            </li>
            <li class="nav-item">
                <a class="nav-link" href="<%=EditUrl("reports") %>"><i class="fas fa-chart-bar"></i>&nbsp;Reports</a>
            </li>
            <li class="nav-item">
                <a class="nav-link" href="<%=EditUrl("calendar") %>"><i class="fas fa-calendar"></i>&nbsp;Event Calendar</a>
            </li>
            <li class="nav-item">
                <a class="nav-link" href="<%=EditUrl("library") %>"><i class="fas fa-folder-open"></i>&nbsp;Document Repository</a>
            </li>
            <li class="nav-item" id="li1" runat="server" visible="false">
                <a class="nav-link" href="<%=EditUrl("admin") %>"><i class="fa fa-tools"></i>&nbsp;Admin</a>
            </li>
            <li class="nav-item">
                <a class="nav-link" href="<%=SharePointSiteURL %>"><i class="fas fa-home"></i>&nbsp;Team Site</a>
            </li>
        </ul>

    </div>
</nav>
<div class="mb-md">
    <div class="btn-group" role="group" aria-label="Search">
        <div class="btn-group" role="group">
            <button id="btnSearchType" type="button" class="btn btn-primary dropdown-toggle" data-toggle="dropdown" aria-expanded="false">
                Search Type
            </button>
            <ul class="dropdown-menu" aria-labelledby="btnSearchType">
                <li><a class="dropdown-item" onclick="SetSearchType(0)" href="#">My Recent</a></li>
                <li><a class="dropdown-item" href="#" onclick="SetSearchType(1)">Case Name</a></li>
                <li><a class="dropdown-item" href="#" onclick="SetSearchType(2)">Case Number</a></li>
                <li><a class="dropdown-item" href="#" onclick="SetSearchType(3)">Attorney</a></li>

            </ul>
        </div>
        <div id="swAttorney" class="input-group">
            <asp:DropDownList ID="drpAttorney" runat="server" CssClass="form-control" aria-label="Select Attorney" ClientIDMode="Static">
            </asp:DropDownList>
        </div>
        <div id="swSearchTerm" class="input-group">
            <asp:TextBox ID="txtSearchTerm" ClientIDMode="Static" runat="server" CssClass="form-control" placeholder="Search Term" aria-label="Search Term" aria-describedby="lblSearchTerm"></asp:TextBox>
        </div>
        <div id="swActive" class="input-group ml-md switch">
            <div class="custom-control custom-switch">
                <asp:CheckBox ID="chkActive" runat="server" ClientIDMode="Static" />
                <label class="custom-control-label" for="chkActive">Active</label>
            </div>
        </div>
        <div id="swPending" class="input-group switch">
            <div class="custom-control custom-switch">
                <asp:CheckBox ID="chkPending" runat="server" ClientIDMode="Static" />
                <label class="custom-control-label" for="chkPending">Pending</label>
            </div>
        </div>
        <div id="swClosed" class="input-group switch">
            <div class="custom-control custom-switch">
                <asp:CheckBox ID="chkClosed" runat="server" ClientIDMode="Static" />
                <label class="custom-control-label" for="chkClosed">Closed</label>
            </div>
        </div>
    </div>
    <asp:Button ID="cmdSearch" OnClick="cmdSearch_Click" ClientIDMode="Static" runat="server" Text="Search" ToolTip="Search Court Counsel Records" CssClass="btn btn-primary" />
    <asp:HiddenField ID="hdSearchType" runat="server" ClientIDMode="Static" Value="0" />
</div>

<asp:UpdatePanel ID="pnlUpdate" runat="server">

    <ContentTemplate>
        <asp:UpdateProgress ID="upProgress" runat="server">
            <ProgressTemplate>
                <div class="modal-progress">
                    <div class="center-progress">
                        <img alt="" src="/images/loading.gif" />
                    </div>
                </div>
            </ProgressTemplate>
        </asp:UpdateProgress>
        <asp:Repeater ID="rptLogEntries" runat="server" OnItemDataBound="rptLogEntries_ItemDataBound" OnItemCommand="rptLogEntries_ItemCommand">
            <HeaderTemplate>
                <table id="log-list" class="table table-striped">
                    <thead>
                        <tr>
                            <th>&nbsp;</th>
                            <th>Case Number</th>
                            <th>Case Name</th>
                            <th>Case Type</th>
                            <th>Action Date</th>
                            <th>Motion Filed</th>
                            <th>Responsible</th>
                            <th>Status</th>
                            <th>&nbsp;</th>

                        </tr>
                    </thead>
                    <tbody>
            </HeaderTemplate>

            <ItemTemplate>
                <tr data-id="<%#DataBinder.Eval(Container.DataItem,"LogId").ToString() %>">
                    <td class="command-icon"><a href="<%#EditUrl("aid",DataBinder.Eval(Container.DataItem,"AssignmentId").ToString(),"logedit") %>"><i title="View Assignment Record" class="fa fa-pencil-alt"></i></a></td>
                    <td><%#DataBinder.Eval(Container.DataItem,"CaseNumber") %></td>
                    <td><%#DataBinder.Eval(Container.DataItem,"Description") %></td>
                    <td><%#DataBinder.Eval(Container.DataItem,"CaseTypeName") %></td>
                    <td><%#DataBinder.Eval(Container.DataItem,"DateReceived", "{0:M/d/yy}") %></td>
                    <td><%#DataBinder.Eval(Container.DataItem,"MotionFiled", "{0:M/d/yy}") %></td>
                    <td><%#DataBinder.Eval(Container.DataItem,"AttorneyName") %></td>
                    <td><%#DataBinder.Eval(Container.DataItem,"PhaseName") %></td>
                    <td class="command-icon"><a href="<%#EditUrl("lid",DataBinder.Eval(Container.DataItem,"LogId").ToString(),"caseview") %>"><i title="View Related Assignment Records" class="fa fa-arrow-circle-down"></i></a></td>

                </tr>

            </ItemTemplate>
            <FooterTemplate>
                </tbody></table>
            </FooterTemplate>
        </asp:Repeater>
    </ContentTemplate>
    <Triggers>
        <asp:AsyncPostBackTrigger ControlID="cmdSearch" EventName="Click" />
    </Triggers>
</asp:UpdatePanel>

<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/DataTables/jquery.dataTables.min.js" />
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/DataTables/dataTables.bootstrap5.min.js" />
<dnn:DnnCssInclude runat="server" FilePath="/Resources/Libraries/DataTables/dataTables.bootstrap5.min.css" />

<script>
    (function ($, Sys) {
        $(document).ready(function () {
            InitializeForm();
            Sys.Application.add_load(function (s, e) { PageInit(); });
            PageInit();
        });
    }(jQuery, window.Sys));
    function PageInit() {

        var table = $('#log-list').DataTable({

            "order": [[3, "desc"]],
            "oLanguage": {

                "sSearch": "Filter by Text"

            },
        });

    }
    function InitializeForm() {
        InitializeAttorneyDropDown();
        var cookie = GetCookie('SearchCookie');
        if (cookie) {
            CookieFieldInitialization(cookie);
        } else {
            DefaultFieldInitialization();
        }

    }
    function DefaultFieldInitialization() {
        $("#swAttorney").hide();
        $("#swSearchTerm").hide();
        $("#swActive").hide();
        $("#swPending").hide();
        $("#swClosed").hide();
        $("#btnSearchType").text("My Recent");
    }
    function CookieFieldInitialization(cookieValuePair) {
        var attorneyId = cookieValuePair["AttorneyId"];
        var searchType = cookieValuePair["SearchType"];
        if (attorneyId) { $("#drpAttorney option[value='" + attorneyId + "']").attr('selected', 'selected'); }
        if (searchType) { SetSearchType(Number($("#hdSearchType").val())); }
    }
    function InitializeAttorneyDropDown() {
        var $select = $('#drpAttorney');
        var optGroup;
        $('#drpAttorney option').each(function () {
            if ($(this).val() == '<') {
                /* Opener */
                optGroup = $('<optGroup>').attr('label', $(this).text());
            } else if ($(this).val() == '>') {
                /* Closer */
                $('</optGroup>').appendTo(optGroup);
                optGroup.appendTo($select);
                optGroup = null;
            } else {
                /* Normal Item */
                if (optGroup) {
                    $('<option class="inactive">' + $(this).text() + '</option>').attr('value', $(this).val()).appendTo(optGroup);
                } else {
                    $('<option>' + $(this).text() + '</option>').attr('value', $(this).val()).appendTo($select);
                }
            }
            $(this).remove();
        });

    }
    function SetSearchType(searchType) {
        // e.preventDefault();
        $("#hdSearchType").val(searchType);
        switch (searchType) {
            case 0: txtSearchTerm
                $("#swAttorney").hide();
                $("#swSearchTerm").hide();
                $("#swActive").hide();
                $("#swPending").hide();
                $("#swClosed").hide();
                $("#btnSearchType").text("My Recent");
                break;
            case 1:
                $("#swAttorney").hide();
                $("#swSearchTerm").show();
                $("#swActive").hide();
                $("#swPending").hide();
                $("#swClosed").hide();
                $("#btnSearchType").text("Case Name");

                break;
            case 2:
                $("#swAttorney").hide();
                $("#swSearchTerm").show();
                $("#swActive").hide();
                $("#swPending").hide();
                $("#swClosed").hide();
                $("#btnSearchType").text("Case Number");

                break;
            case 3:
                $("#swAttorney").show();
                $("#swSearchTerm").hide();
                $("#swActive").show();
                $("#swPending").show();
                $("#swClosed").show();
                $("#btnSearchType").text("Attorney");
                break;
            default:
        }
    }

    function GetCookie(name) {
        let matches = document.cookie.match(new RegExp(
            "(?:^|; )" + name.replace(/([\.$?*|{}\(\)\[\]\\\/\+^])/g, '\\$1') + "=([^;]*)"
        ));
        var valueStr = matches ? decodeURIComponent(matches[1]) : undefined;
        var jsonStr = '{"' + valueStr.replace(/&/g, '", "').replace(/=/g, '": "') + '"}';
        return JSON.parse(jsonStr);

    }
</script>
