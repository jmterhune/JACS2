/*
' Copyright (c) 2025 Joe Terhune
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
using System.Web.Caching;

namespace tjc.Modules.TranscriptDatabase.Components
{
    [TableName("tjc_rec_attorney")]
    [PrimaryKey("AttorneyID", AutoIncrement = true)]
    [Cacheable("Attorneys", CacheItemPriority.Default, 20)]
    public class Attorney : EntityBase
    {
        public int AttorneyID { get; set; }

        public string LastName { get; set; }

        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public int OfficeID { get; set; }

        public string Address1 { get; set; }

        public string Address2 { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string ZipCode { get; set; }
        [IgnoreColumn]
        public string OfficeName
        {
            get
            {
                var ctl = new OfficeController(); Office office = ctl.GetOffice(OfficeID);
                if (office != null)
                    return office.Description;
                return "";
            }
        }
        [IgnoreColumn]
        public string ListName
        {
            get
            {
                return string.Format("{0}, {1}", LastName, FirstName);
            }
        }
        [IgnoreColumn]
        public string AttorneyName
        {
            get
            {
                return string.Format("{0} {1}",FirstName, LastName );
            }
        }
    }
    [TableName("tjc_rec_designation_attorneys")]
    public class DesignationAttorney
    {
        public int AttorneyID { get; set; }
        public int DesignationID { get; set; }
        [IgnoreColumn]
        public string AttorneyName { get; set; }

    }
}
