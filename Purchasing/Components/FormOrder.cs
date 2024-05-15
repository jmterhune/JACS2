using System;
using DotNetNuke.ComponentModel.DataAnnotations;

namespace tjc.Modules.Purchasing.Components
{
    [TableName("tjc_purchasing_form_orders")]
    //setup the primary key for table
    [PrimaryKey("OrderID", AutoIncrement = true)]
    internal class FormOrder
    {
        public int OrderID{ get; set; }
        public string Location { get; set; }
        public string RequestedName { get; set; }
        public DateTime DateRequested { get; set; }
        public DateTime? CompletedDate { get; set; }


        //public List<Components.FormOrderItem> OrderLines
        //{
        //    get
        //    {
        //        return GetLines();
        //    }
        //}

        //public List<Components.FormOrderItem> GetLines()
        //{
        //    var ctl = new Components.Controller();
        //    return ctl.GetFormOrderItems(_orderId).ToList();
        //}
    }

}