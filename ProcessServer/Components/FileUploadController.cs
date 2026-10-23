using DotNetNuke.Abstractions.Application;
using DotNetNuke.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace tjc.Modules.ProcessServer.Components
{
    class FileUploadController
    {
        // The attachment table lives in the jud12 (public) site's database.
        private const string CONN_JUD12 = "Jud12";

        private readonly IHostSettings _hostSettings;

        public FileUploadController(IHostSettings hostSettings)
        {
            _hostSettings = hostSettings;
        }

        public void CreateFileUpload(UploadedFile t)
        {
            using (IDataContext ctx = DataContext.Instance(_hostSettings, CONN_JUD12))
            {
                var rep = ctx.GetRepository<UploadedFile>();
                rep.Insert(t);
            }
        }

        public void DeleteFileUpload(int FileId)
        {
            var t = GetFileUpload(FileId);
            DeleteFileUpload(t);
        }

        public void DeleteFileUpload(UploadedFile t)
        {
            using (IDataContext ctx = DataContext.Instance(_hostSettings, CONN_JUD12))
            {
                var rep = ctx.GetRepository<UploadedFile>();
                rep.Delete(t);
            }
        }

        // Explicit column list for list/count reads so the FileData (varbinary(max)) bytes are
        // never selected. Loading them for every row is what exhausts memory now that files live
        // in the database; only GetFileUpload(int), used to serve a single file, reads FileData.
        private const string ListColumns =
            "AttachmentID, ProcessServerID, DateUploaded, Hidden, FullPath";
        private const string Table = "tjc_process_server_attachments";

        public IEnumerable<UploadedFile> GetFileUploads()
        {
            using (IDataContext ctx = DataContext.Instance(_hostSettings, CONN_JUD12))
            {
                return ctx.ExecuteQuery<UploadedFile>(CommandType.Text,
                    "SELECT " + ListColumns + " FROM " + Table).ToList();
            }
        }
        public int GetFileUploadCount(int processServerId)
        {
            using (IDataContext ctx = DataContext.Instance(_hostSettings, CONN_JUD12))
            {
                return ctx.ExecuteScalar<int>(CommandType.Text,
                    "SELECT COUNT(*) FROM " + Table + " WHERE ProcessServerID = @0",
                    processServerId);
            }
        }
        public IEnumerable<UploadedFile> GetFileUploadsByProcessServerId(int processServerId)
        {
            using (IDataContext ctx = DataContext.Instance(_hostSettings, CONN_JUD12))
            {
                return ctx.ExecuteQuery<UploadedFile>(CommandType.Text,
                    "SELECT " + ListColumns + " FROM " + Table + " WHERE ProcessServerID = @0",
                    processServerId).ToList();
            }
        }
        public IEnumerable<UploadedFile> GetFileUploadsByProcessServerIdYear(int processServerId, int year)
        {
            using (IDataContext ctx = DataContext.Instance(_hostSettings, CONN_JUD12))
            {
                return ctx.ExecuteQuery<UploadedFile>(CommandType.Text,
                    "SELECT " + ListColumns + " FROM " + Table + " WHERE ProcessServerID = @0 AND YEAR(DateUploaded) = @1",
                    processServerId, year).ToList();
            }
        }


        public UploadedFile GetFileUpload(int FileId)
        {
            UploadedFile t;
            using (IDataContext ctx = DataContext.Instance(_hostSettings, CONN_JUD12))
            {
                var rep = ctx.GetRepository<UploadedFile>();
                t = rep.GetById(FileId);
            }
            return t;
        }

        public void UpdateFileUpload(UploadedFile t)
        {
            using (IDataContext ctx = DataContext.Instance(_hostSettings, CONN_JUD12))
            {
                var rep = ctx.GetRepository<UploadedFile>();
                rep.Update(t);
            }
        }
    }
}