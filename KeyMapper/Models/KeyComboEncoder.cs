using System.Windows.Input;

namespace KeyMapper.Models
{
    public static class KeyComboEncoder
    {
        public static KeyCombo Parse(string keyString)
        {
            var parts = keyString.Split('+');
            ModifierKeys modifierKeys = ModifierKeys.None;
            Key actionKey = Key.None;
            for (int i = 0; i < parts.Length; ++i)
            {
                var modifierKey = ParseModifierKey(parts[i]);
                if (modifierKey != ModifierKeys.None)
                    modifierKeys |= modifierKey;
                else actionKey = StringToKey(parts[i]);
            }
            var keyCombo = new KeyCombo(modifierKeys, actionKey);
            return keyCombo;
        }

        private static ModifierKeys ParseModifierKey(string modifierKeyString)
        {
            var normalized = modifierKeyString.ToLowerInvariant().Trim();
            if (normalized == "control" || normalized == "ctrl")
                return ModifierKeys.Control;
            if (normalized == "shift")
                return ModifierKeys.Shift;
            if (normalized == "alt")
                return ModifierKeys.Alt;
            return ModifierKeys.None;
        }

        public static string ToString(this KeyCombo keyCombo)
        {
            var modifierKeys = keyCombo.ModifierKeys;
            var actionKey = keyCombo.ActionKey;
            var parts = new List<string>();
            if (modifierKeys.HasFlag(ModifierKeys.Control))
                parts.Add("Control");
            if (modifierKeys.HasFlag(ModifierKeys.Shift))
                parts.Add("Shift");
            if (modifierKeys.HasFlag(ModifierKeys.Alt))
                parts.Add("Alt");
            parts.Add(KeyToString(actionKey));
            return string.Join(" + ", parts);
        }

        private static Key StringToKey(string actionKeyString)
        {
            var normalized = actionKeyString.ToLowerInvariant().Trim();
            var letterKey = StringToLetter(normalized);
            if (letterKey.HasValue)
                return letterKey.Value;
            var numberKey = StringToNumber(normalized);
            if (numberKey.HasValue)
                return numberKey.Value;
            var specialKey = StringToSpecial(normalized);
            if (specialKey.HasValue)
                return specialKey.Value;
            return Key.None;
        }

        private static string KeyToString(Key key)
        {
            var letterString = LetterToString(key);
            if (letterString != null)
                return letterString;
            var numberString = NumberToString(key);
            if (numberString != null)
                return numberString;
            var specialString = SpecialToString(key);
            if (specialString != null)
                return specialString;
            return "";
        }

        private static Key? StringToSpecial(string specialString)
        {
            if (Enum.TryParse<Key>(specialString, true, out var key))
                return key;
            return null;
        }

        private static string? SpecialToString(Key key)
        {
            return key.ToString();
        }

        private static Key? StringToLetter(string letterString)
        {
            if (letterString.Length == 1 && char.IsLetter(letterString[0]))
            {
                char letter = char.ToUpper(letterString[0]);
                if (letter >= 'A' && letter <= 'Z')
                    return Key.A + (letter - 'A');
            }
            return null;
        }

        private static string? LetterToString(Key key)
        {
            if (key >= Key.A && key <= Key.Z)
                return ((char)('A' + (key - Key.A))).ToString();
            return null;
        }

        private static Key? StringToNumber(string numberString)
        {
            if (numberString.Length == 1 && char.IsDigit(numberString[0]))
            {
                int digit = numberString[0] - '0';
                if (digit >= 0 && digit <= 9)
                    return Key.NumPad0 + digit;
            }
            return null;
        }

        private static string? NumberToString(Key key)
        {
            if (key >= Key.NumPad0 && key <= Key.NumPad9)
                return ((int)(key - Key.NumPad0)).ToString();
            if (key >= Key.D0 && key <= Key.D9)
                return ((int)(key - Key.D0)).ToString();
            return null;
        }
    }
}
