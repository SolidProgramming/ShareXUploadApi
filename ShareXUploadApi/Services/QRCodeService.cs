using Microsoft.AspNetCore.Mvc;

namespace ShareXUploadApi.Services
{
    public interface IQRCodeService
    {
        Task<(bool Success, FileContentResult? File, string? ErrorMessage)> GenerateQRCode(string url);
    }
    public class QRCodeService : IQRCodeService
    {
        private readonly ILogger<QRCodeService> _Logger;
        private readonly HttpClient _HttpClient;

        public QRCodeService(ILogger<QRCodeService> logger, HttpClient httpClient)
        {
            _Logger = logger;
            _HttpClient = httpClient;

            _Logger.LogInformation($"{DateTime.Now}|QRCode Service successfully initialized");
        }

        public async Task<(bool Success, FileContentResult? File, string? ErrorMessage)> GenerateQRCode(string url)
        {
            string qcCodeAPIUri = $"https://api.qrserver.com/v1/create-qr-code/?data={url}&size=150x150";

            try
            {
                using HttpResponseMessage response = await _HttpClient.GetAsync(qcCodeAPIUri);

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    _Logger.LogCritical($"{DateTime.Now}|QRCode API returned an error");
                    return (false, null, response.Content.ToString());
                }

                byte[] content = await response.Content.ReadAsByteArrayAsync();

                return (true, new FileContentResult(content, "image/png"), null);
            }
            catch (Exception ex)
            {
                return (false, null, ex.ToString());
            }
        }
    }
}
