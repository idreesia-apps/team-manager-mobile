using System;
using System.Data;
using Mono.Data.Sqlite;

namespace TeamManager
{
	public class DatabaseQuery
	{
		public string name { get; set; }
		public string query { get; set; }
		public Object[] arguments { get; set; }

		public SqliteCommand GetCommand(SqliteConnection connection)
		{
			SqliteCommand command = connection.CreateCommand();
			command.CommandText = this.query;
			command.CommandType = CommandType.Text;
			if (this.arguments != null)
			{
				int paramCounter = 1;
				foreach (var argument in this.arguments)
				{
					var param = command.CreateParameter();
					param.ParameterName = paramCounter.ToString();
					param.Value = argument;
					command.Parameters.Add(param);
					paramCounter++;
				}
			}

			return command;
		}
	}
}
