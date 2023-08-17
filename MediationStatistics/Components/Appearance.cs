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
    [TableName("tjc_med_appearances")]
    //setup the primary key for table
    [PrimaryKey("AppearanceId", AutoIncrement = true)]
    //configure caching using PetaPoco
    [Cacheable("Appearances", CacheItemPriority.Default, 20)]
    internal class Appearance : EntityBase
    {
        public int AppearanceId { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; }
    }

    [TableName("tjc_med_appearance_by_group")]
    internal class AppearanceGroup : EntityBase
    {
        public int AppearanceId { get; set; }
        public int GroupId { get; set; }
        public int? SortOrder { get; set; }
    }
    internal class AppearanceListItem : Appearance
    {
        public int SortOrder { get; set; }

    }
}
