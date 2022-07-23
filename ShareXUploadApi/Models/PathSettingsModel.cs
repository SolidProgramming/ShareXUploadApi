namespace ShareXUploadApi.Models
{
    public class PathSettingsModel
    {
        [JsonPropertyName("dockerFolder")]
        public string DockerFolder { get; set; }

        [JsonPropertyName("desktopFolder")]
        public string DesktopFolder { get; set; }
    }
}
