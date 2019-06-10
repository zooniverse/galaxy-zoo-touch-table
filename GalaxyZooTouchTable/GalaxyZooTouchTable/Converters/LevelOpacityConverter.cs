using System;
using System.Globalization;
using System.Windows.Data;

namespace GalaxyZooTouchTable.Converters
{
    /// <summary>
    /// This converter takes a parameter and returns the correct block opacity for the Leveler
    /// </summary>
    public class LevelOpacityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double RemainingToUpgrade = System.Convert.ToDouble(value);
            double MinValue = System.Convert.ToDouble(parameter);
            if (RemainingToUpgrade <= MinValue)
            {
                return 1.0;
            }
            return 0.5;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
