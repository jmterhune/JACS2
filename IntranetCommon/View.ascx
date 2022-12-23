<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="View.ascx.cs" Inherits="tjc.Modules.IntranetCommon.View" %>

<div class="row">
    <div class="col-md-3">
        <div class="tabs tabs-vertical tabs-left tabs-navigation">
            <ul class="nav nav-tabs col-sm-3">
                <li class="nav-item">
                    <a href="#nav-team-site"><i class="fas fa-house"></i>Team Site</a>
                </li>
                <li class="nav-item active">
                    <a class="nav-link" href="#nav-dashboard" data-toggle="tab"><i class="fa-solid fa-gauge"></i>Dashboard</a>
                </li>
                <li class="nav-item">
                    <a class="nav-link" href="#nav-edit" data-toggle="tab"><i class="fa-solid fa-pencil"></i>Data Entry</a>
                </li>
                <li class="nav-item">
                    <a class="nav-link" href="#nav-reports" data-toggle="tab"><i class="fa-solid fa-chart-simple"></i>Reports</a>
                </li>
                <li class="nav-item">
                    <a class="nav-link" href="#nav-calendar" data-toggle="tab"><i class="fa-solid fa-calendar"></i>Event Calendar</a>
                </li>
                <li class="nav-item">
                    <a class="nav-link" href="#nav-repository" data-toggle="tab"><i class="fa-solid fa-folder-open"></i>Document Repository</a>
                </li>
                <li class="nav-item" id="liAdmin" runat="server" visible="false">
                    <a class="nav-link" href="#nav-repository" data-toggle="tab"><i class="fa-solid fa-folder-open"></i>Document Repository</a>
                </li>
            </ul>
        </div>
    </div>
    <div class="col-md-9">
        <div>
            <div class="btn-group" role="group" aria-label="Button group with nested dropdown">
                <button type="button" class="btn btn-secondary">1</button>
                <button type="button" class="btn btn-secondary">2</button>

                <div class="btn-group" role="group">
                    <button type="button" class="btn btn-secondary dropdown-toggle" data-toggle="dropdown" aria-expanded="false">
                        Dropdown
                    </button>
                    <div class="dropdown-menu">
                        <a class="dropdown-item" href="#">My Recent</a>
                        <a class="dropdown-item" href="#">Case Name</a>
                        <a class="dropdown-item" href="#">Case Number</a>
                        <a class="dropdown-item" href="#">Attorney</a>

                    </div>
                </div>
            </div>
            <button type="button" class="btn btn-primary mr-md">Search</button>
        </div>
        <asp:Repeater ID="rptLogEntries" runat="server" OnItemDataBound="rptLogEntries_ItemDataBound" OnItemCommand="rptLogEntries_ItemCommand">
            <HeaderTemplate>
                <table id="logList" class="table table-striped">
                    <thead>
                        <tr>
                            <th>Case Number</th>
                            <th>Case Name</th>
                            <th>Case Type</th>
                            <th>Action Date</th>
                            <th>Motion Filed</th>
                            <th>Responsible</th>
                            <th>Status</th>
                        </tr>
                    </thead>
                    <tbody>
            </HeaderTemplate>

            <ItemTemplate>
                <tr data-id="<%#DataBinder.Eval(Container.DataItem,"LogId").ToString() %>">
                    <td><%#DataBinder.Eval(Container.DataItem,"CaseNumber").ToString() %></td>
                    <td><%#DataBinder.Eval(Container.DataItem,"Description").ToString() %></td>
                    <td><%#DataBinder.Eval(Container.DataItem,"CaseTypeName").ToString() %></td>
                    <td><%#DataBinder.Eval(Container.DataItem,"DateReceived").ToString() %></td>
                    <td><%#DataBinder.Eval(Container.DataItem,"MotionFiled").ToString() %></td>
                    <td><%#DataBinder.Eval(Container.DataItem,"AttorneyName").ToString() %></td>
                    <td><%#DataBinder.Eval(Container.DataItem,"PhaseName").ToString() %></td>
                </tr>

            </ItemTemplate>
            <FooterTemplate>
                </tbody></table>
            </FooterTemplate>
        </asp:Repeater>

    </div>
</div>
