<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="View.ascx.cs" Inherits="tjc.Modules.jacs.View" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<%@ Register Src="Controls/navbar.ascx" TagPrefix="tb" TagName="navbar" %>
<section class="border-0 navbar mb-0 justify-content-start">
    <button class="btn btn-default me-3" type="button" data-bs-toggle="collapse" data-bs-target="#sidebarMenu" aria-controls="sidebarMenu" aria-expanded="true" aria-label="Toggle navigation">
        <i class="fa-solid fa-bars"></i>
    </button>
    <h2 class="mb-0">Welcome to JACS!</h2>

</section>

<div class="d-flex">
    <tb:navbar runat="server" ID="navbar" />
    <main class="main flex-grow-1 p-3 pt-0">
        <div class="animated fadeIn">
                <div class="alert alert-info">
                    <i class="fas fa-info-circle"></i> Use the sidebar to the left to create, edit or delete content.
                </div>
            <div class="input-group mb-2">
                <label for="case_num" class="input-group-text mb-0">Case #:</label>
                <input type="search" id="case_num" class="form-control" placeholder="Search by Case Number..." style="max-width: 200px;">
                <button type="button" id="search-button" class="btn btn-primary" onclick="return searchCaseNumber();">Find</button>
            </div>
            <div class="animated">
                <div class="col-sm-12" style="float: left">
                    <div class="card-header">
                        <div style="float: left">
                            <h4>Time Slots</h4>
                        </div>
                        <div style=""><span style=""><a href="http://localhost/timeslot-crud" class="d-flex flex-row justify-content-end">view all</a></span></div>
                    </div>
                    <table id="crudTable1" class="bg-white table table-striped table-hover nowrap rounded shadow-xs border-xs mt-2 dataTable dtr-inline" data-responsive-table="1" data-has-details-row="0" data-has-bulk-actions="0" cellspacing="0" aria-describedby="crudTable_info">
                        <thead>
                            <tr>
                                <th>Court</th>
                                <th>Date / Time</th>
                                <th>Length</th>
                                <th>Available</th>
                                <th>Quantity</th>
                                <th>Actions</th>
                            </tr>
                        </thead>

                        <tbody>
                        </tbody>
                    </table>
                </div>
                <div class="col-sm-12" style="float: left">
                    <div class="card-header">
                        <div style="float: left">
                            <h4>Events</h4>
                        </div>
                        <div><span><a href="http://localhost/event" class="d-flex flex-row justify-content-end">view all</a></span></div>
                    </div>
                    <table id="crudTable" class="bg-white table table-striped table-hover nowrap rounded shadow-xs border-xs mt-2 dataTable dtr-inline" data-responsive-table="1" data-has-details-row="0" data-has-bulk-actions="0" cellspacing="0" aria-describedby="crudTable_info">
                        <thead>
                            <tr>
                                <th>Case Number</th>
                                <th>Motion</th>
                                <th>Timeslot</th>
                                <th>Court</th>
                                <th>Status</th>
                                <th>Attorney</th>
                                <th>Opposing Attorney</th>
                                <th>Actions</th>
                            </tr>
                        </thead>
                        <tbody>
                        </tbody>
                    </table>
                </div>
            </div>
        </div>
    </main>
</div>
<dnn:DnnJsInclude runat="server" FilePath="/DesktopModules/tjc.modules/JACS/js/jacs.js" ForceProvider="DnnFormBottomProvider" Priority="100" />
<script>
    (function ($, Sys) {
        $(document).ready(function () {
            setActiveLink("lnkMain");
        });
    }(jQuery, window.Sys));
</script>
