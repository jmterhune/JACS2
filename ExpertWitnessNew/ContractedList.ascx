<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ContractedList.ascx.cs" Inherits="tjc.Modules.ExpertWitness.ContractedList" %>
<div class="contracted-experts">
    <p class="contracted-intro">
        Attention Attorneys: Expert selection is limited to those under contract with the Twelfth Circuit, as only these individuals can be paid.
        <span class="contracted-updated"><asp:Literal ID="ltUpdated" runat="server" /></span>
    </p>
    <asp:Literal ID="ltList" runat="server" />
</div>
