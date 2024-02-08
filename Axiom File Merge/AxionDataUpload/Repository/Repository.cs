using AxionDataUpload.Data;

namespace AxionDataUpload.Repository
{
    public interface IEmployeeExportRepository<T>
    {
        void ClearExports();
        void InsertEmployees(IEnumerable<T> data);
        void InsertEmployee(T employee);
        List<AxiomExport> RetrieveDataFromDatabase();
        List<AxiomExport> RetrievEmployeesFromDatabase();
    }
    public interface IEventRepository<T>
    {
        void CreateEvent(T data);
    }
}
