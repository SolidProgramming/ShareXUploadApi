using MySql.Data.MySqlClient;
using System.Data;

namespace ShareXUploadApi.Services
{
    public interface IDBService
    {
        Task InsertFileDataAsync(FileModel file);
        Task UpdateFileDataAsync();
        Task DeleteFileDataAsync();
        Task GetFileDataAsync(string guid);

    }
    public class DBService : IDBService
    {
        private readonly DBSettingsModel? _DBSettings;
        private readonly ILogger<DBService> _Logger;
        private readonly MySqlConnection _MysqlConn = default!;

        public DBService(ILogger<DBService> logger, IConfiguration config, MySqlConnection mysqlConn)
        {
            _Logger = logger;

            _DBSettings = SettingsHandler.ReadSettings<DBSettingsModel>();

            if (_DBSettings is null)
            {
                _Logger.LogCritical("Couldn't load database settings");
                return;
            }

            if (string.IsNullOrEmpty(_DBSettings.IP) || string.IsNullOrEmpty(_DBSettings.Database) ||
                string.IsNullOrEmpty(_DBSettings.Username) || string.IsNullOrEmpty(_DBSettings.Password))
            {
                _Logger.LogCritical("Atleast one of the database settings coulnd't load");
                return;
            }

            _Logger.LogInformation("DB Service successfully initialized");

            string dbConn = config.GetValue<string>("ConnectionStrings:DefaultConnection");

            mysqlConn.ConnectionString = dbConn;

            _MysqlConn = mysqlConn;

            TestDBConnection();
        }

        public async Task DeleteFileDataAsync()
        {
            await Task.Delay(500);
        }

        public async Task InsertFileDataAsync(FileModel file)
        {
            string query = "INSERT INTO uploads (guid, filename, filepath) VALUES (?guid, ?filename, ?filepath);";

            file.Guid = "test123";
            file.Filename = "filename";
            file.FilePath = "/asdatest/asdatest";

            Dictionary<string, dynamic> @params = new()
            {
                { "?guid", file.Guid },
                { "?filename", file.Filename },
                { "?filepath", file.FilePath }
            };

        }

        public async Task UpdateFileDataAsync()
        {
            await Task.Delay(500);
        }

        public async Task GetFileDataAsync(string guid)
        {
            string query = "SELECT * FROM uploads;";


        }

        private void TestDBConnection()
        {
            try
            {
                _MysqlConn.Open();

                _Logger.LogInformation("Database reachablility ensured");

                _MysqlConn.Close();
            }
            catch (Exception ex)
            {
                _Logger.LogCritical("DB connection could not be established. Error: " + ex.ToString());
            }

        }

    }
}
