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
using System.Web.Caching;

namespace tjc.Modules.MediationStatistics.Components
{
    [TableName("tjc_med_casetypes")]
    //setup the primary key for table
    [PrimaryKey("CaseTypeId", AutoIncrement = true)]
    //configure caching using PetaPoco
    [Cacheable("CaseTypes", CacheItemPriority.Default, 20)]
    internal class CaseType: EntityBase
    {
        public int CaseTypeId { get; set; }  // int (identity PK)

        public string Description { get; set; }  // nvarchar(50)

        public bool Active { get; set; }  // bit
    }

    [TableName("tjc_med_casetype_by_group")]
    internal class CaseTypeGroup : EntityBase
    {
        public int GroupId { get; set; }  // int

        public int CaseTypeId { get; set; }  // int

        public int? SortOrder { get; set; }  // int
    }

    internal class CaseTypeListItem : CaseType
    {
        public int SortOrder { get; set; }  // int

    }
}
