using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KeyMapper.Config;
using KeyMapper.Models;
using KeyMapper.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace KeyMapper.ViewModels
{
    public class MainWindowViewModel : ObservableObject, IDisposable
    {
        private ObservableCollection<ProfileViewModel> _profiles;
        private ProfileViewModel? _selectedProfile;
        private readonly IProfileViewModelFactory _profileFactory;
        private readonly IProfileDialogService _profileDialogService;
        private readonly ICommand _createProfileCommand;
        private readonly ICommand _removeProfileCommand;
        private readonly ICommand _modifyProfileCommand;
        private readonly ICommand _moveUpProfileCommand;
        private readonly ICommand _moveDownProfileCommand;
        private readonly WindowsHook _windowsHook;
        private readonly ObservableCollection<KeyCombo> _pressedKeys;

        public MainWindowViewModel(IProfileViewModelFactory profileFactory, IProfileDialogService profileDialogService)
        {
            _profileFactory = profileFactory;
            _profileDialogService = profileDialogService;
            _profiles = [];
            _createProfileCommand = new RelayCommand(CreateProfile);
            _removeProfileCommand = new RelayCommand<ProfileViewModel>(RemoveProfile, profile => profile != null);
            _modifyProfileCommand = new RelayCommand<ProfileViewModel>(ModifyProfile, profile => profile != null);
            _moveUpProfileCommand = new RelayCommand<ProfileViewModel>(MoveUpProfile, profile => profile != null);
            _moveDownProfileCommand = new RelayCommand<ProfileViewModel>(MoveDownProfile, profile => profile != null);
            _windowsHook = new WindowsHook();
            _pressedKeys = [];
        }

        public ObservableCollection<ProfileViewModel> Profiles
        {
            get { return _profiles; }
            set { SetProperty(ref _profiles, value); }
        }

        public ProfileViewModel? SelectedProfile
        {
            get { return _selectedProfile; }
            set { SetProperty(ref _selectedProfile, value); }
        }

        public ICommand CreateProfileCommand
        {
            get { return _createProfileCommand; }
        }

        public ICommand RemoveProfileCommand
        {
            get { return _removeProfileCommand; }
        }

        public ICommand ModifyProfileCommand
        {
            get { return _modifyProfileCommand; }
        }

        public ICommand MoveUpProfileCommand
        {
            get { return _moveUpProfileCommand; }
        }

        public ICommand MoveDownProfileCommand
        {
            get { return _moveDownProfileCommand; }
        }

        public ObservableCollection<KeyCombo> PressedKeys
        {
            get { return _pressedKeys; }
        }

        public void Load()
        {
            var app = (App)App.Current;
            var generalSettings = app.Settings.GeneralSettings;
            if (generalSettings == null)
                generalSettings = new GeneralSettings();
            _profiles.Clear();
            var profiles = generalSettings.Profiles;
            foreach (var profileSettings in profiles)
            {
                var profile = _profileFactory.Create(profileSettings.Name);
                profile.IsEnabled = profileSettings.IsEnabled;
                _profiles.Add(profile);
                var keyMappings = profileSettings.KeyMappings;
                foreach (var keyMappingSettings in keyMappings)
                {
                    var sourceKeyCombos = KeyComboSeriesEncoder.Parse(keyMappingSettings.SourceKeyCombos);
                    var targetKeyCombos = KeyComboSeriesEncoder.Parse(keyMappingSettings.TargetKeyCombos);
                    var keyMapping = new KeyMapping(sourceKeyCombos, targetKeyCombos);
                    var keyMappingViewModel = new KeyMappingViewModel(keyMapping);
                    profile.KeyMappings.Add(keyMappingViewModel);
                }
            }

            SelectedProfile = _profiles.FirstOrDefault();
            ReactToKeyPresses();
        }

        public void Save()
        {
            var app = (App)App.Current;
            var generalSettings = app.Settings.GeneralSettings;
            if (generalSettings == null)
                generalSettings = new GeneralSettings();
            var profiles = new List<ProfileSettings>();
            foreach (var profile in _profiles)
            {
                var profileSettings = new ProfileSettings();
                profileSettings.Name = profile.Name;
                profileSettings.IsEnabled = profile.IsEnabled;
                var keyMappings = new List<KeyMappingSettings>();
                foreach (var keyMappingViewModel in profile.KeyMappings)
                {
                    var keyMapping = keyMappingViewModel.GetKeyMapping();
                    var keyMappingSettings = new KeyMappingSettings();
                    keyMappingSettings.SourceKeyCombos = KeyComboSeriesEncoder.Encode(keyMapping.Source);
                    keyMappingSettings.TargetKeyCombos = KeyComboSeriesEncoder.Encode(keyMapping.Target);
                    keyMappings.Add(keyMappingSettings);
                }
                profileSettings.KeyMappings = keyMappings;
                profiles.Add(profileSettings);
            }
            generalSettings.Profiles = profiles;
            app.Settings.GeneralSettings = generalSettings;
        }

        public void CreateProfile()
        {
            var profile = _profileFactory.Create("New Profile");
            if (profile == null)
                return;
            if (_profileDialogService.EditProfile(profile))
                _profiles.Add(profile);
        }

        public void ModifyProfile(ProfileViewModel? profile)
        {
            if (profile == null)
                return;
            _profileDialogService.EditProfile(profile);
        }

        public void RemoveProfile(ProfileViewModel? profile)
        {
            if (profile == null)
                return;
            var result = _profileDialogService.RemoveProfile();
            if (result == true)
                _profiles.Remove(profile);
        }

        public void MoveDownProfile(ProfileViewModel? profile)
        {
            if (profile == null)
                return;
            var index = _profiles.IndexOf(profile);
            if (index >= _profiles.Count - 1)
                return;
            var selectedProfile = SelectedProfile;
            var nextProfile = _profiles[index + 1];
            _profiles[index] = nextProfile;
            _profiles[index + 1] = profile;
            SelectedProfile = selectedProfile;
        }

        public void MoveUpProfile(ProfileViewModel? profile)
        {
            if (profile == null)
                return;
            var index = _profiles.IndexOf(profile);
            if (index <= 0)
                return;
            var selectedProfile = SelectedProfile;
            var prevProfile = _profiles[index - 1];
            _profiles[index] = prevProfile;
            _profiles[index - 1] = profile;
            SelectedProfile = selectedProfile;
        }

        private void ReactToKeyPresses()
        {
            _windowsHook.KeysPressed += WindowsHook_KeyPressed;
        }

        private void StopReactingToKeyPresses()
        {
            _windowsHook.KeysPressed -= WindowsHook_KeyPressed;
        }

        private void WindowsHook_KeyPressed(object? sender, KeyComboEventArgs e)
        {
            var matched = false;
            var completed = false;
            var matchIndex = _pressedKeys.Count;
            var targetKeyCombos = new ObservableCollection<KeyCombo>();

            foreach (var profile in _profiles)
            {
                if (!profile.IsEnabled)
                    continue;

                var keyMappings = profile.KeyMappings;
                foreach (var keyMapping in keyMappings)
                {
                    var sourceKeyCombos = keyMapping.Source.KeyCombos;
                    if (sourceKeyCombos.Count > matchIndex && sourceKeyCombos[matchIndex].Equals(e.KeyCombo))
                    {
                        matched = true;
                        if (sourceKeyCombos.Count == matchIndex + 1)
                        {
                            targetKeyCombos = keyMapping.Target.KeyCombos;
                            completed = true;
                            break;
                        }
                    }
                }
            }
            // if any combo completed, simulate its key presses
            if (completed)
                SimulateKeyPresses(targetKeyCombos);
            // if no combo matched, discard what was previously pressed, or if the combo is complete
            if (!matched || completed)
                _pressedKeys.Clear();
            // if the combo matched, but not completed, add it to the list of currently pressed keys
            else if (matched)
                _pressedKeys.Add(e.KeyCombo);
            // if anything is matched - mark the event as handled, to prevent further processing in the pipeline
            if (matched)
                e.Handled = true;
        }

        private static void SimulateKeyPresses(IEnumerable<KeyCombo> keyCombos)
        {
            foreach (var keyCombo in keyCombos)
                //WindowsKeyPresserViaKeyboardEvent.SimulateKeyPress(keyCombo.ModifierKeys, keyCombo.ActionKey);
                //WindowsKeyPresserViaSendInput.SimulateKeyPress(keyCombo.ModifierKeys, keyCombo.ActionKey);
                //WindowsKeyPresserViaMessage.SimulateKeyPress(keyCombo.ModifierKeys, keyCombo.ActionKey);
                WindowsKeyPresserViaScanCode.SimulateKeyPress(keyCombo.ModifierKeys, keyCombo.ActionKey);
        }

        public void Dispose()
        {
            StopReactingToKeyPresses();
            _windowsHook.Dispose();
        }
    }
}
