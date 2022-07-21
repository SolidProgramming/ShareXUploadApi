using Microsoft.AspNetCore.Components;

namespace ShareXUploadApi.Services
{

    public interface IApiService
    {

        Task<(string? Message, HttpStatusCode StatusCode)> GetAsync(string url);
        Task<(string? Message, HttpStatusCode StatusCode)> PostAsync(string url, object value);
        Task<(string? Message, HttpStatusCode StatusCode)> PutAsync(string url, object value);
        Task<(string? Message, HttpStatusCode StatusCode)> PatchAsync(string url);
        Task<(string? Message, HttpStatusCode StatusCode)> DeleteAsync(string url);
    }

    public class ApiService : IApiService
    {
        private readonly HttpClient _HttpClient;

        public ApiService(HttpClient httpClient)
        {
            _HttpClient = httpClient;
            _HttpClient.BaseAddress = new Uri("https://localhost:44395/sharex");
        }

        public Task<(string? Message, HttpStatusCode StatusCode)> DeleteAsync(string url)
        {
            throw new NotImplementedException();
        }

        public Task<(string? Message, HttpStatusCode StatusCode)> GetAsync(string url)
        {
            throw new NotImplementedException();
        }

        public Task<(string? Message, HttpStatusCode StatusCode)> PatchAsync(string url)
        {
            throw new NotImplementedException();
        }

        public Task<(string? Message, HttpStatusCode StatusCode)> PostAsync(string url, object value)
        {
            throw new NotImplementedException();
        }

        public Task<(string? Message, HttpStatusCode StatusCode)> PutAsync(string url, object value)
        {
            throw new NotImplementedException();
        }
    }

}
