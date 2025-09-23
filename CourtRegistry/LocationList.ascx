<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="LocationList.ascx.cs" Inherits="tjc.Modules.CourtRegistry.LocationList" %>
<fieldset class="bg-light border rounded mb-3 p-3">
    <div class="row">
        <div class="col-auto">
            <label for="drpCategory">Case Type</label>
            <asp:DropDownList runat="server" ID="drpCategory" ClientIDMode="Static" AutoPostBack="true" CssClass="form-control" OnSelectedIndexChanged="drpCategory_SelectedIndexChanged">
            </asp:DropDownList>
        </div>
        <div class="col-md-4">
            <label for="drpJacCode">JAC Code</label>
            <asp:DropDownList runat="server" ID="drpJacCode" ClientIDMode="Static" CssClass="form-control">
            </asp:DropDownList>
        </div>
        <div class="col-auto">
            <label for="drpLocations">Location</label>
            <asp:DropDownList ID="drpLocations" runat="server" ClientIDMode="Static" CssClass="form-control"></asp:DropDownList>
        </div>
        <div class="col-auto">
            <label for="drpYear">Year</label>
            <asp:DropDownList ID="drpYear" runat="server" ClientIDMode="Static" CssClass="form-control" AppendDataBoundItems="true"><asp:ListItem Text="2021" /></asp:DropDownList>
        </div>
    </div>
    <div class="mt-3">
        <asp:Button ID="cmdShow" runat="server" Text="Submit" CssClass="btn btn-primary" OnClick="cmdShow_Click" />
        <asp:Button ID="cmdPrint" runat="server" Text="Print" CausesValidation="false" CssClass="btn btn-default" OnClick="cmdPrint_Click"/>
    </div>
</fieldset>
<asp:Panel ID="pnlList" runat="server" CssClass="printPage">
    <div class="heading heading-border heading-middle-border heading-middle-border-center">
        <h2>
            <asp:Literal ID="ltHeader" runat="server"></asp:Literal>
        </h2>
    </div>
    <asp:Literal ID="ltSubHead" runat="server"></asp:Literal>
    <asp:Repeater ID="rptAttorney" runat="server" OnItemDataBound="rptAttorney_ItemDataBound">
        <HeaderTemplate>
            <h3><%=LocationName %></h3>
        </HeaderTemplate>
        <ItemTemplate>
            <hr />
            <div>
                <strong><%#Eval("LastName") %>, <%#Eval("FirstName") %></strong><br />
                <%# Eval("Address")%> <%# Eval("City")%>, <%# Eval("State")%> <%# Eval("Zip")%><br />
                <%# Eval("Phone")%> <%# Eval("Email")%>
                <ul>
                    <asp:Repeater ID="rptJacCodes" runat="server">
                        <ItemTemplate>
                            <li>
                                <%# Eval("Category")%></li>
                        </ItemTemplate>
                    </asp:Repeater>
                </ul>
            </div>
        </ItemTemplate>
    </asp:Repeater>
</asp:Panel>

