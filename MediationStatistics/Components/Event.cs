/*
' Copyright (c) 2023 12th Judicial Circuit
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
using System.Web.Caching;

namespace tjc.Modules.MediationStatistics.Components
{
    [TableName("tjc_med_events")]
    //setup the primary key for table
    [PrimaryKey("EventId", AutoIncrement = true)]
    //configure caching using PetaPoco
    [Cacheable("Events", CacheItemPriority.Default, 20)]
    internal class Event : EntityBase
    {
        public int EventId { get; set; }  // int (identity PK)

        public int SessionId { get; set; }  // int

        public bool? MediationHeld { get; set; }  // bit

        public string ReasonNotHeld { get; set; }  // nvarchar(50)

        public DateTime? EventDate { get; set; }  // smalldatetime

        public string Comments { get; set; }  // nvarchar(2000)

        public string AgreementType { get; set; }  // char(1)
        public string MediatorType { get; set; }  // nvarchar(50)
        public int MediatorId { get; set; }  // int
        public bool? AgreementSubmittedParties { get; set; }  // bit

        public bool? AgreementPreparedAttorney { get; set; }  // bit

        public bool? AgreementSigned { get; set; }  // bit

        public bool? AdjournedTimeRemaining { get; set; }  // bit

        public decimal? TimeRemaining { get; set; }  // decimal(18,2)

        public int? SignedCount1 { get; set; }  // int

        public int? SignedCount2 { get; set; }  // int

        public int? SignedCount3 { get; set; }  // int

        public bool? Signed1 { get; set; }  // bit

        public bool? Signed2 { get; set; }  // bit

        public bool? Signed3 { get; set; }  // bit
        [IgnoreColumn]
        public IEnumerable<Appearance> EventAppearances
        {
            get
            {
                var ctl = new AppearanceController();
                return ctl.GetEventAppearances(EventId);
            }
        }
        [IgnoreColumn]
        public string MediatorName
        {
            get
            {
                string mediatorName = string.Empty;
                var ctl = new MediatorController();
                Mediator mediator = ctl.GetMediator(MediatorId);
                if (mediator != null)
                    return mediator.MediatorName;
                return mediatorName;
            }
        }
    }

    [TableName("tjc_med_event_appearances")]
    internal class EventAppearance : EntityBase
    {
        public int EventId { get; set; }  // int

        public int AppearanceId { get; set; }  // int
    }
}
