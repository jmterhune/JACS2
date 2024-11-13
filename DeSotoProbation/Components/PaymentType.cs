using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;

namespace tjc.Modules.DeSoto.Probation.Components
{
    [TableName("tjc_desoto_probation_payment_types")]
    //setup the primary key for table
    [PrimaryKey("PaymentTypeID", AutoIncrement = true)]
    //configure caching using PetaPoco
    [Cacheable("PaymentTypes", CacheItemPriority.Default, 20)]
    internal class PaymentType
    {

        public int PaymentTypeID { get; set; }

        public string Name { get; set; }


    }
    [TableName("tjc_desoto_probation_payment_type_xref")]
    internal class PaymentTypeXref
    {
        public int PaymentTypeID { get; set; }
        public int RecordID { get; set; }
    }
}