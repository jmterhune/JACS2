<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EeoSetup.ascx.cs" Inherits="tjc.Modules.EmployeeDB.EeoSetup" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<div class="tabs">
    <ul class="nav nav-tabs">
        <li class="nav-item active">
            <a class="nav-link " href="#EEO" data-toggle="tab">EEO List</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="#Review" data-toggle="tab">Review This Years EEO Data</a>
        </li>


    </ul>
    <div class="tab-content">
        <div id="EEO" class="tab-pane active">
            <asp:UpdatePanel ID="pnlEeo" runat="server" RenderMode="Block" OnUnload="pnlEeo_Unload" UpdateMode="Always">
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
                    <asp:Repeater ID="rptEEO" runat="server" OnItemCommand="rptEEO_ItemCommand" OnItemCreated="rptEEO_ItemCreated">
                        <HeaderTemplate>
                            <table id="tblEEO" class="table table-striped">
                                <thead>
                                    <tr>
                                        <th>&nbsp;</th>
                                        <th>Job Category</th>
                                        <th>Year</th>
                                        <th title="Population Male">A<br />
                                            M</th>
                                        <th title="Population Female">A<br />
                                            F</th>
                                        <th title="Population White">A<br />
                                            W</th>
                                        <th title="Population Black">A<br />
                                            B</th>
                                        <th title="Population Asian">A<br />
                                            A</th>
                                        <th title="Population Hispanic">A<br />
                                            H</th>
                                        <th title="Population Other">A<br />
                                            O</th>
                                        <th title="Hired Male">C<br />
                                            M</th>
                                        <th title="Hired Female">C<br />
                                            F</th>
                                        <th title="Hired White">C<br />
                                            W</th>
                                        <th title="Hired Black">C<br />
                                            B</th>
                                        <th title="Hired Asian">C<br />
                                            A</th>
                                        <th title="Hired Hispanic">C<br />
                                            H</th>
                                        <th title="Hired Other">C<br />
                                            O</th>
                                        <th title="Promoted Male">D<br />
                                            M</th>
                                        <th title="Promoted Female">D<br />
                                            F</th>
                                        <th title="Promoted White">D<br />
                                            W</th>
                                        <th title="Promoted Black">D<br />
                                            B</th>
                                        <th title="Promoted Asian">D<br />
                                            A</th>
                                        <th title="Promoted Hispanic">D<br />
                                            H</th>
                                        <th title="Promoted Other">D<br />
                                            O</th>
                                        <th title="Transferred Male">E<br />
                                            M</th>
                                        <th title="Transferred Female">E<br />
                                            F</th>
                                        <th title="Transferred White">E<br />
                                            W</th>
                                        <th title="Transferred Black">E<br />
                                            B</th>
                                        <th title="Transferred Asian">E<br />
                                            A</th>
                                        <th title="Transferred Hispanic">E<br />
                                            H</th>
                                        <th title="Transferred Other">E<br />
                                            O</th>
                                        <th title="Terminated Male">F<br />
                                            M</th>
                                        <th title="Terminated Female">F<br />
                                            F</th>
                                        <th title="Terminated White">F<br />
                                            W</th>
                                        <th title="Terminated Black">F<br />
                                            B</th>
                                        <th title="Terminated Asian">F<br />
                                            A</th>
                                        <th title="Terminated Hispanic">F<br />
                                            H</th>
                                        <th title="Terminated Other">F<br />
                                            O</th>

                                        <th>&nbsp;</th>
                                    </tr>
                                </thead>
                                <tbody>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <tr>
                                <td class="command-icon">
                                    <asp:LinkButton ID="cmdEdit" runat="server" CommandName="edit" CausesValidation="false" CommandArgument='<%#DataBinder.Eval(Container.DataItem,"EeoId").ToString() %>'><i class="fa fa-pencil"></i></asp:LinkButton>
                                </td>
                                <td><%#DataBinder.Eval(Container.DataItem,"JobGroupName") %></td>
                                <td><%#DataBinder.Eval(Container.DataItem,"Year") %></td>
                                <td><%#DataBinder.Eval(Container.DataItem,"PopulationMale","{0:F0}") %></td>
                                <td><%#DataBinder.Eval(Container.DataItem,"PopulationFemale","{0:F0}") %></td>
                                <td><%#DataBinder.Eval(Container.DataItem,"PopulationWhite","{0:F0}") %></td>
                                <td><%#DataBinder.Eval(Container.DataItem,"PopulationBlack","{0:F0}") %></td>
                                <td><%#DataBinder.Eval(Container.DataItem,"PopulationAsian","{0:F0}") %></td>
                                <td><%#DataBinder.Eval(Container.DataItem,"PopulationHispanic","{0:F0}") %></td>
                                <td><%#DataBinder.Eval(Container.DataItem,"PopulationOther","{0:F0}") %></td>
                                <td><%#DataBinder.Eval(Container.DataItem,"HireMale","{0:F0}") %></td>
                                <td><%#DataBinder.Eval(Container.DataItem,"HireFemale","{0:F0}") %></td>
                                <td><%#DataBinder.Eval(Container.DataItem,"HireWhite","{0:F0}") %></td>
                                <td><%#DataBinder.Eval(Container.DataItem,"HireBlack","{0:F0}") %></td>
                                <td><%#DataBinder.Eval(Container.DataItem,"HireAsian","{0:F0}") %></td>
                                <td><%#DataBinder.Eval(Container.DataItem,"HireHispanic","{0:F0}") %></td>
                                <td><%#DataBinder.Eval(Container.DataItem,"HireOther","{0:F0}") %></td>
                                <td><%#DataBinder.Eval(Container.DataItem,"PromoMale","{0:F0}") %></td>
                                <td><%#DataBinder.Eval(Container.DataItem,"PromoFemale","{0:F0}") %></td>
                                <td><%#DataBinder.Eval(Container.DataItem,"PromoWhite","{0:F0}") %></td>
                                <td><%#DataBinder.Eval(Container.DataItem,"PromoBlack","{0:F0}") %></td>
                                <td><%#DataBinder.Eval(Container.DataItem,"PromoAsian","{0:F0}") %></td>
                                <td><%#DataBinder.Eval(Container.DataItem,"PromoHispanic","{0:F0}") %></td>
                                <td><%#DataBinder.Eval(Container.DataItem,"PromoOther","{0:F0}") %></td>
                                <td><%#DataBinder.Eval(Container.DataItem,"TransferMale","{0:F0}") %></td>
                                <td><%#DataBinder.Eval(Container.DataItem,"TransferFemale","{0:F0}") %></td>
                                <td><%#DataBinder.Eval(Container.DataItem,"TransferWhite","{0:F0}") %></td>
                                <td><%#DataBinder.Eval(Container.DataItem,"TransferBlack","{0:F0}") %></td>
                                <td><%#DataBinder.Eval(Container.DataItem,"TransferAsian","{0:F0}") %></td>
                                <td><%#DataBinder.Eval(Container.DataItem,"TransferHispanic","{0:F0}") %></td>
                                <td><%#DataBinder.Eval(Container.DataItem,"TransferOther","{0:F0}") %></td>
                                <td><%#DataBinder.Eval(Container.DataItem,"TermMale","{0:F0}") %></td>
                                <td><%#DataBinder.Eval(Container.DataItem,"TermFemale","{0:F0}") %></td>
                                <td><%#DataBinder.Eval(Container.DataItem,"TermWhite","{0:F0}") %></td>
                                <td><%#DataBinder.Eval(Container.DataItem,"TermBlack","{0:F0}") %></td>
                                <td><%#DataBinder.Eval(Container.DataItem,"TermAsian","{0:F0}") %></td>
                                <td><%#DataBinder.Eval(Container.DataItem,"TermHispanic","{0:F0}") %></td>
                                <td><%#DataBinder.Eval(Container.DataItem,"TermOther","{0:F0}") %></td>
                                <td class="command-icon">
                                    <asp:LinkButton ID="cmdDelete" CssClass="confirm" runat="server" CausesValidation="false" CommandName="delete" CommandArgument='<%#DataBinder.Eval(Container.DataItem,"EeoId").ToString() %>'><i class="fa fa-trash"></i></asp:LinkButton></td>
                            </tr>
                        </ItemTemplate>
                        <FooterTemplate>
                            </tbody></table><hr />
                        </FooterTemplate>
                    </asp:Repeater>
                    <div class="modal fade" id="EditEeoModal" tabindex="-1" role="dialog" aria-labelledby="EditEeoModalLabel" aria-hidden="true">
                        <div class="modal-dialog modal-lg">
                            <div class="modal-content">
                                <div class="modal-header">
                                    <h4 class="modal-title" id="EditEeoModalLabel">Add / Edit EEO Data</h4>
                                    <button type="button" class="close" data-bs-dismiss="modal" aria-hidden="true">&times;</button>
                                </div>
                                <div class="modal-body">
                                    <div class="row gy-2 gx-3 form-group">
                                        <div class="col-5">
                                            <asp:Label runat="server" AssociatedControlID="drpCategory" Text="Job Category" />
                                            <asp:DropDownList ID="drpCategory" runat="server" CssClass="form-control" AppendDataBoundItems="true">
                                                <asp:ListItem Text="< Select Category >" Value=""></asp:ListItem>
                                            </asp:DropDownList>
                                        </div>
                                        <div class="col-3">
                                            <asp:Label runat="server" AssociatedControlID="txtYear" Text="Year" />
                                            <asp:TextBox runat="server" TextMode="Number" CssClass="form-control" MaxLength="5" ID="txtYear" />
                                        </div>
                                    </div>
                                    <div class="row form-group">
                                        <div class="col-6 row">
                                            <div class="col-3">
                                                <asp:Label runat="server" ToolTip="Population Male" AssociatedControlID="txtPopMale" Text="AM" />
                                                <asp:TextBox runat="server" TextMode="Number" CssClass="form-control" MaxLength="6" ID="txtPopMale" />
                                            </div>
                                            <div class="col-3">
                                                <asp:Label runat="server" ToolTip="Population Female" AssociatedControlID="txtPopFemale" Text="AF" />
                                                <asp:TextBox runat="server" TextMode="Number" CssClass="form-control" MaxLength="6" ID="txtPopFemale" />
                                            </div>
                                            <div class="col-3">
                                                <asp:Label runat="server" ToolTip="Population White" AssociatedControlID="txtPopWhite" Text="AW" />
                                                <asp:TextBox runat="server" TextMode="Number" CssClass="form-control" MaxLength="6" ID="txtPopWhite" />
                                            </div>
                                            <div class="col-3">
                                                <asp:Label runat="server" ToolTip="Population Black" AssociatedControlID="txtPopBlack" Text="AB" />
                                                <asp:TextBox runat="server" TextMode="Number" CssClass="form-control" MaxLength="6" ID="txtPopBlack" />
                                            </div>
                                        </div>
                                        <div class="col-5 row">
                                            <div class="col-4">
                                                <asp:Label runat="server" ToolTip="Population Asian" AssociatedControlID="txtPopAsian" Text="AA" />
                                                <asp:TextBox runat="server" TextMode="Number" CssClass="form-control" MaxLength="6" ID="txtPopAsian" />
                                            </div>
                                            <div class="col-4">
                                                <asp:Label runat="server" ToolTip="Population Hispanic" AssociatedControlID="txtPopHispanic" Text="AH" />
                                                <asp:TextBox runat="server" TextMode="Number" CssClass="form-control" MaxLength="6" ID="txtPopHispanic" />
                                            </div>
                                            <div class="col-4">
                                                <asp:Label runat="server" ToolTip="Population Other" AssociatedControlID="txtPopOther" Text="AO" />
                                                <asp:TextBox runat="server" TextMode="Number" CssClass="form-control" MaxLength="6" ID="txtPopOther" />
                                            </div>
                                        </div>
                                    </div>
                                    <div class="row form-group">
                                        <div class="col-6 row">
                                            <div class="col-3">
                                                <asp:Label runat="server" ToolTip="Hired Male" AssociatedControlID="txtHireMale" Text="CM" />
                                                <asp:TextBox runat="server" TextMode="Number" CssClass="form-control" MaxLength="6" ID="txtHireMale" />
                                            </div>
                                            <div class="col-3">
                                                <asp:Label runat="server" ToolTip="Hired Female" AssociatedControlID="txtHireFemale" Text="CF" />
                                                <asp:TextBox runat="server" TextMode="Number" CssClass="form-control" MaxLength="6" ID="txtHireFemale" />
                                            </div>
                                            <div class="col-3">
                                                <asp:Label runat="server" ToolTip="Hired White" AssociatedControlID="txtHireWhite" Text="CW" />
                                                <asp:TextBox runat="server" TextMode="Number" CssClass="form-control" MaxLength="6" ID="txtHireWhite" />
                                            </div>
                                            <div class="col-3">
                                                <asp:Label runat="server" ToolTip="Hired Black" AssociatedControlID="txtHireBlack" Text="CB" />
                                                <asp:TextBox runat="server" TextMode="Number" CssClass="form-control" MaxLength="6" ID="txtHireBlack" />
                                            </div>
                                        </div>
                                        <div class="col-5 row">
                                            <div class="col-4">
                                                <asp:Label runat="server" ToolTip="Hired Asian" AssociatedControlID="txtHireAsian" Text="CA" />
                                                <asp:TextBox runat="server" TextMode="Number" CssClass="form-control" MaxLength="6" ID="txtHireAsian" />
                                            </div>
                                            <div class="col-4">
                                                <asp:Label runat="server" ToolTip="Hired Hispanic" AssociatedControlID="txtHireHispanic" Text="CH" />
                                                <asp:TextBox runat="server" TextMode="Number" CssClass="form-control" MaxLength="6" ID="txtHireHispanic" />
                                            </div>
                                            <div class="col-4">
                                                <asp:Label runat="server" ToolTip="Hired Other" AssociatedControlID="txtHireOther" Text="CO" />
                                                <asp:TextBox runat="server" TextMode="Number" CssClass="form-control" MaxLength="6" ID="txtHireOther" />
                                            </div>
                                        </div>
                                    </div>
                                    <div class="row form-group">
                                        <div class="col-6 row">
                                            <div class="col-3">
                                                <asp:Label runat="server" ToolTip="Promoted Male" AssociatedControlID="txtPromMale" Text="DM" />
                                                <asp:TextBox runat="server" TextMode="Number" CssClass="form-control" MaxLength="6" ID="txtPromMale" />
                                            </div>
                                            <div class="col-3">
                                                <asp:Label runat="server" ToolTip="Promoted Female" AssociatedControlID="txtPromFemale" Text="DF" />
                                                <asp:TextBox runat="server" TextMode="Number" CssClass="form-control" MaxLength="6" ID="txtPromFemale" />
                                            </div>
                                            <div class="col-3">
                                                <asp:Label runat="server" ToolTip="Promoted White" AssociatedControlID="txtPromWhite" Text="DW" />
                                                <asp:TextBox runat="server" TextMode="Number" CssClass="form-control" MaxLength="6" ID="txtPromWhite" />
                                            </div>
                                            <div class="col-3">
                                                <asp:Label runat="server" ToolTip="Promoted Black" AssociatedControlID="txtPromBlack" Text="DB" />
                                                <asp:TextBox runat="server" TextMode="Number" CssClass="form-control" MaxLength="6" ID="txtPromBlack" />
                                            </div>
                                        </div>
                                        <div class="col-5 row">
                                            <div class="col-4">
                                                <asp:Label runat="server" ToolTip="Promoted Asian" AssociatedControlID="txtPromAsian" Text="DA" />
                                                <asp:TextBox runat="server" TextMode="Number" CssClass="form-control" MaxLength="6" ID="txtPromAsian" />
                                            </div>
                                            <div class="col-4">
                                                <asp:Label runat="server" ToolTip="Promoted Hispanic" AssociatedControlID="txtPromHispanic" Text="DH" />
                                                <asp:TextBox runat="server" TextMode="Number" CssClass="form-control" MaxLength="6" ID="txtPromHispanic" />
                                            </div>
                                            <div class="col-4">
                                                <asp:Label runat="server" ToolTip="Promoted Other" AssociatedControlID="txtPromOther" Text="DO" />
                                                <asp:TextBox runat="server" TextMode="Number" CssClass="form-control" MaxLength="6" ID="txtPromOther" />
                                            </div>
                                        </div>
                                    </div>
                                    <div class="row form-group">
                                        <div class="col-6 row">
                                            <div class="col-3">
                                                <asp:Label runat="server" ToolTip="Transferred Male" AssociatedControlID="txtTransMale" Text="EM" />
                                                <asp:TextBox runat="server" TextMode="Number" CssClass="form-control" MaxLength="6" ID="txtTransMale" />
                                            </div>
                                            <div class="col-3">
                                                <asp:Label runat="server" ToolTip="Transferred Female" AssociatedControlID="txtTransFemale" Text="EF" />
                                                <asp:TextBox runat="server" TextMode="Number" CssClass="form-control" MaxLength="6" ID="txtTransFemale" />
                                            </div>
                                            <div class="col-3">
                                                <asp:Label runat="server" ToolTip="Transferred White" AssociatedControlID="txtTransWhite" Text="EW" />
                                                <asp:TextBox runat="server" TextMode="Number" CssClass="form-control" MaxLength="6" ID="txtTransWhite" />
                                            </div>
                                            <div class="col-3">
                                                <asp:Label runat="server" ToolTip="Transferred Black" AssociatedControlID="txtTransBlack" Text="EB" />
                                                <asp:TextBox runat="server" TextMode="Number" CssClass="form-control" MaxLength="6" ID="txtTransBlack" />
                                            </div>
                                        </div>
                                        <div class="col-5 row">
                                            <div class="col-4">
                                                <asp:Label runat="server" ToolTip="Transferred Asian" AssociatedControlID="txtTransAsian" Text="EA" />
                                                <asp:TextBox runat="server" TextMode="Number" CssClass="form-control" MaxLength="6" ID="txtTransAsian" />
                                            </div>
                                            <div class="col-4">
                                                <asp:Label runat="server" ToolTip="Transferred Hispanic" AssociatedControlID="txtTransHispanic" Text="EH" />
                                                <asp:TextBox runat="server" TextMode="Number" CssClass="form-control" MaxLength="6" ID="txtTransHispanic" />
                                            </div>
                                            <div class="col-4">
                                                <asp:Label runat="server" ToolTip="Transferred Other" AssociatedControlID="txtTransOther" Text="EO" />
                                                <asp:TextBox runat="server" TextMode="Number" CssClass="form-control" MaxLength="6" ID="txtTransOther" />
                                            </div>
                                        </div>
                                    </div>
                                    <div class="row form-group">
                                        <div class="col-6 row">
                                            <div class="col-3">
                                                <asp:Label runat="server" ToolTip="Terminated Male" AssociatedControlID="txtTermMale" Text="FM" />
                                                <asp:TextBox runat="server" TextMode="Number" CssClass="form-control" MaxLength="6" ID="txtTermMale" />
                                            </div>
                                            <div class="col-3">
                                                <asp:Label runat="server" ToolTip="Terminated Female" AssociatedControlID="txtTermFemale" Text="FF" />
                                                <asp:TextBox runat="server" TextMode="Number" CssClass="form-control" MaxLength="6" ID="txtTermFemale" />
                                            </div>
                                            <div class="col-3">
                                                <asp:Label runat="server" ToolTip="Terminated White" AssociatedControlID="txtTermWhite" Text="FW" />
                                                <asp:TextBox runat="server" TextMode="Number" CssClass="form-control" MaxLength="6" ID="txtTermWhite" />
                                            </div>
                                            <div class="col-3">
                                                <asp:Label runat="server" ToolTip="Terminated Black" AssociatedControlID="txtTermBlack" Text="FB" />
                                                <asp:TextBox runat="server" TextMode="Number" CssClass="form-control" MaxLength="6" ID="txtTermBlack" />
                                            </div>
                                        </div>
                                        <div class="col-5 row">
                                            <div class="col-4">
                                                <asp:Label runat="server" ToolTip="Terminated Asian" AssociatedControlID="txtTermAsian" Text="FA" />
                                                <asp:TextBox runat="server" TextMode="Number" CssClass="form-control" MaxLength="6" ID="txtTermAsian" />
                                            </div>
                                            <div class="col-4">
                                                <asp:Label runat="server" ToolTip="Terminated Hispanic" AssociatedControlID="txtTermHispanic" Text="FH" />
                                                <asp:TextBox runat="server" TextMode="Number" CssClass="form-control" MaxLength="6" ID="txtTermHispanic" />
                                            </div>
                                            <div class="col-4">
                                                <asp:Label runat="server" ToolTip="Terminated Other" AssociatedControlID="txtTermOther" Text="FO" />
                                                <asp:TextBox runat="server" TextMode="Number" CssClass="form-control" MaxLength="6" ID="txtTermOther" />
                                                <asp:HiddenField ID="hdEeoId" runat="server" />
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="modal-footer">
                                    <asp:Button OnClientClick="ToggleEditForm(false)" CssClass="btn btn-primary" ID="cmdSave" runat="server" Text="Save" OnClick="cmdSave_Click" />
                                    <button type="button" class="btn btn-default" data-bs-dismiss="modal">Close</button>
                                </div>
                            </div>
                        </div>
                    </div>
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="cmdSave" EventName="Click" />
                </Triggers>

            </asp:UpdatePanel>
        </div>
        <div id="Review" class="tab-pane">
            <div class="alert alert-info"><i class="fa fa-info-circle"></i>This feature allows you to add EEO information extracted from the Employee Database. Select the start and end date for the Year, select Review EEO Values to calculate current values. If the values look correct, select Accept to add the records to the database.</div>
            <asp:UpdatePanel ID="upReview" runat="server" RenderMode="Block" UpdateMode="Always" OnUnload="upReview_Unload">
                <ContentTemplate>
                    <asp:UpdateProgress ID="upReviewProgress" runat="server">
                        <ProgressTemplate>
                            <div class="modal-progress">
                                <div class="center-progress">
                                    <img alt="" src="/images/loading.gif" />
                                </div>
                            </div>
                        </ProgressTemplate>
                    </asp:UpdateProgress>
                    <div class="form-group row">
                        <div class="col-auto">
                            <asp:Label runat="server" AssociatedControlID="txtStartDate" Text="Start Date" />
                            <asp:TextBox runat="server" CssClass="form-control datepicker" MaxLength="25" ID="txtStartDate" />
                            <asp:RequiredFieldValidator runat="server" ControlToValidate="txtStartDate"
                                Display="Dynamic" ValidationGroup="Review" SetFocusOnError="true" CssClass="label label-danger" ErrorMessage="Start Date is Required" />
                        </div>
                        <div class="col-auto">
                            <asp:Label runat="server" AssociatedControlID="txtEndDate" Text="End Date" />
                            <asp:TextBox runat="server" CssClass="form-control datepicker" MaxLength="25" ID="txtEndDate" />
                            <asp:RequiredFieldValidator runat="server" ControlToValidate="txtEndDate"
                                Display="Dynamic" ValidationGroup="Review" SetFocusOnError="true" CssClass="label label-danger" ErrorMessage="End Date is Required" />
                        </div>

                    </div>
                    <asp:Literal runat="server" ID="ltEEOInfo" />
                    <p>
                        <asp:Button Text="Review EEO Values" ID="cmdReview" ValidationGroup="Review" runat="server" CssClass="btn btn-primary" OnClick="cmdReview_Click" />
                        <asp:Button ID="cmdAccept" visible="false" runat="server" CssClass="btn btn-success" CausesValidation="false" Text="Publish Results to Database" OnClick="cmdAccept_Click" />
                    </p>
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="cmdAccept" EventName="Click" />
                     <asp:AsyncPostBackTrigger ControlID="cmdReview" EventName="Click" />
                </Triggers>

            </asp:UpdatePanel>

        </div>
    </div>
    <hr />
    <a class="btn btn-default" href='<%=EmployeeUrl %>'><i class="fas fa-arrow-left"></i>&nbsp;Return to Employee List</a>

