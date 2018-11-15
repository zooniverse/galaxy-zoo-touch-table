using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace GalaxyZooTouchTable.Converters
{
    public class BoundMarginConverter : IValueConverter
    {
        private double GetMarginPartValue(Double size, string defaultValue, IDictionary<string, string> instructions)
        {
            string sourceValue;
            string value = instructions.TryGetValue(defaultValue, out sourceValue) ? sourceValue : defaultValue;

            if (double.TryParse(value, out double numericalValue))
            {
                return numericalValue * size;
            }
            return 0;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Double inputMargin = (Double)value;
            IDictionary<string, string> instructions = (parameter as string).Split()
                                                                            .Select(s => s.Split('='))
                                                                            .ToDictionary(t => t[0].ToLowerInvariant(), t => t[1].ToLowerInvariant());

            return new Thickness
            {
                Left = GetMarginPartValue(inputMargin, "left", instructions),
                Top = GetMarginPartValue(inputMargin, "top", instructions),
                Right = GetMarginPartValue(inputMargin, "right", instructions),
                Bottom = GetMarginPartValue(inputMargin, "bottom", instructions)
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
