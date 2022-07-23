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

        public DBService(ILogger<DBService> logger)
        {
            _Logger = logger;

            _DBSettings = SettingsHandler.ReadSettings<DBSettingsModel>();

            if (_DBSettings is null)
            {
                _Logger.LogCritical("Couldn't load database settings");
                return;
            }
            

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
    }
}
