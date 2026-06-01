<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DataCardView.ascx.cs" Inherits="tjc.Modules.Reports.DataCardView" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<asp:Literal Visible="false" ID="ltMessage" runat="server"><div class="alert alert-{0}"><i class="fa fa-{1}"></i>&nbsp; {2}</div></asp:Literal>
<h2>
    <asp:Literal ID="ltReportTitle" runat="server" Text="DataCard View" />
</h2>
<div class="col-md-3">
    <div class="input-group mb-3">
        <asp:TextBox runat="server" ID="txtLastName" CssClass="form-control" placeholder="Last Name" />
        <asp:Button Text="Search" CssClass="btn btn-primary" ID="cmdSearch" runat="server" OnClick="cmdSearch_Click" />
    </div>
</div>
<asp:HiddenField ID="hdTitle" runat="server" Value="DataCard DB View" ClientIDMode="Static" />
<asp:Repeater ID="rptDataCard" runat="server">
    <HeaderTemplate>
        <ul class="row image-gallery sort-destination lightbox" data-sort-id="portfolio" data-plugin-options="{'delegate': 'a', 'type': 'image', 'gallery': {'enabled': true}}">
    </HeaderTemplate>
    <ItemTemplate>
        <li class="col-md-3 col-sm-6">
                <img alt="Person Photo" src='data:image/png;base64,<%# (Eval("Photo")!=null) ? Convert.ToBase64String((byte[])Eval("Photo")) : string.Empty %>' class="img-fluid" />
            <%#Eval("FirstName") %> <%#Eval("LastName") %>
        </li>
    </ItemTemplate>
    <FooterTemplate>
        </ul>
    </FooterTemplate>
</asp:Repeater>
<script>
    (function ($, Sys) {
        $(document).ready(function () {
            var title = $("#hdTitle").val();
            $(".page-top-info h1").html(title);
            Sys.Application.add_load(function (s, e) { PageInit(); });
            PageInit();
        });
    }(jQuery, window.Sys));

    function PageInit() {
        var title = $("#hdTitle").val();
        $(".page-top-info h1").html(title);
        $(".datepicker").datepicker();
    }
</script>
