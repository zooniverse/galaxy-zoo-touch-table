using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace GalaxyZooTouchTable
{
    /// <summary>
    /// When binding, if an image source is not yet available (null), this converts the object to an acceptable unset value so the UI doesn't throw an exception
    /// </summary>
    public class HidePanelConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return System.Convert.ToDouble(value) * System.Convert.ToDouble(parameter);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}