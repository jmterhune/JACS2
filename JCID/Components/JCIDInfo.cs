/*
' Copyright (c) 2017 12th Judicial Circuit
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
using System.Web.Caching;

namespace tjc.Modules.JCID.Components
{
    [TableName("tjc_jcid_data")]
    //setup the primary key for table
    [PrimaryKey("RequestID", AutoIncrement = true)]
    //configure caching using PetaPoco
    [Cacheable("Requests", CacheItemPriority.Default, 20)]
    class JCIDInfo
    {
        public bool Waiver { get; set; }
        public int RequestID { get; set; }

        public string DateSigned { get; set; }

        public string Signature { get; set; }

        public string PickupLocation { get; set; }

        public string Gender { get; set; }

        public string Race { get; set; }

        public string SSN { get; set; }
        public string BirthDate { get; set; }

        public string StateBarNumber { get; set; }

        public string Email { get; set; }

        public string BusinessPhone { get; set; }

        public string Zip { get; set; }

        public string State { get; set; }

        public string City { get; set; }

        public string MailingAddress { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [IgnoreColumn]
        public string FullName { get { return FirstName + " " + LastName; } }

        public string PhotoType { get; set; }
        public bool Rejected { get; set; }
        public string RejectReason { get; set; }
        public string CompletedBy { get; set; }

        public DateTime? CompletedDate { get; set; }

        public DateTime? PaymentDate { get; set; }

        public string PhotoUrl { get; set; }
        public DateTime? CriminalHistoryCompleted { get; set; }
        public DateTime? CardCreated { get; set; }
        public DateTime? ApplicantNotified { get; set; }
        public bool IsReplacement { get; set; }
    }
}
