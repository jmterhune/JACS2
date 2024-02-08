using AxionDataUpload.Data;
using Dapper;
using System.Data;

namespace AxionDataUpload.Repository
{
    public class ErrorLogRepository : IEventRepository<EventLog>
    {
        private readonly IDbConnection _dbConnection;

        public ErrorLogRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public void CreateEvent(EventLog eventLog)
        {
            _dbConnection.Execute(
                "INSERT INTO tjc_axiom_event_log ( EventName, EventDescription, EventDate, Source, EventType) " +
                "VALUES (@EventName, @EventDescription, @EventDate, @Source, @EventType)",
                eventLog);
        }
    }
}
