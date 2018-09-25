using System;
using System.Globalization;
using System.Windows.Data;

namespace GalaxyZooTouchTable
{
    /// <summary>
    /// This converter is designed to move an element a percentage of the parameter given
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