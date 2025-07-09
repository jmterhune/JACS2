<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="View.ascx.cs" Inherits="tjc.Modules.jacs.View" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<%@ Register Src="Controls/navbar.ascx" TagPrefix="tb" TagName="navbar" %>
<section class="border-0 navbar mb-0 justify-content-start">
    <button class="btn btn-default me-3" id="btnToggleMenu" type="button" data-bs-toggle="collapse" data-bs-target="#sidebarMenu" aria-controls="sidebarMenu" aria-expanded="true" aria-label="Toggle navigation">
        <i class="fa-solid fa-bars"></i>
    </button>
    <h2 class="mb-0">Welcome to JACS!</h2>

</section>

<div class="d-flex">
    <tb:navbar runat="server" ID="navbar" />
    <main class="main flex-grow-1 p-3 pt-0">
        <div class="animated fadeIn">
            <div class="alert alert-info">
                <i class="fas fa-info-circle"></i>Use the sidebar to the left to create, edit or delete content.
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
                        <div style=""><span style=""><a href='<%=TimeSlotListUrl %>' class="d-flex flex-row justify-content-end">view all</a></span></div>
                    </div>
                    <asp:Repeater runat="server" ID="rptTimeslots">
                        <HeaderTemplate>
                            <table id="tblTimeslots" class="bg-white table table-striped table-hover nowrap rounded shadow-xs border-xs mt-2 dataTable dtr-inline">
                                <thead>
                                    <tr>
                                        <th>Court</th>
                                        <th>Date / Time</th>
                                        <th>Length</th>
                                        <th>Available</th>
                                        <th>Quantity</th>
                                        <th></th>
                                    </tr>
                                </thead>
                                <tbody>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <tr>
                                <td><%#Eval("description") %></td>
                                <td><%#Eval("FormattedStart") %></td>
                                <td><%#Eval("duration") %></td>
                                <td><%#Eval("available") != null && Convert.ToBoolean(Eval("available")) ? "Yes" : "No" %></td>
                                <td><%#Eval("quantity") %></td>
                                <td></td>
                            </tr>
                        </ItemTemplate>
                        <FooterTemplate>
                            </tbody>
                            </table>
                        </FooterTemplate>
                    </asp:Repeater>
                </div>
                <div class="col-sm-12" style="float: left">
                    <div class="card-header">
                        <div style="float: left">
                            <h4>Events</h4>
                        </div>
                        <div><span><a href='<%=EventListUrl %>' class="d-flex flex-row justify-content-end">view all</a></span></div>
                    </div>
                    <asp:Repeater runat="server" ID="rptEvents">
                        <HeaderTemplate>
                            <table id="tblEvents" class="bg-white table table-striped table-hover nowrap rounded shadow-xs border-xs mt-2 dataTable dtr-inline">
                                <thead>
                                    <tr>
                                        <th>Case Number</th>
                                        <th>Motion</th>
                                        <th>Timeslot</th>
                                        <th>Court</th>
                                        <th>Status</th>
                                        <th>Attorney</th>
                                        <th>Opposing Attorney</th>
                                        <th></th>
                                    </tr>
                                </thead>
                                <tbody>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <tr>
                                <td><%#Eval("case_num") %></td>
                                <td><%#Eval("motion_name") %></td>
                                <td><%#Eval("timeslot_desc") %></td>
                                <td><%#Eval("court_name") %></td>
                                <td><%#Eval("status_name") %></td>
                                <td><%#Eval("attorney_name") %></td>
                                <td><%#Eval("opp_attorney_name") %></td>
                                <td></td>
                            </tr>
                        </ItemTemplate>
                        <FooterTemplate>
                            </tbody>
                            </table>
                        </FooterTemplate>
                    </asp:Repeater>
                </div>
            </div>
        </div>
    </main>

</div>
<!-- Modal for Case Search Results -->
<div class="modal fade dtr-bs-modal" id="caseSearchModal" role="dialog" aria-hidden="true">
    <div class="modal-dialog" role="document">
        <div class="modal-content">
            <div class="modal-header">
                <h4 class="modal-title">Case Details</h4>
                <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                    <span aria-hidden="true">&times;</span>
                </button>
            </div>
            <div class="modal-body">
                <table id="caseDetailsTable" class="table table-striped mb-0" />
            </div>
        </div>
    </div>
</div>
<div class="modal fade dtr-bs-modal" style="" aria-modal="true" role="dialog">
     <div class="modal-dialog" role="document">
         <div class="modal-content">
             <div class="modal-header">
                 <h4 class="modal-title"></h4><button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">×</span></button>
             </div>
             <div class="modal-body">
                 <table class="table table-striped mb-0">

                 </table>
             </div>
         </div>
     </div>
 </div>

