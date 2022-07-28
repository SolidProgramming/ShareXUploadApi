namespace ShareXUploadApi.Models
{
    public class SettingsModel
    {
        [JsonPropertyName("dbSettings")]    
        public DBSettingsModel DBSettings { get; set; } = default!;


        [JsonPropertyName("pathSettings")]
        public PathSettingsModel PathSettings { get; set; } = default!;
    }
}
