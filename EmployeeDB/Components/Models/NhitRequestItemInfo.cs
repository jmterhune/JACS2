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
        public int NhitRequestItemId { get; set; }  // int
        public int NhitRequestId { get; set; }  // int
        public int NhitItemId { get; set; }  // int
        public string ItemSnapshotName { get; set; }  // varchar(200)
        public string ItemSnapshotCategory { get; set; }  // varchar(20)
        public bool IsChecked { get; set; }  // bit
    }
}
