namespace ShareXUploadApi.Services
{
    public interface IFileService
    {
        Task<(string? Message, HttpStatusCode StatusCode)> UploadAsync(HttpRequest request);
    }
    public class FileService : IFileService
    {
        private const string FOLDER = @"\\192.168.1.10\FileRunData\sharex\";


        public async Task<(string? Message, HttpStatusCode StatusCode)> UploadAsync(HttpRequest request)
        {
            return await Task.Run(() =>
            {
                try
                {
                    using Stream stream = new FileStream(FOLDER + request.Form.Files[0].FileName, FileMode.Create);

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
