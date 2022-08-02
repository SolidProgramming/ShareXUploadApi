namespace ShareXUploadApi.Services
{
    public interface ILinkService
    {
        Task<(bool Success, string? PublicUrl, string? ErrorMessage)> GetLinkByGuidAsync(string guid);
    }
    public class LinkService : ILinkService
    {
        private readonly UriSettingsModel? _UriSettings;
        private readonly ILogger<LinkService> _Logger;
        private readonly IDBService? _DBService;

        public LinkService(ILogger<LinkService> logger, IDBService dbService)
        {
            _Logger = logger;

            _UriSettings = SettingsHandler.ReadSettings<UriSettingsModel>();

            if (_UriSettings is null)
            {
                _Logger.LogCritical($"{DateTime.Now}|Couldn't load uriSettings");
                return;
            }

            if (string.IsNullOrEmpty(_UriSettings.PublicUri))
            {
                _Logger.LogCritical($"{DateTime.Now}|Couldn't load public_uri settings");
                return;
            }

            _DBService = dbService;
            _Logger.LogInformation($"{DateTime.Now}|LinkService Service successfully initialized");
        }

        public async Task<(bool Success, string? PublicUrl, string? ErrorMessage)> GetLinkByGuidAsync(string guid)
        {
            if (_DBService is null) return (false, null, "[LinkService]DB service not initialized");
            if (string.IsNullOrEmpty(_UriSettings.PublicUri)) return (false, null, "No public uri found in settings");
            FileModel? file = await _DBService.GetFileDataAsync(guid);
                        
            string path = Path.Combine(_UriSettings.PublicUri, file.Filename);
            return (true, path, null);

        }
    }
}
