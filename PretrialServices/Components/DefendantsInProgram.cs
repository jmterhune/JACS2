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

namespace tjc.Modules.PretrialServices.Components
{
    [TableName("tjc_pts_defendants_in_program")]
    //setup the primary key for table
    [PrimaryKey("ItemId", AutoIncrement = true)]
    //configure caching using PetaPoco
    [Cacheable("DefendantInProgramItems", CacheItemPriority.Default, 20)]
    //scope the objects to the ModuleId of a module on a page (or copy of a module on a page)
    internal class DefendantInProgram : EntityBase
    {
        public long ItemId // bigint
        {
            get; set;
        }
        public DateTime? IntakeDate // smalldatetime
        {
            get; set;
        }
        public DateTime? FtaDate // smalldatetime
        {
            get; set;
        }
        public DateTime? CompletionDate // smalldatetime
        {
            get; set;
        }
        public string DefendantName // nvarchar(100)
        {
            get; set;
        }
        public string CaseNumber { get; set; } // nvarchar(200)

        public string ArrestCharges { get; set; } // nvarchar(max)

        public string NonCompArrestViolation { get; set; } // nvarchar(50)
        public bool Indigent // bit
        {
            get; set;
        }
        public bool BwOrdered // bit
        {
            get; set;
        }
        public int? Completion // int
        {
            get; set;
        }

        public bool FtaArrestHearing // bit
        {
            get; set;
        }

        public int FcDangerous // int
        {
            get; set;
        }

        public int FcNonDangerous // int
        {
            get; set;
        }

        public int McDangerous // int
        {
            get; set;
        }
        public int McNonDangerous // int
        {
            get; set;
        }
        public int CourtAppearances // int
        {
            get; set;
        }
        public int DaysSpr // int (DaysSPR)
        {
            get; set;
        }
        public int CountyId // int
        {
            get; set;
        }

        public bool IsRevoked { get; set; } // bit
        public bool CaseScreened { get; set; } // bit
        public bool PlacedInProgram { get; set; } // bit
        public int? BondType { get; set; } // int
        public int? NonCompliance { get; set; } // int
        public int? CaseType { get; set; } // int
        public bool Interviewed { get; set; } // bit
        public bool Assessed { get; set; } // bit
        public bool PtrRecommended { get; set; } // bit
        public bool PtrOrdered { get; set; } // bit
        public bool IndigentAssessed { get; set; } // bit
        public bool PtrNotRecommended { get; set; } // bit
        public string MostSeriousOffense { get; set; } // nvarchar(50)



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
        public string FormattedCaseScreened
        {
            get
            {
                if (CaseScreened == true) { return "Screened"; } else if (CaseScreened == false) { return "Not Screened"; }
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
        public string FormattedPlacedInProgram
        {
            get
            {
                if (PlacedInProgram == true) { return "Placed"; } else if (PlacedInProgram == false) { return "Not Placed"; }
                return "";
            }
        }
        [IgnoreColumn]
        public string FormattedCaseType
        {
            get
            {
                if (CaseType == (int)Enumerations.CaseCategoryValue.Felony) { return "CF Case"; } else if (CaseType == (int)Enumerations.CaseCategoryValue.Misdemeanor) { return "MM Case"; }
                return "";
            }
        }
        [IgnoreColumn]
        public string FormattedBondType
        {
            get
            {
                if (BondType == (int)Enumerations.BondTypeValue.Secured) { return "Secured"; } else if (BondType == (int)Enumerations.BondTypeValue.NonSecured) { return "Non-Secured"; }
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
        [IgnoreColumn]
        public string FormattedNonCompliance
        {
            get
            {
                string returnValue = "";
                switch ((Enumerations.ComplianceStatus)NonCompliance)
                {
                    case Enumerations.ComplianceStatus.FTA:
                        returnValue = Enumerations.GetEnumDescription(Enumerations.ComplianceStatus.FTA);
                        break;
                    case Enumerations.ComplianceStatus.WarrantIssuedFTA:
                        returnValue = Enumerations.GetEnumDescription(Enumerations.ComplianceStatus.WarrantIssuedFTA);
                        break;
                    case Enumerations.ComplianceStatus.ReleaseRevokedFTA:
                        returnValue = Enumerations.GetEnumDescription(Enumerations.ComplianceStatus.ReleaseRevokedFTA);
                        break;
                    case Enumerations.ComplianceStatus.NewArrest:
                        returnValue = Enumerations.GetEnumDescription(Enumerations.ComplianceStatus.NewArrest);
                        break;
                    case Enumerations.ComplianceStatus.ReleaseRevokedArrest:
                        returnValue = Enumerations.GetEnumDescription(Enumerations.ComplianceStatus.ReleaseRevokedArrest);
                        break;
                    case Enumerations.ComplianceStatus.SprNonCompliant:
                        returnValue = Enumerations.GetEnumDescription(Enumerations.ComplianceStatus.SprNonCompliant);
                        break;
                    case Enumerations.ComplianceStatus.WarrantIssuedNonCompliant:
                        returnValue = Enumerations.GetEnumDescription(Enumerations.ComplianceStatus.WarrantIssuedNonCompliant);
                        break;
                    default:
                        break;
                }
                return returnValue;
            }
        }
    }
}
