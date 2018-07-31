using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace GalaxyZooTouchTable
{
    public class UserConsoleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double size = System.Convert.ToDouble(value) * System.Convert.ToDouble(parameter, CultureInfo.InvariantCulture);
            Console.WriteLine(size);
            Console.WriteLine("Done");
            return size;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
