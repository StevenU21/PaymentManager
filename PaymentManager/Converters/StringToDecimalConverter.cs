using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace PaymentManager.Converters
{
    public class StringToDecimalConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
            => value?.ToString() ?? "";

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
            => decimal.TryParse(value?.ToString(), out var result) ? result : 0m;
    }
}