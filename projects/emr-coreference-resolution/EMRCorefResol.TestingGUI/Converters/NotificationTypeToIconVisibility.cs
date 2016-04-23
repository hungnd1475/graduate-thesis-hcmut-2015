using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace EMRCorefResol.TestingGUI.Converters
{
    class NotificationTypeToIconVisibility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var v = value as NotificationType?;
            if (v.HasValue)
            {
                var t = v.Value;
                return t == NotificationType.None ? Visibility.Collapsed : Visibility.Visible;
            }

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
