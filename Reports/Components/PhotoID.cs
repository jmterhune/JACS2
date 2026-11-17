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

namespace tjc.Modules.Reports.Components
{
    [TableName("PhotoID")]
    //setup the primary key for table
    [PrimaryKey("ID", AutoIncrement = true)]
    // Deliberately not [Cacheable]. This table lives in the DataCard database, which
    // the badge system writes outside DNN, and the public site's Process Server reads
    // it as well. Nothing here can invalidate a cache when a badge changes elsewhere,
    // and twenty minutes of staleness buys very little on a reporting screen.
    internal class PhotoID
    {
        public int ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string BadgeType { get; set; }
        public string JobTitle { get; set; }
        public string IndexNumber { get; set; }
        public string Title { get; set; }
        public string IdNumber { get; set; }
        public string PetName { get; set; }
        public string TeamName { get; set; }
        public DateTime? HireDate { get; set; }
        public DateTime? IssueDate { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public byte[] Photo { get; set; }
        public byte[] Signature { get; set; }
        public byte[] PetImage { get; set; }
    }
}
