using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.Web.Caching;
namespace tjc.Modules.TranscriptDatabase.Components
{
    [TableName("tjc_rec_attachment")]
    [PrimaryKey("AttachmentID", AutoIncrement = true)]
    [Cacheable("Attachments", CacheItemPriority.Default, 20)]
    internal class Attachment : EntityBase
    {
        public int AttachmentID { get; set; }  // int
        public int DesignationID { get; set; }  // int
        public int FileID { get; set; }  // int
        public string Path { get; set; }  // nvarchar(2000)
        public string FileDescription { get; set; }  // nvarchar(150)
        [IgnoreColumn]
        public string RelativePath
        {
            get
            {
                if (FileID > 0)
                {
                    var file = DotNetNuke.Services.FileSystem.FileManager.Instance.GetFile(FileID);
                    if (file != null)
                    {
                        return string.Format("/portals/0/{0}", file.RelativePath);
                    }
                }
                return string.Format("/portals/0/{0}", Path);
            }
        }
    }
}
