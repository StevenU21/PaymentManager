using System;
using System.Globalization;
using Microsoft.Maui.Controls;
using PaymentManager.Models;

namespace PaymentManager.Converters
{
    public class UserPaymentPlanDisplayConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is UserPaymentPlan upp && upp.User != null && upp.PaymentPlan != null)
            {
                return $"{upp.User.Name}-{upp.PaymentPlan.Name}";
            }
            return string.Empty;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
