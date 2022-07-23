namespace ShareXUploadApi.Models
{
    public class SettingsModel
    {
        [JsonPropertyName("dbSettings")]    
        public DBSettingsModel DBSettings { get; set; }


        [JsonPropertyName("pathSettings")]
        public PathSettingsModel PathSettings { get; set; }
    }
}
