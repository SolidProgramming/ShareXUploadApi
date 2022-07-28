using MySql.Data.MySqlClient;
using System;
using System.Data;

namespace ShareXUploadApi.Services
{
    public interface IDBService
    {
        Task InsertFileDataAsync(FileModel file);
        Task UpdateFileDataAsync();
        Task DeleteFileDataAsync(string guid);
        Task<FileModel?> GetFileDataAsync(string guid);

    }
    public class DBService : IDBService
    {
        private static DBSettingsModel? _DBSettings;
        private readonly ILogger<DBService> _Logger;
        private readonly MySqlConnection _MysqlConn = default!;


        public DBService(ILogger<DBService> logger, IConfiguration config, MySqlConnection mysqlConn)
        {
            _Logger = logger;

            _DBSettings = SettingsHandler.ReadSettings<DBSettingsModel>();

            if (_DBSettings is null)
            {
                logger.LogCritical($"{DateTime.Now}|No Database settings found in settings.json");
                return;
            }

            if (string.IsNullOrEmpty(_DBSettings.IP) ||
                string.IsNullOrEmpty(_DBSettings.Database) ||
                string.IsNullOrEmpty(_DBSettings.Username) ||
                string.IsNullOrEmpty(_DBSettings.Password))
            {
                logger.LogCritical($"{DateTime.Now}|One or more of the database settings is empty");
                return;
            }

            mysqlConn.ConnectionString = $"server={_DBSettings.IP};port=3306;database={_DBSettings.Database};user={_DBSettings.Username};password={_DBSettings.Password};";

            _MysqlConn = mysqlConn;

            if (TestDBConnection())
            {
                Task.Run(async () =>
                {
                    await PrepareTablesIfNeeded();
                });
            }

            _Logger.LogInformation($"{DateTime.Now}|DB Service successfully initialized");
        }

        public async Task DeleteFileDataAsync(string guid)
        {
            await EnsureConnectivity();

            string query = "DELETE FROM uploads WHERE guid = ?guid;";

            MySqlCommand mySqlCommand = new(query, _MysqlConn);

            mySqlCommand.Parameters.AddWithValue("?guid", guid);

            try
            {
                int numberOfRowsAffected = await mySqlCommand.ExecuteNonQueryAsync();
                _Logger.LogInformation($"{DateTime.Now}|Deleted entry with guid: {guid} from database. Rows affected: {numberOfRowsAffected}");
            }
            catch (Exception ex)
            {
                _Logger.LogCritical($"{DateTime.Now}|Could not delete database entry with guid: {guid}. Error: " + ex.ToString());
            }
        }

        public async Task InsertFileDataAsync(FileModel file)
        {
            await EnsureConnectivity();

            string query = "INSERT INTO uploads (guid, filename) VALUES (?guid, ?filename);";

            MySqlCommand mySqlCommand = new(query, _MysqlConn);

            mySqlCommand.Parameters.AddWithValue("?guid", file.Guid);
            mySqlCommand.Parameters.AddWithValue("?filename", file.Filename);

            try
            {
                await mySqlCommand.ExecuteNonQueryAsync();
                _Logger.LogInformation($"{DateTime.Now}|File: {file.Guid} |=| {file.Filename} registered in database");
            }
            catch (Exception ex)
            {
                _Logger.LogCritical($"{DateTime.Now}|File: {file.Guid} |=| {file.Filename} could not be registered in database. Error: " + ex.ToString());
            }
        }

        public async Task UpdateFileDataAsync()
        {
            await Task.Delay(500);
        }

        public async Task<FileModel?> GetFileDataAsync(string guid)
        {
            await EnsureConnectivity();

            string query = "SELECT * FROM uploads WHERE guid = ?guid;";

            MySqlCommand mySqlCommand = new(query, _MysqlConn);

            MySqlDataAdapter adapter = new(mySqlCommand);

            mySqlCommand.Parameters.AddWithValue("?guid", guid);

            DataSet ds = new();

            await adapter.FillAsync(ds);

            if (!HasData(ds))
            {
                _Logger.LogCritical($"{DateTime.Now}|No DB entry with guid {guid} found!");
                return null;
            }

            FileModel file = new()
            {
                Guid = guid,
                Filename = ds.Tables[0].Rows[0]["filename"].ToString()
            };

            _Logger.LogInformation($"{DateTime.Now}|File: {guid} | {file.Filename} read from database");

            return file;
        }

        private bool TestDBConnection()
        {
            try
            {
                _MysqlConn.Open();

                _Logger.LogInformation("Database reachablility ensured");

                _MysqlConn.Close();

                return true;
            }
            catch (Exception ex)
            {
                _Logger.LogCritical("DB connection could not be established. Error: " + ex.ToString());
                return false;
            }

        }

        private async Task PrepareTablesIfNeeded()
        {
            await EnsureConnectivity();

            string query = @"CREATE TABLE IF NOT EXISTS `uploads` (
                           `guid` varchar(50) COLLATE utf8mb4_bin NOT NULL,
                           `filename` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL,
                           PRIMARY KEY(`guid`)
                         ) ENGINE = InnoDB DEFAULT CHARSET = utf8mb4 COLLATE = utf8mb4_bin; ";

            MySqlCommand mySqlCommand = new(query, _MysqlConn);

            mySqlCommand.ExecuteNonQuery();
        }

        private async Task EnsureConnectivity()
        {
            _Logger.LogInformation($"{DateTime.Now}|DB connection state: " + _MysqlConn.State.ToString());

            if (_MysqlConn.State != ConnectionState.Open)
            {
                await _MysqlConn.OpenAsync();
                _Logger.LogInformation($"{DateTime.Now}|Mysql connection opened");
            }
        }

        private bool HasData(DataSet ds)
        {
            if (ds.Tables.Count > 0)
            {
                for (int i = 0; i < ds.Tables.Count; i++)
                {
                    if (ds.Tables[i].Rows.Count == 0)
                    {
                        return false;
                    }
                }

                return true;
            }

            return false;
        }

    }
}
