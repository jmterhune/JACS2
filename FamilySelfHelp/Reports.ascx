<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Reports.ascx.cs" Inherits="tjc.Modules.FamilySelfHelp.Reports" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>

<div id="LogForm">
    <div class="container">
        <div class="btn-group mb-2">
            <asp:HyperLink ID="lnkSearch" CssClass="btn btn-primary" runat="server">Search</asp:HyperLink>
            <asp:HyperLink ID="lnkDataEntry" CssClass="btn btn-primary" runat="server">Data Entry</asp:HyperLink>
            <asp:HyperLink ID="lnkMerge" CssClass="btn btn-primary" runat="server">Merge Clients</asp:HyperLink>
            <asp:HyperLink ID="lnkReports" CssClass="btn btn-primary active" runat="server">Reports</asp:HyperLink>
        </div>
        <div class="row mb-2">
            <div class="col-auto">
                <asp:Label runat="server" AssociatedControlID="txtStartDate" Text="Start Date<em>*</em>" ToolTip="Required" />
                <asp:TextBox runat="server" CssClass="form-control  form-control-sm datepicker" MaxLength="50" ID="txtStartDate" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtStartDate"
                    Display="Dynamic" SetFocusOnError="true" CssClass="label label-danger" ErrorMessage="Start Date is Required" />
                <asp:CompareValidator ID="valIsStartDate" ControlToValidate="txtStartDate" Type="Date" Operator="DataTypeCheck" runat="server" Display="Dynamic" SetFocusOnError="true" CssClass="label label-danger" ErrorMessage="Invalid Date"></asp:CompareValidator>
                <asp:CompareValidator ID="valCompareDates" ControlToCompare="txtStartDate"
                    ControlToValidate="txtEndDate" Type="Date" Operator="GreaterThanEqual" Display="Dynamic"
                    ErrorMessage="Start Date must be less than End Date" runat="server"></asp:CompareValidator>
            </div>
            <div class="col-auto">
                <asp:Label runat="server" AssociatedControlID="txtEndDate" Text="End Date<em>*</em>" ToolTip="Required" />
                <asp:TextBox runat="server" CssClass="form-control  form-control-sm datepicker" MaxLength="50" ID="txtEndDate" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtEndDate"
                    Display="Dynamic" SetFocusOnError="true" CssClass="label label-danger" ErrorMessage="End Date is Required" />
                <asp:CompareValidator ID="valIsEndDate" ControlToValidate="txtEndDate" Type="Date" Operator="DataTypeCheck" runat="server" Display="Dynamic" SetFocusOnError="true" CssClass="label label-danger" ErrorMessage="Invalid Date"></asp:CompareValidator>
            </div>
            <div class="col-auto">
                <asp:Label runat="server" AssociatedControlID="drpDivisions" Text="Select Division" />
                <asp:DropDownList ID="drpDivisions" runat="server" CssClass="form-control form-control-sm">
                    <asp:ListItem Text="*** All Divisions ***" Value="All" />
                    <asp:ListItem Text="Family Division 1" />
                    <asp:ListItem Text="Family Division 2" />
                    <asp:ListItem Text="Family Division 3" />
                    <asp:ListItem Text="Family Division 4" />
                    <asp:ListItem Text="Family Division H" />
                    <asp:ListItem Text="Family DeSoto Division" />
                </asp:DropDownList>
            </div>
        </div>
        <p>
            <asp:Button ID="cmdReport" runat="server" Text="Submit" OnClick="cmdReport_Click" CssClass="btn btn-primary" />
        </p>
        <hr />
    </div>
    <p>
        <asp:Literal ID="ltMessage" runat="server" />
    </p>

    <asp:Panel ID="pnlReport" runat="server" Visible="false">
        <asp:Repeater ID="rptClientTypes" runat="server">
            <HeaderTemplate>
                <h4>Client Types</h4>
                <table class="table table-striped">
                    <thead>
                        <tr>
                            <th>Client Type</th>
                            <th>Count</th>
                        </tr>
                    </thead>
                    <tbody>
            </HeaderTemplate>
            <ItemTemplate>
                <tr>
                    <td><%#Eval("client") %></td>
                    <td><%#Eval("clientCount") %></td>
                </tr>
            </ItemTemplate>
            <FooterTemplate>
                </tbody>
                </table>
            </FooterTemplate>
        </asp:Repeater>
        <asp:Repeater ID="rptContactMethod" runat="server">
            <HeaderTemplate>
                <h4>Contact Methods</h4>
                <table class="table table-striped">
                    <thead>
                        <tr>
                            <th>Method</th>
                            <th>Count</th>
                        </tr>
                    </thead>
                    <tbody>
            </HeaderTemplate>
            <ItemTemplate>
                <tr>
                    <td><%#Eval("Method") %></td>
                    <td><%#Eval("MethodCount") %></td>
                </tr>
            </ItemTemplate>
            <FooterTemplate>
                </tbody>
                </table>
            </FooterTemplate>
        </asp:Repeater>

        <asp:Repeater ID="rptCaseType" runat="server">
            <HeaderTemplate>
                <h4>Case Types</h4>
                <table class="table table-striped">
                    <thead>
                        <tr>
                            <th>Case Type</th>
                            <th>Count</th>
                        </tr>
                    </thead>
                    <tbody>
            </HeaderTemplate>
            <ItemTemplate>
                <tr>
                    <td><%#Eval("casetype") %></td>
                    <td><%#Eval("CaseTypeCount") %></td>
                </tr>
            </ItemTemplate>
            <FooterTemplate>
                </tbody>
                </table>
            </FooterTemplate>
        </asp:Repeater>
        <asp:Repeater ID="rptServiceProvided" runat="server">
            <HeaderTemplate>
                <h4>Services Provided</h4>
                <table class="table table-striped">
                    <thead>
                        <tr>
                            <th>Service</th>
                            <th>Count</th>
                        </tr>
                    </thead>
                    <tbody>
            </HeaderTemplate>
            <ItemTemplate>
                <tr>
                    <td><%#Eval("Service") %></td>
                    <td><%#Eval("ServiceCount") %></td>
                </tr>
            </ItemTemplate>
            <FooterTemplate>
                </tbody>
                </table>
            </FooterTemplate>
        </asp:Repeater>
        <asp:Repeater ID="rptDivision" runat="server">
            <HeaderTemplate>
                <h4>Division Counts</h4>
                <table class="table table-striped">
                    <thead>
                        <tr>
                            <th>Division</th>
                            <th>Count</th>
                        </tr>
                    </thead>
                    <tbody>
            </HeaderTemplate>
            <ItemTemplate>
                <tr>
                    <td><%#Eval("Division") %></td>
                    <td><%#Eval("DivisionCount") %></td>
                </tr>
            </ItemTemplate>
            <FooterTemplate>
                </tbody>
                </table>
            </FooterTemplate>
        </asp:Repeater>

        <h4>Other Counts</h4>
        <table class="table table-striped">
            <thead>
                <tr>
                    <th>Interpreter Requested</th>
                    <th>New Cases</th>
                    <th>Total Time</th>
                    <th>Average Time</th>
                    <th>Customer Count
                        <br />
                        (does not include return visits)</th>
                </tr>
            </thead>
            <tbody>
                <tr>
                    <td>
                        <asp:Literal ID="ltInterpreter" runat="server" /></td>
                    <td>
                        <asp:Literal ID="ltCase" runat="server" /></td>
                    <td>
                        <asp:Literal ID="ltTotal" runat="server" /></td>
                    <td>
                        <asp:Literal ID="ltAverage" runat="server" /></td>
                    <td>
                        <asp:Literal ID="ltCustomerTotal" runat="server" /></td>
                </tr>
            </tbody>
        </table>

    </asp:Panel>
</div>
<dnn:dnncssinclude runat="server" filepath="~/Resources/Shared/components/TimePicker/Themes/jquery-ui.min.css" />

<script>
    var moduleId = <%=ModuleId%>;

    $(function () {
        $(".datepicker").datepicker();
    });

</script>
