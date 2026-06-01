<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DetailPopUp.ascx.cs" Inherits="tjc.Modules.EmployeeDB.Views.DetailPopUp" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>

<div class="container-fluid">
    <div class="card employee-detail-card">
        <div class="card-body">
            <div class="row">
                <div class="col-md-4 text-center">
                    <asp:Image ID="imgPhoto" runat="server" CssClass="img-thumbnail employee-photo" />
                </div>
                <div class="col-md-8">
                    <h4>
                        <asp:Literal ID="ltName" runat="server" />
                    </h4>
                    <div class="employee-title">
                        <strong><asp:Literal ID="ltTitle" runat="server" /></strong>
                    </div>
                    <div class="employee-department">
                        <asp:Literal ID="ltDepartment" runat="server" />
                    </div>
                    <div class="employee-location">
                        <i class="fas fa-map-marker-alt"></i>&nbsp;<asp:Literal ID="ltLocation" runat="server" />
                    </div>
                    <hr />

                    <div class="employee-email">
                        <i class="fas fa-envelope"></i>&nbsp;<asp:Literal ID="ltEmail" runat="server" />
                    </div>

                    <div class="employee-phones">
                        <asp:Repeater ID="rptPhones" runat="server">
                            <HeaderTemplate>
                                <ul class="list-unstyled mb-0">
                            </HeaderTemplate>
                            <ItemTemplate>
                                <li>
                                    <i class="fas fa-phone"></i>&nbsp;
                                    <strong><%# Eval("PhoneType") %>:</strong>
                                    <a href='tel:<%# Eval("PhoneNumber") %>'><%# Eval("DisplayNumber") %></a>
                                </li>
                            </ItemTemplate>
                            <FooterTemplate>
                                </ul>
                            </FooterTemplate>
                        </asp:Repeater>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <asp:Panel ID="pnlNotFound" runat="server" Visible="false">
        <div class="alert alert-warning" role="alert">
            Employee not found.
        </div>
    </asp:Panel>
</div>
