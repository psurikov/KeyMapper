namespace KeyMapper.Models
{
    public class KeyComboSeries : IEquatable<KeyComboSeries>
    {
        private readonly KeyCombo[] _keyCombos;

        public KeyComboSeries(IEnumerable<KeyCombo> keyCombos)
        {
            this._keyCombos = [.. keyCombos];
        }

        public IEnumerable<KeyCombo> KeyCombos
        {
            get { return this._keyCombos; }
        }

        public int Count
        {
            get { return _keyCombos.Length; }
        }

        public override string ToString()
        {
            return KeyComboSeriesEncoder.Encode(this);
        }

        #region IEquatable<T>

        public static bool Equal(KeyComboSeries a, KeyComboSeries b)
        {
            var count1 = a.Count;
            var count2 = b.Count;
            if (count1 != count2)
                return false;
            for (var i = 0; i < count1; ++i)
                if (!a._keyCombos[i].Equals(b._keyCombos[i]))
                    return false;
            return true;
        }

        public bool Equals(KeyComboSeries? other)
        {
            if (other == null)
                return false;
            return Equal(other, this);
        }

        #endregion
    }
}