<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="View.ascx.cs" Inherits="tjc.Modules.DeSoto.Probation.View" %>
<asp:Repeater ID="rptItemList" runat="server" OnItemDataBound="rptItemListOnItemDataBound" OnItemCommand="rptItemListOnItemCommand">
    <HeaderTemplate>
        <ul class="tm_tl">
    </HeaderTemplate>

    <ItemTemplate>
        <li class="tm_t">
            <h3>
                <asp:Label ID="lblitemName" runat="server" Text='<%#DataBinder.Eval(Container.DataItem,"ItemName").ToString() %>' />
            </h3>
            <asp:Label ID="lblItemDescription" runat="server" Text='<%#DataBinder.Eval(Container.DataItem,"ItemDescription").ToString() %>' CssClass="tm_td" />

            <asp:Panel ID="pnlAdmin" runat="server" Visible="false">
                <asp:HyperLink ID="lnkEdit" runat="server" ResourceKey="EditItem.Text" Visible="false" Enabled="false" />
                <asp:LinkButton ID="lnkDelete" runat="server" ResourceKey="DeleteItem.Text" Visible="false" Enabled="false" CommandName="Delete" />
            </asp:Panel>
        </li>
    </ItemTemplate>
    <FooterTemplate>
        </ul>
    </FooterTemplate>
</asp:Repeater>


<div>
    <div class="form-group">
        <label for="Program">Program</label>
        <div>
            <select id="Program" name="Program" required="required" class="custom-select">
                <option value="0">Rabbit</option>
                <option value="1">Duck</option>
                <option value="2">Fish</option>
            </select>
        </div>
    </div>
    <div class="form-group">
        <label for="FirstName">First Name</label>
        <input id="FirstName" name="FirstName" type="text" required="required" class="form-control">
    </div>
    <div class="form-group">
        <label for="LastName">Last Name</label>
        <input id="LastName" name="LastName" type="text" required="required" class="form-control">
    </div>
    <div class="form-group">
        <label for="MiddleName">Middle Name</label>
        <input id="MiddleName" name="MiddleName" type="text" class="form-control">
    </div>
    <div class="form-group">
        <label for="BirthDate">Birth Date</label>
        <div class="input-group">
            <div class="input-group-prepend">
                <div class="input-group-text">
                    <i class="fa fa-calendar"></i>
                </div>
            </div>
            <input id="BirthDate" name="BirthDate" type="text" required="required" class="form-control">
        </div>
    </div>
    <div class="form-group">
        <label for="CaseNumber">Case Number</label>
        <input id="CaseNumber" name="CaseNumber" placeholder="2024MM000123" type="text" required="required" class="form-control">
    </div>
    <div class="form-group">
        <label for="StartingBalance">Starting Balance</label>
        <div class="input-group">
            <div class="input-group-prepend">
                <div class="input-group-text">
                    <i class="fa fa-money"></i>
                </div>
            </div>
            <input id="StartingBalance" name="StartingBalance" type="text" required="required" class="form-control">
        </div>
    </div>
    <div class="form-group">
        <label for="Notes">Notes</label>
        <textarea id="Notes" name="Notes" cols="40" rows="4" class="form-control"></textarea>
    </div>
    <div class="form-group">
        <label for="AssignedUser">Assigned User</label>
        <div>
            <select id="AssignedUser" name="AssignedUser" class="custom-select">
                <option value="1">Rabbit</option>
                <option value="2">Duck</option>
                <option value="3">Fish</option>
            </select>
        </div>
    </div>
    <div class="form-group">
        <label for="DueDate">Due Date</label>
        <div class="input-group">
            <div class="input-group-prepend">
                <div class="input-group-text">
                    <i class="fa fa-calendar"></i>
                </div>
            </div>
            <input id="DueDate" name="DueDate" type="text" class="form-control">
        </div>
    </div>
    <div class="form-group">
        <div>
            <div class="custom-control custom-checkbox custom-control-inline">
                <input name="SpanishSpeaking" id="SpanishSpeaking_0" type="checkbox" class="custom-control-input" value="1">
                <label for="SpanishSpeaking_0" class="custom-control-label">Spanish Speaking</label>
            </div>
        </div>
    </div>
    <div class="form-group">
        <div>
            <div class="custom-control custom-checkbox custom-control-inline">
                <input name="IsActive" id="IsActive_0" type="checkbox" checked="checked" class="custom-control-input" value="0">
                <label for="IsActive_0" class="custom-control-label">Active</label>
            </div>
        </div>
    </div>
    <div class="form-group">
        <button name="submit" type="submit" class="btn btn-primary">Submit</button>
    </div>
</div>
