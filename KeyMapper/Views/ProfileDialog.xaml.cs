using System.ComponentModel;
using System.Windows;

namespace KeyMapper.Views
{
    /// <summary>
    /// Interaction logic for ProfileDialog.xaml
    /// </summary>
    public partial class ProfileDialog : Window, INotifyPropertyChanged
    {
        private string _profileName = "";

        public ProfileDialog()
        {
            InitializeComponent();
        }

        public string ProfileName
        {
            get { return _profileName; }
            set
            {
                if (_profileName == value)
                    return;
                _profileName = value;
                OnPropertyChanged(nameof(ProfileName));
            }
        }

        private void AcceptButtonClick(object sender, RoutedEventArgs args)
        {
            DialogResult = true;
            Close();
        }

        private void CancelButtonClick(object sender, RoutedEventArgs args)
        {
            DialogResult = false;
            Close();
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
