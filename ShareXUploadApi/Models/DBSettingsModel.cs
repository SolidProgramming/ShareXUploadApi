

namespace ShareXUploadApi.Models
{
    public class DBSettingsModel
    {
        [JsonPropertyName("ip")]
        public string IP { get; set; } = default!;

        [JsonPropertyName("database")]
        public string Database { get; set; } = default!;

        [JsonPropertyName("username")]
        public string Username { get; set; } = default!;

        [JsonPropertyName("password")]
        public string Password { get; set; } = default!;
    }
}
