using Avalonia;
using Avalonia.Data.Converters;
using Avalonia.Media;
using Avalonia.Styling;
using Calc.Calculation;
using Calc.Models;
using ExprodesC.Imp;
using FluentAvalonia.UI.Controls;
using Func.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ExprodesC.Converters.CalcConverters;

namespace ExprodesC.Converters;

public static class CommonConverters
{
    // Header
    public static PopulationByDefaultConverter PopulationByDefault => new();
    public static BoolToVisibilityConverter BoolToVisibility => new();
    public static IntToVisibilityConverter IntToVisibility => new();

    public static LocusCalcBackgroundConverter LocusCalcBackground => new();
    public static LocusCalcForegroundConverter LocusCalcForeground => new();
    public static LocusCalcIconVisibilityConverter LocusCalcIconVisibility => new();


    // Implementations
    public class PopulationByDefaultConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is Population population)
            {
                var settings = App.Services.GetRequiredService<ISettingsProvider<ExSettingData>>();
                if (settings != null && settings.Value != null && settings.Value.CurrentPopulation.Id == population.Id)
                    return $"{population.Name} {Lang.Resources.msg_by_default}";
                else
                    return population.Name;
            }

            return value;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return value is bool b && b ? true : false;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class IntToVisibilityConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return value is int i && i != 0 ? true : false;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

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


    public static IValueConverter CollapseExpand { get; } =
      new FuncValueConverter<bool, object>(isExpanded =>
          isExpanded
              ? new SymbolIcon { Symbol = Symbol.ChevronUp }
              : new SymbolIcon { Symbol = Symbol.ChevronDown });
    
}
