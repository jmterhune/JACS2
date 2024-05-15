using AxionDataUpload.Helper;
using AxionDataUpload.Repository;

namespace tjc.axion.console
{
    class Program
    {
        static void Main(string[] args)
        {
            // Assuming you've set up your connection string in the App.config file
            string extension = "partial";
            if (args.Length != 0)
            {
                extension = "full";
            }
            string connectionString = Helper.GetConnectionStringByName("Intranet");
            using var unitOfWork = new UnitOfWork(connectionString);
            try
            {
                unitOfWork.AxiomExportRepository.ClearExports();
                unitOfWork.DeleteExistingFiles();
                string peopleFirstFileName = unitOfWork.DownloadFiles();
                unitOfWork.ImportPeopleSoft(peopleFirstFileName); 
                unitOfWork.ImportEmployees();
                unitOfWork.GeneratePipeDelimitedFile(extension);
                unitOfWork.ErrorLogRepository.CreateEvent(Helper.PopulateEventLog("Main Program Execution Successful", "Task Complete"));

            }
            catch (Exception ex)
            {
                unitOfWork.ErrorLogRepository.CreateEvent(Helper.PopulateErrorLog(ex));
            }
        }
    }
}
