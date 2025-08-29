using System.Diagnostics.CodeAnalysis;
using System.Windows.Input;

namespace KeyMapper.Models
{
    /// <summary>
    /// Defines a combination of keys, that are pressed simultaneously.
    /// </summary>
    public readonly struct KeyCombo : IEquatable<KeyCombo>
    {
        private readonly ModifierKeys _modifierKeys;
        private readonly Key _actionKey;

        public KeyCombo(ModifierKeys modifierKeys, Key actionKey)
        {
            _modifierKeys = modifierKeys;
            _actionKey = actionKey;
        }

        public Key ActionKey
        {
            get { return _actionKey; }
        }

        public ModifierKeys ModifierKeys
        {
            get { return _modifierKeys; }
        }

        public override string ToString()
        {
            return KeyComboEncoder.ToString(this);
        }

        #region Equals and GetHashCode

        public bool Equals(KeyCombo other)
        {
            return (other.ModifierKeys == ModifierKeys) && (other.ActionKey == ActionKey);
        }

        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            if (obj == null)
                return false;
            if (obj is KeyCombo combo)
                return Equals(combo);
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ModifierKeys, ActionKey);
        }

        public static bool operator ==(KeyCombo left, KeyCombo right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(KeyCombo left, KeyCombo right)
        {
            return !(left == right);
        }

        #endregion
    }
}
