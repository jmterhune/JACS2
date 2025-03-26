/*
' Copyright (c) 2023 Joe Terhune
'  All rights reserved.
' 
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
' DEALINGS IN THE SOFTWARE.
' 
*/

using DotNetNuke.Common.Utilities;
using DotNetNuke.ComponentModel.DataAnnotations;
using DotNetNuke.Entities.Content;
using System;
using System.ComponentModel;
using System.Reflection;
using System.Web.Caching;

namespace tjc.Modules.PretrialServices.Sarasota.Components
{
    [TableName("tjc_pts_sarasota_defendants_in_program")]
    //setup the primary key for table
    [PrimaryKey("ItemId", AutoIncrement = true)]
    //configure caching using PetaPoco
    [Cacheable("SarasotaDefendantInProgramItems", CacheItemPriority.Default, 20)]
    //scope the objects to the ModuleId of a module on a page (or copy of a module on a page)
    internal class DefendantInProgram : EntityBase
    {
        public long ItemId
        {
            get; set;
        }
        public DateTime? IntakeDate
        {
            get; set;
        }
        public DateTime? FtaDate
        {
            get; set;
        }
        public DateTime? CompletionDate
        {
            get; set;
        }
        public string DefendantName
        {
            get; set;
        }
        public string CaseNumber { get; set; }

        public string ArrestCharges { get; set; }

        public string NonCompArrestViolation { get; set; }
        public int? Completion
        {
            get; set;
        }


        public int FcDangerous
        {
            get; set;
        }

        public int FcNonDangerous
        {
            get; set;
        }

        public int McDangerous
        {
            get; set;
        }
        public int McNonDangerous
        {
            get; set;
        }        
        public int CourtAppearances
        {
            get; set;
        }
        public int DaysSpr
        {
            get; set;
        }

        public bool Indigent
        {
            get; set;
        }        
        public bool FtaArrestHearing
        {
            get; set;
        }

        public bool BwOrdered
        {
            get; set;
        }

        public bool IsRevoked { get; set; }
        public bool BondPaid { get; set; }
        public string MostSeriousOffense { get; set; }


        [IgnoreColumn]
        public string FormattedIntakeDate
        {
            get
            {
                if (IntakeDate.HasValue)
                {
                    return IntakeDate.Value.ToShortDateString();
                }
                return "";
            }
        }
        [IgnoreColumn]
        public string FormattedFTADate
        {
            get
            {
                if (FtaDate.HasValue)
                {
                    return FtaDate.Value.ToShortDateString();
                }
                return "";
            }
        }
        [IgnoreColumn]
        public string FormattedCompletionDate
        {
            get
            {
                if (CompletionDate.HasValue)
                {
                    return CompletionDate.Value.ToShortDateString();
                }
                return "";
            }
        }
        [IgnoreColumn]
        public string FormattedCompletion
        {
            get
            {
                if (Completion == 1) { return "Successful"; } else if (Completion == 0) { return "Non-Successful"; } else if (Completion == 2) { return "Other"; }
                return "";
            }
        }
        [IgnoreColumn]
        public string FormattedIndigent
        {
            get
            {
                if (Indigent == true) { return "Yes"; } else if (Indigent == false) { return "No"; }
                return "";
            }
        }
        [IgnoreColumn]
        public string FormattedFtaArrest
        {
            get
            {
                if (FtaArrestHearing == true) { return "Yes"; } else if (FtaArrestHearing == false) { return "No"; }
                return "";
            }
        }
       
        [IgnoreColumn]
        public string FormattedBwOrdered
        {
            get
            {
                if (BwOrdered == true) { return "Yes"; } else if (BwOrdered == false) { return "No"; }
                return "";
            }
        }
        [IgnoreColumn]
        public string FormattedBondPaid
        {
            get
            {
                if (BondPaid == true) { return "Yes"; } else if (BondPaid == false) { return "No"; }
                return "";
            }
        }
      
        [IgnoreColumn]
        public int MonthsSPR
        {
            get
            {
                int months = 0;
                if (CompletionDate.HasValue)
                    months = (CompletionDate.Value.Year - IntakeDate.Value.Year) * 12 + (CompletionDate.Value.Month - IntakeDate.Value.Month);
                return months;
            }
        }
    }
}
