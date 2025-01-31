<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="View.ascx.cs" Inherits="tjc.Modules.ExpertWitness.View" %>
<asp:HyperLink ID="lnkAdmin" Visible="false" runat="server" Text="Manage Lists" CssClass="btn btn-secondary mb-3" />

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
        <asp:PlaceHolder ID="plhMessage" runat="server" EnableViewState="false"></asp:PlaceHolder>
        <div class="row form-group">
            <div class="col-auto">
                <asp:Label runat="server" AssociatedControlID="txtCaseNumber" Text="Case Number<em>*</em>" ToolTip="required" />
                <asp:TextBox runat="server" ClientIDMode="Static" CssClass="form-control" MaxLength="50" ID="txtCaseNumber" />
                <asp:RequiredFieldValidator ID="valCaseNumber" runat="server" CssClass="label label-danger" Display="Dynamic" ControlToValidate="txtCaseNumber" ErrorMessage="Casenumber is Required"></asp:RequiredFieldValidator>
            </div>
            <div class="col-md-3">
                <asp:Label runat="server" AssociatedControlID="drpEvaluation" Text="Evaluation Type<em>*</em>" ToolTip="required" />
                <asp:DropDownList ID="drpEvaluation" runat="server" CssClass="form-control" ClientIDMode="Static" AutoPostBack="true" OnSelectedIndexChanged="drpEvaluation_SelectedIndexChanged"></asp:DropDownList>
                <asp:RequiredFieldValidator ID="valEvaluation" runat="server" CssClass="label label-danger" Display="Dynamic" ControlToValidate="drpEvaluation" ErrorMessage="Evaluation Type is Required"></asp:RequiredFieldValidator>
            </div>
            <div class="col-auto">
                <asp:Label runat="server" AssociatedControlID="drpLocation" Text="Location<em>*</em>" ToolTip="required" />
                <asp:DropDownList ID="drpLocation" runat="server" CssClass="form-control" ClientIDMode="Static" AutoPostBack="true" OnSelectedIndexChanged="drpLocation_SelectedIndexChanged"></asp:DropDownList>
                <asp:RequiredFieldValidator ID="valLocation" runat="server" CssClass="label label-danger" Display="Dynamic" ControlToValidate="drpLocation" ErrorMessage="Location is Required"></asp:RequiredFieldValidator>
            </div>
        </div>
        <asp:Repeater ID="rptExpertSelection" runat="server" OnItemDataBound="rptExpertSelection_ItemDataBound" OnItemCreated="rptExpertSelection_ItemCreated" OnItemCommand="rptExpertSelection_ItemCommand">
            <HeaderTemplate>
                <div>
            </HeaderTemplate>
            <ItemTemplate>
                <div class="bg-dark rounded p-2 text-white" id="divHeader" runat="server">
                    <div class="row">
                        <div class="col-8 pt-2">
                            <asp:Literal ID="ltTypeHeader" runat="server"></asp:Literal></div>
                        <div class="col-4  d-flex justify-content-end">
                            <asp:LinkButton ID="cmdAddExpert" runat="server" CommandName="add" CssClass="btn btn-default" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "sequence")%>'><i class="fas fa-plus-circle text-primary"></i> Add Additional Expert</asp:LinkButton>
                        </div>
                    </div>

                </div>
                <div data-comment="<%#DataBinder.Eval(Container.DataItem, "Comments") %>">
                    <div class="bg-warning rounded m-3 p-1 ps-3 text-dark" id="divContainer" runat="server">
                        <div class="row">
                            <div class="col-6 pt-2"><%# DataBinder.Eval(Container.DataItem, "ExpertName") %></div>
                            <div class="col-6 d-flex justify-content-end">
                                <asp:Literal ID="ltViewComment" runat="server" />
                                <asp:HiddenField  id="hdSequence" runat="server" Value='<%# DataBinder.Eval(Container.DataItem, "sequence")%>' />
                                <asp:LinkButton ID="cmdSelect" CausesValidation="false" runat="server" CssClass="btn btn-default me-2" CommandName="select" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ExpertiD")%>'><i class="fas fa-check-circle text-success"></i> Select</asp:LinkButton>
                                <asp:LinkButton ID="cmdPass" CausesValidation="false" runat="server" CssClass="btn btn-default" CommandName="pass" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ExpertiD")%>'><i class="fas fa-minus-circle text-danger"></i> Pass</asp:LinkButton>
                            </div>
                        </div>
                    </div>
                </div>
            </ItemTemplate>
            <FooterTemplate></div></FooterTemplate>
        </asp:Repeater>
        <hr />
        <p>
            <asp:Button ID="cmdUpdate" runat="server" CssClass="btn btn-primary" OnClick="cmdUpdate_Click" Text="Save" />
            <asp:Button ID="cmdReset" runat="server" CssClass="btn btn-secondary" OnClick="cmdReset_Click" Text="Reset" />
        </p>
    </ContentTemplate>
    <Triggers>
        <asp:AsyncPostBackTrigger ControlID="cmdUpdate" EventName="Click" />
        <asp:AsyncPostBackTrigger ControlID="cmdUpdate" EventName="Click" />
        <asp:AsyncPostBackTrigger ControlID="drpEvaluation" EventName="SelectedIndexChanged" />
        <asp:AsyncPostBackTrigger ControlID="drpLocation" EventName="SelectedIndexChanged" />
    </Triggers>
</asp:UpdatePanel>
<script type="text/javascript">
    (function ($, Sys) {
        $(document).ready(function () {
            Sys.Application.add_load(function (s, e) { PageInit(); });
            PageInit();
        });
    }(jQuery, window.Sys));
    function PageInit() {
        $(".view-comment").on("click", function (e) {
            e.preventDefault();
            const comment = $(this).data("comment");
            $.dnnAlert({
                text: comment,
                title:"View Comments",
                width: 500
            });
        });
        $('.comment').on('click', function (e) {
            var title = $(this).attr("title");
            e.preventDefault();
            $.dnnAlert({
                text: title,
                width: 700
            });
        });
    }
    function ShowComment(comment) {

    }
</script>
