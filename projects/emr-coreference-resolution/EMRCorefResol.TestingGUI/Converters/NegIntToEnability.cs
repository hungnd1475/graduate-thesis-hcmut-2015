using System;
using System.Globalization;
using System.Windows.Data;

namespace EMRCorefResol.TestingGUI.Converters
{
    class NegIntToEnability : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var intValue = value as int?;
            return intValue.HasValue ? intValue.Value >= 0 : false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
