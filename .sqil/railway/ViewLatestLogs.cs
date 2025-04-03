using System.Data;
using System.Data.SqlClient;
using CodeMechanic.MySql;
using Dapper;

namespace railway;

public static partial class Procs
{
    public static class ViewLatestLogs
    {
        public static List<SqlParameter> Parameters { get; set; } = new();

        public static async Task<List<LogRow>> QueryAsync()
        {
            using var connection = SQLConnections.CreateConnection();

            var results = (
                await connection.QueryAsync<LogRow>(
                    nameof(ViewLatestLogs),
                    commandType: CommandType.StoredProcedure
                )
            ).ToList();

            return results;
        }
    }
}