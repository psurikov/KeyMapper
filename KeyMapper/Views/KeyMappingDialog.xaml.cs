using KeyMapper.ViewModels;
using System.ComponentModel;
using System.Windows;

namespace KeyMapper.Views
{
    /// <summary>
    /// Interaction logic for KeyMappingDialog.xaml
    /// </summary>
    public partial class KeyMappingDialog : Window, INotifyPropertyChanged
    {
        private KeyMappingViewModel _originalKeyMapping;
        private KeyMappingViewModel _modifiedKeyMapping;

        public KeyMappingDialog()
        {
            _originalKeyMapping = new KeyMappingViewModel();
            _modifiedKeyMapping = new KeyMappingViewModel();
            InitializeComponent();
            Load();
        }

        public KeyMappingDialog(KeyMappingViewModel keyMapping)
        {
            _originalKeyMapping = keyMapping;
            _modifiedKeyMapping = new KeyMappingViewModel();
            InitializeComponent();
            Load();
        }

        public KeyMappingViewModel ModifiedKeyMapping
        {
            get { return _modifiedKeyMapping; }
            set
            {
                if (_modifiedKeyMapping == value)
                    return;
                _modifiedKeyMapping = value;
                OnPropertyChanged(nameof(ModifiedKeyMapping));
            }
        }

        private void AcceptClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Save();
            Close();
        }

        private void CancelClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void Load()
        {
            _modifiedKeyMapping.Copy(_originalKeyMapping);
        }

        private void Save()
        {
            _originalKeyMapping.Copy(_modifiedKeyMapping);
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
