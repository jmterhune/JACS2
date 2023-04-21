using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace tjc.Modules.EmployeeDB.Components
{
    public static class Helper
    {
        private static Regex digitsOnly = new Regex(@"[^\d]");


        public static string CleanPhone(string phone)
        {
            return digitsOnly.Replace(phone, "");
        }
        public static string CleanDecimal(string input)
        {
            return Regex.Replace(input, "[^.0-9]", "");
        }
    }
}