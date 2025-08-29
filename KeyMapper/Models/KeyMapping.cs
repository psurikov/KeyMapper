namespace KeyMapper.Models
{
    public class KeyMapping
    {
        private readonly KeyComboSeries _source;
        private readonly KeyComboSeries _target;

        public KeyMapping(KeyComboSeries source, KeyComboSeries target)
        {
            _source = source;
            _target = target;
        }

        public KeyComboSeries Source
        {
            get { return _source; }
        }

        public KeyComboSeries Target
        {
            get { return _target; }
        }
    }
}
