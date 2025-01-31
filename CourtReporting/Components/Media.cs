using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace tjc.Modules.CourtReporting.Components
{
    [TableName("tjc_dcr_media")]
    //setup the primary key for table
    [PrimaryKey("MediaId", AutoIncrement = true)]
    //configure caching using PetaPoco
    internal class Media
    {
        public int MediaID { get; set; }
        public string Description { get; set; }
        [Required]
        public virtual int MediaTypeID
        {
            get
            {
                return (int)this.MediaType;
            }
            set
            {
                MediaType = (MediaTypes)value;
            }
        }
        [IgnoreColumn]
        [EnumDataType(typeof(MediaTypes))]
        public MediaTypes MediaType { get; set; }

        public decimal Price { get; set; }

    }
    public enum MediaTypes
    {
        [System.ComponentModel.Description("Audio CD")]
        audioCD = 3,
        [System.ComponentModel.Description("PC CD")]
        pcCD = 2,
        [System.ComponentModel.Description("Transcript")]
        transcript = 4,
        [System.ComponentModel.Description("USB Flash Drive")]
        usb = 1,
        [System.ComponentModel.Description("MP3 Download")]
        mp3 = 5
    }
}