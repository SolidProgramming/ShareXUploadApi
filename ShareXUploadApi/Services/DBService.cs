using MySql.Data.MySqlClient;
using System;
using System.Data;

namespace ShareXUploadApi.Services
{
    public interface IDBService
    {
        Task InsertFileDataAsync(FileModel file);
        Task UpdateFileDataAsync();
        Task DeleteFileDataAsync();
        Task<FileModel?> GetFileDataAsync(string guid);

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
            await EnsureConnectivity();

            string query = "INSERT INTO uploads (guid, filename) VALUES (?guid, ?filename);";

            MySqlCommand mySqlCommand = new(query, _MysqlConn);

            MySqlDataAdapter adapter = new(mySqlCommand);

            mySqlCommand.Parameters.AddWithValue("?guid", file.Guid);
            mySqlCommand.Parameters.AddWithValue("?filename", file.Filename);

            await mySqlCommand.ExecuteNonQueryAsync();

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

            if (!HasData(ds)) return null;

            FileModel file = new()
            {
                Guid = guid,
                Filename = ds.Tables[0].Rows[0]["filename"].ToString()
            };

            return file;
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

        private async Task EnsureConnectivity()
        {
            if(_MysqlConn.State != ConnectionState.Open)
            {
                await _MysqlConn.OpenAsync();
            }
        }

        private bool HasData(DataSet ds)
        {
            if(ds.Tables.Count > 0)
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
