using System.Reflection;
using System;
using System.Text.RegularExpressions;
using System.ComponentModel;
using System.Web.UI.WebControls;
using System.Web;

namespace tjc.Modules.MediationStatistics.Components
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

        public static string GetCaseFormatted(string year, string code, string number, string suffix)
        {
            string returnString = "";
            if (year.Length == 2)
                year = "20" + year;
            if (number != "")
                number = number.PadLeft(6, '0');
            returnString = year + " " + code.ToUpper() + " " + number + " " + suffix.ToUpper();
            return returnString.Trim();
        }

        public static string GetCDSPFormatted(string type, string year, string number, string location)
        {
            string returnString = "";
            if (type != "")
                returnString += type + "-";
            if (year != "")
                returnString += year + "-";
            if (number != "")
                returnString += number + "-";
            if (location != "")
                returnString += location;

            return returnString.Trim('-').Trim();
        }
        public static string GetDescription(Enum en)
        {
            Type type = en.GetType();

            MemberInfo[] memInfo = type.GetMember(en.ToString());

            if (memInfo != null && memInfo.Length > 0)
            {
                object[] attrs = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

                if (attrs != null && attrs.Length > 0)
                    return ((DescriptionAttribute)attrs[0]).Description;
            }

            return en.ToString();
        }

        public static Enum GetEnumValueFromDescription(Type MyType, string Description)
        {
            Enum retEnumValue = null;

            foreach (Enum e in Enum.GetValues(MyType))
            {
                string sValue = GetDescription((Enum)e);
                if (sValue.ToLower() == Description.ToLower())
                {
                    retEnumValue = e;
                    break;
                }
            }

            return retEnumValue;
        }
        public static void MoveSelectedItemUp(this ListBox listBox)
        {
            MoveSelectedItem(listBox, -1);
        }

        public static void MoveSelectedItemDown(this ListBox listBox)
        {
            MoveSelectedItem(listBox, 1);
        }

        static void MoveSelectedItem(ListBox listBox, int direction)
        {
            // Checking selected item
            if (listBox.SelectedItem == null || listBox.SelectedIndex < 0)
                return; // No selected item - nothing to do

            // Calculate new index using move direction
            int newIndex = listBox.SelectedIndex + direction;

            // Checking bounds of the range
            if (newIndex < 0 || newIndex >= listBox.Items.Count)
                return; // Index out of range - nothing to do

            ListItem selected = listBox.SelectedItem;

            // Save checked state if it is applicable

            // Removing removable element
            listBox.Items.Remove(selected);
            // Insert it in new position
            listBox.Items.Insert(newIndex, selected);
            // Restore selection
            listBox.SelectedIndex = newIndex;

        }
        public static string GetCookieValue(HttpRequest request, string cookieName, string cookieItem)
        {
            var cookieValue = string.Empty;

            HttpCookie cookie = request.Cookies[cookieName];
            if (cookie == null) return string.Empty;
            if (!string.IsNullOrEmpty(cookieItem))
            {
                cookieValue = cookie[cookieItem].ToString();
            }
            else
            {
                cookieValue = cookie.Value.ToString();
            }
            return cookieValue;
        }
        public static void SetCookieValue(HttpResponse response, HttpRequest request, string cookieName, string cookieItem, string cookieItemValue, int expirationDays)
        {
            HttpCookie cookie = new HttpCookie(cookieName);
            bool cookieExists = false;
            if (request.Cookies[cookieName] != null)
            {
                cookie = request.Cookies[cookieName];
                cookieExists = true;
            }
            cookie.Values.Add(cookieItem, cookieItemValue);
            if (cookieExists)
            {
                response.Cookies.Set(cookie);
            }
            else
            {
                response.Cookies.Add(cookie);
            }
            cookie.Expires = DateTime.Today.AddDays(expirationDays);
        }

    }


}