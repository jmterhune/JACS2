using DotNetNuke.ComponentModel.DataAnnotations;

namespace tjc.Modules.EmployeeDB.Components.Models
{
    /// <summary>
    /// Junction row tying a profile to an item in the catalog with its
    /// checked state. Profiles only store rows for checked items — the
    /// absence of a row means "unchecked" — but the schema permits
    /// IsChecked = 0 if a future use case needs an explicit "off".
    /// </summary>
    [TableName("tjc_nhit_profile_item")]
    [PrimaryKey("NhitProfileItemId", AutoIncrement = true)]
    public class NhitProfileItemInfo
    {
        public int NhitProfileItemId { get; set; }  // int
        public int NhitProfileId { get; set; }  // int
        public int NhitItemId { get; set; }  // int
        public bool IsChecked { get; set; }  // bit
    }
}