</div>

<dnn:dnnjsinclude runat="server" filepath="https://cdn.datatables.net/v/bs5/dt-1.13.1/datatables.min.js" />
<dnn:dnncssinclude runat="server" filepath="https://cdn.datatables.net/v/bs5/dt-1.13.1/datatables.min.css" />
<dnn:dnncssinclude runat="server" filepath="~/Resources/Shared/components/TimePicker/Themes/jquery-ui.min.css" />


<script type="text/javascript">

    (function ($, Sys) {
        $(document).ready(function () {
            Sys.Application.add_load(function (s, e) { PageInit(); });
            PageInit();

        });
    }(jQuery, window.Sys));
    function PageInit() {
        $(".datepicker").datepicker();
        var table = $('#tblEEO').DataTable({

            "order": [[2, "desc"]],
            "oLanguage": {
                "sSearch": "Filter by Text"
            }, "aoColumns": [
                { "bSortable": false },
                { "bSortable": true },
                { "bSortable": true },
                { "bSortable": false },
                { "bSortable": false },
                { "bSortable": false },
                { "bSortable": false },
                { "bSortable": false },
                { "bSortable": false },
                { "bSortable": false },
                { "bSortable": false },
                { "bSortable": false },
                { "bSortable": false },
                { "bSortable": false },
                { "bSortable": false },
                { "bSortable": false },
                { "bSortable": false },
                { "bSortable": false },
                { "bSortable": false },
                { "bSortable": false },
                { "bSortable": false },
                { "bSortable": false },
                { "bSortable": false },
                { "bSortable": false },
                { "bSortable": false },
                { "bSortable": false },
                { "bSortable": false },
                { "bSortable": false },
                { "bSortable": false },
                { "bSortable": false },
                { "bSortable": false },
                { "bSortable": false },
                { "bSortable": false },
                { "bSortable": false },
                { "bSortable": false },
                { "bSortable": false },
                { "bSortable": false },
                { "bSortable": false },
                { "bSortable": false },
            ]

        });
        $("#tblEEO_length").prepend('<button class="btn btn-primary btn-sm me-2" data-bs-toggle="modal" data-bs-target="#EditEeoModal"><i class="fa fa-plus"></i>&nbsp;Add EEO Data</button>');
        table.draw();

        $(".confirm").dnnConfirm({
            text: 'Are you sure you wish to delete this Record?',
            yesText: 'Yes',
            noText: 'No',
            title: 'Delete Record?'
        });
    }
    function ToggleEditForm(toggleValue) {
        if (toggleValue) {
            $('#EditEeoModal').modal('show');
        } else {
            $('#EditEeoModal').modal('hide');
            $('body').removeClass('modal-open');
            $('.modal-backdrop').remove();
        }

        return true;
    }
</script>
