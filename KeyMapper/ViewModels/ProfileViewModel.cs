using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KeyMapper.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace KeyMapper.ViewModels
{
    public class ProfileViewModel : ObservableObject
    {
        private string _name;
        private bool _isEnabled;
        private ObservableCollection<KeyMappingViewModel> _keyMappings;
        private KeyMappingViewModel? _selectedKeyMapping;
        private readonly ICommand _createMappingCommand;
        private readonly ICommand _modifyMappingCommand;
        private readonly ICommand _removeMappingCommand;
        private readonly ICommand _moveUpMappingCommand;
        private readonly ICommand _moveDownMappingCommand;
        private readonly IKeyMappingDialogService _dialogService;

        public ProfileViewModel(string name, IKeyMappingDialogService dialogService)
        {
            _name = name;
            _keyMappings = [];
            _createMappingCommand = new RelayCommand(CreateMapping, () => true);
            _modifyMappingCommand = new RelayCommand<KeyMappingViewModel>(ModifyMapping, mapping => mapping != null);
            _removeMappingCommand = new RelayCommand<KeyMappingViewModel>(RemoveMapping, mapping => mapping != null);
            _moveUpMappingCommand = new RelayCommand<KeyMappingViewModel>(MoveUpMapping, mapping => mapping != null);
            _moveDownMappingCommand = new RelayCommand<KeyMappingViewModel>(MoveDownMapping, mapping => mapping != null);
            _dialogService = dialogService;
        }

        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        public bool IsEnabled
        {
            get { return _isEnabled; }
            set { SetProperty(ref _isEnabled, value); }
        }

        public ObservableCollection<KeyMappingViewModel> KeyMappings
        {
            get { return _keyMappings; }
            set { SetProperty(ref _keyMappings, value); }
        }

        public KeyMappingViewModel? SelectedKeyMapping
        {
            get { return _selectedKeyMapping; }
            set { SetProperty(ref _selectedKeyMapping, value); }
        }

        public ICommand CreateMappingCommand
        {
            get { return _createMappingCommand; }
        }

        public ICommand ModifyMappingCommand
        {
            get { return _modifyMappingCommand; }
        }

        public ICommand RemoveMappingCommand
        {
            get { return _removeMappingCommand; }
        }

        public ICommand MoveUpMappingCommand
        {
            get { return _moveUpMappingCommand; }
        }

        public ICommand MoveDownMappingCommand
        {
            get { return _moveDownMappingCommand; }
        }

        private void CreateMapping()
        {
            var keyMapping = new KeyMappingViewModel();
            if (_dialogService.EditKeyMapping(keyMapping))
                _keyMappings.Add(keyMapping);
        }

        private void ModifyMapping(KeyMappingViewModel? mapping)
        {
            if (mapping == null)
                return;
            _dialogService.EditKeyMapping(mapping);
        }

        private void RemoveMapping(KeyMappingViewModel? keyMapping)
        {
            if (keyMapping == null)
                return;
            _keyMappings.Remove(keyMapping);
        }

        private void MoveUpMapping(KeyMappingViewModel? keyMapping)
        {
            if (keyMapping == null)
                return;
            var indexOf = _keyMappings.IndexOf(keyMapping);
            if (indexOf < 0 || indexOf == 0)
                return;
            var selectedMapping = SelectedKeyMapping;
            var prevMapping = _keyMappings[indexOf - 1];
            _keyMappings[indexOf] = prevMapping;
            _keyMappings[indexOf - 1] = keyMapping;
            SelectedKeyMapping = selectedMapping;
        }

        private void MoveDownMapping(KeyMappingViewModel? keyMapping)
        {
            if (keyMapping == null)
                return;
            var indexOf = _keyMappings.IndexOf(keyMapping);
            if (indexOf == -1 || indexOf >= _keyMappings.Count - 1)
                return;
            var selectedMapping = SelectedKeyMapping;
            var nextMapping = _keyMappings[indexOf + 1];
            _keyMappings[indexOf] = nextMapping;
            _keyMappings[indexOf + 1] = keyMapping;
            SelectedKeyMapping = selectedMapping;
        }
    }
}
