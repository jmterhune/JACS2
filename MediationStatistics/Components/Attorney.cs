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
using System.IO;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Caching;

namespace tjc.Modules.MediationStatistics.Components
{
    [TableName("tjc_med_attorneys")]
    //setup the primary key for table
    [PrimaryKey("AttorneyId", AutoIncrement = true)]
    //configure caching using PetaPoco
    [Cacheable("Attorneys", CacheItemPriority.Default, 20)]
    internal class Attorney : EntityBase
    {
        public int AttorneyId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Firm { get; set; }

        public string Phone { get; set; }

        public string Extension { get; set; }

        public string Address { get; set; }
        public string Email { get; set; }
        public string City { get; set; }

        public string State { get; set; }

        public string Zip { get; set; }

        [IgnoreColumn]
        public string FullName
        {
            get
            {
                if (string.IsNullOrEmpty(FirstName) & string.IsNullOrEmpty(LastName)) { return ""; }
                if (string.IsNullOrEmpty(FirstName) & !string.IsNullOrEmpty(LastName))
                    return LastName;
                if (!string.IsNullOrEmpty(FirstName) & string.IsNullOrEmpty(LastName))
                    return FirstName;

                return string.Format("{1}, {0}", FirstName, LastName);
            }
        }
        [IgnoreColumn]
        public string FormattedPhone { get { return FormatPhone(); } }

        #region Methods
        private string FormatPhone()
        {
            string tempPhone = "";
            string phoneFormatted = "";
            string phoneUrl = "<a class=\"{3}\" data-original-title=\"{2}\" data-plugin-tooltip=\"tooltip\" href=\"tel:{0}\">{1}</a>";
            string tempPhoneExtention = "";
            if (!string.IsNullOrEmpty(Phone))
            {
                if (!string.IsNullOrEmpty(Extension))
                {
                    tempPhone = Regex.Replace(Phone, @"(\d{3})(\d{3})(\d{4})", "($1) $2-$3") + " x" + Extension;
                    tempPhoneExtention = string.Format("{0},{1}", Helper.CleanPhone(Phone), Helper.CleanPhone(Extension));
                }
                else
                {
                    tempPhone = Regex.Replace(Phone, @"(\d{3})(\d{3})(\d{4})", "($1) $2-$3");
                    tempPhoneExtention = Helper.CleanPhone(Phone);
                }
                phoneFormatted = string.Format(phoneUrl, tempPhoneExtention, tempPhone, "Office Phone", "phone");
            }

            return phoneFormatted;
        }
        #endregion

    }
}
