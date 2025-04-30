/*
' Copyright (c) 2025 Joe Terhune
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

namespace tjc.Modules.CourtRegistry.Components
{
    [TableName("tjc_car_applications")]
    //setup the primary key for table
    [PrimaryKey("ApplicationID", AutoIncrement = true)]
    //configure caching using PetaPoco
    internal class Application
    {
        public int ApplicationID { get; set; }
        public int AttorneyID { get; set; }
        public string RemoteContactInfo { get; set; }
        public string RejectionText { get; set; }
        public string CertSignature { get; set; }
        public string GuardianSignature { get; set; }
        public int Status { get; set; }
        public int Year { get; set; }
        public int YearsOnRegistry { get; set; }
        public int CreatedByUserId { get; set; }
        public int LastModifiedByUserId { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateReviewed { get; set; }
        public DateTime? DateOfPeriod { get; set; }
        public DateTime? ExportDate { get; set; }
        public bool IsRenewal { get; set; }
        public bool Exported { get; set; }

    }
    [TableName("tjc_car_application_list")]
    public class ApplicationListItem
    {
        public int ApplicationID { get; set; }
        public int AttorneyID { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }

        public string GuardianSignature { get; set; }
        public int Status { get; set; }
        public int Year { get; set; }
        public int YearsOnRegistry { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateReviewed { get; set; }
        public bool IsRenewal { get; set; }
        public bool IsGuardian { get; set; }
        public string StatusName { get{
                
                return Enumerations.GetEnumDescription((ApplicationStatus)Status);
            } }
    }
    [TableName("tjc_car_current_periods")]
    //setup the primary key for table
    [PrimaryKey("ApplicationYear", AutoIncrement = false)]
    //configure caching using PetaPoco
    internal class ApplicationPeriod
    {
        public int ApplicationYear { get; set; }
        public DateTime? ModificationDeadline { get; set; }
        public bool AcceptingNewApplications { get; set; }
        [IgnoreColumn]
        public string PeriodYear { get{ return string.Format("{0} - {1}", ApplicationYear-1, ApplicationYear); } }

    }
}
