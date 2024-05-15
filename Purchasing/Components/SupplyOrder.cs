using System;
using DotNetNuke.ComponentModel.DataAnnotations;

namespace tjc.Modules.Purchasing.Components
{
    [TableName("tjc_purchasing_supply_orders")]
    //setup the primary key for table
    [PrimaryKey("OrderID", AutoIncrement = true)]

    internal class SupplyOrder
    {
        public int OrderID { get; set; }
        public string Location { get; set; }
        public string RequestedName { get; set; }
        public DateTime DateRequested { get; set; }
        public DateTime? CompletedDate { get; set; }

        //public List<Components.SupplyOrderItem> OrderLines
        //{
        //    get
        //    {
        //        return GetLines();
        //    }
        //}

        //public List<Components.SupplyOrderItem> GetLines()
        //{
        //    var ctl = new Components.Controller();
        //    return ctl.GetSupplyOrderItems(_orderId).ToList();
        //}
    }
}