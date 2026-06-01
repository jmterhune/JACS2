<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ViewThreat.ascx.cs" Inherits="tjc.Modules.ThreatReport.ViewThreat" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>

<div id="ThreatReport">
    <fieldset>
        <legend>Person Making This Report</legend>
        <div class="row">
            <div class="form-group">
                <div class="col-md-6">
                    <asp:Label Text="Name" runat="server" AssociatedControlID="txtPersonReporting" />
                    <asp:TextBox ReadOnly="true" ID="txtPersonReporting" runat="server" MaxLength="50" CssClass="form-control" />
                </div>
                <div class="col-md-6">
                    <asp:Label Text="Date of Report" runat="server" AssociatedControlID="txtDateReported" />
                    <asp:TextBox ID="txtDateReported" runat="server" ReadOnly="true" CssClass="form-control" />

                </div>

            </div>
        </div>
        <div class="row">
            <div class="form-group">
                <div class="col-md-4">
                    <asp:Label Text="Phone" runat="server" AssociatedControlID="txtPersonReportingPhone" />
                    <asp:TextBox ID="txtPersonReportingPhone" ReadOnly="true" runat="server" CssClass="form-control" />
                </div>
                <div class="col-md-2">
                    <asp:Label Text="Extension" runat="server" AssociatedControlID="txtPersonReportingExtension" />
                    <asp:TextBox ID="txtPersonReportingExtension" runat="server" ReadOnly="true" CssClass="form-control" />

                </div>
                <div class="col-md-6">
                    <asp:Label Text="Email" runat="server" AssociatedControlID="txtPersonReportingEmail" />
                    <asp:TextBox ID="txtPersonReportingEmail" runat="server" ReadOnly="true" CssClass="form-control" />
                </div>

            </div>
        </div>
    </fieldset>
    <fieldset>
        <legend>Location of Incident</legend>
        <div class="row">
            <div class="form-group">
                <div class="col-md-6">
                    <asp:TextBox ID="txtLocation" ReadOnly="true" runat="server" CssClass="form-control" />
                </div>
            </div>
        </div>
    </fieldset>
    <fieldset>
        <legend>Nature of Incident</legend>
        <div class="row">
            <div class="form-group">
                <div class="col-md-6">
                    <asp:Label Text="Date of Incident" runat="server" AssociatedControlID="txtDate" />
                    <asp:TextBox ID="txtDate" ReadOnly="true" runat="server" CssClass="form-control" />

                </div>
                <div class="col-md-6">
                    <asp:Label Text="Nature of Incident" runat="server" AssociatedControlID="txtIncidentNature" />
                    <asp:TextBox ReadOnly="true" runat="server" ID="txtIncidentNature" CssClass="form-control" />

                </div>

            </div>
        </div>
        <div class="row">
            <div class="form-group">
                <div class="col-md-12">
                    <asp:Label Text="Briefly describe the incident" runat="server" AssociatedControlID="txtIncidentDescription" />
                    <asp:TextBox ID="txtIncidentDescription" ReadOnly="true" runat="server" TextMode="MultiLine" Rows="5" CssClass="form-control" />

                </div>
            </div>
        </div>
        <div class="row">
            <div class="form-group">
                <div class="col-md-6">
                    <asp:Label Text="If a specific person was targeted, indicate their name" runat="server" AssociatedControlID="txtPersonTargeted" />
                    <asp:TextBox ID="txtPersonTargeted" ReadOnly="true" CssClass="form-control" runat="server" MaxLength="50" />

                </div>
                <div class="col-md-6">
                    <div class="form-control mt-5g" aria-readonly="true" readonly>
                        <div class="form-check form-check-inline">
                            <input disabled type="checkbox" aria-readonly="true" class="form-check-input" id="chkCourtEmployee" name="chkCourtEmployee" runat="server" />
                            <asp:Label Text="Court Employee?" CssClass="form-check-label" runat="server" AssociatedControlID="chkCourtEmployee" />
                        </div>
                        <div class="form-check form-check-inline">
                            <input disabled type="checkbox" class="form-check-input" id="chkTargetNotified" name="chkCourtEmployee" runat="server" />
                            <asp:Label Text="Target Notified?" CssClass="form-check-label" runat="server" AssociatedControlID="chkTargetNotified" />
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </fieldset>
    <fieldset class="mb-4">
        <legend>Suspect's Information</legend>
        <asp:Repeater ID="rptPersonsInvolved" runat="server">
            <HeaderTemplate>
                <p class="alert alert-info"><em class="fa fa-info-circle"></em>&nbsp; Click the Names to Expand Details</p>
                <div class="accordion" id="accordion">
            </HeaderTemplate>

            <ItemTemplate>
                <div class="card card-default">
                    <div class="card-header">
                        <h4 class="card-title">
                            <a class="accordion-toggle collapsed" data-toggle="collapse" data-parent="#accordion" aria-expanded="false" href="<%#"#Suspect-" + Container.ItemIndex + 1 %>"><%#DataBinder.Eval(Container.DataItem, "FirstName")%> <%#DataBinder.Eval(Container.DataItem, "LastName")%> </a>
                        </h4>
                    </div>
                    <div id="<%#"Suspect-" + Container.ItemIndex + 1 %>" class="accordion-body collapse">
                        <div class="card-body container">
                            <div class="row">
                                <div class="form-group">

                                    <div class="col-md-3">
                                        <label class="d-block">
                                            Date of Birth
                                        <input type="text" readonly class="form-control" value="<%#DataBinder.Eval(Container.DataItem, "DateOfBirth", "{0:MM/dd/yyyy}")%>"></label>
                                    </div>
                                    <div class="col-md-3">
                                        <label class="d-block">
                                            Phone
                                        <input type="text" readonly class="form-control phone_us" value="<%#DataBinder.Eval(Container.DataItem, "Phone")%>"></label>
                                    </div>

                                    <div class="col-md-3">
                                        <label class="d-block">
                                            Gender
                                        <input type="text" readonly class="form-control" value="<%#DataBinder.Eval(Container.DataItem, "Gender")%>"></label>
                                    </div>
                                    <div class="col-md-3">
                                        <label class="d-block">
                                            Race
                                        <input type="text" readonly class="form-control" value="<%#DataBinder.Eval(Container.DataItem, "Race")%>"></label>
                                    </div>

                                </div>
                            </div>
                            <div class="row">
                                <div class="form-group">
                                    <div class="col-md-2">
                                        <label class="d-block">
                                            Height
                                        <input type="text" readonly class="form-control" value="<%#DataBinder.Eval(Container.DataItem, "Height")%>"></label>
                                    </div>
                                    <div class="col-md-2">
                                        <label class="d-block">
                                            Weight
                                        <input type="text" readonly class="form-control" value="<%#DataBinder.Eval(Container.DataItem, "Weight")%>"></label>
                                    </div>
                                    <div class="col-md-3">
                                        <label class="d-block">
                                            Hair Color
                                        <input type="text" readonly class="form-control" value="<%#DataBinder.Eval(Container.DataItem, "HairColor")%>"></label>
                                    </div>

                                    <div class="col-md-5">
                                        <label class="d-block">
                                            Voice (accent, slang, speech)
                                        <input type="text" readonly class="form-control" value="<%#DataBinder.Eval(Container.DataItem, "Voice")%>"></label>
                                    </div>
                                </div>
                            </div>
                            <div class="row">
                                <div class="form-group">
                                    <div class="col-md-12">
                                        <label class="d-block">
                                            Vehicle Info
                                        <input type="text" readonly class="form-control" value="<%#DataBinder.Eval(Container.DataItem, "Vehicle")%>"></label>
                                    </div>
                                </div>
                            </div>
                            <div class="row">
                                <div class="form-group">
                                    <div class="col-md-12">
                                        <label class="d-block">
                                            Distinguishing scars/marks/tattoos
                                        <textarea rows="4" readonly class="form-control"><%#DataBinder.Eval(Container.DataItem, "Features")%></textarea></label>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </ItemTemplate>
            <FooterTemplate>
                </div>
            </FooterTemplate>
        </asp:Repeater>
    </fieldset>
    <fieldset>
        <legend>Actions Taken on Scene</legend>
        <div class="row">
            <div class="form-group">
                <div class="col-md-6">
                    <asp:Label Text="Reported to Law Enforcement on (date)" runat="server" AssociatedControlID="txtDateReportedLeo" />
                    <asp:TextBox ID="txtDateReportedLeo" runat="server" ReadOnly="true" CssClass="form-control" />
                </div>
                <div class="col-md-6">
                    <asp:Label Text="Reported By" runat="server" AssociatedControlID="txtPersonReportingLeo" />
                    <asp:TextBox ID="txtPersonReportingLeo" runat="server" ReadOnly="true" CssClass="form-control" />

                </div>

            </div>
        </div>
        <div class="row">
            <div class="form-group">
                <div class="col-md-6">
                    <asp:Label Text="Law Enforcement Agency" runat="server" AssociatedControlID="txtAgency" />
                    <asp:TextBox ID="txtAgency" runat="server" ReadOnly="true" CssClass="form-control" />
                </div>
                <div class="col-md-6">
                    <asp:Label Text="Case Number" runat="server" AssociatedControlID="txtCaseNumber" />
                    <asp:TextBox ID="txtCaseNumber" runat="server" ReadOnly="true" CssClass="form-control" />

                </div>

            </div>
        </div>
        <div class="row">
            <div class="form-group">
                <div class="col-md-12">
                    <asp:Label Text="Other Actions" runat="server" AssociatedControlID="txtActionTaken" />
                    <asp:TextBox ID="txtActionTaken" ReadOnly="true" runat="server" TextMode="MultiLine" CssClass="form-control" Rows="5" />
                </div>
            </div>
        </div>

    </fieldset>
    <fieldset>
        <legend>Attachments</legend>
        <div id="attachments" runat="server">
            <asp:Repeater ID="rptAttachments" runat="server">
                <HeaderTemplate>
                    <ul class="list">
                </HeaderTemplate>
                <FooterTemplate>
                    </ul>
                </FooterTemplate>
                <ItemTemplate>
                    <li class="attachment"><a href='<%# TemplateSourceDirectory +"/attachment.ashx?id=" + Eval("AttachmentID") %>'><%#Eval("FileName") %></a></li>
                </ItemTemplate>
            </asp:Repeater>
        </div>
    </fieldset>
    <div class="row">
        <div class="col-md-12">
            <asp:HyperLink ID="lnkReturn" runat="server" CssClass="btn btn-primary btn-lg" Text="Return to List" />
        </div>
    </div>

</div>
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/jQuery/jquery.mask.js" />
<script type="text/javascript">
    /*globals jQuery, window, Sys */
    (function ($, Sys) {

        $(document).ready(function () {
            $('.phone_us').mask('(000) 000-0000');
        });
    }(jQuery, window.Sys));

</script>
