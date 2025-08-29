namespace KeyMapper.Config
{
    public class AppSettings
    {
        public GeneralSettings? GeneralSettings { get; set; }
        public WindowStartupLocationSettings? WindowsStartupLocationSettings { get; set; }
    }

    public class GeneralSettings
    {
        public List<ProfileSettings> Profiles { get; set; } = [];
    }

    public class ProfileSettings
    {
        public string Name { get; set; } = string.Empty;
        public bool IsEnabled { get; set; } = false;
        public List<KeyMappingSettings> KeyMappings { get; set; } = [];
    }

    public class KeyMappingSettings
    {
        public string SourceKeyCombos { get; set; } = string.Empty;
        public string TargetKeyCombos { get; set; } = string.Empty;
    }

    public class WindowStartupLocationSettings
    {
        public int Left { get; set; }
        public int Top { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int Monitor { get; set; }
        public bool Maximized { get; set; }
    }
}
