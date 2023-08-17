<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="GroupEdit.ascx.cs" Inherits="tjc.Modules.MediationStatistics.GroupEdit" %>
<div class="in-popup">
    <asp:Literal ID="ltInfo" runat="server">
        <div class="alert alert-info"><i class="fas fa-info-circle"></i>Use the list boxes below to assign Case Type Group associations for {0}. </div>
    </asp:Literal>

    <div id="group-relationships" class="groups">
        <fieldset class="outline-fieldset">
            <legend>Case Types By Case Group</legend>
            <div class="row">
                <div class="col-auto">
                    <asp:Label ID="lblSelectedCaseType" runat="server" AssociatedControlID="lsSelectedCaseType">Selected Case Types</asp:Label>
                    <asp:ListBox ID="lsSelectedCaseType" ClientIDMode="Static" SelectionMode="Single" CssClass="group-list" DataTextField="Description" DataValueField="CaseTypeId" runat="server" Rows="12" />
                </div>
                <div class="col-1 list-commands">
                    <div class="text-center mt-5 mb-3">
                        <asp:LinkButton CausesValidation="false" CssClass="text-primary" ToolTip="Add Selected items to the Case Type Group" ID="cmdAddCaseType" runat="server" OnClick="cmdAddCaseType_Click"><i class="fas fa-arrow-alt-circle-left"></i></asp:LinkButton>
                    </div>
                    <div class="text-center">
                        <asp:LinkButton CausesValidation="false" CssClass="text-primary" ToolTip="Remove Selected items from the Case Type Group" ID="cmdRemoveCaseType" runat="server" OnClick="cmdRemoveCaseType_Click"><i class="fas fa-arrow-alt-circle-right"></i></asp:LinkButton>
                    </div>
                    <div class="text-center mt-4 mb-3">
                        <asp:LinkButton CausesValidation="false" CssClass="text-primary" ToolTip="Move Selected Options Up" ID="cmdMoveUpCaseType" runat="server" OnClick="cmdMoveUpCaseType_Click"><i class="fas fa-arrow-alt-circle-up"></i></asp:LinkButton>
                    </div>
                    <div class="text-center">
                        <asp:LinkButton CausesValidation="false" CssClass="text-primary" ToolTip="Move Selected Options Down" ID="cmdMoveDownCaseType" runat="server" OnClick="cmdMoveDownCaseType_Click"><i class="fas fa-arrow-alt-circle-down"></i></asp:LinkButton>
                    </div>
                </div>
                <div class="col-auto">
                    <asp:Label ID="lblAvailableCaseType" runat="server" AssociatedControlID="lsAvailableCaseType">Available Case Types</asp:Label>
                    <asp:ListBox ID="lsAvailableCaseType" SelectionMode="Multiple" CssClass="group-list" DataTextField="Description" DataValueField="CaseTypeId" runat="server" Rows="12" />
                </div>
            </div>

        </fieldset>
        <fieldset class="outline-fieldset">
            <legend>Appearance Items By Case Group</legend>
            <div class="row">
                <div class="col-auto">
                    <asp:Label ID="lblSelectedAppearance" runat="server" AssociatedControlID="lsSelectedAppearance">Selected Items</asp:Label>
                    <asp:ListBox ID="lsSelectedAppearance" ClientIDMode="Static" SelectionMode="Single" CssClass="group-list" DataTextField="Description" DataValueField="AppearanceId" runat="server" Rows="12" />
                </div>
                <div class="col-1 list-commands">
                    <div class="text-center mt-5 mb-3">
                        <asp:LinkButton CausesValidation="false" CssClass="text-primary" ToolTip="Add Selected items to the Case Type Group" ID="cmdAddAppearance" runat="server" OnClick="cmdAddAppearance_Click"><i class="fas fa-arrow-alt-circle-left"></i></asp:LinkButton>
                    </div>
                    <div class="text-center">
                        <asp:LinkButton CausesValidation="false" CssClass="text-primary" ToolTip="Remove Selected items from the Case Type Group" ID="cmdRemoveAppearance" runat="server" OnClick="cmdRemoveAppearance_Click"><i class="fas fa-arrow-alt-circle-right"></i></asp:LinkButton>
                    </div>
                    <div class="text-center mt-4 mb-3">
                        <asp:LinkButton CausesValidation="false" CssClass="text-primary" ToolTip="Move Selected Options Up" ID="cmdMoveUpAppearance" runat="server" OnClick="cmdMoveUpAppearance_Click"><i class="fas fa-arrow-alt-circle-up"></i></asp:LinkButton>
                    </div>
                    <div class="text-center">
                        <asp:LinkButton CausesValidation="false" CssClass="text-primary" ToolTip="Move Selected Options Down" ID="cmdMoveDownAppearance" runat="server" OnClick="cmdMoveDownAppearance_Click"><i class="fas fa-arrow-alt-circle-down"></i></asp:LinkButton>
                    </div>
                </div>
                <div class="col-auto">
                    <asp:Label ID="lblAvailableAppearance" runat="server" AssociatedControlID="lsAvailableAppearance">Available Items</asp:Label>
                    <asp:ListBox ID="lsAvailableAppearance" SelectionMode="Multiple" CssClass="group-list" DataTextField="Description" DataValueField="AppearanceId" runat="server" Rows="12" />
                </div>
            </div>
        </fieldset>
        <fieldset class="outline-fieldset">
            <legend>Issues By Case Group</legend>
            <div class="row">
                <div class="col-auto">
                    <asp:Label ID="lblSelectedIssues" runat="server" AssociatedControlID="lsSelectedIssues">Selected Items</asp:Label>
                    <asp:ListBox ID="lsSelectedIssues" ClientIDMode="Static" SelectionMode="Single" CssClass="group-list" DataTextField="Description" DataValueField="IssueId" runat="server" Rows="12" />
                </div>
                <div class="col-1 list-commands">
                    <div class="text-center mt-5 mb-3">
                        <asp:LinkButton CausesValidation="false" CssClass="text-primary" ToolTip="Add Selected items to the Case Type Group" ID="cmdAddIssue" runat="server" OnClick="cmdAddIssue_Click"><i class="fas fa-arrow-alt-circle-left"></i></asp:LinkButton>
                    </div>
                    <div class="text-center">
                        <asp:LinkButton CausesValidation="false" CssClass="text-primary" ToolTip="Remove Selected items from the Case Type Group" ID="cmdRemoveIssue" runat="server" OnClick="cmdRemoveIssue_Click"><i class="fas fa-arrow-alt-circle-right"></i></asp:LinkButton>
                    </div>
                    <div class="text-center mt-4 mb-3">
                        <asp:LinkButton CausesValidation="false" CssClass="text-primary" ToolTip="Move Selected Options Up" ID="cmdMoveUpIssue" runat="server" OnClick="cmdMoveUpIssue_Click"><i class="fas fa-arrow-alt-circle-up"></i></asp:LinkButton>
                    </div>
                    <div class="text-center">
                        <asp:LinkButton CausesValidation="false" CssClass="text-primary" ToolTip="Move Selected Options Down" ID="cmdMoveDownIssue" runat="server" OnClick="cmdMoveDownIssue_Click"><i class="fas fa-arrow-alt-circle-down"></i></asp:LinkButton>
                    </div>
                </div>
                <div class="col-auto">
                    <asp:Label ID="lblAvailableIssues" runat="server" AssociatedControlID="lsAvailableIssues">Available Items</asp:Label>
                    <asp:ListBox ID="lsAvailableIssues" SelectionMode="Multiple" CssClass="group-list" DataTextField="Description" DataValueField="IssueId" runat="server" Rows="12" />
                </div>
            </div>
        </fieldset>

    </div>
    <hr />
    <p>
        <asp:Button CssClass="btn btn-primary" ID="cmdSave" runat="server" Text="Save" OnClick="cmdSave_Click" />
    </p>
