﻿namespace ShareXUploadApi.Services
{
    public interface IFileService
    {
        Task<(string? Message, HttpStatusCode StatusCode)> UploadAsync(HttpRequest request);
    }
    public class FileService : IFileService
    {
        private readonly PathSettingsModel? _PathSettings;
        private readonly ILogger<FileService> _Logger;

        public FileService(ILogger<FileService> logger)
        {
            _Logger = logger;

            _PathSettings = SettingsHandler.ReadSettings<PathSettingsModel>();

            if (_PathSettings is null)
            {
                _Logger.LogCritical("Couldn't load path settings");
                return;
            }

            if (string.IsNullOrEmpty(_PathSettings.DockerFolder))
            {
                _Logger.LogCritical("Couldn't load DockerFolder settings");
                return;
            }

            if (string.IsNullOrEmpty(_PathSettings.DesktopFolder))
            {
                _Logger.LogCritical("Couldn't load DesktopFolder settings");
                return;
            }

            _Logger.LogInformation("File Service successfully initialized");
        }

        public async Task<(string? Message, HttpStatusCode StatusCode)> UploadAsync(HttpRequest request)
        {
            return await Task.Run(() =>
            {
                string folderPath;

                if (_PathSettings is null) return ("Upload fehlgeschlagen; No path settings found", HttpStatusCode.InternalServerError);
                if (string.IsNullOrEmpty(_PathSettings.DockerFolder) || string.IsNullOrEmpty(_PathSettings.DesktopFolder)) return ("Upload fehlgeschlagen; One of the path settings not set", HttpStatusCode.InternalServerError);

                if (Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true")
                {
                    folderPath = _PathSettings.DockerFolder;
                }
                else
                {
                    folderPath = _PathSettings.DesktopFolder;
                }

                using Stream stream = new FileStream(folderPath + request.Form.Files[0].FileName, FileMode.Create);
                request.Form.Files[0].CopyTo(stream);

                return ("Upload successfull", HttpStatusCode.OK);

            });
        }
    }
}
