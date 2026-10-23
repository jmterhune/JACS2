<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TransactionList.ascx.cs" Inherits="tjc.Modules.JCID.TransactionList" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>

<div class="tabs">
    <ul class="nav nav-tabs">
        <li class="active">
            <a class="nav-link active" href="#Apps" data-bs-toggle="tab">Applications</a>
        </li>
        <li>
            <a class="nav-link" href="#Payments" data-bs-toggle="tab">Credit Card Payments</a>
        </li>
    </ul>
    <div class="tab-content">
        <div id="Apps" class="tab-pane active">
            <asp:UpdatePanel runat="server">
                <ContentTemplate>
                    <asp:Repeater runat="server" ID="rptApps" EnableViewState="false">
                        <HeaderTemplate>
                            <table id="apps" summary="List of Applications" class="table table-striped">
                                <thead>
                                    <tr>
                                        <th>ID</th>
                                        <th>Name</th>
                                        <th>Email</th>
                                        <th>Date Signed</th>
                                        <th>Paid?</th>
                                        <th>Criminal History</th>
                                        <th>Card Created</th>
                                        <th>Applicant Notified</th>
                                        <th>Picked Up</th>
                                        <th title="Payment Link">$</th>
                                    </tr>
                                </thead>
                                <tbody>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <tr>
                                <td><a href='<%#EditUrl("rid",Eval("RequestId").ToString(),"edit")%>'><%#Eval("RequestId")%></a></td>
                                <td><%#DataBinder.Eval(Container.DataItem, "FullName")%></td>
                                <td><a href='<%#"mailto:" + DataBinder.Eval(Container.DataItem, "Email") %>'><%#DataBinder.Eval(Container.DataItem, "Email")%></a></td>
                                <td><%#DataBinder.Eval(Container.DataItem, "DateSigned")%></td>

                                <td data-order="<%# Eval("PaymentDate") != null ? ((DateTime)Eval("PaymentDate")).ToString("yyyy-MM-dd HH:mm:ss") : "" %>">
                                    <%# Eval("PaymentDate") != null ? ((DateTime)Eval("PaymentDate")).ToString("MM/dd/yyyy") : "" %></td>
                                <td data-order="<%# Eval("CriminalHistoryCompleted") != null ? ((DateTime)Eval("CriminalHistoryCompleted")).ToString("yyyy-MM-dd HH:mm:ss") : "" %>">
                                    <%# Eval("CriminalHistoryCompleted") != null ? ((DateTime)Eval("CriminalHistoryCompleted")).ToString("MM/dd/yyyy") : "" %></td>
                                <td data-order="<%# Eval("CardCreated") != null ? ((DateTime)Eval("CardCreated")).ToString("yyyy-MM-dd HH:mm:ss") : "" %>">
                                    <%# Eval("CardCreated") != null ? ((DateTime)Eval("CardCreated")).ToString("MM/dd/yyyy") : "" %></td>
                                <td data-order="<%# Eval("ApplicantNotified") != null ? ((DateTime)Eval("ApplicantNotified")).ToString("yyyy-MM-dd HH:mm:ss") : "" %>">
                                    <%# Eval("ApplicantNotified") != null ? ((DateTime)Eval("ApplicantNotified")).ToString("MM/dd/yyyy") : "" %></td>
                                <td data-order="<%# Eval("CompletedDate") != null ? ((DateTime)Eval("CompletedDate")).ToString("yyyy-MM-dd HH:mm:ss") : "" %>">
                                    <%# Eval("CompletedDate") != null ? ((DateTime)Eval("CompletedDate")).ToString("MM/dd/yyyy") : "" %></td>
                                <td><asp:PlaceHolder runat="server" Visible='<%# PaymentLink(Container.DataItem) != null %>'><a title="Click to Copy Payment Link" href="#" onclick="return copyLink(this)" data-link='<%# PaymentLink(Container.DataItem) %>'><i class="fa fa-copy" aria-hidden="true"></i></a></asp:PlaceHolder></td>
                            </tr>
                        </ItemTemplate>
                        <FooterTemplate>
                            </tbody>
                            </table>
                        </FooterTemplate>
                    </asp:Repeater>

                </ContentTemplate>
                <Triggers>
                </Triggers>

            </asp:UpdatePanel>
        </div>
        <div id="Payments" class="tab-pane">
            <asp:UpdatePanel runat="server">
                <ContentTemplate>
                    <div class="row g-3">
                        <div class="col-sm-2">
                            <asp:Label ID="lblStartDate" runat="server" AssociatedControlID="txtStartDate" CssClass="visually-hidden" Text="Begin Date" />
                            <asp:TextBox ID="txtStartDate" runat="server" MaxLength="20" CssClass="form-control datepicker" />
                        </div>
                        <div class="col-sm-2">
                            <asp:Label ID="lblEndDate" runat="server" AssociatedControlID="txtEndDate" CssClass="visually-hidden" Text="End Date" />
                            <asp:TextBox ID="txtEndDate" runat="server" MaxLength="20" CssClass="form-control datepicker" />
                        </div>
                        <div class="col-sm-2">
                            <asp:Button ID="cmdSubmit" runat="server" OnClick="cmdSubmit_Click" CssClass="btn btn-primary mb-2" Text="Filter by Date Range" />
                        </div>
                    </div>
                    <asp:Repeater runat="server" ID="rptTransactions" EnableViewState="false">
                        <HeaderTemplate>
                            <table id="transactions" summary="List of Transactions" class="table table-striped">
                                <thead>
                                    <tr>
                                        <th>Request ID</th>
                                        <th>Name</th>
                                        <th>OrderId</th>
                                        <th>Email</th>
                                        <th>Amount</th>
                                        <th>Payment Type</th>
                                        <th>Date</th>
                                    </tr>

                                </thead>
                                <tbody>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <tr>
                                <td><%#DataBinder.Eval(Container.DataItem, "RequestID")%></td>
                                <td><%#DataBinder.Eval(Container.DataItem, "CustomerName")%></td>
                                <td><%#DataBinder.Eval(Container.DataItem, "OrderId")%></td>
                                <td><%#DataBinder.Eval(Container.DataItem, "EmailAddress")%></td>
                                <td><%#DataBinder.Eval(Container.DataItem, "PaymentAmount","{0:C}")%></td>
                                <td><%#DataBinder.Eval(Container.DataItem, "PaymentType")%></td>
                                <td data-order="<%# Eval("PaymentDate") != null ? ((DateTime)Eval("PaymentDate")).ToString("yyyy-MM-dd HH:mm:ss") : "" %>">
                                    <%# Eval("PaymentDate") != null ? ((DateTime)Eval("PaymentDate")).ToString("MM/dd/yyyy") : "" %></td>

                            </tr>
                        </ItemTemplate>
                        <FooterTemplate>
                            </tbody>
        </table>
                        </FooterTemplate>
                    </asp:Repeater>

                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="cmdSubmit" EventName="Click" />
                </Triggers>

            </asp:UpdatePanel>
        </div>

    </div>
