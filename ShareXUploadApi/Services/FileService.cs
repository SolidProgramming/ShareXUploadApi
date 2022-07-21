namespace ShareXUploadApi.Services
{
    public interface IFileService
    {
        Task<(string? Message, HttpStatusCode StatusCode)> UploadAsync(HttpRequest request);
    }
    public class FileService : IFileService
    {
        private string DATAFOLDER { get; set; }

        public FileService()
        {
            DATAFOLDER = GetFolderPath();
        }


        private string GetFolderPath()
        {
            if (Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true")
            {               
                return Path.Combine(Directory.GetCurrentDirectory(), "appdata");
            }
            else
            {
                return @"\\192.168.1.10\FileRunData\sharex\";
            }
        }

        public async Task<(string? Message, HttpStatusCode StatusCode)> UploadAsync(HttpRequest request)
        {
            return await Task.Run(() =>
            {
                try
                {
                    using Stream stream = new FileStream(Path.Combine(DATAFOLDER, request.Form.Files[0].FileName), FileMode.Create);

                    request.Form.Files[0].CopyTo(stream);

                    return ("Upload successfull", HttpStatusCode.OK);
                }
                catch (Exception ex)
                {
                    return (ex.ToString(), HttpStatusCode.BadRequest);
                }      
            });            
        }
    }
}
