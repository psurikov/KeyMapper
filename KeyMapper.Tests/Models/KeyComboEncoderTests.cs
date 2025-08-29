using KeyMapper.Models;
using System.Windows.Input;

namespace KeyMapper.Tests.Models
{
    [TestClass]
    public sealed class KeyComboEncoderTests
    {
        [TestMethod]
        public void KeyCombo_Parse()
        {
            Assert.IsTrue(KeyComboEncoder.Parse("Control + A").Equals(new KeyCombo(ModifierKeys.Control, Key.A)));
            Assert.IsTrue(KeyComboEncoder.Parse("Tab").Equals(new KeyCombo(ModifierKeys.None, Key.Tab)));
            Assert.IsTrue(KeyComboEncoder.Parse("Escape").Equals(new KeyCombo(ModifierKeys.None, Key.Escape)));
            Assert.IsTrue(KeyComboEncoder.Parse("Control + Shift + C").Equals(new KeyCombo(ModifierKeys.Control | ModifierKeys.Shift, Key.C)));
            Assert.IsTrue(KeyComboEncoder.Parse("Control + Alt + Shift + Return").Equals(new KeyCombo(ModifierKeys.Control | ModifierKeys.Alt | ModifierKeys.Shift, Key.Return)));
        }

        [TestMethod]
        public void KeyCombo_ToString()
        {
            Assert.IsTrue(KeyComboEncoder.ToString(new KeyCombo(ModifierKeys.Control, Key.A)) == "Control + A");
            Assert.IsTrue(KeyComboEncoder.ToString(new KeyCombo(ModifierKeys.Control, Key.C)) == "Control + C");
            Assert.IsTrue(KeyComboEncoder.ToString(new KeyCombo(ModifierKeys.Shift, Key.Tab)) == "Shift + Tab");
        }
    }
}
