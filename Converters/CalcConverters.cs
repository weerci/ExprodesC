using Avalonia.Controls;
using Avalonia.Data.Converters;
using Calc.Calculation;
using DynamicData.Binding;
using ExprodesC.Models;
using ExprodesC.Views.Controls;
using FluentAvalonia.UI.Controls;
using Func;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using static ExprodesC.Converters.MathConverters;

namespace ExprodesC.Converters;

public static class CalcConverters
{
    public static EnabledCalcResearchConverter EnabledCalc => new();
    public static CheckedCalcMethodConverter EnabledMethod => new();
    public static VisibleCalcMethodConverter VisibleMethod => new();

    public static IValueConverter CollumnFormula { get; } = new FuncValueConverter<string, string>(method =>
    {
        if (method != null && TypeCalcEx.GetHashResearch(method) is Ex<CaseResearch> ecr  && ecr.IsSuccess)
            return ecr.Value!.CurrMethod switch
            {
                MethodCalc.LR => Lang.Resources.gc_direct_hypothesis,
                _ => Lang.Resources.gc_formula
            };

        return Lang.Resources.gc_formula;
    });

    public static IValueConverter CollumnProbability { get; } = new FuncValueConverter<string, string>(method =>
    {
        if (method != null && TypeCalcEx.GetHashResearch(method) is Ex<CaseResearch> ecr && ecr.IsSuccess)
            return ecr.Value!.CurrMethod switch
            {
                MethodCalc.LR => Lang.Resources.gc_probability_dir,
                _ => Lang.Resources.gc_probability
            };

        return Lang.Resources.gc_probability;
    });


    public class EnabledCalcResearchConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is int count && parameter is string param)
            {
                if (TypeCalcEx.CountGenotypeByNameMethod.TryGetValue(param, out int dir_count))
                    return count >= dir_count;
            }

            return false;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class CheckedCalcMethodConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is string research && !string.IsNullOrEmpty(research) && parameter is string method && !string.IsNullOrEmpty(method))
                return TypeCalcEx.GetHashMethod(research).Map(b => b.ToString() == method).GetOrElse(false);

            return false;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class VisibleCalcMethodConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is string research && !string.IsNullOrEmpty(research) && parameter is string method)
                return TypeCalcEx.GetHashResearch(research).Map(cs => cs.Methods.Select(m => m.ToString()).Contains(method)).GetOrElse(false);

            return false;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
