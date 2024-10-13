namespace ShareXUploadApi.Services
{
    public interface IFileService
    {
        Task<(bool Success, string? ErrorMessage)> UploadAsync(FileModel file);
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
                _Logger.LogCritical($"{DateTime.Now}|Couldn't load path settings");
                return;
            }

            if (string.IsNullOrEmpty(_PathSettings.DockerFolder))
            {
                _Logger.LogCritical($"{DateTime.Now}|Couldn't load DockerFolder settings");
                return;
            }

            if (string.IsNullOrEmpty(_PathSettings.DesktopFolder))
            {
                _Logger.LogCritical($"{DateTime.Now}|Couldn't load DesktopFolder settings");
                return;
            }

            _Logger.LogInformation($"{DateTime.Now}|File Service successfully initialized");
        }

        public async Task<(bool Success, string? ErrorMessage)> UploadAsync(FileModel file)
        {
            return await Task.Run<(bool, string?)>(() =>
             {
                 string fileExtension = Path.GetExtension(file.FileExtension);
                 string folderPath;

                 if (_PathSettings is null) return (false, "Upload fehlgeschlagen; No path settings found");
                 if (string.IsNullOrEmpty(_PathSettings.DockerFolder) || string.IsNullOrEmpty(_PathSettings.DesktopFolder)) return (false, "Upload fehlgeschlagen; One of the path settings not set");

                 if (Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true")
                 {
                     folderPath = _PathSettings.DockerFolder;
                 }
                 else
                 {
                     folderPath = _PathSettings.DesktopFolder;
                 }

                 try
                 {
                     using Stream stream = new FileStream(Path.Combine(folderPath, file.Guid) + fileExtension, FileMode.Create);
                     file.File.CopyTo(stream);
                     return (true, null);
                 }
                 catch (Exception ex)
                 {
                     return (false, ex.ToString());
                 }
             });
        }
    }
}
