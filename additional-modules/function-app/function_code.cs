#r "System.Configuration"
#r "System.Data"

using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Threading.Tasks;

public static async Task Run(TimerInfo myTimer, TraceWriter log)
{
	var connStr = ConfigurationManager.ConnectionStrings["sqldb"].ConnectionString;

	using (var conn = new SqlConnection(connStr))
	{
		conn.Open();
		var sqlCmd = "DELETE FROM TimelineItems WHERE deleted = 'True'";
		using (var cmd = new SqlCommand(sqlCmd, conn))
		{
			var rows = await cmd.ExecuteNonQueryAsync();
			log.Info($"{rows} rows deleted.");
		}
	}
}