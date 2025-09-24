using Avalonia.Controls;
using Calc.Calculation;
using Calc.Models;
using Func;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ExprodesC.Wrappers;

public partial class ErWR : ReactiveObject
{

    public ErWR(ER er)
    {
        Er = er;

        ToggleExpandCommand = ReactiveCommand.Create(() =>
        {
            IsExpanded = !IsExpanded;
        });
    }

    #region Properties

    public ER Er { get; }


    [Reactive] public bool IsExpanded { get; set; }
    public bool IsReverse => !string.IsNullOrEmpty(Er.RevFormula);
    public string DirFormulaView => $"{Lang.Resources.cap_formula} {Er.DirFormula}";
    public string RevFormulaView => $"{Lang.Resources.cap_formula} {Er.RevFormula}";
    public string DirExpressionView => $"{Lang.Resources.cap_calc} {Er.DirExpression.Value} = {Er.DirResult:E4}";
    public string RevExpressionView => $"{Lang.Resources.cap_calc} {Er.RevExpression.Value} = {Er.RevResult:E4}";


    //TODO CalcPage Сделать отображение знаков с запятой в соответствии с настройками

    /// <summary>
    /// Sorted unique alleles from all genomes in this ER
    /// </summary>
    public IEnumerable<Allele> SortedUniqueAlleles =>
        Er.Genomes?.Where(g => g.IsSuccess).SelectMany(g => g.Value!.Alleles).Distinct().OrderBy(a => a.Ord) ?? Enumerable.Empty<Allele>();

    #endregion

    #region Command

    public ICommand ToggleExpandCommand { get; }


    #endregion
}
