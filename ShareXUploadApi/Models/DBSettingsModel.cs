

namespace ShareXUploadApi.Models
{
    public class DBSettingsModel
    {
        [JsonPropertyName("ip")]
        public string IP { get; set; }

        [JsonPropertyName("table")]
        public string Table { get; set; }

        [JsonPropertyName("username")]
        public string Username { get; set; }

        [JsonPropertyName("password")]
        public string Password { get; set; }
    }
}
