using KeyMapper.Models;
using System.Windows;

namespace KeyMapper.Controls
{
    public class KeyComboTag : System.Windows.Controls.Control
    {
        public static readonly DependencyProperty KeyComboProperty =
            DependencyProperty.Register(
                nameof(KeyCombo),
                typeof(KeyCombo),
                typeof(KeyComboTag),
                new PropertyMetadata(new KeyCombo()));

        public KeyCombo KeyCombo
        {
            get { return (KeyCombo)GetValue(KeyComboProperty); }
            set { SetValue(KeyComboProperty, value); }
        }
    }
}
