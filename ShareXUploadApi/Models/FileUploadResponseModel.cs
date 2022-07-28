namespace ShareXUploadApi.Models
{
    public class FileUploadResponseModel
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; } = default!;

        [JsonPropertyName("errorMessage")]
        public string ErrorMessage { get; set; } = default!;

        [JsonPropertyName("guid")]
        public string Guid { get; set; } = default!;
    }
}
