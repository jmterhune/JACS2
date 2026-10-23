/*
' Copyright (c) 2021 Joe Terhune
'  All rights reserved.
' 
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
' DEALINGS IN THE SOFTWARE.
' 
*/

using DotNetNuke.Abstractions.Portals;
using DotNetNuke.ComponentModel.DataAnnotations;
using DotNetNuke.Entities.Portals;
using System;
using System.Web.Caching;

namespace tjc.Modules.ProcessServer.Components
{
    [TableName("tjc_process_servers")]
    //setup the primary key for table
    [PrimaryKey("ProcessServerID", AutoIncrement = true)]
    //configure caching using PetaPoco
    [Cacheable("ProcessServers", CacheItemPriority.Default, 20)]
    class ProcessServer
    {
        public int ProcessServerID { get; set; }
        public int UserID { get; set; }
        public int ApplicationYear { get; set; }
        public int ProcessServerNumber { get; set; }
        public int ProfileImageID { get; set; }
        public string FirstName { get; set; }
        public string MiddelInitial { get; set; }
        public string LastName { get; set; }
        public string MaidenName { get; set; }
       
        public string Aliases { get; set; }
        public string Email { get; set; }
        public string HomePhone { get; set; }
        public string CellPhone { get; set; }
        public string SocialSecurityNumber { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string BirthPlace { get; set; }
        public string StreetAddress { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string County { get; set; }
        public string HowLong { get; set; }
        public string Race { get; set; }
        public string Sex { get; set; }
        public string Height { get; set; }
        public string Weight { get; set; }
        public string EyeColor { get; set; }
        public string HairColor { get; set; }
        public string Question6Text { get; set; }
        public string Question7Text { get; set; }
        public string Quesion8Text { get; set; }
        public string Question11Text { get; set; }
        public bool Question1 { get; set; }
        public bool Question2 { get; set; }
        public bool Question3 { get; set; }
        public bool Question4 { get; set; }
        public bool Question5 { get; set; }
        public bool Question6 { get; set; }
        public bool Question7 { get; set; }
        public bool Question8 { get; set; }
        public bool Question9 { get; set; }
        public bool Question10 { get; set; }
        public bool Question11 { get; set; }
        public bool Acknowledged { get; set; }
        public bool Authorized { get; set; }
        public bool Policy { get; set; }
        public int Stage { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public DateTime? ApprovedByCourtCounsel { get; set; }
        public DateTime? ApprovedByChiefJudge { get; set; }
        public DateTime? CriminalHistoryCompleted { get; set; }
        public DateTime CreatedOnDate { get; set; }
        public DateTime LastModifiedOnDate { get; set; }
        public string RejectionReason { get; set; }
        public bool ShowPhone { get; set; }
        public bool IsRenewal { get; set; }
        public string Notes { get; set; }
        public string RejectionNotification { get; set; }
        public applicationStatus Status { get; set; }
        public bool NotificationSent { get; set; }
        public string PaymentType { get; set; }
        public DateTime? PaymentReceivedDate { get; set; }
        public bool Manatee { get; set; }
        public bool Sarasota { get; set; }
        public bool DeSoto { get; set; }

        [IgnoreColumn]
        public bool CCApproved { get { return ApprovedByCourtCounsel.HasValue; } }
        [IgnoreColumn]
        public bool CHCompleted { get { return CriminalHistoryCompleted.HasValue; } }
        [IgnoreColumn]
        public bool CJApproved { get { return ApprovedByChiefJudge.HasValue; } }
        [IgnoreColumn]
        public string PhotoUrl
        {
            get { return GetFileUrl(ProfileImageID); }

        }
        [IgnoreColumn]
        public bool ExpiredOrNoValue
        {
            get
            {
                if (ExpirationDate.HasValue)
                {
                    if (ExpirationDate.Value < DateTime.Today)
                        return true;
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
        [IgnoreColumn]
        public bool Active
        {
            get
            {
                if (CJApproved && CHCompleted && CCApproved && !ExpiredOrNoValue)
                    return true;
                return false;
            }
        }
        [IgnoreColumn]
        public string StatusText
        {
            get
            {
                return GetStatusText(Status);
            }
        }
        [IgnoreColumn]
        public string StatusTooltip
        {
            get
            {
                return GetStatusTooltip(Status);
            }
        }
        private string GetFileUrl(int fileId)
        {
            string url = "";
            var fileManager = DotNetNuke.Services.FileSystem.FileManager.Instance;
            DotNetNuke.Services.FileSystem.IFileInfo fi = fileManager.GetFile(Convert.ToInt32(fileId));
            IPortalSettings portalSettings = PortalController.Instance.GetCurrentSettings();
            try
            {
                url = "/Portals/0/Images/Judges/placeholder.jpg";
                if (fileId > 0)
                    url = fileManager.GetUrl(fi);
            }
            catch
            {
                url = "/Portals/0/Images/Judges/placeholder.jpg";
            }
            return url;
        }
        private string GetStatusText(applicationStatus status)
        {
            string statusText = "";
            switch (status)
            {
                case applicationStatus.New:
                    statusText = "N";
                    break;
                case applicationStatus.CriminalHistoryComplete:
                    statusText = "CH";
                    break;
                case applicationStatus.CourtCounselApproved:
                    statusText = "CCA";
                    break;
                case applicationStatus.ChiefJudgeApproved:
                    statusText = "CJA";
                    break;
                case applicationStatus.CourtCounselRejected:
                    statusText = "CCR";
                    break;
                case applicationStatus.ChiefJudgeRejected:
                    statusText = "CJR";
                    break;
                case applicationStatus.Rejected:
                    statusText = "R";
                    break;
                case applicationStatus.Approved:
                    statusText = "A";
                    break;
                default:
                    statusText = "N";
                    break;
            }
            return statusText;
        }
        private string GetStatusTooltip(applicationStatus status)
        {
            string statusText = "";
            switch (status)
            {
                case applicationStatus.New:
                    statusText = "New";
                    break;
                case applicationStatus.CriminalHistoryComplete:
                    statusText = "Criminal History Completed";
                    break;
                case applicationStatus.CourtCounselApproved:
                    statusText = "General Counsel Approval";
                    break;
                case applicationStatus.ChiefJudgeApproved:
                    statusText = "Chief Judge Approval";
                    break;
                case applicationStatus.CourtCounselRejected:
                    statusText = "General Counsel Rejected";
                    break;
                case applicationStatus.ChiefJudgeRejected:
                    statusText = "Chief Judge Rejected";
                    break;
                case applicationStatus.Rejected:
                    statusText = "Rejected";
                    break;
                case applicationStatus.Approved:
                    statusText = "Approved";
                    break;
                case applicationStatus.CompletedCC:
                    statusText = "Completed with Credit Card";
                    break;
                case applicationStatus.CompletedCheck:
                    statusText = "Completed with Check / Money Order";
                    break;
                default:
                    statusText = "New";
                    break;
            }
            return statusText;
        }
    }
    public enum applicationStatus
    {
        Rejected = -1,
        New = 0,
        CourtCounselRejected = -2,
        ChiefJudgeRejected = -3,
        CriminalHistoryComplete = 1,
        CourtCounselApproved = 2,
        ChiefJudgeApproved = 3,
        Approved = 4,
        CompletedCC = 5,
        CompletedCheck = 6,

    }
}
