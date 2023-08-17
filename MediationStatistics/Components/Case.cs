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
using System.Linq;
using System.Net.NetworkInformation;
using System.Web.Caching;

namespace tjc.Modules.MediationStatistics.Components
{
    [TableName("tjc_med_cases")]
    //setup the primary key for table
    [PrimaryKey("CaseId", AutoIncrement = true)]
    //configure caching using PetaPoco
    [Cacheable("Cases", CacheItemPriority.Default, 20)]
    internal class Case : EntityBase
    {
        public int CaseId { get; set; }

        public int? RegionId { get; set; }

        public int? GroupId { get; set; }

        public string CaseNumber { get; set; }

        public string CDSPNumber { get; set; }

        public string p1_FirstName { get; set; }

        public string p1_LastName { get; set; }

        public string p1_business { get; set; }

        public string p2_FirstName { get; set; }

        public string p2_LastName { get; set; }

        public string p2_business { get; set; }
        [IgnoreColumn]
        public IEnumerable<Session> CaseSessions
        {
            get
            {
                var ctl = new SessionController();
                return ctl.GetSessionsByCase(CaseId);
            }
        }
        [IgnoreColumn]
        public GroupType GroupEnum
        {
            get
            {
                return (GroupType)GroupId;
            }
        }
        public Session GetCurrentSession(int index)
        {
            int sessionCount = CaseSessions.Count();
            if (sessionCount > 0 && index< sessionCount)
            {
                return CaseSessions.ElementAt(index);
            }
            return new Session();
        }
        public string GetPartyFullName(string firstName, string lastName, string businessName)
        {
            string partyname = "";
            if (!string.IsNullOrEmpty(firstName) & !string.IsNullOrEmpty(lastName))
            { partyname = string.Format("{0}, {1}", lastName, firstName); }
            else if (!string.IsNullOrEmpty(businessName))
            {
                partyname = businessName;
            }
            else
            {
                partyname = string.Format("{0} {1}", firstName, lastName);
            }
            return partyname;
        }
    }
    [TableName("tjc_med_cases")]
    internal class CaseListItem : Case
    {
        public string Region { get; set; }
        public string Group { get; set; }
        public string PartyOne { get; set; }
        public string PartyTwo { get; set; }
        public string ListNumber { get; set; }
    }

}
