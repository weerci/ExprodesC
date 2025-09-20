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

        ToggleExpandCommand = ReactiveCommand.Create(fff);
    }

    private void fff()
    {
        IsExpanded = !IsExpanded;
    }

    #region Properties

    public ER Er { get; }

    public bool ShowReverse =>  !string.IsNullOrEmpty(Er.RevFormula);
    public bool ShowDirect =>  !string.IsNullOrEmpty(Er.DirFormula);

    [Reactive] public bool IsExpanded { get; set; }

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
