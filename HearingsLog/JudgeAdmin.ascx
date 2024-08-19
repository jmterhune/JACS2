<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="JudgeAdmin.ascx.cs" Inherits="tjc.Modules.HearingLog.JudgeAdmin" %>
<%@ Register TagPrefix="dnn" TagName="label" Src="~/controls/LabelControl.ascx" %>

<asp:UpdatePanel ID="pnlJacsJudges" runat="server" RenderMode="Block" OnUnload="pnlJacsJudges_Unload">
    <ContentTemplate>
        <asp:UpdateProgress ID="upProgressEvent" runat="server">
            <ProgressTemplate>
                <div class="modal-progress">
                    <div class="center-progress">
                        <img alt="" src="/images/loading.gif" />
                    </div>
                </div>
            </ProgressTemplate>
        </asp:UpdateProgress>
        <div>
            <div class="row">
                <div class="col-auto">
                    <div class="mb-3">
                        <asp:Label Text="Select Judge" AssociatedControlID="drpJudge" runat="server" class="form-label" />
                        <asp:DropDownList ID="drpJudge" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="drpJudge_SelectedIndexChanged">
                        </asp:DropDownList>
                        <div id="judgeHelp" class="form-text">Select the Judge or Clerk App to configure from the list</div>
                    </div>
                    <div>
                        <asp:Label Text="Select County" AssociatedControlID="drpCounty" runat="server" class="form-label" />
                        <asp:DropDownList ID="drpCounty" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="drpCounty_SelectedIndexChanged">
                            <asp:ListItem Text="< Select County >" Value="" />
                            <asp:ListItem Text="DeSoto" />
                            <asp:ListItem Text="Manatee" />
                            <asp:ListItem Text="Sarasota" />
                            <asp:ListItem Text="Benchmark" />
                            <asp:ListItem Text="Clericus" />
                        </asp:DropDownList>
                        <div id="countyHelp" class="form-text">Select the JACS County or Clerk App to get judges from</div>
                    </div>

                </div>
                <div class="col-auto">
                    <div>
                        <asp:Label Text="Assign JA" AssociatedControlID="drpJA" runat="server" class="form-label" />
                        <asp:DropDownList ID="drpJA" runat="server" CssClass="form-control">
                        </asp:DropDownList>
                        <div id="jaHelp" class="form-text">Select the JA to assign from the list</div>
                    </div>
                </div>
                <div class="col-12 mt-3">
                    <asp:Label Text="Check all Judge Names associated with this user" AssociatedControlID="chlJacsJudges" runat="server" class="form-label" />
                    <asp:CheckBoxList ID="chlJacsJudges" runat="server" CssClass="check-list" RepeatLayout="UnorderedList">
                    </asp:CheckBoxList>
                </div>
            </div>
            <hr />
            <p>
                <asp:LinkButton ID="cmdSaveJudge" runat="server" OnClick="cmdSaveJudge_Click" CssClass="btn btn-primary"><i class="fas fa-save"></i> Save</asp:LinkButton>
                <asp:HyperLink ID="lnkCancel" Visible="false" runat="server" CssClass="btn btn-danger"><i class="fas fa-undo"></i> Cancel</asp:HyperLink>
            </p>
        </div>
    </ContentTemplate>
    <Triggers>
        <asp:AsyncPostBackTrigger ControlID="cmdSaveJudge" EventName="Click" />
        <asp:AsyncPostBackTrigger ControlID="drpJudge" EventName="SelectedIndexChanged" />
        <asp:AsyncPostBackTrigger ControlID="drpCounty" EventName="SelectedIndexChanged" />
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
    }
</script>

