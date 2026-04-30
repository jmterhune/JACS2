﻿<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="View.ascx.cs" Inherits="tjc.Modules.DocketInmateCompare.View" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<div class="container">
    <div class="mb-4">
        <h2>Upload Files</h2>
        <div class="row">
            <div class="col-md-6">
                <div class="mb-3">
                    <label for="fuCourtCSV" class="form-label">Court CSV File</label>
                    <asp:FileUpload ID="fuCourtCSV" runat="server" CssClass="form-control" />
                </div>
            </div>
            <div class="col-md-6">
                <div class="mb-3">
                    <label for="fuJailXLSX" class="form-label">Jail XLSX File</label>
                    <asp:FileUpload ID="fuJailXLSX" runat="server" CssClass="form-control" />
                </div>
            </div>
        </div>
        <asp:Button ID="btnProcess" runat="server" CssClass="btn btn-primary" Text="Process Files" OnClick="btnProcess_Click" />
    </div>
    
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <asp:GridView ID="gvMatches" ClientIDMode="Static" runat="server" CssClass="table table-striped table-hover" AutoGenerateColumns="false" UseAccessibleHeader="true"
                OnRowCommand="gvMatches_RowCommand" OnRowDataBound="gvMatches_RowDataBound" OnPreRender="gvMatches_PreRender" OnRowCreated="gvMatches_RowCreated">
                <Columns>
                    <asp:BoundField DataField="Id" HeaderText="ID" ItemStyle-CssClass="d-none" HeaderStyle-CssClass="d-none" />
                    <asp:BoundField DataField="CourtName" HeaderText="Court Name" />
                    <asp:BoundField DataField="JailName" HeaderText="Jail Name" />
                    <asp:BoundField DataField="CourtCase" HeaderText="Court Case" />
                    <asp:TemplateField HeaderText="Start Time">
                        <ItemTemplate>
                            <asp:TextBox ID="txtStart" Width="90" runat="server" Text='<%# Bind("Start") %>' CssClass="form-control"></asp:TextBox>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Event Type">
                        <ItemTemplate>
                            <asp:TextBox ID="txtEventType" Width="125" runat="server" Text='<%# Bind("EventType") %>' CssClass="form-control"></asp:TextBox>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Mode">
                        <ItemTemplate>
                            <asp:DropDownList ID="ddlMode" runat="server" CssClass="form-select" Width="100">
                                <asp:ListItem Value="Remote">Remote</asp:ListItem>
                                <asp:ListItem Value="In-Person">In-Person</asp:ListItem>
                            </asp:DropDownList>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="Similarity" HeaderText="Similarity" DataFormatString="{0:P0}" />
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:LinkButton ID="btnDelete" runat="server" CommandName="DeleteRow" CommandArgument="<%# Container.DataItemIndex %>" 
                                CssClass="btn btn-danger btn-sm" >
                                <i class="fas fa-trash"></i>
                            </asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </ContentTemplate>
    </asp:UpdatePanel>
    
    <asp:Panel ID="pnlFormDetails" runat="server" Visible="false">
        <div class="mt-4">
            <h2>Form Details</h2>
            <div class="row">
                <div class="col-md-6">
                    <div class="mb-3">
                        <label for="txtJudge" class="form-label">Requesting Judge</label>
                        <asp:TextBox ID="txtJudge" runat="server" CssClass="form-control"></asp:TextBox>
                    </div>
                    <div class="mb-3">
                        <label for="txtCourtroom" class="form-label">Courtroom</label>
                        <asp:TextBox ID="txtCourtroom" runat="server" CssClass="form-control"></asp:TextBox>
                    </div>
                    <div class="mb-3">
                        <label for="txtDate" class="form-label">Date of Hearing</label>
                        <asp:TextBox ID="txtDate" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                    </div>
                </div>
                <div class="col-md-6">
                    <div class="mb-3">
                        <label for="txtSubmittedBy" class="form-label">Submitted By</label>
                        <asp:TextBox ID="txtSubmittedBy" runat="server" CssClass="form-control"></asp:TextBox>
                    </div>
                    <div class="mb-3">
                        <label for="txtZoomID" class="form-label">Zoom Meeting ID</label>
                        <asp:TextBox ID="txtZoomID" runat="server" CssClass="form-control"></asp:TextBox>
                    </div>
                    <div class="mb-3">
                        <label for="txtPassword" class="form-label">Zoom Password</label>
                        <asp:TextBox ID="txtPassword" runat="server" CssClass="form-control"></asp:TextBox>
                    </div>
                </div>
            </div>
            <asp:Button ID="btnGeneratePDF" runat="server" CssClass="btn btn-primary" Text="Generate PDF" OnClick="btnGeneratePDF_Click" />
        </div>
    </asp:Panel>
</div>
<dnn:dnncssinclude runat="server" filepath="/Resources/Libraries/DataTables/dataTables.bootstrap5.min.css" />
<dnn:dnnjsinclude runat="server" filepath="/Resources/Libraries/DataTables/dataTables.min.js" />
<dnn:dnnjsinclude runat="server" filepath="/Resources/Libraries/DataTables/dataTables.bootstrap5.min.js" />
<script type="text/javascript">
    function initDataTable() {
        if ($.fn.DataTable.isDataTable('#gvMatches')) {
            $('#gvMatches').DataTable().destroy();
        }
        $('#gvMatches').DataTable({
            paging: false,
            searching: false,
            order: [],
            dom: 'Bfrtip',
            columns: [
                { orderable: false },
                { orderable: true },
                { orderable: true },
                { orderable: true },
                { orderable: true },
                { orderable: true },
                { orderable: false },
                { orderable: true },
                { orderable: false }
            ]
        });
    }

    $(document).ready(function () {
        initDataTable();
    });

    var prm = Sys.WebForms.PageRequestManager.getInstance();
    prm.add_endRequest(function () {
        initDataTable();
    });
</script>
