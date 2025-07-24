using Avalonia.Data.Converters;
using System.Globalization;

namespace ExprodesC.Converters;

public static class MathConverters
{
    public static DivideByConverter DivideByConv => new();

    public class DivideByConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            try
            {
                return (double?)value / System.Convert.ToDouble(parameter, CultureInfo.InvariantCulture);
            }
            catch (Exception)
            {
                return value;
            }
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
