using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Input;

namespace KeyMapper.Behaviors
{
    public class WindowDragBehavior : Behavior<UIElement>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.MouseLeftButtonDown += OnMouseLeftButtonDown;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.MouseLeftButtonDown -= OnMouseLeftButtonDown;
            base.OnDetaching();
        }

        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var window = Window.GetWindow(AssociatedObject);
            if (window != null && e.ButtonState == MouseButtonState.Pressed)
                window.DragMove();
        }
    }
}
