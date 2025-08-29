using System.IO;
using System.Text.Json;

namespace KeyMapper.Config
{
    public class JsonConfig : IConfig
    {
        private readonly string _filePath;

        public JsonConfig(string filePath, ConfigLocation location) 
        {
            var directory = "";
            if (location == ConfigLocation.AppDirectory)
                directory = Path.Combine(AppContext.BaseDirectory);
            else if (location == ConfigLocation.UserDirectory)
                directory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "KeyMapper");
            _filePath = Path.Combine(directory, filePath);
        }

        public AppSettings Load()
        {
            if (!File.Exists(_filePath))
                return new AppSettings();
            var json = File.ReadAllText(_filePath);
            return JsonSerializer.Deserialize<AppSettings>(json) ?? new AppSettings();
        }

        public void Save(AppSettings settings)
        {
            var directory = Path.GetDirectoryName(_filePath);
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory!);
            var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_filePath, json);
        }
    }
}
