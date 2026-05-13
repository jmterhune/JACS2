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
            <asp:DropDownList ID="drpYear" runat="server" ClientIDMode="Static" CssClass="form-control"></asp:DropDownList>
        </div>
    </div>
    <div class="mt-3">
        <asp:Button ID="cmdShow" runat="server" Text="Submit" CssClass="btn btn-primary" OnClick="cmdShow_Click" />
        <asp:Button ID="cmdPrint" runat="server" Text="Print" CausesValidation="false" CssClass="btn btn-default" OnClick="cmdPrint_Click"/>
    </div>
</fieldset>
<div class="row">
    <div class="col-md-4 ms-auto">
        <input type="search" id="txtListSearch" class="form-control" placeholder="Search list..." autocomplete="off" />
    </div>
</div>
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
            <div class="attorney-row">
                <hr />
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
<script type="text/javascript">
    (function () {
        var input = document.getElementById('txtListSearch');
        if (!input) return;
        input.addEventListener('input', function () {
            var q = input.value.trim().toLowerCase();
            document.querySelectorAll('.attorney-row').forEach(function (row) {
                if (q === '' || row.textContent.toLowerCase().indexOf(q) !== -1) {
                    row.style.display = '';
                } else {
                    row.style.display = 'none';
                }
            });
        });
    })();
</script>
