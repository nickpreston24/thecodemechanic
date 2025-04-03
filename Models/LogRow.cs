using System.Data.SqlClient;

namespace railway;

public class LogRow
{
    public int id { get; set; } = -1;

    public string exception_text { get; set; } = string.Empty;
}