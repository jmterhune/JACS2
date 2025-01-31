/*
' Copyright (c) 2020 12th Judicial Circuit Court
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
using System.Collections.Generic;
using System.Linq;

namespace tjc.Modules.CourtReporting.Components
{
    [TableName("tjc_dcr_requests")]
    //setup the primary key for table
    [PrimaryKey("RequestID", AutoIncrement = true)]
    //configure caching using PetaPoco

   internal class Request : RequestBase
    {
        [IgnoreColumn]
        public List<Proceeding> Proceedings { get; set; }
        public Request()
        {
            Proceedings = new List<Proceeding>();
        }

        [IgnoreColumn]
        public decimal GrandTotal
        {
            get
            {
                return Proceedings.Sum(x => x.SubTotal);
            }
        }
    }
    public class RequestInfo : RequestBase
    {
        public int Stage { get; set; }

        public int ProceedingCount { get; set; }

    }
    public class RequestBase
    {
        public int RequestID { get; set; }

        public string CaseNumber { get; set; }

        public string CaseName { get; set; }

        public string Judge { get; set; }

        public string Involvement { get; set; }

        public string Jurisdiction { get; set; }
        public string Location { get; set; }

        public string Instructions { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Address { get; set; }
        public string Address2 { get; set; }


        public string City { get; set; }

        public string State { get; set; }

        public string Zip { get; set; }

        public string Phone { get; set; }

        public string Fax { get; set; }

        public string Email { get; set; }

        public string DeliveryMethod { get; set; }

        public string TranscriptionList { get; set; }

        public string LawFirm { get; set; }

        public bool PaymentRequired { get; set; }

        public DateTime RequestedDate { get; set; }

        public PaymentType PaymentType { get; set; }

        public int UserId { get; set; }

        public OrderStatus OrderStatus { get; set; }

        public bool CA { get; set; }

        public bool IsInquiry { get; set; }

        public Guid Guid { get; set; }

        public decimal TotalAmount { get; set; }

        public bool Deposit { get; set; }

        public decimal DepositAmount { get; set; }

        public DateTime? PaymentDate { get; set; }

        [IgnoreColumn]
        public string FullAddress
        {
            get
            {

                return string.Format("{0}{1}<br />{2}, {3} {4}", Address, Address2 != null && Address2.Length > 0 ? "<br />" + Address2 : "", City, State, Zip);
            }
        }

    }
    public enum PaymentType
    {
        [System.ComponentModel.Description("Credit Card/Debit Card/e-Check")]
        card = 1,
        [System.ComponentModel.Description("Check")]
        check = 2,
        [System.ComponentModel.Description("Money Order")]
        moneyOrder = 3
    }

    public enum OrderStatus
    {
        submitted = 0,
        reviewed = 1,
        paid = 2,
        mediaCreated = 3,
        notified = 4,
        completed = 5,
        paymentRejected = 6,
        cancelled = 7,
        resubmitted = 8,
        repopened = 9
    }
}
