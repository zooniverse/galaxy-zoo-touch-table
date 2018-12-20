using System;
using System.Globalization;
using System.Windows.Data;

namespace GalaxyZooTouchTable.Converters
{
    public class FontSizeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var originalFontSize = (double)value;
            var ratio = System.Convert.ToDouble(parameter);
            double alteredFontSize = originalFontSize * ratio;

            return alteredFontSize;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
