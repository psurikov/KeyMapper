namespace KeyMapper.Config
{
    public interface IConfig
    {
        AppSettings Load();
        void Save(AppSettings settings);
    }

    public enum ConfigLocation
    {
        Custom,
        AppDirectory,
        UserDirectory
    }

    public class ConfigException : Exception
    {
        public ConfigException(string message) : base(message) { }
    }
}
