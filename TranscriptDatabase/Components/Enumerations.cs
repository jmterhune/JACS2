using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
namespace tjc.Modules.TranscriptDatabase.Components
{
    public static class Enumerations
    {
       
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
        public static IEnumerable<T> GetValues<T>()
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }
    }
    public enum EmployeeTypes
    {
        [Description("Judge")]
        Judge = 0,
        [Description("Court Reporter")]
        CourtReporter = 1,
        [Description("Scopist")]
        Scopist = 2,
        [Description("Transcriptionist")]
        Transcriptionist = 3,
        [Description("Staff")]
        Staff = 4
    }
    public enum DeliveryTypes
    {
        [Description("Interoffice")]
        Interoffice = 0,
        [Description("U.S. Postage")]
        UsPostage = 1,
        [Description("Email")]
        Email = 2
    }
    public enum DocumentTypes
    {
        [Description("Acknowledgment Fee or Deposit Waived")]
        FeeDepositWaived = 0,
        [Description("Acknowledgment Private Paying")]
        PrivatePaying = 1,
        [Description("Acknowledgment Private Paid")]
        PrivatePaid = 2,
        [Description("Extension Request")]
        ExtensionRequest = 3,
        [Description("Extension Request No Deposit")]
        ExtensionRequestNoDeposit = 4
    }
    public enum EventTypes
    {
        [Description("First Extension")]
        firstExtension = 2,
        [Description("Second Extension")]
        secondExtension = 3,
        [Description("Third Extension")]
        thirdExtension = 4,
        [Description("Due Date")]
        dueDate = 1,
        [Description("Other")]
        Other = 6,
        [Description("Transcript Filed")]
        transcriptFiled = 5
    }

}