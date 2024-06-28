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
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Web.Caching;
using System.Web.Services.Protocols;
using tjc.Intranet.API.Services.ViewModels.Mediation;

namespace tjc.Intranet.API.Components.Mediation
{
    [TableName("tjc_med_cases")]
    public class CaseListItem : EntityBase
    {
        public string Region { get; set; }
        public string Group { get; set; }
        public string PartyOne { get; set; }
        public string PartyTwo { get; set; }
        public string ListNumber { get; set; }
        public int CaseId { get; set; }
        public int RegionId { get; set; }
        public int GroupId { get; set; }
        public string CaseNumber { get; set; }
        public string Comment { get; set; }
        public string CDSPNumber { get; set; }
        public string p1_FirstName { get; set; }
        public string p1_LastName { get; set; }
        public string p1_business { get; set; }
        public string p2_FirstName { get; set; }
        public string p2_LastName { get; set; }
        public string p2_business { get; set; }
        [IgnoreColumn]
        public string GroupTypeName { get {
                return ((GroupType)GroupId).ToString();
            } }
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
        [IgnoreColumn]
        public string FormattedComments { get { return Comment.Replace("|", Environment.NewLine); } }
    }
    public enum GroupType
    {
        CDSP = 1,
        CountyClaims = 2,
        Dependency = 3,
        Family = 4,
        FamilyPreFile = 5,
        Juvenile = 6,
        SmallClaims = 7
    }
}
