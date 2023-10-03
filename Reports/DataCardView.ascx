<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DataCardView.ascx.cs" Inherits="tjc.Modules.Reports.DataCardView" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<asp:Literal Visible="false" ID="ltMessage" runat="server"><div class="alert alert-{0}"><i class="fa fa-{1}"></i>&nbsp; {2}</div></asp:Literal>

<h2>
    <asp:Literal ID="ltReportTitle" runat="server" Text="DataCard View" />
</h2>
<asp:HiddenField ID="hdTitle" runat="server" Value="DataCard DB View" ClientIDMode="Static" />
<asp:Repeater ID="rptDataCard" runat="server">
    <HeaderTemplate><ul class="row image-gallery sort-destination lightbox" data-sort-id="portfolio" data-plugin-options="{'delegate': 'a', 'type': 'image', 'gallery': {'enabled': true}}"></HeaderTemplate>
    <ItemTemplate>
        
	<li class="col-md-3 col-sm-6 isotope-item websites">
		<div class="image-gallery-item">
			<a href="/portals/22/img/projects/project.jpg">
				<div class="thumb-info">
					<span class="thumb-info-wrapper">
						<img alt="Person Photo" src='data:image/png;base64,<%# (Eval("Photo")!=null) ? Convert.ToBase64String((byte[])Eval("Photo")) : string.Empty %>'
                    class="img-fluid"  />
						<span class="thumb-info-title">
							<span class="thumb-info-inner"><%#Eval("FirstName") %> <%#Eval("LastName") %></span>
							<span class="thumb-info-type"><%#Eval("Title")!=null ? Eval("Title").ToString():"No Title" %></span>
						</span>
						<span class="thumb-info-action-icon"> <em class="fas fa-link"></em> </span>
					</span>
				</div>
				<span class="btn-text-indent"></span>
			</a>
		</div>
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
