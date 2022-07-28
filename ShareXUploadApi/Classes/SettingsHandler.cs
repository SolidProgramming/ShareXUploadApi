using System.Text.Json;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection;
using System.Diagnostics;
using System.Data.Common;

namespace ShareXUploadApi.Classes
{
    public static class SettingsHandler
    {
        public static T? ReadSettings<T>()
        {
            using StreamReader r = new("settings.json");
            string json = r.ReadToEnd();

            SettingsModel? settings = JsonSerializer.Deserialize<SettingsModel>(json);

            if (settings is null) return default;

            return settings.GetSetting<T>();

        }

        public static T? GetSetting<T>(this SettingsModel settings)
        {
            return (T?)settings?.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .First(_ => _.PropertyType == typeof(T))
                .GetValue(settings, null);
        }

    }
}
