using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace KeyMapper.Behaviors
{
    public enum ResizeDirection
    {
        Left, 
        Right, 
        Top, 
        Bottom,
        TopLeft, 
        TopRight,
        BottomLeft,
        BottomRight
    }

    public class WindowResizeBehavior : Behavior<Thumb>
    {
        public ResizeDirection Direction { get; set; }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.DragDelta += OnDragDelta;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.DragDelta -= OnDragDelta;
            base.OnDetaching();
        }

        private void OnDragDelta(object sender, DragDeltaEventArgs e)
        {
            var window = Window.GetWindow(AssociatedObject);
            if (window == null || window.WindowState == WindowState.Maximized)
                return;

            var minWidth = window.MinWidth;
            var minHeight = window.MinHeight;

            switch (Direction)
            {
                case ResizeDirection.Left:
                    ResizeLeft(window, e, minWidth);
                    break;
                case ResizeDirection.Right:
                    ResizeRight(window, e, minWidth);
                    break;
                case ResizeDirection.Top:
                    ResizeTop(window, e, minHeight);
                    break;
                case ResizeDirection.Bottom:
                    ResizeBottom(window, e, minHeight);
                    break;
                case ResizeDirection.TopLeft:
                    ResizeLeft(window, e, minWidth);
                    ResizeTop(window, e, minHeight);
                    break;
                case ResizeDirection.TopRight:
                    ResizeTop(window, e, minHeight);
                    ResizeRight(window, e, minWidth);
                    break;
                case ResizeDirection.BottomLeft:
                    ResizeBottom(window, e, minHeight);
                    ResizeLeft(window, e, minWidth);
                    break;
                case ResizeDirection.BottomRight:
                    ResizeBottom(window, e, minHeight);
                    ResizeRight(window, e, minWidth);
                    break;
            }
        }

        private static void ResizeLeft(Window window, DragDeltaEventArgs e, double minWidth)
        {
            if (window.Width - e.HorizontalChange >= minWidth)
            {
                window.Left += e.HorizontalChange;
                window.Width -= e.HorizontalChange;
            }
        }

        private static void ResizeTop(Window window, DragDeltaEventArgs e, double minHeight)
        {
            if (window.Height - e.VerticalChange >= minHeight)
            {
                window.Top += e.VerticalChange;
                window.Height -= e.VerticalChange;
            }
        }

        private static void ResizeRight(Window window, DragDeltaEventArgs e, double minWidth)
        {
            if (window.Width + e.HorizontalChange >= minWidth)
                window.Width += e.HorizontalChange;
        }

        private static void ResizeBottom(Window window, DragDeltaEventArgs e, double minHeight)
        {
            if (window.Height + e.VerticalChange >= minHeight)
                window.Height += e.VerticalChange;
        }
    }
}
