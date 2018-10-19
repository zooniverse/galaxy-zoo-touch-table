using System;
using System.Globalization;
using System.Windows.Data;

namespace GalaxyZooTouchTable.Converters
{
    public class ObjectToTypeStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                Console.WriteLine(value.GetType().Name);
                return value.GetType().Name;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
