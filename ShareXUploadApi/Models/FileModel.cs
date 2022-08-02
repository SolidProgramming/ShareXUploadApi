namespace ShareXUploadApi.Models
{
    public class FileModel
    {
        public string Guid { get; set; } = default!;
        public string FileExtension { get; set; } = default!;
        public string Filename
        {
            get
            {
                return Guid + FileExtension;
            }
        }
        public IFormFile File { get; set; } = default!;
    }
}
