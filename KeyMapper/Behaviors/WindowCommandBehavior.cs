using Microsoft.Xaml.Behaviors;
using System.Windows;

namespace KeyMapper.Behaviors
{
    public enum WindowAction
    {
        Minimize,
        MaximizeRestore,
        Close
    }

    public class WindowCommandBehavior : Behavior<System.Windows.Controls.Button>
    {
        public WindowAction Action { get; set; }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.Click += AssociatedObject_Click;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.Click -= AssociatedObject_Click;
            base.OnDetaching();
        }

        private void AssociatedObject_Click(object sender, RoutedEventArgs e)
        {
            if (AssociatedObject == null)
                return;

            var window = Window.GetWindow(AssociatedObject);
            if (window == null)
                return;

            switch (Action)
            {
                case WindowAction.Minimize:
                    window.WindowState = WindowState.Minimized;
                    break;
                case WindowAction.MaximizeRestore:
                    window.WindowState = window.WindowState == WindowState.Normal
                        ? WindowState.Maximized
                        : WindowState.Normal;
                    break;
                case WindowAction.Close:
                    window.Close();
                    break;
            }
        }
    }
}
