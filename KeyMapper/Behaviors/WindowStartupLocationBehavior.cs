using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Media3D;

namespace KeyMapper.Behaviors
{
    public class WindowStartupLocationBehavior : Behavior<Window>
    {
        public static readonly DependencyProperty StorageProperty =
            DependencyProperty.Register(
                nameof(Storage),
                typeof(IWindowStartupLocationStorage),
                typeof(WindowStartupLocationBehavior),
                new PropertyMetadata(null));

        public IWindowStartupLocationStorage Storage
        {
            get => (IWindowStartupLocationStorage)GetValue(StorageProperty);
            set => SetValue(StorageProperty, value);
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.SourceInitialized += OnInitialize;
            AssociatedObject.Closing += OnClosing;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.SourceInitialized -= OnInitialize;
            AssociatedObject.Closing -= OnClosing;
        }

        private void OnInitialize(object? sender, EventArgs e)
        {
            Load();
        }

        private void OnClosing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            Save();
        }

        private void Load()
        {
            var settings = Storage?.Load();
            settings ??= Construct(AssociatedObject);
            AssociatedObject.Left = settings.Left;
            AssociatedObject.Top = settings.Top;
            AssociatedObject.Width = settings.Width;
            AssociatedObject.Height = settings.Height;
            AssociatedObject.WindowState = settings.Maximized ? WindowState.Maximized : WindowState.Normal;
        }

        private void Save()
        {
            var settings = new WindowStartupLocationSettings();
            settings.Left = (int)AssociatedObject.Left;
            settings.Top = (int)AssociatedObject.Top;
            settings.Width = (int)AssociatedObject.Width;
            settings.Height = (int)AssociatedObject.Height;
            settings.Monitor = GetMonitorIndex(AssociatedObject);
            settings.Maximized = AssociatedObject.WindowState == WindowState.Maximized;
            Storage?.Save(settings);
        }

        private static WindowStartupLocationSettings Construct(Window window)
        {
            var screenSize = GetScreenSize(window);
            var idealSize = GetIdealWindowSize(screenSize);
            var settings = new WindowStartupLocationSettings();
            settings.Left = (int)(screenSize.Width - idealSize.Width) / 2;
            settings.Top = (int)(screenSize.Height - idealSize.Height) / 2;
            settings.Width = (int)(idealSize.Width);
            settings.Height = (int)(idealSize.Height);
            settings.Monitor = GetMonitorIndex(window);
            settings.Maximized = false;
            return settings;
        }

        private static (double Width, double Height) GetIdealWindowSize((double width, double height) screenSize)
        {
            var (width, height) = screenSize;
            var desiredWidth = 1115;
            var desiredHeight = 800;
            var idealWidth = (int)(width * 0.6);
            var idealHeight = (int)(height * 0.8);
            if (idealWidth > desiredWidth)
                idealWidth = desiredWidth;
            if (idealHeight > desiredHeight)
                idealHeight = desiredHeight;
            return (idealWidth, idealHeight);
        }

        private static (double Width, double Height) GetScreenSize(Window window)
        {
            var handle = new WindowInteropHelper(window).Handle;
            var source = HwndSource.FromHwnd(handle);
            var matrix = source.CompositionTarget.TransformFromDevice;

            var screen = Screen.FromHandle(handle);
            var width = screen.Bounds.Width;
            var height = screen.Bounds.Height;

            var topLeft = matrix.Transform(new System.Windows.Point(width, height));
            return (topLeft.X, topLeft.Y);
        }

        private static int GetMonitorIndex(Window window)
        {
            var helper = new WindowInteropHelper(window);
            var handle = helper.Handle;
            if (handle == IntPtr.Zero)
                return 0;
            var screen = Screen.FromHandle(handle);
            var allScreens = Screen.AllScreens;
            for (int i = 0; i < allScreens.Length; i++)
            {
                if (allScreens[i].Equals(screen))
                    return i;
            }
            return 0;
        }
    }

    public interface IWindowStartupLocationStorage
    {
        WindowStartupLocationSettings? Load();
        void Save(WindowStartupLocationSettings settings);
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
