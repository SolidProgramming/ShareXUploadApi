namespace ShareXUploadApi.Classes
{
    public class SettingsMapping : ISettingsMapping
    {
        public DBSettingsModel DBSettings { get; }
        public PathSettingsModel PathSettings { get; }
    }
}
