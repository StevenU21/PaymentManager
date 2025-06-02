using System;
using System.Globalization;

namespace PaymentManager.Converters
{
    public class StringToIntConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return value?.ToString() ?? "";
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (int.TryParse(value as string, out int result))
                return result;
            return 0;
        }
    }
}