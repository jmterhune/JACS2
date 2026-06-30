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
        public int AttorneyID { get; set; }  // int

        public string LastName { get; set; }  // nvarchar(50)

        public string FirstName { get; set; }  // nvarchar(50)

        public string MiddleName { get; set; }  // nvarchar(50)

        public int OfficeID { get; set; }  // int

        public string Address1 { get; set; }  // nvarchar(150)

        public string Address2 { get; set; }  // nvarchar(150)

        public string City { get; set; }  // nvarchar(50)

        public string State { get; set; }  // nvarchar(50)

        public string ZipCode { get; set; }  // nvarchar(10)
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
        public int AttorneyID { get; set; }  // int
        public int DesignationID { get; set; }  // int
        [IgnoreColumn]
        public string AttorneyName { get; set; }

    }
}
