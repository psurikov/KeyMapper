using KeyMapper.ViewModels;
using KeyMapper.Views;

namespace KeyMapper.Services
{
    public interface IKeyMappingDialogService
    {
        bool EditKeyMapping(KeyMappingViewModel keyMapping);
    }

    public class KeyMappingDialogService : IKeyMappingDialogService
    {
        public bool EditKeyMapping(KeyMappingViewModel keyMapping)
        {
            var dialog = new KeyMappingDialog(keyMapping);
            dialog.Owner = App.Current.MainWindow;
            return dialog.ShowDialog() == true;
        }
    }
}