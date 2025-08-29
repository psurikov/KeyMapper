using KeyMapper.Behaviors;
using KeyMapper.Services;
using KeyMapper.ViewModels;
using System.ComponentModel;
using System.Windows;

namespace KeyMapper.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private IWindowStartupLocationStorage _windowStartupLocationStorage = null!;

        public MainWindow()
        {
            InitializeComponent();
            _windowStartupLocationStorage = new WindowStartupLocationStorage();
            var keyMappingDialogService = new KeyMappingDialogService();
            var profileViewModelFactory = new ProfileViewModelFactory(keyMappingDialogService);
            var profileDialogService = new ProfileDialogService();
            DataContext = new MainWindowViewModel(profileViewModelFactory, profileDialogService);
            Loaded += WindowLoaded;
            Closing += WindowClosing;
            Closed += WindowClosed;
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as MainWindowViewModel;
            if (viewModel != null)
                viewModel.Load();
        }

        private void WindowClosing(object? sender, CancelEventArgs e)
        {
            var viewModel = DataContext as MainWindowViewModel;
            if (viewModel != null)
                viewModel.Save();
        }

        private void WindowClosed(object? sender, EventArgs e)
        {
            (DataContext as IDisposable)?.Dispose();
        }

        public IWindowStartupLocationStorage WindowStartupLocationStorage
        {
            get { return _windowStartupLocationStorage; }
            set
            {
                if (_windowStartupLocationStorage == value)
                    return;
                _windowStartupLocationStorage = value;
                OnPropertyChanged(nameof(WindowStartupLocationStorage));
            }
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }

    public class WindowStartupLocationStorage : IWindowStartupLocationStorage
    {
        public Behaviors.WindowStartupLocationSettings? Load()
        {
            var app = (App)App.Current;
            var storedSettings = app.Settings.WindowsStartupLocationSettings;
            if (storedSettings == null)
                return null;
            var behaviorSettings = new Behaviors.WindowStartupLocationSettings();
            behaviorSettings.Left = storedSettings.Left;
            behaviorSettings.Top = storedSettings.Top;
            behaviorSettings.Width = storedSettings.Width;
            behaviorSettings.Height = storedSettings.Height;
            behaviorSettings.Monitor = storedSettings.Monitor;
            behaviorSettings.Maximized = storedSettings.Maximized;
            return behaviorSettings;
        }

        public void Save(Behaviors.WindowStartupLocationSettings behaviorSettings)
        {
            var storedSettings = new Config.WindowStartupLocationSettings();
            storedSettings.Left = behaviorSettings.Left;
            storedSettings.Top = behaviorSettings.Top;
            storedSettings.Width = behaviorSettings.Width;
            storedSettings.Height = behaviorSettings.Height;
            storedSettings.Monitor = behaviorSettings.Monitor;
            storedSettings.Maximized = behaviorSettings.Maximized;
            var app = (App)App.Current;
            app.Settings.WindowsStartupLocationSettings = storedSettings;
        }
    }
}