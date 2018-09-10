using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace GalaxyZooTouchTable
{
    /// <summary>
    /// When binding, if an image source is not yet available (null), this converts the object to an acceptable unset value so the UI doesn't throw an exception
    /// </summary>
    public class NullImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return DependencyProperty.UnsetValue;
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