<!-- Include jQuery, SweetAlert2, and Bootstrap for styling and functionality -->
<dnn:DnnJsInclude runat="server" FilePath="/Resources/Libraries/sweetalert/sweetalert2.min.js" />
<dnn:DnnCssInclude runat="server" FilePath="/Resources/Libraries/sweetalert/sweetalert2.min.css" />
<dnn:DnnJsInclude runat="server" FilePath="/DesktopModules/tjc.modules/JACS/js/jacs.js" ForceProvider="DnnFormBottomProvider" Priority="100" />
<script>
    var moduleId = <%=ModuleId%>;
    var service = {
        path: "JACS",
        framework: $.ServicesFramework(moduleId)
    };
   
    (function ($, Sys) {
        $(document).ready(function () {
            setActiveLink("lnkMain");
        });
    }(jQuery, window.Sys));

    function searchCaseNumber() {
        var caseNum = { searchTerm: $("#case_num").val()} ;
        service.baseUrl = service.framework.getServiceRoot(service.path);
        var searchUrl = `${service.baseUrl}EventAPI/SearchCaseNumber`;
        $.ajax({
            url: searchUrl,
            method: 'POST',
            contentType: 'application/json; charset=utf-8',
            data: JSON.stringify(caseNum),
            success: function (response) {
                if (!response.data) {
                    alert("No Event found with this case number!");
                    return;
                }
                var data = response.data;
                var caseDetails = '<tbody>' +
                    '<tr><td style="vertical-align:top; border:none;"><strong>Case Number:</strong></td><td style="padding-left:10px;padding-bottom:10px; border:none;">' + data.CaseNum + '</td></tr>' +
                    '<tr><td style="vertical-align:top; border:none;"><strong>Motion:</strong></td><td style="padding-left:10px;padding-bottom:10px; border:none;">' + (data.Motion ? data.Motion.Description : '-') + '</td></tr>' +
                    '<tr><td style="vertical-align:top; border:none;"><strong>Timeslot:</strong></td><td style="padding-left:10px;padding-bottom:10px; border:none;">' + data.Timeslot.Date + ' @ ' + data.Timeslot.StartTime + '</td></tr>' +
                    '<tr><td style="vertical-align:top; border:none;"><strong>Court:</strong></td><td style="padding-left:10px;padding-bottom:10px; border:none;">' + data.Timeslot.Court.Description + '</td></tr>' +
                    '<tr><td style="vertical-align:top; border:none;"><strong>Status:</strong></td><td style="padding-left:10px;padding-bottom:10px; border:none;">' + (data.Status ? data.Status.Name : '-') + '</td></tr>' +
                    '<tr><td style="vertical-align:top; border:none;"><strong>Attorney:</strong></td><td style="padding-left:10px;padding-bottom:10px; border:none;">' + (data.Attorney ? data.Attorney.Name : '-') + '</td></tr>' +
                    '<tr><td style="vertical-align:top; border:none;"><strong>Opposing Attorney:</strong></td><td style="padding-left:10px;padding-bottom:10px; border:none;">' + (data.OppAttorney ? data.OppAttorney.Name : '-') + '</td></tr>' +
                    '<tr><td style="vertical-align:top; border:none;"><strong>Plaintiff:</strong></td><td style="padding-left:10px;padding-bottom:10px; border:none;">' + (data.Plaintiff ? data.Plaintiff : '-') + '</td></tr>' +
                    '<tr><td style="vertical-align:top; border:none;"><strong>Defendant:</strong></td><td style="padding-left:10px;padding-bottom:10px; border:none;">' + (data.Defendant ? data.Defendant : '-') + '</td></tr>' +
                    '<tr><td style="vertical-align:top; border:none;"><strong>Category:</strong></td><td style="padding-left:10px;padding-bottom:10px; border:none;">' + (data.Timeslot.Category ? data.Timeslot.Category.Description : '-') + '</td></tr>' +
                    '<tr><td style="vertical-align:top; border:none;"><strong>Actions:</strong></td><td style="padding-left:10px;padding-bottom:10px; border:none;">' +
                    '<a href="<%= EventListUrl %>/eid=' + data.Id + '" class="btn btn-sm btn-link"><i class="la la-edit"></i> Edit</a>' +
                    '<a href="javascript:void(0)" onclick="cancelEntry(' + data.Id + ')" class="btn btn-sm btn-link" data-button-type="delete"><i class="la la-trash"></i> Cancel</a>' +
                    '<a href="<%=TimeSlotListUrl %>/tid=' + data.Id + '" class="btn btn-sm btn-link"><i class="la la-history"></i> Revisions</a>' +
                    '</td></tr>' +
                    '</tbody>';
                $("#caseDetailsTable").empty().append(caseDetails);
                $("#caseSearchModal").modal('show');
            },
            error: function () {
                alert("No Event found with this case number!");
            }
        });
        return false;
    }

    function cancelEntry(eventId) {
        service.baseUrl = service.framework.getServiceRoot(service.path);
        var cancelUrl = `${service.baseUrl}EventAPI/CancelEvent`;

        Swal.fire({
            title: 'Are you sure?',
            text: "You won't be able to revert this!",
            icon: 'warning',
            input: 'textarea',
            inputLabel: 'Cancellation Reason',
            inputPlaceholder: 'Type your message here...',
            inputAttributes: { 'aria-label': 'Type your message here' },
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Yes, cancel it!',
            preConfirm: (value) => {
                if (!value) {
                    Swal.showValidationMessage('<i class="fa fa-info-circle"></i> Cancellation reason is required!');
                }
            }
        }).then((result) => {
            if (result.isConfirmed) {
                $.ajax({
                    url: cancelUrl,
                    method: 'POST',
                    contentType: 'application/json; charset=utf-8',
                    data: JSON.stringify({ eventId: eventId, reason: result.value }),
                    dataType: 'json',
                    success: function () {
                        Swal.fire('Cancelled!', 'Your hearing has now been cancelled.', 'success').then(() => {
                            window.location.href = '/';
                        });
                    },
                    error: function () {
                        Swal.fire({ icon: 'error', title: 'Oops...', text: 'Something went wrong!' });
                    }
                });
            }
        });
    }
</script>
