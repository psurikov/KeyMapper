using System.Windows.Input;

namespace KeyMapper.Models
{
    public static class KeyComboSeriesEncoder
    {
        public static string Encode(KeyComboSeries keyComboSeries)
        {
            return string.Join(",", keyComboSeries.KeyCombos.Select(k => k.ToString()));
        }

        public static KeyComboSeries Parse(string keyComboSeriesString)
        {
            var keyCombos = new List<KeyCombo>();
            var parts = keyComboSeriesString.Split(',');
            foreach (var part in parts)
            {
                var trimmed = part.Trim();
                var keyCombo = KeyComboEncoder.Parse(trimmed);
                if (keyCombo.ActionKey != Key.None)
                keyCombos.Add(keyCombo);
            }
            return new KeyComboSeries(keyCombos);
        }
    }
}
