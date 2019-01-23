using System;
using System.Linq;
using System.Windows.Data;

namespace GalaxyZooTouchTable
{
    /// <summary>
    /// This converter is designed to move an element a percentage of the parameter given
    /// </summary>
    public class ProgressWidthConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values.Length >= 3)
            {
                const double MINIMUM_PERCENT = 0.06;
                double Width = System.Convert.ToDouble(values[0]);
                double Value = System.Convert.ToDouble(values[1]);
                double Maximum = System.Convert.ToDouble(values[2]);

                double Percent = Value / Maximum;
                Percent = Percent > 0 ? Math.Max(Percent, MINIMUM_PERCENT) : Percent;
                return Percent * Width;
            }
            return null;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}