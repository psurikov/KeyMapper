using CommunityToolkit.Mvvm.ComponentModel;
using KeyMapper.Models;
using System.Collections.ObjectModel;

namespace KeyMapper.ViewModels
{
    public class KeyComboSeriesViewModel : ObservableObject
    {
        private ObservableCollection<KeyCombo> _keyCombos;

        public KeyComboSeriesViewModel()
        {
            _keyCombos = [];
        }

        public KeyComboSeriesViewModel(KeyComboSeries keyComboSeries)
        {
            _keyCombos = [.. keyComboSeries.KeyCombos];
        }

        public ObservableCollection<KeyCombo> KeyCombos
        {
            get { return _keyCombos; }
            set { SetProperty(ref _keyCombos, value); }
        }

        public KeyComboSeries GetKeyComboSeries()
        {
            return new KeyComboSeries(KeyCombos);
        }

        public override string ToString()
        {
            return string.Join(",", _keyCombos);
        }
    }
}
