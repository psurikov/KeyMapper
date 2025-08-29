using KeyMapper.Models;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace KeyMapper.Controls
{
    public class KeyComboSeriesBox : System.Windows.Controls.Control
    {
        static KeyComboSeriesBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(KeyComboSeriesBox),
                new FrameworkPropertyMetadata(typeof(KeyComboSeriesBox)));
        }

        public KeyComboSeriesBox()
        {
            PreviewKeyDown += KeyComboSeriesBox_PreviewKeyDown;
            Focusable = true;
            MouseDown += KeyComboSeriesBox_MouseDown;
        }

        public static readonly DependencyProperty KeyCombosProperty =
            DependencyProperty.Register(
                nameof(KeyCombos),
                typeof(ObservableCollection<KeyCombo>),
                typeof(KeyComboSeriesBox),
                new PropertyMetadata(null));

        public ObservableCollection<KeyCombo> KeyCombos
        {
            get { return (ObservableCollection<KeyCombo>)GetValue(KeyCombosProperty); }
            set { SetValue(KeyCombosProperty, value); }
        }

        public static readonly DependencyProperty KeyCombosMaxProperty =
            DependencyProperty.Register(
                nameof(KeyCombosMax),
                typeof(int),
                typeof(KeyComboSeriesBox),
                new PropertyMetadata(2));

        public int KeyCombosMax
        {
            get { return (int)GetValue(KeyCombosMaxProperty); }
            set { SetValue(KeyCombosMaxProperty, value); }
        }

        private void KeyComboSeriesBox_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.LeftCtrl ||
                e.Key == Key.RightCtrl ||
                e.Key == Key.LeftShift ||
                e.Key == Key.RightShift ||
                e.Key == Key.LeftAlt ||
                e.Key == Key.RightAlt || e.Key == Key.Tab || e.Key == Key.Return)
                return;

            var keyCombos = KeyCombos;
            if (keyCombos == null)
                return;

            if (e.Key == Key.Back)
            {
                if (keyCombos.Count > 0)
                    keyCombos.RemoveAt(keyCombos.Count - 1);
            }
            else
            {
                var modifierKeys = Keyboard.Modifiers;
                var actionKey = e.Key;
                var keyCombo = new KeyCombo(modifierKeys, actionKey);
                if (keyCombos.Count < KeyCombosMax)
                    keyCombos.Add(keyCombo);
                else
                {
                    keyCombos.Clear();
                    keyCombos.Add(keyCombo);
                }
            }
        }

        private void KeyComboSeriesBox_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Focus();
            e.Handled = true;
        }
    }
}