</div>
<script type="text/javascript">
    (function ($, Sys) {
        $(document).ready(function () {
            Sys.Application.add_load(function (s, e) { PageInit(); });
            PageInit();
        });
    }(jQuery, window.Sys));

    function PageInit() {
        $('#caseTypeUp').click(CaseTypeMoveUp);
        $('#caseTypeDown').click(CaseTypeMoveDown);
        $('#appearanceUp').click(AppearanceMoveUp);
        $('#appearanceDown').click(AppearanceMoveDown);
        $('#issueUp').click(IssueMoveUp);
        $('#issueDown').click(IssueMoveDown);
    }
    function CaseTypeMoveUp() {
        $('#lsSelectedCaseType :selected').each(function (i, selected) {
            if (!$(this).prev().length) return false;
            $(this).insertBefore($(this).prev());
        });
        $('#lsSelectedCaseType').focus().blur();
    }
    function CaseTypeMoveDown() {
        $($('#lsSelectedCaseType :selected').get().reverse()).each(function (i, selected) {
            if (!$(this).next().length) return false;
            $(this).insertAfter($(this).next());
        });
        $('#lsSelectedCaseType').focus().blur();
    }
    function AppearanceMoveUp() {
        $('#lsSelectedAppearance :selected').each(function (i, selected) {
            if (!$(this).prev().length) return false;
            $(this).insertBefore($(this).prev());
        });
        $('#lsSelectedAppearance').focus().blur();
    }
    function AppearanceMoveDown() {
        $($('#lsSelectedAppearance :selected').get().reverse()).each(function (i, selected) {
            if (!$(this).next().length) return false;
            $(this).insertAfter($(this).next());
        });
        $('#lsSelectedAppearance').focus().blur();
    }
    function IssueMoveUp() {
        $('#lsSelectedIssues :selected').each(function (i, selected) {
            if (!$(this).prev().length) return false;
            $(this).insertBefore($(this).prev());
        });
        $('#lsSelectedIssues').focus().blur();
    }
    function IssueMoveDown() {
        $($('#lsSelectedIssues :selected').get().reverse()).each(function (i, selected) {
            if (!$(this).next().length) return false;
            $(this).insertAfter($(this).next());
        });
        $('#lsSelectedIssues').focus().blur();
    }
</script>

