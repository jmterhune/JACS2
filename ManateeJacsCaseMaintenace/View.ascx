<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="View.ascx.cs" Inherits="tjc.Modules.JacsCaseMaint.View" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<%-- SweetAlert2 + Noty for confirms / toast notifications --%>
<dnn:DnnCssInclude runat="server" FilePath="/Resources/Libraries/sweetalert/sweetalert2.min.css" />
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/sweetalert/sweetalert2.all.min.js" />
<dnn:DnnCssInclude runat="server" FilePath="/Resources/Libraries/Noty/noty.min.css" />
<dnn:DnnCssInclude runat="server" FilePath="/Resources/Libraries/Noty/bootstrap-v4.min.css" />
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/Noty/noty.min.js" />
<div class="alert alert-info"><i class="fa fa-info-circle"></i>&nbsp;Search for case numbers in the Clericus - jacsManatee Interface by filling out the fields below.</div>
<div class="row g-3">
    <div class="col-auto">

        <asp:Label ID="lblYear" CssClass="visually-hidden" AssociatedControlID="txtYear" runat="server"></asp:Label>
        <asp:TextBox ID="txtYear" CssClass="form-control" runat="server" placeholder="Year"></asp:TextBox>
    </div>
    <div class="col-auto">
        <asp:Label ID="lblCaseType" CssClass="visually-hidden" AssociatedControlID="txtCaseType" runat="server"></asp:Label>
        <asp:TextBox ID="txtCaseType" CssClass="form-control" runat="server" placeholder="Case Type"></asp:TextBox>
    </div>
    <div class="col-auto">
        <asp:Label ID="lblSequence" CssClass="visually-hidden" AssociatedControlID="txtSequence" runat="server"></asp:Label>
        <asp:TextBox ID="txtSequence" CssClass="form-control" runat="server" placeholder="Sequence"></asp:TextBox>
    </div>

    <div class="col-auto">
        <asp:Button ID="cmdSubmit" OnClick="cmdSubmit_Click" runat="server" CssClass="btn btn-primary mt-3" Text="Search" />
    </div>
