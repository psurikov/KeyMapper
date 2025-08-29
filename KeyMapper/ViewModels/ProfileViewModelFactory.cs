using KeyMapper.Services;

namespace KeyMapper.ViewModels
{
    public interface IProfileViewModelFactory
    {
        ProfileViewModel Create(string name);
    }

    public class ProfileViewModelFactory : IProfileViewModelFactory
    {
        private IKeyMappingDialogService _keyMappingDialogService;

        public ProfileViewModelFactory(IKeyMappingDialogService keyMappingDialogService)
        {
            _keyMappingDialogService = keyMappingDialogService;
        }

        public ProfileViewModel Create(string name)
        {
            return new ProfileViewModel(name, _keyMappingDialogService);
        }
    }
}