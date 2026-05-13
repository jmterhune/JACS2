using DotNetNuke.ComponentModel.DataAnnotations;

namespace tjc.Modules.EmployeeDB.Components.Models
{
    /// <summary>
    /// One catalog item's checked state on a submitted request. Snapshotting
    /// the item's name + category at submission time means a later admin
    /// rename / re-category in tjc_nhit_item doesn't rewrite history.
    /// </summary>
    [TableName("tjc_nhit_request_item")]
    [PrimaryKey("NhitRequestItemId", AutoIncrement = true)]
    public class NhitRequestItemInfo
    {
        public int NhitRequestItemId { get; set; }
        public int NhitRequestId { get; set; }
        public int NhitItemId { get; set; }
        public string ItemSnapshotName { get; set; }
        public string ItemSnapshotCategory { get; set; }
        public bool IsChecked { get; set; }
    }
}
