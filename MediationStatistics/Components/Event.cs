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
        public int EventId { get; set; }

        public int SessionId { get; set; }

        public bool? MediationHeld { get; set; }

        public string ReasonNotHeld { get; set; }

        public DateTime? EventDate { get; set; }

        public string Comments { get; set; }

        public string AgreementType { get; set; }
        public string MediatorType { get; set; }
        public int MediatorId { get; set; }
        public bool? AgreementSubmittedParties { get; set; }

        public bool? AgreementPreparedAttorney { get; set; }

        public bool? AgreementSigned { get; set; }

        public bool? AdjournedTimeRemaining { get; set; }

        public decimal? TimeRemaining { get; set; }

        public int? SignedCount1 { get; set; }

        public int? SignedCount2 { get; set; }

        public int? SignedCount3 { get; set; }

        public bool? Signed1 { get; set; }

        public bool? Signed2 { get; set; }

        public bool? Signed3 { get; set; }
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
        public int EventId { get; set; }

        public int AppearanceId { get; set; }
    }
}
