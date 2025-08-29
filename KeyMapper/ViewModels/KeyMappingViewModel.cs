using CommunityToolkit.Mvvm.ComponentModel;
using KeyMapper.Models;

namespace KeyMapper.ViewModels
{
    public class KeyMappingViewModel : ObservableObject
    {
        private KeyComboSeriesViewModel _source;
        private KeyComboSeriesViewModel _target;

        public KeyMappingViewModel()
        {
            _source = new KeyComboSeriesViewModel();
            _target = new KeyComboSeriesViewModel();
        }

        public KeyMappingViewModel(KeyMapping keyMapping)
        {
            _source = new KeyComboSeriesViewModel(keyMapping.Source);
            _target = new KeyComboSeriesViewModel(keyMapping.Target);
        }

        public KeyComboSeriesViewModel Source
        {
            get { return _source; }
            set { SetProperty(ref _source, value); }
        }

        public KeyComboSeriesViewModel Target
        {
            get { return _target; }
            set { SetProperty(ref _target, value); }
        }

        public void Copy(KeyMappingViewModel original)
        {
            Source = new KeyComboSeriesViewModel(original.Source.GetKeyComboSeries());
            Target = new KeyComboSeriesViewModel(original.Target.GetKeyComboSeries());
        }

        public KeyMapping GetKeyMapping()
        {
            var source = new KeyComboSeries(_source.KeyCombos);
            var target = new KeyComboSeries(_target.KeyCombos);
            var mapping = new KeyMapping(source, target);
            return mapping;
        }

        public override string ToString()
        {
            return Source.ToString() + " -> " + Target.ToString();
        }
    }
}
