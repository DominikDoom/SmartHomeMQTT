using SmartHomeMQTT.UI.Converters;
using System.Globalization;
using Xunit;

namespace SmartHomeMqttTests
{
    /// <summary>
    /// Tests various converters used in the UI DataBinding process
    /// </summary>
    public class ConverterTests
    {
        [Fact(DisplayName = "Bool to On / Off")]
        public void TestBoolToOnOff()
        {
            BoolToOnOffConverter c = new();
            Assert.Equal("On", c.Convert(true, typeof(string), null, CultureInfo.InvariantCulture));
            Assert.Equal(true, c.ConvertBack("On", typeof(bool), null, CultureInfo.InvariantCulture));
            Assert.Equal("Off", c.Convert(false, typeof(string), null, CultureInfo.InvariantCulture));
            Assert.Equal(false, c.ConvertBack("Off", typeof(bool), null, CultureInfo.InvariantCulture));
        }

        // Note: Inverse converter for buttons
        [Fact(DisplayName = "Bool to Open / Close")]
        public void TestBoolToOpenClose()
        {
            BoolToOpenCloseConverter c = new();
            Assert.Equal("Close", c.Convert(true, typeof(string), null, CultureInfo.InvariantCulture));
            Assert.Equal(true, c.ConvertBack("Close", typeof(bool), null, CultureInfo.InvariantCulture));
            Assert.Equal("Open", c.Convert(false, typeof(string), null, CultureInfo.InvariantCulture));
            Assert.Equal(false, c.ConvertBack("Open", typeof(bool), null, CultureInfo.InvariantCulture));
        }

        [Fact(DisplayName = "Bool to Open / Closed")]
        public void TestBoolToOpenClosed()
        {
            BoolToOpenClosedConverter c = new();
            Assert.Equal("Open", c.Convert(true, typeof(string), null, CultureInfo.InvariantCulture));
            Assert.Equal(true, c.ConvertBack("Open", typeof(bool), null, CultureInfo.InvariantCulture));
            Assert.Equal("Closed", c.Convert(false, typeof(string), null, CultureInfo.InvariantCulture));
            Assert.Equal(false, c.ConvertBack("Closed", typeof(bool), null, CultureInfo.InvariantCulture));
        }

        // Note: Inverse converter for buttons
        [Fact(DisplayName = "Bool to Turn On / Off")]
        public void TestBoolToTurnOnOff()
        {
            BoolToTurnOnOffConverter c = new();
            Assert.Equal("Turn Off", c.Convert(true, typeof(string), null, CultureInfo.InvariantCulture));
            Assert.Equal(true, c.ConvertBack("Turn Off", typeof(bool), null, CultureInfo.InvariantCulture));
            Assert.Equal("Turn On", c.Convert(false, typeof(string), null, CultureInfo.InvariantCulture));
            Assert.Equal(false, c.ConvertBack("Turn On", typeof(bool), null, CultureInfo.InvariantCulture));
        }
    }
}
