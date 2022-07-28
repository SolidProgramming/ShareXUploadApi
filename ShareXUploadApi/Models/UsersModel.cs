namespace ShareXUploadApi.Models
{
    public class UsersModel
    {
        [JsonPropertyName("id")]
        public int Id { get; set; } = default!;

        [JsonPropertyName("username")]
        public string Username { get; set; } = default!;

        [JsonPropertyName("password")]
        public string Password { get; set; } = default!;
    }
}
