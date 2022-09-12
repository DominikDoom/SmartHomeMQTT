using System;
using System.Globalization;
using System.Windows.Data;

namespace SmartHomeMQTT.UI.Converters
{
    /// <summary>
    /// Converts a bool to "Close" for true and "Open" for false.
    /// Used for button text as it is the inverse of the actual state.
    /// </summary>
    public class BoolToOpenCloseConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool v = (bool)value;
            return v ? "Close" : "Open";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string s = (string)value;
            return s == "Close";
        }
    }
}
