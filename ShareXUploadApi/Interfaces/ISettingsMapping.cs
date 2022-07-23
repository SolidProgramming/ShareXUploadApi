namespace ShareXUploadApi.Interfaces
{
    public interface ISettingsMapping
    {
        public DBSettingsModel DBSettings { get; }
        public PathSettingsModel PathSettings { get; }
    }
}
