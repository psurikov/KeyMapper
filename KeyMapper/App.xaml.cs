using KeyMapper.Config;
using KeyMapper.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using Application = System.Windows.Application;

namespace KeyMapper
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private AppSettings _settings = new();
        private IServiceProvider _serviceProvider = null!;

        public AppSettings Settings
        {
            get { return _settings; }
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            InitializeServices();
            LoadSettings();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            SaveSettings();
        }

        private void InitializeServices()
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
            _serviceProvider = services.BuildServiceProvider();
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IConfig>(new JsonConfig("config.json", ConfigLocation.AppDirectory));
            services.AddSingleton<IMessageReportingService, MessageReportingService>();
        }

        private void LoadSettings()
        {
            try
            {
                _settings = _serviceProvider.GetRequiredService<IConfig>().Load();
            }
            catch (ConfigException)
            {
                var messageReportingService = _serviceProvider.GetRequiredService<IMessageReportingService>();
                messageReportingService.ReportError("Could not load settings, the config file is malformed");
            }
        }

        private void SaveSettings()
        {
            try
            {
                _serviceProvider.GetRequiredService<IConfig>().Save(_settings);
            }
            catch (ConfigException)
            {
                // there is nothing that can be done here. We couldn't save the settings, but at least we can allow the program to continue shutting down.
            }
        }
    }
}