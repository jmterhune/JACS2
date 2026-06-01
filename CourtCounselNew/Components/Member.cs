using DotNetNuke.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations;
using System.Web.Caching;

namespace tjc.Modules.CourtCounsel.Components
{
    [TableName("court_counsel_members")]
    //setup the primary key for table
    [PrimaryKey("MemberId", AutoIncrement = true)]
    //configure caching using PetaPoco
    [Cacheable("Members", CacheItemPriority.Default, 20)]
    internal class Member : EntityBase
    {
        public int MemberId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public int UserId { get; set; }
        public bool Active { get; set; }
        public virtual int MemberTypeId { get; set; }
        [IgnoreColumn]
        [EnumDataType(typeof(MemberTypes))]
        public MemberTypes MemberType
        {
            get
            {
                return (MemberTypes)this.MemberTypeId;
            }
            set
            {
                this.MemberTypeId = (int)value;
            }
        }
        [IgnoreColumn]
        public string ListName { get { return string.Format("{0}, {1}", LastName, FirstName); } }
        [IgnoreColumn]
        public string FullName { get { return string.Format("{0} {1}", FirstName, LastName); } }
    }
    public enum MemberTypes
    {
        judge,
        attorney
    }
}
