using System;
using System.Globalization;
using System.Windows.Data;

namespace SmartHomeMQTT.UI.Converters
{
    /// <summary>
    /// Converts a bool to "Turn off" for true and "Turn on" for false
    /// Used for button text as it is the inverse of the actual state.
    /// </summary>
    public class BoolToTurnOnOffConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool v = (bool)value;
            return v ? "Turn Off" : "Turn On";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string s = (string)value;
            return s == "Turn Off";
        }
    }
}
