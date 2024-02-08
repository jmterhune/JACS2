using AxionDataUpload.Data;
using Renci.SshNet;
using System;
using System.Data;
using System.Data.SqlClient;

namespace AxionDataUpload.Repository
{
    public class UnitOfWork : IDisposable
    {
        private readonly IDbConnection _dbConnection;
        private readonly AppSettings _appSettings = new();
        private string _connectionString = Helper.Helper.GetConnectionStringByName("Intranet");
        public AxiomExportRepository AxiomExportRepository { get; }
        public ErrorLogRepository ErrorLogRepository { get; }

        public UnitOfWork(string connectionString)
        {
            _dbConnection = new SqlConnection(connectionString);
            _dbConnection.Open();

            AxiomExportRepository = new AxiomExportRepository(_dbConnection);
            ErrorLogRepository = new ErrorLogRepository(_dbConnection);
        }
        public string DownloadFiles()
        {
            using var unitOfWork = new UnitOfWork(_connectionString);
            string peopleFirstFileName = string.Empty;
            try
            {
                string sftpHost = _appSettings.DlFtpHost;
                string sftpUsername = _appSettings.DlFtpUsername;
                string sftpPassword = _appSettings.DlFtpPassword;
                string peopleFirstDirectory = _appSettings.PeopleFirstDirectory;
                string localDirectory = _appSettings.LocalDirectoryPath;

                DirectoryInfo dir=new DirectoryInfo(localDirectory);
                if(!dir.Exists ) {
                    dir.Create();
                }

                using (var client = new SftpClient(sftpHost, sftpUsername, sftpPassword))
                {
                    client.Connect();

                    // Get the list of files in the remote directory
                    var files = client.ListDirectory(peopleFirstDirectory)
                        .Where(f => !f.IsDirectory)
                        .OrderByDescending(f => f.LastWriteTime)
                        .ToList();

                    if (files.Count > 0)
                    {
                        var newestFile = files.First();
                        using (var fileStream = File.Create(Path.Combine(localDirectory, newestFile.Name)))
                        {
                            client.DownloadFile(newestFile.FullName, fileStream);
                        }
                        peopleFirstFileName = newestFile.Name;
                        peopleFirstFileName= Path.Combine(localDirectory, peopleFirstFileName);
                        ErrorLogRepository.CreateEvent(Helper.Helper.PopulateEventLog("People First File Downloaded", $"Downloaded the newest file: {newestFile.Name}"));
                    }
                    else
                    {
                        ErrorLogRepository.CreateEvent(Helper.Helper.PopulateEventLog("People First File Not Find", "No files found in the remote directory."));
                    }
                    client.Disconnect();
                }
                ErrorLogRepository.CreateEvent(Helper.Helper.PopulateEventLog("File Downloads Complete", "All Downloads Complete"));
            }
            catch (Exception ex)
            {
                ErrorLogRepository.CreateEvent(Helper.Helper.PopulateErrorLog(ex));
            }
            return peopleFirstFileName;
        }
        public void ImportEmployees()
        {
            try
            {
                List<AxiomExport> employees=   AxiomExportRepository.RetrievEmployeesFromDatabase();
                foreach (AxiomExport employee in employees)
                {
                    employee.EmployeeID = string.Format("C{0}", employee.EmployeeID);
                    AxiomExportRepository.InsertEmployee(employee);
                }
            }
            catch (Exception ex)
            {
                ErrorLogRepository.CreateEvent(Helper.Helper.PopulateErrorLog(ex));
            }
        }
        public void ImportPeopleSoft(string peopleFirstFileName)
        {
            try
            {
                var peopleSoftList = Helper.Helper.ReadPipeDelimitedFile(peopleFirstFileName);
                string ssn = "";
                foreach (var person in peopleSoftList)
                {
                    if (person.SSN != ssn)
                    {
                        ssn = person.SSN;
                        AxiomExportRepository.InsertEmployee(person);
                    }
                }
                ErrorLogRepository.CreateEvent(new EventLog { EventDate = DateTime.Now, EventDescription = "People Soft Import Completed", EventName = "Import PeopleSoft", Source = "ImportPeopleSoft" });
            }
            catch (Exception ex)
            {
                ErrorLogRepository.CreateEvent(Helper.Helper.PopulateErrorLog(ex));
            }
        }
        public void GeneratePipeDelimitedFile(string extension)
        {
            using var unitOfWork = new UnitOfWork(_connectionString);
            try
            {

                List<AxiomExport> data = unitOfWork.AxiomExportRepository.RetrieveDataFromDatabase();
                string filename = string.Format("{0}.{1}", _appSettings.ExportFileName.Replace("[DATE]", DateTime.Now.ToString("yyyyMMdd")), extension);
                string filePath = Path.Combine(_appSettings.LocalDirectoryPath, filename);
                if (data.Count > 0)
                {
                    using (StreamWriter writer = new(filePath))
                    {
                        foreach (var record in data)
                        {
                            string line = $"Update|{record.SSN}|{record.EmployeeType}|{record.LastName}|{record.FirstName}|" +
                                $"{record.MiddleInitial}|{record.EmployeeID}|{record.Position}|{record.SeparationDate}|" +
                                $"{record.ClassCode}|{record.Classification}|{record.RestrictedPosition}|{record.RestrictedEmployee}|" +
                                $"{record.RestrictedRelative}|{record.Active}|{record.County}";

                            writer.WriteLine(line);
                        }

                    }
                    ErrorLogRepository.CreateEvent(new EventLog { EventDate = DateTime.Now, EventDescription = $"Pipe-delimited file generated: {_appSettings.ExportFileName}", EventName = "Export File Generated", Source = "GeneratePipeDelimitedFile" });
                    UploadFile(filePath, _appSettings.UlFtpDirectory, filename);
                }
                else
                {
                    ErrorLogRepository.CreateEvent(new EventLog { EventDate = DateTime.Now, EventDescription = "No data available to generate the file.", EventName = "No Records Found", Source = "GeneratePipeDelimitedFile" });
                }
            }
            catch (Exception ex)
            {
                ErrorLogRepository.CreateEvent(Helper.Helper.PopulateErrorLog(ex));
            }
        }

        public void UploadFile(string localFilePath, string remoteDirectory, string remoteFileName)
        {
            try
            {
                using (var client = new SftpClient(_appSettings.UlFtpHost, _appSettings.UlFtpUsername, _appSettings.UlFtpPassword))
                {
                    client.Connect();

                    using (var fileStream = new FileStream(localFilePath, FileMode.Open))
                    {
                        client.UploadFile(fileStream, $"{remoteDirectory}{remoteFileName}", true);
                    }
                    client.Disconnect();
                }
                ErrorLogRepository.CreateEvent(new EventLog { EventDate = DateTime.Now, EventDescription = $"File {remoteFileName} uploaded to {remoteDirectory} on SFTP server.", EventName = "File Uploaded", Source = "UploadFile" });

                Console.WriteLine();
            }
            catch (Exception ex)
            {
                ErrorLogRepository.CreateEvent(Helper.Helper.PopulateErrorLog(ex));
            }
        }

        public void Dispose()
        {
            if (_dbConnection != null && _dbConnection.State == ConnectionState.Open)
            {
                _dbConnection.Close();
                _dbConnection.Dispose();
            }
        }
    }

}
