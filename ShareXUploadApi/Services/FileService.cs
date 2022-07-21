namespace ShareXUploadApi.Services
{
    public interface IFileService
    {
        Task<(string? Message, HttpStatusCode StatusCode)> UploadAsync();
    }
    public class FileService : IFileService
    {
        private readonly IApiService _ApiService;

        public FileService(IApiService apiService)
        {
            _ApiService = apiService;
        }

        public async Task<(string? Message, HttpStatusCode StatusCode)> UploadAsync()
        {
            return await Task.Run(() =>
            {
                return ("", HttpStatusCode.OK);
            });
            //return await _ApiService.PostAsync("upload", new object()); ;
        }
    }
}