</div>
<hr />
<div class="accordion" id="dataLists">
    <div class="card card-default">
		<div class="card-header">
			<h4 class="card-title">
				<a class="accordion-toggle" data-toggle="collapse" data-parent="#dataLists" href="#collapseMessage"> Clericus Interface Messages </a>
			</h4>
		</div>
		<div id="collapseMessage" class="accordion-body collapse show">
			<div class="card-body">
                <asp:Repeater ID="rptInterfaceList" runat="server" OnItemCommand="rptInterfaceList_ItemCommand">
                    <HeaderTemplate>
                        <table class="table table-striped">
                            <thead>
                                <tr>
                                    <th>Message ID</th>
                                    <th>Case Number</th>
                                    <th>Return String</th>
                                    <th>Petitioner</th>
                                    <th>Respondent</th>
                                    <th>Case ID</th>
                                    <th>Modified</th>
                                    <th>&nbsp;</th>
                                </tr>
                            </thead>
                            <tbody>
                    </HeaderTemplate>

                    <ItemTemplate>
                        <tr>
                            <td class="command-icon-container"><%#Eval("message_id") %></td>
                            <td><%#Eval("CaseNumber") %></td>
                            <td><%#Eval("retstr") %></td>
                            <td><%#Eval("Petitioner") %> &ndash; <%#Eval("PetitionerAtty") %></td>
                            <td><%#Eval("Respondent") %> &ndash; <%#Eval("RespondentAtty") %></td>
                            <td><%#Eval("CASEID") %></td>
                            <td><%#Eval("lupddate") %></td>
                            <td class="command-icon-container">
                                <asp:LinkButton runat="server" ID="cmdSearchCycle" CommandArgument='<%#Eval("CASEID") %>' CommandName="scc" CssClass="command-icon"><i class="fa fa-search"></i>&nbsp;Search Case Cyle</asp:LinkButton>
                            </td>
                        </tr>
                    </ItemTemplate>
                    <FooterTemplate>
                        </tbody></table><hr />
                    </FooterTemplate>
                </asp:Repeater>
            </div>
        </div>
    </div>
    <div class="card card-default">
		<div class="card-header">
			<h4 class="card-title">
				<a class="accordion-toggle" data-toggle="collapse" data-parent="#dataLists" href="#collapseCases"> Matching JACS Case Table Records </a>
			</h4>
		</div>
		<div id="collapseCases" class="accordion-body collapse show">
			<div class="card-body">
                <asp:Repeater ID="rptCaseList" runat="server" OnItemCommand="rptCaseList_ItemCommand">
                    <HeaderTemplate>

                        <table class="table table-striped">
                            <thead>
                                <tr>
                                    <th>Case Number</th>
                                    <th>Motion Code</th>
                                    <th>Plaintiff</th>
                                    <th>Defendant</th>
                                    <th>Case ID</th>
                                    <th>Modified</th>
                                    <th>&nbsp;</th>
                                </tr>
                            </thead>
                            <tbody>
                    </HeaderTemplate>

                    <ItemTemplate>
                        <tr>
                            <td><%#Eval("CASENUM") %></td>
                            <td><%#Eval("MOTIONCODE") %></td>
                            <td><%#Eval("PLAINTIFF") %> &ndash; <%#Eval("BARNUM") %></td>
                            <td><%#Eval("DEFENDANT") %> &ndash; <%#Eval("OPPOSINGBARNUM") %></td>
                            <td><%#Eval("CASEID") %></td>
                            <td><%#Eval("LUPDDATE") %></td>
                            <td>
                                <asp:LinkButton runat="server" CssClass="text-danger" OnClientClick="return Jud12ConfirmPostback(this, 'Delete this Case?', 'Delete?');" CommandArgument='<%#Eval("CASENUM") %>' CommandName="Delete"><i class="fas fa-trash"></i></asp:LinkButton></td>

                        </tr>
                    </ItemTemplate>
                    <FooterTemplate>
                        </tbody></table><hr />
                    </FooterTemplate>
                </asp:Repeater>
            </div>
        </div>
    </div>
    <div class="card card-default">
		<div class="card-header">
			<h4 class="card-title">
				<a class="accordion-toggle" data-toggle="collapse" data-parent="#dataLists" href="#collapseCycle"> Matching Case Cycle Records</a>
			</h4>
		</div>
		<div id="collapseCycle" class="accordion-body collapse show">
			<div class="card-body">
                <asp:Repeater ID="rptCaseCycle" runat="server" OnItemCommand="rptCaseCycle_ItemCommand">
                    <HeaderTemplate>
                        <table class="table table-striped">
                            <thead>
                                <tr>
                                    <th>Case Number</th>
                                    <th>Case ID</th>
                                    <th>Modified</th>
                                    <th>&nbsp;</th>
                                </tr>
                            </thead>
                            <tbody>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <tr>
                            <td><%#Eval("CASENUM") %></td>
                            <td><%#Eval("FLRC_Id") %></td>
                            <td><%#Eval("LUPDDATE") %></td>
                            <td>
                                <asp:LinkButton runat="server" CssClass="text-danger" OnClientClick="return Jud12ConfirmPostback(this, 'Delete this Case Cyle Record?', 'Delete?');" CommandArgument='<%#Eval("CaseCycle_Id") %>' CommandName="Delete"><i class="fas fa-trash"></i></asp:LinkButton></td>

                        </tr>
                    </ItemTemplate>
                    <FooterTemplate>
                        </tbody></table>
                    </FooterTemplate>
                </asp:Repeater>
            </div>
        </div>
    </div>
</div>
<script type="text/javascript">
    function Jud12ConfirmPostback(btn, msg, title) {
        if (!window.Swal) { return window.confirm(msg); }
        if (btn && btn.dataset && btn.dataset.jud12Confirmed === '1') {
            btn.dataset.jud12Confirmed = '';
            return true;
        }
        Swal.fire({
            title: title || 'Confirm', text: msg, icon: 'warning',
            showCancelButton: true, confirmButtonText: 'Yes', cancelButtonText: 'No',
            confirmButtonColor: '#d33'
        }).then(function (r) {
            if (!r.isConfirmed) return;
            var href = btn.href || '';
            var m = href.match(/__doPostBack\(['"]([^'"]+)['"],\s*['"]([^'"]*)['"]\)/);
            if (m && typeof __doPostBack === 'function') {
                __doPostBack(m[1], m[2]);
            } else if (btn && btn.tagName === 'INPUT' && (btn.type === 'submit' || btn.type === 'button')) {
                btn.dataset.jud12Confirmed = '1';
                btn.click();
            } else if (btn && typeof btn.click === 'function') {
                btn.dataset.jud12Confirmed = '1';
                btn.click();
            }
        });
        return false;
    }
</script>
