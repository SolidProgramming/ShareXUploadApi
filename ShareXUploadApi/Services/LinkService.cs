namespace ShareXUploadApi.Services
{
    public interface ILinkService
    {
        Task<(string? Message, HttpStatusCode StatusCode)> GetLinkByGuidAsync(string guid);
    }
    public class LinkService : ILinkService
    {
        public async Task<(string? Message, HttpStatusCode StatusCode)> GetLinkByGuidAsync(string guid)
        {
            return await Task.Run(() =>
            {
                return ("", HttpStatusCode.OK);
            });
        }
    }
}