</div>
<dnn:DnnCssInclude runat="server" FilePath="~/Resources/Shared/components/TimePicker/Themes/jquery-ui.min.css" />
<dnn:DnnCssInclude runat="server" FilePath="/Resources/Shared/components/TimePicker/jquery.timepicker.min.css" />
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Shared/components/TimePicker/jquery.timepicker.min.js" />
<dnn:dnncssInclude runat="server" FilePath="/Resources/Libraries/DataTables/dataTables.bootstrap5.min.css" />
<dnn:dnnjsInclude runat="server" FilePath="/Resources/Libraries/DataTables/dataTables.min.js" />
<dnn:dnnjsinclude runat="server" filepath="/Resources/Libraries/DataTables/dataTables.bootstrap5.min.js" />

<script type="text/javascript">

    (function ($, Sys) {
        $(document).ready(function () {
            Sys.Application.add_load(function (s, e) { SetupDnn(); });
            SetupDnn();
        });
    }(jQuery, window.Sys));

    function SetupDnn() {
        $(document).ready(function () {
            $(".datepicker").datepicker();
            var table = $('#transactions').DataTable({
                "order": [[0, "desc"]],
                "oLanguage": {

                    "sSearch": "Filter by Text"

                }
            });
            var table = $('#apps').DataTable({
                "order": [[0, "desc"]],
                "oLanguage": {

                    "sSearch": "Filter by Text"

                }
            });

        });
    }
    async function copyLink(item) {
        try {
            var text = item.dataset.link;
            await navigator.clipboard.writeText(text);
            alert('Link copied to clipboard');
        } catch (e) {
            alert('Unable to copy');
        }

        return false;
    }
</script>
