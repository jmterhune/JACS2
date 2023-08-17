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
    [TableName("tjc_med_groups")]
    //setup the primary key for table
    [PrimaryKey("GroupId", AutoIncrement = true)]
    //configure caching using PetaPoco
    [Cacheable("MediationGroups", CacheItemPriority.Default, 20)]
    internal class Group : EntityBase
    {
        public int GroupId { get; set; }

        public string Description { get; set; }

        public bool? CourtOrdered { get; set; }
        [IgnoreColumn]
        public GroupType GroupEnum
        {
            get
            {
                return (GroupType)GroupId;
            }
        }
    }
    public enum GroupType
    {
        CDSP = 1,
        CountyClaims = 2,
        Dependency = 3,
        Family = 4,
        FamilyPreFile = 5,
        Juvenile = 6,
        SmallClaims = 7
    }
}
