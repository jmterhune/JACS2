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
        public int AttachmentID { get; set; }
        public int DesignationID { get; set; }
        public int FileID { get; set; }
        public string Path { get; set; }
        public string FileDescription { get; set; }
        [IgnoreColumn]
        public string RelativePath
        {
            get
            {
                var file = DotNetNuke.Services.FileSystem.FileManager.Instance.GetFile(FileID);
                if (file != null)
                {
                    return file.RelativePath;
                }
                return Path;
            }
        }
    }
}
