using Microsoft.Data.SqlClient;

namespace CourtCounsel.Desktop.Data;

public static class SqlConnectionFactory
{
    public static SqlConnection Create()
    {
        var settings = AppSettingsService.Current;
        if (string.IsNullOrWhiteSpace(settings.ConnectionString))
            throw new InvalidOperationException(
                "No database connection string is configured. Open Settings and enter one.");
        return new SqlConnection(settings.ConnectionString);
    }
}
