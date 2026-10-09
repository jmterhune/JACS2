<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="View.ascx.cs" Inherits="tjc.Modules.FileManager.Views.View" %>
<asp:Label ID="lblMessage" runat="server" CssClass="dnnFormMessage dnnFormWarning" Visible="false" />
<asp:TreeView ID="tvFiles" runat="server" CssClass="fileList" ShowLines="true" ViewStateMode="Disabled"></asp:TreeView>
