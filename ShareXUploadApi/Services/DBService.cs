using MySql.Data.MySqlClient;
using System;
using System.Data;

namespace ShareXUploadApi.Services
{
    public interface IDBService
    {
        Task<(bool Success, string? ErrorMessage)> InsertFileDataAsync(FileModel file);
        Task<FileModel?> GetFileDataAsync(string guid);
        Task<(bool Success, string? ErrorMessage)> InsertShortLinkAsync(string guid, string linkId);
        Task<(bool Success, DataSet? Data, string? ErrorMessage)> SelectAsync(string query, Dictionary<string, dynamic>? @params = null);
        Task<bool> IsUserAuthenticated(UsersModel user);

    }
    public class DBService : IDBService
    {
        private static DBSettingsModel? _DBSettings;
        private readonly ILogger<DBService> _Logger;
        private readonly MySqlConnection _MysqlConn = default!;


        public DBService(ILogger<DBService> logger, MySqlConnection mysqlConn)
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

        public async Task<(bool Success, string? ErrorMessage)> InsertFileDataAsync(FileModel file)
        {
            await EnsureConnectivity();

            string query = "INSERT INTO uploads (guid, file_extension) VALUES (?guid, ?file_extension);";

            MySqlCommand mySqlCommand = new(query, _MysqlConn);

            string fileExtension = Path.GetExtension(file.FileExtension);

            mySqlCommand.Parameters.AddWithValue("?guid", file.Guid);
            mySqlCommand.Parameters.AddWithValue("?file_extension", fileExtension);

            try
            {
                await mySqlCommand.ExecuteNonQueryAsync();
                _Logger.LogInformation($"{DateTime.Now}|File: {file.Guid} |=| {file.FileExtension} registered in database");
                return (true, null);
            }
            catch (Exception ex)
            {
                _Logger.LogCritical($"{DateTime.Now}|File: {file.Guid} |=| {file.FileExtension} could not be registered in database. Error: " + ex.ToString());
                return (true, ex.ToString());
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
                Id = (int)ds.Tables[0].Rows[0]["id"],
                Guid = guid,
                FileExtension = ds.Tables[0].Rows[0]["file_extension"].ToString()
            };

            _Logger.LogInformation($"{DateTime.Now}|File: {file.Filename} read from database");

            return file;
        }

        public async Task<bool> IsUserAuthenticated(UsersModel user)
        {
            await EnsureConnectivity();

            string query = "SELECT id FROM users WHERE username = ?username AND password = ?password;";

            MySqlCommand mySqlCommand = new(query, _MysqlConn);

            MySqlDataAdapter adapter = new(mySqlCommand);

            mySqlCommand.Parameters.AddWithValue("?username", user.Username);
            mySqlCommand.Parameters.AddWithValue("?password", user.Password);

            DataSet ds = new();

            await adapter.FillAsync(ds);

            if (!HasData(ds))
            {
                _Logger.LogCritical($"{DateTime.Now}|Can't find user with corresponding password found: {user.Username}");
                return false;
            }

            _Logger.LogInformation($"{DateTime.Now}|User found with name and matching password: {user.Username}");
            return true;
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
                           `file_extension` varchar(10) CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL,
                           PRIMARY KEY(`guid`)
                         ) ENGINE = InnoDB DEFAULT CHARSET = utf8mb4 COLLATE = utf8mb4_bin; ";

            MySqlCommand mySqlCommand = new(query, _MysqlConn);

            await mySqlCommand.ExecuteNonQueryAsync();

            query = @"CREATE TABLE IF NOT EXISTS `users` (
                     `id` int NOT NULL AUTO_INCREMENT,
                     `username` varchar(50) COLLATE utf8mb4_bin NOT NULL,
                     `password` varchar(50) COLLATE utf8mb4_bin NOT NULL,
                     PRIMARY KEY(`id`) USING BTREE
                   ) ENGINE = InnoDB AUTO_INCREMENT = 2 DEFAULT CHARSET = utf8mb4 COLLATE = utf8mb4_bin; ";
            mySqlCommand.CommandText = query;
            await mySqlCommand.ExecuteNonQueryAsync();

            query = @"INSERT INTO `users` (`id`, `username`, `password`) VALUES (1, 'admin', 'admin');";
            mySqlCommand.CommandText = query;
            await mySqlCommand.ExecuteNonQueryAsync();

            query = @"CREATE TABLE IF NOT EXISTS `shortlinks` (
                    `guid` varchar(50) COLLATE utf8mb4_bin NOT NULL,
                    `link_id` varchar(10) COLLATE utf8mb4_bin NOT NULL,
                    PRIMARY KEY(`guid`)
                  ) ENGINE = InnoDB DEFAULT CHARSET = utf8mb4 COLLATE = utf8mb4_bin; ";
            mySqlCommand.CommandText = query;
            await mySqlCommand.ExecuteNonQueryAsync();

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

        private static bool HasData(DataSet ds)
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

        public async Task<(bool Success, string? ErrorMessage)> InsertShortLinkAsync(string guid, string linkId)
        {
            await EnsureConnectivity();

            string query = "INSERT INTO shortlinks (guid, link_id) VALUES (?guid, ?link_id);";

            MySqlCommand mySqlCommand = new(query, _MysqlConn);

            mySqlCommand.Parameters.AddWithValue("?guid", guid);
            mySqlCommand.Parameters.AddWithValue("?link_id", linkId);

            try
            {
                await mySqlCommand.ExecuteNonQueryAsync();
                _Logger.LogInformation($"{DateTime.Now}|Shortlink: {guid} |=| {linkId} registered in database");
                return (true, null);
            }
            catch (Exception ex)
            {
                _Logger.LogCritical($"{DateTime.Now}Shortlink: {guid} |=| {linkId} could not be registered in database. Error: " + ex.ToString());
                return (false, ex.ToString());
            }
        }

        public async Task<(bool Success, DataSet? Data, string? ErrorMessage)> SelectAsync(string query, Dictionary<string, dynamic>? @params = null)
        {
            await EnsureConnectivity();

            MySqlCommand mySqlCommand = new(query, _MysqlConn);

            MySqlDataAdapter adapter = new(mySqlCommand);

            if (@params is not null)
            {
                foreach (KeyValuePair<string, dynamic> param in @params)
                {
                    mySqlCommand.Parameters.AddWithValue(param.Key, param.Value);
                }
            }

            DataSet ds = new();

            await adapter.FillAsync(ds);

            if (!HasData(ds))
            {
                return (false, default, "Query successfull but it returned no data");
            }

            return (true, ds, null);
        }
    }
}
