namespace ShareXUploadApi.Models
{
    public class PathSettingsModel : ISettingsMapping
    {
        [JsonPropertyName("dockerFolder")]
        public string DockerFolder { get; set; }

        [JsonPropertyName("desktopFolder")]
        public string DesktopFolder { get; set; }
        public DBSettingsModel DBSettings { get; }
        public PathSettingsModel PathSettings { get; }
    }
}
