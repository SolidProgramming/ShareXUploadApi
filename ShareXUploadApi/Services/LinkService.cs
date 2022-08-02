using MySql.Data.MySqlClient;
using System;
using System.Data;

namespace ShareXUploadApi.Services
{
    public interface ILinkService
    {
        Task<(bool Success, string? PublicUrl, string? ErrorMessage)> GetLinkByGuidAsync(string guid);
        Task<(bool Success, string? ShortLink, string? ErrorMessage)> CreateShortLinkAsync(string guid);
        Task<(bool Success, string? PublicUrl, string? ErrorMessage)> GetLinkByShortLinkIdAsync(string linkId);
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

        public async Task<(bool Success, string? ShortLink, string? ErrorMessage)> CreateShortLinkAsync(string guid)
        {
            if (_DBService is null) return (false, null, "[LinkService]DB service not initialized");

            FileModel? file = await _DBService.GetFileDataAsync(guid);

            if (file is null) return (false, null, $"Could't find file with guid: {guid} in database");

            string shorturlId = ShortUrl.Encode(file.Id);

            (bool shortLinkInsertSuccess, string? shortLinkInsertErrorMessage) = await _DBService.InsertShortLinkAsync(guid, shorturlId);

            if (shortLinkInsertSuccess)
            {
                return (true, "reducemy.link/" + shorturlId, null);
            }

            return (false, null, shortLinkInsertErrorMessage);
        }

        public async Task<(bool Success, string? PublicUrl, string? ErrorMessage)> GetLinkByShortLinkIdAsync(string linkId)
        {
            if (_DBService is null) return (false, null, "[LinkService]DB service not initialized");
            if (string.IsNullOrEmpty(_UriSettings.PublicUri)) return (false, null, "No public uri found in settings");

            string query = "SELECT uploads.* FROM uploads LEFT JOIN shortlinks ON uploads.guid = shortlinks.guid WHERE shortlinks.link_Id = ?link_id;";

            Dictionary<string, dynamic> @params = new()
            {
                { "?link_id", linkId }
           };

            (bool selectSuccess, DataSet? data, string? selectErrorMessage) = await _DBService.SelectAsync(query, @params);

            if (!selectSuccess) return (false, null, selectErrorMessage);

            FileModel file = new()
            {
                Id = (int)data.Tables[0].Rows[0]["id"],
                Guid = data.Tables[0].Rows[0]["guid"].ToString(),
                FileExtension = data.Tables[0].Rows[0]["file_extension"].ToString()
            };

            string publicUrl = Path.Combine(_UriSettings.PublicUri, file.Filename);

            return (true, publicUrl, null);
        }
    }
}
