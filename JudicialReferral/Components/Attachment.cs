using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.Web.Caching;



namespace tjc.Modules.JudicialReferral.Components
{
    [TableName("tjc_judicial_referral_attachments")]
    //setup the primary key for table
    [PrimaryKey("AttachmentID", AutoIncrement = true)]
    //configure caching using PetaPoco
    [Cacheable("Referrals", CacheItemPriority.Default, 20)]
    //scope the objects to the ModuleId of a module on a page (or copy of a module on a page)
    internal class Attachment
    {
        public int AttachmentID { get; set; }
        public int FileID { get; set; }
        public string Path { get; set; }
        public int ReferralID { get; set; }
        public string FileName { get; set; }
    }
}