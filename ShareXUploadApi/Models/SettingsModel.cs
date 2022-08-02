namespace ShareXUploadApi.Models
{
    public class SettingsModel
    {
        [JsonPropertyName("dbSettings")]    
        public DBSettingsModel DBSettings { get; set; } = default!;


        [JsonPropertyName("pathSettings")]
        public PathSettingsModel PathSettings { get; set; } = default!;

        [JsonPropertyName("uriSettings")]
        public UriSettingsModel UriSettings { get; set; } = default!;
    }
}
