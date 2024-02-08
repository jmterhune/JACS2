using AxionDataUpload.Data;
using AxionDataUpload.Repository;
using System.Configuration;
using System.Globalization;

namespace AxionDataUpload.Helper
{

    public static class Helper
    {
        public static EventLog PopulateEventLog(string description, string message) => new() { EventDate = DateTime.Now, EventType = EventType.Event, EventDescription = description, EventName = message, Source = "Process Event" };
        public static EventLog PopulateErrorLog(Exception ex) => new() { EventDate = DateTime.Now, EventType = EventType.Error, EventDescription = ex.StackTrace.ToString(), EventName = ex.Message, Source = "Program Error" };

        public static string GetConnectionStringByName(string name)
        {
            // Look for the name in the connectionStrings section.
            ConnectionStringSettings settings =
                ConfigurationManager.ConnectionStrings[name];

            // If found, return the connection string (otherwise return null)
            return settings.ConnectionString;
        }
        public static List<AxiomExport> ReadPipeDelimitedFile(string filePath)
        {
            string connectionString = GetConnectionStringByName("Intranet");

            using var unitOfWork = new UnitOfWork(connectionString);
            List<AxiomExport> data = new();

            try
            {
                using (StreamReader reader = new(filePath))
                {
                    if(!reader.EndOfStream)
                    {
                        reader.ReadLine();
                    }
                    while (!reader.EndOfStream)
                    {
                        
                        string line = reader.ReadLine();
                        string[] fields = line.Split('|');
                        AxiomExport record = new()
                        {
                            HireDate = FormattedDate(fields[7]),
                            EmployeeID = string.Format("S{0}",fields[1]),
                            SSN = fields[2],
                            EmployeeType = "State",
                            LastName = fields[4],
                            FirstName = fields[5],
                            MiddleInitial = fields[6],
                            Position = fields[3],
                            SeparationDate = FormattedDate(fields[8]),
                            ClassCode = fields[11],
                            Classification = fields[12],
                            RestrictedPosition = fields[18],
                            RestrictedEmployee = fields[15],
                            RestrictedRelative = fields[17],
                            Active = fields[13],
                            County = "",
                        };
                        data.Add(record);
                    }
                }
                return data.OrderBy(x=>x.SSN).ThenByDescending(x=>x.HireDate).ThenByDescending(x => x.SeparationDate).ToList();
            }
            catch (Exception ex)
            {
                unitOfWork.ErrorLogRepository.CreateEvent(Helper.PopulateErrorLog(ex));
                return new();
            }
        }
        private static string FormattedDate(string date)
        {
            string formattedDate = "";
            if (!string.IsNullOrEmpty(date))
            {
                string[] dateString = date.Split(new char[] { '-' });
                if (dateString.Length == 3)
                {
                    return string.Format("{0}/{1}/{2}", dateString[1], dateString[2], dateString[0]);
                }
            }
            return formattedDate;
        }
    }
}
