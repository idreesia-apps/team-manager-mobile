using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using Mono.Data.Sqlite;

namespace TeamManager
{
	public class DatabaseManager
	{
		private string GetConnectionString(string databaseFilePath)
		{
			var connectionString = string.Format("Data Source={0};Version=3;", databaseFilePath);
			return connectionString;
		}

		public async Task CreateDatabase(string databaseFilePath, bool refreshDatabase = true) {

			if (refreshDatabase == true)
				this.RemoveDatabaseFile(databaseFilePath);
			
			// Ensure that the database file is created. Get the database version from it.
			int databaseVersion = await this.CreateDatabaseFile(databaseFilePath);
			Console.WriteLine(string.Format("Database is currently at version {0}.", databaseVersion));

			// Check if there are scripts that need to be run based on the current database version
			List<DatabaseQuery> queries = this.getDatabaseCreationScripts(databaseVersion);

			if (queries.Count > 0)
			{
				try
				{
					Console.WriteLine("Running update scripts on the database.");
					// Get the connection string for the database
					var connectionString = this.GetConnectionString(databaseFilePath);
					using (var conn = new SqliteConnection((connectionString)))
					{
						await conn.OpenAsync();
						using (SqliteTransaction transaction = conn.BeginTransaction())
						{
							foreach (DatabaseQuery query in queries)
							{
								using (var command = query.GetCommand(conn))
								{
									command.ExecuteNonQuery();
								}
							}
							transaction.Commit();
							Console.WriteLine("Update scripts completed successfully.");
						}
					}
				}
				catch (Exception ex)
				{
					var reason = string.Format("Update scripts could not be run - reason = {0}", ex.Message);
					Console.WriteLine(reason);
				}
			}
			else
			{
				Console.WriteLine("Database is up to date. No scripts need to be run.");
			}
		}

		// **************************************************************************************************
		// Helper/Utility Methods
		// **************************************************************************************************
		private void RemoveDatabaseFile(string databaseFilePath)
		{
			Console.WriteLine("Removing the database file.");
			// Check if the file exists before attempting to remove it 
			if (File.Exists(databaseFilePath))
				File.Delete(databaseFilePath);
		}

		private async Task<int> CreateDatabaseFile(string databaseFilePath) {

			// Check if the file already exists 
			if (!File.Exists(databaseFilePath))
			{
				Console.WriteLine("Database file wasn't found. Creating a new database file.");
				// Create it if it does not
				SqliteConnection.CreateFile(databaseFilePath);
				// Since we have just created the database file, return 0 to indicate that there
				// is nothing in there, and we need to create the database tables from scratch.
				return 0;
			}
		
			// Get the connection string for the database
			var connectionString = this.GetConnectionString(databaseFilePath);

			try
			{
				using (var conn = new SqliteConnection((connectionString)))
				{
					await conn.OpenAsync();
					using (var command = conn.CreateCommand())
					{
						command.CommandText = "Select ConstantValue FROM GlobalConstants WHERE ConstantName = 'DatabaseVersion'";
						command.CommandType = CommandType.Text;
						int version = (int)await command.ExecuteScalarAsync();
						return version;
					}
				}
			}
			catch (Exception ex)
			{
				var reason = string.Format("Could not read the database version - reason = {0}", ex.Message);
				Console.WriteLine(reason);
				return 0;
			}		
		}

		private List<DatabaseQuery> getDatabaseCreationScripts(int databaseVersion) {

			List<DatabaseQuery> queryList = new List<DatabaseQuery>();
			if (databaseVersion < 1)
			{

				queryList.Add(
					new DatabaseQuery
					{
						query = @"CREATE TABLE IF NOT EXISTS 'GlobalConstants' (
									'ConstantName' VARCHAR, 
									'ConstantValue' VARCHAR
								)"
					}
				);

				queryList.Add(
					new DatabaseQuery
					{
						query = @"CREATE TABLE IF NOT EXISTS 'Teams' (
									'TeamId' VARCHAR PRIMARY KEY, 
									'TeamName' VARCHAR NOT NULL UNIQUE
								)"
					}
				);

				queryList.Add(
					new DatabaseQuery
					{
						query = @"CREATE TABLE IF NOT EXISTS 'Members' (
									'MemberId' VARCHAR PRIMARY KEY, 
									'MemberName' VARCHAR NOT NULL UNIQUE
								)"
					}
				);

				queryList.Add(
					new DatabaseQuery
					{
						query = @"CREATE TABLE IF NOT EXISTS 'TeamMembers' (
									'MemberId' VARCHAR NOT NULL, 
									'TeamId' VARCHAR NOT NULL, 
									'Role' VARCHAR NOT NULL,
									PRIMARY KEY (MemberId, TeamId)
								)"
					}
				);

				queryList.Add(
					new DatabaseQuery
					{
						query = @"CREATE TABLE IF NOT EXISTS 'TeamDuties' (
									'DutyId' VARCHAR PRIMARY KEY, 
									'TeamId' VARCHAR, 
									'DutyName' VARCHAR NOT NULL
								)"
					}
				);

				queryList.Add(
					new DatabaseQuery
					{
						query = @"CREATE TABLE IF NOT EXISTS 'MemberDuties' (
									'DutyId' VARCHAR NOT NULL, 
									'MemberId' VARCHAR NOT NULL, 
									'StartTime' DATETIME,
									'EndTime' DATETIME,
									PRIMARY KEY (DutyId, MemberId)
								)"
					}
				);

				queryList.Add(
					new DatabaseQuery
					{
						query = @"INSERT INTO 'GlobalConstants' VALUES (@1,@2)",
						arguments = new object[] { "DatabaseVersion", 1 }
					}
				);
			}

			return queryList;
		}
	}
}
