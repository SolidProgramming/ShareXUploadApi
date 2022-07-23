namespace ShareXUploadApi.Services
{
    public interface IDBService
    {
        Task InsertFileDataAsync();
        Task UpdateFileDataAsync();
        Task DeleteFileDataAsync();

    }
    public class DBService : IDBService
    {
        public async Task DeleteFileDataAsync()
        {
            await Task.Delay(500);
        }

        public async Task InsertFileDataAsync()
        {
            await Task.Delay(500);
        }

        public async Task UpdateFileDataAsync()
        {
            await Task.Delay(500);
        }
    }
}
