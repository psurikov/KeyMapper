using KeyMapper.ViewModels;
using KeyMapper.Views;
using System.Windows;

namespace KeyMapper.Services
{
    public interface IProfileDialogService
    {
        bool EditProfile(ProfileViewModel profile);
        bool? RemoveProfile();
    }

    public class ProfileDialogService : IProfileDialogService
    {
        public bool EditProfile(ProfileViewModel profile)
        {
            var dialog = new ProfileDialog();
            dialog.ProfileName = profile.Name;
            dialog.Owner = App.Current.MainWindow;
            var success = (dialog.ShowDialog() == true);
            if (success)
                profile.Name = dialog.ProfileName;
            return success;
        }

        public bool? RemoveProfile()
        {
            var result = System.Windows.MessageBox.Show("Are you sure you want to remove this profile?", "Confirm Removal", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
                return true;
            return false;
        }
    }
}
