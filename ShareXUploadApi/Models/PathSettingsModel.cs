namespace ShareXUploadApi.Models
{
    public class PathSettingsModel
    {
        [JsonPropertyName("dockerFolder")]
        public string DockerFolder { get; set; } = default!;

        [JsonPropertyName("desktopFolder")]
        public string DesktopFolder { get; set; } = default!;
    }
}
