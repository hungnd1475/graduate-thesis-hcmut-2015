using HungND.WPF.Controls;
using System;
using System.Globalization;
using System.Windows.Data;

namespace EMRCorefResol.TestingGUI.Converters
{
    class NotificationTypeToColor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var v = value as NotificationType?;
            if (v.HasValue)
            {
                var t = v.Value;
                switch (t)
                {
                    case NotificationType.Error:
                        return IconColors.Negative;
                    case NotificationType.Information:
                        return IconColors.Information;
                    case NotificationType.Warning:
                        return IconColors.Warning;
                }
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
