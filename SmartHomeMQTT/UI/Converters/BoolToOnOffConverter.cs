using System;
using System.Globalization;
using System.Windows.Data;

namespace SmartHomeMQTT.UI.Converters
{
    /// <summary>
    /// Converts a bool to "On" for true and "Off" for false
    /// </summary>
    public class BoolToOnOffConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool v = (bool)value;
            return v ? "On" : "Off";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string s = (string)value;
            return s == "On";
        }
    }
}
