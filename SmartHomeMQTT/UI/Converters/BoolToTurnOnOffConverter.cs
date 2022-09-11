﻿using System;
using System.Globalization;
using System.Windows.Data;

namespace SmartHomeMQTT.UI.Converters
{
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