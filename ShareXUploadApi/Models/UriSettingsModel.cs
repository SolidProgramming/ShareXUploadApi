namespace ShareXUploadApi.Models
{
    public class UriSettingsModel
    {
        [JsonPropertyName("public_uri")]
        public string PublicUri { get; set; } = default!;
    }
}
