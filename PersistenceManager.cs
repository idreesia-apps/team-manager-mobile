using System;
using System.IO;
using System.Threading.Tasks;

namespace TeamManager
{
	public class PersistenceManager
	{
		private static PersistenceManager _instance;
		public static async Task<PersistenceManager> getInstance() 
		{
			if (PersistenceManager._instance == null)
			{
				PersistenceManager._instance = new PersistenceManager();
				await PersistenceManager._instance.initialize();
			}

			return PersistenceManager._instance;
		}

		private String DatabasePath
		{
			get
			{
				var sqliteFilename = "TeamManager.db3";
				#if __ANDROID__
					// Just use whatever directory SpecialFolder.Personal returns
					string libraryPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
				#else
					// we need to put in /Library/ on iOS5.1 to meet Apple's iCloud terms
					// (they don't want non-user-generated data in Documents)
					string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal); // Documents folder
					string libraryPath = Path.Combine(documentsPath, "..", "Library"); // Library folder instead
				#endif

				var path = Path.Combine(libraryPath, sqliteFilename);
				return path;
			}
		}

		private PersistenceManager() { }

		private async Task initialize()
		{
			var databaseManager = new DatabaseManager();
			await databaseManager.CreateDatabase(this.DatabasePath);
		}
	}
}
