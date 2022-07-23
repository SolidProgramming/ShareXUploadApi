namespace ShareXUploadApi.Services
{
    public interface IDBService
    {
        Task InsertFileDataAsync();
        Task UpdateFileDataAsync();
        Task DeleteFileDataAsync();

    }
    public class DBService : IDBService
    {
        private readonly DBSettingsModel? _DBSettings;
        private readonly ILogger<DBService> _Logger;
        public static string? ConnectionString;

        public DBService(ILogger<DBService> logger)
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

            ConnectionString = GetConnectionString();

            _Logger.LogInformation("DB Service successfully initialized");
        }

        public async Task DeleteFileDataAsync()
        {
            await Task.Delay(500);
        }

        public async Task InsertFileDataAsync()
        {
            await Task.Delay(500);
        }

        public async Task UpdateFileDataAsync()
        {
            await Task.Delay(500);
        }

        public string? GetConnectionString()
        {
            if(_DBSettings is null) return null;
            return $"server={_DBSettings.IP}; database={_DBSettings.Database}; user={_DBSettings.Username}; password={_DBSettings.Password}";
        }

    }
}
