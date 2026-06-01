<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Stats.ascx.cs" Inherits="tjc.Modules.ProSeLog.Stats" %>
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
        <li class="nav-item active">
            <a class="nav-link" href="#stats" data-toggle="tab">Search</a>
        </li>
    </ul>
    <div class="tab-content">
        <div id="stats" class="tab-pane active">
             <div class="bg-light border rounded mb-3 p-3">
                <div class="row">
                    <div class="col-6">
                        <div class="row form-group">
                            <div class="col-auto">
                                <asp:Label runat="server" AssociatedControlID="drpMonths" Text="Month / Year" />
                                <div class="input-group">
                                    <asp:DropDownList ID="drpMonths" runat="server" CssClass="form-control" />
                                    <asp:DropDownList ID="drpYear" runat="server" CssClass="form-control" style="min-width:70px" />
                                </div>
                            </div>
                            <div class="col-auto">
                                <asp:Label runat="server" AssociatedControlID="drpCounty" Text="County" ToolTip="Search By County" />
                                <asp:DropDownList runat="server" CssClass="form-control" ID="drpCounty" DataTextField="CountyName" DataValueField="CountyID" AppendDataBoundItems="true">
                                    <asp:ListItem Value="0">&lt; Filter By Location &gt;</asp:ListItem>
                                </asp:DropDownList>
                            </div>
                            <div class="col-auto  mt-4">
                                <asp:Button ID="cmdSubmit" runat="server" CausesValidation="false" CssClass="btn btn-primary" Text="Submit" OnClick="cmdSubmit_Click" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <asp:Literal ID="ltMessage" runat="server" />
            <table id="Stats" class="table table-striped">
                <thead>
                    <tr>
                        <th>Case Type</th>
                        <th>
                            <abbr title="Simple Dissolution of Marriage">Simp DOM</abbr>
                        </th>
                        <th>
                            <abbr title="Dissolution of Marriage">DOM</abbr>
                        </th>
                        <th>
                            <abbr title="Dissolution of Marriage with Children">DOM w/Ch</abbr>
                        </th>
                        <th>
                            <abbr title="Name Change">NC</abbr>
                        </th>
                        <th>
                            <abbr title="Stepparent Adoption">SPA</abbr>
                        </th>
                        <th>
                            <abbr title="Temporary Custody">CUST</abbr>
                        </th>
                        <th>
                            <abbr title="Modification">MOD</abbr>
                        </th>
                        <th>
                            <abbr title="Contempt">CONT</abbr>
                        </th>
                        <th>
                            <abbr title="Paternity">PAT</abbr>
                        </th>
                        <th>
                            <abbr title="Child Support">CS</abbr>
                        </th>
                        <th>
                            <abbr title="Other">OT</abbr>
                        </th>
                        <th>Total</th>
                    </tr>
                </thead>
                <tbody>
                    <tr class="bg-dark text-white">
                        <th colspan="13">Initial Contact</th>
                    </tr>
                    <asp:Repeater ID="rptContact" runat="server">
                        <ItemTemplate>
                            <tr>
                                <th>
                                    <%#Eval("FieldName")%></th>
                                <td>
                                    <%#Eval("SimpDOM")%></td>
                                <td>
                                    <%#Eval("DOM")%></td>
                                <td>
                                    <%#Eval("DOMCH")%></td>
                                <td>
                                    <%#Eval("NC")%></td>
                                <td>
                                    <%#Eval("SPA")%></td>
                                <td>
                                    <%#Eval("CUST")%></td>
                                <td>
                                    <%#Eval("MODIF")%></td>
                                <td>
                                    <%#Eval("CONT")%></td>
                                <td>
                                    <%#Eval("PAT")%></td>
                                <td>
                                    <%#Eval("CS")%></td>
                                <td>
                                    <%#Eval("Other")%></td>
                                <td>
                                    <strong><%#Eval("Total")%></strong></td>
                            </tr>
                        </ItemTemplate>
                    </asp:Repeater>
                    <tr class="bg-dark text-white">
                        <th colspan="13">Initial Resolution</th>
                    </tr>
                    <asp:Repeater ID="rptResolution" runat="server">
                        <ItemTemplate>
                            <tr>
                                <th>
                                    <%#Eval("FieldName")%></th>
                                <td>
                                    <%#Eval("SimpDOM")%></td>
                                <td>
                                    <%#Eval("DOM")%></td>
                                <td>
                                    <%#Eval("DOMCH")%></td>
                                <td>
                                    <%#Eval("NC")%></td>
                                <td>
                                    <%#Eval("SPA")%></td>
                                <td>
                                    <%#Eval("CUST")%></td>
                                <td>
                                    <%#Eval("MODIF")%></td>
                                <td>
                                    <%#Eval("CONT")%></td>
                                <td>
                                    <%#Eval("PAT")%></td>
                                <td>
                                    <%#Eval("CS")%></td>
                                <td>
                                    <%#Eval("Other")%></td>
                                <td>
                                    <strong><%#Eval("Total")%></strong></td>
                            </tr>
                        </ItemTemplate>
                    </asp:Repeater>
                </tbody>
            </table>
        </div>
    </div>
</div>

<script type="text/javascript">
    (function ($, Sys) {

        $(document).ready(function () {
            Sys.Application.add_load(function (s, e) { PageInit(); });
            PageInit();
        });

    }(jQuery, window.Sys));

    function PageInit() {
    }
</script>
