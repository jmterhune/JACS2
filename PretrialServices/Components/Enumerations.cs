using DotNetNuke.Common.Utilities;
using DotNetNuke.ComponentModel.DataAnnotations;
using DotNetNuke.Entities.Content;
using System;
using System.ComponentModel;
using System.Reflection;
using System.Web.Caching;


namespace tjc.Modules.PretrialServices.Components
{
    public class Enumerations
    {
        public enum BondTypeValue
        {
            NonSecured = 0,
            Secured = 1
        }
        public enum CaseCategoryValue
        {
            Felony = 0, Misdemeanor = 1
        }
        public enum CompletionStatus
        {
            unsuccessful = 0,
            successful = 1,
            other = 2
        }
        public enum ComplianceStatus
        {
            [Description("Failure to Appear")]
            FTA = 0,
            [Description("Warrants Issued for Failure to Appear")]
            WarrantIssuedFTA = 1,
            [Description("Release Revoked due to Failure to Appear")]
            ReleaseRevokedFTA = 2,
            [Description("Arrested for New Offense")]
            NewArrest = 3,
            [Description("Release Revoked due to New Offense")]
            ReleaseRevokedArrest = 4,
            [Description("Non-Compliant with Supervised Release Conditions")]
            SprNonCompliant = 5,
            [Description("Warrant Issued for Non-Compliance with Supervised Release Conditions")]
            WarrantIssuedNonCompliant = 6
        }
        public enum SearchType
        {
            date = 0,
            defendantName = 1,
            caseNumber = 2
        }
        public static string GetEnumDescription(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());
            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (attributes.Length > 0)
            {
                return attributes[0].Description;
            }
            else
            {
                return value.ToString();
            }
        }
    }
}