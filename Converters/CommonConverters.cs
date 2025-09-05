using Avalonia.Data.Converters;
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

    public static PopulationByDefaultConverter PopulationByDefault => new();


    public static IValueConverter CollapseExpand { get; } =
       new FuncValueConverter<bool, object>(isExpanded =>
           isExpanded
               ? new SymbolIcon { Symbol = Symbol.ChevronUp }
               : new SymbolIcon { Symbol = Symbol.ChevronDown });

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

    public static LocusCalcBackgroundConverter LocusCalcBackground => new();
    public static LocusCalcForegroundConverter LocusCalcForeground => new();
    public static LocusCalcIconVisibilityConverter LocusCalcIconVisibility => new();
}
