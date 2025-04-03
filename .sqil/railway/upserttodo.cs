using System.Data;
using System.Data.SqlClient;
using CodeMechanic.MySql;
using Dapper;
using justdoit;

namespace railway;

public static partial class Procs
{
    public static class upserttodo
    {
        public static List<SqlParameter> Parameters { get; set; } = new();

        public static async Task<int> UpsertAsync(Todo todo)
        {
            using var connection = SQLConnections.CreateConnection();

            var results = (
                await connection.ExecuteAsync(
                    nameof(upserttodo),
                    todo,
                    commandType: CommandType.StoredProcedure
                )
            );

            return results;
        }
    }
}