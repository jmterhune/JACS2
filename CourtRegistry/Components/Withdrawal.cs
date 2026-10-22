/*
' Copyright (c) 2026 Joe Terhune
'  All rights reserved.
'
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
' DEALINGS IN THE SOFTWARE.
'
*/

using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.ComponentModel;

namespace tjc.Modules.CourtRegistry.Components
{
    /// <summary>
    /// An attorney's request to come off the court appointed registry, raised from
    /// the public site. Approving it archives their application rather than deleting
    /// anything: the application stays as the record of what they were approved for.
    /// </summary>
    [TableName("tjc_car_withdrawal_requests")]
    [PrimaryKey("WithdrawalId", AutoIncrement = true)]
    internal class WithdrawalRequest
    {
        public int WithdrawalId { get; set; }
        public int AttorneyId { get; set; }
        public int? ApplicationId { get; set; }
        public int Year { get; set; }
        public int Status { get; set; }
        public string Reason { get; set; }
        public string Signature { get; set; }
        public bool Acknowledged { get; set; }
        public DateTime DateRequested { get; set; }
        public DateTime? DateCancelled { get; set; }
        public DateTime? DateReviewed { get; set; }
        public int? ReviewedByUserId { get; set; }
        public string ReviewNote { get; set; }
        public DateTime? EffectiveDate { get; set; }
    }

    /// <summary>A request with the attorney's details, for the review list.</summary>
    public class WithdrawalListItem
    {
        public int WithdrawalId { get; set; }
        public int AttorneyId { get; set; }
        public int BarNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public int? ApplicationId { get; set; }
        public int Year { get; set; }
        public int Status { get; set; }
        public string Reason { get; set; }
        public string Signature { get; set; }
        public bool Acknowledged { get; set; }
        public DateTime DateRequested { get; set; }
        public DateTime? DateReviewed { get; set; }
        public string ReviewNote { get; set; }
        public DateTime? EffectiveDate { get; set; }
        /// <summary>Status of the application the request was made against.</summary>
        public int? ApplicationStatus { get; set; }

        public string AttorneyName { get { return LastName + ", " + FirstName; } }

        public string StatusName
        {
            get { return Enumerations.GetEnumDescription((WithdrawalStatus)Status); }
        }

        public string StatusCss
        {
            get
            {
                switch ((WithdrawalStatus)Status)
                {
                    case WithdrawalStatus.Pending: return "badge bg-warning text-dark";
                    case WithdrawalStatus.Approved: return "badge bg-success";
                    case WithdrawalStatus.Denied: return "badge bg-danger";
                    default: return "badge bg-secondary";
                }
            }
        }

        public string PeriodYear
        {
            get { return Year > 0 ? string.Format("{0} - {1}", Year - 1, Year) : "No application"; }
        }

        public string RequestedDisplay { get { return DateRequested.ToShortDateString(); } }

        public string ReviewedDisplay
        {
            get { return DateReviewed.HasValue ? DateReviewed.Value.ToShortDateString() : string.Empty; }
        }

        public bool IsPending { get { return (WithdrawalStatus)Status == WithdrawalStatus.Pending; } }

        /// <summary>Link to the application this request was made against, blank when there is none.</summary>
        public string ApplicationLabel
        {
            get { return ApplicationId.HasValue && ApplicationId.Value > 0 ? "#" + ApplicationId.Value : "None"; }
        }
    }

    /// <summary>
    /// The wording court administration requires whenever removal from the registry
    /// is described. It matches the public site's request form word for word.
    /// </summary>
    internal static class WithdrawalText
    {
        public const string Caveat =
            "Removal from the registry applies to future appointments only. You are required to " +
            "continue representation on every case you are currently appointed to until you file a " +
            "motion to withdraw in that case, set the motion for hearing, and the motion is granted " +
            "by the court. A case is not closed out by this request.";
    }

    public enum WithdrawalStatus
    {
        [Description("Pending")]
        Pending = 0,
        [Description("Approved")]
        Approved = 1,
        [Description("Denied")]
        Denied = 2,
        [Description("Cancelled by attorney")]
        Cancelled = 3
    }
}
