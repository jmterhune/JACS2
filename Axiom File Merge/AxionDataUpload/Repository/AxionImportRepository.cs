using AxionDataUpload.Data;
using Dapper;
using System.Data;

namespace AxionDataUpload.Repository
{

    public class AxiomExportRepository : IEmployeeExportRepository<AxiomExport>
    {
        private readonly IDbConnection _dbConnection;

        public AxiomExportRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public void ClearExports()
        {
            _dbConnection.Execute("DELETE FROM tjc_axiom_export_employees");
        }

        public void InsertEmployees(IEnumerable<AxiomExport> employees)
        {
            _dbConnection.Execute(
                "INSERT INTO tjc_axiom_export_employees (SSN, EmployeeType, LastName, FirstName, MiddleInitial,EmployeeID, " +
                "Position, SeparationDate, ClassCode, Classification, RestrictedPosition, " +
                "RestrictedEmployee, RestrictedRelative, Active, County) " +
                "VALUES ( @SSN, @EmployeeType, @LastName, @FirstName, @MiddleInitial, @EmployeeID, " +
                "@Position, @SeparationDate, @ClassCode, @Classification, @RestrictedPosition, " +
                "@RestrictedEmployee, @RestrictedRelative, @Active, @County)",
                employees);
        }
        public void InsertEmployee(AxiomExport employee)
        {
            _dbConnection.Execute(
                "INSERT INTO tjc_axiom_export_employees ( SSN, EmployeeType, LastName, FirstName, MiddleInitial,EmployeeID, " +
                "Position, SeparationDate, ClassCode, Classification, RestrictedPosition, " +
                "RestrictedEmployee, RestrictedRelative, Active, County) " +
                "VALUES ( @SSN, @EmployeeType, @LastName, @FirstName, @MiddleInitial, @EmployeeID, " +
                "@Position, @SeparationDate, @ClassCode, @Classification, @RestrictedPosition, " +
                "@RestrictedEmployee, @RestrictedRelative, @Active, @County)",
                employee);
        }
        public List<AxiomExport> RetrieveDataFromDatabase()
        {
                return _dbConnection.Query<AxiomExport>("SELECT * FROM tjc_axiom_export_employees").ToList();
        }
        public List<AxiomExport> RetrievEmployeesFromDatabase()
        {
            return _dbConnection.Query<AxiomExport>("SELECT * FROM emp_axiom_export").ToList();
        }
    }
}
