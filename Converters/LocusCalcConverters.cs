using Avalonia;
using Avalonia.Data.Converters;
using Avalonia.Media;
using Avalonia.Styling;
using System;
using System.Globalization;

namespace ExprodesC.Converters;

public class LocusCalcBackgroundConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        // If calculated -> transparent
        if (!(value is bool isCalc) || isCalc)
            return new SolidColorBrush(Colors.Transparent);

        // Not calculated -> theme-aware background
        var app = Application.Current;
        var theme = app?.RequestedThemeVariant;

        if (theme != null && theme.Equals(ThemeVariant.Dark))
            return new SolidColorBrush(Color.Parse("#2B1414")); // subtle dark red background

        // Light or default
        return new SolidColorBrush(Colors.MistyRose);
    }
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}

public class LocusCalcForegroundConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var app = Application.Current;
        var theme = app?.RequestedThemeVariant;
        // If calculated -> default foreground
        if (!(value is bool isCalc) || isCalc)
        {
            if (theme != null && theme.Equals(ThemeVariant.Dark))
                return new SolidColorBrush(Color.Parse("#48B748")); // для темной темы
            return new SolidColorBrush(Colors.Green); // для светлой
        }

        if (theme != null && theme.Equals(ThemeVariant.Dark))
            return new SolidColorBrush(Color.Parse("#FFB399")); // lighter warning color on dark theme

        // Light or default
        return new SolidColorBrush(Colors.OrangeRed);
    }
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}

public class LocusCalcIconVisibilityConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is bool isCalc && !isCalc;
    }
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}
