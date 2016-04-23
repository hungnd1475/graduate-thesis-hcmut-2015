using HungND.WPF.Controls;
using System;
using System.Globalization;
using System.Windows.Data;

namespace EMRCorefResol.TestingGUI.Converters
{
    class NotificationTypeToPath : IValueConverter
    {
        private static readonly PathIconSource INFO_ICON = new PathIconSource()
        {
            Data = PathData.InformationCircle
        };

        private static readonly PathIconSource WARNING_ICON = new PathIconSource()
        {
            Data = PathData.WarningMessage
        };

        private static readonly PathIconSource ERROR_ICON = new PathIconSource()
        {
            Data = PathData.Cancel1
        };

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var v = value as NotificationType?;
            if (v.HasValue)
            {
                var t = v.Value;
                switch (t)
                {
                    case NotificationType.Error:
                        return ERROR_ICON;
                    case NotificationType.Information:
                        return INFO_ICON;
                    case NotificationType.Warning:
                        return WARNING_ICON;
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
