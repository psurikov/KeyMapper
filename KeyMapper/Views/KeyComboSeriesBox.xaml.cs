using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace KeyMapper.Views
{
    /// <summary>
    /// Interaction logic for KeyComboSeriesBox.xaml
    /// </summary>
    public partial class KeyComboSeriesBox : System.Windows.Controls.UserControl
    {
        public KeyComboSeriesBox()
        {
            InitializeComponent();
            PreviewKeyDown += KeyComboSeriesBox_PreviewKeyDown;
        }

        private void KeyComboSeriesBox_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.LeftCtrl ||
                e.Key == Key.RightCtrl ||
                e.Key == Key.LeftShift ||
                e.Key == Key.RightShift ||
                e.Key == Key.LeftAlt ||
                e.Key == Key.RightAlt)
                return;
            
        }
    }
}
