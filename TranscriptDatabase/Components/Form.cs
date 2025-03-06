using DotNetNuke.ComponentModel.DataAnnotations;
using DotNetNuke.Services.FileSystem;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Web.Caching;
namespace tjc.Modules.TranscriptDatabase.Components
{
    [TableName("tjc_rec_form")]
    [PrimaryKey("FormID", AutoIncrement = true)]
    [Cacheable("Forms", CacheItemPriority.Default, 20)]
    internal class Form : EntityBase
    {
        public int FormID { get; set; }
        public int FileID { get; set; }
        [IgnoreColumn]
        public string FilePath
        {
            get
            {
                if (FileID > 0)
                {
                    var ctl = new FileManager();
                    var file = ctl.GetFile(FileID);
                    if (file != null)
                        return file.RelativePath;
                }
                return null;
            }
        }
        public int DocumentTypeID { get; set; }
        [IgnoreColumn]
        [EnumDataType(typeof(DocumentTypes))]
        public DocumentTypes DocumentType
        {
            get
            {
                return (DocumentTypes)this.DocumentTypeID;
            }
            set
            {
                this.DocumentTypeID = (int)value;
            }
        }
        [IgnoreColumn]
        public string FileName { get { return Path.GetFileName(FilePath); } }
        [IgnoreColumn]
        public string FormText
        {
            get
            {
                return Enumerations.GetEnumDescription(DocumentType);
            }
        }
    }

}
